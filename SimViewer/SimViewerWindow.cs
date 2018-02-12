using System;
using Eto.Forms;
using Eto.Drawing;
using NLog;
using NGAPI;

namespace NGSim
{
	public class SimViewerWindow : Form
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		// Controls
		private TableCell BlueUAVButton;
		private TableCell RedUAVButton;
		private TableCell BlueTankButton;
		private TableCell RedTankButton;
		private TableCell FreeCameraButton;
		private TableCell ResetButton;

		private Button TranslateRightButton;
		private Button TranslateLeftButton;
		private Button TranslateUpButton;
		private Button TranslateDownButton;

		private Button RotateRightButton;
		private Button RotateLeftButton;
		private Button RotateUpButton;
		private Button RotateDownButton;

		private TableCell IncreaseZoomButton;
		private TableCell DecreaseZoomButton;
		private TableCell ZoomSlider;
		private TableCell ZoomTextBox;

		private TextArea StateInfoTextArea;

		// Main Window contains 
		public SimViewerWindow()
		{
			logger.Info("Sample Log Message");
			Title = "SimViewer";

			var winLayout = new StackLayout();
			winLayout.Orientation = Orientation.Horizontal;
			//winLayout.VerticalContentAlignment = VerticalAlignment.Top;

			var leftGroup = prepareLeftGroup();
			winLayout.Items.Add(new StackLayoutItem(leftGroup));

			var rightGroup = prepareRightGroup();
			winLayout.Items.Add(new StackLayoutItem(rightGroup));

			Content = winLayout;
		}

		// Left Group
		private Control prepareLeftGroup()
		{
			var group = new GroupBox();

			// Create the controls
			BlueUAVButton = new TableCell(new Button { Text = "BLUE UAV" }, true);
			BlueTankButton = new TableCell(new Button { Text = "BLUE TANK" }, true);
			RedUAVButton = new TableCell(new Button { Text = "RED UAV" }, true);
			RedTankButton = new TableCell(new Button { Text = "RED TANK" }, true);
			FreeCameraButton = new TableCell(new Button { Text = "FREE CAMERA" }, true);
			ResetButton = new TableCell(new Button { Text = "RESET" }, true);

			TranslateRightButton = new Button();
			TranslateLeftButton = new Button();
			TranslateUpButton = new Button();
			TranslateDownButton = new Button();

			RotateRightButton = new Button();
			RotateLeftButton = new Button();
			RotateUpButton = new Button();
			RotateDownButton = new Button();

			IncreaseZoomButton = new TableCell(new Button { Text = "+" }, true);
			DecreaseZoomButton = new TableCell(new Button { Text = "-" }, true);
			var ZoomSlider_ = new Slider();
			ZoomSlider_.Orientation = Orientation.Vertical;
			ZoomSlider_.Height = 150;
			ZoomSlider_.Value = 50;
			ZoomSlider = new TableCell(ZoomSlider_, true);
			var ZoomTextBox_ = new TextBox { Text = ZoomSlider_.Value.ToString() };
			ZoomTextBox = new TableCell(ZoomTextBox_, true);

			// Layout for view modes
			var entityControls = new TableLayout
			{
				Spacing = new Size(5, 5),
				Padding = new Padding(10, 10, 10, 10),
				Rows =
				{
					new TableRow(BlueUAVButton, RedUAVButton),
					new TableRow(BlueTankButton, RedTankButton)
				}
			};

			var freeCameraControls = new TableLayout
			{
				Spacing = new Size(5, 5),
				Padding = new Padding(10, 10, 10, 10),
				Rows =
				{
					new TableRow(FreeCameraButton)
				}
			};

			var resetControls = new TableLayout
			{
				Spacing = new Size(5, 5),
				Padding = new Padding(10, 10, 10, 10),
				Rows =
				{
					new TableRow(ResetButton)
				}
			};

			// Layout for translate controls
			var translateControls = new TableLayout
			{
				Spacing = new Size(5, 5),
				Padding = new Padding(10, 10, 10, 10),
				Rows =
				{
					new TableRow(null, TranslateUpButton, null) { ScaleHeight = true },
					new TableRow(TranslateLeftButton, null, TranslateRightButton) { ScaleHeight = true },
					new TableRow(null, TranslateDownButton, null) { ScaleHeight = true },
					new TableRow(null, new Label { Text = "TRANSLATION" }, null) { ScaleHeight = false }
				}
			};

			// Layout for rotate controls
			var rotateControls = new TableLayout
			{
				Spacing = new Size(5, 5),
				Padding = new Padding(10, 10, 10, 10),
				Rows =
				{
					new TableRow(null, RotateUpButton, null) { ScaleHeight = true },
					new TableRow(RotateLeftButton, null, RotateRightButton) { ScaleHeight = true },
					new TableRow(null, RotateDownButton, null) { ScaleHeight = true },
					new TableRow(null, new Label { Text = "ROTATION" }, null) { ScaleHeight = false } 
				}
			};

			// Layout for zoom controls
			var zoomControls = new TableLayout
			{
				Spacing = new Size(5, 5),
				Padding = new Padding(10, 10, 10, 10),
				Width = 10,
				Rows =
				{
					new TableRow(IncreaseZoomButton) { ScaleHeight = true },
					new TableRow(ZoomSlider) { ScaleHeight = true },
					new TableRow(DecreaseZoomButton) { ScaleHeight = true },
					new TableRow(ZoomTextBox) { ScaleHeight = true }
				}
			};

			var topRow = new TableRow(entityControls, freeCameraControls, resetControls);
			var bottomRow = new TableRow(translateControls, rotateControls, zoomControls);

			// Add the controls to the layout
			var layout = new TableLayout
			{
				Spacing = new Size(5, 5),
				Padding = new Padding(20),
				Rows =
				{
					topRow,
					bottomRow,
				}
			};

			// Add the layout to the returned group
			group.Content = layout;
			return group;
		}

		// Right Group
		private Control prepareRightGroup()
		{
			var group = new GroupBox();

			// Create the controls

			// Add controls to layout
			var layout = new TableLayout
			{
				Spacing = new Size(5, 5),
				Padding = new Padding(10),
				Width = 800,
				Rows =
				{
					new TableRow(
						new Label { Text = "Right Group" }
					)
				}
			};

			// Add layout to group and return 
			group.Content = layout;
			return group;
		}

	}

}