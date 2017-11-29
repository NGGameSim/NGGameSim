using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGAPI
{
    class EntityUtility<T>
    {
        //T will be expected to be either a Tank or UAV
        static Type type = typeof(T);
        
        //Returns a float to be used as real world units when translating objects
        //Floats should be seeen as number of units travelled per iteration (10 per second)
        public static float SpeedToUnits(Speeds gameSpeed)
        {
            if(!Enum.IsDefined(typeof(Speeds),gameSpeed))
            {
                throw new Exception("Invalid Speed");
            }

            //Using real world RQ-4 Global Hawk speeds as reference
            if (type == typeof(UAV))
            {
                if (gameSpeed == Speeds.Low)
                {
                    return 7.0f;
                }
                else if (gameSpeed == Speeds.Med)
                {
                    return 13.5f;
                }
                else
                {
                    return 17.5f;
                }
            }

            //Same as above, uses real world speeds for M1 Abrams
            else if (type == typeof(Tank))
            {
                if (gameSpeed == Speeds.Low)
                {
                    return 0.5f;
                }
                else if (gameSpeed == Speeds.Med)
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

        public static int SpeedToDegrees(Speeds gameSpeed, Directions direction, Headings currentHeading, Headings targetHeading)
        {
            int difference;
            int turns;
            int adjustment; //adjustment on number of turns depending on entity being referenced

            //Calculate how far the user is trying to turn
            if (direction == Directions.Left)
            {
                if ((int)currentHeading > (int)targetHeading)
                {
                    difference = (int)currentHeading - (int)targetHeading;
                }
                else if ((int)currentHeading < (int)targetHeading)
                {
                    difference = (int)currentHeading + (360 - (int)targetHeading);
                }
                else { difference = 0; }
            }
            else
            {
                if ((int)currentHeading < (int)targetHeading)
                {
                    difference = (int)targetHeading - (int)currentHeading;
                }
                else if ((int)currentHeading > (int)targetHeading)
                {
                    difference = (360 - (int)currentHeading) + (int)targetHeading;
                }
                else { difference = 0; }
            }

            //Determine adjustment values
            if (type == typeof(UAV)) { adjustment = 0; }
            else if (type == typeof(Tank)) { adjustment = 1; }
            else { throw new Exception("Invalid Entity Type"); }

            //Calculate how many turns the user needs to complete the turn with adjustment
            if (gameSpeed == Speeds.High)
            {
                if (difference < 45) { turns = 3; }
                else if (difference > 45 && difference < 90) { turns = 4  - adjustment; }
                else if (difference > 90 && difference < 135) { turns = 5 - adjustment; }
                else if (difference > 135 && difference < 180) { turns = 6 - adjustment; }
                else if (difference > 180 && difference < 225) { turns = 7 - adjustment; }
                else if (difference > 225 && difference < 270) { turns = 8 - adjustment; }
                else if (difference > 270 && difference < 315) { turns = 9 - adjustment; }
                else { turns = 10 - adjustment; }
            }
            else if (gameSpeed == Speeds.Med)
            {
                if (difference < 45) { turns = 2 - adjustment; }
                else if (difference > 45 && difference < 90) { turns = 3 - adjustment; }
                else  if (difference > 90 && difference < 135) { turns = 4 - adjustment; }
                else if (difference > 135 && difference < 180) { turns = 5 - adjustment; }
                else if (difference > 180 && difference < 225) { turns = 6 - adjustment; }
                else if (difference > 225 && difference < 270) { turns = 7 - adjustment; }
                else if (difference > 270 && difference < 315) { turns = 8 - adjustment; }
                else { turns = 9 - adjustment; }
            }
            else if (gameSpeed == Speeds.Low)
            {
                if (difference < 45) { turns = 1 - adjustment; }
                else if (difference > 45 && difference < 90) { turns = 2 - adjustment; }
                else if (difference > 90 && difference < 135) { turns = 3 - adjustment; }
                else if (difference > 135 && difference < 180) { turns = 4 - adjustment; }
                else if (difference > 180 && difference < 225) { turns = 5 - adjustment; }
                else if (difference > 225 && difference < 270) { turns = 6 - adjustment; }
                else if (difference > 270 && difference < 315) { turns = 7 - adjustment; }
                else { turns = 8 - adjustment; }
            }
            else { throw new Exception("Invalid Speed"); }

            //Calculate how many degrees the user can turn per iteration
            return difference / turns;
        }

        public EntityUtility()
            : base()
        {

        }
    }
}
