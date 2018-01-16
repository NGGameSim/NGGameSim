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

			var GoButton = new Button { Text = "GO" };

			GoButton.Click += GoButton_Click;

			var GoRow = new TableRow(
				new TableCell(GoButton, true)
			);

			layout.Rows.Add(GoRow);

			this.Content = layout;
		}

		private void GoButton_Click(object sender, EventArgs e)
		{
			//var OutString = "You entered: " + AlgorithmTextBox1.Text + " and " + AlgorithmTextBox2.Text;
			//MessageBox.Show(Application.Instance.MainForm, OutString, "GO Button", MessageBoxButtons.OK);

			UpdateManager.SimManager.running = true;
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