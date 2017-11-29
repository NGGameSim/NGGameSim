using System;

namespace NGAPI
{
	public class UAV : Entity
	{
		public int ViewRadius;
        public Headings currentHeading { get; internal set; }
        public Headings targetHeading { get; internal set; }
        public Speeds currentSpeed { get; internal set; }
        public Speeds targetSpeed { get; internal set; }

        public UAV() :
			base()
		{

		}
	}
}
