using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpIp_ChatApp.Net.IO
{
    public class PacketReader : BinaryReader
    {
        private NetworkStream _ns;
        public PacketReader(NetworkStream ns) : base(ns)
        {
            _ns = ns;
        }
        public string ReadMessage()
        {
           
            var msgLength = ReadInt32();
            byte[] msgBytes = ReadBytes(msgLength);
            _ns.Write(msgBytes, 0, msgBytes.Length);
            return Encoding.UTF8.GetString(msgBytes);
        }

        

    }
}
