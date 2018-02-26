using Eto;
using Eto.Forms;
using Eto.Wpf.Forms;
using Eto.CustomControls;
using NGAPI;
using System;

namespace NGSim
{
	// control to use in your eto.forms code
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

		public Position RedTankXY
		{
			get { return Handler.RedTankXY; }
			set { Handler.RedTankXY = value; }
		}

		public Position RedMissileXY
		{
			get { return Handler.RedMissileXY; }
			set { Handler.RedMissileXY = value; }
		}

		public int RedMissilesRemaining
		{
			get { return Handler.RedMissilesRemaining; }
			set { Handler.RedMissilesRemaining = value; }
		}

		public Position LastKnownRedTankXY
		{
			get { return Handler.LastKnownRedTankXY; }
			set { Handler.LastKnownRedTankXY = value; }
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

		// interface to the platform implementations
		public interface IStateInfoTextArea : Control.IHandler
		{
			Position RedUAVXY { get; set; }
			Position RedTankXY { get; set; }
			Position RedMissileXY { get; set; }
			int RedMissilesRemaining { get; set; }
			Position LastKnownRedTankXY { get; set; }
			int TurnsElapsed { get; set; }
			float WinPercent { get; set; }
			int GamesRun { get; set; }
			string Warnings { get; set; }
			event EventHandler PropertyChanged;
			void OnPropertyChanged(EventArgs e);
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
			Control.AppendText((RedUAVXY.X).ToString());
			Control.AppendText(",");
			Control.AppendText((RedUAVXY.Y).ToString());
			Control.AppendText("\t\t\t0,0\n");
			
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

		private Position redTankXY;
		public Position RedTankXY
		{
			get { return redTankXY; }
			set { redTankXY = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private Position redMissileXY;
		public Position RedMissileXY
		{
			get { return redMissileXY; }
			set { redMissileXY = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private int redMissilesRemaining;
		public int RedMissilesRemaining
		{
			get { return redMissilesRemaining; }
			set { redMissilesRemaining = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private Position lastKnownRedTankXY;
		public Position LastKnownRedTankXY
		{
			get { return lastKnownRedTankXY; }
			set { lastKnownRedTankXY = value; OnPropertyChanged(EventArgs.Empty); }
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
