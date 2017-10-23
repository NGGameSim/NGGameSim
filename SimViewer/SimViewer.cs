using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NGSim.Graphics;

namespace NGSim
{
	public class SimViewer : Game
	{
		private GraphicsDeviceManager _graphics;

		private BasicEffect _effect;
		private IndexBuffer _iBuffer;
		private VertexBuffer _vBuffer;
		private ArcBallCamera _camera;

		public SimViewer() :
			base()
		{
			_graphics = new GraphicsDeviceManager(this);
		}

		protected override void Initialize()
		{
			base.Initialize();

			// Create basic shader
			_effect = new BasicEffect(GraphicsDevice);
			_effect.VertexColorEnabled = true;
			_effect.LightingEnabled = false;
			_effect.TextureEnabled = false;

			// Populate the vertex buffer
			_vBuffer = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, 8, BufferUsage.None);
			_vBuffer.SetData(new VertexPositionColor[8]
			{
				new VertexPositionColor(new Vector3(-1, -1, -1), Color.White),
				new VertexPositionColor(new Vector3( 1, -1, -1), Color.Blue),
				new VertexPositionColor(new Vector3( 1, -1,  1), Color.Yellow),
				new VertexPositionColor(new Vector3(-1, -1,  1), Color.Red),
				new VertexPositionColor(new Vector3(-1,  1, -1), Color.Green),
				new VertexPositionColor(new Vector3( 1,  1, -1), Color.Magenta),
				new VertexPositionColor(new Vector3( 1,  1,  1), Color.Cyan),
				new VertexPositionColor(new Vector3(-1,  1,  1), Color.Pink)
			});

			// Populate the index buffer
			_iBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, 36, BufferUsage.None);
			_iBuffer.SetData(new ushort[36]
			{
				6, 5, 1, 6, 1, 2, // +x
				4, 7, 3, 4, 3, 0, // -x
				4, 5, 6, 4, 6, 7, // +y
				1, 0, 3, 1, 3, 2, // -y
				7, 6, 2, 7, 2, 3, // +z
				5, 4, 0, 5, 0, 1  // -z
			});

			// Create the camera
			_camera = new ArcBallCamera(GraphicsDevice, yaw: 45f, pitch: 45f);
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			GraphicsDevice.SetVertexBuffer(_vBuffer);
			GraphicsDevice.Indices = _iBuffer;

			//_effect.View = _camera.ViewMatrix;
			//_effect.Projection = _camera.ProjectionMatrix;
			_effect.View = Matrix.CreateLookAt(new Vector3(5, 5, 5), Vector3.Zero, Vector3.Up);
			_effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 16f / 9f, 0.001f, 1000f);
			_effect.World = Matrix.Identity;

			_effect.CurrentTechnique.Passes[0].Apply();
			GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 12);

			base.Draw(gameTime);
		}
	}
}
