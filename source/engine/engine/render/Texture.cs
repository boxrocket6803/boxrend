using System.Diagnostics;
using System.IO;
using Silk.NET.OpenGL;

public class Texture {
	public int Hash;
	public uint Width;
	public uint Height;
	public uint Depth;
	public uint Handle;

	public void Dispose() {
		Graphics.Instance.DeleteTexture(Handle);
		Resident.Remove(Hash);
	}
		
	private readonly static Dictionary<int,Texture> Resident = [];
	public unsafe static Texture LoadPalette(string path) { //TODO shouldnt this be more generic, will we even use this for anything (might as well with big int16 sized one?), shouldnt our file types have headers
		var hc = HashCode.Combine(path.ToLower(), "PALETTE");
		if (Resident.TryGetValue(hc, out var et))
			return et;
		var timer = Stopwatch.StartNew();
		var r = new BinaryReader(Assets.GetStream(path)); //TODO null check this
		r.ReadInt32(); //hash
		var count = r.ReadByte();
		var t = new Texture {
			Hash = hc,
			Handle = Graphics.Instance.GenTexture(),
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
		Graphics.Instance.ActiveTexture(TextureUnit.Texture0);
		Graphics.Instance.BindTexture(TextureTarget.Texture1D, t.Handle);
		fixed (byte* ptr = pixels)
			Graphics.Instance.TexImage1D(TextureTarget.Texture1D, 0, InternalFormat.Rgb8, t.Width, 0, PixelFormat.Rgb, PixelType.UnsignedByte, ptr);
		Graphics.Instance.TextureParameter(t.Handle, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge); //TODO do we really have to set all of these
		Graphics.Instance.TextureParameter(t.Handle, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
		Graphics.Instance.TextureParameter(t.Handle, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
		Graphics.Instance.TextureParameter(t.Handle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
		Graphics.Instance.TextureParameter(t.Handle, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
		Graphics.Instance.BindTexture(TextureTarget.Texture1D, 0);
		Resident.Add(hc, t);
		Log.Info($"{path} load in {Math.Round(timer.Elapsed.TotalSeconds * 1000, 2)}ms");
		return t;
	}
	public unsafe static Texture Load(string path) { //TODO probably not a good idea to always assume 3d textures, but also we can determine that based on if depth is 1
		if (path.EndsWith(".bpal")) //gross
			return LoadPalette(path);
		var hc = HashCode.Combine(path.ToLower());
		if (Resident.TryGetValue(hc, out var et))
			return et;
		var timer = Stopwatch.StartNew();
		var f = Assets.GetStream(path);
		if (f is null)
			return null;
		var r = new BinaryReader(f); //TODO null check this
		r.ReadInt32(); //hash
		var t = new Texture {
			Hash = hc,
			Handle = Graphics.Instance.GenTexture(),
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
		Graphics.Instance.ActiveTexture(TextureUnit.Texture0);
		Graphics.Instance.BindTexture(TextureTarget.Texture3D, t.Handle);
		fixed (byte* ptr = pixels)
			Graphics.Instance.TexImage3D(TextureTarget.Texture3D, 0, InternalFormat.R8, t.Width, t.Height, t.Depth, 0, PixelFormat.Red, PixelType.UnsignedByte, ptr);
		Graphics.Instance.TextureParameter(t.Handle, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge); //TODO do we really have to set all of these
		Graphics.Instance.TextureParameter(t.Handle, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
		Graphics.Instance.TextureParameter(t.Handle, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
		Graphics.Instance.TextureParameter(t.Handle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
		Graphics.Instance.TextureParameter(t.Handle, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
		Graphics.Instance.BindTexture(TextureTarget.Texture3D, 0);
		Resident.Add(hc, t);
		Log.Info($"{path} load in {Math.Round(timer.Elapsed.TotalSeconds * 1000, 2)}ms");
		return t;
	}
	public static void FlushAll() { //TODO should be per file name, means we need to actually save file name in instance
		foreach (var texture in Resident.Values)
			texture.Dispose();
		Resident.Clear();
	}
	public static void Flush(Assets system, string path) {
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