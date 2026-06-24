public partial struct Vector3 {
	internal System.Numerics.Vector3 _vec;

#pragma warning disable IDE1006 //naming styles
	public float x {readonly get => _vec.X; set => _vec.X = value;}
	public float y {readonly get => _vec.Y; set => _vec.Y = value;}
	public float z {readonly get => _vec.Z; set => _vec.Z = value;}
#pragma warning restore IDE1006

	public Vector3(float x, float y, float z) : this(new System.Numerics.Vector3(x, y, z)) { }
	public Vector3(float x, float y) : this(x, y, 0) { }
	public Vector3(in Vector3 other) : this(other.x, other.y, other.z) { }
	public Vector3(in Vector2 other, float z) : this(other.x, other.y, z) { }
	public Vector3(float all = 0.0f) : this(all, all, all) { }
	public Vector3(System.Numerics.Vector3 v) {_vec = v;}

	public static readonly Vector3 One = new(1);
	public static readonly Vector3 Zero = new(0);
	//TODO public static Vector3 Random

	public static readonly Vector3 Forward = new(1, 0, 0);
	public static readonly Vector3 Backward = new(-1, 0, 0);
	public static readonly Vector3 Up = new(0, 0, 1);
	public static readonly Vector3 Down = new(0, 0, -1);
	public static readonly Vector3 Right = new(0, -1, 0);
	public static readonly Vector3 Left = new(0, 1, 0);

	public readonly Vector3 Normal => IsNearZeroLength ? Zero : System.Numerics.Vector3.Normalize(_vec);
	public readonly float Length => _vec.Length();
	public readonly float LengthSquared => _vec.LengthSquared();
	public readonly Vector3 Inverse => new(1f / x, 1f / y, 1f / z);

	public readonly bool IsNaN => float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z);
	public readonly bool IsInfinity => float.IsInfinity(x) || float.IsInfinity(y) || float.IsInfinity(z);
	public readonly bool IsNearZeroLength => LengthSquared <= 1e-8f;

	public readonly Vector3 WithX(float x) => new(x, y, z);
	public readonly Vector3 WithY(float y) => new(x, y, z);
	public readonly Vector3 WithZ(float z) => new(x, y, z);

	public readonly bool IsNearlyZero(float tolerance = 0.0001f) {
		var abs = System.Numerics.Vector3.Abs(_vec);
		return abs.X <= tolerance &&
			   abs.Y <= tolerance &&
			   abs.Z <= tolerance;
	}

	public readonly Vector3 ClampLength(float max) {
		var lenSqr = LengthSquared;
		if (lenSqr <= 0.0f)
			return Zero;
		if (lenSqr <= max * max)
			return this;
		return this * (max / MathF.Sqrt(lenSqr));
	}

	public readonly Vector3 ClampLength(float min, float max) {
		float minSqr = min * min;
		float maxSqr = max * max;
		float lenSqr = LengthSquared;
		if (lenSqr <= 0.0f)
			return Zero;
		if (lenSqr <= minSqr)
			return this * (min / MathF.Sqrt(lenSqr));
		if (lenSqr >= maxSqr)
			return this * (max / MathF.Sqrt(lenSqr));
		return this;
	}

	public readonly Vector3 Clamp(float min, float max) => Clamp(new Vector3(min), new Vector3(max));
	public readonly Vector3 Clamp(Vector3 otherMin, Vector3 otherMax) => System.Numerics.Vector3.Clamp(_vec, otherMin._vec, otherMax._vec);
	public static Vector3 Clamp(in Vector3 value, in Vector3 min, in Vector3 max) => System.Numerics.Vector3.Clamp(value, min, max);

	public readonly Vector3 ComponentMin(in Vector3 other) => System.Numerics.Vector3.Min(_vec, other._vec);
	public static Vector3 Min(in Vector3 a, in Vector3 b) => a.ComponentMin(b);

	public readonly Vector3 ComponentMax(in Vector3 other) => System.Numerics.Vector3.Max(_vec, other._vec);
	public static Vector3 Max(in Vector3 a, in Vector3 b) => a.ComponentMax( b );

	public readonly Vector3 LerpTo(in Vector3 target, float frac, bool clamp = true) => Lerp(this, target, frac, clamp);
	public static Vector3 Lerp(Vector3 a, Vector3 b, float frac, bool clamp = true) {
		if (clamp)
			frac = frac.Clamp(0, 1);
		return System.Numerics.Vector3.Lerp(a._vec, b._vec, frac);
	}
	public readonly Vector3 LerpTo(in Vector3 target, in Vector3 frac, bool clamp = true) => Lerp(this, target, frac, clamp);
	public static Vector3 Lerp(in Vector3 a, in Vector3 b, Vector3 frac, bool clamp = true) {
		if (clamp)
			frac = frac.Clamp(0, 1);
		return System.Numerics.Vector3.Lerp(a._vec, b._vec, frac._vec);
	}

	public readonly Vector3 SlerpTo(in Vector3 target, float frac, bool clamp = true) => Slerp(this, target, frac, clamp);
	public static Vector3 Slerp(Vector3 a, Vector3 b, float frac, bool clamp = true) {
		if (clamp)
			frac = frac.Clamp(0, 1);
		var dot = Dot(a, b).Clamp(-1.0f, 1.0f);
		var theta = MathF.Acos(dot) * frac;
		var relative = (b - a * dot).Normal;
		var c = (a * MathF.Cos(theta)) + (relative * MathF.Sin(theta));
		return c.Normal;
	}

	public static float InverseLerp(Vector3 pos, Vector3 a, Vector3 b, bool clamp = true) {
		var delta = b - a;
		var delta2 = pos - a;
		var dot = Dot(delta2, delta) / Dot(delta, delta);
		if (clamp)
			dot = dot.Clamp(0, 1);
		return dot;
	}

	public static Vector3 Cross(in Vector3 a, in Vector3 b) => System.Numerics.Vector3.Cross(a._vec, b._vec);
	public readonly Vector3 Cross(in Vector3 b) => System.Numerics.Vector3.Cross(_vec, b._vec);

	public readonly float Dot(in Vector3 b) => Dot(this, b);
	public static float Dot(in Vector3 a, in Vector3 b) => System.Numerics.Vector3.Dot(a._vec, b._vec);

	public readonly float Distance(in Vector3 target) => Distance(this, target);
	public static float Distance(in Vector3 a, in Vector3 b) => System.Numerics.Vector3.Distance(a._vec, b._vec);

	public readonly float DistanceSquared(in Vector3 target) => DistanceSquared(this, target);
	public static float DistanceSquared(in Vector3 a, in Vector3 b) => System.Numerics.Vector3.DistanceSquared(a._vec, b._vec);

	public static Vector3 Direction(in Vector3 from, in Vector3 to) => (to - from).Normal;
	public readonly Vector3 SubtractDirection(in Vector3 direction, float strength = 1.0f) => this - (direction * Dot(direction) * strength);

	public readonly Vector3 Approach(float length, float amount) => Normal * Length.Approach(length, amount);

	public readonly Vector3 Abs() => System.Numerics.Vector3.Abs(_vec);
	public static Vector3 Abs(in Vector3 value) => System.Numerics.Vector3.Abs(value);

	public static Vector3 Reflect(in Vector3 direction, in Vector3 normal) => System.Numerics.Vector3.Reflect(direction._vec, normal._vec);
	public static Vector3 VectorPlaneProject(in Vector3 v, in Vector3 planeNormal) => v - v.ProjectOnNormal( planeNormal );
	public readonly Vector3 ProjectOnNormal(in Vector3 normal) => (normal * Dot(this, normal));

	public static void Sort(ref Vector3 min, ref Vector3 max) {
		var a = System.Numerics.Vector3.Min(min._vec, max._vec);
		var b = System.Numerics.Vector3.Max(min._vec, max._vec);
		min = a;
		max = b;
	}
	public readonly bool AlmostEqual(in Vector3 v, float delta = 0.0001f) {
		if (MathF.Abs(x - v.x) > delta) return false;
		if (MathF.Abs(y - v.y) > delta) return false;
		if (MathF.Abs(z - v.z) > delta) return false;
		return true;
	}

	public static Vector3 CubicBezier(in Vector3 source, in Vector3 target, in Vector3 sourceTangent, in Vector3 targetTangent, float t) {
		t = t.Clamp(0, 1);
		var invT = 1 - t;
		return invT * invT * invT * source +
			3 * invT * invT * t * sourceTangent +
			3 * invT * t * t * targetTangent +
			t * t * t * target;
	}

	public readonly Vector3 SnapToGrid(float gridSize, bool sx = true, bool sy = true, bool sz = true) {
		return gridSize.AlmostEqual(0) ? this : new Vector3(sx ? x.SnapToGrid(gridSize) : x, sy ? y.SnapToGrid(gridSize) : y, sz ? z.SnapToGrid(gridSize) : z);
	}

	public static float GetAngle(in Vector3 v1, in Vector3 v2) => MathF.Acos(Dot(v1.Normal, v2.Normal).Clamp(-1, 1)).RadianToDegree();
	public readonly float Angle(in Vector3 other) => GetAngle(this, other);
	public static Angles VectorAngle(in Vector3 vec) {
		float tmp, yaw, pitch;
		if (vec.y == 0.0f && vec.x == 0.0f) {
			yaw = 0.0f;
			pitch = (vec.z > 0.0f) ? 270.0f : 90.0f;
		} else {
			yaw = MathF.Atan2( vec.y, vec.x ) * (180.0f / MathF.PI);
			if (yaw < 0.0f)
				yaw += 360.0f;
			tmp = MathF.Sqrt( vec.x * vec.x + vec.y * vec.y );
			pitch = MathF.Atan2( -vec.z, tmp ) * (180.0f / MathF.PI);
			if (pitch < 0.0f)
				pitch += 360.0f;
		}
		return new Angles(pitch, yaw, 0);
	}
	public Angles EulerAngles {
		readonly get => VectorAngle(this);
		set {this = Angles.AngleVector(value);}
	}

	public readonly Vector3 AddClamped(in Vector3 toAdd, float maxLength) {
		var dir = toAdd.Normal;

		// Already over - just return self
		var dot = Dot(dir);
		if (dot > maxLength)
			return this;
		// Add it
		var vec = this + toAdd;
		dot = vec.Dot(dir);
		if (dot < maxLength)
			return vec;
		// We're over, take off the rest
		vec -= dir * (dot - maxLength);
		return vec;
	}

	public readonly Vector3 RotateAround(in Vector3 center, in Rotation rot) => center + (rot * (this - center));

	public readonly Vector3 WithAcceleration(Vector3 target, float acceleration) {
		if (target.IsNearZeroLength)
			return this;
		Vector3 wishdir = target.Normal;
		float wishspeed = target.Length;
		// See if we are changing direction a bit
		var currentspeed = Dot(wishdir);
		// Reduce wishspeed by the amount of veer.
		var addspeed = wishspeed - currentspeed;
		// If not going to add any speed, done.
		if (addspeed <= 0)
			return this;
		// Determine amount of acceleration.
		var accelspeed = acceleration * wishspeed;
		// Cap at addspeed
		if (accelspeed > addspeed)
			accelspeed = addspeed;
		return this + wishdir * accelspeed;
	}
	public readonly Vector3 WithFriction(float frictionAmount, float stopSpeed = 140.0f) {
		var speed = Length;
		if (speed < 0.01f)
			return this;

		// Bleed off some speed, but if we have less than the bleed
		//  threshold, bleed the threshold amount.
		float control = (speed < stopSpeed) ? stopSpeed : speed;
		// Add the amount to the drop amount.
		var drop = control * frictionAmount;
		// scale the velocity
		float newspeed = speed - drop;
		if (newspeed < 0)
			newspeed = 0;
		if (newspeed == speed)
			return this;
		newspeed /= speed;
		return this * newspeed;
	}

	public static Vector3 CatmullRomSpline(in Vector3 p0, in Vector3 p1, in Vector3 p2, in Vector3 p3, float t) {
		var t2 = t * t;
		var t3 = t2 * t;

		var part1 = -t3 + 2.0f * t2 - t;
		var part2 = 3.0f * t3 - 5.0f * t2 + 2.0f;
		var part3 = -3.0f * t3 + 4.0f * t2 + t;
		var part4 = t3 - t2;

		var blendedPoint = 0.5f * (p0 * part1 + p1 * part2 + p2 * part3 + p3 * part4);
		return blendedPoint;
	}

	public static Vector3 TcbSpline(in Vector3 p0, in Vector3 p1, in Vector3 p2, in Vector3 p3, float tension, float continuity, float bias, float u) {
		// Compute the tangent vectors using the TCB parameters
		Vector3 m1 = (1 - tension) * (1 + continuity) * (1 + bias) * (p1 - p0) / 2 +
					 (1 - tension) * (1 - continuity) * (1 - bias) * (p2 - p1) / 2;
		Vector3 m2 = (1 - tension) * (1 - continuity) * (1 + bias) * (p2 - p1) / 2 +
					 (1 - tension) * (1 + continuity) * (1 - bias) * (p3 - p2) / 2;

		// Compute the coefficients of the cubic polynomial
		Vector3 a = 2 * (p1 - p2) + m1 + m2;
		Vector3 bCoeff = -3 * (p1 - p2) - 2 * m1 - m2;
		Vector3 cCoeff = m1;
		Vector3 d = p1;

		// Compute and return the position on the curve
		return a * u * u * u + bCoeff * u * u + cCoeff * u + d;
	}
}