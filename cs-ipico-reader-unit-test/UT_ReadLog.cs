using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IpicoReader.UT
{
    public class UT_ReadLog
    {
        public static void Run(string host)
        {
            string logname = ReaderLog.FS_LS;
            string logpath = Path.Combine("/tmp", logname);

            if (!File.Exists(logpath))
            {
                Console.WriteLine("Start downloading {0} ...", logname);

                try
                {
                    ReaderUtil.DownloadLog(host, logname, logpath, (ln, perc) =>
                    {
                        Console.WriteLine("Downloading {0}: {1}%\r", logname, perc);
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            string line = null;
            using (StreamReader reader = new StreamReader(logpath))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    string tag = ReaderDecoder.ExtractTagID(line);
                    DateTime? dt=ReaderDecoder.ExtractDateTime(line);
                    if (dt.HasValue)
                    {
                        Console.WriteLine("{0}:{1}", tag, dt.Value);
                    }
                }
            }
            
        }

        public static void Run2(string host)
        {
            string logname = ReaderLog.FS_LS;
            DateTime start_time = new DateTime(2013, 10, 29, 15, 14, 0);
            List<string> tags = ReaderUtil.ReadTags(host, logname, (fLine) =>
                {
                    DateTime? rec_time = ReaderDecoder.ExtractDateTime(fLine);
                    if (rec_time != null)
                    {
                        if (rec_time.Value > start_time)
                        {
                            return true;
                        }
                    }
                    return false;
                });
            foreach (string tag in tags)
            {
                Console.WriteLine("Tag: {0}", tag);
            }
        }
    }
}
