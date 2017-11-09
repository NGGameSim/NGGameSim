using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NGSim.Input;

namespace NGSim.Graphics
{
	// Used to implement update behavior 
	public abstract class CameraBehavior
	{
		public abstract void Update(GameTime gameTime, Camera camera);
	}


	// Implements the arc-ball camera that be have been using in testing
	public class ArcBallCameraBehavior : CameraBehavior
	{
		public override void Update(GameTime gameTime, Camera camera)
		{
			ArcBallCamera cam = camera as ArcBallCamera;
			if (cam == null)
				return;

			if (InputManager.IsKeyDown(Keys.W))
				cam.Pitch += (float)gameTime.ElapsedGameTime.TotalSeconds * 20f;
			if (InputManager.IsKeyDown(Keys.S))
				cam.Pitch -= (float)gameTime.ElapsedGameTime.TotalSeconds * 20f;
			if (InputManager.IsKeyDown(Keys.D))
				cam.Yaw += (float)gameTime.ElapsedGameTime.TotalSeconds * 30f;
			if (InputManager.IsKeyDown(Keys.A))
				cam.Yaw -= (float)gameTime.ElapsedGameTime.TotalSeconds * 30f;
			if (InputManager.IsKeyDown(Keys.Q))
				cam.Distance += (float)gameTime.ElapsedGameTime.TotalSeconds * 3f;
			if (InputManager.IsKeyDown(Keys.E))
				cam.Distance -= (float)gameTime.ElapsedGameTime.TotalSeconds * 3f;
			cam.Distance += InputManager.GetScrollDelta() * 0.005f;
		}
	}
}
