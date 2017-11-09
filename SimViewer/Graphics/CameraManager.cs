using System;
using Microsoft.Xna.Framework;

namespace NGSim.Graphics
{
	public static class CameraManager
	{
		public static Camera ActiveCamera = null;
		public static CameraBehavior ActiveBehavior = null;

		public static void Set(Camera cam, CameraBehavior beh)
		{
			ActiveCamera = cam;
			ActiveBehavior = beh;
		}

		public static void Update(GameTime gameTime)
		{
			if (ActiveCamera != null)
				ActiveBehavior?.Update(gameTime, ActiveCamera);
		}
	}
}
