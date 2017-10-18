using System;
using Microsoft.Xna.Framework;
using NLog;

namespace NGSim
{
	public class SimViewer : Game
	{
		private GraphicsDeviceManager _graphics;

		public SimViewer() : base()
		{
			_graphics = new GraphicsDeviceManager(this);
		}
	}
}
