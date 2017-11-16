using System;
using NGAPI;

namespace NGSim
{
	// On the client side, there is no "updating", as all of the updates are managed by the server, and implemented
	// by the network manager (Client). The client simulation manager simply holds the simulation state, as well as
	// all of the code needed to render the simulation.
	public class SimulationManager
	{
		internal Simulation Simulation { get; private set; }

		public SimulationManager()
		{
			Simulation = new Simulation();
		}

		public void Render()
		{

		}
	}
}
