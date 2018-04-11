using System;

namespace NGAPI
{
	internal static class Constants
	{
		// Scan angle of the UAVs in radians
		internal const float UAVScanAngle = (float)(15 * Math.PI / 180);
		// The height that the uav is rendered at in the visualizer
		internal const float UAVRenderHeight = 20f;
		// The firing range of the tank
		internal const int TankFiringRange = 400;

		internal const int MaxTurns = 10000;   //maximum number of turns in one game 
		internal static readonly Position WorldSize = new Position(2000, 2000); // The size of the world, the valid place for entities to stay, centered on 0, 0
		internal const int BoomRange = 22;  //meters, 22 meters is effective blast radius of 120mm cannon on M1 Abrams
		internal const float UAVAltitude = 1500;

		internal const float MaxUAVSpeed = 26f;  // meters per second
		internal const float MinUAVSpeed = 7f;  // meters per second
		internal const float MaxTankSpeed = 13f;  // meters per second

		//These are not used anywhere in the code, but should be in the future
		internal const int MissilesTankCanFireInOneTurn = 1;
		internal const int MissileAmmoCapacity = 1;
		
		internal const float MaxUAVAcceleration = 1f;  // meters per second per second
		internal const float MaxTankAcceleration = 1f; // meters per second per second
		internal const float MaxUAVTurningSpeed = 1f; //angles per second
		internal const float MaxTankTurningSpeed = 1f; //angles per second
		internal const float UAVScanRange = 100f;  //meters
		internal const float TankScanRange = 10f;
		internal const float MissileSpeed = 1f; // meters per second
	}
}
