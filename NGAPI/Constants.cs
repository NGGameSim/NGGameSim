using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGAPI
{
	internal static class Constants
	{
		internal const int maxTurns = 10000;   //maximum number of turns in one game 
		internal static readonly Position WorldSize = new Position(1000, 1000); // The size of the world, the valid place for entities to stay, centered on 0, 0
		internal const int boomRange = 22;  ///meters, 22 meters is effective blast radius of 120mm cannon on M1 Abrams
		internal const int firingRange = 4000;  ///meters, 4000 meters is firing range of M1 Abrams cannon

		internal const float maxUAVSpeed = 26f;  /// meters per second
		internal const float minUAVSpeed = 7f;  /// meters per second
		internal const float maxTankSpeed = 13f;  /// meters per second

		//These are not used anywhere in the code, but should be in the future
		internal const int MissilesTankCanFireInOneTurn = 1;
		
		internal const float maxUAVAcceleration = 1f;  /// meters per second per second
		internal const float maxTankAcceleration = 1f; /// meters per second per second
		internal const float maxUAVTurningSpeed = 1f; ///angles per second
		internal const float maxTankTurningSpeed = 1f; ///angles per second
		internal const float UAVScanRange = 100f;  ///meters
        internal const float TankScanRange = 10f;
		internal const float missileSpeed = 1f; ///meters per second
	}
}
