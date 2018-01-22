using System;
using NGAPI;

namespace NGSim
{
    class StupidAlgorithm1 : Algorithm
    {
		int lastChange = 30;
		Random rnd = new Random();

		public override void Update()
		{

			Position CurrentTankPosition = API.GetTankPosition();
			float CurrentTankHeading = API.GetTankHeading();
			float CurrentTankSpeed = API.GetTankSpeed();

			Position CurrentUAVPosition = API.GetUAVPosition();
			float CurrentUAVHeading = API.GetUAVHeading();
			float CurrentUAVSpeed = API.GetUAVSpeed();

			if (lastChange-- == 0)
			{
				float newTankHeading = API.GetRandomInteger(360);
				float newUAVHeading = API.GetRandomInteger(360);
				float newTankSpeed = API.GetRandomInteger(14);
				float newUAVSpeed = 7 + API.GetRandomInteger(20);
				API.SetTankHeading(newTankHeading);
				API.SetTankSpeed(newTankSpeed);
				API.SetUAVHeading(newUAVHeading);
				API.SetUAVSpeed(newUAVSpeed);
				lastChange = 30;
			}

			
			
			if(API.FriendlyUAV.DetectedTankThisTurn == true)
			{
				API.Fire(API.GetLastKnownPosition());
			}

		}
    }
}
