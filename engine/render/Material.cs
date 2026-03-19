using Silk.NET.OpenGL;

public class Material {
	public class Resource : Config { //TODO this should probably be part of the main class
		public string Vertex {get; set;} = "shaders/vs_model.glsl";
		public string Depth {get; set;} = "shaders/ds_opaque.glsl";
		public string Fragment {get; set;}
		public Dictionary<string, Texture> Textures {get; set;} = [];
		public Material GetMaterial() {
			var m = From(Vertex, Fragment, Depth);
			if (m is null)
				return null;
			foreach (var texture in Textures)
				m.Set(texture.Key, texture.Value);
			return m;
		}
	}

	public Guid Id = Guid.NewGuid();
	private uint DepthHandle;
	private uint ColorHandle;
	public Shader Vertex;
	public Shader Color;
	public Shader Depth;
	public uint Handle => Graphics.Stage == Graphics.RenderStage.Depth ? DepthHandle : ColorHandle;
	private readonly Dictionary<string,object> Attributes = [];
	
	public void Set(string property, float value) => Set(property, (object)value);
	public void Set(string property, Vector2 value) => Set(property, (object)value);
	public void Set(string property, Texture value) => Set(property, (object)value);
	public void Set(string property, Matrix4x4 value) => Set(property, (object)value);
	private void Set(string property, object value) {
		Attributes[property] = value;
		if (Graphics.Stage == Graphics.RenderStage.Idle || Graphics.Stage == Graphics.RenderStage.Submit)
			return;
		SetUniform(Handle, property, value);
	}
	public void Bind() {
		if (Active != Handle)
			Graphics.Instance.UseProgram(Handle);
		Active = Handle;
		foreach (var attribute in Attributes)
			SetUniform(Handle, attribute.Key, attribute.Value);
		Scene.Active.MainCamera.Update(this);
	}
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
		if (value is Texture tval) {
			Graphics.Instance.BindTextureUnit((uint)location, tval.Handle);
			Graphics.Instance.Uniform1(location, location);
			return;
		}
		if (value is Matrix4x4 m3val) {
			Graphics.Instance.UniformMatrix4(location, 1, false, (float*)&m3val);
			return;
		}
	}
	private void Link() {
		if (ColorHandle == 0)
			ColorHandle = Graphics.Instance.CreateProgram();
		Link(ColorHandle, Vertex.Handle, Color.Handle);
		if (DepthHandle == 0)
			DepthHandle = Graphics.Instance.CreateProgram();
		Link(DepthHandle, Vertex.Handle, Depth.Handle);
	}

	private readonly static Dictionary<int,Material> Resident = [];
	private readonly static Dictionary<uint,Dictionary<string, int>> ProgState = [];
	private static uint Active {get; set;}
	public static Material From(string file) => From(Resource.Load<Resource>(file));
	public static Material From(Resource r) => r?.GetMaterial() ?? null;
	public static Material From(string vert, string frag, string depth) {
		if (vert is null || frag is null || depth is null)
			return null;
		var v = Resource.Load<Shader.Vertex>(vert);
		var f = Resource.Load<Shader.Fragment>(frag);
		var d = Resource.Load<Shader.Fragment>(depth);
		return From(v, f, d);
	}
	public static Material From(Shader vert, Shader color, Shader depth) {
		if (vert is null || color is null || depth is null)
			return null;
		var hc = HashCode.Combine(vert, color, depth);
		if (Resident.TryGetValue(hc, out var m)) {
			return new() {
				Vertex = m.Vertex,
				Color = m.Color,
				Depth = m.Depth,
				DepthHandle = m.DepthHandle,
				ColorHandle = m.ColorHandle
			};
		}
		m = new() {
			Vertex = vert,
			Color = color,
			Depth = depth
		};
		m.Link();
		Resident.Add(hc, m);
		return m;
	}
	private static void Link(uint handle, uint vert, uint frag) {
		Graphics.Instance.AttachShader(handle, vert);
		Graphics.Instance.AttachShader(handle, frag);
		Graphics.Instance.LinkProgram(handle);
		Graphics.Instance.GetProgram(handle, ProgramPropertyARB.LinkStatus, out var status);
		if (status != (int)GLEnum.True)
			Log.Exception($"program failed to link {Graphics.Instance.GetProgramInfoLog(handle)}");
		Graphics.Instance.DetachShader(handle, vert);
		Graphics.Instance.DetachShader(handle, frag);
	}
	public static void Flush() {
		ProgState.Clear();
		Active = 0;
		foreach (var m in Resident.Values)
			m.Link();
	}
}