namespace Scene.Camera;

public class Perspective : Base {
	public float FieldOfView {get; set;} = 90;

	public override void Update() => Data = new() {
		Projection = Matrix4x4.CreatePerspectiveFieldOfView(Math.Clamp(FieldOfView * 0.0174533f, 0.01f, 3f), Graphics.Screen.Ratio, 3f, 4000f),
		View = Matrix4x4.CreateLookTo(WorldPosition, WorldRotation.Forward, WorldRotation.Up),
		Position = WorldPosition,
		Forward = WorldRotation.Forward,
	};
}