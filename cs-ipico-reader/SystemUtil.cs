using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace IpicoReader
{
    public class SystemUtil
    {
        public static string ComputeCheckSum(string cmd)
        {
            /* Start the byte addition by turning it into a byte array */
            byte[] cmd_byte = new byte[cmd.Length];
            cmd_byte = Encoding.Default.GetBytes(cmd);
            /* Need to convert character-by-character into hex; need temporary integer */
            int chk_int = 0;
            for (int i = 0; i <= cmd_byte.Length - 1; i++)
            {
                chk_int += Convert.ToInt32(cmd_byte[i].ToString(), 10);
            }
            string chk_str = Convert.ToString(chk_int, 16);
            /* Return the string type, but need to remove the first digit, and ensure that it's lower case */
            return chk_str.Remove(0, 1).ToLower();
        }

        public static DateTime DateTimeParse(string text)
        {
            DateTime result;
            if (DateTime.TryParseExact(text, new string[] { "yyyy-MM-dd HH:mm:ss" }, new CultureInfo("en-US"), DateTimeStyles.None, out result))
            {
                return result;
            }
            if (DateTime.TryParse(text, out result))
            {
                return result;
            }
            return DateTime.MaxValue;
        }

        public static string GetFormattedPCTime(DateTime pcTime, double latency)
        {
            string dt = string.Empty;
            string dayOfWeek = "00";
            switch (pcTime.DayOfWeek.ToString())
            {
                case "Sunday":
                    dayOfWeek = "00";
                    break;
                case "Monday":
                    dayOfWeek = "01";
                    break;
                case "Tuesday":
                    dayOfWeek = "02";
                    break;
                case "Wednesday":
                    dayOfWeek = "03";
                    break;
                case "Thursday":
                    dayOfWeek = "04";
                    break;
                case "Friday":
                    dayOfWeek = "05";
                    break;
                case "Saturday":
                    dayOfWeek = "06";
                    break;
            }
            dt = DateTime.Now.ToString("yyMMdd") + dayOfWeek + (DateTime.Now.AddMilliseconds(latency)).ToString("HHmmss");
            return dt;
        }
    }
}
