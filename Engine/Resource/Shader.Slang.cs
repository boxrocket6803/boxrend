using Silk.NET.OpenGL;
using Slang.Sdk;
using Slang.Sdk.Interop;

namespace Resource;

public partial class Shader {
	//Module.Builder destroys everything when it gets disposed so just keep it alive forever lol
	private readonly static List<Module.Builder> _coolmemoryleak = [];
	public static Slang.Sdk.ProgramTarget OpenSlangProgram(string path, string source, string entry = null, Stage? stage = null) {
		var s = new Session.Builder().AddTarget(Targets.Glsl.v450);
		foreach (var sp in Assets.SearchPaths)
			s.AddSearchPath(System.IO.Path.Join([sp.Engine.Directory, sp.Folder, "shaders"]));
		var m = new Module.Builder(s.Create()).AddTranslationUnit(SourceLanguage.Slang, path, out var idx);
		m = m.AddTranslationUnitSourceString(idx, path, source);
		if (entry is not null && stage.HasValue)
			m = m.AddEntryPoint(idx, entry, stage.Value);
		_coolmemoryleak.Add(m);
		try {
			return m.Create().Program.Targets[Targets.Glsl.v450];
		} catch (Exception e) {Log.Error($"{path} failed to compile:\n{e.Message[e.Message.IndexOf('\n')..].Trim()}");}
		return null;
	}
}

public partial class Shader<T> {
	private static string[] ProcessSlang(string path, ShaderType type) {
		var meta = Shader.Metadata.Load(path);
		if (meta is null)
			return null;
		if (type == ShaderType.VertexShader && !meta.HasVertex)
			return null;
		if (type == ShaderType.FragmentShader && !meta.HasFragment)
			return null;
		var glsl = SlangCompile(path, meta.Source, type);
		return [glsl];
	}

	private static string SlangCompile(string path, string slang, ShaderType type) {
		try {
			ProgramEntryPoint e = null;
			if (type == ShaderType.FragmentShader)
				e = Shader.OpenSlangProgram(path, slang, "Fragment", Stage.Fragment).EntryPoints["Fragment"];
			else if (type == ShaderType.VertexShader)
				e = Shader.OpenSlangProgram(path, slang, "Vertex", Stage.Vertex).EntryPoints["Vertex"];
			else if (type == ShaderType.ComputeShader)
				e = Shader.OpenSlangProgram(path, slang, "Compute", Stage.Compute).EntryPoints["Compute"];
			else Log.Error($"unsupported shader type {type} encountered in SlangCompile");
			return e?.Compile().SourceCode;
		} catch (Exception e) {Log.Error($"{path} failed to compile:\n{e.Message[e.Message.IndexOf('\n')..].Trim()}");}
		return null;
	}
}