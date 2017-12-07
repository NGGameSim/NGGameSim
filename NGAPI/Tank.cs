using System;

namespace NGAPI
{
    public class Tank : Entity
    {
		public int MisslesLeft { get; internal set; } = 0;
		public bool Alive { get; internal set; } = true;
		public Position Missle1FiredTarget;
		public bool Missle1FiredThisTurn = false; //must be set back to false ever turn
		public int TurnsItTakesMissle1;
		public Position Missle2FiredTarget;
		public bool Missle2FiredThisTurn = false; //must be set back to false ever turn
		public int TurnsItTakesMissle2;
		public Tank() :
			base()
		{

		}
	}
}
