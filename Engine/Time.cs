public static class Time {
	public static float Now => (float)_now;
	public static float RealNow => (float)(_now + (Timer?.Elapsed.TotalSeconds ?? 0));
	public static float Delta => (float)_delta;

	private static double _now;
	private static double _delta;

	private static System.Diagnostics.Stopwatch Timer;
	public static void Update() {
		Timer ??= System.Diagnostics.Stopwatch.StartNew();
		_delta = Timer.Elapsed.TotalSeconds;
		_now += _delta;
		Timer.Restart();
	}
}