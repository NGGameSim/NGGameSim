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
		private Button AlgorithmBrowseButton1;
		private Button AlgorithmBrowseButton2;
		private OpenFileDialog AlgorithmOpenFile1;
		private OpenFileDialog AlgorithmOpenFile2;

		public MainWindow()
		{
			logger.Info("Sample Log Message");
			ClientSize = new Size(800, 100);
			Title = "SimManager";

			var layout = new TableLayout();
			layout.Spacing = new Size(5, 5);
			layout.Padding = new Padding(10, 10, 10, 10);

			AlgorithmTextBox1 = new TextBox { Text = "Algorithm 1 Path" };
			AlgorithmTextBox2 = new TextBox { Text = "Algorithm 2 Path" };

			AlgorithmBrowseButton1 = new Button { Text = "Browse" };
			AlgorithmBrowseButton2 = new Button { Text = "Browse" };

			AlgorithmBrowseButton1.Click += AlgorithmBrowseButton1_Click;
			AlgorithmBrowseButton2.Click += AlgorithmBrowseButton2_Click;

			AlgorithmOpenFile1 = new OpenFileDialog();
			AlgorithmOpenFile2 = new OpenFileDialog();

			layout.Rows.Add(new TableRow(
				new TableCell(AlgorithmTextBox1, true),
				new TableCell(AlgorithmBrowseButton1, false),
				new TableCell(),
				new TableCell(AlgorithmTextBox2, true),
				new TableCell(AlgorithmBrowseButton2, false)
			));

			var Run1GameInDepthButton = new Button { Text = "Single Game" }; //Run 1 Game In Depth With Positions Shown
			var RunGamesContinuallyButton = new Button { Text = "Continuous Games" }; //Continually run games and print who's the winner
			var Run500GamesContinuallyButton = new Button { Text = "500 Continuous Games" }; //Continually run 500 games and print the winning percentages

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

		private void AlgorithmBrowseButton1_Click(object sender, EventArgs e)
		{
			DialogResult result = AlgorithmOpenFile1.ShowDialog(AlgorithmBrowseButton1);
			AlgorithmTextBox1.Text = AlgorithmOpenFile1.FileName;
		}
		private void AlgorithmBrowseButton2_Click(object sender, EventArgs e)
		{
			DialogResult result = AlgorithmOpenFile2.ShowDialog(AlgorithmBrowseButton2);
			AlgorithmTextBox2.Text = AlgorithmOpenFile2.FileName;
		}
	}

	public class ClientWindow : Form
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		private TextBox AlgorithmTextBox1;
		private TextBox AlgorithmTextBox2;

		public ClientWindow()
		{
			logger.Info("Sample Log Message");
			ClientSize = new Size(500, 100);
			Title = "ViewManager";

			var layout = new TableLayout();
			layout.Spacing = new Size(5, 5);
			layout.Padding = new Padding(10, 10, 10, 10);

			layout.Rows.Add(new TableRow(
				new TableCell(AlgorithmTextBox1, true),
				new TableCell(AlgorithmTextBox2, true)
			));

			var Team1Button = new Button { Text = "Team 1" };
			var Team2Button = new Button { Text = "Team 2" };
			var GodButton = new Button { Text = "God Mode" };

			Team1Button.Click += Team1Button_Click;
			Team2Button.Click += Team2Button_Click;
			GodButton.Click += GodButton_Click;

			var GoRow = new TableRow(
				new TableCell(Team1Button, true),
				new TableCell(Team2Button, true),
				new TableCell(GodButton, true)
			);

			layout.Rows.Add(GoRow);

			this.Content = layout;
		}

		private void Team1Button_Click(object sender, EventArgs e)
		{
			//var OutString = "You entered: " + AlgorithmTextBox1.Text + " and " + AlgorithmTextBox2.Text;
			//MessageBox.Show(Application.Instance.MainForm, OutString, "GO Button", MessageBoxButtons.OK);

			UpdateManager.SimManager.running = true;
		}
		private void Team2Button_Click(object sender, EventArgs e)
		{
			//var OutString = "You entered: " + AlgorithmTextBox1.Text + " and " + AlgorithmTextBox2.Text;
			//MessageBox.Show(Application.Instance.MainForm, OutString, "GO Button", MessageBoxButtons.OK);

			UpdateManager.SimManager.running = true;
		}
		private void GodButton_Click(object sender, EventArgs e)
		{
			//var OutString = "You entered: " + AlgorithmTextBox1.Text + " and " + AlgorithmTextBox2.Text;
			//MessageBox.Show(Application.Instance.MainForm, OutString, "GO Button", MessageBoxButtons.OK);

			UpdateManager.SimManager.running = true;
		}

	}
}