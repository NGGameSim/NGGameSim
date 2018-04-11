﻿using System;
using Eto.Forms;
using Eto.Drawing;
using Eto;
using NLog;
using NGAPI;

namespace NGSim
{
	public class SimManagerWindow : Form
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		// Controls
		private TextBox AlgorithmTextBox1;
		private TextBox AlgorithmTextBox2;
		private Button AlgorithmBrowseButton1;
		private Button AlgorithmBrowseButton2;
		private CheckBox SingleGameCheckBox;
		private CheckBox NGameCheckBox;
		private TextBox NGameTextBox;
		private Button LaunchButton;
		public static StateInfoTextArea MyStateInfoTextArea;
		public static NetworkInfoTextArea MyNetworkInfoTextArea;
		private Button PlayPauseButton;
		private Button GodModeButton;
		private OpenFileDialog AlgorithmOpenFile1;
		private OpenFileDialog AlgorithmOpenFile2;

		// Main Window contains a layout with the algorithm controls and simulation controls added to it
		public SimManagerWindow()
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

		// Algorithm Controls
		// Includes text boxes for algorithms, browse buttons, checkboxes for simulation type, and a launch button
		private Control prepareAlgorithmControlGroup()
		{
			var algoGroup = new GroupBox { Text = "Algorithm Selection" };

			// Create the controls
			AlgorithmTextBox1 = new TextBox { Text = "" };
			AlgorithmTextBox2 = new TextBox { Text = "" };

			AlgorithmBrowseButton1 = new Button { Text = "Browse" };
			AlgorithmBrowseButton2 = new Button { Text = "Browse" };

			AlgorithmBrowseButton1.Click += AlgorithmBrowseButton1_Click;
			AlgorithmBrowseButton2.Click += AlgorithmBrowseButton2_Click;

			SingleGameCheckBox = new CheckBox { Text = "Single Game", Checked = true };
			NGameCheckBox = new CheckBox { Text = "N Games" };
			NGameTextBox = new TextBox { Text = "500" };

			SingleGameCheckBox.CheckedChanged += SingleGameCheckBox_CheckChanged;
			NGameCheckBox.CheckedChanged += NGameCheckBox_CheckChanged;

			LaunchButton = new Button { Text = "LAUNCH" };
			LaunchButton.Click += LaunchButton_Click;

			AlgorithmOpenFile1 = new OpenFileDialog();
			AlgorithmOpenFile2 = new OpenFileDialog();

			var GameLaunchRow = new TableRow(
				new TableLayout
				{
					Spacing = new Size(5,5),
					Padding = new Padding(5),
					Rows =
					{
						new TableRow(
							SingleGameCheckBox, new Label { }
						),
						new TableRow(
							NGameCheckBox, new Label { }, NGameTextBox
						)
					}
				}, new TableCell(LaunchButton, true)
			);

			var AlgoLoaderRows = new TableLayout
			{
				Spacing = new Size(5, 5),
				Padding = new Padding(5),
				Rows =
				{
					new TableRow(
						new Label { Text = "Red Team Algorithm", TextAlignment = TextAlignment.Center }, new TableCell(AlgorithmTextBox1, true), AlgorithmBrowseButton1
					),
					new TableRow(
						new Label { Text = "Blue Team Algorithm", TextAlignment = TextAlignment.Center }, new TableCell(AlgorithmTextBox2, true), AlgorithmBrowseButton2
					)
				}
			};

			// Add the controls to the layout
			var algoLayout = new TableLayout
			{
				Spacing = new Size(10, 5),
				Padding = new Padding(20),
				Width = 800,
				Rows =
				{
					new TableRow(AlgoLoaderRows),
					new TableRow( new Label { } ),
					new TableRow(GameLaunchRow)
					
				}
			};

			// Add the layout to the returned group
			algoGroup.Content = algoLayout;
			return algoGroup;
		}

		// Simulation Controls
		// Includes 2 text areas for logging
		// Will include controls for the simulation once those have been decided on
		private Control prepareSimulationControlGroup()
		{
			var group = new GroupBox { Text = "Simulation Control" };

			MyStateInfoTextArea = new StateInfoTextArea();
			MyNetworkInfoTextArea = new NetworkInfoTextArea();

			PlayPauseButton = new Button { Text = "PLAY/PAUSE", Height = 30 };
			GodModeButton = new Button { Text = "GODMODE", Height = 30 };

			PlayPauseButton.Click += PlayPause_Click;

			var rightLayout = new TableLayout
			{
				Spacing = new Size(5, 5),
				Padding = new Padding(0),
				Width = 400,
				Rows =
				{
					new TableRow(new TableCell(MyNetworkInfoTextArea, false)){ ScaleHeight = true },
					new TableRow(new TableRow(new TableCell(PlayPauseButton, true), new TableCell(GodModeButton, true)){ ScaleHeight = false }) 
				}
			};


			var layout = new TableLayout
			{
				Spacing = new Size(5, 5),
				Padding = new Padding(10),
				Width = 800,
				Rows =
				{
					new TableRow(
						new TableCell(MyStateInfoTextArea, true), rightLayout
					)
				}
			};

			group.Content = layout;
			return group;
		}

		// Controls the N-Game Checkbox
		private void NGameCheckBox_CheckChanged(object sender, EventArgs e)
		{
			if(NGameCheckBox.Checked == true)
			{
				SingleGameCheckBox.Checked = false;
			}
			if(NGameCheckBox.Checked == false && SingleGameCheckBox.Checked == false)
			{
				NGameCheckBox.Checked = true;
			}
		}

		// Controls the Single-Game Checkbox
		private void SingleGameCheckBox_CheckChanged(object sender, EventArgs e)
		{
			if(SingleGameCheckBox.Checked == true)
			{
				NGameCheckBox.Checked = false;
			}
			if(SingleGameCheckBox.Checked == false && NGameCheckBox.Checked == false)
			{
				SingleGameCheckBox.Checked = true;
			}
		}

		// Controls the 'LAUNCH' button
		// Attemps to load the specified algorithms, then run the specified simulation
		private void LaunchButton_Click(object sender, EventArgs e)
		{
			bool result = LoadAlgorithms();
			if(result == true)
			{
				if (SingleGameCheckBox.Checked == true)
				{
					RunSingleGame();
				}
				else
				{
					RunNGames(Int32.Parse(NGameTextBox.Text));
				}
			}
		}

		private void PlayPause_Click(object sender, EventArgs e)
		{
			UpdateManager.SimManager.PausePlay();
		}

		// Function to log information about the game state
		private void LogStateInfo(string text)
		{
			//MyStateInfoTextArea.Append(text);
		}
		// Function to log information about the network state
		private void LogNetworkInfo(string text)
		{
			//NetworkInfoTextArea.Append(text);
		}

		// Function to attempt to load the specified algorithms
		private bool LoadAlgorithms()
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
				//Run1GameInDepthButton.Enabled = false;
				return false;
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
				//Run1GameInDepthButton.Enabled = false;
				return false;
			}

			// Enable buttons
			//Run1GameInDepthButton.Enabled = true;

			UpdateManager.SimManager.SetGameRunningMode(UpdateManager.SimManager.gameRunningMode);
			return true;
			//UpdateManager.SimManager.running = true;
		}

		// Function to run a single game
		private void RunSingleGame()
		{
			//var OutString = "You entered: " + AlgorithmTextBox1.Text + " and " + AlgorithmTextBox2.Text;
			//MessageBox.Show(Application.Instance.MainForm, OutString, "GO Button", MessageBoxButtons.OK);

			UpdateManager.SimManager.running = true;
			UpdateManager.SimManager.SetGameRunningMode(0);
		}

		// Function to run games until the window is closed
		private void RunGamesContinuallyButton_Click(object sender, EventArgs e)
		{ 
			UpdateManager.SimManager.running = true;
			UpdateManager.SimManager.SetGameRunningMode(1);
		}

		// Function to run N games
		private void RunNGames(int N)
		{
			UpdateManager.SimManager.running = true;
			UpdateManager.SimManager.SetNGames(N);
			UpdateManager.SimManager.SetGameRunningMode(2);
		}

		// Functions for browsing files
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
}