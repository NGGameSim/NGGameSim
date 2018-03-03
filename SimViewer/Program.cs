using System;
using Eto.Forms;
using Eto.Wpf;
using NLog;

namespace NGSim
{
	static class Program
	{
        private static Logger logger = LogManager.GetCurrentClassLogger();

		[STAThread]
		public static void Main(string[] args)
		{
			var pf = Platform.Detect;
			if (pf.IsWpf)
			{
				Console.WriteLine("Using wpf...");
				pf.Add(typeof(StateInfoTextArea), () => new StateInfoTextAreaHandler());
			}
			else
			{
				Console.WriteLine("Platform not supported...");
				return;
			}

			logger.Fatal("Sample fatal error message");

			Application app = new Application();

			SimViewerWindow mainWindow = new SimViewerWindow();
			mainWindow.Show();

			using (SimViewer game = new SimViewer())
			game.Run();
		}
	}
}
