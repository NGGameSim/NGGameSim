using NGAPI;
using System.Collections.Generic;
using System;
using NGSim.Network;
using static NGAPI.Constants;

namespace NGSim.Simulation
{
	// On the server-side, the simulation manager is reponsible for quite a lot, including updating the algorithms,
	// performing entity and game state logic updates, and dispatching state packets to all of the connected clients.
	public class SimulationManager
	{
		internal NGAPI.Simulation Simulation { get; private set; }

		//when gameRunningMode = 0, 1 game is ran and game state info is printed,
		//when it is 1, games are continuously ran and the result is displayed
		//when it is 2, 500 games are ran the winning percentage is displayed
		private int gameRunningMode;
		private int gameResult; //0 means game is running, 1 means Team1 won, 2  means Team2 won, 3 means a draw
		private int numMoves;
		private bool switchedGameMode = false;  //prevents race condition when switching the mode

		Random rand = new Random();
		StupidAlgorithm1 algo1 = new StupidAlgorithm1();
		StupidAlgorithm2 algo2 = new StupidAlgorithm2();

		//contains the position data of the missles in air. Since there can only be 20 missles(10 in each tank, each can have its own place
		// private Missile[] MissilesInAir = new Missile[];
		private List<Missile> MissileInAir = new List<Missile>();

		public Boolean running = false;

		public SimulationManager()
		{
			Simulation = new NGAPI.Simulation();
			API.Simulation = Simulation;
		}

		public void Update()
		{
			if(switchedGameMode)
			{
				gameResult = 0;
				numMoves = 0;
				SetInitialRandomPositions();
				switchedGameMode = false;
			}

			if (gameRunningMode == 0)
			{
				
				if (gameResult == 0 && numMoves < maxTurns)
				{
					UpdateGameState();
					numMoves++;
				}
				else if (gameResult == 0)
				{
					Console.WriteLine("Maximum time exceeded, result is a draw");
				}
				else if (gameResult == 3)
				{
					Console.WriteLine("Both tanks destroyed, result is a draw");
				}
				else
				{
					Console.WriteLine("Winner is team {0}", gameResult);
				}
			}
			else if (gameRunningMode == 1)
			{
				RunOneGame();
				if (gameResult == 0)
				{
					Console.WriteLine("Maximum time exceeded, result is a draw");
				}
				else if (gameResult == 3)
				{
					Console.WriteLine("Both tanks destroyed, result is a draw");
				}
				else
				{
					Console.WriteLine("Winner is team {0}", gameResult);
				}
			}
			else if (gameRunningMode == 2)
			{
				Run500Games();
			}
		}

		public void SetGameRunningMode(int mode)
		{
			switchedGameMode = true;
			gameRunningMode = mode;
			Console.WriteLine("Game set to run in mode {0}", gameRunningMode);
		}

		public void Run500Games()
		{
			int numberOfVictoriesTeam1 = 0;
			int numberOfVictoriesTeam2 = 0;
			int numberOfDraws = 0;

			for(int i=0; i<500;i++)
			{
				RunOneGame();
				if(gameResult == 1)
				{
					numberOfVictoriesTeam1++;
				}
				else if (gameResult == 2)
				{
					numberOfVictoriesTeam2++;
				}
				else
				{
					numberOfDraws++;
				}
			}

			Console.WriteLine("Of the 500 games Team 1 won {0}%, team 2 won {1}%, and {2}% are draws", 100*((float)numberOfVictoriesTeam1)/500, 100 * ((float)numberOfVictoriesTeam2) / 500, 100 * ((float)numberOfDraws) / 500);
		}

		public void RunOneGame()
		{
			gameResult = 0;
			numMoves = 0;

			SetInitialRandomPositions();

			while (numMoves < maxTurns && gameResult == 0)
			{
				UpdateGameState();
				numMoves++;
			}
		}

		public void SetInitialRandomPositions()
		{
			int xlim = (int)(WorldSize.X / 3);
			int ylim = (int)(WorldSize.Y / 3);

			int randX = rand.Next(-xlim, xlim);
			int randY = rand.Next(-ylim, ylim);
			Simulation.Team1.Tank.Position = new Position(randX, randY);
			Simulation.Team1.UAV.Position = new Position(randX, randY);

			randX = rand.Next(-xlim, xlim);
			randY = rand.Next(-ylim, ylim);
			Simulation.Team2.Tank.Position = new Position(randX, randY);
			Simulation.Team2.UAV.Position = new Position(randX, randY);
		}

		public void UpdateGameState()
		{
			if (gameResult != 0)
			{
				return;
				//Environment.Exit(0);
			}

			// Perform interpolations for speeds and headings
			updateEntityVelocities();
			// Update the positions on the new speed and headings
			updateEntityPositions();
			// Checks for out of bounds entities
			checkBounds();
			// UAVs scan to see if tank is in range
			UAVScan();
			// Fire new missiles
			fireNewMissiles();
			// Update the existing missile positions
			updateMissiles();
			// Check if any teams have been hit with missiles
			checkMissileImpacts();
			// Run the user algorithms
			runUserAlgorithms();

			// Send network update packets
			sendNetworkPackets();
		}

		private void sendNetworkPackets()
		{
			// Entity update packet
			var entityPacket = Server.Instance.CreateMessage(1);
			entityPacket.Write(Simulation.Team1.Tank.Position.X);
			entityPacket.Write(Simulation.Team1.Tank.Position.Y);
			entityPacket.Write(Simulation.Team1.Tank.CurrentHeading);
			entityPacket.Write(Simulation.Team1.UAV.Position.X);
			entityPacket.Write(Simulation.Team1.UAV.Position.Y);
			entityPacket.Write(Simulation.Team1.UAV.CurrentHeading);
			entityPacket.Write(Simulation.Team2.Tank.Position.X);
			entityPacket.Write(Simulation.Team2.Tank.Position.Y);
			entityPacket.Write(Simulation.Team2.Tank.CurrentHeading);
			entityPacket.Write(Simulation.Team2.UAV.Position.X);
			entityPacket.Write(Simulation.Team2.UAV.Position.Y);
			entityPacket.Write(Simulation.Team2.UAV.CurrentHeading);
			entityPacket.Write((byte)Simulation.Team1.Tank.MisslesLeft);
			entityPacket.Write((byte)Simulation.Team2.Tank.MisslesLeft);

			// Missile update packet
			var missilePacket = Server.Instance.CreateMessage(2);
			missilePacket.Write((byte)(Simulation.Team1.Missiles.Count + Simulation.Team2.Missiles.Count));
			foreach (var missile in Simulation.Team1.Missiles)
			{
				missilePacket.Write(missile.Target.X); // TODO: Get the position
				missilePacket.Write(missile.Target.Y); // TODO: Get the position
				missilePacket.Write(0f); // TODO: Calculate Heading
				missilePacket.Write((byte)1);
			}
			foreach (var missile in Simulation.Team2.Missiles)
			{
				missilePacket.Write(missile.Target.X); // TODO: Get the position
				missilePacket.Write(missile.Target.Y); // TODO: Get the position
				missilePacket.Write(0f); // TODO: Calculate Heading
				missilePacket.Write((byte)2);
			}

			// Send the packets
			Server.Instance.SendMessage(entityPacket);
			Server.Instance.SendMessage(missilePacket);
		}

		private void runUserAlgorithms()
		{
			algo1.Update();
			API.CurrentTeam = 2;
			algo2.Update();
			API.CurrentTeam = 1;
		}

		private void checkMissileImpacts()
		{
			bool team1Hit = false;
			bool team2Hit = false;
			for (int i = 0; i < MissileInAir.Count; i++)
			{
				if(MissileInAir[i].TurnsRemaining == 0)
				{
					if(MissileInAir[i].Target.DistanceTo(Simulation.Team1.Tank.Position) < boomRange)
					{
						// Team 1 tank is hit, Team two wins.
						team1Hit = true;
						Console.WriteLine("Tank was destroyed!!");
					}
					if(MissileInAir[i].Target.DistanceTo(Simulation.Team2.Tank.Position) < boomRange)
					{
						// Team 2 tank is hit, Team one wins.
						team2Hit = true;
						Console.WriteLine("Tank was destroyed!!");
					}
					MissileInAir.RemoveAt(i);
				}
			}
			if(team1Hit && team2Hit)
			{
				gameResult = 3;
			}
			else if(team1Hit)
			{
				gameResult = 2;
			}
			else if(team2Hit)
			{
				gameResult = 1;
			}
		}
		private void updateMissiles()
		{
			MissileInAir.ForEach((missile) => { missile.TurnsRemaining -= 1; });
		}

		private void fireNewMissiles()
		{
			if(Simulation.Team1.Tank.Cooldown != 0) { Simulation.Team1.Tank.Cooldown--; }
			if(Simulation.Team2.Tank.Cooldown != 0) { Simulation.Team2.Tank.Cooldown--; }

			if (Simulation.Team1.Tank.FiresThisTurn == true)
			{
				Console.WriteLine("Team 1 Fired!");
				Missile missile = new NGAPI.Missile();
				MissileInAir.Add(missile);
				missile.Source = Simulation.Team1.Tank.Position;
				missile.Target = Simulation.Team1.Tank.MissileTarget;
                missile.TurnsRemaining = (int)missile.Source.DistanceTo(missile.Target) / 30; //M1 Abrams missiles move at 300 m/s
				Simulation.Team1.Tank.FiresThisTurn = false;
				Simulation.Team1.Tank.Cooldown = 20;
				Simulation.Team1.Tank.MisslesLeft--;
				Console.WriteLine("Team 1 has {0} Missiles Left", Simulation.Team1.Tank.MisslesLeft);
			}
			if(Simulation.Team2.Tank.FiresThisTurn == true)
			{
				Console.WriteLine("Team 2 Fired!");
				Missile missile = new NGAPI.Missile();
				MissileInAir.Add(missile);
				missile.Source = Simulation.Team2.Tank.Position;
				missile.Target = Simulation.Team2.Tank.MissileTarget;
                missile.TurnsRemaining = (int)missile.Source.DistanceTo(missile.Target) / 30;
                Simulation.Team2.Tank.FiresThisTurn = false;
				Simulation.Team2.Tank.Cooldown = 20;
				Simulation.Team2.Tank.MisslesLeft--;
				Console.WriteLine("Team 2 has {0} Missiles Left", Simulation.Team2.Tank.MisslesLeft);
			}
		}

		private void checkBounds()
		{
			// TODO: these dont need to check the current team, both teams update here

			bool team1Disqualified = false;
			bool team2Disqualified = false;

			if (!inBounds(Simulation.Team1.Tank.Position)) { team1Disqualified = true; }
			else if (!inBounds(Simulation.Team1.UAV.Position)) { team1Disqualified = true; }
			
			if (!inBounds(Simulation.Team2.Tank.Position)) { team2Disqualified = true; }
			else if (!inBounds(Simulation.Team2.UAV.Position)) { team2Disqualified = true; }
			
			if(team1Disqualified && team2Disqualified)
			{
				gameResult = 3;
			}
			else if(team1Disqualified)
			{
				gameResult = 2;
			}
			else if (team2Disqualified)
			{
				gameResult = 1;
			}
		}

		private bool inBounds(Position pos)
		{
			float xlim = WorldSize.X / 2;
			float ylim = WorldSize.Y / 2;

			return (pos.X < xlim && pos.X > -xlim) && (pos.Y < ylim && pos.Y > -ylim);
		}

		private void updateEntityPositions()
		{

			float X1Tank = Simulation.Team1.Tank.CurrentSpeed * (float)Math.Sin(Simulation.Team1.Tank.CurrentHeading);
			float X1UAV = Simulation.Team1.UAV.CurrentSpeed * (float)Math.Sin(Simulation.Team1.UAV.CurrentHeading);
			float X2Tank = Simulation.Team2.Tank.CurrentSpeed * (float)Math.Sin(Simulation.Team2.Tank.CurrentHeading);
			float X2UAV = Simulation.Team2.UAV.CurrentSpeed * (float)Math.Sin(Simulation.Team2.UAV.CurrentHeading);

			float Y1Tank = Simulation.Team1.Tank.CurrentSpeed * (float)Math.Cos(Simulation.Team1.Tank.CurrentHeading);
			float Y1UAV = Simulation.Team1.UAV.CurrentSpeed * (float)Math.Cos(Simulation.Team1.UAV.CurrentHeading);
			float Y2Tank = Simulation.Team2.Tank.CurrentSpeed * (float)Math.Cos(Simulation.Team2.Tank.CurrentHeading);
			float Y2UAV = Simulation.Team2.UAV.CurrentSpeed * (float)Math.Cos(Simulation.Team2.UAV.CurrentHeading);

			Simulation.Team1.Tank.Position = new Position(Simulation.Team1.Tank.Position.X + X1Tank, Simulation.Team1.Tank.Position.Y + Y1Tank);
			Simulation.Team1.UAV.Position = new Position(Simulation.Team1.UAV.Position.X + X1UAV, Simulation.Team1.UAV.Position.Y + Y1UAV);

			Simulation.Team2.Tank.Position = new Position(Simulation.Team2.Tank.Position.X + X2Tank, Simulation.Team2.Tank.Position.Y + Y2Tank);
			Simulation.Team2.UAV.Position = new Position(Simulation.Team2.UAV.Position.X + X2UAV, Simulation.Team2.UAV.Position.Y + Y2UAV);

		}

		private void updateEntityVelocities()
		{
			// TODO: right now this just immediately updates the speeds and headings, interpolate in the future
			Simulation.Team1.Tank.CurrentHeading = Simulation.Team1.Tank.TargetHeading;
			Simulation.Team1.UAV.CurrentHeading = Simulation.Team1.UAV.TargetHeading;
			Simulation.Team2.Tank.CurrentHeading = Simulation.Team2.Tank.TargetHeading;
			Simulation.Team2.UAV.CurrentHeading = Simulation.Team2.UAV.TargetHeading;
			Simulation.Team1.Tank.CurrentSpeed = Simulation.Team1.Tank.TargetSpeed;
			Simulation.Team1.UAV.CurrentSpeed = Simulation.Team1.UAV.TargetSpeed;
			Simulation.Team2.Tank.CurrentSpeed = Simulation.Team2.Tank.TargetSpeed;
			Simulation.Team2.UAV.CurrentSpeed = Simulation.Team2.UAV.TargetSpeed;
		}

		private void UAVScan()
		{
			if(Simulation.Team1.UAV.Position.DistanceTo(Simulation.Team2.Tank.Position) < UAVScanRange)
			{
				Simulation.Team1.UAV.DetectedTankThisTurn = true;
				Simulation.Team1.UAV.LastKnownPosition = Simulation.Team2.Tank.Position;
			}

			if (Simulation.Team2.UAV.Position.DistanceTo(Simulation.Team1.Tank.Position) < UAVScanRange)
			{
				Simulation.Team2.UAV.DetectedTankThisTurn = true;
				Simulation.Team2.UAV.LastKnownPosition = Simulation.Team1.Tank.Position;
			}
		}

		public int getRandomInteger(int maximum)
		{
			return rand.Next(0, maximum);
		}
	}
}
