using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NGSim.Graphics
{
	public class RingModel
	{
		private static VertexBuffer vertices = null;
		private static IndexBuffer indices = null;
		private static BasicEffect effect = null;
		private static readonly Color VERT_COLOR = Color.White;

		public RingModel(GraphicsDevice device, float radius, float thickness)
		{
			if (vertices == null)
			{
				vertices = new VertexBuffer(device, VertexPositionColor.VertexDeclaration, 202, BufferUsage.WriteOnly);
				VertexPositionColor[] verts = new VertexPositionColor[202];
				for (int i = 0; i < 101; ++i)
				{
					float ang = i * (float)Math.PI / 50f;
					float x1 = radius * (float)Math.Cos(ang);
					float y1 = radius * (float)Math.Sin(ang);
					float x2 = (radius + thickness) * (float)Math.Cos(ang);
					float y2 = (radius + thickness) * (float)Math.Sin(ang);
					verts[(i * 2)    ] = new VertexPositionColor(new Vector3(x1, 0, y1), VERT_COLOR);
					verts[(i * 2) + 1] = new VertexPositionColor(new Vector3(x2, 0, y2), VERT_COLOR);
				}
				vertices.SetData(verts);

				indices = new IndexBuffer(device, IndexElementSize.SixteenBits, 600, BufferUsage.WriteOnly);
				ushort[] inds = new ushort[600];
				ushort start = 0;
				for (int i = 0; i < 100; ++i)
				{
					int BASE = i * 6;
					inds[BASE + 0] = start;
					inds[BASE + 1] = (ushort)(start + 1);
					inds[BASE + 2] = (ushort)(start + 3);
					inds[BASE + 3] = start;
					inds[BASE + 4] = (ushort)(start + 3);
					inds[BASE + 5] = (ushort)(start + 2);
					start += 2;
				}
				indices.SetData(inds);

				effect = new BasicEffect(device);
				effect.VertexColorEnabled = true;
				effect.LightingEnabled = false;
				effect.TextureEnabled = false;
			}
		}

		public void Render(GraphicsDevice device, Vector2 pos, Camera camera, int team)
		{
			Matrix world = Matrix.CreateTranslation(pos.X, 0.01f, pos.Y);
			effect.World = world;
			effect.View = camera.ViewMatrix;
			effect.Projection = camera.ProjectionMatrix;
			effect.EmissiveColor = ((team == 1) ? Color.Blue : Color.Red).ToVector3();
			device.SetVertexBuffer(vertices);
			device.Indices = indices;
			effect.CurrentTechnique.Passes[0].Apply();
			device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 200);
		}
	}
}
