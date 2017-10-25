using System;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NGSim
{
	public class SimViewer : Game
	{
		private GraphicsDeviceManager _graphics;
        bool doOnce = false;
        public SimViewer() : base() {
            _graphics = new GraphicsDeviceManager(this);
            if (!doOnce)
            {
                doOnce = true;
                SetClient();
            }
        }
        public void SetClient() { 
            NetPeerConfiguration config = new NetPeerConfiguration("test");
            NetClient client = new NetClient(config);
            client.Start();
            client.Connect("127.0.0.1", 5432);
            /*NetOutgoingMessage msg = client.CreateMessage();
            msg.Write((Int16)3346);
            msg.Write("Test Message 1 whooahaa");
            client.SendMessage(msg, NetDeliveryMethod.ReliableUnordered);
            client.FlushSendQueue();
            NetOutgoingMessage msg2 = client.CreateMessage();
            msg2.Write((Int16)48);
            msg2.Write("deaxjnasxnajs");
            client.SendMessage(msg2, NetDeliveryMethod.ReliableUnordered);
            client.FlushSendQueue();*/
        }
    }
}