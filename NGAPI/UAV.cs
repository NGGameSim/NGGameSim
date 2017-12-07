using System;

namespace NGAPI
{
	public class UAV : Entity
	{
		public int ViewRadius { get; internal set; }
		public Position LastKnownPosition { get; internal set; }
		public bool DetectedThisTurn = false;  //must be set back to false ever turn

        public UAV() :
			base()
		{

		}
	}
}
