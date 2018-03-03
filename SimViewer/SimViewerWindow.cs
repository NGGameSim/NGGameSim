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

		private StateInfoTextArea MyStateInfoTextArea;

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

			// Create the top row controls (view modes)
			BlueUAVButton = new TableCell(new Button { Text = "BLUE UAV" }, true);
			BlueTankButton = new TableCell(new Button { Text = "BLUE TANK" }, true);
			RedUAVButton = new TableCell(new Button { Text = "RED UAV" }, true);
			RedTankButton = new TableCell(new Button { Text = "RED TANK" }, true);
			FreeCameraButton = new TableCell(new Button { Text = "FREE CAMERA" }, true);
			ResetButton = new TableCell(new Button { Text = "RESET" }, true);

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

			// Add view mode controls to top row
			var topRow = new TableRow(new TableRow(entityControls, freeCameraControls, resetControls));


			// Create the bottom row controls (translate, rotate, zoom)
			var ButtonSize = new Size(50, 50);

			TranslateRightButton = new Button() { Size = ButtonSize };
			TranslateLeftButton = new Button() { Size = ButtonSize };
			TranslateUpButton = new Button() { Size = ButtonSize };
			TranslateDownButton = new Button() { Size = ButtonSize };

			RotateRightButton = new Button() { Size = ButtonSize };
			RotateLeftButton = new Button() { Size = ButtonSize };
			RotateUpButton = new Button() { Size = ButtonSize };
			RotateDownButton = new Button() { Size = ButtonSize };


			// Layout for translate controls
			var TranslateTM = new TableCell(TranslateUpButton) { ScaleWidth = false };
			var TranslateML = new TableCell(TranslateLeftButton) { ScaleWidth = false };
			var TranslateMR = new TableCell(TranslateRightButton) { ScaleWidth = false };
			var TranslateBM = new TableCell(TranslateDownButton) { ScaleWidth = false };

			var TranslateNull = new TableCell(null) { ScaleWidth = false };

			var TranslateLabel = new TableCell(new Label { Text = "TRANSLATION", TextAlignment = TextAlignment.Center }) { ScaleWidth = false };

			var translateButtons = new TableLayout
			{
				Spacing = new Size(0, 0),
				Padding = new Padding(0),
				Rows =
				{
					new TableRow(TranslateNull, TranslateTM, TranslateNull) { ScaleHeight = false },
					new TableRow(TranslateML, TranslateNull, TranslateMR) { ScaleHeight = false },
					new TableRow(TranslateNull, TranslateBM, TranslateNull) { ScaleHeight = false },
				}
			};

			var translateControls = new TableLayout
			{
				Spacing = new Size(0, 10),
				Padding = new Padding(0, 15, 10, 10),
				Rows =
				{
					new TableRow(translateButtons),
					new TableRow(TranslateLabel)
				}
			};


			// Layout for rotate controls
			var RotateTM = new TableCell(RotateUpButton) { ScaleWidth = false };
			var RotateML = new TableCell(RotateLeftButton) { ScaleWidth = false };
			var RotateMR = new TableCell(RotateRightButton) { ScaleWidth = false };
			var RotateBM = new TableCell(RotateDownButton) { ScaleWidth = false };

			var RotateNull = new TableCell(null) { ScaleWidth = false };

			var RotateLabel = new TableCell(new Label { Text = "ROTATION", TextAlignment = TextAlignment.Center }) { ScaleWidth = false };
			var rotateButtons = new TableLayout
			{
				Spacing = new Size(0,0),
				Padding = new Padding(0),
				Rows =
				{
					new TableRow(RotateNull, RotateTM, RotateNull) { ScaleHeight = false },
					new TableRow(RotateML, RotateNull, RotateMR) { ScaleHeight = false },
					new TableRow(RotateNull, RotateBM, RotateNull) { ScaleHeight = false },
				}
			};

			var rotateControls = new TableLayout
			{
				Spacing = new Size(0, 10),
				Padding = new Padding(10, 15, 10, 10),
				Rows =
				{
					new TableRow(rotateButtons),
					new TableRow(RotateLabel)
				}
			};

			// Layout for zoom controls

			var zoomButtonSize = new Size(30, 30);

			IncreaseZoomButton = new TableCell(new Button { Text = "+", Size = zoomButtonSize }, false);
			DecreaseZoomButton = new TableCell(new Button { Text = "-", Size = zoomButtonSize }, false);

			var ZoomSlider_ = new Slider();
			ZoomSlider_.Orientation = Orientation.Vertical;
			ZoomSlider_.Height = 100;
			ZoomSlider_.Value = 100;
			ZoomSlider = new TableCell(ZoomSlider_, false);

			var ZoomTextBox_ = new TextBox { Text = ZoomSlider_.Value.ToString(), Size = zoomButtonSize };
			ZoomTextBox = new TableCell(ZoomTextBox_, false);

			var zoomControls = new TableLayout
			{
				Spacing = new Size(5, 5),
				Padding = new Padding(5, 0),
				Rows =
				{
					new TableRow(IncreaseZoomButton) { ScaleHeight = true },
					new TableRow(ZoomSlider) { ScaleHeight = true },
					new TableRow(DecreaseZoomButton) { ScaleHeight = true },
					new TableRow(ZoomTextBox) { ScaleHeight = true }
				}
			};


			// Add controls to bottom row
			var bottomRow = new TableRow(translateControls, rotateControls, zoomControls) { ScaleHeight = true };


			// Add the controls to the layout
			var layout = new TableLayout
			{
				Spacing = new Size(5, 5),
				Padding = new Padding(20),
				Rows =
				{
					topRow,
					new TableRow(bottomRow),
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
			MyStateInfoTextArea = new StateInfoTextArea();

			// Add controls to layout
			var layout = new TableLayout
			{
				Spacing = new Size(5, 5),
				Padding = new Padding(10),
				//Width = 800,
				Rows =
				{
					new Label{ Text = "STATE INFORMATION" },
					new TableRow(MyStateInfoTextArea){ ScaleHeight = true }
				}
			};

			// Add layout to group and return 
			group.Content = layout;
			return group;
		}

	}

}