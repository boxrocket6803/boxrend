namespace Scene.Camera;

public class Perspective : Base {
	public Transform WorldTransform {get; set;} = Transform.Indentity; //TODO move to Scene.Object
	public float FieldOfView {get; set;} = 90;

	private int Cache;
	public override void Update() {
		var hc = HashCode.Combine(WorldTransform, FieldOfView, Graphics.AspectRatio);
		if (hc != Cache) {
			Attributes.Set("view", Matrix4x4.CreateLookAt(Vector3.UnitX * 80f + Vector3.UnitZ * 32f, Vector3.UnitZ * 32f, Vector3.UnitZ)); //TODO use real numbesr
			Attributes.Set("proj", Matrix4x4.CreatePerspectiveFieldOfView(Math.Clamp(FieldOfView * 0.0174533f, 0f, 1.57f), Graphics.AspectRatio, 3f, 4000f));
			Cache = hc;
		}
	}
}