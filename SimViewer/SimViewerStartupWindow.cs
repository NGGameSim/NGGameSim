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
		public static TextBox IPTextBox;
		public static TextBox IDTextBox;
		private Button JoinSimButton;

		// Events
		public event EventHandler JoinAttempt;

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

			IPTextBox = new TextBox { Text = "ENTER YOUR IP" };
			IDTextBox = new TextBox { Text = "ENTER YOUR ID" };
			JoinSimButton = new Button { Text = "JOIN" };

			JoinSimButton.Click += JoinSimButton_Click;

			// Add the controls to the layout
			var layout = new TableRow(IPTextBox, IDTextBox, JoinSimButton);

			// Add the layout to the returned group
			group.Content = layout;
			return group;
		}

		public virtual void OnJoinAttempt(EventArgs e)
		{
			if (JoinAttempt != null)
			{
				JoinAttempt(this, e);
			}
		}

		private void JoinSimButton_Click(object sender, EventArgs e)
		{
			// TODO
			// Ask the server for a game mode based on ID
			// Start the mainWindow with the correct view mode 

			// Trigger event to tell main to run game
			OnJoinAttempt(new EventArgs());
			
		}
	}

}