using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NGSim.Graphics
{
	public class CModel
	{
		private static BasicEffect modelEffect = null;
		private static readonly object effectMutex = new object();

		public Model Model { get; private set; } = null;
		public Texture2D Texture { get; private set; } = null;
		public readonly GraphicsDevice Device = null;

		public CModel(GraphicsDevice device, Model model, Texture2D tex)
		{
			Model = model;
			Device = device;
			Texture = tex;

			lock (effectMutex)
			{
				if (modelEffect == null)
				{
					modelEffect = new BasicEffect(device);
					modelEffect.VertexColorEnabled = true;
					modelEffect.LightingEnabled = false;
					modelEffect.TextureEnabled = true;
				}
			}
		}

		public void Render(Camera camera, Vector3 position)
		{
			Matrix world = Matrix.CreateTranslation(position);

			foreach (var mesh in Model.Meshes)
			{
				foreach (BasicEffect effect in mesh.Effects)
				{
					effect.World = world;
					effect.View = camera.ViewMatrix;
					effect.Projection = camera.ProjectionMatrix;
				}

				mesh.Draw();
			}
		}
	}
}
