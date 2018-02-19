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

			MainWindow mainWindow = new MainWindow();
			//ClientWindow clientWindow = new ClientWindow();

			mainWindow.Show();
			//clientWindow.Show();

			mainWindow.Shown += (sender, e) => { UpdateManager.LaunchThread(); };
			mainWindow.Closing += (sender, e) => { UpdateManager.CloseThread(); };

			app.Run(mainWindow);
		}
	}
}
