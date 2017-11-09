using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace NGSim.Network
{
    class Client
    {
        private NetClient _client;
        int _lastMessage = 0;
        public Client()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("NGGameSim");
            _client = new NetClient(config);
            _client.Start();
            _client.Connect("127.0.0.1", 8100);
        }

        public void Disconnect()
        {
            _client.Disconnect("Client Disconnecting...");
        }

        // Process messages from the server, if needed
        public void ProcessMessage()
        {
            NetIncomingMessage inmsg;
            while ((inmsg = _client.ReadMessage()) != null)
            {
                Console.WriteLine("Got Packet!");
                if (inmsg.MessageType == NetIncomingMessageType.Data)
                {
                    Console.WriteLine("Data: {{ '{0}' }}", inmsg.ReadString());
                }
                _client.Recycle(inmsg);
            }
        }

        public void SendMessage()
        {
            NetOutgoingMessage msg = _client.CreateMessage();
            msg.Write(_lastMessage);
            msg.Write("This is message " + (_lastMessage++));
            _client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }

    }
}
