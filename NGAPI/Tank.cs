using System;

namespace NGAPI
{
    public class Tank : Entity
    {
		public int MisslesLeft { get; internal set; } = 0;
		public bool Alive { get; internal set; } = true;
        public Headings currentHeading { get; internal set; }
        public Headings targetHeading { get; internal set; }
        public Speeds currentSpeed { get; internal set; }
        public Speeds targetSpeed { get; internal set; }

        public Tank() :
			base()
		{

		}
    }
}
