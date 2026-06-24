public partial struct Angles {
	public static Angles operator +(Angles c1, Angles c2) => new(c1.pitch + c2.pitch, c1.yaw + c2.yaw, c1.roll + c2.roll);
	public static Angles operator +(Angles c1, Vector3 c2) => new(c1.pitch + c2.x, c1.yaw + c2.y, c1.roll + c2.z);
	public static Angles operator -(Angles c1, Angles c2) => new(c1.pitch - c2.pitch, c1.yaw - c2.yaw, c1.roll - c2.roll);

	public static Angles operator *(Angles c1, float c2) => new(c1.pitch * c2, c1.yaw * c2, c1.roll * c2);
	public static Angles operator /(Angles c1, float c2) => new(c1.pitch / c2, c1.yaw / c2, c1.roll / c2);

	public static bool operator ==(in Angles left, in Angles right) => left.Equals( right );
	public static bool operator !=(in Angles left, in Angles right) => !(left == right);

	public override readonly bool Equals(object obj) => obj is Angles o && Equals(o);
	public readonly bool Equals(Angles o) => (pitch, yaw, roll) == (o.pitch, o.yaw, o.roll);
	public readonly override int GetHashCode() => HashCode.Combine(pitch, yaw, roll);
}