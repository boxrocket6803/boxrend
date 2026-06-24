public partial struct Rotation {
	public static Rotation FromAxis(Vector3 axis, float degrees) => Quaternion.CreateFromAxisAngle(axis, degrees.DegreeToRadian());
	public static Rotation From(Angles angles) => From(angles.pitch, angles.yaw, angles.roll);
	public static Rotation FromPitch(float pitch) => From(pitch, 0, 0);
	public static Rotation FromYaw(float yaw) => From(0, yaw, 0);
	public static Rotation FromRoll(float roll) => From(0, 0, roll);
	public static Rotation From(float pitch, float yaw, float roll) {
		Rotation rot = default;

		pitch = pitch.DegreeToRadian() * 0.5f;
		yaw = yaw.DegreeToRadian() * 0.5f;
		roll = roll.DegreeToRadian() * 0.5f;

		float sp = MathF.Sin(pitch);
		float cp = MathF.Cos(pitch);

		float sy = MathF.Sin(yaw);
		float cy = MathF.Cos(yaw);

		float sr = MathF.Sin(roll);
		float cr = MathF.Cos(roll);

		float srXcp = sr * cp, crXsp = cr * sp;
		rot.x = srXcp * cy - crXsp * sy; //x
		rot.y = crXsp * cy + srXcp * sy; //y

		float crXcp = cr * cp, srXsp = sr * sp;
		rot.z = crXcp * sy - srXsp * cy; //z
		rot.w = crXcp * cy + srXsp * sy; //w

		return rot;
	}

	public static Rotation LookAt(Vector3 forward) {
		if (forward.WithZ(0f).IsNearZeroLength)
			return LookAt(forward, Vector3.Left);
		return LookAt(forward, Vector3.Up);
	}
	public static Rotation LookAt(Vector3 forward, Vector3 up) {
		forward = forward.Normal;
		up = up.Normal;

		float flRatio = forward.Dot(up);

		up = (up - (forward * flRatio)).Normal;
		var right = forward.Cross(up).Normal;

		var vX = forward;
		var vY = -right;
		var vZ = up;

		float flTrace = vX.x + vY.y + vZ.z;

		Quaternion q;
		if (flTrace >= 0.0f) {
			q.X = vY.z - vZ.y;
			q.Y = vZ.x - vX.z;
			q.Z = vX.y - vY.x;
			q.W = flTrace + 1.0f;
		} else {
			if (vX.x > vY.y && vX.x > vZ.z) {
				q.X = vX.x - vY.y - vZ.z + 1.0f;
				q.Y = vY.x + vX.y;
				q.Z = vZ.x + vX.z;
				q.W = vY.z - vZ.y;
			} else if (vY.y > vZ.z) {
				q.X = vX.y + vY.x;
				q.Y = vY.y - vZ.z - vX.x + 1.0f;
				q.Z = vZ.y + vY.z;
				q.W = vZ.x - vX.z;
			} else {
				q.X = vX.z + vZ.x;
				q.Y = vY.z + vZ.y;
				q.Z = vZ.z - vX.x - vY.y + 1.0f;
				q.W = vX.y - vY.x;
			}
		}
		return Quaternion.Normalize(q);
	}

	public static Rotation FromToRotation(in Vector3 fromDirection, in Vector3 toDirection) {
		Vector3 axis = Vector3.Cross(fromDirection, toDirection);
		float angle = Vector3.GetAngle(fromDirection, toDirection);
		return FromAxis(axis.Normal, angle); 
	}
}