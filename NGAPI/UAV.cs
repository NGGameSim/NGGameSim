using System;

namespace NGAPI
{
	internal class UAV : Entity
	{
		public int ViewRadius { get; internal set; } = 100;
		public Position LastKnownPosition { get; internal set; }
		public bool DetectedTankThisTurn { get; internal set; }
        public float Altitude { get; internal set; } = Constants.UAVAltitude;

		public UAV() :
			base()
		{

		}
	}
}
