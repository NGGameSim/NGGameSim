using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NGSim.Graphics;
using NGSim.Network;
using NGSim.Input;
using NLog;

namespace NGSim
{
	public class SimViewer : Game
	{
		private GraphicsDeviceManager _graphics;
		private Client _client;
		public SimulationManager _simManager;

		public event EventHandler JoinSuccess;

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
			_graphics.PreferMultiSampling = true;
			_graphics.PreferredBackBufferWidth = 1600;
			_graphics.PreferredBackBufferHeight = 900;
			GraphicsDevice.PresentationParameters.MultiSampleCount = 4;
			_graphics.ApplyChanges();

			// Initialize the custom input manager
			InputManager.Initialize();

			// Create the camera
			CameraManager.Set(new ArcBallCamera(GraphicsDevice, distance: 200f, yaw: 45f, pitch: 35f), new ArcBallCameraBehavior());
			(CameraManager.ActiveCamera as ArcBallCamera).MinDistance = 50f;

			// Setup the network stuff
			_client = new Client();

			// Create the simulation
			_simManager = new SimulationManager(GraphicsDevice, Content);
			
		}

		public virtual void OnJoinSuccess(EventArgs e)
		{
			if (JoinSuccess != null)
			{
				JoinSuccess(this, e);
			}
		}

		public void connectClient()
		{
			Console.WriteLine("Attempting to join the server...");
			// if IP present, use it to connect
			// otherwise use the default connection
			if (SimViewerStartupWindow.IPTextBox.Text != "ENTER YOUR IP")
			{
				Console.WriteLine("Using IP: " + SimViewerStartupWindow.IPTextBox.Text);
				_client.Connect(SimViewerStartupWindow.IPTextBox.Text);
			}
			else
			{
				_client.Connect();
			}

			OnJoinSuccess(new EventArgs()); 
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			// Load the models
			
		}

		protected override void Update(GameTime gameTime)
		{
			// Update the custom input manager
			InputManager.Update(gameTime);

			// Update the camera manager
			CameraManager.Update(gameTime);

			// Update the client (network manager)
			_client.Update(gameTime);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(new Color(153, 217, 234));

			Camera camera = CameraManager.ActiveCamera;

			// Render the simulation
			_simManager.Render();

			base.Draw(gameTime);
		}
	
		protected override void OnExiting(object sender, EventArgs args)
		{
			_client.Disconnect();

			base.OnExiting(sender, args);
		}
	}
}