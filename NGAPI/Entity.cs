using System;

namespace NGAPI
{
	public abstract class Entity
	{
		public Position Position { get; internal set; }
		public Heading CurrentHeading { get; internal set; }
		public Heading TargetHeading { get; internal set; }
		public Speed CurrentSpeed { get; internal set; }
		public Speed TargetSpeed { get; internal set; }
		public Direction MoveDirection { get; internal set; }

		protected Entity() :
			this(Position.Zero)
		{ }

		protected Entity(Position pos)
		{
			Position = pos;
		}

		public void ChangeHeading()
		{
			if(MoveDirection == Direction.Left && TargetHeading != CurrentHeading) {
				CurrentHeading = (Heading)(((int)CurrentHeading + 45) % 350);
			}
			else if (MoveDirection == Direction.Right && TargetHeading != CurrentHeading)
			{
				CurrentHeading = (Heading)(((int)CurrentHeading - 45) % 350);
			}
		}
	}
}
