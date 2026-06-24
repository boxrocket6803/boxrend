using System.Globalization;

public partial struct Rotation {
	public override readonly string ToString() => $"{x:0.#####},{y:0.#####},{z:0.#####},{w:0.#####}";

	public static Rotation Parse(string str) {
		if (TryParse(str, CultureInfo.InvariantCulture, out var res))
			return res;
		return default;
	}
	public static Rotation Parse(string str, IFormatProvider _) => Parse(str);
	public static bool TryParse(string str, out Rotation result) => TryParse(str, CultureInfo.InvariantCulture, out result);
	public static bool TryParse(string str, IFormatProvider provider, out Rotation result) {
		result = Identity;
		if (string.IsNullOrWhiteSpace(str))
			return false;
		str = str.Trim('[', ']', ' ', '\n', '\r', '\t', '"');
		var components = str.Split([' ', ',', ';', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries);

		if (components.Length == 4) {
			// Try to parse as a Rotation (x, y, z, w)
			if ( float.TryParse( components[0], NumberStyles.Float, provider, out float x ) &&
				float.TryParse( components[1], NumberStyles.Float, provider, out float y ) &&
				float.TryParse( components[2], NumberStyles.Float, provider, out float z ) &&
				float.TryParse( components[3], NumberStyles.Float, provider, out float w ) )
			{
				result = new Rotation(x, y, z, w);
				return true;
			}
		} else if (components.Length == 3) {
			// Try to parse as Euler angles (pitch, yaw, roll)
			if ( float.TryParse( components[0], NumberStyles.Float, provider, out float pitch ) &&
				float.TryParse( components[1], NumberStyles.Float, provider, out float yaw ) &&
				float.TryParse( components[2], NumberStyles.Float, provider, out float roll ) )
			{
				var angles = new Angles(pitch, yaw, roll);
				result = angles.ToRotation();
				return true;
			}
		}

		return false;
	}
}