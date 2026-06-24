public partial struct Rotation {
	public static bool operator ==(Rotation left, Rotation right) => left.AlmostEqual(right);
	public static bool operator !=(Rotation left, Rotation right) => !left.AlmostEqual(right);
	public readonly override bool Equals(object obj) => obj is Rotation o && Equals(o);
	public readonly bool Equals(Rotation o) => _quat.Equals(o._quat);
	public readonly override int GetHashCode() => _quat.GetHashCode();

	public readonly bool AlmostEqual(in Rotation r, float delta = 0.00001f) {
		static bool AlmostEqualCore(in Quaternion a, in Quaternion b, float delta) {
			return a.X.AlmostEqual(b.X, delta)
				&& a.Y.AlmostEqual(b.Y, delta)
				&& a.Z.AlmostEqual(b.Z, delta)
				&& a.W.AlmostEqual(b.W, delta);
		}
		// quat and -quat represent the same rotation
		return AlmostEqualCore(_quat, r._quat, delta) || AlmostEqualCore(_quat, -r._quat, delta);
	}
}