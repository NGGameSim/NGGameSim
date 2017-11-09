using System;
using Microsoft.Xna.Framework.Input;

namespace NGSim.Input
{
	// Implements a MouseButton type, since MonoGame frustratingly lacks one
	[Flags]
	public enum MouseButton : byte
	{
		None = 0x00,
		Left = 0x01,
		Middle = 0x02,
		Right = 0x04,
		X1 = 0x08,
		X2 = 0x10,
		Any = Left | Middle | Right | X1 | X2
	}

	// Extension methods for easily mapping between MonoGame and MouseButton states
	public static class MouseButtonExtensions
	{
		public static ButtonState GetState(this MouseButton button, MouseState state)
		{
			switch (button)
			{
				case MouseButton.Left:
					return state.LeftButton;
				case MouseButton.Middle:
					return state.MiddleButton;
				case MouseButton.Right:
					return state.RightButton;
				case MouseButton.X1:
					return state.XButton1;
				case MouseButton.X2:
					return state.XButton2;
				default:
					throw new InvalidOperationException("Cannot get the mapped state of the 'None' mouse button.");
			}
		}

		public static ButtonState GetState(this MouseState state, MouseButton button)
		{
			switch (button)
			{
				case MouseButton.Left:
					return state.LeftButton;
				case MouseButton.Middle:
					return state.MiddleButton;
				case MouseButton.Right:
					return state.RightButton;
				case MouseButton.X1:
					return state.XButton1;
				case MouseButton.X2:
					return state.XButton2;
				default:
					throw new InvalidOperationException("Cannot get the mapped state of the 'None' mouse button.");
			}
		}
	}
}
