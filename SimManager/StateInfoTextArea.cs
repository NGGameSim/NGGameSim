using Eto;
using Eto.Forms;
using Eto.Wpf.Forms;
using NGAPI;
using System;

namespace NGSim
{
	[Handler(typeof(IStateInfoTextArea))]
	public class StateInfoTextArea : Control
	{
		new IStateInfoTextArea Handler
		{
			get { return (IStateInfoTextArea)base.Handler; }
		}

		public Position RedUAVXY
		{
			get { return Handler.RedUAVXY; }
			set { Handler.RedUAVXY = value; }
		}

		public Position BlueUAVXY
		{
			get { return Handler.BlueUAVXY; }
			set { Handler.BlueUAVXY = value; }
		}

		public Position RedTankXY
		{
			get { return Handler.RedTankXY; }
			set { Handler.RedTankXY = value; }
		}

		public Position BlueTankXY
		{
			get { return Handler.BlueTankXY; }
			set { Handler.BlueTankXY = value; }
		}

		public Position RedMissileXY
		{
			get { return Handler.RedMissileXY; }
			set { Handler.RedMissileXY = value; }
		}

		public Position BlueMissileXY
		{
			get { return Handler.BlueMissileXY; }
			set { Handler.BlueMissileXY = value; }
		}

		public int RedMissilesRemaining
		{
			get { return Handler.RedMissilesRemaining; }
			set { Handler.RedMissilesRemaining = value; }
		}

		public int BlueMissilesRemaining
		{
			get { return Handler.BlueMissilesRemaining; }
			set { Handler.BlueMissilesRemaining = value; }
		}

		public Position LastKnownRedTankXY
		{
			get { return Handler.LastKnownRedTankXY; }
			set { Handler.LastKnownRedTankXY = value; }
		}

		public Position LastKnownBlueTankXY
		{
			get { return Handler.LastKnownBlueTankXY; }
			set { Handler.LastKnownBlueTankXY = value; }
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
			set { Handler.Warnings = value; OnPropertyChanged(EventArgs.Empty); }
		}

		public event EventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(EventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}

		public void GameReset()
		{

		}

		// interface to the platform implementations
		public interface IStateInfoTextArea : Control.IHandler
		{
			Position RedUAVXY { get; set; }
			Position BlueUAVXY { get; set; }
			Position RedTankXY { get; set; }
			Position BlueTankXY { get; set; }
			Position RedMissileXY { get; set; }
			Position BlueMissileXY { get; set; }
			int RedMissilesRemaining { get; set; }
			int BlueMissilesRemaining { get; set; }
			Position LastKnownRedTankXY { get; set; }
			Position LastKnownBlueTankXY { get; set; }
			int TurnsElapsed { get; set; }
			float WinPercent { get; set; }
			int GamesRun { get; set; }
			string Warnings { get; set; }
			event EventHandler PropertyChanged;
			void OnPropertyChanged(EventArgs e);
			void GameReset();
		}
	}

	public class StateInfoTextAreaHandler : WpfControl<System.Windows.Controls.TextBox, StateInfoTextArea , StateInfoTextArea.ICallback>, StateInfoTextArea.IStateInfoTextArea
	{
		public StateInfoTextAreaHandler()
		{
			Control = new System.Windows.Controls.TextBox{ IsReadOnly = true };
			Control.AppendText("\t\tRed Team\t\tBlue Team\n");
			Control.AppendText("UAV Position:\t0,0\t\t\t0,0\n");
			Control.AppendText("Tank Position:\t0,0\t\t\t0,0\n");
			Control.AppendText("Missile Position:\t0,0\t\t\t0,0\n");
			Control.AppendText("Missiles Left:\t0\t\t\t0\n");
			Control.AppendText("Enemy Last Seen:\t0,0\t\t\t0,0\n");
			Control.AppendText("\n");
			Control.AppendText("Game #1\n");
			Control.AppendText("Turns Elapsed: 0\n");
			Control.AppendText("Red Win %: 50%\t\t Blue Win %: 50%\n");
			Control.AppendText("\n");
			Control.AppendText("Warnings:\n");
			PropertyChanged += UpdateText;
		}

		private void UpdateText(object sender, EventArgs e)
		{
			Control.Clear();
			Control.AppendText("\t\tRed Team\t\tBlue Team\n");
			Control.AppendText("UAV Position:\t");
			Control.AppendText(RedUAVXY.X.ToString());
			Control.AppendText(",");
			Control.AppendText(RedUAVXY.Y.ToString());
			Control.AppendText("\t\t\t");
			Control.AppendText(BlueUAVXY.X.ToString());
			Control.AppendText(",");
			Control.AppendText(BlueUAVXY.Y.ToString());
			Control.AppendText("\n");

			Control.AppendText("Tank Position:\t");
			Control.AppendText(RedTankXY.X.ToString());
			Control.AppendText(",");
			Control.AppendText(RedTankXY.Y.ToString());
			Control.AppendText("\t\t\t");
			Control.AppendText(BlueTankXY.X.ToString());
			Control.AppendText(",");
			Control.AppendText(BlueTankXY.Y.ToString());
			Control.AppendText("\n");

			Control.AppendText("Missile Position:\t");
			Control.AppendText(RedMissileXY.X.ToString());
			Control.AppendText(",");
			Control.AppendText(RedMissileXY.Y.ToString());
			Control.AppendText("\t\t\t");
			Control.AppendText(BlueMissileXY.X.ToString());
			Control.AppendText(",");
			Control.AppendText(BlueMissileXY.Y.ToString());
			Control.AppendText("\n");

			Control.AppendText("Missiles Left:\t");
			Control.AppendText(RedMissilesRemaining.ToString());
			Control.AppendText("\t\t\t");
			Control.AppendText(BlueMissilesRemaining.ToString());
			Control.AppendText("\n");

			Control.AppendText("Enemy Last Seen:\t0,0\t\t\t0,0\n");
			Control.AppendText("Enemy Last Seen:\t");
			Control.AppendText(LastKnownBlueTankXY.X.ToString());
			Control.AppendText(",");
			Control.AppendText(LastKnownBlueTankXY.Y.ToString());
			Control.AppendText("\t\t\t");
			Control.AppendText(lastKnownRedTankXY.X.ToString());
			Control.AppendText(",");
			Control.AppendText(lastKnownRedTankXY.Y.ToString());
			Control.AppendText("\n");

			Control.AppendText("\n");
			Control.AppendText("Game #");
			Control.AppendText(GamesRun.ToString());
			Control.AppendText("\n");

			Control.AppendText("Turns Elapsed: ");
			Control.AppendText(TurnsElapsed.ToString());
			Control.AppendText("\n");

			Control.AppendText("Red Win %: ");
			Control.AppendText(WinPercent.ToString());
			Control.AppendText("%\t\t Blue Win %: ");
			Control.AppendText((100 - WinPercent).ToString());
			Control.AppendText("%\n");

			Control.AppendText("\n");
			Control.AppendText("Warnings:\n");
			Control.AppendText(Warnings);
		}

		public void GameReset()
		{
			Control.Clear();
			Control.AppendText("\t\tRed Team\t\tBlue Team\n");
			Control.AppendText("UAV Position:\t0,0\t\t\t0,0\n");
			Control.AppendText("Tank Position:\t0,0\t\t\t0,0\n");
			Control.AppendText("Missile Position:\t0,0\t\t\t0,0\n");
			Control.AppendText("Missiles Left:\t0\t\t\t0\n");
			Control.AppendText("Enemy Last Seen:\t0,0\t\t\t0,0\n");
			Control.AppendText("\n");
			Control.AppendText("Game #1\n");
			Control.AppendText("Turns Elapsed: 0\n");
			Control.AppendText("Red Win %: 50%\t\t Blue Win %: 50%\n");
			Control.AppendText("\n");
			Control.AppendText("Warnings:\n");
		}

		public event EventHandler PropertyChanged;
		public void OnPropertyChanged(EventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}

		private Position redUAVXY;
		public Position RedUAVXY
		{
			get { return redUAVXY; }
			set { redUAVXY = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private Position blueUAVXY;
		public Position BlueUAVXY
		{
			get { return blueUAVXY; }
			set { blueUAVXY = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private Position redTankXY;
		public Position RedTankXY
		{
			get { return redTankXY; }
			set { redTankXY = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private Position blueTankXY;
		public Position BlueTankXY
		{
			get { return blueTankXY; }
			set { blueTankXY = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private Position redMissileXY;
		public Position RedMissileXY
		{
			get { return redMissileXY; }
			set { redMissileXY = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private Position blueMissileXY;
		public Position BlueMissileXY
		{
			get { return blueMissileXY; }
			set { blueMissileXY = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private int redMissilesRemaining;
		public int RedMissilesRemaining
		{
			get { return redMissilesRemaining; }
			set { redMissilesRemaining = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private int blueMissilesRemaining;
		public int BlueMissilesRemaining
		{
			get { return blueMissilesRemaining; }
			set { blueMissilesRemaining = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private Position lastKnownRedTankXY;
		public Position LastKnownRedTankXY
		{
			get { return lastKnownRedTankXY; }
			set { lastKnownRedTankXY = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private Position lastKnownblueTankXY;
		public Position LastKnownBlueTankXY
		{
			get { return lastKnownblueTankXY; }
			set { lastKnownblueTankXY = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private int turnsElapsed;
		public int TurnsElapsed
		{
			get { return turnsElapsed; }
			set { turnsElapsed = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private float winPercent;
		public float WinPercent
		{
			get { return winPercent; }
			set { winPercent = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private int gamesRun;
		public int GamesRun
		{
			get { return gamesRun; }
			set { gamesRun = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private string warnings;
		public string Warnings
		{
			get { return warnings; }
			set { warnings = value; OnPropertyChanged(EventArgs.Empty); }
		}
	}
}
