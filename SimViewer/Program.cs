using System;
using Eto.Forms;
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

			Application app = new Application();

			SimViewerWindow mainWindow = new SimViewerWindow();
			mainWindow.Show();

			using (SimViewer game = new SimViewer())
			game.Run();
		}
	}
}
