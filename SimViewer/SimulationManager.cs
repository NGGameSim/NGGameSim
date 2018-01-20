using System;
using NGAPI;
using Lidgren.Network;

namespace NGSim
{
	// On the client side, there is no "updating", as all of the updates are managed by the server, and implemented
	// by the network manager (Client). The client simulation manager simply holds the simulation state, as well as
	// all of the code needed to render the simulation.
	public class SimulationManager
	{
		public static SimulationManager Instance { get; private set; } = null;

		internal Simulation Simulation { get; private set; }

		public SimulationManager()
		{
			if (Instance != null)
				throw new InvalidOperationException("Cannot create more than once instance of the simulation manager.");
			Instance = this;

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

		}
	}
}
