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

        public static Position GetTankPosition() { return FriendlyTank.Position; }
        public static int GetTankSpeed() { return FriendlyTank.CurrentSpeed; }
        public static int GetTankHeading() { return FriendlyTank.CurrentHeading; }
        public static int GetRemainingMissiles() { return FriendlyTank.MisslesLeft; }

        public static Position GetUAVPosition() { return FriendlyUAV.Position; }
        public static int GetUAVSpeed() { return FriendlyUAV.CurrentSpeed; }
        public static int GetUAVHeading() { return FriendlyUAV.CurrentHeading; }

        public static void SetUAVSpeed()
        {

        }
        public static void SetUAVHeading()
        {

        }
        
        public static void SetTankSpeed(int targetSpeed)
        {

        }
        public static void SetTankHeading(int targetHeading)
        {

        }

        public static Position GetLastKnownPosition()
        {
            return EnemyTank.Position; //Just a placeholder for build purposes
        }
        public static bool DetectedThisTurn()
        {
            return false;
        }

        public static bool Fire(Position Target)
        {
            return false;
        }
        public static bool CanFire()
        {
            return false;
        }


    }
}
