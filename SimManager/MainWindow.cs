using System;
using Eto.Forms;
using Eto.Drawing;
using Lidgren.Network;
using System.Net;
using System.Net.Sockets;

namespace NGSim
{
	public class MainWindow : Form
	{
        bool doOnce = false;
        public MainWindow()
        {
            ClientSize = new Size(400, 300);
            Title = "SimManager";
            if (!doOnce)
            {
                doOnce = true;
                SetServer();
            }
        }
        public void SetServer() { 
            NetPeerConfiguration config = new NetPeerConfiguration("test");
            config.Port = 5432;
            config.LocalAddress = new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 });
            config.MaximumConnections = 1000;
            NetServer server = new NetServer(config);
            server.Start();
            NetIncomingMessage msg = null;
            while (true)
            {
                while ((msg = server.ReadMessage()) != null)
                {

                    Console.WriteLine(msg.MessageType.ToString());
                    /*string str = msg.ReadString();
                    Int16 int16 = msg.ReadInt16();
                    Console.WriteLine(str);
                    Console.WriteLine("Ints: {0}", int16);*/
                    if (msg.MessageType == NetIncomingMessageType.Data)
                    {
                        Console.WriteLine(msg.ReadInt16());
                        Console.WriteLine(msg.ReadString());
                    }
                }
            }
        }
    }
}
