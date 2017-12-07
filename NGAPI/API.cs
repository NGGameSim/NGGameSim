using System;

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

		//Get Functions for Tank
		public static Position GetTankPosition() { return FriendlyTank.Position; }
		public static float GetTankSpeed() { return FriendlyTank.CurrentSpeed; }
		public static float GetTankHeading() { return FriendlyTank.CurrentHeading; }
		public static int GetRemainingMissiles() { return FriendlyTank.MisslesLeft; }

		//Get Functions for UAV
		public static Position GetUAVPosition() { return FriendlyUAV.Position; }
		public static float GetUAVSpeed() { return FriendlyUAV.CurrentSpeed; }
		public static float GetUAVHeading() { return FriendlyUAV.CurrentHeading; }

		//Set Functions for UAV
		public static bool SetUAVSpeed(float targetSpeed)
		{
			if (targetSpeed < 7.0f || targetSpeed > 17.5f)
				return false;
			FriendlyUAV.TargetSpeed = targetSpeed;
			return true;
		}
		public static void SetUAVHeading(float targetHeading)
		{
			targetHeading %= 360.0f;
			FriendlyUAV.TargetHeading = targetHeading;
		}
		
		//Set Functions for Tank
		public static bool SetTankSpeed(float targetSpeed)
		{
			if (targetSpeed < 0.0f || targetSpeed > 2.0f)
				return false;
			FriendlyTank.TargetSpeed = targetSpeed;
			return true;
		}
		public static void SetTankHeading(float targetHeading)
		{
			targetHeading %= 360.0f;
			FriendlyTank.TargetHeading = targetHeading;
		}

		//Detection functions
		public static Position GetLastKnownPosition() { return FriendlyUAV.LastKnownPosition; }
		public static bool DetectedThisTurn() { return FriendlyUAV.DetectedTankThisTurn; }

		//Returns true if missile was fired, false otherwise
		public static bool Fire(Position Target)
		{
			if (CanFire(Target)) { FriendlyTank.FiresThisTurn = true; }
			else { FriendlyTank.FiresThisTurn = false; }
			return FriendlyTank.FiresThisTurn;
		}
		//Checks to see if the tank is allowed to fire at a given target
		public static bool CanFire(Position Target)
		{
			if (FriendlyTank.MisslesLeft <= 0) { return false; }
			else if (FriendlyTank.Position.DistanceTo(Target) > 4000) { return false; }
			else if (FriendlyTank.Cooldown != 0) { return false; }
			else { return true; }
		}


	}
}
