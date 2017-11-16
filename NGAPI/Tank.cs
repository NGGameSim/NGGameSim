using System;

namespace NGAPI
{
    public class Tank : Entity
    {
		public int MisslesLeft { get; internal set; } = 0;
		public bool Alive { get; internal set; } = true;

		public Tank() :
			base()
		{

		}
    }
}
