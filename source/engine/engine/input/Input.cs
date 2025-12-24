using Silk.NET.Input;
using Silk.NET.Windowing;
using System.Collections.Generic;

public class Input {
	private IInputContext Context;
	private readonly Dictionary<string, MouseButton> MouseMapping = new() {
		{"DRAG", MouseButton.Middle},
	};
	private readonly Dictionary<string, Key> KeyMapping = new() {

	};
	private static readonly HashSet<string> InternalPressed = [];
	private static readonly HashSet<string> InternalDown = [];
	private static readonly HashSet<string> InternalReleased = [];
	public static System.Numerics.Vector2 MousePosition {get; private set;}
	public static System.Numerics.Vector2 MouseDelta {get; private set;}
	public static float MouseWheel {get; set;}
	public static bool Pressed(string action) => InternalPressed.Contains(action);
	public static bool Down(string action) => InternalDown.Contains(action);
	public static bool Released(string action) => InternalReleased.Contains(action);

	public void Init(IWindow window) {
		Context = window.CreateInput();
		MousePosition = GetMouse();
	}

	private System.Numerics.Vector2 GetMouse() {
		var pos = Context.Mice[0].Position * 2;
		return new(pos.X, -pos.Y);
	}
	public void Update() {
		InternalReleased.Clear();
		foreach (var input in InternalDown)
			InternalReleased.Add(input);
		InternalPressed.Clear();
		foreach (var mouse in Context.Mice) {
			foreach (var input in MouseMapping) {
				if (mouse.IsButtonPressed(input.Value))
					InternalPressed.Add(input.Key);
			}
		}
		foreach (var keyboard in Context.Keyboards) {
			foreach (var input in KeyMapping) {
				if (keyboard.IsKeyPressed(input.Value))
					InternalPressed.Add(input.Key);
			}
		}
		foreach (var input in InternalPressed) {
			InternalReleased.Remove(input);
			if (InternalDown.Contains(input))
				InternalPressed.Remove(input);
			else
				InternalDown.Add(input);
		}
		foreach (var input in InternalReleased)
			InternalDown.Remove(input);
		MouseDelta = GetMouse() - MousePosition;
		MousePosition += MouseDelta;
		MouseWheel = Context.Mice[0].ScrollWheels[0].Y;
	}
}