using Silk.NET.Input;

public partial class Input {
	public class BindingAccessor {
		public readonly Dictionary<string, MouseButton> Mouse = [];
		public readonly Dictionary<string, Key> Keyboard = [];

		public void Add(string action, MouseButton button) {
			if (Mouse.ContainsKey(action))
				return;
			Mouse[action] = button;
		}
		public void Add(string action, Key button) {
			if (Keyboard.ContainsKey(action))
				return;
			Keyboard[action] = button;
		}

		public void Init() {
			Add("Move.Forward", Key.W);
			Add("Move.Left", Key.A);
			Add("Move.Right", Key.D);
			Add("Move.Backward", Key.S);
		}
	}

	public static BindingAccessor Bindings {get; set;} = new();
}