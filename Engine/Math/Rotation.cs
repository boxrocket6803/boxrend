public partial struct Rotation {
	internal Quaternion _quat = Quaternion.Identity;

#pragma warning disable IDE1006 //naming styles
	public float x {readonly get => _quat.X; set => _quat.X = value;}
	public float y {readonly get => _quat.Y; set => _quat.Y = value;}
	public float z {readonly get => _quat.Z; set => _quat.Z = value;}
	public float w {readonly get => _quat.W; set => _quat.W = value;}
#pragma warning restore IDE1006

	public Rotation(float x, float y, float z, float w) => _quat = new(x, y, z, w);
	public Rotation(Vector3 v, float w) => _quat = new Quaternion(v.x, v.y, v.z, w);

	public static readonly Rotation Identity = new();
	//TODO public static Rotation Random;

	public readonly Rotation Inverse => Quaternion.Inverse(_quat);
	public readonly Rotation Normal => Quaternion.Normalize(_quat);
	public readonly Rotation Conjugate => Quaternion.Conjugate(_quat);

	public readonly Vector3 Forward => Vector3.Forward * this;
	public readonly Vector3 Backward => Vector3.Backward * this;
	public readonly Vector3 Right => Vector3.Right * this;
	public readonly Vector3 Left => Vector3.Left * this;
	public readonly Vector3 Up => Vector3.Up * this;
	public readonly Vector3 Down => Vector3.Down * this;

	public readonly float Pitch() {
		float m13 = (2.0f * x * z) - (2.0f * w * y);
		return MathF.Asin(Math.Clamp(-m13, -1f, 1f)).RadianToDegree();
	}
	public readonly float Yaw() {
		float m11 = (2.0f * w * w) + (2.0f * x * x) - 1.0f;
		float m12 = (2.0f * x * y) + (2.0f * w * z);
		return MathF.Atan2(m12, m11).RadianToDegree();
	}
	public readonly float Roll() {
		float m23 = (2.0f * y * z) + (2.0f * w * x);
		float m33 = (2.0f * w * w) + (2.0f * z * z) - 1.0f;
		return MathF.Atan2(m23, m33).RadianToDegree();
	}

	public readonly float Distance(Rotation to) => Difference(this, to).Angle();

	public static Rotation Difference(Rotation from, Rotation to) {
		var fromInv = Quaternion.Conjugate(from._quat);
		var diff = Quaternion.Multiply(to._quat, fromInv);
		return Quaternion.Normalize(diff);
	}

	public readonly float Angle() {
		float d = MathF.Acos(w.Clamp(-1, 1)).RadianToDegree() * 2.0f;
		if (d > 180) d -= 360;
		return MathF.Abs(d);
	}

	public readonly Angles Angles() {
		Angles a;

		var m13 = (2.0f * x * z) - (2.0f * w * y);
		a.pitch = MathF.Asin(Math.Clamp(-m13, -1f, 1f)).RadianToDegree();
		if (Math.Abs(m13).AlmostEqual(1f)) {
			// North / south pole singularities

			var m21 = 2f * (w * z - x * y);
			var m31 = 2f * (x * z + w * y);

			var sign = -Math.Sign( m13 );

			a.pitch = (MathF.PI / 2 * sign).RadianToDegree();
			a.yaw = sign * MathF.Atan2( m21 * sign, m31 * sign ).RadianToDegree();
			a.roll = 0f;
		} else {
			// Normal case

			var m11 = 2f * (w * w + x * x) - 1f;
			var m12 = 2f * (x * y + w * z);
			var m23 = 2f * (y * z + w * x);
			var m33 = 2f * (w * w + z * z) - 1f;

			a.yaw = MathF.Atan2( m12, m11 ).RadianToDegree();
			a.roll = MathF.Atan2( m23, m33 ).RadianToDegree();
		}

		return a;
	}

	public readonly Rotation LerpTo(Rotation target, float frac, bool clamp = true) => Lerp(this, target, frac, clamp);
	public static Rotation Lerp(Rotation a, Rotation b, float frac, bool clamp = true) {
		if (clamp)
			frac = frac.Clamp(0, 1);
		return Quaternion.Lerp(a._quat, b._quat, frac);
	}

	public readonly Rotation SlerpTo(Rotation target, float frac, bool clamp = true) => Slerp(this, target, frac, clamp);
	public static Rotation Slerp(Rotation a, Rotation b, float amount, bool clamp = true) {
		if (clamp)
			amount = amount.Clamp(0, 1);
		return Quaternion.Slerp(a._quat, b._quat, amount);
	}

	public readonly Rotation Clamp(Rotation to, float degrees) => Clamp(to, degrees, out var _);
	public readonly Rotation Clamp(Rotation to, float degrees, out float change) {
		change = 0;
		if (degrees <= 0)
			return to;
		var diff = Difference(this, to);
		var d = diff.Angle();
		if ( d <= degrees )
			return this;
		change = d - degrees;
		var amount = degrees / d;
		return Slerp(this, to, 1 - amount);
	}

	public readonly Rotation RotateAroundAxis(Vector3 axis, float degrees) => this * FromAxis(axis, degrees);

	internal static Rotation Exp(Vector3 V) {
		const float kThreshold = 0.018581361f;
		float Angle = V.Length;
		if (Angle < kThreshold)
			return new Rotation((0.5f + Angle * Angle / 48.0f) * V, MathF.Cos(0.5f * Angle));
		else
			return Quaternion.CreateFromAxisAngle(V / Angle, Angle);
	}
	
	public readonly Vector3 ClosestAxis(Vector3 normal) {
		normal = normal.Normal;

		var axis = new Vector3[6];
		axis[0] = Forward;
		axis[1] = Left;
		axis[2] = Up;
		axis[3] = -axis[0];
		axis[4] = -axis[1];
		axis[5] = -axis[2];

		var bestAxis = Vector3.Zero;
		var bestDot = -1.0f;

		for (var i = 0; i < 6; i++) {
			var dot = normal.Dot(axis[i]);
			if (dot <= bestDot)
				continue;
			bestDot = dot;
			bestAxis = axis[i];
		}

		return bestAxis;
	}
}