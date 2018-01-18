using System;
using static NGAPI.Constants;

namespace NGAPI
{
	public static class API
	{
		// Because both alogorithms share the same API "instance", we need to differentiate which team is currently
		// updating so that we can get the correct "Friendly" entities. The first team is given as 1 and the second as 2.
		internal static int CurrentTeam;

		// This gets populated my SimulationManager in SimManager
		internal static Simulation Simulation = null;
		internal static UAV FriendlyUAV
		{ get { return (CurrentTeam == 1) ? Simulation.Team1.UAV : Simulation.Team2.UAV; } }
		internal static Tank FriendlyTank
		{ get { return (CurrentTeam == 1) ? Simulation.Team1.Tank : Simulation.Team2.Tank; } }
		internal static UAV EnemyUAV
		{ get { return (CurrentTeam == 1) ? Simulation.Team2.UAV : Simulation.Team1.UAV; } }
		internal static Tank EnemyTank
		{ get { return (CurrentTeam == 1) ? Simulation.Team2.Tank : Simulation.Team1.Tank; } }

		///Gets the tank's position in meters from the center in the X and Y directions.
		public static Position GetTankPosition() { return FriendlyTank.Position; }

		///Gets the tank's speed in meters per second.
		public static float GetTankSpeed() { return FriendlyTank.CurrentSpeed; }

		///Gets the tank's heading in degrees.
		public static float GetTankHeading() { return FriendlyTank.CurrentHeading; }

		///Gets the number of remaining missiles.
		public static int GetRemainingMissiles() { return FriendlyTank.MisslesLeft; }

		///Gets the UAV's position in meters from the center in the X and Y directions.
		public static Position GetUAVPosition() { return FriendlyUAV.Position; }

		///Gets the UAV's speed in meters per second.
		public static float GetUAVSpeed() { return FriendlyUAV.CurrentSpeed; }

		///Gets the UAV's heading in degrees.
		public static float GetUAVHeading() { return FriendlyUAV.CurrentHeading; }

		///Set the sped for the for UAV in meters per second. @param targetSpeed: Any speed from 7 to 26 meters per second
		public static bool SetUAVSpeed(float targetSpeed)
		{
			if (targetSpeed < minUAVSpeed || targetSpeed > maxUAVSpeed)
				return false;
			FriendlyUAV.TargetSpeed = targetSpeed;
			return true;
		}

		//TODO: Add direction parameter to this function
		///Sets the UAV's heading(angle in which it is pointing) in degrees. @param targetHeading: What you direction you want the UAV to turn toward. This can be any number, including negative numbers and numbers above 360. @param direction: In which direction you want it to turn torward your target heading
		public static void SetUAVHeading(float targetHeading)
		{
			targetHeading %= 360.0f;
			FriendlyUAV.TargetHeading = targetHeading;
		}

		///Sets the tank's speed in meters per second. @param targetSpeed: Any speed from 0 to 13 meters per second
		public static bool SetTankSpeed(float targetSpeed)
		{
			if (targetSpeed < 0.0f || targetSpeed > maxTankSpeed)
				return false;
			FriendlyTank.TargetSpeed = targetSpeed;
			return true;
		}

		//TODO: Add direction parameter to this function
		///Sets the tank's heading(angle in which it is pointing) in degrees. @param targetHeading: What you direction you want the tank to turn toward. This can be any number, including negative numbers and numbers above 360. @param direction: In which direction you want it to go turn torward your target heading
		public static void SetTankHeading(float targetHeading)
		{
			targetHeading %= 360.0f;
			FriendlyTank.TargetHeading = targetHeading;
		}

		///Gets the last known position of the enemy tank, if DetectedThisTurn returns true, this is the current position of the enemy tank.
		public static Position GetLastKnownPosition() { return FriendlyUAV.LastKnownPosition; }

		///Returns true if the enemy tank was detected in this turn.
		public static bool DetectedThisTurn() { return FriendlyUAV.DetectedTankThisTurn; }

		///Returns true if missile was fired, false otherwise
		public static bool Fire(Position Target)
		{
			if (CanFire(Target))
			{
			  FriendlyTank.FiresThisTurn = true;
			  FriendlyTank.MissileTarget = Target;
			}
			else { FriendlyTank.FiresThisTurn = false; }
			return FriendlyTank.FiresThisTurn;
		}

		///Checks to see if the tank is allowed to fire at a given target
		public static bool CanFire(Position Target)
		{
			if (FriendlyTank.MisslesLeft <= 0) { return false; }
			else if (FriendlyTank.Position.DistanceTo(Target) > firingRange) { return false; }
			else if (FriendlyTank.Cooldown != 0) { return false; }
			else { return true; }
		}

		//TODO: Implement these.

		/// <summary>
		/// Returns a random integer between 0(inclusive) and max(exclusive). So, GetRandomInteger(2) will return either 0 or 1. Trying to call Random yourself every turn may lead to non-random behavior.
		/// </summary>
		/// <param name="max">Positive integer greater than 1</param>
		public static int GetRandomInteger(int max)
		{
			return 0;
		}

		/// <summary>
		/// Returns the number of turns since the first turn, if called at the first turn this will return 0.
		/// </summary>
		public static int GetNumberOfTurnsSinceStart()
		{
			return 0;
		}
	}
}
