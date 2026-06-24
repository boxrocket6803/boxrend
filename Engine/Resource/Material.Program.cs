namespace Resource;

using Silk.NET.OpenGL;

public partial class Material {
	private struct Program {
		public Graphics.Attributes Attributes {get; set;}
		public uint Handle {get; set;}
	}
	private static Dictionary<int, Program> Programs {get; set;} = [];

	private Program GetProgram(bool depth = false) {
		var h = HashCode.Combine(Vertex.Handle, depth ? Depth.Handle : Fragment.Handle);
		if (!Programs.TryGetValue(h, out var program)) {
			program.Attributes = new();
			program.Handle = Graphics.Manager.Instance.CreateProgram();
			Graphics.Manager.Instance.AttachShader(program.Handle, Vertex.Handle);
			Graphics.Manager.Instance.AttachShader(program.Handle, depth ? Depth.Handle : Fragment.Handle);
			Graphics.Manager.Instance.LinkProgram(program.Handle);
			Graphics.Manager.Instance.GetProgram(program.Handle, ProgramPropertyARB.LinkStatus, out var status);
			if (status != (int)GLEnum.True)
				Log.Exception($"program failed to link {Graphics.Manager.Instance.GetProgramInfoLog(program.Handle)}");
			Graphics.Manager.Instance.DetachShader(program.Handle, Vertex.Handle);
			Graphics.Manager.Instance.DetachShader(program.Handle, depth ? Depth.Handle : Fragment.Handle);
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