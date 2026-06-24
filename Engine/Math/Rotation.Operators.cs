public partial struct Rotation {
	public static implicit operator Rotation(in Quaternion value) => new() {_quat = value};

	public static implicit operator Rotation(in Angles value) => From(value);
	public static implicit operator Angles(in Rotation value) => value.Angles();

	public static implicit operator Quaternion(in Rotation value) => new(value.x, value.y, value.z, value.w);

	public static Vector3 operator *(in Rotation f, in Vector3 c1) => System.Numerics.Vector3.Transform(c1._vec, f._quat);
	public static Rotation operator *(Rotation a, Rotation b) => Quaternion.Multiply(a._quat, b._quat);
	public static Rotation operator *(Rotation a, float f) => Quaternion.Slerp(Quaternion.Identity, a._quat, f);

	public static Rotation operator /(Rotation a, float f) => Quaternion.Slerp(Quaternion.Identity, a._quat, 1 / f);
}