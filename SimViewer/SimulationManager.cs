using System;
using NGAPI;

namespace NGSim
{
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
