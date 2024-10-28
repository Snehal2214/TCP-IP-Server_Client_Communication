using ChatServer.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }
        
        PacketReader _packetReader;
        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();

            _packetReader = new PacketReader(ClientSocket.GetStream());

            var opcode = _packetReader.ReadByte();
            Username = _packetReader.ReadMessage();

            Console.WriteLine($"[{DateTime.Now}]: Client has connected with the username: {Username}");

            Task.Run(() => Process());
        }

        void Process() 
        {
            while (true) 
            {
                try 
                { 
                    var opcode = _packetReader.ReadByte();
                    switch (opcode) 
                    {
                        case 5:
                            var msg = _packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: Message Received! {msg}");
                            Program.BroadcastMessage($"[{DateTime.Now}]: [{Username}]: {msg}");

                            SendAcknowledgment("ACK");

                            break;
                        default:
                            break;
                    }
                }
                catch(Exception) 
                {
                    Console.WriteLine($"[{UID.ToString()}]: Disconnected!");
                    Program.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }

        }

        private void SendAcknowledgment(string ackMessage)
        {
            try
            {
                var ackPacket = new PacketBuilder();
                ackPacket.WriteOpcode(6);  // Opcode for acknowledgment
                ackPacket.WriteMessage("ACK\r\n");  // Acknowledgment message
                ClientSocket.Client.Send(ackPacket.GetPacketBytes());
                Console.WriteLine("Acknowledgement sent to client.");
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Failed to send acknowledgment : {ex.Message}");
            }
            

        }
        

    }

}
