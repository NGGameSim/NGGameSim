using System;
using System.Threading;
using System.Diagnostics;
using NGSim.Simulation;
using NGSim.Network;

namespace NGSim
{
	// This class manages the 10 UPS update loop that runs on a background thread behind the admin interface. Because
	// the interface is a GUI, we cannot do the updates on the main thread, as it will lock up 
	public static class UpdateManager
	{
		public static readonly double UPDATE_RATE = 100.0; // The number of milliseconds between each update

		private static bool shouldClose = false;
		private static readonly object closeLock = new object();
		private static Thread updateThread = null;

		public static int UpdateCount { get; private set; } = 0; // How many updates have been completed
		public static int CurrentUpdate { get { return UpdateCount + 1; } } // The current update cycle number

		public static SimulationManager SimManager { get; private set; } = null;
		public static Server Server { get; private set; } = null;

		public static void Initialize()
		{
			SimManager = new SimulationManager();
			Server = new Server();
		}

		public static void LaunchThread()
		{
			if (updateThread == null)
			{
				updateThread = new Thread(threadUpdateFunction);
				updateThread.Start(); 
			}
		}

		public static void CloseThread()
		{
			lock (closeLock)
			{
				shouldClose = true;
			}
			updateThread.Join(); // Should never have to wait for more than a couple 1/10ths of a second
		}

		private static void threadUpdateFunction()
		{
			// Do all of the startup
			Server.StartServer();

			Stopwatch stopwatch = Stopwatch.StartNew(); // This tracks the time since the last update
			while (true)
			{
				lock (closeLock)
				{
					if (shouldClose)
						break;
				}

				if (stopwatch.Elapsed.TotalMilliseconds >= UPDATE_RATE)
				{
					// Perform all of the update logic that should take place at 10UPS
					SimManager.Update();
					Server.Update();

					stopwatch.Restart(); // Restart the count to the next update
					++UpdateCount;
				}

				Thread.Sleep(5); // Wait for 5ms, then check again if it should update
			}

			// Do all of the shutdown
			Server.StopServer();
		}
	}
}
