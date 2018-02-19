using System;
using NGAPI;

namespace NGSim
{
	class StupidAlgorithm2 : Algorithm
	{
		int lastChange = 30;

		public override void Update()
		{
			Position CurrentTankPosition = API.GetTankPosition();
			float CurrentTankHeading = API.GetTankHeading();
			float CurrentTankSpeed = API.GetTankSpeed();

			Position CurrentUAVPosition = API.GetUAVPosition();
			float CurrentUAVHeading = API.GetUAVHeading();
			float CurrentUAVSpeed = API.GetUAVSpeed();

            Position Origin = new Position(0, 0);

			if (lastChange-- == 0)
			{
				float newTankHeading = API.GetRandomInteger(360);
				float newUAVHeading = API.GetRandomInteger(360);
				float newTankSpeed = API.GetRandomInteger(14);
				float newUAVSpeed = 7 + API.GetRandomInteger(20);
				
                int xlim = 10000 / 3;
                int ylim = 10000 / 3;

                float newXTank = CurrentTankPosition.X + CurrentTankSpeed * (float)Math.Sin(CurrentTankHeading);
                float newYTank = CurrentTankPosition.Y + CurrentTankSpeed * (float)Math.Cos(CurrentTankHeading);

                float newXUAV = CurrentUAVPosition.X + CurrentUAVSpeed * (float)Math.Sin(CurrentUAVHeading);
                float newYUAV = CurrentUAVPosition.Y + CurrentUAVSpeed * (float)Math.Cos(CurrentUAVHeading);

                while (CurrentTankPosition.X + newXTank > xlim && CurrentTankPosition.X + newXTank < -xlim && CurrentTankPosition.Y + newYTank > ylim && CurrentTankPosition.Y + newYTank < -ylim)
                {
                    newTankHeading = API.GetRandomInteger(360);
                    newTankSpeed = API.GetRandomInteger(14);

                    newXTank = CurrentTankPosition.X + CurrentTankSpeed * (float)Math.Sin(CurrentTankHeading);
                    newYTank = CurrentTankPosition.Y + CurrentTankSpeed * (float)Math.Cos(CurrentTankHeading);
                }

                while (CurrentUAVPosition.X + newXUAV > xlim && CurrentUAVPosition.X + newXUAV < -xlim && CurrentUAVPosition.Y + newYUAV > ylim && CurrentUAVPosition.Y + newYUAV < -ylim)
                {
                    newUAVHeading = API.GetRandomInteger(360);
                    newUAVSpeed = 7 + API.GetRandomInteger(20);

                    newXUAV = CurrentUAVPosition.X + CurrentUAVSpeed * (float)Math.Sin(CurrentUAVHeading);
                    newYUAV = CurrentUAVPosition.Y + CurrentUAVSpeed * (float)Math.Cos(CurrentUAVHeading);
                }

                API.SetTankHeading(newTankHeading);
                API.SetTankSpeed(newTankSpeed);
                API.SetUAVHeading(newUAVHeading);
                API.SetUAVSpeed(newUAVSpeed);
                lastChange = 30;
			}

            API.Fire(Origin);

			/*if (API.DetectedThisTurn())
			{
				API.Fire(API.GetLastKnownPosition());
			}*/
		}
	}
}