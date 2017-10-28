using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NGSim.Input
{
	// The event type for a key event
	public enum KeyEventType
	{
		Pressed,
		Released,
		Held
	}
	// The event type for a mouse button event
	public enum ButtonEventType
	{
		Pressed,
		Released,
		Clicked,
		DoubleClicked
	}

	// Delegate for KeyPressed, KeyReleased, and KeyHeld
	public delegate void KeyEvent(KeyEventType type, Keys key);

	// Delegate for MousePressed, MouseReleased, MouseClicked, and MouseDoubleClicked
	public delegate void ButtonEvent(ButtonEventType type, MouseButton button);

	// Delegate for MouseMoved and MouseDragged
	public delegate void MouseMoveEvent(MouseButton buttons, Point lastPos, Point currPos);

	// Delegate for MouseScroll
	public delegate void MouseScrollEvent(int lastVal, int currVal);
}
