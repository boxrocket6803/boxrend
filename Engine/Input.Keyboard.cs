using Silk.NET.Input;

public partial class Input {
	public class KeyboardAccessor {
		public Vector3 Move {get; private set;}

		public void Update() {
			Move = Vector3.Zero;
			if (State.Down.Contains("Move.Forward"))
				Move += Vector3.Forward;
			if (State.Down.Contains("Move.Left"))
				Move += Vector3.Left;
			if (State.Down.Contains("Move.Right"))
				Move += Vector3.Right;
			if (State.Down.Contains("Move.Backward"))
				Move += Vector3.Backward;
			Move = Move.Normal;
		}
	}
	private struct InternalState() {
		public readonly HashSet<string> Pressed = [];
		public readonly HashSet<string> Down = [];
		public readonly HashSet<string> Released = [];

		public void Update(IInputContext c) {
			State.Released.Clear();
			foreach (var input in State.Down)
				State.Released.Add(input);
			State.Pressed.Clear();
			foreach (var mouse in c.Mice) {
				foreach (var input in Bindings.Mouse) {
					if (mouse.IsButtonPressed(input.Value))
						State.Pressed.Add(input.Key);
				}
			}
			foreach (var keyboard in c.Keyboards) {
				foreach (var input in Bindings.Keyboard) {
					if (keyboard.IsKeyPressed(input.Value))
						State.Pressed.Add(input.Key);
				}
			}
			foreach (var input in State.Pressed) {
				State.Released.Remove(input);
				if (State.Down.Contains(input))
					State.Pressed.Remove(input);
				else
					State.Down.Add(input);
			}
			foreach (var input in State.Released)
				State.Down.Remove(input);
		}
	}

	private static readonly InternalState State = new();
	public static KeyboardAccessor Keyboard {get; set;} = new();

	public static bool Pressed(string action) => State.Pressed.Contains(action);
	public static bool Down(string action) => State.Down.Contains(action);
	public static bool Released(string action) => State.Released.Contains(action);
}