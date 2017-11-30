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

        public static void SetUAVHeading(Heading targetHeading)
        {
            if(!Enum.IsDefined(typeof(Heading), targetHeading))
            {
                throw new Exception("Invalid Heading");
            }
            FriendlyUAV.CurrentHeading = targetHeading;
        }

        public static void SetUAVSpeed(Speed targetSpeed)
        {
            if(!Enum.IsDefined(typeof(Speed),targetSpeed))
            {
                throw new Exception("InvalidSpeed");
            }
            FriendlyUAV.CurrentSpeed = targetSpeed;
        }

        //Returns True if Enemy Tank is within the UAVs view radius
        public static bool UAVScan()
        {
            int viewRadius = Simulation.Team1.UAV.ViewRadius;
            float distance = FriendlyUAV.Position.DistanceTo(EnemyTank.Position);

            if(distance < viewRadius) { return true; }
            else { return false; }
        }

        public static void TankSetHeading(Heading targetHeading)
        {
            if(!Enum.IsDefined(typeof(Heading),targetHeading))
            {
                throw new Exception("Invalid Heading");
            }
            FriendlyTank.CurrentHeading = targetHeading;
        }

        public static void TankSetSpeed(Speed targetSpeed)
        {
            if (!Enum.IsDefined(typeof(Speed), targetSpeed))
            {
                throw new Exception("Invalid Heading");
            }
            FriendlyTank.CurrentSpeed = targetSpeed;
        }
        
        //Return True on a Hit on the Enemy Tank
        //Return False on a Miss or a failure to fire
        public static bool Fire(Position Target)
        {
            FriendlyTank.MisslesLeft--;

            //Out of Missiles (failure to fire)
            if(FriendlyTank.MisslesLeft == 0)
            {
                //TODO: Besides an obvious miss, what do we do if we're out of missiles
                return false;
            }

            //Out of Range (Failure to Fire)
            //4000 is just a number and can be changed, but it is the real firing range of the M1 Abrams
            if(FriendlyTank.Position.DistanceTo(Target) > 4000)
            {
                return false;
            }

            //Good Hit Good Kill (Rifle Rifle Splash)
            //22 is effective blast radius of 120mm cannon on M1 Abrams
            if(EnemyTank.Position.DistanceTo(Target) < 22)
            {
                return true;
            }
            //Return False
            else
            {
                return false;
            }
        }
    }
}
