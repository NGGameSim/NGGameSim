using System;

namespace NGAPI
{
	public class UAV : Entity
	{
		public int ViewRadius { get; internal set; }
		public Position LastKnownPosition { get; internal set; }
		public bool DetectedTankThisTurn { get; internal set; }

		public UAV() :
			base()
		{

		}
	}
}
