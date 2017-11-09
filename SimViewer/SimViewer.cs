using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NGSim.Graphics;
using NGSim.Input;
using Lidgren.Network;
using NLog;

namespace NGSim
{
	public class SimViewer : Game
	{
		private GraphicsDeviceManager _graphics;
		private NetClient _client;

		private CModel _uavModel;
		private CModel _tankModel;

		// Temporary ground model
		private VertexBuffer _vBuffer;
		private IndexBuffer _iBuffer;
		private BasicEffect _groundEffect;
		private readonly int WORLD_SIZE = 50;

		public SimViewer() :
			base()
		{
			Content.RootDirectory = "Content";
			_graphics = new GraphicsDeviceManager(this);
			IsFixedTimeStep = true;
			TargetElapsedTime = TimeSpan.FromSeconds(1f / 30);
		}

		protected override void Initialize()
		{
			base.Initialize();

			// Setup graphics device settings
			_graphics.GraphicsProfile = GraphicsProfile.HiDef;
			_graphics.PreferMultiSampling = true;
			GraphicsDevice.PresentationParameters.MultiSampleCount = 4;
			_graphics.ApplyChanges();

			// Initialize the custom input manager
			InputManager.Initialize();

			// Initialize the ground
			int won2 = WORLD_SIZE / 2;
			VertexPositionColor[] vGround = new VertexPositionColor[4]
			{
				new VertexPositionColor(new Vector3(-won2, 0, -won2), Color.Green),
				new VertexPositionColor(new Vector3(won2, 0, -won2), Color.Green),
				new VertexPositionColor(new Vector3(won2, 0, won2), Color.Green),
				new VertexPositionColor(new Vector3(-won2, 0, won2), Color.Green)
			};
			_vBuffer = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, 4, BufferUsage.None);
			_vBuffer.SetData(vGround);
			ushort[] iGround = new ushort[6] { 0, 1, 3, 1, 2, 3 };
			_iBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, 6, BufferUsage.None);
			_iBuffer.SetData(iGround);

			// Initialize the ground shader
			_groundEffect = new BasicEffect(GraphicsDevice);
			_groundEffect.VertexColorEnabled = true;
			_groundEffect.TextureEnabled = false;
			_groundEffect.LightingEnabled = false;

			// Create the camera
			CameraManager.Set(new ArcBallCamera(GraphicsDevice, distance: 20f, yaw: 0f, pitch: 45f), new ArcBallCameraBehavior());
			(CameraManager.ActiveCamera as ArcBallCamera).MinDistance = 2f;

			// Load the models
			_uavModel = new CModel(GraphicsDevice, Content.Load<Model>("UAV"));
			_tankModel = new CModel(GraphicsDevice, Content.Load<Model>("tank"));
      
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
			// Update the custom input manager
			InputManager.Update(gameTime);

			// Update the camera manager
			CameraManager.Update(gameTime);

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

			Camera camera = CameraManager.ActiveCamera;

			_groundEffect.World = Matrix.Identity;
			_groundEffect.View = camera.ViewMatrix;
			_groundEffect.Projection = camera.ProjectionMatrix;
			GraphicsDevice.SetVertexBuffer(_vBuffer);
			GraphicsDevice.Indices = _iBuffer;
			_groundEffect.CurrentTechnique.Passes[0].Apply();
			GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4);

			_tankModel.Render(camera, Vector3.Right * 3 + Vector3.Up * 2);
			_uavModel.Render(camera, Vector3.Left * 3 + Vector3.Up * 5);

			base.Draw(gameTime);
		}
    
		protected override void OnExiting(object sender, EventArgs args)
		{
			_client.Disconnect("Client Disconnecting...");

			base.OnExiting(sender, args);
		}
	}
}