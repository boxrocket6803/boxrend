using System.Numerics;

public class Camera2D : Scene.Camera {
	public float Zoom {get; set;} = 1;
	public Vector2 Position {get; set;}

	public Vector2 ScreenToWorld(Vector2 p) {
		p.X -= Scene.Game.Window.FramebufferSize.X;
		p.Y += Scene.Game.Window.FramebufferSize.Y;
		p -= Position;
		p /= Zoom;
		return p;
	}

	public Vector2 WorldToScreen(Vector2 p) {
		p *= Zoom;
		p += Position;
		p.X += Scene.Game.Window.FramebufferSize.X;
		p.Y -= Scene.Game.Window.FramebufferSize.Y;
		return p;
	}

	public override void Update(Graphics.Program p) {
		p.Set("inv_frame_size", Vector2.Create(1f / Scene.Game.Window.FramebufferSize.X, 1f / Scene.Game.Window.FramebufferSize.Y));
		p.Set("camera_position", Position);
		p.Set("camera_zoom", Zoom);
		base.Update(p);
	}
}