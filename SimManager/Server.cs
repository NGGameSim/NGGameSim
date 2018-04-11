using System;
using Lidgren.Network;

namespace NGSim
{
	public class Server
	{
		public static Server Instance { get; private set; } = null;

		private NetPeerConfiguration _config = new NetPeerConfiguration("NGGameSim");
		private NetServer _server;

		public uint TotalSentMessages
		{
			get { return (uint)_server.Statistics.SentMessages; }
		}

		public uint TotalSentBytes
		{
			get { return (uint)_server.Statistics.SentBytes; }
		}

		public bool isConnected
		{
			get { if(_server.ConnectionsCount >= 1) { return true; } else { return false; } }
		}

		public Server()
		{
			if (Instance != null)
				throw new InvalidOperationException("Cannot create more than once server instance.");
			Instance = this;

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

		// Create a new message with the given opcode
		public NetOutgoingMessage CreateMessage(byte opcode)
		{
			var message = _server.CreateMessage();
			message.Write(opcode);
			return message;
		}

		// Sends a prepared message to all connections
		public void SendMessage(NetOutgoingMessage msg)
		{
			_server.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
		}

		public void Update()
		{
			NetIncomingMessage msg;
			while ((msg = _server.ReadMessage()) != null)
			{
				Console.WriteLine("Message Type: " + msg.MessageType);
				_server.Recycle(msg);
			}
		}
	}
}
