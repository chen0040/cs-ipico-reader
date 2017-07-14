using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace SimuKit.Sports.IPICO.TcpSockets
{
    public class AsyncTcpSocketUtil
    {
        private static int rPort;

        private static ManualResetEvent connectComplete = new ManualResetEvent(false);
        private static ManualResetEvent sendComplete = new ManualResetEvent(false);
        private static ManualResetEvent receiveComplete = new ManualResetEvent(false);

        public static string socketResponse = String.Empty;

        public static bool haltReceive = false;

        public static string SendRecieve(string rdrAddr, int port, string cmd)
        {
            Socket s = null;
            try
            {
                IPAddress rdrIp = IPAddress.Parse(rdrAddr);
                IPEndPoint rdrEP = new IPEndPoint(rdrIp, port);
                rPort = port;
                s = new Socket(rdrEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                s.BeginConnect(rdrEP, new AsyncCallback(connectCallBack), s);
                connectComplete.WaitOne();
                send(s, cmd + Environment.NewLine);
                sendComplete.WaitOne();
                recieve(s);
                receiveComplete.WaitOne();
                s.Shutdown(SocketShutdown.Both);
                s.Close();
                connectComplete.Reset();
                sendComplete.Reset();
                receiveComplete.Reset();
            }
            catch
            {
                //Debug.msgbox.show(e.ToString() + "\n" + "Reader address supplied:" + rdrAddr);
            }
            return socketResponse;
        }

        private static void send(Socket s, string cmd)
        {
            byte[] cmdByte = Encoding.ASCII.GetBytes(cmd);
            s.BeginSend(cmdByte, 0, cmdByte.Length, 0, new AsyncCallback(sendCallBack), s);
        }

        private static void recieve(Socket s)
        {
            try
            {
                TcpStateObject state = new TcpStateObject();
                state.workerSock = s;
                s.BeginReceive(state.socketBuffer, 0, state.socketBuffer.Length, 0, new AsyncCallback(receieveCallback), state);
            }
            catch
            {
                //Debug.msgbox.show(e.ToString());
            }
        }

        private static void connectCallBack(IAsyncResult ar)
        {
            try
            {
                Socket s = (Socket)ar.AsyncState;
                s.EndConnect(ar);
                connectComplete.Set();
            }
            catch
            {

            }
        }
        private static void sendCallBack(IAsyncResult ar)
        {
            try
            {
                Socket s = (Socket)ar.AsyncState;
                int bytesSent = s.EndSend(ar);
                sendComplete.Set();
            }
            catch
            {
                //Console.WriteLine(e.ToString());
            }
        }

        private static void receieveCallback(IAsyncResult ar)
        {
            try
            {
                TcpStateObject state = (TcpStateObject)ar.AsyncState;
                Socket s = state.workerSock;
                int bytesRead = s.EndReceive(ar);
                if (bytesRead > 0)
                {
                    state.sb.Append(Encoding.ASCII.GetString(state.socketBuffer, 0, bytesRead));
                    s.BeginReceive(state.socketBuffer, 0, TcpStateObject.maxBytes, 0, new AsyncCallback(receieveCallback), state);
                }
                else
                {
                    if (state.sb.Length > 1)
                    {
                        socketResponse = state.sb.ToString();
                    }
                    receiveComplete.Set();
                }
            }
            catch
            {

            }
        }
    }
}
