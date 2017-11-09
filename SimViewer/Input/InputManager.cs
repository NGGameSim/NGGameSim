using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NGSim.Input
{
	// Better input system than the vanilla MonoGame one. It supports (easier) state information, event and polling
	// input, and extra events such as double clicks and key held events
	public static class InputManager
	{
		#region Members
		public static KeyboardState PreviousKeyState { get; private set; }
		public static MouseState PreviousMouseState { get; private set; }
		public static KeyboardState CurrentKeyState { get; private set; }
		public static MouseState CurrentMouseState { get; private set; }

		private static Dictionary<Keys, double> holdTimes; // The hold time (in seconds) for each key
		private static Dictionary<MouseButton, double> lastPressTime; // The last time the button was pressed
		private static Dictionary<MouseButton, double> lastReleaseTime; // The last time the button was released
		private static Dictionary<MouseButton, double> lastClickTime; // The last time the button was clicked
		private static Dictionary<MouseButton, bool> doubleClickNext; // If next next button click is a double click

		// Tracks events to fire at the end of the update method
		private static List<KeyEventArgs> keyEventList;
		private static List<ButtonEventArgs> buttonEventList;
		private static MouseMoveEventArgs? mouseMoveEvent;
		private static MouseScrollEventArgs? mouseScrollEvent;

		#region Events
		public static event KeyEvent KeyPressed; // If a key was pressed since the last frame
		public static event KeyEvent KeyReleased; // If a key was released since the last frame
		public static event KeyEvent KeyHeld; // If a key was held during this frame
		public static event ButtonEvent ButtonPressed; // If a mouse button was pressed since the last frame
		public static event ButtonEvent ButtonReleased; // If a mouse button was released since the last frame
		public static event ButtonEvent ButtonClicked; // If a mouse button was clicked in this frame
		public static event ButtonEvent ButtonDoubleClicked; // If a mouse button was double clicked in this frame
		public static event MouseMoveEvent MouseMoved; // If the mouse was moved since the last frame
		public static event MouseScrollEvent MouseScrolled; // If the mouse wheel position was changed since the last frame
		#endregion // Events

		#region Input Settings
		// These control which buttons can fire drag events for the mouse
		public static bool LeftButtonDrag = true;
		public static bool MiddleButtonDrag = false;
		public static bool RightButtonDrag = true;
		public static bool X1ButtonDrag = false;
		public static bool X2ButtonDrag = false;

		public static float KeyHoldTime = 0.5f; // The number of seconds before keys can start to fire key held events
		public static float ClickTime = 0.25f; // The max time between button press and release to count as a click
		public static float DoubleClickTime = 0.5f; // The max time between two clicks to count as a double click
		#endregion // Input Settings

		// A quick list of all mouse buttons that are considered valid singular buttons
		private static readonly MouseButton[] validButtons = new MouseButton[5]
			{ MouseButton.Left, MouseButton.Middle, MouseButton.Right, MouseButton.X1, MouseButton.X2 };
		#endregion // Members

		#region Input State Polling
		public static bool IsKeyDown(Keys key) { return CurrentKeyState.IsKeyDown(key); }
		public static bool IsKeyUp(Keys key) { return CurrentKeyState.IsKeyUp(key); }
		public static bool IsKeyPreviouslyDown(Keys key) { return PreviousKeyState.IsKeyDown(key); }
		public static bool IsKeyPreviouslyUp(Keys key) { return PreviousKeyState.IsKeyUp(key); }
		public static bool IsKeyPressed(Keys key) { return PreviousKeyState.IsKeyUp(key) && CurrentKeyState.IsKeyDown(key); }
		public static bool IsKeyReleased(Keys key) { return PreviousKeyState.IsKeyDown(key) && CurrentKeyState.IsKeyUp(key); }
		public static bool IsKeyHeld(Keys key) { return holdTimes[key] >= KeyHoldTime; }
		public static Keys[] GetPressedKeys() { return CurrentKeyState.GetPressedKeys(); }

		public static ButtonState GetButtonState(MouseButton b) { return CurrentMouseState.GetState(b); }
		public static ButtonState GetPreviousButtonState(MouseButton b) { return PreviousMouseState.GetState(b); }
		public static bool IsButtonDown(MouseButton b) { return CurrentMouseState.GetState(b) == ButtonState.Pressed; }
		public static bool IsButtonUp(MouseButton b) { return CurrentMouseState.GetState(b) == ButtonState.Released; }
		public static bool IsButtonPreviouslyDown(MouseButton b) { return PreviousMouseState.GetState(b) == ButtonState.Released; }
		public static bool IsButtonPreviouslyUp(MouseButton b) { return PreviousMouseState.GetState(b) == ButtonState.Pressed; }
		public static bool IsButtonPressed(MouseButton b) { return IsButtonDown(b) && IsButtonPreviouslyUp(b); }
		public static bool IsButtonReleased(MouseButton b) { return IsButtonUp(b) && IsButtonPreviouslyDown(b); }
		public static MouseButton GetPressedButtons()
		{
			MouseButton buttons = MouseButton.None;
			foreach (var b in validButtons)
			{
				if (CurrentMouseState.GetState(b) == ButtonState.Pressed)
					buttons &= b;
			}
			return buttons;
		}
		public static IEnumerable<MouseButton> EnumeratePressedButtons()
		{
			foreach (var b in validButtons)
			{
				if (CurrentMouseState.GetState(b) == ButtonState.Pressed)
					yield return b;
			}
		}

		public static Point GetMousePos() { return CurrentMouseState.Position; }
		public static Point GetPreviousMousePos() { return PreviousMouseState.Position; }
		public static Point GetMouseDelta() { return CurrentMouseState.Position - PreviousMouseState.Position; }
		public static void SetMousePos(Point p) { Mouse.SetPosition(p.X, p.Y); }
		public static void SetMousePos(int x, int y) { Mouse.SetPosition(x, y); }
		public static bool IsMouseMoving() { return CurrentMouseState.Position != PreviousMouseState.Position; }
		public static bool IsMouseDragging() { return GetDraggingButtons() != MouseButton.None; }
		public static bool CanButtonDrag(MouseButton b)
		{
			switch (b)
			{
				case MouseButton.Left: return LeftButtonDrag;
				case MouseButton.Middle: return MiddleButtonDrag;
				case MouseButton.Right: return RightButtonDrag;
				case MouseButton.X1: return X1ButtonDrag;
				case MouseButton.X2: return X2ButtonDrag;
				default:
					throw new InvalidOperationException("Cannot check the allowed drag state for mouse button 'None'.");
			}
		}
		public static MouseButton GetDraggingButtons()
		{
			if (CurrentMouseState.Position == PreviousMouseState.Position)
				return MouseButton.None;

			MouseButton buttons = MouseButton.None;
			foreach (var b in EnumeratePressedButtons())
			{
				if (CanButtonDrag(b))
					buttons &= b;
			}
			return buttons;
		}

		public static int GetScrollValue() { return CurrentMouseState.ScrollWheelValue; }
		public static int GetPreviousScrollValue() { return PreviousMouseState.ScrollWheelValue; }
		public static int GetScrollDelta() { return CurrentMouseState.ScrollWheelValue - PreviousMouseState.ScrollWheelValue; }
		#endregion // Input State Polling

		public static void Update(GameTime gameTime)
		{
			// Update the states
			PreviousKeyState = CurrentKeyState;
			PreviousMouseState = CurrentMouseState;
			CurrentKeyState = Keyboard.GetState();
			CurrentMouseState = Mouse.GetState();

			// Extract time info
			double totalSeconds = gameTime.TotalGameTime.TotalSeconds;
			double totalMilliseconds = gameTime.TotalGameTime.TotalMilliseconds;
			double elapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds;
			double elapsedMilliseconds = gameTime.ElapsedGameTime.TotalMilliseconds;

			// Check for one-off mouse events
			MouseButton pressedButtons = GetPressedButtons();
			MouseButton draggingButtons = GetDraggingButtons();
			if (CurrentMouseState.Position != PreviousMouseState.Position)
			{
				mouseMoveEvent = new MouseMoveEventArgs()
				{
					Buttons = pressedButtons,
					LastPoint = PreviousMouseState.Position,
					CurrPoint = CurrentMouseState.Position,
					IsDrag = draggingButtons != MouseButton.None
				};
			}
			if (CurrentMouseState.ScrollWheelValue != PreviousMouseState.ScrollWheelValue)
			{
				mouseScrollEvent = new MouseScrollEventArgs()
				{
					LastVal = PreviousMouseState.ScrollWheelValue,
					CurrVal = CurrentMouseState.ScrollWheelValue
				};
			}

			// Detect key events
			foreach (Keys key in Enum.GetValues(typeof(Keys)))
			{
				if (IsKeyDown(key))
				{
					double htime = holdTimes[key] += elapsedSeconds;
					if (htime > KeyHoldTime)
						keyEventList.Add(new KeyEventArgs() { Type = KeyEventType.Held, Key = key });
					if (IsKeyPreviouslyUp(key))
						keyEventList.Add(new KeyEventArgs() { Type = KeyEventType.Pressed, Key = key });
				}
				else if (IsKeyPreviouslyDown(key))
				{
					keyEventList.Add(new KeyEventArgs() { Type = KeyEventType.Released, Key = key });
					holdTimes[key] = 0.0;
				}
			}

			// Detect mouse events
			foreach (MouseButton button in Enum.GetValues(typeof(MouseButton)))
			{
				if (button == MouseButton.None || button == MouseButton.Any)
					continue;

				if (IsButtonDown(button))
				{
					if (IsButtonPreviouslyUp(button))
					{
						buttonEventList.Add(new ButtonEventArgs() { Type = ButtonEventType.Pressed, Button = button });
						lastPressTime[button] = totalSeconds;
					}
				}
				else if (IsButtonPreviouslyDown(button))
				{
					buttonEventList.Add(new ButtonEventArgs() { Type = ButtonEventType.Released, Button = button });
					lastReleaseTime[button] = totalSeconds;

					if (doubleClickNext[button] && canDoubleClick(button))
					{
						buttonEventList.Add(new ButtonEventArgs() { Type = ButtonEventType.DoubleClicked, Button = button });
						lastClickTime[button] = totalSeconds;
						doubleClickNext[button] = false;
					}
					else if (canClick(button))
					{
						buttonEventList.Add(new ButtonEventArgs() { Type = ButtonEventType.Clicked, Button = button });
						lastClickTime[button] = totalSeconds;
						doubleClickNext[button] = true;
					}
					else
					{
						doubleClickNext[button] = false; // The double click timed out
					}
				}
			}

			// Fire off the events
			flushEvents();
		}

		private static bool canClick(MouseButton button)
		{
			double time = lastReleaseTime[button] - lastPressTime[button];
			return (time > 0.0 && time < ClickTime);
		}

		private static bool canDoubleClick(MouseButton button)
		{
			double time = lastReleaseTime[button] - lastClickTime[button];
			return (time > 0.0 && time < DoubleClickTime);
		}

		private static void flushEvents()
		{
			foreach (var evt in keyEventList)
			{
				switch (evt.Type)
				{
					case KeyEventType.Pressed: KeyPressed(evt.Type, evt.Key); break;
					case KeyEventType.Released: KeyReleased(evt.Type, evt.Key); break;
					case KeyEventType.Held: KeyHeld(evt.Type, evt.Key); break;
				}
			}
			foreach (var evt in buttonEventList)
			{
				switch (evt.Type)
				{
					case ButtonEventType.Pressed: ButtonPressed(evt.Type, evt.Button); break;
					case ButtonEventType.Released: ButtonReleased(evt.Type, evt.Button); break;
					case ButtonEventType.Clicked: ButtonClicked(evt.Type, evt.Button); break;
					case ButtonEventType.DoubleClicked: ButtonDoubleClicked(evt.Type, evt.Button); break;
				}
			}
			if (mouseMoveEvent.HasValue)
			{
				MouseMoveEventArgs args = mouseMoveEvent.Value;
				MouseMoved(args.Buttons, args.LastPoint, args.CurrPoint, args.IsDrag);
			}
			if (mouseScrollEvent.HasValue)
			{
				MouseScrollEventArgs args = mouseScrollEvent.Value;
				MouseScrolled(args.LastVal, args.CurrVal);
			}
			keyEventList.Clear();
			buttonEventList.Clear();
			mouseMoveEvent = null;
			mouseScrollEvent = null;
		}

		public static void Initialize()
		{
			if (holdTimes != null)
				return; // Detect if we have already been initialized

			// Create the data containers
			holdTimes = new Dictionary<Keys, double>();
			lastPressTime = new Dictionary<MouseButton, double>();
			lastReleaseTime = new Dictionary<MouseButton, double>();
			lastClickTime = new Dictionary<MouseButton, double>();
			doubleClickNext = new Dictionary<MouseButton, bool>();

			// Populate the data containers
			foreach (Keys k in Enum.GetValues(typeof(Keys)))
				holdTimes.Add(k, 0.0);
			foreach (MouseButton b in Enum.GetValues(typeof(MouseButton)))
			{
				if (b == MouseButton.None || b == MouseButton.Any)
					continue;

				lastPressTime.Add(b, 0.0);
				lastReleaseTime.Add(b, 0.0);
				lastClickTime.Add(b, 0.0);
				doubleClickNext.Add(b, false);
			}

			// Create the event data (initial capacity for 10 events)
			keyEventList = new List<KeyEventArgs>(10);
			buttonEventList = new List<ButtonEventArgs>(10);
			mouseMoveEvent = null;
			mouseScrollEvent = null;

			// Add no-op handlers to all events to avoid event null-checks later
			KeyPressed = delegate { };
			KeyReleased = delegate { };
			KeyHeld = delegate { };
			ButtonPressed = delegate { };
			ButtonReleased = delegate { };
			ButtonClicked = delegate { };
			ButtonDoubleClicked = delegate { };
			MouseMoved = delegate { };
			MouseScrolled = delegate { };
		}

#pragma warning disable 0649
		// Private structs for queueing up input events
		private struct KeyEventArgs
		{
			public KeyEventType Type;
			public Keys Key;
		}
		private struct ButtonEventArgs
		{
			public ButtonEventType Type;
			public MouseButton Button;
		}
		private struct MouseMoveEventArgs
		{
			public MouseButton Buttons;
			public Point LastPoint;
			public Point CurrPoint;
			public bool IsDrag;
		}
		private struct MouseScrollEventArgs
		{
			public int LastVal;
			public int CurrVal;
		}
#pragma warning restore 0649
	}
}
