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

			MainWindow window = new MainWindow();
			window.Shown += (sender, e) => { UpdateManager.LaunchThread(); };
			window.Closing += (sender, e) => { UpdateManager.CloseThread(); };

			app.Run(window);
		}
	}
}
