using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

public partial struct Vector2 {
	internal System.Numerics.Vector2 _vec;

#pragma warning disable IDE1006 //naming styles
	public float x {readonly get => _vec.X; set => _vec.X = value;}
	public float y {readonly get => _vec.Y; set => _vec.Y = value;}
#pragma warning restore IDE1006

	public Vector2(float x, float y) : this(new System.Numerics.Vector2(x, y)) { }
	public Vector2(in Vector2 other) : this(other.x, other.y) { }
	public Vector2(float all) : this(all, all) { }
	public Vector2(Vector3 v) : this(new System.Numerics.Vector2(v.x, v.y)) { }
	//TODO public Vector2(Vector4 v) : this(new System.Numerics.Vector2(v.x, v.y)) { }
	public Vector2(System.Numerics.Vector2 v) {_vec = v;}

	public static readonly Vector2 One = new(1);
	public static readonly Vector2 Zero = new(0);
	//TODO public static Vector2 Random

	public static readonly Vector2 Up = new(0, -1);
	public static readonly Vector2 Down = new(0, 1);
	public static readonly Vector2 Left = new(-1, 0);
	public static readonly Vector2 Right = new(1, 0);

	public readonly Vector2 Normal => IsNearZeroLength ? Zero : System.Numerics.Vector2.Normalize(_vec);
	public readonly float Length => _vec.Length();
	public readonly float LengthSquared => _vec.LengthSquared();
	public readonly Vector2 Inverse => new(1.0f / x, 1.0f / y);
	public readonly float Degrees => MathF.Atan2(x, -y).RadianToDegree().NormalizeDegrees();
	public readonly Vector2 Perpendicular => new( -y, x );

	public readonly bool IsNaN => float.IsNaN(x) || float.IsNaN(y);
	public readonly bool IsInfinity => float.IsInfinity(x) || float.IsInfinity(y);
	public readonly bool IsNearZeroLength => LengthSquared <= 1e-8;

	public readonly Vector2 WithX(float x) => new(x, y);
	public readonly Vector2 WithY(float y) => new(x, y);

	public static Vector2 FromRadians(float radians) => new(MathF.Sin(radians), -MathF.Cos(radians));
	public static Vector2 FromDegrees(float degrees) => FromRadians(degrees.DegreeToRadian());

	public readonly bool IsNearlyZero(float tolerance = 0.0001f) {
		var abs = System.Numerics.Vector2.Abs(_vec);
		return abs.X < tolerance && abs.Y < tolerance;
	}

	public readonly Vector2 ClampLength(float maxLength) {
		if (LengthSquared <= 0)
			return Zero;
		if (LengthSquared < (maxLength * maxLength))
			return this;
		return Normal * maxLength;
	}
	public readonly Vector2 ClampLength(float minLength, float maxLength) {
		float minSqr = minLength * minLength;
		float maxSqr = maxLength * maxLength;
		float lenSqr = LengthSquared;
		if ( lenSqr <= 0.0f )
			return Zero;
		if ( lenSqr <= minSqr )
			return Normal * minLength;
		if ( lenSqr >= maxSqr )
			return Normal * maxLength;
		return this;
	}

	public readonly Vector2 Clamp(float min, float max) => Clamp(new Vector2(min), new Vector2(max));
	public readonly Vector2 Clamp(Vector2 min, Vector2 max) => new(Math.Clamp(x, min.x, max.x), Math.Clamp(y, min.y, max.y));
	public static Vector2 Clamp(in Vector2 value, in Vector2 min, in Vector2 max) => System.Numerics.Vector2.Clamp(value, min, max);

	public static Vector2 Min(Vector2 a, Vector2 b) => a.ComponentMin(b);
	public readonly Vector2 ComponentMin(Vector2 other) => System.Numerics.Vector2.Min(_vec, other._vec);
	public static Vector2 Max(Vector2 a, Vector2 b) => a.ComponentMax(b);
	public readonly Vector2 ComponentMax(Vector2 other) => System.Numerics.Vector2.Max(_vec, other._vec);

	public readonly Vector2 LerpTo(Vector2 target, float t, bool clamp = true) => Lerp(this, target, t, clamp);
	public static Vector2 Lerp(Vector2 a, Vector2 b, float frac, bool clamp = true) {
		if (clamp)
			frac = frac.Clamp(0.0f, 1.0f);
		return System.Numerics.Vector2.Lerp(a._vec, b._vec, frac);
	}
	public readonly Vector2 LerpTo(Vector2 target, Vector2 t, bool clamp = true) => Lerp(this, target, t, clamp);
	public static Vector2 Lerp(Vector2 a, Vector2 b, Vector2 t, bool clamp = true) {
		if (clamp)
			t = t.Clamp(0.0f, 1.0f);
		return System.Numerics.Vector2.Lerp(a._vec, b._vec, t);
	}

	public readonly float Dot(in Vector2 b) => Dot(this, b);
	public static float Dot(Vector2 a, Vector2 b) => System.Numerics.Vector2.Dot(a._vec, b._vec);

	public readonly float Distance(Vector2 target) => Distance(this, target);
	public static float Distance(Vector2 a, Vector2 b) => System.Numerics.Vector2.Distance(a._vec, b._vec);
	public static float Distance(in Vector2 a, in Vector2 b) => System.Numerics.Vector2.Distance(a._vec, b._vec);

	public readonly float DistanceSquared(Vector2 target) => DistanceSquared(this, target);
	public static float DistanceSquared(Vector2 a, Vector2 b) => (b - a).LengthSquared;
	public static float DistanceSquared(in Vector2 a, in Vector2 b) => System.Numerics.Vector2.DistanceSquared(a._vec, b._vec);

	public static Vector2 Direction(in Vector2 from, in Vector2 to) => (to - from).Normal;

	public readonly Vector2 SubtractDirection(in Vector2 direction, float strength = 1.0f) => this - (direction * Dot( direction ) * strength);

	public readonly Vector2 Approach(float length, float amount) => Normal * Length.Approach(length, amount);

	public readonly Vector2 Abs() => System.Numerics.Vector2.Abs(_vec);
	public static Vector2 Abs(in Vector2 value) => System.Numerics.Vector2.Abs(value._vec);

	public static Vector2 Reflect(in Vector2 direction, in Vector2 normal) => System.Numerics.Vector2.Reflect(direction._vec, normal._vec);

	public static void Sort(ref Vector2 min, ref Vector2 max) {
		var a = new Vector2(Math.Min(min.x, max.x), Math.Min(min.y, max.y));
		var b = new Vector2(Math.Max(min.x, max.x), Math.Max(min.y, max.y));
		min = a;
		max = b;
	}

	public readonly bool AlmostEqual(Vector2 v, float delta = 0.0001f) {
		if (Math.Abs( x - v.x) > delta) return false;
		if (Math.Abs( y - v.y) > delta) return false;
		return true;
	}

	public static Vector2 CubicBezier(in Vector2 source, in Vector2 target, in Vector2 sourceTangent, in Vector2 targetTangent, float t) {
		t = t.Clamp(0, 1);
		var invT = 1 - t;
		return invT * invT * invT * source +
			3 * invT * invT * t * sourceTangent +
			3 * invT * t * t * targetTangent +
			t * t * t * target;
	}

	public readonly Vector2 SnapToGrid(float gridSize, bool sx = true, bool sy = true) => new(sx ? x.SnapToGrid(gridSize) : x, sy ? y.SnapToGrid(gridSize) : y);

	public readonly float Angle(in Vector2 other) => GetAngle(this, other);
	public static float GetAngle(in Vector2 v1, in Vector2 v2) => MathF.Acos(Dot(v1.Normal, v2.Normal).Clamp(-1, 1)).RadianToDegree();

	public readonly Vector2 AddClamped(in Vector2 toAdd, float maxLength) {
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

	public readonly Vector2 RotateAround(in Vector2 center, float angleDegrees) {
		var radians = angleDegrees.DegreeToRadian();
		var cos = MathF.Cos(radians);
		var sin = MathF.Sin(radians);
		var dx = x - center.x;
		var dy = y - center.y;
		return new(center.x + (dx * cos - dy * sin), center.y + (dx * sin + dy * cos));
	}

	public readonly Vector2 WithAcceleration(Vector2 target, float accelerate) {
		if (target.IsNearZeroLength)
			return this;
		Vector2 wishDir = target.Normal;
		float wishSpeed = target.Length;
		// See if we are changing direction a bit
		var currentSpeed = Dot(wishDir);
		// Reduce wishspeed by the amount of veer
		var addSpeed = wishSpeed - currentSpeed;
		// If not going to add any speed, done.
		if (addSpeed <= 0.0f)
			return this;
		// Determine amount of acceleration
		var accelSpeed = accelerate * wishSpeed;
		// Cap at addSpeed
		if (accelSpeed > addSpeed)
			accelSpeed = addSpeed;
		return this + wishDir * accelSpeed;
	}
	public readonly Vector2 WithFriction(float frictionAmount, float stopSpeed = 140.0f) {
		var speed = Length;
		if (speed < 0.01f)
			return this;
		// Bleed off some speed, but if we have less than the bleed
		// threshold, bleed the threshold amount
		float control = (speed < stopSpeed) ? stopSpeed : speed;
		// Add the amount to the drop amount
		var drop = control * frictionAmount;
		// Scale the velocity
		float newSpeed = speed - drop;
		if (newSpeed < 0)
			newSpeed = 0;
		if (newSpeed == speed)
			return this;
		newSpeed /= speed;
		return this * newSpeed;
	}
}