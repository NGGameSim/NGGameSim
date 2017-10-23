using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NGSim.Graphics
{
	public class ArcBallCamera : Camera
	{
		#region Members
		#region Camera Position
		protected float _pitch;
		// The angle above or below the xy plane (in degrees)
		public float Pitch
		{
			get { return _pitch; }
			set
			{
				_pitch = MathHelper.Clamp(value, _pitchMin, _pitchMax);
				updatePosition();
			}
		}
		protected float _yaw;
		// The angle of rotation around the z-axis, in the xy plane (in degrees)
		public float Yaw
		{
			get { return _yaw; }
			set
			{
				_yaw = MathHelper.Clamp((value + 360.0f) % 360.0f, _yawMin, _yawMax);
				updatePosition();
			}
		}
		protected float _distance;
		// The distance from the target to the camera (in world units)
		public float Distance
		{
			get { return _distance; }
			set
			{
				_distance = MathHelper.Clamp(value, _distMin, _distMax);
				updatePosition();
			}
		}
		#endregion // Camera Position

		#region Camera Limits
		protected float _pitchMin = -89.0f;
		protected float _pitchMax = 89.0f;
		public float MinPitch
		{
			get { return _pitchMin; }
			set
			{
				_pitchMin = value;
				if (_pitchMax < _pitchMin)
					_pitchMax = _pitchMin;
				Pitch = _pitch;
			}
		}
		public float MaxPitch
		{
			get { return _pitchMax; }
			set
			{
				_pitchMax = value;
				if (_pitchMin > _pitchMax)
					_pitchMin = _pitchMax;
				Pitch = _pitch;
			}
		}
		protected float _yawMin = 0.0f;
		protected float _yawMax = 360.0f;
		public float MinYaw
		{
			get { return _yawMin; }
			set
			{
				_yawMin = MathHelper.Clamp(value, 0.0f, 360.0f);
				if (_yawMax < _yawMin)
					_yawMax = _yawMin;
				Yaw = _yaw;
			}
		}
		public float MaxYaw
		{
			get { return _yawMax; }
			set
			{
				_yawMax = MathHelper.Clamp(value, 0.0f, 360.0f);
				if (_yawMin > _yawMax)
					_yawMin = _yawMax;
				Yaw = _yaw;
			}
		}
		protected float _distMin = 0.01f;
		protected float _distMax = 1000.0f;
		public float MinDistance
		{
			get { return _distMin; }
			set
			{
				_distMin = MathHelper.Max(value, 0.0f);
				if (_distMax < _distMin)
					_distMax = _distMin;
				Distance = _distance;
			}
		}
		public float MaxDistance
		{
			get { return _distMax; }
			set
			{
				_distMax = MathHelper.Max(value, 0.0f);
				if (_distMin > _distMax)
					_distMin = _distMax;
				Distance = _distance;
			}
		}
		#endregion // Camera Limits

		public new Vector3 Target
		{
			get { return base.Target; }
			set
			{
				base.Target = value;
				updatePosition();
			}
		}
		#endregion // Members

		public ArcBallCamera(GraphicsDevice device) :
			base(device)
		{

		}

		protected virtual void updatePosition()
		{
			Vector3 diff = Vector3.Backward * _distance;
			Matrix rotMat;
			Matrix.CreateFromYawPitchRoll(_yaw, _pitch, 0.0f, out rotMat);
			Vector3.Transform(ref diff, ref rotMat, out diff);
			Position = Target + diff;
		}
	}
}
