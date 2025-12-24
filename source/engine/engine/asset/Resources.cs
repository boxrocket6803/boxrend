using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

public class Resources(Game game, string folder) {
	public readonly Game Game = game;
	public readonly string Folder = folder;

	private bool Loose;
	private ZipArchive Package;

	private FileSystemWatcher Watcher;
	private readonly HashSet<string> HotloadList = [];
	public bool Init() {
		Log.Info("resource manager loading "+Folder);
		Reload();
		return Loose || Package != null;
	}

	public void Update() {
		if (HotloadList.Count == 0)
			return;
		try {
			foreach (var item in HotloadList)
				File.Open(Path.Combine([Game.Directory, Folder, item]), FileMode.Open).Close();
		} catch {return;}
		Reload();
		foreach (var item in HotloadList) {
			if (item.EndsWith(".glsl")) {
				Graphics.Program.FlushAll();
				Graphics.Shader.FlushAll();
			}
			if (item.EndsWith(".btex") || item.EndsWith(".bpal"))
				Graphics.Texture.Flush(this, item);
		}
		Scene.FlushActive();
		HotloadList.Clear();
	}

	public void Reload() {
		Package?.Dispose();
		Package = null;
		Loose = false;

		var path = Path.Combine(Game.Directory, Folder);
		if (File.Exists(path+".zip"))
			Package = new ZipArchive(File.Open(path+".zip", FileMode.Open, FileAccess.Read, FileShare.ReadWrite), ZipArchiveMode.Read);
		if (Directory.Exists(path))
			Loose = true;
		if (Watcher == null && Path.Exists(Path.Combine(Game.Directory, Folder))) {
			Watcher = new(Path.Combine(Game.Directory, Folder)) {
				IncludeSubdirectories = true,
				EnableRaisingEvents = true,
			};
			Watcher.Changed += (s,e) => HotloadList.Add(e.Name);
		}
	}

	public byte[] ReadAllBytes(string path) {
		if (Loose && File.Exists(Path.Combine([Game.Directory, Folder, path])))
			return File.ReadAllBytes(Path.Combine([Game.Directory, Folder, path]));
		var entry = Package?.GetEntry(path) ?? null;
		if (entry != null) {
			using var outstream = new MemoryStream();
			entry.Open().CopyTo(outstream);
			return outstream.ToArray();
		}
		Log.Error($"resource manager couldn't find file {path}");
		return [];
	}

	public string ReadAllText(string path) {
		if (Loose && File.Exists(Path.Combine([Game.Directory, Folder, path])))
			return File.ReadAllText(Path.Combine([Game.Directory, Folder, path]));
		var entry = Package?.GetEntry(path) ?? null;
		if (entry != null)
			return new StreamReader(entry.Open()).ReadToEnd();
		Log.Error($"resource manager couldn't find file {path}");
		return null;
	}

	public Stream GetStream(string path) {
		if (Loose && File.Exists(Path.Combine([Game.Directory, Folder, path])))
			return File.Open(Path.Combine([Game.Directory, Folder, path]), FileMode.Open);
		var entry = Package?.GetEntry(path) ?? null;
		if (entry != null)
			return entry.Open();
		Log.Error($"resource manager couldn't find file {path}");
		return null;
	}
}