namespace Scene;

public class Transform {
	public global::Transform WorldTransform {get; set;} = global::Transform.Zero;
	public Vector3 WorldPosition {
		get => WorldTransform.Position;
		set => WorldTransform = WorldTransform.WithPosition(value);
	}
	public Rotation WorldRotation {
		get => WorldTransform.Rotation;
		set => WorldTransform = WorldTransform.WithRotation(value);
	}
	public Vector3 WorldScale {
		get => WorldTransform.Scale;
		set => WorldTransform = WorldTransform.WithScale( value );
	}
}