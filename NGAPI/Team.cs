using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGAPI
{
	internal class Team
	{
		public UAV UAV { get; private set; }
		public Tank Tank { get; private set; }

		public Team()
		{
			UAV = new UAV();
			Tank = new Tank();
		}
	}
}
