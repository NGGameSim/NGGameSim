using System;
using Eto.Forms;
using Eto.Drawing;
using NLog;
using NGSim.Graphics;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using XnaMatrix = Microsoft.Xna.Framework.Matrix;

namespace NGSim
{
	public class SimViewerStartupWindow : Form
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		// Controls
		private TextBox IDTextBox;
		private Button JoinSimButton;

		// Main Window contains 
		public SimViewerStartupWindow()
		{
			logger.Info("Sample Log Message");
			Title = "SimViewer";

			var winLayout = new StackLayout();
			winLayout.Orientation = Orientation.Horizontal;

			var controlGroup = prepareControlGroup();

			winLayout.Items.Add(new StackLayoutItem(controlGroup));

			Content = winLayout;
		}

		// Left Group
		private Control prepareControlGroup()
		{
			var group = new GroupBox();

			IDTextBox = new TextBox { Text = "ENTER YOUR ID" };
			JoinSimButton = new Button { Text = "JOIN" };

			JoinSimButton.Click += JoinSimButton_Click;

			// Add the controls to the layout
			var layout = new TableRow(IDTextBox, JoinSimButton);

			// Add the layout to the returned group
			group.Content = layout;
			return group;
		}

		private void JoinSimButton_Click(object sender, EventArgs e)
		{
			this.Close();
			SimViewerWindow mainWindow = new SimViewerWindow();
			mainWindow.Show();
		}
	}

}