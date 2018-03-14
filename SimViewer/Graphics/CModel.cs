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
		public readonly GraphicsDevice Device = null;

		public CModel(GraphicsDevice device, Model model)
		{
			Model = model;
			Device = device;

			lock (effectMutex)
			{
				if (modelEffect == null)
				{
					modelEffect = new BasicEffect(device);
					modelEffect.VertexColorEnabled = true;
					modelEffect.LightingEnabled = true;
					modelEffect.TextureEnabled = true;
				}
			}
		}

		public void Render(Camera camera, Vector3 position, float rotation, Color color)
		{
			Matrix world = Matrix.CreateRotationY(MathHelper.ToRadians(-rotation)) * Matrix.CreateTranslation(position);

			foreach (var mesh in Model.Meshes)
			{
				foreach (BasicEffect effect in mesh.Effects)
				{
					effect.World = world;
					effect.View = camera.ViewMatrix;
					effect.Projection = camera.ProjectionMatrix;
					effect.DiffuseColor = color.ToVector3();

					effect.LightingEnabled = true; 

					effect.AmbientLightColor = new Vector3(0.6f, 0.6f, 0.6f);

					effect.DirectionalLight0.DiffuseColor = new Vector3(1f, 1f, 1f); 
					effect.DirectionalLight0.Direction = new Vector3(0, -1, 0);  
				}

				mesh.Draw();
			}
		}
	}
}
