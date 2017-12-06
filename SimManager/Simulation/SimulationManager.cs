using NGAPI;
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
		private Position[] MisslesInAirAimedAt = new Position[20];
		private int[] turnsUntilMisslesHit = new int[20];
		private int missleIndex = 0;

		public SimulationManager()
		{

		}
		public int RunSimulation()
		{ 
			Simulation = new NGAPI.Simulation();
			API.Simulation = Simulation;
			//Set a random Position for the Tank and UAV, between -19999 and 19999, field bounds are -20000 and 20000
			int randX = rand.Next(1, (positiveBoundX - negativeBoundX) - negativeBoundX);
			int randY = rand.Next(1, (positiveBoundY - negativeBoundY) - negativeBoundY);
			Simulation.Team1.Tank.Position = new Position(randX, randY);
			Simulation.Team1.UAV.Position = new Position(randX, randY);

			randX = rand.Next(1, (positiveBoundX - negativeBoundX) - negativeBoundX);
			randY = rand.Next(1, (positiveBoundY - negativeBoundY) - negativeBoundY);
			Simulation.Team1.Tank.Position = new Position(randX, randY);
			Simulation.Team1.UAV.Position = new Position(randX, randY);

			//initialize the missle holding system
			for(int i=0; i<20; i++)
			{
				turnsUntilMisslesHit[i] = 0;
			}

			for(int i=0; i<maxTurns && gameResult == 0; i++)
			{
				Update();
			}

			return gameResult;
		}

		public void Update()
		{
			
			bool team1Lost = false;
			bool team2Lost = false;

			SetTeam1Friendly();
			algo1();
			SetTeam2Friendly();
			algo2();

			//update the "DetectedThisTurn" variable
			Simulation.Team1.UAV.DetectedThisTurn = false;

			//change UAV and Tank Heading
			UpdateHeading(Simulation.Team1.Tank);
			UpdateHeading(Simulation.Team1.UAV);

			//change UAV and Tank Speed
			UpdateSpeed(Simulation.Team1.Tank, timeInTurn);
			UpdateSpeed(Simulation.Team1.UAV, timeInTurn);

			//move tank and UAV with new heading and speed values
			MoveWithSpeed(Simulation.Team1.Tank.Position,Simulation.Team1.Tank.CurrentHeading, Simulation.Team1.Tank.CurrentSpeed, timeInTurn);
			MoveWithSpeed(Simulation.Team1.UAV.Position, Simulation.Team1.UAV.CurrentHeading, Simulation.Team1.UAV.CurrentSpeed, timeInTurn);

			//do the same thing as above for Team2
			Simulation.Team2.UAV.DetectedThisTurn = false;

			UpdateHeading(Simulation.Team2.Tank);
			UpdateHeading(Simulation.Team2.UAV);

			UpdateSpeed(Simulation.Team2.Tank, timeInTurn);
			UpdateSpeed(Simulation.Team2.UAV, timeInTurn);

			MoveWithSpeed(Simulation.Team2.Tank.Position, Simulation.Team2.Tank.CurrentHeading, Simulation.Team2.Tank.CurrentSpeed, timeInTurn);
			MoveWithSpeed(Simulation.Team2.UAV.Position, Simulation.Team2.UAV.CurrentHeading, Simulation.Team2.UAV.CurrentSpeed, timeInTurn);

			//check for bounds
			if (CheckOutOfBounds(Simulation.Team1.Tank.Position) || CheckOutOfBounds(Simulation.Team1.UAV.Position))
				team1Lost = true;

			if (CheckOutOfBounds(Simulation.Team2.Tank.Position) || CheckOutOfBounds(Simulation.Team2.UAV.Position))
				team2Lost = true;

			//fire new missiles
			if(Simulation.Team1.Tank.Missle1FiredThisTurn)
			{
				MisslesInAirAimedAt[missleIndex] = Simulation.Team1.Tank.Missle1FiredTarget;
				turnsUntilMisslesHit[missleIndex] = Simulation.Team1.Tank.TurnsItTakesMissle1;
				missleIndex++;
				Simulation.Team1.Tank.Missle1FiredThisTurn = false;
			}
			if (Simulation.Team1.Tank.Missle2FiredThisTurn)
			{
				MisslesInAirAimedAt[missleIndex] = Simulation.Team1.Tank.Missle2FiredTarget;
				turnsUntilMisslesHit[missleIndex] = Simulation.Team1.Tank.TurnsItTakesMissle2;
				missleIndex++;
				Simulation.Team1.Tank.Missle2FiredThisTurn = false;
			}

			if (Simulation.Team2.Tank.Missle1FiredThisTurn)
			{
				MisslesInAirAimedAt[missleIndex] = Simulation.Team2.Tank.Missle1FiredTarget;
				turnsUntilMisslesHit[missleIndex] = Simulation.Team2.Tank.TurnsItTakesMissle1;
				missleIndex++;
				Simulation.Team2.Tank.Missle1FiredThisTurn = false;
			}
			if (Simulation.Team2.Tank.Missle2FiredThisTurn)
			{
				MisslesInAirAimedAt[missleIndex] = Simulation.Team2.Tank.Missle2FiredTarget;
				turnsUntilMisslesHit[missleIndex] = Simulation.Team2.Tank.TurnsItTakesMissle2;
				missleIndex++;
				Simulation.Team2.Tank.Missle2FiredThisTurn = false;
			}

			//Handle existing missiles(including the just fired)
			for(int i=0; i < missleIndex; i++)
			{
				if(turnsUntilMisslesHit[i] == 1)
				{
					if (MisslesInAirAimedAt[i].DistanceTo(Simulation.Team1.Tank.Position) < boomRange) {
						team1Lost = true;
					}
					if (MisslesInAirAimedAt[i].DistanceTo(Simulation.Team2.Tank.Position) < boomRange)
					{
						team2Lost = true;
					}
					turnsUntilMisslesHit[i] = 0;
				}
				else if(turnsUntilMisslesHit[i] > 1)
				{
					turnsUntilMisslesHit[i]--;
				}
			}

			if (team1Lost && team2Lost)
			{
				gameResult = 3;
			}
			else if (team1Lost)
			{
				gameResult = 2;
			}
			else if (team2Lost)
			{
				gameResult = 1;
			}

			//Check if neither tank has any missiles left, it's a draw
			if(Simulation.Team1.Tank.MisslesLeft == 0 && Simulation.Team1.Tank.MisslesLeft ==0)
			{
				gameResult = 3;
			}

			numberOfTurns++;
		}

		public void SetTeam1Friendly()
		{
			API.FriendlyTank = Simulation.Team1.Tank;
			API.FriendlyUAV = Simulation.Team1.UAV;
			API.EnemyTank = Simulation.Team2.Tank;
		}

		public void SetTeam2Friendly()
		{
			API.FriendlyTank = Simulation.Team2.Tank;
			API.FriendlyUAV = Simulation.Team2.UAV;
			API.EnemyTank = Simulation.Team1.Tank;
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

		public void UpdateHeading(Tank tank)
		{
			if (tank.MoveDirection == Direction.Left)
			{
				float targetHeadingChange = (tank.TargetHeading - tank.CurrentHeading) % 360;
				if (targetHeadingChange <= headingChangePerTurnTank)
				{
					tank.TargetHeading = tank.CurrentHeading;
				}
				else
				{
					tank.TargetHeading = (tank.CurrentHeading + headingChangePerTurnTank) % 360;
				}
			}
			else if (tank.MoveDirection == Direction.Right)
			{
				float targetHeadingChange = (tank.CurrentHeading - tank.TargetHeading) % 360;
				if (targetHeadingChange <= headingChangePerTurnTank)
				{
					tank.TargetHeading = tank.CurrentHeading;
				}
				else
				{
					tank.TargetHeading = (tank.CurrentHeading - headingChangePerTurnTank) % 360;
				}
			}
		}

		public void UpdateHeading(UAV uav)
		{
			if (uav.MoveDirection == Direction.Left)
			{
				float targetHeadingChange = (uav.TargetHeading - uav.CurrentHeading) % 360;
				if (targetHeadingChange <= headingChangePerTurnUAV)
				{
					uav.TargetHeading = uav.CurrentHeading;
				}
				else
				{
					uav.TargetHeading = (uav.CurrentHeading + headingChangePerTurnUAV) % 360;
				}
			}
			else if (uav.MoveDirection == Direction.Right)
			{
				float targetHeadingChange = (uav.CurrentHeading - uav.TargetHeading) % 360;
				if (targetHeadingChange <= headingChangePerTurnUAV)
				{
					uav.TargetHeading = uav.CurrentHeading;
				}
				else
				{
					uav.TargetHeading = (uav.CurrentHeading - headingChangePerTurnUAV) % 360;
				}
			}
		}

		//note that the targetSpeed cannot be less than zero or greater than the max
		internal void UpdateSpeed(Tank tank, float time)
		{
			float targetAcceleration = tank.TargetSpeed - tank.CurrentSpeed;
			if(Math.Abs(targetAcceleration) <= accelerationPerTurnTank)
			{
				tank.CurrentSpeed = tank.TargetSpeed;
			}
			else if(tank.TargetSpeed > tank.CurrentSpeed)
			{
				tank.CurrentSpeed += accelerationPerTurnTank*time;
			}
			else if (tank.TargetSpeed < tank.CurrentSpeed)
			{
				tank.CurrentSpeed -= accelerationPerTurnTank*time;
			}
			else
			{
				//Should never happen
			}
		}

		internal void UpdateSpeed(UAV uav, float time)
		{
			float targetAcceleration = uav.TargetSpeed - uav.CurrentSpeed;
			if (accelerationPerTurnUAV >= Math.Abs(targetAcceleration))
			{
				uav.CurrentSpeed = uav.TargetSpeed;
			}
			else if (uav.TargetSpeed > uav.CurrentSpeed)
			{
				uav.CurrentSpeed += accelerationPerTurnTank;
			}
			else if (uav.TargetSpeed < uav.CurrentSpeed)
			{
				uav.CurrentSpeed -= accelerationPerTurnTank;
			}
			else
			{
				//Should never happen
			}
		}

		public void MoveWithSpeed(Position p, float direction, float speed, float time)
		{
			float speedX = speed * (float)Math.Cos(Math.PI / 180 * direction);
			float speedY = speed * (float)Math.Sin(Math.PI / 180 * direction);
			p.X += speedX * time;
			p.Y += speedY * time;
		}

		public bool CheckOutOfBounds(Position p)
		{
			if (p.X < negativeBoundX || p.X > positiveBoundX || p.Y < negativeBoundY || p.Y > positiveBoundY)
				return true;
			return false;
		}

		public int getRandomInteger(int maximum)
		{
			return rand.Next(0, maximum);
		}
	}
}
