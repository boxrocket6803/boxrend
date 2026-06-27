using Silk.NET.OpenGL;
using Slang.Sdk;
using Slang.Sdk.Interop;

namespace Resource;

public partial class Shader<T> {
	private static string[] ProcessSlang(string path, ShaderType type) {
		var slang = Assets.ReadText(path);
		if (slang is null) {
			Log.Error($"couldn't read file {path}");
			return null;
		}
		//TODO depth and shadow combos
		var glsl = SlangCompile(path, slang, type);
		//Log.Info(glsl);
		return [glsl];
	}

	private static string GetStageString(ShaderType type) {
		if (type == ShaderType.FragmentShader) return "Fragment";
		if (type == ShaderType.VertexShader) return "Vertex";
		if (type == ShaderType.ComputeShader) return "Compute";
		Log.Exception("unknown stage in Shader.SLANG.GetStageString()");
		return null;
	}
	private static Stage GetStage(ShaderType type) {
		if (type == ShaderType.FragmentShader) return Stage.Fragment;
		if (type == ShaderType.VertexShader) return Stage.Vertex;
		if (type == ShaderType.ComputeShader) return Stage.Compute;
		Log.Exception("unknown stage in Shader.SLANG.GetStage()");
		return Stage.None;
	}

	//Module.Builder destroys everything when it gets disposed so just keep it alive forever lol
	private readonly static List<Module.Builder> _coolmemoryleak = [];
	private static string SlangCompile(string path, string slang, ShaderType type) {
		// Create the session
		var s = new Session.Builder().AddTarget(Targets.Glsl.v450).AddSearchPath("E:/bkit/bkit/shaders").Create(); //TODO make dynamic
		var m = new Module.Builder(s).AddTranslationUnit(SourceLanguage.Slang, "ROOT", out var idx);
		m = m.AddTranslationUnitSourceString(idx, path, slang);
		m = m.AddEntryPoint(idx, GetStageString(type), GetStage(type));
		_coolmemoryleak.Add(m);
		try {
			var e = m.Create().Program.Targets[Targets.Glsl.v450].EntryPoints[GetStageString(type)];
			return e.Compile().SourceCode;
		} catch (Exception e) {Log.Error($"{path} failed to compile:\n{e.Message[e.Message.IndexOf('\n')..].Trim()}");}
		return null;
	}
}