using Silk.NET.Input;

public partial class Input {
	public class MouseAccessor {
		public Vector2 Position {get; set;}
		public Vector2 Delta {get; private set;}
		public Angles Look {get; private set;}
		public float Wheel {get; private set;}
		public bool Freeze {get; set;}

		public void Init(IInputContext c) {
			var pos = c.Mice[0].Position * 2;
			Position = new(pos.X, -pos.Y);
		}
		public void Update(IInputContext c) {
			var pos = c.Mice[0].Position * 2;
			Mouse.Delta = new Vector2(pos.X, -pos.Y) - Mouse.Position;
			if (Freeze)
				c.Mice[0].Position = new Vector2(Mouse.Position.x, -Mouse.Position.y) * 0.5f;
			else
				Mouse.Position += Mouse.Delta;
			Mouse.Wheel = c.Mice[0].ScrollWheels[0].Y;

			var sens = MathF.Max(Graphics.Screen.Size.x, Graphics.Screen.Size.y) * 0.5f;
			if (sens < 1) sens = 1;
			sens = 10 / sens;
			sens *= Bindings.MouseSensitivity;
			Look = new(-Delta.y * sens, -Delta.x * sens, 0);
		}
	}

	public static MouseAccessor Mouse {get; set;} = new();
}