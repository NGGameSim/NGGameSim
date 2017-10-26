using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NGSim.Graphics;
using Lidgren.Network;

namespace NGSim
{
	public class SimViewer : Game
	{
		private GraphicsDeviceManager _graphics;
		private NetClient _client;

		private BasicEffect _effect;
		private IndexBuffer _iBuffer;
		private VertexBuffer _vBuffer;
		private ArcBallCamera _camera;

		public SimViewer() :
			base()
		{
			_graphics = new GraphicsDeviceManager(this);
		}

		protected override void Initialize()
		{
			base.Initialize();

			// Setup graphics device settings
			_graphics.GraphicsProfile = GraphicsProfile.HiDef;
			_graphics.PreferMultiSampling = true;
			GraphicsDevice.PresentationParameters.MultiSampleCount = 4;
			_graphics.ApplyChanges();

			// Create basic shader
			_effect = new BasicEffect(GraphicsDevice);
			_effect.VertexColorEnabled = true;
			_effect.LightingEnabled = false;
			_effect.TextureEnabled = false;

			// Populate the vertex buffer
			_vBuffer = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, 8, BufferUsage.None);
			_vBuffer.SetData(new VertexPositionColor[8]
			{
				new VertexPositionColor(new Vector3(-1, -1, -1), Color.White),
				new VertexPositionColor(new Vector3( 1, -1, -1), Color.Blue),
				new VertexPositionColor(new Vector3( 1, -1,  1), Color.Yellow),
				new VertexPositionColor(new Vector3(-1, -1,  1), Color.Red),
				new VertexPositionColor(new Vector3(-1,  1, -1), Color.Green),
				new VertexPositionColor(new Vector3( 1,  1, -1), Color.Magenta),
				new VertexPositionColor(new Vector3( 1,  1,  1), Color.Cyan),
				new VertexPositionColor(new Vector3(-1,  1,  1), Color.Pink)
			});

			// Populate the index buffer
			_iBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, 36, BufferUsage.None);
			_iBuffer.SetData(new ushort[36]
			{
				6, 5, 1, 6, 1, 2, // +x
				4, 7, 3, 4, 3, 0, // -x
				4, 5, 6, 4, 6, 7, // +y
				1, 0, 3, 1, 3, 2, // -y
				7, 6, 2, 7, 2, 3, // +z
				5, 4, 0, 5, 0, 1  // -z
			});

			// Create the camera
			_camera = new ArcBallCamera(GraphicsDevice, yaw: 0f, pitch: 0f);
      
			// Setup the network stuff
			NetPeerConfiguration config = new NetPeerConfiguration("NGGameSim");
			_client = new NetClient(config);
			_client.Start();
			_client.Connect("127.0.0.1", 8100);
		}

		double _lastSendTime = 0;
		int _lastMessage = 0;
		protected override void Update(GameTime gameTime)
		{
			// Update the camera
			_camera.Pitch += (float)gameTime.ElapsedGameTime.TotalSeconds * 3f;
			_camera.Yaw += (float)gameTime.ElapsedGameTime.TotalSeconds * 12f;
			_camera.Distance += (float)gameTime.ElapsedGameTime.TotalSeconds;
      
			// Send messages to the server
			_lastSendTime += gameTime.ElapsedGameTime.TotalSeconds;
			if (_lastSendTime > 1.0f) // Send a message every second
			{
				NetOutgoingMessage msg = _client.CreateMessage();
				msg.Write(_lastMessage);
				msg.Write("This is message " + (_lastMessage++));
				_client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
				_lastSendTime = 0;
			}
      
			// Process messages from the server, if needed
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

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			GraphicsDevice.SetVertexBuffer(_vBuffer);
			GraphicsDevice.Indices = _iBuffer;

			_effect.View = _camera.ViewMatrix;
			_effect.Projection = _camera.ProjectionMatrix;
			_effect.World = Matrix.Identity;

			_effect.CurrentTechnique.Passes[0].Apply();
			GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 12);

			base.Draw(gameTime);
		}
    
		protected override void OnExiting(object sender, EventArgs args)
		{
			_client.Disconnect("Client Disconnecting...");

			base.OnExiting(sender, args);
		}
	}
}