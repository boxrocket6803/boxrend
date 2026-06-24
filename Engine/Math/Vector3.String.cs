using System.Globalization;

public partial struct Vector3 {
	public readonly override string ToString() => $"{x:0.####},{y:0.####},{z:0.####}";

	public static Vector3 Parse(string str, IFormatProvider _) => Parse(str);
	public static Vector3 Parse(string str) {
		if (TryParse(str, CultureInfo.InvariantCulture, out var res))
			return res;
		return default;
	}
	public static bool TryParse(string str, out Vector3 result) => TryParse(str, CultureInfo.InvariantCulture, out result);
	public static bool TryParse(string str, IFormatProvider provider, out Vector3 result) {
		result = Zero;
		if (string.IsNullOrWhiteSpace(str))
			return false;
		str = str.Trim('[', ']', ' ', '\n', '\r', '\t', '"');
		var components = str.Split([' ', ',', ';', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
		if (components.Length != 3)
			return false;

		if (!float.TryParse(components[0], NumberStyles.Float, provider, out float x)) return false;
		if (!float.TryParse(components[1], NumberStyles.Float, provider, out float y)) return false;
		if (!float.TryParse(components[2], NumberStyles.Float, provider, out float z)) return false;
		result = new(x, y, z);
		return true;
	}
}