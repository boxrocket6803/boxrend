public partial struct Angles {
	public float pitch;
	public float yaw;
	public float roll;

	public Angles(float pitch, float yaw, float roll) {
		this.pitch = pitch;
		this.yaw = yaw;
		this.roll = roll;
	}
	public Angles(Angles other) {
		pitch = other.pitch;
		yaw = other.yaw;
		roll = other.roll;
	}
	public Angles(Vector3 vector) {
		pitch = vector.x;
		yaw = vector.y;
		roll = vector.z;
	}
	public Angles(float all = 0.0f) : this(all, all, all) { }

	public static readonly Angles Zero = new(0);
	//TODO public static Angles Random => Rotation.Random.Angles();

	public readonly Angles Normal => new(NormalizeAngle(pitch), NormalizeAngle(yaw), NormalizeAngle(roll));
	public Vector3 Forward {
		readonly get => AngleVector(this);
		set {this = Vector3.VectorAngle(value);}
	}

	public readonly Angles WithPitch(float pitch) => new(pitch, yaw, roll);
	public readonly Angles WithYaw(float yaw) => new(pitch, yaw, roll);
	public readonly Angles WithRoll(float roll) => new(pitch, yaw, roll);

	public readonly bool IsNearlyZero(double tolerance = 0.000001) {
		return MathF.Abs(pitch) <= tolerance &&
			   MathF.Abs(yaw) <= tolerance &&
			   MathF.Abs(roll) <= tolerance;
	}

	public readonly Rotation ToRotation() => Rotation.From(this);
	public readonly Vector3 AsVector3() => new(pitch, yaw, roll);

	public readonly Angles Clamped() => new( ClampAngle(pitch), ClampAngle(yaw), ClampAngle(roll));
	public static float ClampAngle(float v) {
		v %= 360.0f;
		return (v < 0.0f) ? v + 360.0f : v;
	}
	public static float NormalizeAngle(float v) {
		v = ClampAngle(v);
		return (v > 180.0f) ? v - 360.0f : v;
	}

	public readonly Angles LerpTo(Angles target, float frac) => Lerp(this, target, frac);
	public static Angles Lerp(in Angles source, in Angles target, float frac) => source + (target - source).Normal * frac;

	public static Vector3 AngleVector(Angles ang) {
		const float piOver180 = (float)(Math.PI / 180.0);
		var vSines = new float[2];
		vSines[0] = MathF.Sin(ang.yaw * piOver180);
		vSines[1] = MathF.Sin(ang.pitch * piOver180);
		var vCosines = new float[2];
		vCosines[0] = MathF.Cos(ang.yaw * piOver180);
		vCosines[1] = MathF.Cos(ang.pitch * piOver180);
		return new(vCosines[1] * vCosines[0], vCosines[1] * vSines[0], -vSines[1]);
	}
	public readonly Angles SnapToGrid(float gridSize, bool sx = true, bool sy = true, bool sz = true) {
		return new(sx ? pitch.SnapToGrid(gridSize) : pitch, sy ? yaw.SnapToGrid(gridSize) : yaw, sz ? roll.SnapToGrid(gridSize) : roll);
	}
}