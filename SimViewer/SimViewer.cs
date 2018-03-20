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
		private SimulationManager _simManager;

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
			_client.Connect();

			// Create the simulation
			_simManager = new SimulationManager(GraphicsDevice, Content);
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

			//_world.Draw(GraphicsDevice, camera);
			//_tankModel.Render(camera, Vector3.Right * 3 + Vector3.Up * 2);
			//_uavModel.Render(camera, Vector3.Left * 3 + Vector3.Up * 5);

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