namespace Scene.Camera;

public class Perspective : Base {
	public float FieldOfView {get; set;} = 90;

	private int Cache;
	public override void Update() {
		var hc = HashCode.Combine(WorldTransform, FieldOfView, Graphics.Screen.Ratio);
		if (hc != Cache) {
			Attributes.Set("Camera.View", Matrix4x4.CreateLookTo(WorldPosition, WorldRotation.Forward, WorldRotation.Up));
			Attributes.Set("Camera.Proj", Matrix4x4.CreatePerspectiveFieldOfView(Math.Clamp(FieldOfView * 0.0174533f, 0.01f, 3f), Graphics.Screen.Ratio, 3f, 4000f));
			Attributes.Set("Camera.Position", WorldPosition);
			Attributes.Set("Camera.Forward", WorldRotation.Forward);
			Cache = hc;
		}
	}
}