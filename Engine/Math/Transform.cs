public struct Transform() {
	public static Transform Indentity = new();

	public Vector3 Position {get; set;} = Vector3.Zero;
	public Vector3 Scale {get; set;} = Vector3.One;
	public Quaternion Rotation {get; set;} = Quaternion.Identity;

	public readonly Matrix4x4 ToMatrix() {
		var sca = Matrix4x4.CreateScale(Scale);
		var rot = Matrix4x4.CreateFromQuaternion(Rotation);
		var pos = Matrix4x4.CreateTranslation(Position);
		return sca * rot * pos;
	}
}