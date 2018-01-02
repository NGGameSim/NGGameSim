using System;
using Eto.Forms;
using Eto.Drawing;
using NLog;

namespace NGSim
{
	public class MainWindow : Form
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		private TextBox AlgorithmTextBox1;
		private TextBox AlgorithmTextBox2;

		public MainWindow()
		{
			logger.Info("Sample Log Message");
			ClientSize = new Size(500, 100);
			Title = "SimManager";

			var layout = new TableLayout();
			layout.Spacing = new Size(5, 5);
			layout.Padding = new Padding(10, 10, 10, 10);

			AlgorithmTextBox1 = new TextBox { Text = "Algorithm 1 Path" };
			AlgorithmTextBox2 = new TextBox { Text = "Algorithm 2 Path" };

			layout.Rows.Add(new TableRow(
				new TableCell(AlgorithmTextBox1, true),
				new TableCell(AlgorithmTextBox2, true)
			));

			var Run1GameInDepthButton = new Button { Text = "Run 1 Game In Depth With Positions Shown" };
			var RunGamesContinuallyButton = new Button { Text = "Continually run games and print who's the winner" };
			var Run500GamesContinuallyButton = new Button { Text = "Continually run 500 games and print the winning percentages" };

			Run1GameInDepthButton.Click += Run1GameInDepthButton_Click;
			RunGamesContinuallyButton.Click += RunGamesContinuallyButton_Click;
			Run500GamesContinuallyButton.Click += Run500GamesContinuallyButton_Click;

			var GoRow = new TableRow(
				new TableCell(Run1GameInDepthButton, true), 
				new TableCell(RunGamesContinuallyButton, true),
				new TableCell(Run500GamesContinuallyButton, true)
			);

			layout.Rows.Add(GoRow);

			this.Content = layout;
		}

		private void Run1GameInDepthButton_Click(object sender, EventArgs e)
		{
			//var OutString = "You entered: " + AlgorithmTextBox1.Text + " and " + AlgorithmTextBox2.Text;
			//MessageBox.Show(Application.Instance.MainForm, OutString, "GO Button", MessageBoxButtons.OK);

			UpdateManager.SimManager.running = true;
			UpdateManager.SimManager.SetGameRunningMode(0);
		}

		private void RunGamesContinuallyButton_Click(object sender, EventArgs e)
		{ 
			UpdateManager.SimManager.running = true;
			UpdateManager.SimManager.SetGameRunningMode(1);
		}

		private void Run500GamesContinuallyButton_Click(object sender, EventArgs e)
		{
			UpdateManager.SimManager.running = true;
			UpdateManager.SimManager.SetGameRunningMode(2);
		}
	}
}