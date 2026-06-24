public struct Transform() {
	public static Transform Indentity = new();

	public Vector3 Position {get; set;} = Vector3.Zero;
	public Vector3 Scale {get; set;} = Vector3.One;
	public Quaternion Rotation {get; set;} = Quaternion.Identity;
}