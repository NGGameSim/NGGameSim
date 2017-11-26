using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NGSim.Graphics
{
	public class WorldModel : IDisposable
	{
		private VertexBuffer _vBuffer;
		private IndexBuffer _iBuffer;
		private BasicEffect _groundEffect;

		public readonly int Size;

		public WorldModel(GraphicsDevice device, int worldsize)
		{
			Size = worldsize;
			int won2 = worldsize / 2;
			VertexPositionColor[] vGround = new VertexPositionColor[4]
			{
				new VertexPositionColor(new Vector3(-won2, 0, -won2), Color.Green),
				new VertexPositionColor(new Vector3(won2, 0, -won2), Color.Green),
				new VertexPositionColor(new Vector3(won2, 0, won2), Color.Green),
				new VertexPositionColor(new Vector3(-won2, 0, won2), Color.Green)
			};
			_vBuffer = new VertexBuffer(device, VertexPositionColor.VertexDeclaration, 4, BufferUsage.None);
			_vBuffer.SetData(vGround);
			ushort[] iGround = new ushort[6] { 0, 1, 3, 1, 2, 3 };
			_iBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, 6, BufferUsage.None);
			_iBuffer.SetData(iGround);

			_groundEffect = new BasicEffect(device);
			_groundEffect.VertexColorEnabled = true;
			_groundEffect.TextureEnabled = false;
			_groundEffect.LightingEnabled = false;
		}

		public void Draw(GraphicsDevice device, Camera camera)
		{
			_groundEffect.World = Matrix.Identity;
			_groundEffect.View = camera.ViewMatrix;
			_groundEffect.Projection = camera.ProjectionMatrix;
			device.SetVertexBuffer(_vBuffer);
			device.Indices = _iBuffer;
			_groundEffect.CurrentTechnique.Passes[0].Apply();
			device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4);
		}

		public void Dispose()
		{
			_iBuffer.Dispose();
			_vBuffer.Dispose();
			_groundEffect.Dispose();
		}
	}
}
