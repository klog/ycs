using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace YCSLib
{
    internal class ReceiveStore
    {
        public ReceiveStore(Socket s)
        {
            socket = s;
        }

        public readonly Socket socket;
        public byte[] buffer;
    }
}
