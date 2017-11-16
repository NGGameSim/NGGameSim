using System;

namespace NGAPI
{
	public abstract class Entity
	{
		public Position Position { get; internal set; }
		
		protected Entity() :
			this(Position.Zero)
		{ }

		protected Entity(Position pos)
		{
			Position = pos;
		}
	}
}
