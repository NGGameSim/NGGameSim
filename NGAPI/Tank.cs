using System;

namespace NGAPI
{
	internal class Tank : Entity
	{
		public int MisslesLeft { get; internal set; } = 0;
		public bool Alive { get; internal set; } = true;
		public bool FiresThisTurn { get; internal set; }
		public int Cooldown { get; internal set; }

		public Tank() :
			base()
		{

		}
	}
}
