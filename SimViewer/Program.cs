using System;

namespace NGSim
{
	static class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			using (SimViewer game = new SimViewer())
				game.Run();
		}
	}
}
