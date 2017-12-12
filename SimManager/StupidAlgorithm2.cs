using System;
using NGAPI;
using System.Collections.Generic;
namespace NGSim
{
    class StupidAlgorithm2 : NGAPI.Algorithm
    {
		int lastChange = 30;

        public override void Update()
		{
			Random rnd = new Random();

			Position CurrentTankPosition = API.GetTankPosition();
			float CurrentTankHeading = API.GetTankHeading();
			float CurrentTankSpeed = API.GetTankSpeed();

			Position CurrentUAVPosition = API.GetUAVPosition();
			float CurrentUAVHeading = API.GetUAVHeading();
			float CurrentUAVSpeed = API.GetUAVSpeed();

			if (lastChange-- == 0)
			{
				float newTankHeading = rnd.Next(0, 359);
				float newUAVHeading = rnd.Next(0, 359);
				float newTankSpeed = rnd.Next(0, 2);
				float newUAVSpeed = rnd.Next(7, 17);
				API.SetTankHeading(newTankHeading);
				API.SetTankSpeed(newTankSpeed);
				API.SetUAVHeading(newUAVHeading);
				API.SetUAVSpeed(newUAVSpeed);
				lastChange = 30;
			}

			if (API.FriendlyUAV.DetectedTankThisTurn == true)
			{
				API.Fire(API.GetLastKnownPosition());
			}
		}
    }
}
