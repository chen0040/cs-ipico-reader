using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace SimuKit.Sports.IPICO
{
    /// <summary>
    /// Description of tagChars.
    /// </summary>
    public class ReaderDecoder
    {
        private static int strLen(string fileLine)
        {
            return fileLine.Length;
        }

        public static string ExtractTagID(string fileLine)
        {
            if (fileLine.Length < 16) return null;
            string tag = fileLine.Substring(4, 12);
            return tag;
        }

        public static string ExtractDate(string fileLine)
        {
            if (fileLine.Length < 26) return null;
            string result = fileLine.Substring(20, 6);
            return result;
        }

        public static DateTime? ExtractDateTime(string fileLine)
        {
            string date_string = ExtractDate(fileLine);
            string time_string = ExtractTime(fileLine);

            if (string.IsNullOrEmpty(date_string) || string.IsNullOrEmpty(time_string))
            {
                return null;
            }

            DateTime parsedDate = DateTime.Now;
            if(DateTime.TryParseExact(string.Format("{0} {1}", date_string, time_string),
                "yyMMdd HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                return parsedDate;
            }
            return null;
        }

        public static string ExtractTime(string fileLine)
        {
            if (fileLine.Length < 32) return null;
            string result = fileLine.Substring(26, 6);
            return result;
        }

        public static string ExtractFSLS(string fileLine)
        {
            if (fileLine.Length < 38) return "None";
            string result = fileLine.Substring(36, 2);
            return result;
        }

        public static int ExtractReaderMilliSeconds(string fileLine)
        {
            if (fileLine.Length < 34) return 0;
            string result = fileLine.Substring(32, 2);
            int result1;
            if (int.TryParse(result, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result1))
            {
                return result1;
            }
            return 0;
        }

        public static int ExtractPCMilliSeconds(string fileLine)
        {
            if (fileLine.Length < 36) return 0;
            string result = fileLine.Substring(34, 2);
            int result1;
            if (int.TryParse(result, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result1))
            {
                return result1;
            }
            return 0;
        }

        public static string ExtractChan1(string fileLine)
        {
            if (fileLine.Length < 18) return null;
            string result = fileLine.Substring(16, 2);
            return result;
        }

        public static string ExtractChan2(string fileLine)
        {
            if (fileLine.Length < 20) return null;
            string result = fileLine.Substring(18, 2);
            return result;
        }

        public static string ExtractDecId(string fileLine)
        {
            if (fileLine.Length < 4) return null;
            string result = fileLine.Substring(2, 2);
            return result;
        }


    }
}
