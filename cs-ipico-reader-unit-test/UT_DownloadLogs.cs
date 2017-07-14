using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IpicoReader.UT
{
    [TestClass]
    public class UT_DownloadLogs
    {

        public static void Run(string host)
        {
            List<string> logs = ReaderLog.Files;
            foreach (string logname in logs)
            {
                Console.WriteLine("Start downloading {0} ...", logname);

                string logpath = Path.Combine("/tmp", logname);
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
        }
    }
}
