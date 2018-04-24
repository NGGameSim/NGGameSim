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

		public Position MyUAVXY
		{
			get { return Handler.MyUAVXY; }
			set { Handler.MyUAVXY = value; }
		}

		public Position MyTankXY
		{
			get { return Handler.MyTankXY; }
			set { Handler.MyTankXY = value; }
		}

		public Position MyMissileXY
		{
			get { return Handler.MyMissileXY; }
			set { Handler.MyMissileXY = value; }
		}

		public int MyMissilesRemaining
		{
			get { return Handler.MyMissilesRemaining; }
			set { Handler.MyMissilesRemaining = value; }
		}

		public Position LastKnownEnemyTankXY
		{
			get { return Handler.LastKnownEnemyTankXY; }
			set { Handler.LastKnownEnemyTankXY = value; }
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
			Position MyUAVXY { get; set; }
			Position MyTankXY { get; set; }
			Position MyMissileXY { get; set; }
			int MyMissilesRemaining { get; set; }
			Position LastKnownEnemyTankXY { get; set; }
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
			Control.AppendText("UAV Position:\t0,0\n");
			Control.AppendText("Tank Position:\t0,0\n");
			Control.AppendText("Missile Position:\t0,0\n");
			Control.AppendText("Missiles Left:\t0\n");
			Control.AppendText("Enemy Last Seen:\t0,0\n");
			Control.AppendText("\n");
			Control.AppendText("Game #1\n");
			Control.AppendText("Turns Elapsed: 0\n");
			Control.AppendText("Win %: 50%\n");
			Control.AppendText("\n");
			Control.AppendText("Warnings:\n");
			PropertyChanged += UpdateText;
		}

		private void UpdateText(object sender, EventArgs e)
		{
			Control.Clear();
			string text = String.Format("{0,35}", "State Information\n");
			text += String.Format("{0,-18} {1,13}\n", "UAV Position:", MyUAVXY);
			text += String.Format("{0,-18} {1,14}\n", "Tank Position:", MyTankXY);
			text += String.Format("{0,-18} {1,18}\n", "Missiles Left:", MyMissilesRemaining);
			text += String.Format("{0,-18} {1,10}\n\n", "Enemy Last Seen:", LastKnownEnemyTankXY);
			text += String.Format("Game #{0}\n", GamesRun);
			text += String.Format("Turns Elapsed: {0}\n", TurnsElapsed);
			if (GamesRun <= 1)
			{
				text += String.Format("Win Percent: {0,10}%\n", "0");
			}
			else
			{
				text += String.Format("Win Percent: {0,10}%\n", WinPercent);
			}
			text += String.Format("\nWarnings:\n{0}", Warnings);
			Control.AppendText(text);
		}

		public event EventHandler PropertyChanged;
		public void OnPropertyChanged(EventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}

		private Position myUAVXY;
		public Position MyUAVXY
		{
			get { return myUAVXY; }
			set { myUAVXY = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private Position myTankXY;
		public Position MyTankXY
		{
			get { return myTankXY; }
			set { myTankXY = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private Position myMissileXY;
		public Position MyMissileXY
		{
			get { return myMissileXY; }
			set { myMissileXY = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private int myMissilesRemaining;
		public int MyMissilesRemaining
		{
			get { return myMissilesRemaining; }
			set { myMissilesRemaining = value; OnPropertyChanged(EventArgs.Empty); }
		}

		private Position lastKnownEnemyTankXY;
		public Position LastKnownEnemyTankXY
		{
			get { return lastKnownEnemyTankXY; }
			set { lastKnownEnemyTankXY = value; OnPropertyChanged(EventArgs.Empty); }
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
