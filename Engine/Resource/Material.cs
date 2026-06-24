namespace Resource;

using Silk.NET.OpenGL;

public class Material : Config.Base<Material> {
	private struct Program {
		public Graphics.Attributes Attributes {get; set;}
		public uint Handle {get; set;}
	}
	private static Dictionary<int, Program> Programs {get; set;} = [];

	public readonly Guid Id = Guid.NewGuid();
	public Shader.Vertex Vertex {get; set;}
	public Shader.Fragment Fragment {get; set;}
	public Shader.Fragment Depth {get; set;}

	public Graphics.Attributes Attributes {get;} = new();

	public void Bind(Graphics.Attributes a, bool depth = false) {
		var h = HashCode.Combine(Vertex.Handle, depth ? Depth.Handle : Fragment.Handle);
		if (!Programs.TryGetValue(h, out var program)) {
			program.Attributes = new();
			program.Handle = Graphics.Manager.Instance.CreateProgram();
			Graphics.Manager.Instance.AttachShader(program.Handle, Vertex.Handle);
			Graphics.Manager.Instance.AttachShader(program.Handle, depth ? Depth.Handle : Fragment.Handle);
			Graphics.Manager.Instance.LinkProgram(program.Handle);
			Graphics.Manager.Instance.GetProgram(program.Handle, ProgramPropertyARB.LinkStatus, out var status);
			if (status != (int)GLEnum.True) Log.Exception($"program failed to link {Graphics.Manager.Instance.GetProgramInfoLog(program.Handle)}");
			Graphics.Manager.Instance.DetachShader(program.Handle, Vertex.Handle);
			Graphics.Manager.Instance.DetachShader(program.Handle, depth ? Depth.Handle : Fragment.Handle);
			Programs[h] = program;
		}
		Attributes.Clear();
		Attributes.Combine(Scene.Manager.Active.MainCamera.Attributes);
		Attributes.Combine(Attributes);
		Attributes.Combine(a);
		Attributes.Bind(program.Handle);
	}

	public static Material From(string v, string f, string d) => new() {
		Vertex = Shader.Vertex.Load(v),
		Fragment = Shader.Fragment.Load(f),
		Depth = Shader.Fragment.Load(d)
	};

	public override bool Reload(string path) {
		if (!base.Reload(path))
			return false;
		//TODO load textures from bmat
		return true;
	}
}