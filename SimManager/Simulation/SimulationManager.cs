using NGAPI;
using System.Collections.Generic;
using System;

namespace NGSim.Simulation
{
	// On the server-side, the simulation manager is reponsible for quite a lot, including updating the algorithms,
	// performing entity and game state logic updates, and dispatching state packets to all of the connected clients.
	public class SimulationManager
	{
		internal NGAPI.Simulation Simulation { get; private set; }
		private int gameResult = 0; //0 means game is running, 1 means Team1 won, 2  means Team2 won, 3 means a draw
		private int negativeBoundX = -500;
		private int positiveBoundX = 500;
		private int negativeBoundY = -500;
		private int positiveBoundY = 500;
		private int boomRange = 22; //22 meters is effective blast radius of 120mm cannon on M1 Abrams
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

			int randX = rand.Next(1, positiveBoundX - negativeBoundX) + negativeBoundX;
            int randY = rand.Next(1, positiveBoundY - negativeBoundY) + negativeBoundY;
			Simulation.Team1.Tank.Position = new Position(randX, randY);
            Simulation.Team1.UAV.Position = new Position(randX, randY);

            randX = rand.Next(1, positiveBoundX - negativeBoundX) + negativeBoundX;
            randY = rand.Next(1, positiveBoundY - negativeBoundY) + negativeBoundY;
            Simulation.Team2.Tank.Position = new Position(randX, randY);
            Simulation.Team2.UAV.Position = new Position(randX, randY);
        }

		public void Update()
		{
			if (gameResult != 0)
			{
				Console.WriteLine("Winner is team {0}", gameResult);
				return;
				//Environment.Exit(0);
			}
			Console.WriteLine("X1Tank is {0} Y1Tank is {1}", Simulation.Team1.Tank.Position.X, Simulation.Team1.Tank.Position.Y);
			Console.WriteLine("X1UAV is {0} Y1UAV is {1}", Simulation.Team1.UAV.Position.X, Simulation.Team1.UAV.Position.Y);
			Console.WriteLine("X2Tank is {0} Y2Tank is {1}", Simulation.Team2.Tank.Position.X, Simulation.Team2.Tank.Position.Y);
			Console.WriteLine("X2UAV is {0} Y2UAV is {1}", Simulation.Team2.UAV.Position.X, Simulation.Team2.UAV.Position.Y);
			// Perform interpolations for speeds and headings
			updateEntityVelocities();
			// Update the positions on the new speed and headings
			updateEntityPositions();
			// Checks for out of bounds entities
			checkBounds();
			// Fires new missiles from the teams
			fireNewMissiles();
			// Update the existing missile positions
			updateMissiles();
			// Check if any teams have been hit with missiles
			checkMissileImpacts();
			// Run the user algorithms
			runUserAlgorithms();


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
			foreach(Missile missile in MissileInAir)
			{
				if(missile.TurnsRemaining == 0)
				{
					if(missile.Target.DistanceTo(Simulation.Team1.Tank.Position) < boomRange)
					{
						// Team 1 tank is hit, Team two wins.
						gameResult = 2;
					}
					if(missile.Target.DistanceTo(Simulation.Team2.Tank.Position) < boomRange)
					{
						// Team 2 tank is hit, Team one wins.
						gameResult = 1;
					}
				}
			}
		}

		private void updateMissiles()
		{
			MissileInAir.ForEach((missile) => { missile.TurnsRemaining -= 1; });
		}

		private void fireNewMissiles()
		{
			// TODO: update the Tank Cooldowns

			if (Simulation.Team1.Tank.FiresThisTurn == true)
			{
				Missile missile = new NGAPI.Missile();
				MissileInAir.Add(missile);
				missile.TurnsRemaining = 20;
				missile.Source = Simulation.Team1.Tank.Position;
				missile.Target = Simulation.Team1.Tank.MissileTarget;
				Simulation.Team1.Tank.FiresThisTurn = false;
				Simulation.Team1.Tank.Cooldown = 20;
			}
			if(Simulation.Team2.Tank.FiresThisTurn == true)
			{
				Missile missile = new NGAPI.Missile();
				MissileInAir.Add(missile);
				missile.TurnsRemaining = 20;
				missile.Source = Simulation.Team2.Tank.Position;
				missile.Target = Simulation.Team2.Tank.MissileTarget;
				Simulation.Team2.Tank.FiresThisTurn = false;
				Simulation.Team2.Tank.Cooldown = 20;
			}
		}

		private void checkBounds()
		{
			// TODO: these dont need to check the current team, both teams update here
			
			if (Simulation.Team1.Tank.Position.X > positiveBoundX || Simulation.Team1.Tank.Position.X < negativeBoundX) { gameResult = 2; }
			else if (Simulation.Team1.UAV.Position.X > positiveBoundX || Simulation.Team1.UAV.Position.X < negativeBoundX) { gameResult = 2; }
			else if (Simulation.Team1.Tank.Position.Y > positiveBoundY || Simulation.Team1.Tank.Position.Y < negativeBoundY) { gameResult = 2; }
			else if (Simulation.Team1.UAV.Position.Y > positiveBoundY || Simulation.Team1.UAV.Position.Y < negativeBoundY) { gameResult = 2; }
			
			if (Simulation.Team2.Tank.Position.X > positiveBoundX || Simulation.Team2.Tank.Position.X < negativeBoundX) { gameResult = 1; }
			else if (Simulation.Team2.UAV.Position.X > positiveBoundX || Simulation.Team2.UAV.Position.X < negativeBoundX) { gameResult = 1; }
			else if (Simulation.Team2.Tank.Position.Y > positiveBoundY || Simulation.Team2.Tank.Position.Y < negativeBoundY) { gameResult = 1; }
			else if (Simulation.Team2.UAV.Position.Y > positiveBoundY || Simulation.Team2.UAV.Position.Y < negativeBoundY) { gameResult = 1; }
			
        }

        private void updateEntityPositions()
		{
			// TODO: we need to do vector math here to actually generate new positions, right now they will never move
			// TODO: no if statements, both teams update here

			float X1Tank = Simulation.Team1.Tank.CurrentSpeed * (float)Math.Cos(Simulation.Team1.Tank.CurrentHeading);
			float X1UAV = Simulation.Team1.UAV.CurrentSpeed * (float)Math.Cos(Simulation.Team1.UAV.CurrentHeading);
			float X2Tank = Simulation.Team2.Tank.CurrentSpeed * (float)Math.Cos(Simulation.Team2.Tank.CurrentHeading);
			float X2UAV = Simulation.Team2.UAV.CurrentSpeed * (float)Math.Cos(Simulation.Team2.UAV.CurrentHeading);

			float Y1Tank = Simulation.Team1.Tank.CurrentSpeed * (float)Math.Sin(Simulation.Team1.Tank.CurrentHeading);
			float Y1UAV = Simulation.Team1.UAV.CurrentSpeed * (float)Math.Sin(Simulation.Team1.UAV.CurrentHeading);
			float Y2Tank = Simulation.Team2.Tank.CurrentSpeed * (float)Math.Sin(Simulation.Team2.Tank.CurrentHeading);
			float Y2UAV = Simulation.Team2.UAV.CurrentSpeed * (float)Math.Sin(Simulation.Team2.UAV.CurrentHeading);

			Simulation.Team1.Tank.Position = new Position(Simulation.Team1.Tank.Position.X + X1Tank, Simulation.Team1.Tank.Position.Y + Y1Tank);
			Simulation.Team1.UAV.Position = new Position(Simulation.Team1.UAV.Position.X + X1UAV, Simulation.Team1.UAV.Position.Y + Y1UAV);

			Simulation.Team2.Tank.Position = new Position(Simulation.Team2.Tank.Position.X + X2Tank, Simulation.Team2.Tank.Position.Y + Y2Tank);
			Simulation.Team2.UAV.Position = new Position(Simulation.Team2.UAV.Position.X + X2UAV, Simulation.Team2.UAV.Position.Y + Y2UAV);

		}

		private void updateEntityVelocities()
		{
			// TODO: right now this just immediately updates the speeds and headings, interpolate in the future
			API.FriendlyTank.CurrentHeading = API.FriendlyTank.TargetHeading;
			API.FriendlyUAV.CurrentHeading = API.FriendlyUAV.TargetHeading;
			API.EnemyTank.CurrentHeading = API.EnemyTank.TargetHeading;
			API.EnemyUAV.CurrentHeading = API.EnemyUAV.TargetHeading;
			API.FriendlyTank.CurrentSpeed = API.FriendlyTank.TargetSpeed;
			API.FriendlyUAV.CurrentSpeed = API.FriendlyUAV.TargetSpeed;
			API.EnemyTank.CurrentSpeed = API.EnemyTank.TargetSpeed;
			API.EnemyUAV.CurrentSpeed = API.EnemyUAV.TargetSpeed;
		}

		public int getRandomInteger(int maximum)
		{
			return rand.Next(0, maximum);
		}
	}
}
