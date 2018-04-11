﻿using System;
using NGAPI;
using Lidgren.Network;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using NGSim.Graphics;

namespace NGSim
{
	// On the client side, there is no "updating", as all of the updates are managed by the server, and implemented
	// by the network manager (Client). The client simulation manager simply holds the simulation state, as well as
	// all of the code needed to render the simulation.
	public class SimulationManager
	{
		public static SimulationManager Instance { get; private set; } = null;

		internal Simulation Simulation { get; private set; }
		private GraphicsDevice _device;

		private List<Position> mposList = new List<Position>();
		private List<float> mheadingList = new List<float>();

		private WorldModel _world;
		private CModel _uavModel;
		private CModel _tankModel;
		private CModel _missModel;
		private ConeModel _cone;

		private readonly SpriteBatch _sb;
		private readonly Texture2D _blankTex;
		private readonly SpriteFont _font;
		private readonly Rectangle _lRect;

		private Vector2 origin = new Vector2(0, 0);


		public SimulationManager(GraphicsDevice device, ContentManager content)
		{
			if (Instance != null)
				throw new InvalidOperationException("Cannot create more than once instance of the simulation manager.");
			Instance = this;

			_sb = new SpriteBatch(device);
			_blankTex = new Texture2D(device, 1, 1);
			_blankTex.SetData(new Color[] { Color.White });

			_device = device;
			_world = new WorldModel(device, (int)Constants.WorldSize.X / 10);
			_uavModel = new CModel(device, content.Load<Model>("UAV"));
			_tankModel = new CModel(device, content.Load<Model>("tank"));
			_missModel = new CModel(device, content.Load<Model>("sphere_missile"));
			_cone = new ConeModel(device);

			_font = content.Load<SpriteFont>("debugfont");
			_lRect = new Rectangle(0, 0, 130, 70);

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
			int mcount = msg.ReadInt32();
			mposList.Clear();

			for (int i = 0; i < mcount; ++i)
			{
				Position mpos = new Position(msg.ReadSingle(), msg.ReadSingle());
				float heading = msg.ReadSingle();
				byte team = msg.ReadByte();

				mposList.Add(mpos);
				mheadingList.Add(heading);
			}
		}

		public void Render()
		{
			// Draw the world
			Camera camera = CameraManager.ActiveCamera;
			_world.Draw(_device, camera);

			// Draw the entities
			Position t1 = Simulation.Team1.Tank.Position;
			Position t2 = Simulation.Team2.Tank.Position;
			Position u1 = Simulation.Team1.UAV.Position;
			Position u2 = Simulation.Team2.UAV.Position;
			float t1h = Simulation.Team1.Tank.CurrentHeading;
			float t2h = Simulation.Team2.Tank.CurrentHeading;
			float u1h = Simulation.Team1.UAV.CurrentHeading;
			float u2h = Simulation.Team2.UAV.CurrentHeading;
			_tankModel.Render(camera, new Vector3(t1.X / 10, 0, t1.Y / 10), t1h, Color.Blue);
			_tankModel.Render(camera, new Vector3(t2.X / 10, 0, t2.Y / 10), t2h, Color.Red);
			_uavModel.TextureRender(camera, new Vector3(u1.X / 10, 10, u1.Y / 10), u1h);
			_uavModel.TextureRender(camera, new Vector3(u2.X / 10, 10, u2.Y / 10), u2h);

			//Draw all missiles
			for (int i = 0; i < mposList.Count; i++)
			{
				_missModel.Render(camera, new Vector3(mposList[i].X / 10, 1, mposList[i].Y / 10), mheadingList[i], Color.Black);
			}

			_cone.Render(_device, new Vector2(u1.X / 10, u1.Y / 10), camera, 1, Simulation.Team1.UAV.DetectedTankThisTurn);
			_cone.Render(_device, new Vector2(u2.X / 10, u2.Y / 10), camera, 2, Simulation.Team2.UAV.DetectedTankThisTurn);

			// Draw legend
			_sb.Begin();
			_sb.Draw(_blankTex, _lRect, Color.White);
			_sb.DrawString(_font, "Blue = Team 1 Tank", new Vector2(5, 5), Color.Black, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
			_sb.DrawString(_font, "Cyan = Team 1 UAV", new Vector2(5, 18), Color.Black, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
			_sb.DrawString(_font, "Red  = Team 2 Tank", new Vector2(5, 31), Color.Black, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
			_sb.DrawString(_font, "Pink = Team 2 UAV", new Vector2(5, 44), Color.Black, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
			_sb.DrawString(_font, "Black = Missiles", new Vector2(5, 57), Color.Black, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
			_sb.End();
		}

		private Vector2 posToVec(Position pos)
		{
			return new Vector2(pos.X, pos.Y);
		}
	}
}
