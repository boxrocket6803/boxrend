using Silk.NET.OpenGL;

public class Shader {
	public int Hash;
	public uint Handle;

	public void Dispose() {
		Graphics.Instance.DeleteShader(Handle);
		Resident.Remove(Hash);
	}

	private readonly static Dictionary<int,Shader> Resident = [];
	public static Shader Get(string path, ShaderType type) {
		var hash = HashCode.Combine(path.ToLower(), type);
		if (Resident.TryGetValue(hash, out var es))
			return es;
		Log.Info($"compiling {path}");
		var s = Graphics.Instance.CreateShader(type);
		var glsl = ResourceSystem.ReadText(path);
		if (glsl == null)
			return new Shader();
		Graphics.Instance.ShaderSource(s, glsl);
		Graphics.Instance.CompileShader(s);
		Graphics.Instance.GetShader(s, GLEnum.CompileStatus, out var status);
		if (status != (int)GLEnum.True)
			Log.Exception($"{path} failed to compile: {Graphics.Instance.GetShaderInfoLog(s)}");
		var ns = new Shader {
			Hash = hash,
			Handle = s
		};
		Resident[hash] = ns;
		return ns;
	}
	public static void FlushAll() {
		foreach (var shader in Resident.Values)
			shader.Dispose();
		Resident.Clear();
	}
}