namespace Graphics;

public partial class Attributes {
	private static uint Active {get; set;}
	
	public void Bind(uint handle) {
		if (Active != handle)
			Manager.Instance.UseProgram(handle);
		Active = handle;
		foreach (var attribute in Current)
			SetUniform(handle, attribute.Key, attribute.Value);
	}

	private readonly static Dictionary<uint,Dictionary<string, int>> ProgState = [];
	private static unsafe void SetUniform(uint handle, string property, object value) {
		var hc = value.GetHashCode();
		if (!ProgState.TryGetValue(handle, out var state))
			state = ProgState[handle] = [];
		if (state.GetValueOrDefault(property) == hc)
			return;
		state[property] = hc;
		var location = Manager.Instance.GetUniformLocation(handle, property);
		if (value is float flval) {
			Manager.Instance.Uniform1(location, flval);
			return;
		}
		if (value is Vector2 v2val) {
			Manager.Instance.Uniform2(location, v2val);
			return;
		}
		if (value is Resource.Texture tval) {
			Manager.Instance.BindTextureUnit((uint)location, tval.Handle);
			Manager.Instance.Uniform1(location, location);
			return;
		}
		if (value is Matrix4x4 m3val) {
			Manager.Instance.UniformMatrix4(location, 1, false, (float*)&m3val);
			return;
		}
	}

	public static void Flush() {
		ProgState.Clear();
		Active = 0;
	}
}