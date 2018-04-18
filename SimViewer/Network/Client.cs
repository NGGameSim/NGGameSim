using System;
using Microsoft.Xna.Framework;
using Lidgren.Network;

namespace NGSim.Network
{
	public class Client
	{
		public static Client Instance { get; private set; } = null;

		private NetClient _client;

		public Client()
		{
			if (Instance != null)
				throw new InvalidOperationException("Cannot create more than one network client instance.");
			Instance = this;

			NetPeerConfiguration config = new NetPeerConfiguration("NGGameSim");
			_client = new NetClient(config);
			_client.Start();
		}

		public void Connect()
		{
			_client.Connect("127.0.0.1", 8100);
		}

		public void Connect(String IP)
		{
			_client.Connect(IP, 8100);
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
					byte opcode = inmsg.ReadByte();
					switch (opcode)
					{
						case 1:
							SimulationManager.Instance.TranslateEntityPacket(inmsg);
							break;
						case 2:
							SimulationManager.Instance.TranslateMissilePacket(inmsg);
							break;
						default:
							Console.WriteLine($"Unknown data packet type ({opcode}).");
							break;
					}
				}
				_client.Recycle(inmsg);
			}
		}
	}
}
