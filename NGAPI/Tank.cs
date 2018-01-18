using System;

namespace NGAPI
{
	internal class Tank : Entity
	{
		public int MisslesLeft { get; internal set; } = 15;
		public bool Alive { get; internal set; } = true;
		public bool FiresThisTurn { get; internal set; } = false;
		public int Cooldown { get; internal set; }
		public Position MissileTarget { get; internal set; }
		
		public Tank() :
			base()
		{

		}
	}
}
