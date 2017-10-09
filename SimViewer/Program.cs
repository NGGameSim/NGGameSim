using System;

namespace NGSim
{
	static class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			using (SimClient game = new SimClient())
				game.Run();
		}
	}
}
