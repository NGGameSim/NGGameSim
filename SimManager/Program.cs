using System;
using Eto.Forms;
using NLog;

namespace NGSim
{
	public static class Program
	{
        private static Logger logger2 = LogManager.GetCurrentClassLogger();
		[STAThread]
		public static void Main(string[] args)
		{
            logger2.Fatal("Sample fatal error message. Yes, it's really working this time. You can relax.");
            
			Application app = new Application();
			app.Run(new MainWindow());
		}
	}
}
