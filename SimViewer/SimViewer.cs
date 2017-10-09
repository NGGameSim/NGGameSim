using System;
using Microsoft.Xna.Framework;

namespace NGSim
{
	public class SimClient : Game
	{
		private GraphicsDeviceManager _graphics;

		public SimClient() :
			base()
		{
			_graphics = new GraphicsDeviceManager(this);
		}
	}
}
