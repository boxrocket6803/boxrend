public class Attributes {
	private readonly Dictionary<string,object> Current = [];
	private static uint Active {get; set;}

	public void Set(string property, float value) => Current[property] = value;
	public void Set(string property, Vector2 value) => Current[property] = value;
	public void Set(string property, Resource.Texture value) => Current[property] = value.Handle;
	public void Set(string property, Matrix4x4 value) => Current[property] = value;
	
	public void Combine(Attributes a) {
		foreach (var attr in a.Current)
			Current[attr.Key] = attr.Value;
	}
	public void Clear() => Current.Clear(); //TODO do we need to clear programs too?

	public void Bind(uint handle) {
		if (Active != handle)
			Graphics.Instance.UseProgram(handle);
		Active = handle;
		foreach (var attribute in Current)
			SetUniform(handle, attribute.Key, attribute.Value);
	}
	public static void Flush() {
		ProgState.Clear();
		Active = 0;
	}

	//TODO custom hash

	private readonly static Dictionary<uint,Dictionary<string, int>> ProgState = [];
	private static unsafe void SetUniform(uint handle, string property, object value) {
		var hc = value.GetHashCode();
		if (!ProgState.TryGetValue(handle, out var state))
			state = ProgState[handle] = [];
		if (state.GetValueOrDefault(property) == hc)
			return;
		state[property] = hc;
		var location = Graphics.Instance.GetUniformLocation(handle, property);
		if (value is float flval) {
			Graphics.Instance.Uniform1(location, flval);
			return;
		}
		if (value is Vector2 v2val) {
			Graphics.Instance.Uniform2(location, v2val);
			return;
		}
		if (value is Resource.Texture tval) {
			Graphics.Instance.BindTextureUnit((uint)location, tval.Handle);
			Graphics.Instance.Uniform1(location, location);
			return;
		}
		if (value is Matrix4x4 m3val) {
			Graphics.Instance.UniformMatrix4(location, 1, false, (float*)&m3val);
			return;
		}
	}
}