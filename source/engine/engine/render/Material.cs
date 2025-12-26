using Silk.NET.OpenGL;

public class Material {
	public Guid Id = Guid.NewGuid();
	public uint Handle;
	private readonly Dictionary<string,int> State = [];
	private readonly Dictionary<string,object> Attributes = [];
	
	//TODO we should also be able to load these from files, json makes sense i think
	public void Set(string property, float value) => Attributes[property] = value;
	public void Set(string property, Vector2 value) => Attributes[property] = value;
	public void Set(string property, Texture value) => Attributes[property] = value;
	public void Bind() {
		if (Active != this)
			Graphics.Instance.UseProgram(Handle);
		Active = this;
		foreach (var attribute in Attributes) {
			var hc = attribute.Value.GetHashCode();
			if (State.GetValueOrDefault(attribute.Key) == hc)
				continue;
			State[attribute.Key] = hc;
			var location = Graphics.Instance.GetUniformLocation(Handle, attribute.Key);
			if (attribute.Value is float flval) {
				Graphics.Instance.Uniform1(location, flval);
				continue;
			}
			if (attribute.Value is Vector2 v2val) {
				Graphics.Instance.Uniform2(location, v2val);
				continue;
			}
			if (attribute.Value is Texture tval) {
				Graphics.Instance.BindTextureUnit((uint)location, tval.Handle);
				Graphics.Instance.Uniform1(location, location);
				continue;
			}
		}
	}

	private readonly static Dictionary<int,Material> Resident = [];
	private static Material Active {get; set;}
	public static Material From(string vert, string frag) {
		var v = Shader.Get(vert, ShaderType.VertexShader);
		var f = Shader.Get(frag, ShaderType.FragmentShader);
		return From(v, f);
	}
	public static Material From(Shader vert, Shader frag) {
		var hc = HashCode.Combine(vert.Hash, frag.Hash);
		if (Resident.TryGetValue(hc, out var ep))
			return new() {Handle = ep.Handle};
		var p = new Material {Handle = Graphics.Instance.CreateProgram()};
		Graphics.Instance.AttachShader(p.Handle, vert.Handle);
		Graphics.Instance.AttachShader(p.Handle, frag.Handle);
		Graphics.Instance.LinkProgram(p.Handle);
		Graphics.Instance.GetProgram(p.Handle, ProgramPropertyARB.LinkStatus, out var status);
		if (status != (int)GLEnum.True)
			Log.Exception($"program failed to link {Graphics.Instance.GetProgramInfoLog(p.Handle)}");
		Graphics.Instance.DetachShader(p.Handle, vert.Handle);
		Graphics.Instance.DetachShader(p.Handle, frag.Handle);
		Resident.Add(hc, p);
		return p;
	}
	public static void FlushAll() { //TODO should be per shader, which is of course really annoying
		HashSet<uint> handles = [];
		foreach (var program in Resident.Values) {
			program.State.Clear();
			handles.Add(program.Handle);
		}
		foreach (var handle in handles)
			Graphics.Instance.DeleteProgram(handle);
		Resident.Clear();
	}
}