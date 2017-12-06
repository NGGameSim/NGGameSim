using System;

namespace NGAPI
{
    public static class API
    {
		// This gets populated my SimulationManager in SimManager
        internal static Simulation Simulation = null;
        internal static UAV FriendlyUAV = Simulation.Team1.UAV;
        internal static Tank FriendlyTank = Simulation.Team1.Tank;
        internal static Tank EnemyTank = Simulation.Team2.Tank;
		private static float maxSpeedTank = 13;  //meters per second
		private static float maxSpeedUAV = 60;  //meters per second

		public static Position GetUAVPosition()
		{
			return FriendlyUAV.Position;
		}

		public static Position GetTankPosition()
		{
			return FriendlyTank.Position;
		}

		public static float GetUAVSpeed()
		{
			return FriendlyUAV.CurrentSpeed;
		}

		public static float GetTankSpeed()
		{
			return FriendlyTank.CurrentSpeed;
		}

		public static float GetUAVHeading()
		{
			return FriendlyUAV.CurrentHeading;
		}

		public static float GetTankHeading()
		{
			return FriendlyTank.CurrentHeading;
		}

		public static int GetRemainingMissles()
		{
			return FriendlyTank.MisslesLeft;
		}

		public static void SetUAVSpeed(float targetSpeed)
		{
			if (targetSpeed > maxSpeedUAV)
				targetSpeed = maxSpeedUAV;
			else if (targetSpeed < 0)
				targetSpeed = 0;
			FriendlyUAV.CurrentSpeed = targetSpeed;
		}

		public static void SetTankSpeed(float targetSpeed)
		{
			if (targetSpeed > maxSpeedTank)
				targetSpeed = maxSpeedTank;
			else if (targetSpeed < 0)
				targetSpeed = 0;
			FriendlyUAV.CurrentSpeed = targetSpeed;
		}

		public static void SetUAVHeading(float targetHeading, Direction moveDirection)
        {
            if(!Enum.IsDefined(typeof(Direction), moveDirection))
            {
                throw new Exception("Invalid Direction");
            }
            FriendlyUAV.TargetHeading = targetHeading;
			FriendlyUAV.MoveDirection = moveDirection;
		}

		public static void SetTankHeading(float targetHeading, Direction moveDirection)
		{
			if (!Enum.IsDefined(typeof(Direction), moveDirection))
			{
				throw new Exception("Invalid Direction");
			}
			FriendlyTank.TargetHeading = targetHeading;
			FriendlyUAV.MoveDirection = moveDirection;
		}

		//Returns True if Enemy Tank is within the UAVs view radius
		public static Position GetLastKnownPosition()
        {
            int viewRadius = FriendlyUAV.ViewRadius;
            float distance = FriendlyUAV.Position.DistanceTo(EnemyTank.Position);

            if(distance < viewRadius) {
				FriendlyUAV.LastKnownPosition = FriendlyUAV.Position;
				FriendlyUAV.DetectedThisTurn = true;
			}
			return FriendlyUAV.LastKnownPosition;
		}

		public static bool DetectedThisTurn()
		{
			return FriendlyUAV.DetectedThisTurn;
		}
        
        //Return True on a Hit on the Enemy Tank
        //Return False on a Miss or a failure to fire
        public static bool Fire(Position Target)
        {

            //Out of Missiles (failure to fire)
            if(FriendlyTank.MisslesLeft == 0)
            {
                //TODO: Besides an obvious miss, what do we do if we're out of missiles
                return false;
            }

			FriendlyTank.MisslesLeft--;

			//Out of Range (Failure to Fire)
			//4000 is just a number and can be changed, but it is the real firing range of the M1 Abrams in meters
			if (FriendlyTank.Position.DistanceTo(Target) > 4000)
            {
                return false;
            }

            if(FriendlyTank.Missle2FiredThisTurn == true)
			{
				return false;
			}

			if(FriendlyTank.Missle1FiredThisTurn == true)
			{
				FriendlyTank.Missle2FiredTarget = Target;
				FriendlyTank.Missle2FiredThisTurn = true;
				FriendlyTank.TurnsItTakesMissle2 = 1 + ((int)FriendlyTank.Position.DistanceTo(Target)) / 1000;
			}
			else
			{
				FriendlyTank.Missle1FiredTarget = Target;
				FriendlyTank.Missle1FiredThisTurn = true;
				FriendlyTank.TurnsItTakesMissle1 = 1 + ((int)FriendlyTank.Position.DistanceTo(Target)) / 1000;
			}
			return true;
		}

		public static bool CanFire()
		{
			//Out of Missiles (failure to fire)
			if (FriendlyTank.MisslesLeft == 0)
			{
				//TODO: Besides an obvious miss, what do we do if we're out of missiles
				return false;
			}

			if (FriendlyTank.Missle2FiredThisTurn == true)
			{
				return false;
			}

			return true;
		}
    }
}
