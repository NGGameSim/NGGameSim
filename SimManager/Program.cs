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
			app.Run(new MainWindow());
		}
	}
}
