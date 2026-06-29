namespace Resource;

using System.Diagnostics;
using System.IO;
using Silk.NET.OpenGL;

public class Texture : Base<Texture> {
	public enum Format {
		rgba8888 = 0,
		rgb888 = 1,
		g8 = 2,
		g32 = 3,
	}

	public Format Type;
	public uint Width;
	public uint Height;
	public uint Depth;
	public Vector2 Size => new(Width, Height);
	public uint Handle;

	public override bool Reload(string path) {
		Log.Info(path);
		var timer = Stopwatch.StartNew();
		var f = Assets.GetStream(path);
		if (f is null)
			return false;
		var r = new BinaryReader(f);
		if (r is null)
			return false;
		ReadHeader(r);
		switch (Type) {
			case Format.rgba8888:
				Load<byte>(r, InternalFormat.Rgba8, PixelFormat.Rgba, 4); break;
			case Format.rgb888:
				Load<byte>(r, InternalFormat.Rgb8, PixelFormat.Rgb, 3); break;
			case Format.g8:
				Load<byte>(r, InternalFormat.R8, PixelFormat.Red, 1); break;
			case Format.g32:
				Load<uint>(r, InternalFormat.R32ui, PixelFormat.RedInteger, 1); break;
		}
		r.Close();
		Log.Info($"{path} load in {Math.Round(timer.Elapsed.TotalSeconds * 1000, 2)}ms");
		return true;
	}
	private unsafe void Load<T>(BinaryReader r, InternalFormat i, PixelFormat p, int c) {
		if (Handle == 0)
			Handle = Graphics.Manager.Instance.CreateTexture(TextureTarget.Texture2D);
		Graphics.Manager.Instance.ActiveTexture(TextureUnit.Texture0);
		Graphics.Manager.Instance.BindTexture(TextureTarget.Texture2D, Handle);
		if (typeof(T) == typeof(byte)) {
			var pixels = ReadData<byte>(r, c);
			fixed (byte* ptr = pixels)
				Graphics.Manager.Instance.TexImage2D(TextureTarget.Texture3D, 0, i, Width, Height, 0, p, PixelType.UnsignedByte, ptr);
		} else if (typeof(T) == typeof(uint)) {
			var pixels = ReadData<uint>(r, c);
			fixed (uint* ptr = pixels)
				Graphics.Manager.Instance.TexImage2D(TextureTarget.Texture3D, 0, i, Width, Height, 0, p, PixelType.UnsignedByte, ptr);
		} else
			Log.Error("unsupported type in Texture.Load");
		Graphics.Manager.Instance.BindTexture(TextureTarget.Texture2D, 0);
	}

	protected void ReadHeader(BinaryReader r) {
		r.ReadBytes(4); //btex
		Type = (Format)r.ReadByte();
		Width = r.ReadUInt16();
		Height = r.ReadUInt16();
		Depth = r.ReadUInt16();
	}
	protected T[] ReadData<T>(BinaryReader r, int channels) {
		Log.Info($"{typeof(T)} {Type}");
		var d = new T[Width * Height * Depth * channels];
		var w = 0;
		var c = new object[channels];
		var run = 0;
		for (var z = 0; z < Depth; z++) {
			for (var y = 0; y < Height; y++) {
				for (var x = 0; x < Width; x++) {
					if (run <= 0) {
						for (var ch = 0; ch < c.Length; ch++)
							c[ch] = Type == Format.g32 ? (object)r.ReadUInt32() : r.ReadByte();
						run = r.ReadByte() + 1;
					}
					for (var ch = 0; ch < channels; ch++)
						d[w++] = (T)(ch < c.Length ? c[ch] : (ch < 4 ? 0 : 1));
					run--;
				}
			}
		}
		return d;
	}
}