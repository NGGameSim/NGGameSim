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
		private float timeInTurn = 1;
		private int gameResult = 0; //0 means game is running, 1 means Team1 won, 2  means Team2 won, 3 means a draw
		private int numberOfTurns = 0;
		private int maxTurns = 100000;
		private int accelerationPerTurnTank = 2;
		private int accelerationPerTurnUAV = 4;
		private int headingChangePerTurnTank = 45;
		private int headingChangePerTurnUAV = 45;
		private int negativeBoundX = -20000;
		private int positiveBoundX = 20000;
		private int negativeBoundY = -20000;
		private int positiveBoundY = 20000;
		private int boomRange = 22; //22 meters is effective blast radius of 120mm cannon on M1 Abrams
		Random rand = new Random();

		//contains the position data of the missles in air. Since there can only be 20 missles(10 in each tank, each can have its own place
		// private Missile[] MissilesInAir = new Missile[];
		private List<Missile> MissileInAir = new List<Missile>();

		public SimulationManager()
		{
            Simulation = new NGAPI.Simulation();
            API.Simulation = Simulation;

            int randX = rand.Next(1, (positiveBoundX - negativeBoundX) - negativeBoundX);
            int randY = rand.Next(1, (positiveBoundY - negativeBoundY) - negativeBoundY);
            Simulation.Team1.Tank.Position = new Position(randX, randY);
            Simulation.Team1.UAV.Position = new Position(randX, randY);

            randX = rand.Next(1, (positiveBoundX - negativeBoundX) - negativeBoundX);
            randY = rand.Next(1, (positiveBoundY - negativeBoundY) - negativeBoundY);
            Simulation.Team2.Tank.Position = new Position(randX, randY);
            Simulation.Team2.UAV.Position = new Position(randX, randY);
        }

		public void Update()
		{
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
			throw new NotImplementedException();
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
			if(Simulation.Team1.Tank.FiresThisTurn == true)
			{
				Missile missile = new NGAPI.Missile();
				MissileInAir.Add(missile);
				missile.TurnsRemaining = 20;
				missile.Source = Simulation.Team1.Tank.Position;
				missile.Target = Simulation.Team1.Tank.MissileTarget;
			}
			else if(Simulation.Team2.Tank.FiresThisTurn == true)
			{
				Missile missile = new NGAPI.Missile();
				MissileInAir.Add(missile);
				missile.TurnsRemaining = 20;
				missile.Source = Simulation.Team2.Tank.Position;
				missile.Target = Simulation.Team2.Tank.MissileTarget;
			}
		}

		private void checkBounds()
		{
			if(API.CurrentTeam == 1)
			{
				if (Simulation.Team1.Tank.Position.X > positiveBoundX || Simulation.Team1.Tank.Position.X < negativeBoundX) { gameResult = 2; }
				else if (Simulation.Team1.UAV.Position.X > positiveBoundX || Simulation.Team1.UAV.Position.X < negativeBoundX) { gameResult = 2; }
				else if (Simulation.Team1.Tank.Position.Y > positiveBoundY || Simulation.Team1.Tank.Position.Y < negativeBoundY) { gameResult = 2; }
				else if (Simulation.Team1.UAV.Position.Y > positiveBoundY || Simulation.Team1.UAV.Position.Y < negativeBoundY) { gameResult = 2; }
			}
			if(API.CurrentTeam != 1)
			{
				if (Simulation.Team2.Tank.Position.X > positiveBoundX || Simulation.Team2.Tank.Position.X < negativeBoundX) { gameResult = 1; }
				else if (Simulation.Team2.UAV.Position.X > positiveBoundX || Simulation.Team2.UAV.Position.X < negativeBoundX) { gameResult = 1; }
				else if (Simulation.Team2.Tank.Position.Y > positiveBoundY || Simulation.Team2.Tank.Position.Y < negativeBoundY) { gameResult = 1; }
				else if (Simulation.Team2.UAV.Position.Y > positiveBoundY || Simulation.Team2.UAV.Position.Y < negativeBoundY) { gameResult = 1; }
			}
        }

        private void updateEntityPositions()
		{
			if(API.CurrentTeam == 1)
			{
				Simulation.Team1.Tank.Position = API.FriendlyTank.Position;
				Simulation.Team1.UAV.Position = API.FriendlyUAV.Position; 
			}
            else if(API.CurrentTeam != 1)
			{
				Simulation.Team2.Tank.Position = API.FriendlyTank.Position;
				Simulation.Team2.UAV.Position = API.FriendlyUAV.Position;
			}
		}

		private void updateEntityVelocities()
		{
			API.FriendlyTank.CurrentHeading = API.FriendlyTank.TargetHeading;
			API.FriendlyUAV.CurrentHeading = API.FriendlyUAV.TargetHeading;
			API.EnemyTank.CurrentHeading = API.EnemyTank.TargetHeading;
			API.EnemyUAV.CurrentHeading = API.EnemyUAV.TargetHeading;
			API.FriendlyTank.CurrentSpeed = API.FriendlyTank.TargetSpeed;
			API.FriendlyUAV.CurrentSpeed = API.FriendlyUAV.TargetSpeed;
			API.EnemyTank.CurrentSpeed = API.EnemyTank.TargetSpeed;
			API.EnemyUAV.CurrentSpeed = API.EnemyUAV.TargetSpeed;
		}

		//We are assuming that the algorithm is somewhere else where it can't modify the number of turns or underlying tank values
		public void algo1()
		{
			//set random speed
			int speed = getRandomInteger(60);
			API.SetUAVSpeed(speed);
			int speedTank = getRandomInteger(13);
			API.SetTankSpeed(speedTank);

			//set Random headings
			int heading = getRandomInteger(360);
			API.SetUAVHeading(heading, Direction.Left);
			int headingTank = getRandomInteger(360);
			API.SetTankHeading(headingTank, Direction.Left);

			Position UAVPos = API.GetLastKnownPosition();
			Position tankPos = API.GetTankPosition();
			if(API.DetectedThisTurn() && UAVPos.DistanceTo(tankPos) < 22)
			{
				API.Fire(UAVPos);
			}

			//Bounds Checking
			if (UAVPos.X < -18000)
				API.SetUAVHeading(0, Direction.Left);
			else if (UAVPos.X > 18000)
				API.SetUAVHeading(180, Direction.Left);
			else if (UAVPos.Y < -18000)
				API.SetUAVHeading(90, Direction.Left);
			else if (UAVPos.Y > 18000)
				API.SetUAVHeading(215, Direction.Left);

			if (tankPos.X < -18000)
				API.SetTankHeading(0, Direction.Left);
			else if (tankPos.X > 18000)
				API.SetTankHeading(180, Direction.Left);
			else if (tankPos.Y < -18000)
				API.SetTankHeading(90, Direction.Left);
			else if (tankPos.Y > 18000)
				API.SetTankHeading(215, Direction.Left);
		}

		//currently identical to algo1
		public void algo2()
		{
			//set random speed
			int speed = getRandomInteger(60);
			API.SetUAVSpeed(speed);
			int speedTank = getRandomInteger(13);
			API.SetTankSpeed(speedTank);

			//set Random headings
			int heading = getRandomInteger(360);
			API.SetUAVHeading(heading, Direction.Left);
			int headingTank = getRandomInteger(360);
			API.SetTankHeading(headingTank, Direction.Left);

			Position UAVPos = API.GetLastKnownPosition();
			Position tankPos = API.GetTankPosition();
			if (API.DetectedThisTurn() && UAVPos.DistanceTo(tankPos) < 22)
			{
				API.Fire(UAVPos);
			}

			//Bounds Checking
			if (UAVPos.X < -18000)
				API.SetUAVHeading(0, Direction.Left);
			else if (UAVPos.X > 18000)
				API.SetUAVHeading(180, Direction.Left);
			else if (UAVPos.Y < -18000)
				API.SetUAVHeading(90, Direction.Left);
			else if (UAVPos.Y > 18000)
				API.SetUAVHeading(215, Direction.Left);

			if (tankPos.X < -18000)
				API.SetTankHeading(0, Direction.Left);
			else if (tankPos.X > 18000)
				API.SetTankHeading(180, Direction.Left);
			else if (tankPos.Y < -18000)
				API.SetTankHeading(90, Direction.Left);
			else if (tankPos.Y > 18000)
				API.SetTankHeading(215, Direction.Left);
		}

		public int getRandomInteger(int maximum)
		{
			return rand.Next(0, maximum);
		}
	}
}
