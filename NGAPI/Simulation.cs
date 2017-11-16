using System;

namespace NGAPI
{
    internal class Simulation
    {
		public Team Team1 { get; private set; }
		public Team Team2 { get; private set; }

		public Simulation()
		{
			Team1 = new Team();
			Team2 = new Team();
		}
    }
}
