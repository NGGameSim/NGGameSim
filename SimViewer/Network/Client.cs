using System;
using Microsoft.Xna.Framework;
using Lidgren.Network;

namespace NGSim.Network
{
	public class Client
	{
		private NetClient _client;

		private int _lastMessage = 0;
		private TimeSpan _lastSendTime = TimeSpan.Zero;

		public Client()
		{
			NetPeerConfiguration config = new NetPeerConfiguration("NGGameSim");
			_client = new NetClient(config);
			_client.Start();
		}

		public void Connect()
		{
			_client.Connect("127.0.0.1", 8100);
		}

		public void Disconnect()
		{
			_client.Disconnect("Client Disconnecting...");
		}

		// Process messages from the server, if needed
		public void Update(GameTime gameTime)
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
			
			if ((gameTime.TotalGameTime - _lastSendTime).TotalSeconds >= 1)
			{
				NetOutgoingMessage msg = _client.CreateMessage();
				msg.Write(_lastMessage);
				msg.Write("This is message " + (_lastMessage++));
				_client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
				_lastSendTime = gameTime.TotalGameTime;
			}
		}
	}
}
