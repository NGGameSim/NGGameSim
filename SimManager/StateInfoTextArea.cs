using Eto;
using Eto.Forms;
using NGAPI;

namespace NGSim
{
	// control to use in your eto.forms code
	[Handler(typeof(IStateInfoTextArea))]
	public class StateInfoTextArea : TextArea
	{
		new IStateInfoTextArea Handler
		{ get { return (IStateInfoTextArea)base.Handler; } }

		public Position UAVXY
		{
			get { return Handler.UAVXY; }
			set { Handler.UAVXY = value; }
		}

		public Position TankXY
		{
			get { return Handler.TankXY; }
			set { Handler.TankXY = value; }
		}

		public Position MissileXY
		{
			get { return Handler.MissileXY; }
			set { Handler.MissileXY = value; }
		}

		public int MissilesRemaining
		{
			get { return Handler.MissilesRemaining; }
			set { Handler.MissilesRemaining = value; }
		}

		public int TurnsElapsed
		{
			get { return Handler.TurnsElapsed; }
			set { Handler.TurnsElapsed = value; }
		}

		public float WinPercent
		{
			get { return Handler.WinPercent; }
			set { Handler.WinPercent = value; }
		}

		public int GamesRun
		{
			get { return Handler.GamesRun; }
			set { Handler.GamesRun = value; }
		}

		public string Warnings
		{
			get { return Handler.Warnings; }
			set { Handler.Warnings = value; }
		}

		public Position LastKnownTankXY
		{
			get { return Handler.LastKnownTankXY; }
			set { Handler.LastKnownTankXY = value; }
		}

		// interface to the platform implementations
		public interface IStateInfoTextArea : TextArea.IHandler
		{
			Position UAVXY { get; set; }
			Position TankXY { get; set; }
			Position MissileXY { get; set; }
			int MissilesRemaining { get; set; }
			int TurnsElapsed { get; set; }
			float WinPercent { get; set; }
			int GamesRun { get; set; }
			string Warnings { get; set; }
			Position LastKnownTankXY { get; set; }
		}
	}
}
