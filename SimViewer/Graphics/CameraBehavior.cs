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

			if (InputManager.IsKeyDown(Keys.W) || InputManager.IsKeyDown(Keys.Up))
				cam.Pitch += (float)gameTime.ElapsedGameTime.TotalSeconds * 20f;
			if (InputManager.IsKeyDown(Keys.S) || InputManager.IsKeyDown(Keys.Down))
				cam.Pitch -= (float)gameTime.ElapsedGameTime.TotalSeconds * 20f;
			if (InputManager.IsKeyDown(Keys.D) || InputManager.IsKeyDown(Keys.Right))
				cam.Yaw += (float)gameTime.ElapsedGameTime.TotalSeconds * 30f;
			if (InputManager.IsKeyDown(Keys.A) || InputManager.IsKeyDown(Keys.Left))
				cam.Yaw -= (float)gameTime.ElapsedGameTime.TotalSeconds * 30f;
			if (InputManager.IsKeyDown(Keys.Q))
				cam.Distance += (float)gameTime.ElapsedGameTime.TotalSeconds * 25f;
			if (InputManager.IsKeyDown(Keys.E))
				cam.Distance -= (float)gameTime.ElapsedGameTime.TotalSeconds * 25f;
			cam.Distance += InputManager.GetScrollDelta() * 0.005f;
		}
	}
}
