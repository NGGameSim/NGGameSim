using NGAPI;
using System.Collections.Generic;
using System;
using NLog;
using System.Windows.Threading;
using System.Diagnostics;

namespace NGSim
{
	// On the server-side, the simulation manager is reponsible for quite a lot, including updating the algorithms,
	// performing entity and game state logic updates, and dispatching state packets to all of the connected clients.
	public class SimulationManager
	{
		private const float TURN_RATE = 6f;
		private const float ACC_RATE = 1f;
		private const float TWO_PI = (float)(Math.PI * 2);

		internal Simulation Simulation { get; private set; }
		public Algorithm Algo1;
		public Algorithm Algo2;

		//when gameRunningMode = 0, 1 game is ran and game state info is printed,
		//when it is 1, games are continuously ran and the result is displayed
		//when it is 2, N games are ran the winning percentage is displayed
		public int gameRunningMode;
		public int NGames;
		private int CurrentGameNumber;
		private int gameResult; //0 means game is running, 1 means Team1 won, 2  means Team2 won, 3 means a draw
		private int numMoves;
		private bool switchedGameMode = false;  //prevents race condition when switching the mode
		private bool playing = true;

		Random rand = new Random();

		private static Logger logger = LogManager.GetCurrentClassLogger();

		//contains the position data of the missles in air. Since there can only be 20 missles(10 in each tank, each can have its own place
		// private Missile[] MissilesInAir = new Missile[];
		private List<Missile> MissileInAir = new List<Missile>();

		public Boolean running = false;

		private int numberOfVictoriesTeam1;
		private int numberOfVictoriesTeam2;
		private int numberOfDraws;

		// Statistics
		private Stopwatch myStopwatch = new Stopwatch();
		private int sentBytesInitial = 0;
		private int totalSentBytes = 0;
		private int bytesPerSecond = 0;

		public SimulationManager()
		{
			Simulation = new Simulation();
			API.Simulation = Simulation;
		}

		public void Update()
		{
			if(switchedGameMode)
			{
				Console.WriteLine("Game Mode Reset");
				gameResult = 0;
				numMoves = 0;
				SetInitialRandomPositions();
				switchedGameMode = false;
			}

			if(!playing)
			{
				return;
			}

			if (gameRunningMode == 0)
			{
				
				if (gameResult == 0 && numMoves < Constants.MaxTurns)
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
					//Console.Write("Type any character to continue...");
					//var tmp = Console.ReadLine();
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
					//Console.Write("Type any character to continue...");
					//var tmp = Console.ReadLine();
				}
			}
			else if (gameRunningMode == 2)
			{
				//Run500Games();
				RunNGames();
			}
		}

		public void SetGameRunningMode(int mode)
		{
			switchedGameMode = true;
			gameRunningMode = mode;
			Console.WriteLine("Game set to run in mode {0}", gameRunningMode);
		}

		public void SetNGames(int games)
		{
			NGames = games;
		}

		public void PausePlay()
		{
			playing = !playing;
		}

		public void Run500Games()
		{
			numberOfVictoriesTeam1 = 0;
			numberOfVictoriesTeam2 = 0;
			numberOfDraws = 0;

			for(CurrentGameNumber = 0; CurrentGameNumber<500;CurrentGameNumber++)
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

		public void RunNGames()
		{
			numberOfVictoriesTeam1 = 0;
			numberOfVictoriesTeam2 = 0;
			numberOfDraws = 0;

			for (CurrentGameNumber = 0; CurrentGameNumber < NGames-1; CurrentGameNumber++)
			{
				RunOneGame();
				if (gameResult == 1)
				{
					Console.WriteLine("Team 1 Victory");
					numberOfVictoriesTeam1++;
				}
				else if (gameResult == 2)
				{
					Console.WriteLine("Team 2 Victory");
					numberOfVictoriesTeam2++;
				}
				else
				{
					Console.WriteLine("Draw");
					numberOfDraws++;
				}
			}

			Console.WriteLine("Of the {0} games Team 1 won {1}%, team 2 won {2}%, and {3}% are draws", NGames, 100 * ((float)numberOfVictoriesTeam1) / NGames, 100 * ((float)numberOfVictoriesTeam2) / NGames, 100 * ((float)numberOfDraws) / NGames);
			SetGameRunningMode(0);
		}

		public void RunOneGame()
		{
			gameResult = 0;
			numMoves = 0;

			SetInitialRandomPositions();

			while (numMoves < Constants.MaxTurns && gameResult == 0)
			{
				UpdateGameState();
				numMoves++;
			}

			SimManagerWindow.MyStateInfoTextArea.GameReset();
		}

		public void SetInitialRandomPositions()
		{
			//Set limits on the spawn positions
			int xlim = (int)(Constants.WorldSize.X / 3);
			int ylim = (int)(Constants.WorldSize.Y / 3);

			//Generate random positions and place team 1
			int randX1 = rand.Next(-xlim, xlim);
			int randY1 = rand.Next(-ylim, ylim);
			
			logger.Info($"Team 1 Initial Position {Simulation.Team1.Tank.Position.X} {Simulation.Team1.Tank.Position.Y}");

			//Do the same for team 2
			int randX2 = rand.Next(-xlim, xlim);
			int randY2 = rand.Next(-ylim, ylim);
			
			logger.Info($"Team 2 Initial Position {Simulation.Team2.Tank.Position.X} {Simulation.Team2.Tank.Position.Y}");

			//Make sure they don't spawn right next to each other
			Position Team1Pos = new Position(randX1, randY1);
			Position Team2Pos = new Position(randX2, randY2);

			while (Team1Pos.DistanceTo(Team2Pos) < 100)
			{
				randX1 = rand.Next(-xlim, xlim);
				randY1 = rand.Next(-ylim, ylim);
				randX2 = rand.Next(-xlim, xlim);
				randY2 = rand.Next(-ylim, ylim);

				Team1Pos = new Position(randX1, randY1);
				Team2Pos = new Position(randX2, randY2);
			}

			//Set the positions of the entities
			Simulation.Team1.Tank.Position = new Position(randX1, randY1);
			Simulation.Team1.UAV.Position = new Position(randX1, randY1);
			Simulation.Team2.Tank.Position = new Position(randX2, randY2);
			Simulation.Team2.UAV.Position = new Position(randX2, randY2);

			//Reset Team Missiles
			Simulation.Team1.Tank.MisslesLeft = 15;
			Simulation.Team2.Tank.MisslesLeft = 15;
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

			// Send network update packets with timing

			if (!myStopwatch.IsRunning)
			{
				myStopwatch.Reset();
				myStopwatch.Start();
				sentBytesInitial = totalSentBytes;
			}
			else
			{
				if (myStopwatch.ElapsedMilliseconds >= 1000) // After 1 second, check the bytes sent
				{
					bytesPerSecond = totalSentBytes - sentBytesInitial;
					myStopwatch.Stop();
				}
			}
			sendNetworkPackets();

			// Write relevant information to the TextAreas
			writeNetworkInfo(bytesPerSecond);
			writeStateInfo();
			
		}

		private void writeNetworkInfo(double bytesPerSec)
		{
			System.Windows.Application.Current.Dispatcher.Invoke(() =>
			{
				SimManagerWindow.MyNetworkInfoTextArea.BitRate = bytesPerSec.ToString();
				SimManagerWindow.MyNetworkInfoTextArea.ConnectionStatus = Server.Instance.isConnected;
				SimManagerWindow.MyNetworkInfoTextArea.View = "Normal";
				//SimManagerWindow.MyNetworkInfoTextArea.Warnings
			});
		}

		private void writeStateInfo()
		{
			System.Windows.Application.Current.Dispatcher.Invoke(() =>
			{
				SimManagerWindow.MyStateInfoTextArea.RedTankXY = Simulation.Team1.Tank.Position;
				SimManagerWindow.MyStateInfoTextArea.RedUAVXY = Simulation.Team1.UAV.Position;
				SimManagerWindow.MyStateInfoTextArea.RedMissilesRemaining = Simulation.Team1.Tank.MisslesLeft;
				SimManagerWindow.MyStateInfoTextArea.LastKnownBlueTankXY = Simulation.Team1.UAV.LastKnownPosition;

				SimManagerWindow.MyStateInfoTextArea.BlueTankXY = Simulation.Team2.Tank.Position;
				SimManagerWindow.MyStateInfoTextArea.BlueUAVXY = Simulation.Team2.UAV.Position;
				SimManagerWindow.MyStateInfoTextArea.BlueMissilesRemaining = Simulation.Team2.Tank.MisslesLeft;
				SimManagerWindow.MyStateInfoTextArea.LastKnownRedTankXY = Simulation.Team2.UAV.LastKnownPosition;

				SimManagerWindow.MyStateInfoTextArea.TurnsElapsed = numMoves;
				if (CurrentGameNumber != 0)
				{
					SimManagerWindow.MyStateInfoTextArea.WinPercent = numberOfVictoriesTeam1 / (CurrentGameNumber + 1);
				}
				else
				{
					SimManagerWindow.MyStateInfoTextArea.WinPercent = 0;
				}
				SimManagerWindow.MyStateInfoTextArea.GamesRun = CurrentGameNumber + 1;
			});
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
			entityPacket.Write((byte)gameResult);

			// Missile update packet
			var missilePacket = Server.Instance.CreateMessage(2);
			missilePacket.Write(MissileInAir.Count);
			foreach (Missile missile in MissileInAir)
			{
				missilePacket.Write(missile.CurrentPostion.X); 
				missilePacket.Write(missile.CurrentPostion.Y);
				missilePacket.Write(missile.CurrentHeading); 
				missilePacket.Write((byte)1);
			}

			// Send the packets
			totalSentBytes += Server.Instance.SendMessage(entityPacket);
			totalSentBytes += Server.Instance.SendMessage(missilePacket);
		}

		private void runUserAlgorithms()
		{
			Algo1.Update();
			API.CurrentTeam = 2;
			Algo2.Update();
			API.CurrentTeam = 1;
		}

		private void checkMissileImpacts()
		{
			bool team1Hit = false;
			bool team2Hit = false;
			List<Missile> toRemove = new List<Missile>();
			for (int i = 0; i < MissileInAir.Count; i++)
			{
				if(MissileInAir[i].TurnsRemaining == 0)
				{
					if (MissileInAir[i].Target.DistanceTo(Simulation.Team1.Tank.Position) < Constants.BoomRange)
					{
						// Team 1 tank is hit, Team two wins.
						team1Hit = true;
					}
					if (MissileInAir[i].Target.DistanceTo(Simulation.Team2.Tank.Position) < Constants.BoomRange)
					{
						// Team 2 tank is hit, Team one wins.
						team2Hit = true;
					}
					if (team2Hit || team1Hit == false) // If no team has won.
						toRemove.Add(MissileInAir[i]); // Remove grounded missile.
					else
					{
						//Else remove both missile lists for a clean slate.
						MissileInAir.Clear();
						toRemove.Clear();
					}
				}

				//Check for impacts mid flight
				else
				{
					if ((MissileInAir[i].CurrentPostion.DistanceTo(Simulation.Team1.Tank.Position) < Constants.BoomRange))
					{
						if((MissileInAir[i].Source.X == Simulation.Team2.Tank.Position.X) && (MissileInAir[i].Source.Y == Simulation.Team2.Tank.Position.Y))
						{
							team1Hit = true;
							toRemove.Add(MissileInAir[i]);
						}
					}

					if (MissileInAir[i].CurrentPostion.DistanceTo(Simulation.Team2.Tank.Position) < Constants.BoomRange)
					{
						if ((MissileInAir[i].Source.X == Simulation.Team1.Tank.Position.X) && (MissileInAir[i].Source.Y == Simulation.Team1.Tank.Position.Y))
						{
							team2Hit = true;
							toRemove.Add(MissileInAir[i]);
						} 
					}
				}

			}
			//Remove missils listed as reached their target.
			foreach (Missile missile in toRemove)
				if(!MissileInAir.Remove(missile)) { logger.Debug("CRITICAL ERROR IN REMOVING MISSILES");  }
			toRemove.Clear();

			//Check for game ending scenarios
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
			else if(Simulation.Team1.Tank.Position.DistanceTo(Simulation.Team2.Tank.Position) < 15)
			{
				gameResult = 3;
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
				//Update all members of the missile class
				Missile missile = new NGAPI.Missile();
				MissileInAir.Add(missile);
				missile.Source = new Position(Simulation.Team1.Tank.Position.X, Simulation.Team1.Tank.Position.Y);
				missile.Target = Simulation.Team1.Tank.MissileTarget;
				missile.TurnsRemaining = (int)missile.Source.DistanceTo(missile.Target) / 30; //M1 Abrams missiles move at 300 m/s
				Simulation.Team1.Tank.FiresThisTurn = false;
				Simulation.Team1.Tank.Cooldown = 4;
				Simulation.Team1.Tank.MisslesLeft--;

				//Calculate the Missile Heading
				double dx = missile.Target.X - missile.Source.X;
				double dy = missile.Target.Y - missile.Source.Y;
				missile.CurrentHeading = (float)(Math.Atan2(dy, dx) * (180 / Math.PI));
				if(missile.CurrentHeading < 0) { missile.CurrentHeading = missile.CurrentHeading + 360; }

				//TARGET TESTING
				if (missile.Target.X != Simulation.Team1.Tank.MissileTarget.X && missile.Target.Y != Simulation.Team1.Tank.MissileTarget.Y) { logger.Debug($"TARGET 1: FAIL {missile.Target.X} {missile.Target.Y}"); }
				else { logger.Debug($"TARGET 1: PASS {missile.Target.X} {missile.Target.Y}"); }

				//SOURCE TESTING
				if (missile.Source.X != Simulation.Team1.Tank.Position.X || missile.Source.Y != Simulation.Team1.Tank.Position.Y) { logger.Debug($"SOURCE 1: FAIL {missile.Source.X} {missile.Source.Y}"); }
				else { logger.Debug($"SOURCE 1: PASS {missile.Source.X} {missile.Source.Y}"); }

				//DIFFERENCE TESTING
				if (dx != missile.Target.X - missile.Source.X || dy != missile.Target.Y - missile.Source.Y) { logger.Debug($"DIFFERENCE 1: FAIL {dx} {dy}"); }
				else { logger.Debug($"DIFFERENCE 1: PASS {dx} {dy}"); }

				//HEADING TESTING
				if (missile.CurrentHeading != (float)(Math.Atan2(dy, dx) * (180 / Math.PI))) { logger.Debug($"HEADING 1: FAIL {missile.CurrentHeading}"); }
				else { logger.Debug($"HEADING 1: PASS {missile.CurrentHeading}"); }

				missile.CurrentPostion = missile.Source;
			}
			if(Simulation.Team2.Tank.FiresThisTurn == true)
			{
				Missile missile2 = new NGAPI.Missile();
				MissileInAir.Add(missile2);
				missile2.Source = new Position(Simulation.Team2.Tank.Position.X, Simulation.Team2.Tank.Position.Y);
				missile2.Target = Simulation.Team2.Tank.MissileTarget;
				missile2.TurnsRemaining = (int)missile2.Source.DistanceTo(missile2.Target) / 30;
				Simulation.Team2.Tank.FiresThisTurn = false;
				Simulation.Team2.Tank.Cooldown = 4;
				Simulation.Team2.Tank.MisslesLeft--;

				double dx = missile2.Target.X - missile2.Source.X;
				double dy = missile2.Target.Y - missile2.Source.Y;
				missile2.CurrentHeading = (float)(Math.Atan2(dy, dx) * (180 / Math.PI));
				if (missile2.CurrentHeading < 0) { missile2.CurrentHeading = missile2.CurrentHeading + 360; }

				//TARGET TESTING
				if (missile2.Target.X != Simulation.Team2.Tank.MissileTarget.X && missile2.Target.Y != Simulation.Team2.Tank.MissileTarget.Y) { logger.Debug($"TARGET 1: FAIL {missile2.Target.X} {missile2.Target.Y}"); }
				else { logger.Debug($"TARGET 1: PASS {missile2.Target.X} {missile2.Target.Y}"); }

				//SOURCE TESTING
				if (missile2.Source.X != Simulation.Team2.Tank.Position.X || missile2.Source.Y != Simulation.Team2.Tank.Position.Y) { logger.Debug($"SOURCE 2: FAIL {missile2.Source.X} {missile2.Source.Y}"); }
				else { logger.Debug($"SOURCE 2: PASS {missile2.Source.X} {missile2.Source.Y}"); }

				//DIFFERENCE TESTING
				if (dx != missile2.Target.X - missile2.Source.X || dy != missile2.Target.Y - missile2.Source.Y) { logger.Debug($"DIFFERENCE 2: FAIL {dx} {dy}"); }
				else { logger.Debug($"DIFFERENCE 2: PASS {dx} {dy}"); }

				//HEADING TESTING
				if (missile2.CurrentHeading != (float)(Math.Atan2(dy, dx) * (180 / Math.PI))) { logger.Debug($"HEADING 1: FAIL {missile2.CurrentHeading}"); }
				else { logger.Debug($"HEADING 1: PASS {missile2.CurrentHeading}"); }

				//Set an initial position for the missile
				missile2.CurrentPostion = missile2.Source;
			}
			//Reload if both tanks are empty
			if (Simulation.Team1.Tank.MisslesLeft == 0 && Simulation.Team2.Tank.MisslesLeft == 0)
			{
				Simulation.Team1.Tank.MisslesLeft = 15;
				Simulation.Team2.Tank.MisslesLeft = 15;
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
			float xlim = Constants.WorldSize.X / 2;
			float ylim = Constants.WorldSize.Y / 2;

			return (pos.X < xlim && pos.X > -xlim) && (pos.Y < ylim && pos.Y > -ylim);
		}

		private void updateEntityPositions()
		{
			//Generate the changes in the X and Y position for all entities
			float X1Tank = Simulation.Team1.Tank.CurrentSpeed * (float)Math.Cos(Simulation.Team1.Tank.CurrentHeading * Math.PI/180);
			float X1UAV = Simulation.Team1.UAV.CurrentSpeed * (float)Math.Cos(Simulation.Team1.UAV.CurrentHeading * Math.PI / 180);
			float X2Tank = Simulation.Team2.Tank.CurrentSpeed * (float)Math.Cos(Simulation.Team2.Tank.CurrentHeading * Math.PI / 180);
			float X2UAV = Simulation.Team2.UAV.CurrentSpeed * (float)Math.Cos(Simulation.Team2.UAV.CurrentHeading * Math.PI / 180);

			float Y1Tank = Simulation.Team1.Tank.CurrentSpeed * (float)Math.Sin(Simulation.Team1.Tank.CurrentHeading * Math.PI / 180);
			float Y1UAV = Simulation.Team1.UAV.CurrentSpeed * (float)Math.Sin(Simulation.Team1.UAV.CurrentHeading * Math.PI / 180);
			float Y2Tank = Simulation.Team2.Tank.CurrentSpeed * (float)Math.Sin(Simulation.Team2.Tank.CurrentHeading * Math.PI / 180);
			float Y2UAV = Simulation.Team2.UAV.CurrentSpeed * (float)Math.Sin(Simulation.Team2.UAV.CurrentHeading * Math.PI / 180);

			//Add the X and Y changes
			Simulation.Team1.Tank.Position = new Position(Simulation.Team1.Tank.Position.X + X1Tank, Simulation.Team1.Tank.Position.Y + Y1Tank);
			Simulation.Team1.UAV.Position = new Position(Simulation.Team1.UAV.Position.X + X1UAV, Simulation.Team1.UAV.Position.Y + Y1UAV);

			Simulation.Team2.Tank.Position = new Position(Simulation.Team2.Tank.Position.X + X2Tank, Simulation.Team2.Tank.Position.Y + Y2Tank);
			Simulation.Team2.UAV.Position = new Position(Simulation.Team2.UAV.Position.X + X2UAV, Simulation.Team2.UAV.Position.Y + Y2UAV);

			List<float> XMissiles = new List<float>();
			List<float> YMissiles = new List<float>();

			//Do the same for each missile
			foreach (Missile missile in MissileInAir)
			{
				float XMissile = 30 * (float)Math.Cos(missile.CurrentHeading * Math.PI/180);
				float YMissile = 30 * (float)Math.Sin(missile.CurrentHeading * Math.PI/180);
				XMissiles.Add(XMissile);
				YMissiles.Add(YMissile);
			}

			for (int i = 0; i < MissileInAir.Count; i++)
			{
                //MissileInAir.ForEach((missile) => { missile.CurrentPostion = new Position(missile.CurrentPostion.X + XMissiles[i], missile.CurrentPostion.Y + YMissiles[i]); });
                MissileInAir[i].CurrentPostion = new Position(MissileInAir[i].CurrentPostion.X + XMissiles[i], MissileInAir[i].CurrentPostion.Y + YMissiles[i]);
			}
		}

		private float angleClamp(float angle)
		{
			return (angle + 360f) % 360f;
		}

		private void entityNeedsInterp(Entity e, out bool heading, out bool speed)
		{
			heading = (e.CurrentHeading != e.TargetHeading);
			speed = (e.CurrentSpeed != e.TargetSpeed);
		}

		private float getAngleDiff(Entity e)
		{
			float nc = angleClamp(e.CurrentHeading);
			float nt = angleClamp(e.TargetHeading);
			float diff = (((nt - nc) + 180f) % 360f) - 180f;
			if (diff < -180f)
				diff += 360f;
			else if (diff > 180f)
				diff -= 360f;
			return diff;
		}

		private void updateEntityVelocities()
		{
			// Get the needed changes
			bool t1h, t1s;
			entityNeedsInterp(Simulation.Team1.Tank, out t1h, out t1s);
			bool t2h, t2s;
			entityNeedsInterp(Simulation.Team2.Tank, out t2h, out t2s);
			bool u1h, u1s;
			entityNeedsInterp(Simulation.Team1.UAV, out u1h, out u1s);
			bool u2h, u2s;
			entityNeedsInterp(Simulation.Team2.UAV, out u2h, out u2s);

			// Interpolate Team 1 Tank Heading
			if (t1h)
			{
				float diff = getAngleDiff(Simulation.Team1.Tank);
				int dir = Math.Sign(diff);
				bool substep = Math.Abs(diff) < TURN_RATE;
				Simulation.Team1.Tank.CurrentHeading = angleClamp(Simulation.Team1.Tank.CurrentHeading + (substep ? diff : TURN_RATE * dir));
			}
			// Interpolate Team 1 UAV Heading
			if (u1h)
			{
				float diff = getAngleDiff(Simulation.Team1.UAV);
				int dir = Math.Sign(diff);
				bool substep = Math.Abs(diff) < TURN_RATE;
				Simulation.Team1.UAV.CurrentHeading = angleClamp(Simulation.Team1.UAV.CurrentHeading + (substep ? diff : TURN_RATE * dir));
			}

			// Interpolate Team 2 Tank Heading
			if (t2h)
			{
				float diff = getAngleDiff(Simulation.Team2.Tank);
				int dir = Math.Sign(diff);
				bool substep = Math.Abs(diff) < TURN_RATE;
				Simulation.Team2.Tank.CurrentHeading = angleClamp(Simulation.Team2.Tank.CurrentHeading + (substep ? diff : TURN_RATE * dir));
			}
			// Interpolate Team 2 UAV Heading
			if (u2h)
			{
				float diff = getAngleDiff(Simulation.Team2.UAV);
				int dir = Math.Sign(diff);
				bool substep = Math.Abs(diff) < TURN_RATE;
				Simulation.Team2.UAV.CurrentHeading = angleClamp(Simulation.Team2.UAV.CurrentHeading + (substep ? diff : TURN_RATE * dir));
			}

			// Interpolate Team 1 Tank Speed
			if (t1s)
			{
				float diff = Simulation.Team1.Tank.TargetSpeed - Simulation.Team1.Tank.CurrentSpeed;
				int dir = Math.Sign(diff);
				bool substep = Math.Abs(diff) < ACC_RATE;
				Simulation.Team1.Tank.CurrentSpeed = angleClamp(Simulation.Team1.Tank.CurrentSpeed + (substep ? diff : ACC_RATE * dir));
			}
			// Interpolate Team 1 UAV Speed
			if (u1s)
			{
				float diff = Simulation.Team1.UAV.TargetSpeed - Simulation.Team1.UAV.CurrentSpeed;
				int dir = Math.Sign(diff);
				bool substep = Math.Abs(diff) < ACC_RATE;
				Simulation.Team1.UAV.CurrentSpeed = angleClamp(Simulation.Team1.UAV.CurrentSpeed + (substep ? diff : ACC_RATE * dir));
			}

			// Interpolate Team 2 Tank Speed
			if (t2s)
			{
				float diff = Simulation.Team2.Tank.TargetSpeed - Simulation.Team2.Tank.CurrentSpeed;
				int dir = Math.Sign(diff);
				bool substep = Math.Abs(diff) < ACC_RATE;
				Simulation.Team2.Tank.CurrentSpeed = angleClamp(Simulation.Team2.Tank.CurrentSpeed + (substep ? diff : ACC_RATE * dir));
			}
			// Interpolate Team 2 UAV Speed
			if (u2s)
			{
				float diff = Simulation.Team2.UAV.TargetSpeed - Simulation.Team2.UAV.CurrentSpeed;
				int dir = Math.Sign(diff);
				bool substep = Math.Abs(diff) < ACC_RATE;
				Simulation.Team2.UAV.CurrentSpeed = angleClamp(Simulation.Team2.UAV.CurrentSpeed + (substep ? diff : ACC_RATE * dir));
			}
		}

		private void UAVScan()
		{
			float viewRadius = Constants.UAVAltitude * (float)Math.Tan(Constants.UAVScanAngle);

			if(Simulation.Team1.UAV.Position.DistanceTo(Simulation.Team2.Tank.Position) < viewRadius)
			{
				Simulation.Team1.UAV.DetectedTankThisTurn = true;
				Simulation.Team1.UAV.LastKnownPosition = Simulation.Team2.Tank.Position;
			}

			if (Simulation.Team2.UAV.Position.DistanceTo(Simulation.Team1.Tank.Position) < viewRadius)
			{
				Simulation.Team2.UAV.DetectedTankThisTurn = true;
				Simulation.Team2.UAV.LastKnownPosition = Simulation.Team1.Tank.Position;
			}
			if (Simulation.Team1.Tank.Position.DistanceTo(Simulation.Team2.Tank.Position) < Constants.TankScanRange)
			{
				Simulation.Team1.UAV.DetectedTankThisTurn = true;
				Simulation.Team1.UAV.LastKnownPosition = Simulation.Team2.Tank.Position;
			}

			if (Simulation.Team2.Tank.Position.DistanceTo(Simulation.Team1.Tank.Position) < Constants.TankScanRange)
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
