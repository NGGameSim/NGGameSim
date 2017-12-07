using System;
using System.Collections.Generic;

namespace NGAPI
{
	internal class Team
	{
		public UAV UAV { get; private set; }
		public Tank Tank { get; private set; }
		public List<Missile> Missiles { get; private set; } // Missiles that are in the air

		public Team()
		{
			UAV = new UAV();
			Tank = new Tank();
			Missiles = new List<Missile>();
		}
	}
}
