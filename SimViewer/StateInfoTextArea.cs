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
			Control.AppendText("Win %: 50%\t Lose %: 50%\n");
			Control.AppendText("\n");
			Control.AppendText("Warnings:\n");
			PropertyChanged += UpdateText;
		}

		private void UpdateText(object sender, EventArgs e)
		{
			Console.WriteLine("A field was changed...");
			Control.Clear();
			Control.AppendText("UAV Position:\t");
			Control.AppendText(MyUAVXY.X.ToString());
			Control.AppendText(",");
			Control.AppendText(MyUAVXY.Y.ToString());
			Control.AppendText("\n");

			Control.AppendText("Tank Position:\t");
			Control.AppendText(MyTankXY.X.ToString());
			Control.AppendText(",");
			Control.AppendText(MyTankXY.Y.ToString());
			Control.AppendText("\n");

			Control.AppendText("Missile Position:\t");
			Control.AppendText(MyMissileXY.X.ToString());
			Control.AppendText(",");
			Control.AppendText(MyMissileXY.Y.ToString());
			Control.AppendText("\n");

			Control.AppendText("Missiles Left:\t");
			Control.AppendText(MyMissilesRemaining.ToString());
			Control.AppendText("\n");

			Control.AppendText("Enemy Last Seen:\t");
			Control.AppendText(lastKnownEnemyTankXY.X.ToString());
			Control.AppendText(",");
			Control.AppendText(lastKnownEnemyTankXY.Y.ToString());
			Control.AppendText("\n");

			Control.AppendText("\n");
			Control.AppendText("Game #");
			Control.AppendText(GamesRun.ToString());
			Control.AppendText("\n");

			Control.AppendText("Turns Elapsed: ");
			Control.AppendText(TurnsElapsed.ToString());
			Control.AppendText("\n");

			Control.AppendText("Win %: ");
			Control.AppendText(WinPercent.ToString());
			Control.AppendText("%\t Lose %: ");
			Control.AppendText((100 - WinPercent).ToString());
			Control.AppendText("%\n");

			Control.AppendText("\n");
			Control.AppendText("Warnings:\n");
			Control.AppendText(Warnings);
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
