using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Lidgren.Network;

namespace NGSim.Network
{
    class Server
    {
        NetPeerConfiguration config = new NetPeerConfiguration("NGGameSim");
        private static bool shouldClose = false;
        NetServer server;
        public Server()
        {
            config.Port = 8100;
            config.LocalAddress = new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 });
            config.MaximumConnections = 5;
            server = new NetServer(config);
            server.Start();
            Console.WriteLine("Server Started");
        }

        public void CloseServer()
        {
            server.Shutdown("Shutting Down...");
            shouldClose = true;
        }

        public void WaitForMessage()
        {
            NetIncomingMessage msg;
            while (!shouldClose)
            {
                while ((msg = server.ReadMessage()) != null)
                {
                    Console.WriteLine("Message Type: " + msg.MessageType);
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.Data:
                            int dataInt = msg.ReadInt32();
                            string dataStr = msg.ReadString();
                            Console.WriteLine("Data Packet: {{ {0}, '{1}' }}", dataInt, dataStr);
                            // Send a response back
                            NetOutgoingMessage outmsg = server.CreateMessage();
                            outmsg.Write("This is a response to message " + dataInt);
                            server.SendMessage(outmsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                            break;
                        default: break;
                    }
                    server.Recycle(msg);
                }
                Thread.Sleep(100); // Sleep for 1/10 of a second to simluate a 10UPS simulation
            }
        }
    }
}
