using System;

namespace NGAPI
{
    public static class EntityUtility
    {
        private static readonly Type TankType = typeof(Tank);
		private static readonly Type UAVType = typeof(UAV);
        
        //Returns a float to be used as real world units when translating objects
        //Floats should be seeen as number of units travelled per iteration (10 per second)
        public static float SpeedToUnits(Speed gameSpeed, Type type)
        {
            if(!Enum.IsDefined(typeof(Speed),gameSpeed))
            {
                throw new Exception("Invalid Speed");
            }

            //Using real world RQ-4 Global Hawk speeds as reference
            if (type == UAVType)
            {
                if (gameSpeed == Speed.Low)
                {
                    return 7.0f;
                }
                else if (gameSpeed == Speed.Med)
                {
                    return 13.5f;
                }
                else
                {
                    return 17.5f;
                }
            }

            //Same as above, uses real world speeds for M1 Abrams
            else if (type == TankType)
            {
                if (gameSpeed == Speed.Low)
                {
                    return 0.5f;
                }
                else if (gameSpeed == Speed.Med)
                {
                    return 1.0f;
                }
                else
                {
                    return 2.0f;
                }
            }

            //Handling for trying to convert for something other than tank or UAV
            else
            {
                throw new Exception("Invalid Entity Type");
            }
        }

        public static int SpeedToDegrees(Speed gameSpeed, Direction direction, int currentHeading, int targetHeading, Type type)
        {
            int difference;
            int turns;
            int adjustment; //adjustment on number of turns depending on entity being referenced

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

        //    //Determine adjustment values
        //    if (type == UAVType) { adjustment = 0; }
        //    else if (type == TankType) { adjustment = 1; }
        //    else { throw new Exception("Invalid Entity Type"); }

        //    //Calculate how many turns the user needs to complete the turn with adjustment
        //    if (gameSpeed == Speed.High)
        //    {
        //        if (difference < 45) { turns = 3; }
        //        else if (difference > 45 && difference < 90) { turns = 4  - adjustment; }
        //        else if (difference > 90 && difference < 135) { turns = 5 - adjustment; }
        //        else if (difference > 135 && difference < 180) { turns = 6 - adjustment; }
        //        else if (difference > 180 && difference < 225) { turns = 7 - adjustment; }
        //        else if (difference > 225 && difference < 270) { turns = 8 - adjustment; }
        //        else if (difference > 270 && difference < 315) { turns = 9 - adjustment; }
        //        else { turns = 10 - adjustment; }
        //    }
        //    else if (gameSpeed == Speed.Med)
        //    {
        //        if (difference < 45) { turns = 2 - adjustment; }
        //        else if (difference > 45 && difference < 90) { turns = 3 - adjustment; }
        //        else  if (difference > 90 && difference < 135) { turns = 4 - adjustment; }
        //        else if (difference > 135 && difference < 180) { turns = 5 - adjustment; }
        //        else if (difference > 180 && difference < 225) { turns = 6 - adjustment; }
        //        else if (difference > 225 && difference < 270) { turns = 7 - adjustment; }
        //        else if (difference > 270 && difference < 315) { turns = 8 - adjustment; }
        //        else { turns = 9 - adjustment; }
        //    }
        //    else if (gameSpeed == Speed.Low)
        //    {
        //        if (difference < 45) { turns = 1 - adjustment; }
        //        else if (difference > 45 && difference < 90) { turns = 2 - adjustment; }
        //        else if (difference > 90 && difference < 135) { turns = 3 - adjustment; }
        //        else if (difference > 135 && difference < 180) { turns = 4 - adjustment; }
        //        else if (difference > 180 && difference < 225) { turns = 5 - adjustment; }
        //        else if (difference > 225 && difference < 270) { turns = 6 - adjustment; }
        //        else if (difference > 270 && difference < 315) { turns = 7 - adjustment; }
        //        else { turns = 8 - adjustment; }
        //    }
        //    else { throw new Exception("Invalid Speed"); }

        //    //Calculate how many degrees the user can turn per iteration
        //    return difference / turns;
        //}
    }
}
