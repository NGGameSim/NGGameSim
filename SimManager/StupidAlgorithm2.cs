using System;
using NGAPI;

namespace NGSim
{
	class StupidAlgorithm2 : Algorithm
	{
		int lastChange = 30;

		public override void Update()
		{
			Position TankPosition = API.GetTankPosition();
			float TankHeading = API.GetTankHeading();
			float TankSpeed = API.GetTankSpeed();

			Position UAVPosition = API.GetUAVPosition();
			float UAVHeading = API.GetUAVHeading();
			float UAVSpeed = API.GetUAVSpeed();

			Position Origin = new Position(0, 0);

			if (lastChange-- == 0)
			{
				TankHeading = API.GetRandomInteger(360);
				UAVHeading = API.GetRandomInteger(360);
				TankSpeed = API.GetRandomInteger(14);
				UAVSpeed = 7 + API.GetRandomInteger(20);
				API.SetTankHeading(TankHeading);
				API.SetTankSpeed(TankSpeed);
				API.SetUAVHeading(UAVHeading);
				API.SetUAVSpeed(UAVSpeed);
				lastChange = 30;
			}

			//Check if tank is heading out of bounds
			/*Position tankDirection = new Position((float)Math.Cos(TankHeading * Math.PI / 180), (float)Math.Sin(TankHeading * Math.PI / 180));
			Position headingToPositionTank = TankPosition + TankSpeed * tankDirection;
			while (headingToPositionTank.X >= 500 || headingToPositionTank.X <= -500 || headingToPositionTank.Y >= 500 || headingToPositionTank.Y <= -500)
			{
				TankHeading = API.GetRandomInteger(360);
				API.SetTankHeading(TankHeading);
				tankDirection = new Position((float)Math.Cos(TankHeading * Math.PI / 180), (float)Math.Sin(TankHeading * Math.PI / 180));
				headingToPositionTank = TankPosition + TankSpeed * tankDirection;

			//Check if UAV is heading out of bounds
			Position UAVDirection = new Position((float)Math.Cos(UAVHeading * Math.PI / 180), (float)Math.Sin(UAVHeading * Math.PI / 180));
			Position headingToPositionUAV = UAVPosition + UAVSpeed * UAVDirection;
			while (headingToPositionUAV.X >= 500 || headingToPositionUAV.X <= -500 || headingToPositionUAV.Y >= 500 || headingToPositionUAV.Y <= -500)
			{
				UAVHeading = API.GetRandomInteger(360);
				API.SetUAVHeading(UAVHeading);
				UAVDirection = new Position((float)Math.Cos(UAVHeading * Math.PI / 180), (float)Math.Sin(UAVHeading * Math.PI / 180));
				headingToPositionUAV = UAVPosition + UAVSpeed * UAVDirection;
			}*/

			API.Fire(Origin);
		}
	}
}