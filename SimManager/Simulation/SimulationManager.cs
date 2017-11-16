using System;

namespace NGSim.Simulation
{
	// On the server-side, the simulation manager is reponsible for quite a lot, including updating the algorithms,
	// performing entity and game state logic updates, and dispatching state packets to all of the connected clients.
	public class SimulationManager
	{
		internal NGAPI.Simulation Simulation { get; private set; }

		public SimulationManager()
		{
			Simulation = new NGAPI.Simulation();
		}

		public void Update()
		{

		}
	}
}
