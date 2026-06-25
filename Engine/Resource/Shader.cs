namespace Resource;

using Silk.NET.OpenGL;
using System.Diagnostics;

public abstract class Shader {
	public class Vertex : Shader<Vertex> {public override ShaderType Type => ShaderType.VertexShader;}
	public class Fragment : Shader<Fragment> {public override ShaderType Type => ShaderType.FragmentShader;}
}

public abstract partial class Shader<T> : Base<T> where T : Base, new() {
	public virtual ShaderType Type {get;}
	private uint ColorHandle;
	private uint DepthHandle;
	private uint ShadwHandle;

	public uint Handle(bool depth, bool shadow) {
		if (shadow)
			return ShadwHandle;
		if (depth)
			return DepthHandle;
		return ColorHandle;
	}

	public override bool Reload(string path) {
		if (string.IsNullOrEmpty(path))
			return false;
		var timer = Stopwatch.StartNew();
		var glsl = Assets.ReadText(path);
		if (glsl is null) {
			Log.Error($"couldn't read file {path}");
			return false;
		}
		if (ShadwHandle == DepthHandle)
			ShadwHandle = 0;
		if (DepthHandle == ColorHandle)
			DepthHandle = 0;
		var perms = Precompile(path, glsl);
		//COLOR
		if (perms.Length > 0) {
			if (!Compile(ref ColorHandle, Type, perms[0]))
				Fail(ref ColorHandle, 0, path, null); //TODO default shader
		} else ColorHandle = 0; //TODO default shader
		//DEPTH
		if (perms.Length > 1) {
			if(!Compile(ref DepthHandle, Type, perms[1]))
				Fail(ref DepthHandle, ColorHandle, path, "DEPTH");
		} else DepthHandle = ColorHandle;
		//SHADW
		if (perms.Length > 2) {
			if (!Compile(ref ShadwHandle, Type, perms[2]))
				Fail(ref ShadwHandle, DepthHandle, path, "DEPTH SHADOW");
		} else ShadwHandle = DepthHandle;
		Log.Info($"{path} compile in {Math.Round(timer.Elapsed.TotalSeconds * 1000, 2)}ms ({perms.Length} COMBOS)");
		return true;
	}

	private static bool Compile(ref uint handle, ShaderType type, string glsl) {
		if (handle == 0)
			handle = Graphics.Manager.Instance.CreateShader(type);
		Graphics.Manager.Instance.ShaderSource(handle, glsl);
		Graphics.Manager.Instance.CompileShader(handle);
		Graphics.Manager.Instance.GetShader(handle, GLEnum.CompileStatus, out var status);
		return status == (int)GLEnum.True;
	}

	private static void Fail(ref uint handle, uint replace, string path, string combos) {
		Log.Error($"{path} failed to compile{(combos is null ? "" : " with combos "+combos)}: \n{Graphics.Manager.Instance.GetShaderInfoLog(handle)}");
		Graphics.Manager.Instance.DeleteShader(handle);
		handle = replace;
	}
}