namespace Scene.Camera;

public class Perspective : Base {
	public float FieldOfView {get; set;} = 90;

	private int Cache;
	public override void Update() {
		var hc = HashCode.Combine(WorldTransform, FieldOfView, Graphics.Screen.Ratio);
		if (hc != Cache) {
			Attributes.Set("view", Matrix4x4.CreateLookToLeftHanded(WorldPosition, WorldRotation.Forward, WorldRotation.Up));
			Attributes.Set("proj", Matrix4x4.CreatePerspectiveFieldOfView(Math.Clamp(FieldOfView * 0.0174533f, 0f, 1.57f), Graphics.Screen.Ratio, 3f, 4000f));
			Cache = hc;
		}
	}
}