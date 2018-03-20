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

				if (!API.IsTankTurning())
					API.SetTankHeading(TankHeading);
				API.SetTankSpeed(TankSpeed);
				if (!API.IsUAVTurning())
					API.SetUAVHeading(UAVHeading);
				API.SetUAVSpeed(UAVSpeed);
				lastChange = 30;
			}

			//Check if tank is heading out of bounds
			float ToCenterTank = 180 + (float)Math.Atan2(TankPosition.Y, TankPosition.X) * 180 / (float)Math.PI;
			float ToCenterUAV = 180 + (float)Math.Atan2(UAVPosition.Y, UAVPosition.X) * 180 / (float)Math.PI;

			if ((TankHeading < 90 || TankHeading > 270) && TankPosition.X > 400)
			{
				API.SetTankHeading(ToCenterTank);
			}

			else if ((TankHeading > 90 && TankHeading < 270) && TankPosition.X < -400)
			{
				API.SetTankHeading(ToCenterTank);
			}

			else if (TankHeading < 180 && TankPosition.Y > 400)
			{
				API.SetTankHeading(ToCenterTank);
			}

			else if (TankHeading > 180 && TankPosition.Y < -400)
			{
				API.SetTankHeading(ToCenterTank);
			}

			if ((UAVHeading < 90 || UAVHeading > 270) && UAVPosition.X > 400)
			{
				API.SetUAVHeading(ToCenterUAV);
			}

			else if ((UAVHeading > 90 && UAVHeading < 270) && UAVPosition.X < -400)
			{
				API.SetUAVHeading(ToCenterUAV);
			}

			else if (UAVHeading < 180 && UAVPosition.Y > 400)
			{
				API.SetUAVHeading(ToCenterUAV);
			}

			else if (UAVHeading > 180 && UAVPosition.Y < -400)
			{
				API.SetUAVHeading(ToCenterUAV);
			}

			API.Fire(Origin);
		}
	}
}