using System;
using Eto.Forms;

namespace NGSim
{
	public static class Program
	{
        [STAThread]
		public static void Main(string[] args)
		{
			Application app = new Application();

			UpdateManager.Initialize();

			SimManagerWindow mainWindow = new SimManagerWindow();

			mainWindow.Shown += (sender, e) => { UpdateManager.LaunchThread(); };
			mainWindow.Closing += (sender, e) => { UpdateManager.CloseThread(); };

			app.Run(mainWindow);
		}
	}
}
