public partial struct Rotation {
	public static Rotation SmoothDamp(Rotation current, in Rotation target, ref Vector3 velocity, float smoothTime, float deltaTime) {
		// If smoothing time is zero, directly jump to target (independent of timestep)
		if (smoothTime <= 0.0f)
			return target;

		// If timestep is zero, stay at current position
		if (deltaTime <= 0.0f)
			return current;

		// Implicit integration of critically damped spring
		if (Quaternion.Dot(current._quat, target._quat) < 0.0f)
			current = new Rotation( -current.x, -current.y, -current.z, -current.w );
		var delta = Quaternion.Multiply(target._quat - current._quat, 2.0f) * Quaternion.Conjugate(current._quat);
		var omega = MathF.PI * 2.0f / smoothTime;
		var v = new Vector3(delta.X, delta.Y, delta.Z);
		velocity = (velocity + omega * omega * deltaTime * v) / ((1.0f + omega * deltaTime) * (1.0f + omega * deltaTime));

		return (Exp(velocity * deltaTime) * current).Normal;
	}
}