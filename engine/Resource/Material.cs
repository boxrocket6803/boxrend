namespace Resource;

using Silk.NET.OpenGL;

public class Material : Config.Base {
	private struct Program {
		public Attributes Attributes {get; set;}
		public uint Handle {get; set;}
	}
	private static Dictionary<int, Program> Programs {get; set;} = [];

	public readonly Guid Id = Guid.NewGuid();
	public Shader.Vertex Vertex {get; set;}
	public Shader.Fragment Fragment {get; set;}
	public Shader.Fragment Depth {get; set;}

	public Attributes Attributes {get;} = new();

	public void Bind(Attributes a, bool depth = false) {
		var h = HashCode.Combine(Vertex.Handle, depth ? Depth.Handle : Fragment.Handle);
		if (!Programs.TryGetValue(h, out var program)) {
			program.Attributes = new();
			program.Handle = Graphics.Instance.CreateProgram();
			Programs[h] = program;
			//link program
			Graphics.Instance.AttachShader(program.Handle, Vertex.Handle);
			Graphics.Instance.AttachShader(program.Handle, depth ? Depth.Handle : Fragment.Handle);
			Graphics.Instance.LinkProgram(program.Handle);
			Graphics.Instance.GetProgram(program.Handle, ProgramPropertyARB.LinkStatus, out var status);
			if (status != (int)GLEnum.True) Log.Exception($"program failed to link {Graphics.Instance.GetProgramInfoLog(program.Handle)}");
			Graphics.Instance.DetachShader(program.Handle, Vertex.Handle);
			Graphics.Instance.DetachShader(program.Handle, depth ? Depth.Handle : Fragment.Handle);
		}
		Attributes.Clear();
		Attributes.Combine(Scene.Manager.Active.MainCamera.Attributes);
		Attributes.Combine(Attributes);
		Attributes.Combine(a);
		Attributes.Bind(program.Handle);
		Graphics.Instance.UseProgram(program.Handle);
	}

	public static Material From(string v, string f, string d) => new() {
		Vertex = Load<Shader.Vertex>(v),
		Fragment = Load<Shader.Fragment>(f),
		Depth = Load<Shader.Fragment>(d)
	};

	public override bool Load(string path) {
		if (!base.Load(path))
			return false;
		//TODO load textures from bmat
		return true;
	}
}