using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NGAPI;

namespace NGSim.Graphics
{
	public class ConeModel
	{
		private static VertexBuffer vertices = null;
		private static IndexBuffer indices = null;
		private static BasicEffect effect = null;
		private static readonly Color VERT_COLOR = Color.White;

		public ConeModel(GraphicsDevice device)
		{
			if (vertices == null)
			{
				vertices = new VertexBuffer(device, VertexPositionColor.VertexDeclaration, 52, BufferUsage.WriteOnly);
				VertexPositionColor[] verts = new VertexPositionColor[52];
				verts[0] = new VertexPositionColor(Vector3.Zero, VERT_COLOR);
				for (int i = 0; i < 51; ++i)
				{
					double angle = i * Math.PI / 25f;
					float x = (Constants.UAVScanRange / 10f) * (float)Math.Cos(angle);
					float y = (Constants.UAVScanRange / 10f) * (float)Math.Sin(angle);
					verts[i + 1] = new VertexPositionColor(new Vector3(x, -10.1f, y), VERT_COLOR);
				}
				vertices.SetData(verts);

				indices = new IndexBuffer(device, IndexElementSize.SixteenBits, 153, BufferUsage.WriteOnly);
				short[] inds = new short[153];
				for (int i = 0; i < 51; ++i)
				{
					int BASE_INDEX = i * 3;
					inds[BASE_INDEX]     = 0;
					inds[BASE_INDEX + 1] = (short)(i + 1);
					inds[BASE_INDEX + 2] = (short)(i + 2);
				}
				indices.SetData(inds);

				effect = new BasicEffect(device);
				effect.VertexColorEnabled = true;
				effect.LightingEnabled = false;
				effect.TextureEnabled = false;
				effect.Alpha = 0.5f;
			}
		}

		public void Render(GraphicsDevice device, Vector2 pos, Camera camera, int team, bool seen)
		{
			Matrix world = Matrix.CreateTranslation(pos.X, 10, pos.Y);
			effect.World = world;
			effect.View = camera.ViewMatrix;
			effect.Projection = camera.ProjectionMatrix;
			if (seen)
				effect.EmissiveColor = Color.Yellow.ToVector3(); // Vector3.Lerp(((team == 1) ? Color.Blue : Color.Red).ToVector3(), Color.Yellow.ToVector3(), 0.5f);
			else
				effect.EmissiveColor = ((team == 1) ? Color.Blue : Color.Red).ToVector3();
			device.SetVertexBuffer(vertices);
			device.Indices = indices;
			effect.CurrentTechnique.Passes[0].Apply();
			device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 51);
		}
	}
}
