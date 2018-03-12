using System;
using Eto.Forms;
using Eto;

namespace NGSim
{
	public static class Program
	{
        [STAThread]
		public static void Main(string[] args)
		{
			var pf = Platform.Detect;
			if (pf.IsWpf)
			{
				Console.WriteLine("Using wpf...");
				pf.Add(typeof(StateInfoTextArea), () => new StateInfoTextAreaHandler());
				pf.Add(typeof(NetworkInfoTextArea), () => new NetworkInfoTextAreaHandler());
			} else
			{
				Console.WriteLine("Platform not supported...");
				return;
			}

			UpdateManager.Initialize();

			SimManagerWindow mainWindow = new SimManagerWindow();

			mainWindow.Shown += (sender, e) => { UpdateManager.LaunchThread(); };
			mainWindow.Closing += (sender, e) => { UpdateManager.CloseThread(); };

			Application app = new Application();
			app.Run(mainWindow);
		}
	}
}
