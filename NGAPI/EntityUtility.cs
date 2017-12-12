using System;

namespace NGAPI
{
    public static class EntityUtility
    {
        private static readonly Type TankType = typeof(Tank);
		private static readonly Type UAVType = typeof(UAV);
        
        //Returns a float to be used as real world units when translating objects
        //Floats should be seeen as number of units travelled per iteration (10 per second)
        public static float SpeedToUnits(float gameSpeed, Type type)
        {
            //Using real world RQ-4 Global Hawk speeds as reference
            //Range from 70 m/s to 175 m/s
            if (type == UAVType)
            {
                if (gameSpeed < 7.0f || gameSpeed > 17.5f)
                {
                    throw new Exception("Invalid Speed");
                }
                else { return gameSpeed; }
            }

            //Same as above, uses real world speeds for M1 Abrams
            //Range from 0.0 m/s to 20 m/s
            else if (type == TankType)
            {
                if(gameSpeed < 0.0f || gameSpeed > 2.0f)
                {
                    throw new Exception("Invalid float");
                }
                else { return gameSpeed; }
            }

            else
            {
                throw new Exception("Invalid Entity Type");
            }
        }

        public static float SpeedToDegrees(float gameSpeed, Direction direction, float currentHeading, float targetHeading, Type type)
        {
            float difference;
            float turns;
            

            //Calculate how far the user is trying to turn
            if (direction == Direction.Left)
            {
                if (currentHeading > targetHeading)
                {
                    difference = currentHeading - targetHeading;
                }
                else if (currentHeading < targetHeading)
                {
                    difference = currentHeading + (360 - (targetHeading));
                }
                else { difference = 0; }
            }
            else if (direction == Direction.Right)
            {
                if (currentHeading < targetHeading)
                {
                    difference = targetHeading - currentHeading;
                }
                else if (currentHeading > targetHeading)
                {
                    difference = (360 - currentHeading) + targetHeading;
                }
                else { difference = 0; }
            }
            else if (direction == Direction.Null) { difference = 0; }
            else { throw new Exception("Invalid Direction"); }

			if (type == UAVType)
            {
                if (gameSpeed <= 10.0f)
                {
                    if (difference < 45) { turns = 3; }
                    else if (difference > 45 && difference < 90) { turns = 4; }
                    else if (difference > 90 && difference < 135) { turns = 5; }
                    else if (difference > 135 && difference < 180) { turns = 6; }
                    else if (difference > 180 && difference < 225) { turns = 7; }
                    else if (difference > 225 && difference < 270) { turns = 8; }
                    else if (difference > 270 && difference < 315) { turns = 9; }
                    else { turns = 10; }
                }
                else if (gameSpeed > 10.0f && gameSpeed <= 13.5f)
                {
                    if (difference < 45) { turns = 2; }
                    else if (difference > 45 && difference < 90) { turns = 3; }
                    else if (difference > 90 && difference < 135) { turns = 4; }
                    else if (difference > 135 && difference < 180) { turns = 5; }
                    else if (difference > 180 && difference < 225) { turns = 6; }
                    else if (difference > 225 && difference < 270) { turns = 7; }
                    else if (difference > 270 && difference < 315) { turns = 8; }
                    else { turns = 9; }
                }
                else if (gameSpeed > 13.5f && gameSpeed <= 17.5f)
                {
                    if (difference < 45) { turns = 1; }
                    else if (difference > 45 && difference < 90) { turns = 2; }
                    else if (difference > 90 && difference < 135) { turns = 3; }
                    else if (difference > 135 && difference < 180) { turns = 4; }
                    else if (difference > 180 && difference < 225) { turns = 5; }
                    else if (difference > 225 && difference < 270) { turns = 6; }
                    else if (difference > 270 && difference < 315) { turns = 7; }
                    else { turns = 8; }
                }
                else { throw new Exception("Invalid Speed"); }
            }
            else if (type == TankType)
            {
                if (gameSpeed <= 0.75f)
                {
                    if (difference < 45) { turns = 3; }
                    else if (difference > 45 && difference < 90) { turns = 4; }
                    else if (difference > 90 && difference < 135) { turns = 5; }
                    else if (difference > 135 && difference < 180) { turns = 6; }
                    else if (difference > 180 && difference < 225) { turns = 7; }
                    else if (difference > 225 && difference < 270) { turns = 8; }
                    else if (difference > 270 && difference < 315) { turns = 9; }
                    else { turns = 10; }
                }
                else if (gameSpeed > 0.75f && gameSpeed <= 1.5f)
                {
                    if (difference < 45) { turns = 2; }
                    else if (difference > 45 && difference < 90) { turns = 3; }
                    else if (difference > 90 && difference < 135) { turns = 4; }
                    else if (difference > 135 && difference < 180) { turns = 5; }
                    else if (difference > 180 && difference < 225) { turns = 6; }
                    else if (difference > 225 && difference < 270) { turns = 7; }
                    else if (difference > 270 && difference < 315) { turns = 8; }
                    else { turns = 9; }
                }
                else if (gameSpeed > 1.5f && gameSpeed <= 2.0f)
                {
                    if (difference < 45) { turns = 1; }
                    else if (difference > 45 && difference < 90) { turns = 2; }
                    else if (difference > 90 && difference < 135) { turns = 3; }
                    else if (difference > 135 && difference < 180) { turns = 4; }
                    else if (difference > 180 && difference < 225) { turns = 5; }
                    else if (difference > 225 && difference < 270) { turns = 6; }
                    else if (difference > 270 && difference < 315) { turns = 7; }
                    else { turns = 8; }
                }
                else { throw new Exception("Invalid Speed"); }
            }
            else { throw new Exception("Invalid Type"); }

            return difference / turns;
		}
	}
}
