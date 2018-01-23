using System;
using NGAPI;
using Lidgren.Network;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace NGSim
{
	// On the client side, there is no "updating", as all of the updates are managed by the server, and implemented
	// by the network manager (Client). The client simulation manager simply holds the simulation state, as well as
	// all of the code needed to render the simulation.
	public class SimulationManager
	{
		public static SimulationManager Instance { get; private set; } = null;

		internal Simulation Simulation { get; private set; }

		private readonly SpriteBatch _sb;
		private readonly Texture2D _blankTex;
		private readonly Rectangle _bgRect;
		private readonly Matrix _projMatrix;
		private readonly SpriteFont _font;
		private readonly Rectangle _lRect;

		public SimulationManager(GraphicsDevice device, ContentManager content)
		{
			if (Instance != null)
				throw new InvalidOperationException("Cannot create more than once instance of the simulation manager.");
			Instance = this;

			_sb = new SpriteBatch(device);
			_blankTex = new Texture2D(device, 1, 1);
			_blankTex.SetData(new Color[] { Color.White });

			int wx = (int)(Constants.WorldSize.X / 2);
			int wy = (int)(Constants.WorldSize.Y / 2);
			Rectangle view = _bgRect = new Rectangle(-wx, -wy, wx * 2, wy * 2);
			//Matrix halfPix = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
			//_projMatrix = Matrix.CreateOrthographic(200, 200, 0, 1);// Matrix.CreateOrthographic(Constants.WorldSize.X, Constants.WorldSize.Y, 0, 1);

			_projMatrix = Matrix.CreateScale(900 / Constants.WorldSize.Y) * Matrix.CreateTranslation(device.Viewport.Width / 2, device.Viewport.Height / 2, 0);

			_font = content.Load<SpriteFont>("debugfont");
			_lRect = new Rectangle(0, 0, 130, 62);

			Simulation = new Simulation();
		}

		// Reads information for an entity update packet (opcode 1)
		public void TranslateEntityPacket(NetIncomingMessage msg)
		{
			Simulation.Team1.Tank.Position = new Position(msg.ReadSingle(), msg.ReadSingle());
			Simulation.Team1.Tank.CurrentHeading = msg.ReadSingle();
			Simulation.Team1.UAV.Position = new Position(msg.ReadSingle(), msg.ReadSingle());
			Simulation.Team1.UAV.CurrentHeading = msg.ReadSingle();
			Simulation.Team2.Tank.Position = new Position(msg.ReadSingle(), msg.ReadSingle());
			Simulation.Team2.Tank.CurrentHeading = msg.ReadSingle();
			Simulation.Team2.UAV.Position = new Position(msg.ReadSingle(), msg.ReadSingle());
			Simulation.Team2.UAV.CurrentHeading = msg.ReadSingle();
			Simulation.Team1.Tank.MisslesLeft = msg.ReadByte();
			Simulation.Team2.Tank.MisslesLeft = msg.ReadByte();
		}

		// Reads information for a missile update packet (opcode 2)
		public void TranslateMissilePacket(NetIncomingMessage msg)
		{
			int mcount = msg.ReadByte();
			for (int i = 0; i < mcount; ++i)
			{
				Position mpos = new Position(msg.ReadSingle(), msg.ReadSingle());
				float heading = msg.ReadSingle();
				byte team = msg.ReadByte();

				// TODO: something with the missiles
			}
		}

		public void Render()
		{
			// Draw legend
			_sb.Begin();
			_sb.Draw(_blankTex, _lRect, Color.White);
			_sb.DrawString(_font, "Blue = Team 1 Tank", new Vector2(5, 5), Color.Black, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
			_sb.DrawString(_font, "Cyan = Team 1 UAV", new Vector2(5, 18), Color.Black, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
			_sb.DrawString(_font, "Red  = Team 2 Tank", new Vector2(5, 31), Color.Black, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
			_sb.DrawString(_font, "Pink = Team 2 UAV", new Vector2(5, 44), Color.Black, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
			_sb.End();

			_sb.Begin(transformMatrix: _projMatrix);

			// Draw background
			_sb.Draw(_blankTex, _bgRect, Color.DarkGreen);

			// Draw entities
			_sb.Draw(_blankTex, posToVec(Simulation.Team1.Tank.Position), null, Color.Blue, 0, Vector2.One /2 , 20, SpriteEffects.None, 0);
			_sb.Draw(_blankTex, posToVec(Simulation.Team2.Tank.Position), null, Color.Red, 0, Vector2.One / 2, 20, SpriteEffects.None, 0);
			_sb.Draw(_blankTex, posToVec(Simulation.Team1.UAV.Position), null, Color.Cyan, (float)Math.PI / 4, Vector2.One / 2, 20, SpriteEffects.None, 0);
			_sb.Draw(_blankTex, posToVec(Simulation.Team2.UAV.Position), null, Color.Pink, (float)Math.PI / 4, Vector2.One / 2, 20, SpriteEffects.None, 0);

			_sb.End();
		}

		private Vector2 posToVec(Position pos)
		{
			return new Vector2(pos.X, pos.Y);
		}
	}
}
