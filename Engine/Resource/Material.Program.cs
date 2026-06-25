namespace Resource;

using Silk.NET.OpenGL;

public partial class Material {
	private struct Program {
		public Graphics.Attributes Attributes {get; set;}
		public uint Handle {get; set;}
	}
	private static Dictionary<int, Program> Programs {get; set;} = [];

	private Program GetProgram(bool depth, bool shadow) {
		var v = Vertex.Handle(depth, shadow);
		var f = Fragment.Handle(depth, shadow);
		var h = HashCode.Combine(v, f);
		if (!Programs.TryGetValue(h, out var program)) {
			program.Attributes = new();
			program.Handle = Graphics.Manager.Instance.CreateProgram();
			Graphics.Manager.Instance.AttachShader(program.Handle, v);
			Graphics.Manager.Instance.AttachShader(program.Handle, f);
			Graphics.Manager.Instance.LinkProgram(program.Handle);
			Graphics.Manager.Instance.GetProgram(program.Handle, ProgramPropertyARB.LinkStatus, out var status);
			if (status != (int)GLEnum.True)
				Log.Exception($"program failed to link {Graphics.Manager.Instance.GetProgramInfoLog(program.Handle)}");
			Graphics.Manager.Instance.DetachShader(program.Handle, v);
			Graphics.Manager.Instance.DetachShader(program.Handle, f);
			Programs[h] = program;
		}
		return program;
	}

	public static void Flush() {
		foreach (var p in Programs.Values)
			Graphics.Manager.Instance.DeleteProgram(p.Handle);
		Programs.Clear();
	}
}