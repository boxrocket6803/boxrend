using System.Runtime.CompilerServices;

public static class MathX {
	internal const float toRadians = MathF.PI * 2f / 360F;
	internal const float toDegrees = 1.0f / toRadians;
	internal const float toGradiansDegrees = 0.9f;
	internal const float toGradiansRadians = 0.01570796326f;

	public static float DegreeToRadian(this float deg) => deg * toRadians;
	public static float RadianToDegree(this float rad) => rad * toDegrees;

	public static float GradiansToDegrees(this float grad) => grad * toGradiansDegrees;
	public static float GradiansToRadians(this float grad) => grad * toGradiansRadians;

	internal const float toMeters = 0.0254f;
	internal const float toInches = 1.0f / toMeters;
	internal const float toMillimeters = 25.4f;

	public static float MeterToInch(this float meters) => meters * toInches;
	public static float InchToMeter(this float inches) => inches * toMeters;

	public static float InchToMillimeter(this float inches) => inches * toMillimeters;
	public static float MillimeterToInch(this float millimeters) => millimeters * (1.0f / toMillimeters);

	public static float SnapToGrid(this float f, float gridSize) {
		if (gridSize.AlmostEqual(0))
			return f;
		var snapped = MathF.Round(f / gridSize) * gridSize;
		return snapped == 0.0f ? 0.0f : snapped;
	}
	public static int SnapToGrid(this int f, int gridSize) => (f / gridSize) * gridSize;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int FloorToInt(this float f) => (int)MathF.Floor(f);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Floor(this float f) => MathF.Floor(f);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int CeilToInt(this float f) => (int)MathF.Ceiling(f);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Ceil(this float f) => MathF.Ceiling(f);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Clamp(this float v, float min, float max) {
		if (min > max)
			(max, min) = (min, max);
		return v < min ? min : v < max ? v : max;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Clamp(this double v, double min, double max) {
		if (min > max)
			(max, min) = (min, max);
		return v < min ? min : v < max ? v : max;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float LerpTo(this float from, float to, float frac, bool clamp = true) {
		if (clamp)
			frac = frac.Clamp(0, 1);
		return from + frac * (to - from);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float[] LerpTo(this float[] from, float[] to, float delta, bool clamp = true) {
		if (from == null)
			return null;
		if (to == null)
			return from;
		float[] output = new float[Math.Min(from.Length, to.Length)];
		for (int i = 0; i < output.Length; i++)
			output[i] = from[i].LerpTo(to[i], delta, clamp);
		return output;
	}

	public static float LerpDegreesTo(this float from, float to, float frac, bool clamp = true) {
		var delta = DeltaDegrees(from, to);
		var lerped = from.LerpTo(from + delta, frac, clamp).UnsignedMod(360f);
		return lerped >= 180f ? lerped - 360f : lerped;
	}

	public static float LerpRadiansTo(this float from, float to, float frac, bool clamp = true) {
		var delta = DeltaRadians(from, to);
		var lerped = from.LerpTo(from + delta, frac, clamp).UnsignedMod(MathF.Tau);
		return lerped >= MathF.PI ? lerped - MathF.Tau : lerped;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float LerpInverse(this float value, float from, float to, bool clamp = true) {
		if (clamp)
			value = value.Clamp(from, to);
		value -= from;
		to -= from;
		if (to == 0)
			return 0;
		return value / to;
	}

	public static float Approach(this float f, float target, float delta) {
		if (f > target) {
			f -= delta;
			if (f < target)
				return target;
		} else {
			f += delta;
			if (f > target)
				return target;
		}
		return f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool AlmostEqual(this float value, float b, float within = 0.0001f) => MathF.Abs( value - b ) <= within;

	public static float UnsignedMod(this float a, float b) => a - b * (a / b).Floor();

	public static float NormalizeDegrees(this float degree) {
		degree %= 360;
		if (degree < 0)
			degree += 360;
		return degree;
	}

	public static float DeltaDegrees(float from, float to) {
		var delta = (to - from).UnsignedMod(360f);
		return delta >= 180f ? delta - 360f : delta;
	}
	public static float DeltaRadians(float from, float to) {
		var delta = (to - from).UnsignedMod(MathF.Tau);
		return delta >= MathF.PI ? delta - MathF.Tau : delta;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Remap(this float value, float oldLow, float oldHigh, float newLow = 0, float newHigh = 1) => Remap(value, oldLow, oldHigh, newLow, newHigh, true);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Remap(this float value, float oldLow, float oldHigh, float newLow, float newHigh, bool clamp) {
		if (MathF.Abs(oldHigh - oldLow) < 0.0001f)
			return clamp ? newLow : value;
		var v = newLow + (value - oldLow) * (newHigh - newLow) / (oldHigh - oldLow);
		if (clamp)
			v = v.Clamp(newLow, newHigh);
		return v;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Remap(this double value, double oldLow, double oldHigh, double newLow = 0, double newHigh = 1) => Remap(value, oldLow, oldHigh, newLow, newHigh, true);
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static double Remap(this double value, double oldLow, double oldHigh, double newLow, double newHigh, bool clamp) {
		if (Math.Abs(oldHigh - oldLow) < 0.0001)
			return clamp ? newLow : value;
		var v = newLow + (value - oldLow) * (newHigh - newLow) / (oldHigh - oldLow);
		if (clamp)
			v = v.Clamp(newLow, newHigh);
		return v;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static int Remap(this int value, int oldLow, int oldHigh, int newLow, int newHigh) => (int)Remap(value, oldLow, oldHigh, newLow, newHigh, true);

	/// <summary>
	/// Given a sphere and a field of view, how far from the camera should we be to fully see the sphere?
	/// </summary>
	public static float SphereCameraDistance(float radius, float fieldOfView) {
		if ( radius < 0.001f )
			return 0.01f;
		if ( fieldOfView <= 0.01f )
			return 0.01f;
		return radius / MathF.Abs(MathF.Sin(fieldOfView.DegreeToRadian() * 0.5f));
	}

	/// <summary>
	/// Smoothly approach the target value using exponential decay.
	/// Cheaper than SmoothDamp but doesn't track velocity for momentum.
	/// Good for non-physical smoothing.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ExponentialDecay(float current, float target, float halflife, float deltaTime) => target + (current - target) * MathF.Exp(-0.69314718f / halflife * deltaTime);

	public static float SmoothDamp(float current, float target, ref float velocity, float smoothTime, float deltaTime) {
		var displacement = current - target;
		(displacement, velocity) = SpringDamper.FromSmoothingTime(smoothTime).Simulate( displacement, velocity, deltaTime );
		return displacement + target;
	}
	public static float SpringDamp(float current, float target, ref float velocity, float deltaTime, float frequency = 2.0f, float damping = 0.5f) {
		var displacement = current - target;
		(displacement, velocity) = SpringDamper.FromDamping(frequency, damping).Simulate( displacement, velocity, deltaTime );
		return displacement + target;
	}

	//TODO maybe move this to seperate class
	public static float ToFloat(this string str, float Default = 0) => (float)str.ToDecimal((decimal)Default);
	public static decimal ToDecimal(this string str, decimal Default = 0) {
		decimal res = Default;
		if (!decimal.TryParse(str, out res))
			return default;
		return res;
	}
}