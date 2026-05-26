public struct Transform() { //TODO find a better place for this
	public static Transform Indentity {get; private set;} = new();

	public Vector3 Position {get; set;} = Vector3.Zero;
	public Vector3 Scale {get; set;} = Vector3.One;
	public Quaternion Rotation {get; set;} = Quaternion.Identity;
}