using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace IpicoReader.Ftp
{
    public class FtpUtil
    {
        public static bool inUse = false;
        public static double perc = 0;
        public static string name;

        public static int GetFileSize(string host, string user, string pass)
        {
            string ftpPath = host;
            int fileSize = 0;
            try
            {
                FtpWebRequest rq = (FtpWebRequest)WebRequest.Create(ftpPath);
                rq.Credentials = new NetworkCredential(user, pass);
                rq.Method = WebRequestMethods.Ftp.GetFileSize;
                fileSize = (int)rq.GetResponse().ContentLength;
            }
            catch
            {
            }
            return fileSize;
        }

        public delegate bool CriteriaHandle(string fLine);

        public static List<string> ReadTags(string host, string logname, CriteriaHandle handle=null)
        {
            List<string> fileContents = new List<string>();
            FtpWebRequest rq = (FtpWebRequest)FtpWebRequest.Create("ftp://" + host + "/" + logname);
            rq.Method = WebRequestMethods.Ftp.DownloadFile;
            rq.Credentials = new NetworkCredential("anonymous", "");
            Stream resp = rq.GetResponse().GetResponseStream(); //rq.GetRequestStream();
            StreamReader ftpReader = new StreamReader(resp);
            
            string fileLine=null;
            while((fileLine=ftpReader.ReadLine())!=null)
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
            ftpReader.Close();
            resp.Close();
            return fileContents;
        }

        public static List<ReaderRecord> ReadRecords(string host, string logname, CriteriaHandle handle = null)
        {
            List<string> fileContents = new List<string>();
            FtpWebRequest rq = (FtpWebRequest)FtpWebRequest.Create("ftp://" + host + "/" + logname);
            rq.Method = WebRequestMethods.Ftp.DownloadFile;
            rq.Credentials = new NetworkCredential("anonymous", "");
            Stream resp = rq.GetResponse().GetResponseStream(); //rq.GetRequestStream();
            StreamReader ftpReader = new StreamReader(resp);

            string fileLine = null;
            while ((fileLine = ftpReader.ReadLine()) != null)
            {
                if (handle != null)
                {
                    if (!handle(fileLine))
                    {
                        continue;
                    }
                }
                fileContents.Add(fileLine);
            }
            ftpReader.Close();
            resp.Close();

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

        public static void DownloadFileByName(string host, string filename, string local_filepath, DownloadFileProgressHandle handle=null)
        {
            int fileSize = 0;
            byte[] fileContents = null;
            int bytesReadable = 0;
            fileSize = GetFileSize("ftp://" + host + "/" + filename, "anonymous", "");
            fileContents = new Byte[fileSize];
            try
            {
                FtpWebRequest rq = (FtpWebRequest)WebRequest.Create("ftp://" + host + "/" + filename);
                rq.Method = WebRequestMethods.Ftp.DownloadFile;
                rq.Credentials = new NetworkCredential("anonymous", "");
                name = filename;
                FtpWebResponse resp = (FtpWebResponse)rq.GetResponse();
                Stream ftpReader = resp.GetResponseStream();
                FileStream fileWriter = File.Create(local_filepath);
                int perc = 0;
                int blockSum = 0;
                do
                {
                    bytesReadable = ftpReader.Read(fileContents, 0, fileContents.Length);
                    blockSum += bytesReadable;
                    if (fileSize > 0)
                    {
                        perc = ((blockSum / fileSize)) * 100;
                        if (handle != null)
                        {
                            handle(filename, perc);
                        }
                    }
                    fileWriter.Write(fileContents, 0, bytesReadable);
                } while (bytesReadable != 0);
                ftpReader.Close();
                resp.Close();
                fileWriter.Close();
                if (handle != null)
                {
                    handle(filename, 100);
                }
            }
            catch
            {
            }
        }
    }
}
