public readonly struct SpringDamper {
	public float Frequency {get;}
	public float DecayRate {get;}
	private readonly float _omega;

	public SpringDamper(float frequency, float decayRate) {
		Frequency = frequency;
		DecayRate = decayRate;
		var omega0 = Frequency * MathF.PI * 2f;
		_omega = MathF.Sqrt(Math.Max(0f, omega0 * omega0 - DecayRate * DecayRate));
	}

	public static SpringDamper FromDamping(float frequency = 2f, float damping = 0.5f) => new(frequency, damping * frequency * MathF.PI * 2f);
	public static SpringDamper FromSmoothingTime(float smoothingTime) => smoothingTime <= 0f ? FromDamping(1f, float.PositiveInfinity) : FromDamping(1f / smoothingTime, 1f);

	public (float Position, float Velocity) Simulate(float position, float velocity, float deltaTime) {
		if (deltaTime <= 0.0f)
			return (position, velocity);
		if (float.IsPositiveInfinity(DecayRate))
			return (0f, 0f);

		// Correct for what our velocity would be without damping, so we can extrapolate the oscillation
		var velocityWithoutDecay = velocity + DecayRate * position;

		// Simulate spring without decay
		(position, velocityWithoutDecay) = SimulateOscillator( position, velocityWithoutDecay, deltaTime );

		// Apply exponential decay
		(position, velocity) = SimulateDecay( position, velocityWithoutDecay, deltaTime );

		return (position, velocity);
	}

	public (Vector2 Position, Vector2 Velocity) Simulate(Vector2 position, Vector2 velocity, float deltaTime) {
		(position.x, velocity.x) = Simulate(position.x, velocity.x, deltaTime);
		(position.y, velocity.y) = Simulate(position.y, velocity.y, deltaTime);
		return (position, velocity);
	}

	public (Vector3 Position, Vector3 Velocity) Simulate(Vector3 position, Vector3 velocity, float deltaTime) {
		(position.x, velocity.x) = Simulate(position.x, velocity.x, deltaTime);
		(position.y, velocity.y) = Simulate(position.y, velocity.y, deltaTime);
		(position.z, velocity.z) = Simulate(position.z, velocity.z, deltaTime);
		return (position, velocity);
	}

	private (float MaxPosition, float MaxVelocity, float Phase) FindOscillationParameters(float position, float velocity) {
		// Total energy (kinetic + potential) x 2, assuming unit mass
		var energy2 = velocity * velocity + _omega * _omega * position * position;

		// Snap to 0 if energy is super low, so we don't wobble forever
		if (energy2 <= 0.001f)
			return (0f, 0f, 0f);

		// Find maximum velocity by turning all energy into kinetic
		var vMax = MathF.Sqrt(energy2);

		// Find maximum amplitude by turning all energy into potential
		var amplitude = vMax / _omega;

		// Where are we in the oscillation
		var phase = MathF.Atan2(-velocity, position * _omega);

		return (amplitude, vMax, phase);
	}

	private (float Position, float Velocity) SimulateOscillator(float position, float velocity, float deltaTime) {
		if (_omega <= 0.0001f) // We're not oscillating, just moving at constant velocity
			return (position + velocity * deltaTime, velocity);

		// Work out where we are in the oscillation (the phase), and what its amplitude / max velocity is
		var (xMax, vMax, phase) = FindOscillationParameters(position, velocity);
		if (xMax <= 0f) // Fast path if we're at equilibrium
			return (0f, 0f);

		// Project it into the future
		return (MathF.Cos( deltaTime * _omega + phase ) * xMax, -MathF.Sin( deltaTime * _omega + phase ) * vMax);
	}

	private (float Position, float Velocity) SimulateDecay(float position, float velocity, float deltaTime) {
		var scale = float.IsPositiveInfinity(DecayRate) ? 0f : MathF.Exp(-deltaTime * DecayRate);

		// Apply exponential decay
		position *= scale;
		velocity *= scale;

		// Apply gradient of exponential decay
		velocity -= DecayRate * position;

		return (position, velocity);
	}
}