namespace Resource;

using System.Diagnostics;
using System.IO;
using Silk.NET.OpenGL;

public class Texture : Base {
	public uint Width;
	public uint Height;
	public uint Depth;
	public uint Handle;

	public override unsafe bool Load(string path) { //TODO probably not a good idea to always assume 3d textures, but also we can determine that based on if depth is 1
		var timer = Stopwatch.StartNew();
		var f = Assets.GetStream(path);
		if (f is null)
			return false;
		var r = new BinaryReader(f);
		if (r is null)
			return false;
		r.ReadInt32(); //hash
		if (Handle == 0)
			Handle = Graphics.Instance.GenTexture();
		Width = r.ReadUInt16();
		Height= r.ReadUInt16();
		Depth = r.ReadUInt16();
		var pixels = new byte[Width * Height * Depth];
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
		Graphics.Instance.ActiveTexture(TextureUnit.Texture0);
		Graphics.Instance.BindTexture(TextureTarget.Texture3D, Handle);
		fixed (byte* ptr = pixels)
			Graphics.Instance.TexImage3D(TextureTarget.Texture3D, 0, InternalFormat.R8, Width, Height, Depth, 0, PixelFormat.Red, PixelType.UnsignedByte, ptr);
		Graphics.Instance.TextureParameter(Handle, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge); //TODO do we really have to set all of these
		Graphics.Instance.TextureParameter(Handle, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
		Graphics.Instance.TextureParameter(Handle, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
		Graphics.Instance.TextureParameter(Handle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
		Graphics.Instance.TextureParameter(Handle, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
		Graphics.Instance.BindTexture(TextureTarget.Texture3D, 0);
		Log.Info($"{path} load in {Math.Round(timer.Elapsed.TotalSeconds * 1000, 2)}ms");
		return true;
	}
}