using System;
using Eto.Forms;
using Eto.Drawing;
using NLog;
using NGAPI;
using NGSim.Graphics;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using XnaMatrix = Microsoft.Xna.Framework.Matrix;

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

		private Button IncreaseZoomButton_;
		private TableCell IncreaseZoomButton;
		private Button DecreaseZoomButton_;
		private TableCell DecreaseZoomButton;
		private Slider ZoomSlider_;
		private TableCell ZoomSlider;
		private TextBox ZoomTextBox_;
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

			// Create Buttons to add to cells
			var BlueUAVButton_ = new Button { Text = "BLUE UAV" };
			BlueUAVButton_.Click += BlueUAVButton_Click;
			var BlueTankButton_ = new Button { Text = "BLUE Tank" };
			BlueTankButton_.Click += BlueTankButton_Click;
			var RedUAVButton_ = new Button { Text = "Red UAV" };
			RedUAVButton_.Click += RedUAVButton_Click;
			var RedTankButton_ = new Button { Text = "Red Tank" };
			RedTankButton_.Click += RedTankButton_Click;
			var FreeCameraButton_ = new Button { Text = "FREE CAMERA" };
			FreeCameraButton_.Click += FreeCameraButton_Click;
			var ResetButton_ = new Button { Text = "RESET" };
			ResetButton_.Click += ResetButton_Click;

			// Create the top row controls (view modes)
			BlueUAVButton = new TableCell(BlueUAVButton_, true);
			BlueTankButton = new TableCell(BlueTankButton_, true);
			RedUAVButton = new TableCell(RedUAVButton_, true);
			RedTankButton = new TableCell(RedTankButton_, true);
			FreeCameraButton = new TableCell(FreeCameraButton_, true);
			ResetButton = new TableCell(ResetButton_, true);

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

			Assembly _assembly = Assembly.GetExecutingAssembly();
			Stream RightArrowStream = _assembly.GetManifestResourceStream("NGSim.Graphics.RightArrow.ico");
			Stream LeftArrowStream = _assembly.GetManifestResourceStream("NGSim.Graphics.LeftArrow.ico");
			Stream DownArrowStream = _assembly.GetManifestResourceStream("NGSim.Graphics.DownArrow.ico");
			Stream UpArrowStream = _assembly.GetManifestResourceStream("NGSim.Graphics.UpArrow.ico");

			var RightButtonImage = new Icon(RightArrowStream);
			var LeftButtonImage = new Icon(LeftArrowStream);
			var DownButtonImage = new Icon(DownArrowStream);
			var UpButtonImage = new Icon(UpArrowStream);

			TranslateRightButton = new Button() { Size = ButtonSize, Image = RightButtonImage };
			TranslateLeftButton = new Button() { Size = ButtonSize, Image = LeftButtonImage };
			TranslateUpButton = new Button() { Size = ButtonSize, Image = UpButtonImage };
			TranslateDownButton = new Button() { Size = ButtonSize, Image = DownButtonImage };

			TranslateRightButton.Click += TranslateRightButton_Click;
			TranslateLeftButton.Click += TranslateLeftButton_Click;
			TranslateUpButton.Click += TranslateUpButton_Click;
			TranslateDownButton.Click += TranslateDownButton_Click;

			RotateRightButton = new Button() { Size = ButtonSize, Image = RightButtonImage };
			RotateLeftButton = new Button() { Size = ButtonSize, Image = LeftButtonImage };
			RotateUpButton = new Button() { Size = ButtonSize, Image = UpButtonImage };
			RotateDownButton = new Button() { Size = ButtonSize, Image = DownButtonImage };

			RotateRightButton.Click += RotateRightButton_Click;
			RotateLeftButton.Click += RotateLeftButton_Click;
			RotateUpButton.Click += RotateUpButton_Click;
			RotateDownButton.Click += RotateDownButton_Click;


			// Layout for translate controls
			var TranslateTM = new TableCell(TranslateUpButton) { ScaleWidth = false };
			var TranslateML = new TableCell(TranslateLeftButton) { ScaleWidth = false };
			var TranslateMR = new TableCell(TranslateRightButton) { ScaleWidth = false };
			var TranslateBM = new TableCell(TranslateDownButton) { ScaleWidth = false };

			var TranslateNull = new TableCell(null) { ScaleWidth = false };

			var TranslateLabel = new TableCell(new Label { Text = "TRANSLATION", TextAlignment = TextAlignment.Center }) { ScaleWidth = false };

			var translateButtons = new TableLayout
			{
				Spacing = new Size(5, 5),
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
				Spacing = new Size(5, 5),
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

			IncreaseZoomButton_ = new Button { Text = "+", Size = zoomButtonSize };
			IncreaseZoomButton_.Click += IncreaseZoomButton__Click;
			IncreaseZoomButton = new TableCell(IncreaseZoomButton_, false);

			DecreaseZoomButton_ = new Button { Text = "-", Size = zoomButtonSize };
			DecreaseZoomButton_.Click += DecreaseZoomButton__Click;
			DecreaseZoomButton = new TableCell(DecreaseZoomButton_, false);

			ZoomSlider_ = new Slider();
			ZoomSlider_.Orientation = Orientation.Vertical;
			ZoomSlider_.Height = 100;
			ZoomSlider_.Value = 100;
			ZoomSlider_.ValueChanged += ZoomSliderValueChanged;
			ZoomSlider = new TableCell(ZoomSlider_, false);

			ZoomTextBox_ = new TextBox { Text = ZoomSlider_.Value.ToString(), Size = zoomButtonSize };
			ZoomTextBox_.TextChanged += ZoomTextBoxValueChanged;
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

		private void RotateDownButton_Click(object sender, EventArgs e)
		{
			// Pitch Down
			ArcBallCamera cam = CameraManager.ActiveCamera as ArcBallCamera;
			cam.Pitch -= 20f;
		}

		private void RotateUpButton_Click(object sender, EventArgs e)
		{
			// Pitch Up
			ArcBallCamera cam = CameraManager.ActiveCamera as ArcBallCamera;
			cam.Pitch += 20f;
		}

		private void RotateLeftButton_Click(object sender, EventArgs e)
		{
			// Yaw -
			ArcBallCamera cam = CameraManager.ActiveCamera as ArcBallCamera;
			cam.Yaw -= 20f;
		}

		private void RotateRightButton_Click(object sender, EventArgs e)
		{
			// Yaw +
			ArcBallCamera cam = CameraManager.ActiveCamera as ArcBallCamera;
			cam.Yaw += 20f;
		}

		private void TranslateDownButton_Click(object sender, EventArgs e)
		{
			ArcBallCamera cam = CameraManager.ActiveCamera as ArcBallCamera;
			Vector3 dfvect = new Vector3(0, -1, 0);
			cam.Target += dfvect;
		}

		private void TranslateUpButton_Click(object sender, EventArgs e)
		{
			ArcBallCamera cam = CameraManager.ActiveCamera as ArcBallCamera;
			Vector3 dfvect = new Vector3(0, 1, 0);
			cam.Target += dfvect;
		}

		private void TranslateLeftButton_Click(object sender, EventArgs e)
		{
			ArcBallCamera cam = CameraManager.ActiveCamera as ArcBallCamera;

			XnaMatrix rotationmatrix = XnaMatrix.CreateRotationY(MathHelper.ToRadians(cam.Yaw));

			Vector3 dfvect = new Vector3(-1, 0, 0); //Shift the camera Left
			Vector3 dfTransVect = Vector3.Transform(dfvect, rotationmatrix);
			
			cam.Target += dfTransVect;
		}

		private void TranslateRightButton_Click(object sender, EventArgs e)
		{
			ArcBallCamera cam = CameraManager.ActiveCamera as ArcBallCamera;

			XnaMatrix rotationmatrix = XnaMatrix.CreateRotationY(MathHelper.ToRadians(cam.Yaw));

			Vector3 dfvect = new Vector3(1, 0, 0); //Shift the camera Right
			Vector3 dfTransVect = Vector3.Transform(dfvect, rotationmatrix);

			cam.Target += dfTransVect;
		}

		private void ResetButton_Click(object sender, EventArgs e)
		{
			//Zoom out and reset
			ArcBallCamera cam = CameraManager.ActiveCamera as ArcBallCamera;
			ArcBallCameraBehavior beh = new ArcBallCameraBehavior();
			cam.Pitch = 45f;
			cam.Yaw = 0f;
			cam.Distance = 200f;
			cam.Target = new Vector3(0, 0, 0);
			CameraManager.ActiveBehavior = new ArcBallCameraBehavior();
		}

		private void FreeCameraButton_Click(object sender, EventArgs e)
		{
			// Enable translate buttons
			// Set target to current posititon (stop following entity)
			//Zoom out and reset
			ArcBallCamera cam = CameraManager.ActiveCamera as ArcBallCamera;
			ArcBallCameraBehavior beh = new ArcBallCameraBehavior();
			cam.Pitch = 45f;
			cam.Yaw = 0f;
			cam.Distance = 200f;
			cam.Target = new Vector3(0, 0, 0);
			CameraManager.ActiveBehavior = new ArcBallCameraBehavior();

		}

		private void RedTankButton_Click(object sender, EventArgs e)
		{
			// Disable translate buttons
			// Set target to red tank
			EntityFollowBehavior entityBeh = new EntityFollowBehavior();
			entityBeh.Choice = "Team1.Tank";
			CameraManager.ActiveBehavior = entityBeh;
		}

		private void RedUAVButton_Click(object sender, EventArgs e)
		{
			// Disable translate buttons
			// Set target to red uav
			EntityFollowBehavior entityBeh = new EntityFollowBehavior();
			entityBeh.Choice = "Team1.UAV";
			CameraManager.ActiveBehavior = entityBeh;
		}

		private void BlueTankButton_Click(object sender, EventArgs e)
		{
			// Disable translate buttons
			// Set target to blue tank
			EntityFollowBehavior entityBeh = new EntityFollowBehavior();
			entityBeh.Choice = "Team2.Tank";
			CameraManager.ActiveBehavior = entityBeh;

		}

		private void BlueUAVButton_Click(object sender, EventArgs e)
		{
			// Disable translate buttons
			// Set target to blue uav
			EntityFollowBehavior entityBeh = new EntityFollowBehavior();
			entityBeh.Choice = "Team2.UAV";
			CameraManager.ActiveBehavior = entityBeh;

		}

		private void DecreaseZoomButton__Click(object sender, EventArgs e)
		{
			if (ZoomSlider_.Value > 0)
			{
				ZoomSlider_.Value -= 1;
			}
			// Update camera distance
			//Normalize distance for 0 - 100, then adjust distance accordingly
		}

		private void IncreaseZoomButton__Click(object sender, EventArgs e)
		{
			if (ZoomSlider_.Value < 100)
			{
				ZoomSlider_.Value += 1;
			}
			// Update camera distance
			//Normalize distance for 0 - 100
		}

		private void ZoomSliderValueChanged(object sender, EventArgs e)
		{
			ZoomTextBox_.Text = ZoomSlider_.Value.ToString();
			// Update camera distance
		}

		private void ZoomTextBoxValueChanged(object sender, EventArgs e)
		{
			var parseResult = 0;
			if (Int32.TryParse(ZoomTextBox_.Text, out parseResult))
			{
				ZoomSlider_.Value = parseResult;
			}
			// Update camera distance
		}

		// Right Group
		private Control prepareRightGroup()
		{
			var group = new GroupBox();

			// Create the controls
			MyStateInfoTextArea = new StateInfoTextArea();
			MyStateInfoTextArea.Height = 286;
			MyStateInfoTextArea.Width = 200;

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