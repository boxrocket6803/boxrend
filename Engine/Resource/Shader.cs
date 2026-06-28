namespace Resource;

using Silk.NET.OpenGL;
using System.Diagnostics;

public abstract partial class Shader {
	public class Vertex : Shader<Vertex> {
		public override ShaderType Type => ShaderType.VertexShader;
		protected override uint Fallback() => Load("shaders/fallback.slang").Handle(false, false);
	}
	public class Fragment : Shader<Fragment> {
		public override ShaderType Type => ShaderType.FragmentShader;
		protected override uint Fallback() => Load("shaders/fallback.slang").Handle(false, false);
	}
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

	private static bool _failed = false;
	public override bool Reload(string path) {
		_failed = false;
		if (string.IsNullOrEmpty(path))
			return false;
		var timer = Stopwatch.StartNew();
		var perms = ProcessSlang(path, Type);
		perms ??= [];
		if (ShadwHandle == DepthHandle)
			ShadwHandle = 0;
		if (DepthHandle == ColorHandle)
			DepthHandle = 0;
		//COLOR
		if (perms.Length > 0) {
			if (!Compile(ref ColorHandle, Type, perms[0]))
				Fail(ref ColorHandle, Fallback(), path, null);
		} else ColorHandle = Fallback();
		//DEPTH
		if (perms.Length > 1) {
			if(!Compile(ref DepthHandle, Type, perms[1]))
				Fail(ref DepthHandle, Fallback(), path, "DEPTH");
		} else DepthHandle = ColorHandle;
		//SHADW
		if (perms.Length > 2) {
			if (!Compile(ref ShadwHandle, Type, perms[2]))
				Fail(ref ShadwHandle, DepthHandle, path, "DEPTH SHADOW");
		} else ShadwHandle = DepthHandle;
		if (!_failed)
			Log.Info($"{path} compile in {Math.Round(timer.Elapsed.TotalSeconds * 1000, 2)}ms for {GetStageString(Type).ToLower()} with {perms.Length} combos");
		return true;
	}

	protected virtual uint Fallback() => 0;
	private static bool Compile(ref uint handle, ShaderType type, string glsl) {
		if (glsl is null)
			return false;
		if (handle == 0)
			handle = Graphics.Manager.Instance.CreateShader(type);
		Graphics.Manager.Instance.ShaderSource(handle, glsl);
		Graphics.Manager.Instance.CompileShader(handle);
		Graphics.Manager.Instance.GetShader(handle, GLEnum.CompileStatus, out var status);
		return status == (int)GLEnum.True;
	}

	private static void Fail(ref uint handle, uint replace, string path, string combos) {
		_failed = true;
		var log = Graphics.Manager.Instance.GetShaderInfoLog(handle);
		Log.Error($"{path} failed to compile{(combos is null ? "" : " with combos "+combos)}{(string.IsNullOrEmpty(log) ? "!" : ":\n"+log)}");
		Graphics.Manager.Instance.DeleteShader(handle);
		handle = replace;
	}
}