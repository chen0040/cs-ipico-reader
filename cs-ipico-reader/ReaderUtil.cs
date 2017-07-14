using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using IpicoReader.TcpSockets;
using IpicoReader.Ftp;

namespace IpicoReader
{
    public class ReaderUtil
    {
        public static bool CanConnect2Reader(string host)
        {
            try
            {
                IPAddress.Parse(host);
                Ping pingServer = new Ping();
                PingOptions pingOpt = new PingOptions();
                pingOpt.DontFragment = true;
                pingOpt.Ttl = 64;
                PingReply serverReply = pingServer.Send(host);
                if (serverReply.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (FormatException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static DateTime GetReaderTime(string host, ReaderType reader_type, bool is_async=false)
        {
            int port = 0;
            string cmd = "ab00000222";
            if (reader_type==ReaderType.Lite)
            {
                port = 10000;
            }
            else if (reader_type==ReaderType.Elite)
            {
                port = 9999;
            }
            DateTime reader_time = DateTime.Now;
            if (is_async)
            {
                reader_time = ParseReaderTime(AsyncTcpSocketUtil.SendRecieve(host, port, cmd).Trim());
            }
            else
            {
                reader_time = ParseReaderTime(SyncTcpSocketUtil.SendRecieve(host, port, cmd, 36).Trim());
            }
            return (reader_time);
        }

        public static string UpdateReaderTime(string address, ReaderType reader_type, bool is_async=false)
        {
            int port = 0;
            string cmd = "000701";
            string rdrResponse = String.Empty;
            DateTime pcTime = DateTime.Now;
            string formattedTime = String.Empty;
            if (reader_type==ReaderType.Lite)
            {
                port = 10000;
            }
            else if (reader_type==ReaderType.Elite)
            {
                port = 9999;
            }
            formattedTime = SystemUtil.GetFormattedPCTime(pcTime, GetLatency(address));
            cmd += formattedTime;
            cmd = "ab" + cmd + SystemUtil.ComputeCheckSum(cmd);
            rdrResponse = null;
            if(is_async)
            {
                rdrResponse=AsyncTcpSocketUtil.SendRecieve(address, port, cmd);
            }
            else
            {
                rdrResponse = SyncTcpSocketUtil.SendRecieve(address, port, cmd, 10);
            }
            if (rdrResponse.Trim() == "ab00000121" || rdrResponse == "ab00000121" + System.Environment.NewLine || rdrResponse != null)
            {
                return (pcTime.ToString());
            }
            else
            {
                return ("Error!");

            }
        }

        /// <summary>
        /// Compare the reader time and the computer system time and return the difference
        /// </summary>
        /// <param name="address"></param>
        /// <param name="reader_type"></param>
        /// <param name="is_async"></param>
        /// <returns>Difference between the computer time and the reader time</returns>
        public static TimeSpan CompareClocks(string address, ReaderType reader_type, bool is_async=false)
        {
            DateTime rdrTime = (DateTime)GetReaderTime(address, reader_type, is_async);
            DateTime pcTime = DateTime.Now;
            //Must account for rudimentary latency between request and receipt
            TimeSpan latency = TimeSpan.FromMilliseconds(GetLatency(address));
            TimeSpan rdrDifference = (pcTime.Subtract(rdrTime)).Subtract(latency);
            return rdrDifference;
        }

        /// <summary>
        /// Return the latency time for connecting to the reader.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static double GetLatency(string address)
        {
            double timeToTalk = 0;
            try
            {
                IPAddress.Parse(address);
                Ping pingServer = new Ping();
                PingOptions pingOpt = new PingOptions();
                pingOpt.DontFragment = true;
                pingOpt.Ttl = 64;
                PingReply serverReply = pingServer.Send(address);
                timeToTalk = (double)((Int64)serverReply.RoundtripTime);
            }
            catch
            {

            }
            return timeToTalk;
        }

        public static void DownloadLog(string host, string logname, string local_path, FtpUtil.DownloadFileProgressHandle handle=null)
        {
            FtpUtil.DownloadFileByName(host, logname, local_path, handle);
        }

        public static List<string> ReadTags(string host, string logname, FtpUtil.CriteriaHandle handle=null)
        {
            return FtpUtil.ReadTags(host, logname, handle);
        }

        public static List<ReaderRecord> ReadRecords(string host, string logname, FtpUtil.CriteriaHandle handle = null)
        {
            return FtpUtil.ReadRecords(host, logname, handle);
        }

        /// <summary>
        /// Clearly history of a reader, only apply to elite
        /// </summary>
        /// <param name="host"></param>
        public static string ClearEliteReaderHistory(string host)
        {
			string rdrResponse = String.Empty;
		    rdrResponse = SyncTcpSocketUtil.SendRecieve(host,9999,"clear_history",15);
			return rdrResponse;
        }

        public static DateTime ParseReaderTime(string reader_response)
        {
            //Console.WriteLine(reader_response);
            DateTime dt = DateTime.Now;
            if (reader_response.Trim().Length == 28)
            {
                string rdrDateTime = reader_response.Remove(0, 8);
                rdrDateTime = rdrDateTime.Remove(14, 6);
                string year, month, day, dayOfWeek, hour, minute, second;
                year = rdrDateTime.Remove(2, 12);
                month = rdrDateTime.Remove(0, 2);
                month = month.Remove(2, 10);
                day = rdrDateTime.Remove(0, 4);
                day = day.Remove(2, 8);
                dayOfWeek = rdrDateTime.Remove(0, 6);
                dayOfWeek = dayOfWeek.Remove(2, 6);
                hour = rdrDateTime.Remove(0, 8);
                hour = hour.Remove(2, 4);
                minute = rdrDateTime.Remove(0, 10);
                minute = minute.Remove(2, 2);
                second = rdrDateTime.Remove(0, 12);
                DateTime.TryParse(month + "/" + day + "/" + year + " " + hour + ":" + minute + ":" + second,
                    out dt);
            }
            else
            {

            }
            return dt;
        }

        
    }
}
