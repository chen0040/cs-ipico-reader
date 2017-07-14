using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace IpicoReader.TcpSockets
{
    public class SyncTcpSocketUtil
    {
        public static Socket CreateSocket(string address, int port)
        {
            Socket s = null;
            
            IPAddress ip_address = IPAddress.Parse(address);
            IPEndPoint end_point = new IPEndPoint(ip_address, port);
            Socket socket = new Socket(end_point.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.ReceiveTimeout = 5000;
            socket.SendTimeout = 5000;
            socket.Connect(end_point);
            if (socket.Connected)
            {
                s = socket;
            }
            return s;
        }

        public static string SendRecieve(string address, int port, string cmd, int expected_length)
        {
            Socket s = CreateSocket(address, port);
            byte[] cmd_data = Encoding.ASCII.GetBytes(cmd + Environment.NewLine);
            byte[] response_data = new Byte[expected_length];
            if (s == null)
            {
                return ("Connection error (socket is null).");
            }
            s.Send(cmd_data, cmd_data.Length, 0);

            Console.WriteLine("Sent (" + cmd_data.Length + "): " + cmd);

            int byteOffset = 0;
            string response = String.Empty;

            Thread.Sleep(35);
            byteOffset = s.Receive(response_data, 0, response_data.Length, SocketFlags.None);
            response = Encoding.ASCII.GetString(response_data, 0, byteOffset);
            s.Disconnect(true);

            return response;
        }
    }
}
