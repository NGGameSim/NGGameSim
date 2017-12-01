using System;

namespace NGAPI
{
    public class Tank : Entity
    {
		public int MisslesLeft { get; internal set; } = 0;
		public bool Alive { get; internal set; } = true;

        public Tank() :
			base()
		{

		}

		public float GetSpeed()
		{
			if (CurrentSpeed == Speed.Low)
				return 4F;
			else if (CurrentSpeed == Speed.Med)
				return 8F;
			else if (CurrentSpeed == Speed.High)
				return 13.4F;
			return 0F;	//should never happen
		}

		public void ChangeSpeed()
		{
			if (CurrentSpeed == Speed.Low && TargetSpeed != Speed.Low)
				CurrentSpeed = Speed.Med;
			else if (CurrentSpeed == Speed.Med && TargetSpeed != Speed.Med)
				CurrentSpeed = TargetSpeed;
			else if (CurrentSpeed == Speed.High && TargetSpeed == Speed.Med)
				CurrentSpeed = Speed.Med;
			else if (CurrentSpeed == Speed.High && TargetSpeed == Speed.Low)
				CurrentSpeed = Speed.Low;
		}
	}
}
