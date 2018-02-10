using System;
using Eto.Forms;
using Eto.Drawing;
using NLog;
using NGAPI;

namespace NGSim
{
	public class MainWindow : Form
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		private TextBox AlgorithmTextBox1;
		private TextBox AlgorithmTextBox2;
		private Button AlgorithmBrowseButton1;
		private Button AlgorithmBrowseButton2;
		// private Button AlgorithmLoadButton;
		// private Button Run1GameInDepthButton;
		// private Button RunGamesContinuallyButton;
		// private Button Run500GamesContinuallyButton;
		private CheckBox SingleGameCheckBox;
		private CheckBox NGameCheckBox;
		private TextBox NGameTextBox;
		private Button LaunchButton;
		private TextArea StateInfoTextArea;
		private TextArea NetoworkInfoTextArea;
		private OpenFileDialog AlgorithmOpenFile1;
		private OpenFileDialog AlgorithmOpenFile2;

		public MainWindow()
		{
			logger.Info("Sample Log Message");
			Title = "SimManager";

			var winLayout = new StackLayout();
			winLayout.Orientation = Orientation.Vertical;
			winLayout.VerticalContentAlignment = VerticalAlignment.Top;

			var algoGroup = prepareAlgorithmControlGroup();
			winLayout.Items.Add(new StackLayoutItem(algoGroup));

			var simGroup = prepareSimulationControlGroup();
			winLayout.Items.Add(new StackLayoutItem(simGroup));

			Content = winLayout;
		}

		private Control prepareAlgorithmControlGroup()
		{
			var algoGroup = new GroupBox { Text = "Algorithm Selection" };

			AlgorithmTextBox1 = new TextBox { Text = "" };
			AlgorithmTextBox2 = new TextBox { Text = "" };

			AlgorithmBrowseButton1 = new Button { Text = "Browse" };
			AlgorithmBrowseButton2 = new Button { Text = "Browse" };

			AlgorithmBrowseButton1.Click += AlgorithmBrowseButton1_Click;
			AlgorithmBrowseButton2.Click += AlgorithmBrowseButton2_Click;

			AlgorithmLoadButton = new Button { Text = "Load Algorithms" };
			AlgorithmLoadButton.Click += AlgorithmLoadButton_Click;

			AlgorithmOpenFile1 = new OpenFileDialog();
			AlgorithmOpenFile2 = new OpenFileDialog();

			var algoLayout = new TableLayout
			{
				Spacing = new Size(5, 5),
				Padding = new Padding(10),
				Width = 800,
				Rows =
				{
					new TableRow(
						new Label { Text = "Algorithm 1 Path" }
					),
					new TableRow(
						new TableCell(AlgorithmTextBox1, true), AlgorithmBrowseButton1	
					),
					new TableRow(
						new Label { Text = "Algorithm 2 Path" }
					),
					new TableRow(
						new TableCell(AlgorithmTextBox2, true), AlgorithmBrowseButton2	
					),
					new TableRow(
						new Label { }	
					),
					new TableRow(
						new Label { Text = "\t\tNote: Leave path blank to load dummy algorithm" }, AlgorithmLoadButton
					)
				}
			};

			algoGroup.Content = algoLayout;
			return algoGroup;
		}

		private Control prepareSimulationControlGroup()
		{
			var group = new GroupBox { Text = "Simulation Control" };

			Run1GameInDepthButton = new Button { Text = "Single Game", Height = 30, Enabled = false }; //Run 1 Game In Depth With Positions Shown
			RunGamesContinuallyButton = new Button { Text = "Continuous Games", Height = 30, Enabled = false }; //Continually run games and print who's the winner
			Run500GamesContinuallyButton = new Button { Text = "500 Continuous Games", Height = 30, Enabled = false }; //Continually run 500 games and print the winning percentages

			Run1GameInDepthButton.Click += Run1GameInDepthButton_Click;
			RunGamesContinuallyButton.Click += RunGamesContinuallyButton_Click;
			Run500GamesContinuallyButton.Click += Run500GamesContinuallyButton_Click;

			var layout = new TableLayout
			{
				Spacing = new Size(5, 5),
				Padding = new Padding(10),
				Width = 800,
				Rows =
				{
					new TableRow(
						new TableCell(Run1GameInDepthButton, true), new TableCell(RunGamesContinuallyButton, true), new TableCell(Run500GamesContinuallyButton, true)
					)
				}
			};

			group.Content = layout;
			return group;
		}

		private void AlgorithmLoadButton_Click(object sender, EventArgs e)
		{
			UpdateManager.SimManager.running = false;

			// Try to load the new algo 1
			try
			{
				Algorithm a = String.IsNullOrWhiteSpace(AlgorithmTextBox1.Text) ?
							  new StupidAlgorithm1() :
							  AlgorithmLoader.LoadAlgorithm(AlgorithmTextBox1.Text);

				UpdateManager.SimManager.Algo1 = a;
				Console.WriteLine("Loaded algorithm 1");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Unable to load Algorithm 1", MessageBoxType.Error);
				Run1GameInDepthButton.Enabled = false;
				return;
			}

			// Try to load the new algo 2
			try
			{
				Algorithm a = String.IsNullOrWhiteSpace(AlgorithmTextBox2.Text) ?
							  new StupidAlgorithm1() :
							  AlgorithmLoader.LoadAlgorithm(AlgorithmTextBox2.Text);

				UpdateManager.SimManager.Algo2 = a;
				Console.WriteLine("Loaded algorithm 2");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Unable to load Algorithm 2", MessageBoxType.Error);
				Run1GameInDepthButton.Enabled = false;
				return;
			}

			// Enable buttons
			Run1GameInDepthButton.Enabled = true;

			UpdateManager.SimManager.SetGameRunningMode(UpdateManager.SimManager.gameRunningMode);
			//UpdateManager.SimManager.running = true;
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