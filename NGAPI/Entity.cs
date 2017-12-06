using System;

namespace NGAPI
{
	public abstract class Entity
	{
		public Position Position { get; internal set; }
		public float CurrentHeading { get; internal set; }
		public float TargetHeading { get; internal set; }
		public float CurrentSpeed { get; internal set; }
		public float TargetSpeed { get; internal set; }
		public Direction MoveDirection { get; internal set; }

		protected Entity() :
			this(Position.Zero)
		{ }

		protected Entity(Position pos)
		{
			Position = pos;
		}

		public void UpdateHeading()
		{
			// TODO: reimplement using the new non-Heading code
		}
	}
}
