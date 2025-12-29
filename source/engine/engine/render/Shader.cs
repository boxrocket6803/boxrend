using Silk.NET.OpenGL;
using System.Diagnostics;

public class Shader {
	public int Hash;
	public uint Handle;

	public void Dispose() {
		Graphics.Instance.DeleteShader(Handle);
		Resident.Remove(Hash);
	}

	private readonly static Dictionary<int,Shader> Resident = [];
	private static void Precompile(string filename, ref string glsl) {
		foreach (var line in glsl.Split('\n')) {
			if (line.Trim().StartsWith("#include")) {
				var file = line.Trim()["#include".Length..].Trim().Trim('"');
				var rep = Assets.ReadText(file);
				if (rep is null)
					Log.Error($"couldn't find file {file} for #include directive in {filename}");
				else
					Precompile(file, ref rep);
				glsl = glsl.Replace(line, rep);
				continue;
			}
		}
	}
	public static Shader Get(string path, ShaderType type) {
		if (string.IsNullOrEmpty(path))
			return null;
		var hash = HashCode.Combine(path.ToLower(), type);
		if (Resident.TryGetValue(hash, out var es))
			return es;
		var timer = Stopwatch.StartNew();
		var s = Graphics.Instance.CreateShader(type);
		var glsl = Assets.ReadText(path);
		if (glsl is null) {
			Log.Error($"couldn't read file {path}");
			return null;
		}
		Precompile(path, ref glsl);
		Graphics.Instance.ShaderSource(s, glsl);
		Graphics.Instance.CompileShader(s);
		Graphics.Instance.GetShader(s, GLEnum.CompileStatus, out var status);
		if (status != (int)GLEnum.True)
			Log.Exception($"{path} failed to compile: \n{Graphics.Instance.GetShaderInfoLog(s)}");
		else
			Log.Info($"{path} compile in {Math.Round(timer.Elapsed.TotalSeconds * 1000, 2)}ms");
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