using System;

namespace NGAPI
{
	public abstract class Entity
	{
		public Position Position { get; internal set; }
		public int CurrentHeading { get; internal set; }
		public int TargetHeading { get; internal set; }
		public int CurrentSpeed { get; internal set; }
		public int TargetSpeed { get; internal set; }
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
