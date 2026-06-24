using System.Diagnostics.CodeAnalysis;
using System.Globalization;

public partial struct Angles {
	public readonly override string ToString() => $"Pitch = {pitch:0.00}, Yaw = {yaw:0.00}, Roll = {roll:0.00}";

	public static Angles Parse(string str) {
		if (TryParse(str, CultureInfo.InvariantCulture, out var res))
			return res;
		return default;
	}
	public static Angles Parse(string str, IFormatProvider _) => Parse(str);
	public static bool TryParse(string str, out Angles result) => TryParse(str, CultureInfo.InvariantCulture, out result);
	public static bool TryParse(string str, IFormatProvider provider, out Angles result) {
		result = Zero;
		if (string.IsNullOrWhiteSpace(str))
			return false;
		str = str.Trim('[', ']', ' ', '\n', '\r', '\t', '"');
		var components = str.Split([' ', ',', ';', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
		if (components.Length != 3)
			return false;
		if (!float.TryParse(components[0], NumberStyles.Float, provider, out float p)) return false;
		if (!float.TryParse(components[1], NumberStyles.Float, provider, out float y)) return false;
		if (!float.TryParse(components[2], NumberStyles.Float, provider, out float r)) return false;
		result = new(p, y, r);
		return true;
	}
}