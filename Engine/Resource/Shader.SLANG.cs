using Silk.NET.OpenGL;
using System.Diagnostics;
using System.IO;

namespace Resource;

public partial class Shader<T> {
	private static string[] ProcessSlang(string path, ShaderType type) {
		//TODO split by shader type
		//TODO depth and shadow combos
		var glsl = SlangCompile(path, type);
		Log.Info(glsl);
		return [glsl];
	}

	private static string GetStageString(ShaderType type) {
		if (type == ShaderType.FragmentShader) return "Fragment";
		if (type == ShaderType.VertexShader) return "Vertex";
		if (type == ShaderType.ComputeShader) return "Compute";
		Log.Exception("unknown stage in Shader.HLSL.GetStageString()");
		return null;
	}
	private static string SlangCompile(string path, ShaderType type) {
		var fullpath = Assets.GetFullPath(path);
		if (fullpath is null) {
			Log.Error($"couldn't read file {path}");
			return null;
		}
		var stage = GetStageString(type);
		var p = new Process();
		p.StartInfo.FileName = Path.Join(Path.GetDirectoryName(Environment.ProcessPath), "slangc.exe");
		p.StartInfo.Arguments = $"{fullpath} -entry {stage} -stage {stage.ToLower()} -target glsl";
		foreach (var sp in Assets.SearchPaths)
			p.StartInfo.Arguments += $" -I {Path.Combine([sp.Engine.Directory, sp.Folder, "shaders"]).Replace('\\','/')}";
		p.StartInfo.RedirectStandardOutput = true;
		p.StartInfo.RedirectStandardError = true;
		p.StartInfo.UseShellExecute = false;
		p.StartInfo.CreateNoWindow = true;
		p.Start();
		var o = p.StandardOutput.ReadToEnd();
		var e = p.StandardError.ReadToEnd();
		p.WaitForExit();
		if (!string.IsNullOrWhiteSpace(e))
			Log.Error($"{path} {(p.ExitCode != 0 ? "failed to compile" : "compiled with warnings")}:\n{e}");
		if (p.ExitCode != 0)
			return null;
		return o;
	}
}