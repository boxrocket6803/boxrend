using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public class Graphics(Game game) {
	public readonly Game Game = game;

	public class Texture {
		public int Hash;
		public uint Width;
		public uint Height;
		public uint Depth;
		public uint Handle;

		public void Dispose() {
			Instance.DeleteTexture(Handle);
			Resident.Remove(Hash);
		}
		
		private readonly static Dictionary<int,Texture> Resident = [];
		public unsafe static Texture LoadPalette(Resources system, string path) {
			var hc = HashCode.Combine(system, path.ToLower(), "PALETTE");
			if (Resident.TryGetValue(hc, out var et))
				return et;
			var timer = Stopwatch.StartNew();
			var r = new BinaryReader(system.GetStream(path));
			r.ReadInt32(); //hash
			var count = r.ReadByte();
			var t = new Texture {
				Hash = hc,
				Handle = Instance.GenTexture(),
				Width = 256,
				Height= 1,
				Depth = 1
			};
			var pixels = new byte[t.Width * 3];
			for (int i = 0; i < pixels.Length;) {
				if (i < count * 3) {
					pixels[i++] = r.ReadByte();
					pixels[i++] = r.ReadByte();
					pixels[i++] = r.ReadByte();
				} else {
					pixels[i++] = 255;
					pixels[i++] = 0;
					pixels[i++] = 255;
				}
			}
			r.Close();
			Instance.ActiveTexture(TextureUnit.Texture0);
			Instance.BindTexture(TextureTarget.Texture1D, t.Handle);
			fixed (byte* ptr = pixels)
				Instance.TexImage1D(TextureTarget.Texture1D, 0, InternalFormat.Rgb8, t.Width, 0, PixelFormat.Rgb, PixelType.UnsignedByte, ptr);
			Instance.TextureParameter(t.Handle, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			Instance.TextureParameter(t.Handle, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
			Instance.TextureParameter(t.Handle, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
			Instance.TextureParameter(t.Handle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			Instance.TextureParameter(t.Handle, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			Instance.BindTexture(TextureTarget.Texture1D, 0);
			Resident.Add(hc, t);
			Log.Info($"{path} load in {Math.Round(timer.Elapsed.TotalSeconds * 1000, 2)}ms");
			return t;
		}
		public unsafe static Texture Load(Resources system, string path) {
			var hc = HashCode.Combine(system, path.ToLower());
			if (Resident.TryGetValue(hc, out var et))
				return et;
			var timer = Stopwatch.StartNew();
			var r = new BinaryReader(system.GetStream(path));
			r.ReadInt32(); //hash
			var t = new Texture {
				Hash = hc,
				Handle = Instance.GenTexture(),
				Width = r.ReadUInt16(),
				Height= r.ReadUInt16(),
				Depth = r.ReadUInt16()
			};
			var pixels = new byte[t.Width * t.Height * t.Depth];
			for (int i = 0; i < pixels.Length; i++) {
				pixels[i] = r.ReadByte();
				if (pixels[i] == 00) {
					var run = r.ReadByte();
					while (run > 0) {
						i++; run--;
						pixels[i] = 00;
					}
				}
			}
			r.Close();
			Instance.ActiveTexture(TextureUnit.Texture0);
			Instance.BindTexture(TextureTarget.Texture3D, t.Handle);
			fixed (byte* ptr = pixels)
				Instance.TexImage3D(TextureTarget.Texture3D, 0, InternalFormat.R8, t.Width, t.Height, t.Depth, 0, PixelFormat.Red, PixelType.UnsignedByte, ptr);
			Instance.TextureParameter(t.Handle, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			Instance.TextureParameter(t.Handle, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
			Instance.TextureParameter(t.Handle, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
			Instance.TextureParameter(t.Handle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			Instance.TextureParameter(t.Handle, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			Instance.BindTexture(TextureTarget.Texture3D, 0);
			Resident.Add(hc, t);
			Log.Info($"{path} load in {Math.Round(timer.Elapsed.TotalSeconds * 1000, 2)}ms");
			return t;
		}
		public static void FlushAll() {
			foreach (var texture in Resident.Values)
				texture.Dispose();
			Resident.Clear();
		}
		public static void Flush(Resources system, string path) {
			path = path.ToLower().Replace('\\', '/');
			var hc1 = HashCode.Combine(system, path);
			var hc2 = HashCode.Combine(system, path, "PALETTE");
			foreach (var texture in Resident.Values) {
				if (texture.Hash != hc1 && texture.Hash != hc2)
					continue;
				texture.Dispose();
			}
		}
	}
	public class Mesh {
		public uint Handle;
		public uint Count;

		public unsafe void Render() {
			Instance.BindVertexArray(Handle);
			Instance.DrawElements(PrimitiveType.Triangles, Count, DrawElementsType.UnsignedInt, (void*)0);
		}

		public static Mesh Sprite {get; set;}
		public unsafe static Mesh From(float[] verticies, uint[] indicies) {
			var m = new Mesh {
				Handle = Instance.GenVertexArray(),
				Count = (uint)indicies.Length
			};
			Instance.BindVertexArray(m.Handle);

			var vbo = Instance.GenBuffer();
			Instance.BindBuffer(BufferTargetARB.ArrayBuffer, m.Handle);
			fixed (float* buf = verticies)
				Instance.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(verticies.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);

			var ebo = Instance.GenBuffer();
			Instance.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
			fixed (uint* buf = indicies)
				Instance.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indicies.Length * sizeof(uint)), buf, BufferUsageARB.StaticDraw);

			Instance.EnableVertexAttribArray(0);
			Instance.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)0);
			Instance.EnableVertexAttribArray(1);
			Instance.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));

			Instance.BindVertexArray(0);
			Instance.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
			Instance.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

			return m;
		}
	}
	public class Shader {
		public int Hash;
		public uint Handle;

		public void Dispose() {
			Instance.DeleteShader(Handle);
			Resident.Remove(Hash);
		}

		private readonly static Dictionary<int,Shader> Resident = [];
		public static Shader Get(Resources system, string path, ShaderType type) {
			var hash = HashCode.Combine(system.Folder, path.ToLower(), type);
			if (Resident.TryGetValue(hash, out var es))
				return es;
			Log.Info($"compiling {path}");
			var s = Instance.CreateShader(type);
			var glsl = system.ReadAllText(path);
			if (glsl == null)
				return new Shader();
			Instance.ShaderSource(s, glsl);
			Instance.CompileShader(s);
			Instance.GetShader(s, GLEnum.CompileStatus, out var status);
			if (status != (int)GLEnum.True)
				Log.Exception($"{path} failed to compile: {Instance.GetShaderInfoLog(s)}");
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
	public class Program {
		public int Hash;
		public uint Handle;
		private readonly Dictionary<string,object> Attributes = [];

		public void Dispose() {
			Attributes.Clear();
			Instance.DeleteProgram(Handle);
			Resident.Remove(Hash);
		}
		private bool Check<T>(string property, T value) => Attributes.TryGetValue(property, out var a) && a.GetType() == typeof(T) && value.Equals((T)a);
		public void Set(string property, float value) {
			if (Check(property, value))
				return;
			Attributes[property] = value;
			UseProgram(this);
			Instance.Uniform1(Instance.GetUniformLocation(Handle, property), value);
		}
		public void Set(string property, System.Numerics.Vector2 value) {
			if (Check(property, value))
				return;
			Attributes[property] = value;
			UseProgram(this);
			Instance.Uniform2(Instance.GetUniformLocation(Handle, property), value);
		}
		public void Set(string property, Texture value) {
			if (Check(property, value))
				return;
			Attributes[property] = value;
			UseProgram(this);
			var loc = Instance.GetUniformLocation(Handle, property);
			Instance.BindTextureUnit((uint)loc, value.Handle);
			Instance.Uniform1(loc, loc);
		}

		private readonly static Dictionary<int,Program> Resident = [];
		public static Program From(Resources system, string vert, string frag) {
			var v = Shader.Get(system, vert, ShaderType.VertexShader);
			var f = Shader.Get(system, frag, ShaderType.FragmentShader);
			return From(v, f);
		}
		public static Program From(Shader vert, Shader frag) {
			var hc = HashCode.Combine(vert.Hash, frag.Hash);
			if (Resident.TryGetValue(hc, out var ep))
				return ep;
			var p = new Program {
				Handle = Instance.CreateProgram(),
				Hash = hc,
			};
			Instance.AttachShader(p.Handle, vert.Handle);
			Instance.AttachShader(p.Handle, frag.Handle);
			Instance.LinkProgram(p.Handle);
			Instance.GetProgram(p.Handle, ProgramPropertyARB.LinkStatus, out var status);
			if (status != (int)GLEnum.True)
				Log.Exception($"program failed to link {Instance.GetProgramInfoLog(p.Handle)}");
			Instance.DetachShader(p.Handle, vert.Handle);
			Instance.DetachShader(p.Handle, frag.Handle);
			Resident.Add(hc, p);
			return p;
		}
		public static void FlushAll() {
			foreach (var program in Resident.Values)
				program.Dispose();
			Resident.Clear();
		}
	}

	public static GL Instance {get; private set;}

	public static void Init(IWindow window) {
		var timer = Stopwatch.StartNew();
		Instance = window.CreateOpenGL();
		Instance.ClearColor(System.Drawing.Color.DarkGray);
		Instance.Enable(EnableCap.DepthTest);
		Instance.DepthFunc(DepthFunction.Less);

		Mesh.Sprite = Mesh.From([1,1,0,1,0,1,-1,0,1,1,-1,-1,0,0,1,-1,1,0,0,0], [0,1,3,1,2,3]);

		Log.Info($"graphics init in {Math.Round(timer.Elapsed.TotalSeconds * 1000, 2)}ms");
	}

	private static Program CurrentProgram;
	public static void UseProgram(Program program) {
		if (program == CurrentProgram)
			return;
		CurrentProgram = program;
		Instance.UseProgram(program.Handle);
	}

	public unsafe void Render() {
		if (Game.Window.FramebufferSize.X == 0 || Game.Window.FramebufferSize.Y == 0)
			return;
		Instance.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		Scene.RenderActive();
	}
}