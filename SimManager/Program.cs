using System;
using Eto.Forms;
using System.Threading;
using Lidgren.Network;

namespace NGSim
{
	public static class Program
	{
		private static bool shouldClose = false;

		[STAThread]
		public static void Main(string[] args)
		{
			Thread thread = new Thread(testServer);

			Application app = new Application();
			app.Initialized += (sender, e) => { thread.Start(); };
			app.Terminating += (sender, e) => { shouldClose = true; };
			app.Run(new MainWindow());
		}

		private static void testServer()
		{
			NetPeerConfiguration config = new NetPeerConfiguration("NGGameSim");
			config.Port = 8100;
			config.LocalAddress = new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 });
			config.MaximumConnections = 5;
			NetServer server = new NetServer(config);
			server.Start();
			Console.WriteLine("Server Started");

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

			server.Shutdown("Shutting Down...");
		}
	}
}
