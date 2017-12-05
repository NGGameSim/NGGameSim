using System;

namespace NGAPI
{
	public struct Position
	{
		public static readonly Position Zero = new Position(0, 0);
		public static readonly Position North = new Position(0, -1);
		public static readonly Position East = new Position(1, 0);
		public static readonly Position South = new Position(0, 1);
		public static readonly Position West = new Position(-1, 0);
		public static readonly Position NorthEast = (North + East).Normalized();
		public static readonly Position SouthEast = (South + East).Normalized();
		public static readonly Position SouthWest = (South + West).Normalized();
		public static readonly Position NorthWest = (North + West).Normalized();

		public float X;
		public float Y;

		public Position(float x, float y)
		{
			X = x;
			Y = x;
		}

		public float Length()
		{
			return (float)Math.Sqrt(X * X + Y * Y);
		}

		public float LengthSquared()
		{
			return X * X + Y * Y;
		}

		public float DistanceTo(Position other)
		{
			return (this - other).Length();
		}

		public Position Normalized()
		{
			return (this / Length());
		}

		public void MoveWithSpeed(float direction, float speed, float time)
		{
			float speedX = speed*(float)Math.Cos(Math.PI / 180 * direction);
			float speedY = speed*(float)Math.Sin(Math.PI / 180 * direction);
			X += speedX * time;
			Y += speedY * time;
		}

		public static Position operator + (Position l, Position r)
		{
			return new Position(l.X + r.X, l.Y + r.Y);
		}

		public static Position operator - (Position l, Position r)
		{
			return new Position(l.X - r.X, l.Y - r.Y);
		}

		public static Position operator - (Position pos)
		{
			return new Position(-pos.X, -pos.Y);
		}

		public static Position operator * (Position pos, float f)
		{
			return new Position(pos.X * f, pos.Y * f);
		}

		public static Position operator * (float f, Position pos)
		{
			return new Position(pos.X * f, pos.Y * f);
		}

		public static Position operator / (Position pos, float f)
		{
			return new Position(pos.X / f, pos.Y / f);
		}
	}
}
