using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using CoreFtp;
using CoreFtp.Enum;

namespace SimuKit.Sports.IPICO.Ftp
{
    public class FtpUtil
    {
        public static bool inUse = false;
        public static double perc = 0;
        public static string name;

        public static async System.Threading.Tasks.Task<long> GetFileSizeAsync(string host, string filename, string user, string pass)
        {
            using (var ftpClient = new FtpClient(new FtpClientConfiguration
            {
                Host = host,
                Username = user,
                Password = pass,
                IgnoreCertificateErrors = true
            }))
            {
                await ftpClient.LoginAsync();
                long x = await ftpClient.GetFileSizeAsync(filename);
                return x;
            }
        }

        public delegate bool CriteriaHandle(string fLine);

        public static async System.Threading.Tasks.Task<List<string>> ReadTagsAsync(string host, string logname, CriteriaHandle handle = null)
        {
            string user = "anonymous";
            string pass = "";
            List<string> fileContents = new List<string>();
            using (var ftpClient = new FtpClient(new FtpClientConfiguration
            {
                Host = host,
                Username = user,
                Password = pass,
                IgnoreCertificateErrors = true
            }))
            {
                await ftpClient.LoginAsync();
                using (var ftpReadStream = await ftpClient.OpenFileReadStreamAsync(logname))
                {
                    using (TextReader ftpReader = new StreamReader(ftpReadStream))
                    {
                        string fileLine = null;
                        while ((fileLine = await ftpReader.ReadLineAsync()) != null)
                        {
                            string tag = ReaderDecoder.ExtractTagID(fileLine);
                            if (!fileContents.Contains(tag))
                            {
                                if (handle != null)
                                {
                                    if (!handle(fileLine))
                                    {
                                        continue;
                                    }
                                }
                                fileContents.Add(tag);
                            }
                        }
                    }
                        
                }
            }


            return fileContents;
        }

        public static async System.Threading.Tasks.Task<List<ReaderRecord>> ReadRecordsAsync(string host, string logname, CriteriaHandle handle = null)
        {
            List<string> fileContents = new List<string>();
            string user = "anonymous";
            string pass = "";
            using (var ftpClient = new FtpClient(new FtpClientConfiguration
            {
                Host = host,
                Username = user,
                Password = pass,
                IgnoreCertificateErrors = true
            }))
            {
                await ftpClient.LoginAsync();
                using (var ftpReadStream = await ftpClient.OpenFileReadStreamAsync(logname))
                {
                    using (TextReader ftpReader = new StreamReader(ftpReadStream))
                    {
                        string fileLine = null;
                        while ((fileLine = await ftpReader.ReadLineAsync()) != null)
                        {
                            string tag = ReaderDecoder.ExtractTagID(fileLine);
                            if (!fileContents.Contains(tag))
                            {
                                if (handle != null)
                                {
                                    if (!handle(fileLine))
                                    {
                                        continue;
                                    }
                                }
                                fileContents.Add(tag);
                            }
                        }
                    }

                }
            }



            List<ReaderRecord> records = new List<ReaderRecord>();
            foreach (string line in fileContents)
            {
                ReaderRecord rec = ReaderRecord.Parse(line);
                if (rec != null)
                {
                    records.Add(rec);
                }
            }
            return records;
        }

        public delegate void DownloadFileProgressHandle(string logname, int progress);

        public static async System.Threading.Tasks.Task DownloadFileByNameAsync(string host, string filename, string local_filepath, DownloadFileProgressHandle handle = null)
        {
            byte[] fileContents = null;
            int bytesReadable = 0;
            long fileSize = await GetFileSizeAsync(host, filename, "anonymous", "");

            string user = "anonymous";
            string pass = "";
            using (var ftpClient = new FtpClient(new FtpClientConfiguration
            {
                Host = host,
                Username = user,
                Password = pass,
                IgnoreCertificateErrors = true
            }))
            {
                await ftpClient.LoginAsync();
                using (var ftpReadStream = await ftpClient.OpenFileReadStreamAsync(filename))
                {
                    using (FileStream fileWriter = File.Create(local_filepath))
                    {
                        int perc = 0;
                        int blockSum = 0;
                        do
                        {
                            bytesReadable = ftpReadStream.Read(fileContents, 0, fileContents.Length);
                            blockSum += bytesReadable;
                            if (fileSize > 0)
                            {
                                perc = (int)(blockSum * 100 / fileSize);
                                if (handle != null)
                                {
                                    handle(filename, perc);
                                }
                            }
                            fileWriter.Write(fileContents, 0, bytesReadable);
                        } while (bytesReadable != 0);
                    }
                }

                if (handle != null)
                {
                    handle(filename, 100);
                }
            }

        }
    }
}
