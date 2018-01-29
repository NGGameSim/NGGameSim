using System;

namespace NGAPI
{
	public struct Missile
	{
		public Position Source; // The source the missile was fired from
		public Position Target; // The target position of the missile
		public Position CurrentPostion;
		public float CurrentHeading;
		public int TurnsRemaining; // The amount of turns left before the missile arrives at the target
	}
}
