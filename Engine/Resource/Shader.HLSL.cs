using Silk.NET.OpenGL;
using System.Diagnostics;
using System.IO;

namespace Resource;

public partial class Shader<T> {
	private static string[] ProcessHLSL(string path, ShaderType type) {
		//TODO split by shader type
		//TODO depth and shadow combos
		return [SlangCompile(path, type)];
	}

	private static string GetStageString(ShaderType type) {
		if (type == ShaderType.FragmentShader) return "fragment";
		if (type == ShaderType.VertexShader) return "vertex";
		if (type == ShaderType.ComputeShader) return "compute";
		Log.Exception("unknown stage in Shader.HLSL.GetStageString()");
		return null;
	}
	private static string SlangCompile(string path, ShaderType type) {
		var p = new Process();
		p.StartInfo.FileName = Path.Join(Path.GetDirectoryName(Environment.ProcessPath), "slangc.exe");
		p.StartInfo.Arguments = $"{Assets.GetFullPath(path)} -entry Main -stage {GetStageString(type)} -target glsl";
		p.StartInfo.RedirectStandardOutput = true;
		p.StartInfo.RedirectStandardError = true;
		p.StartInfo.UseShellExecute = false;
		p.StartInfo.CreateNoWindow = true;
		p.Start();
		var o = p.StandardOutput.ReadToEnd();
		var e = p.StandardError.ReadToEnd();
		p.WaitForExit();
		if (p.ExitCode != 0) {
			Log.Error($"{path} failed to compile:\n{e}");
			return null;
		}
		return o;
	}
}