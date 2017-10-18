using System;
using NLog;

namespace NGSim
{
	static class Program
	{
        private static Logger logger = LogManager.GetCurrentClassLogger();

		[STAThread]
		public static void Main(string[] args)
		{
            logger.Fatal("Sample fatal error message");
			using (SimViewer game = new SimViewer())
			game.Run();
		}
	}
}
