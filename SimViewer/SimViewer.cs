using System;
using Microsoft.Xna.Framework;
using Lidgren.Network;

namespace NGSim
{
	public class SimViewer : Game
	{
		private GraphicsDeviceManager _graphics;
		private NetClient _client;

		public SimViewer() :
			base()
		{
			_graphics = new GraphicsDeviceManager(this);
		}

		protected override void Initialize()
		{
			base.Initialize();

			NetPeerConfiguration config = new NetPeerConfiguration("NGGameSim");
			_client = new NetClient(config);
			_client.Start();
			_client.Connect("127.0.0.1", 8100);
		}

		double _lastSendTime = 0;
		int _lastMessage = 0;
		protected override void Update(GameTime gameTime)
		{
			_lastSendTime += gameTime.ElapsedGameTime.TotalSeconds;
			if (_lastSendTime > 1.0f) // Send a message every second
			{
				NetOutgoingMessage msg = _client.CreateMessage();
				msg.Write(_lastMessage);
				msg.Write("This is message " + (_lastMessage++));
				_client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
				_lastSendTime = 0;
			}

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

			base.Update(gameTime);
		}

		protected override void OnExiting(object sender, EventArgs args)
		{
			_client.Disconnect("Client Disconnecting...");

			base.OnExiting(sender, args);
		}
	}
}