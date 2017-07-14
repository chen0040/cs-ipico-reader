using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace SimuKit.Sports.IPICO.TcpSockets
{
    public class TcpStateObject
    {
        public Socket workerSock = null;
        public const int maxBytes = 256;
        public byte[] socketBuffer = new Byte[maxBytes];
        public StringBuilder sb = new StringBuilder();
    }
}
