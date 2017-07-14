using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IpicoReader
{
    public class ReaderLog
    {
        public static List<string> Files
        {
            get
            {
                List<string> files = new List<string>();
                files.Add(ttyS0);
                files.Add(ttyS1);
                files.Add(FS_LS);
                files.Add(infod);

                return files;
            }
        }

        public static string ttyS0
        {
            get
            {
                return "ttyS0.log";
            }
        }

        public static string ttyS1
        {
            get
            {
                return "ttyS1.log";
            }
        }

        public static string FS_LS
        {
            get
            {
                return "FS_LS.log";
            }
        }

        public static string infod
        {
            get
            {
                return "infod.log";
            }
        }

    }
}
