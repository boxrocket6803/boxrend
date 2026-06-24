public partial struct Vector3 {
	public record struct SmoothDamped {
		public Vector3 Current;
		public Vector3 Target;
		public float SmoothTime;
		public Vector3 Velocity;

		public SmoothDamped(Vector3 current, Vector3 target, float smoothTime = 0.5f) {
			Current = current;
			Target = target;
			SmoothTime = smoothTime;
		}

		public void Update(float timeDelta) => Current = SmoothDamp(Current, Target, ref Velocity, SmoothTime, timeDelta);
	}

	public record struct SpringDamped {
		public Vector3 Current;
		public Vector3 Target;
		public float Frequency;
		public float Damping;
		public Vector3 Velocity;

		public SpringDamped(Vector3 current, Vector3 target, float frequency = 2.0f, float damping = 0.5f) {
			Current = current;
			Target = target;
			Frequency = frequency;
			Damping = damping;
		}

		public void Update(float timeDelta) => Current = SpringDamp(Current, Target, ref Velocity, timeDelta, Frequency, Damping);
	}

	public static Vector3 SmoothDamp(in Vector3 current, in Vector3 target, ref Vector3 velocity, float smoothTime, float deltaTime) {
		// If smoothing time is zero, directly jump to target (independent of timestep)
		if (smoothTime <= 0.0f)
			return target;

		// If timestep is zero, stay at current position
		if (deltaTime <= 0.0f)
			return current;

		// Implicit integration of critically damped spring
		var omega = MathF.Tau / smoothTime;
		var denom = (1.0f + omega * deltaTime);
		velocity = (velocity - (omega * omega) * deltaTime * (current - target)) / (denom * denom);
		return current + velocity * deltaTime;
	}

	public static Vector3 SpringDamp(in Vector3 current, in Vector3 target, ref Vector3 velocity, float deltaTime, float frequency = 2.0f, float damping = 0.5f) {
		var displacement = current - target;
		(displacement, velocity) = SpringDamper.FromDamping(frequency, damping).Simulate(displacement, velocity, deltaTime);
		return displacement + target;
	}
}