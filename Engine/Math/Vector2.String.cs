using System.Diagnostics.CodeAnalysis;
using System.Globalization;

public partial struct Vector2  {
	public override readonly string ToString() => $"{x:0.###},{y:0.###}";

	public static Vector2 Parse(string str) {
		if (TryParse(str, CultureInfo.InvariantCulture, out var res))
			return res;
		return default;
	}
	public static bool TryParse(string str, out Vector2 result) => TryParse(str, CultureInfo.InvariantCulture, out result);
	public static Vector2 Parse(string str, IFormatProvider _) => Parse(str);
	public static bool TryParse(string str, IFormatProvider provider, out Vector2 result) {
		result = Zero;
		if (string.IsNullOrWhiteSpace(str))
			return false;
		str = str.Trim('[', ']', ' ', '\n', '\r', '\t', '"');
		var components = str.Split([' ', ',', ';', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
		if (components.Length != 2)
			return false;
		if (!float.TryParse(components[0], NumberStyles.Float, provider, out float x)) return false;
		if (!float.TryParse(components[1], NumberStyles.Float, provider, out float y)) return false;
		result = new(x, y);
		return true;
	}
}