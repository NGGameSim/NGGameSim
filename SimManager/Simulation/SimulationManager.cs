using NGAPI;
using System.Collections.Generic;
using System;
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
			int randX = rand.Next(1, positiveBoundX - negativeBoundX) + negativeBoundX;
			int randY = rand.Next(1, positiveBoundY - negativeBoundY) + negativeBoundY;
			Simulation.Team1.Tank.Position = new Position(randX, randY);
			Simulation.Team1.UAV.Position = new Position(randX, randY);

			randX = rand.Next(1, positiveBoundX - negativeBoundX) + negativeBoundX;
			randY = rand.Next(1, positiveBoundY - negativeBoundY) + negativeBoundY;
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
			//Update the detection variables
			updateDetection();
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

		private void updateDetection()
		{
			if (Simulation.Team1.UAV.Position.DistanceTo(Simulation.Team2.Tank.Position) < Simulation.Team1.UAV.ViewRadius)
			{
				Simulation.Team1.UAV.DetectedTankThisTurn = true;
				Simulation.Team1.UAV.LastKnownPosition = Simulation.Team2.Tank.Position;
			}
			if (Simulation.Team2.UAV.Position.DistanceTo(Simulation.Team1.Tank.Position) < Simulation.Team2.UAV.ViewRadius)
			{
				Simulation.Team2.UAV.DetectedTankThisTurn = true;
				Simulation.Team2.UAV.LastKnownPosition = Simulation.Team1.Tank.Position;
			}
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
				missile.TurnsRemaining = 20;
				missile.Source = Simulation.Team1.Tank.Position;
				missile.Target = Simulation.Team1.Tank.MissileTarget;
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
				missile.TurnsRemaining = 20;
				missile.Source = Simulation.Team2.Tank.Position;
				missile.Target = Simulation.Team2.Tank.MissileTarget;
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

			if (Simulation.Team1.Tank.Position.X > positiveBoundX || Simulation.Team1.Tank.Position.X < negativeBoundX) { team1Disqualified = true; }
			else if (Simulation.Team1.UAV.Position.X > positiveBoundX || Simulation.Team1.UAV.Position.X < negativeBoundX) { team1Disqualified = true; }
			else if (Simulation.Team1.Tank.Position.Y > positiveBoundY || Simulation.Team1.Tank.Position.Y < negativeBoundY) { team1Disqualified = true; }
			else if (Simulation.Team1.UAV.Position.Y > positiveBoundY || Simulation.Team1.UAV.Position.Y < negativeBoundY) { team1Disqualified = true; }
			
			if (Simulation.Team2.Tank.Position.X > positiveBoundX || Simulation.Team2.Tank.Position.X < negativeBoundX) { team2Disqualified = true; }
			else if (Simulation.Team2.UAV.Position.X > positiveBoundX || Simulation.Team2.UAV.Position.X < negativeBoundX) { team2Disqualified = true; }
			else if (Simulation.Team2.Tank.Position.Y > positiveBoundY || Simulation.Team2.Tank.Position.Y < negativeBoundY) { team2Disqualified = true; }
			else if (Simulation.Team2.UAV.Position.Y > positiveBoundY || Simulation.Team2.UAV.Position.Y < negativeBoundY) { team2Disqualified = true; }
			
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
