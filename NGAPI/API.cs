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
        public static void SetUAVSpeed(float targetSpeed)
        {
            if (targetSpeed < 7.0f || targetSpeed > 17.5f) { throw new Exception("Invalid Speed"); }
            FriendlyUAV.TargetSpeed = targetSpeed;
        }
        public static void SetUAVHeading(float targetHeading)
        {
            if (targetHeading < 0.0f || targetHeading > 360.0f) { throw new Exception("Invalid Heading"); }
            FriendlyUAV.TargetHeading = targetHeading;
        }
        
        //Set Functions for Tank
        public static void SetTankSpeed(float targetSpeed)
        {
            if (targetSpeed < 0.0f || targetSpeed > 2.0f) { throw new Exception("Invalid Speed"); }
            FriendlyTank.TargetSpeed = targetSpeed;
        }
        public static void SetTankHeading(float targetHeading)
        {
            if (targetHeading < 0.0f || targetHeading > 360.0f) { throw new Exception("Invalid Heading"); }
            FriendlyTank.TargetHeading = targetHeading;
        }

        //Detection functions
        public static Position GetLastKnownPosition()
        {
            return EnemyTank.Position; //Just a placeholder for build purposes
        }
        public static bool DetectedThisTurn()
        {
            return false;
        }

        //Returns true if missile was fired, false otherwise
        public static bool Fire(Position Target)
        {
            if (FriendlyTank.MisslesLeft <= 0) { return false; }
            else if (FriendlyTank.Position.DistanceTo(Target) > 4000) { return false; }
            //TODO: Need way to check for cooldown

            else { return false; }
        }
        public static bool CanFire()
        {
            return false;
        }


    }
}
