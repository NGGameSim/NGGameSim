using System;

namespace NGAPI
{
	public class UAV : Entity
	{
		public int ViewRadius { get; internal set; }

        public UAV() :
			base()
		{

		}

		internal float GetSpeedUnits()
		{
			if (CurrentSpeed == Speed.Low)
				return 4F;
			else if (CurrentSpeed == Speed.Med)
				return 8F;
			else if (CurrentSpeed == Speed.High)
				return 13.4F;
			return 0F;  //should never happen
		}

		internal void UpdateSpeed()
		{
			if (CurrentSpeed == Speed.Low && TargetSpeed != Speed.Low)
				CurrentSpeed = Speed.Med;
			else if (CurrentSpeed == Speed.Med && TargetSpeed != Speed.Med)
				CurrentSpeed = TargetSpeed;
			else if (CurrentSpeed == Speed.High && TargetSpeed != Speed.High)
				CurrentSpeed = Speed.Med;
		}
	}
}
