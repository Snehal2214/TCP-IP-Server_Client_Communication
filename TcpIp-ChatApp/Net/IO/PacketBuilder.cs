using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpIp_ChatApp.Net.IO
{
    public class PacketBuilder
    {
        MemoryStream _ms;
        public PacketBuilder() 
        {
            _ms = new MemoryStream();
        }
        public void WriteOpcode(byte opcode) 
        {
            _ms.WriteByte(opcode);
        }
        public void WriteMessage(string msg) 
        {
            var msgLength = msg.Length;
            _ms.Write(BitConverter.GetBytes(msgLength),0, sizeof(int));
            byte[] msgBytes = Encoding.UTF8.GetBytes(msg);
            _ms.Write(msgBytes, 0, msgBytes.Length);
        }
        public Byte[] GetPacketBytes() 
        {
            return _ms.ToArray();
        }
    }
}
