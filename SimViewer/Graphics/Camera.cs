using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NGSim.Graphics
{
	// Style of camera projection
	public enum CameraStyle
	{
		Perspective,
		Orthographic
	}

	// Base camera class, only matrix generation and basic standard camera components
	public abstract class Camera
	{
		#region Members
		#region Camera Matrices
		private Matrix _viewMatrix = Matrix.Identity;
		// The view matrix for the camera, encodes where the camera is, what its looking at, and which way is up
		public Matrix ViewMatrix
		{
			get
			{
				if (_viewDirty)
				{
					updateViewMatrix(ref _viewMatrix);
					_viewDirty = false;
				}
				return _viewMatrix;
			}
		}
		private Matrix _projectionMatrix = Matrix.Identity;
		// The projection matrix for the camera, encodes how depth is handled, field of view, and frustum shape
		public Matrix ProjectionMatrix
		{
			get
			{
				if (_projectionDirty)
				{
					updateProjectionMatrix(ref _projectionMatrix);
					_projectionDirty = false;
				}
				return _projectionMatrix;
			}
		}

		private bool _viewDirty = true;
		private bool _projectionDirty = true;
		#endregion // Camera Matrices

		#region Camera View Components
		private Vector3 _position = Vector3.UnitZ;
		// The position of the camera
		public Vector3 Position
		{
			get { return _position; }
			set
			{
				_position = value;
				_viewDirty = true;
			}
		}
		private Vector3 _target = Vector3.Zero;
		// The position that the camera is looking at
		public Vector3 Target
		{
			get { return _target; }
			set
			{
				_target = value;
				_viewDirty = true;
			}
		}
		private Vector3 _up = Vector3.Up;
		// The up direction of the camera, to define the roll point around the line between the position and target
		public Vector3 Up
		{
			get { return _up; }
			set
			{
				_up = value;
				_viewDirty = true;
			}
		}
		#endregion // Camera View Components

		#region Camera Projection Components
		private CameraStyle _style = CameraStyle.Perspective;
		// The projection style of the camera
		public CameraStyle Style
		{
			get { return _style; }
			set
			{
				_style = value;
				_projectionDirty = true;
			}
		}
		private float _fov = 45.0f;
		// The field of view, assuming a perspective projection
		public float FieldOfView
		{
			get { return _fov; }
			set
			{
				_fov = value;
				_projectionDirty = true;
			}
		}
		private float _aspectRatio = 0.0f;
		// The aspect ratio of the camera, assuming a perspective projection
		public float AspectRatio
		{
			get { return _aspectRatio; }
			set
			{
				_aspectRatio = value;
				_projectionDirty = true;
			}
		}
		private float _nearPlane = 1e-4f;
		// The distance to the camera near plane, for both perspective and orthographic projections
		public float NearPlane
		{
			get { return _nearPlane; }
			set
			{
				_nearPlane = value;
				_projectionDirty = true;
			}
		}
		private float _farPlane = 1e4f;
		// The distance to the camera far plane, for both perspective and orthographic projections
		public float FarPlane
		{
			get { return _farPlane; }
			set
			{
				_farPlane = value;
				_projectionDirty = true;
			}
		}
		private Vector2 _viewSize = Vector2.Zero;
		// The dimensions of the view, assuming an orthographic projection
		public Vector2 ViewSize
		{
			get { return _viewSize; }
			set
			{
				_viewSize = value;
				_projectionDirty = true;
			}
		}
		#endregion // Camera Projection Components

		// A bounding frustum that describes the space inside the view of the camera
		public BoundingFrustum Frustum
		{
			get
			{
				return new BoundingFrustum(ViewMatrix * ProjectionMatrix);
			}
		}
		// The viewport of the camera
		public Viewport Viewport;

		// The cached graphics device manager
		protected readonly GraphicsDevice graphicsDevice = null;
		#endregion // Members

		protected Camera(GraphicsDevice device)
		{
			graphicsDevice = device;
			_aspectRatio = (float)device.DisplayMode.Width / device.DisplayMode.Height;
			_viewSize = new Vector2(device.DisplayMode.Width, device.DisplayMode.Height);
			ResetViewport();
		}

		protected virtual void updateViewMatrix(ref Matrix matrix)
		{
			Matrix.CreateLookAt(ref _position, ref _target, ref _up, out matrix);
		}

		protected virtual void updateProjectionMatrix(ref Matrix matrix)
		{
			if (Style == CameraStyle.Perspective)
				Matrix.CreatePerspectiveFieldOfView(_fov, _aspectRatio, _nearPlane, _farPlane, out matrix);
			else
				Matrix.CreateOrthographic(_viewSize.X, _viewSize.Y, _nearPlane, _farPlane, out matrix);
		}

		public void ResetViewport()
		{
			Viewport = new Viewport(0, 0, graphicsDevice.DisplayMode.Width, graphicsDevice.DisplayMode.Height);
		}
	}
}
