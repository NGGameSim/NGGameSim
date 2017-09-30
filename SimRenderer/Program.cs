using System;

namespace NGSim
{
	static class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			using (SimRenderer game = new SimRenderer())
				game.Run();
		}
	}
}
