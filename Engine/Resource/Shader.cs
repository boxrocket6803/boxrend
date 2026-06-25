namespace Resource;

using Silk.NET.OpenGL;
using System.Diagnostics;

public abstract class Shader {
	public class Vertex : Shader<Vertex> {public override ShaderType Type => ShaderType.VertexShader;}
	public class Fragment : Shader<Fragment> {public override ShaderType Type => ShaderType.FragmentShader;}
}

public abstract class Shader<T> : Base<T> where T : Base, new() {
	public virtual ShaderType Type {get;}
	public uint Handle;

	public override bool Reload(string path) {
		if (string.IsNullOrEmpty(path))
			return false;
		var timer = Stopwatch.StartNew();
		if (Handle == 0)
			Handle = Graphics.Manager.Instance.CreateShader(Type);
		var glsl = Assets.ReadText(path);
		if (glsl is null) {
			Log.Error($"couldn't read file {path}");
			return false;
		}
		Precompile(path, ref glsl);
		Graphics.Manager.Instance.ShaderSource(Handle, glsl);
		Graphics.Manager.Instance.CompileShader(Handle);
		Graphics.Manager.Instance.GetShader(Handle, GLEnum.CompileStatus, out var status);
		if (status != (int)GLEnum.True)
			Log.Error($"{path} failed to compile: \n{Graphics.Manager.Instance.GetShaderInfoLog(Handle)}");
		else
			Log.Info($"{path} compile in {Math.Round(timer.Elapsed.TotalSeconds * 1000, 2)}ms");
		return true;
	}

	private static void Precompile(string path, ref string glsl) { //TODO combos
		foreach (var line in glsl.Split('\n')) {
			if (line.Trim().StartsWith("#include")) {
				var file = line.Trim()["#include".Length..].Trim().Trim('"');
				var rep = Assets.ReadText(file);
				if (rep is null)
					Log.Error($"couldn't find file {file} for #include directive in {path}");
				else
					Precompile(file, ref rep);
				glsl = glsl.Replace(line, rep);
				continue;
			}
		}
	}
}