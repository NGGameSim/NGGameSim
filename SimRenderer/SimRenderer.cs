using System;
using Microsoft.Xna.Framework;

namespace NGSim
{
	public class SimRenderer : Game
	{
		private GraphicsDeviceManager _graphics;

		public SimRenderer() :
			base()
		{
			_graphics = new GraphicsDeviceManager(this);
		}
	}
}
