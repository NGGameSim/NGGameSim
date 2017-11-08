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

		private ArcBallCamera _camera;
		private CModel _uavModel;
		private CModel _tankModel;

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

			// Create the camera
			_camera = new ArcBallCamera(GraphicsDevice, yaw: 0f, pitch: 0f);
			_camera.MinDistance = 2f;

			// Load the models
			Texture2D checker = Content.Load<Texture2D>("checkers");
			_uavModel = new CModel(GraphicsDevice, Content.Load<Model>("UAV"), checker);
			_tankModel = new CModel(GraphicsDevice, Content.Load<Model>("tank"), checker);
      
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

			// Update the camera
			if (InputManager.IsKeyDown(Keys.W))
				_camera.Pitch += (float)gameTime.ElapsedGameTime.TotalSeconds * 20f;
			if (InputManager.IsKeyDown(Keys.S))
				_camera.Pitch -= (float)gameTime.ElapsedGameTime.TotalSeconds * 20f;
			if (InputManager.IsKeyDown(Keys.D))
				_camera.Yaw += (float)gameTime.ElapsedGameTime.TotalSeconds * 30f;
			if (InputManager.IsKeyDown(Keys.A))
				_camera.Yaw -= (float)gameTime.ElapsedGameTime.TotalSeconds * 30f;
			if (InputManager.IsKeyDown(Keys.Q))
				_camera.Distance += (float)gameTime.ElapsedGameTime.TotalSeconds * 3f;
			if (InputManager.IsKeyDown(Keys.E))
				_camera.Distance -= (float)gameTime.ElapsedGameTime.TotalSeconds * 3f;
			_camera.Distance += InputManager.GetScrollDelta() * 0.005f;

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

			//_tankModel.Render(_camera, Vector3.Zero);
			_uavModel.Render(_camera, Vector3.Zero);

			base.Draw(gameTime);
		}
    
		protected override void OnExiting(object sender, EventArgs args)
		{
			_client.Disconnect("Client Disconnecting...");

			base.OnExiting(sender, args);
		}
	}
}