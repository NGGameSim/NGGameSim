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
			Control = new System.Windows.Controls.TextBox{ IsReadOnly = true, TextWrapping = System.Windows.TextWrapping.Wrap };
			string text = String.Format("{0,35} {1,35}", "Red Team", "Blue Team\n");
			text += String.Format("{0,-18} {1,13} {2,40}\n", "UAV Position:", RedUAVXY, BlueUAVXY);
			text += String.Format("{0,-18} {1,14} {2,40}\n", "Tank Position:", RedTankXY, BlueTankXY);
			text += String.Format("{0,-18} {1,18} {2,42}\n", "Missiles Left:", RedMissilesRemaining, BlueMissilesRemaining);
			text += String.Format("{0,-18} {1,10} {2,40}\n\n", "Enemy Last Seen:", LastKnownBlueTankXY, LastKnownRedTankXY);
			text += String.Format("Game #{0}\n", GamesRun);
			text += String.Format("Turns Elapsed: {0}\n", TurnsElapsed);
			if (GamesRun <= 1)
			{
				text += String.Format("Red Win: {0,10}%\t{1,10}{2,10}%\n", "0", "Blue Win: ", "0");
			}
			else
			{
				text += String.Format("Red Win: {0,10}%\t{1,10}{2,10}%\n", WinPercent, "Blue Win: ", (100 - WinPercent));
			}
			text += String.Format("\nWarnings:\n{0}", Warnings);
			Control.AppendText(text);
			PropertyChanged += UpdateText;
		}

		private void UpdateText(object sender, EventArgs e)
		{
			Control.Clear();
			string text = String.Format("{0,35} {1,35}", "Red Team", "Blue Team\n");
			text += String.Format("{0,-18} {1,13} {2,40}\n", "UAV Position:", RedUAVXY, BlueUAVXY);
			text += String.Format("{0,-18} {1,14} {2,40}\n", "Tank Position:", RedTankXY, BlueTankXY);
			text += String.Format("{0,-18} {1,18} {2,42}\n", "Missiles Left:", RedMissilesRemaining, BlueMissilesRemaining);
			text += String.Format("{0,-18} {1,10} {2,40}\n\n", "Enemy Last Seen:", LastKnownBlueTankXY, LastKnownRedTankXY);
			text += String.Format("Game #{0}\n", GamesRun);
			text += String.Format("Turns Elapsed: {0}\n", TurnsElapsed);
			if(GamesRun <= 1)
			{
				text += String.Format("Red Win: {0,10}%\t{1,10}{2,10}%\n", "0", "Blue Win: ", "0");
			}
			else
			{
				text += String.Format("Red Win: {0,10}%\t{1,10}{2,10}%\n", WinPercent, "Blue Win: ", (100 - WinPercent));
			}
			text += String.Format("\nWarnings:\n{0}", Warnings);
			Control.AppendText(text);
		}

		public void GameReset()
		{
			Control.Clear();
			var zeroPosition = new Position(0, 0);
			redUAVXY = zeroPosition;
			blueUAVXY = zeroPosition;
			redTankXY = zeroPosition;
			blueTankXY = zeroPosition;
			lastKnownRedTankXY = zeroPosition;
			lastKnownBlueTankXY = zeroPosition;

			redMissilesRemaining = 0;
			blueMissilesRemaining = 0;
			turnsElapsed = 0;
			winPercent = 0;
			warnings = "";

			EventArgs args = new EventArgs();
			OnPropertyChanged(args);
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

		private Position lastKnownBlueTankXY;
		public Position LastKnownBlueTankXY
		{
			get { return lastKnownBlueTankXY; }
			set { lastKnownBlueTankXY = value; OnPropertyChanged(EventArgs.Empty); }
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
