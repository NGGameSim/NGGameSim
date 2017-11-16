using System;
using Lidgren.Network;

namespace NGSim.Network
{
	public class Server
	{
		private NetPeerConfiguration _config = new NetPeerConfiguration("NGGameSim");
		private NetServer _server;

		public Server()
		{
			_config.Port = 8100;
			_config.LocalAddress = new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 });
			_config.MaximumConnections = 5;
		}

		public void StartServer()
		{
			if (_server == null)
			{
				_server = new NetServer(_config);
				_server.Start();
				Console.WriteLine("Server Started"); 
			}
		}

		public void StopServer()
		{
			if (_server != null)
			{
				_server.Shutdown("Shutting down...");
				Console.WriteLine("Server Shutdown"); 
			}
		}

		public void Update()
		{
			NetIncomingMessage msg;
			while ((msg = _server.ReadMessage()) != null)
			{
				Console.WriteLine("Message Type: " + msg.MessageType);
				switch (msg.MessageType)
				{
					case NetIncomingMessageType.Data:
						int dataInt = msg.ReadInt32();
						string dataStr = msg.ReadString();
						Console.WriteLine("Data Packet: {{ {0}, '{1}' }}", dataInt, dataStr);
						// Send a response back
						NetOutgoingMessage outmsg = _server.CreateMessage();
						outmsg.Write("This is a response to message " + dataInt);
						_server.SendMessage(outmsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);
						break;
					default: break;
				}
				_server.Recycle(msg);
			}
		}
	}
}
