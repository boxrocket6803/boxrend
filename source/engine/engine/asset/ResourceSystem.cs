using System;
using System.IO;
using System.IO.Compression;

public class ResourceSystem(Engine engine, string folder) {
	public readonly Engine Engine = engine;
	public readonly string Folder = folder;

	private bool Loose;
	private ZipArchive Package;

	private FileSystemWatcher Watcher;
	private readonly HashSet<string> HotloadList = [];
	private void UpdateDir() {
		if (HotloadList.Count == 0)
			return;
		try {
			foreach (var item in HotloadList) //this is messy and terrible, only way to do it though
				File.Open(Path.Combine([Engine.Directory, Folder, item]), FileMode.Open).Close();
		} catch {return;}
		Reload();
		foreach (var item in HotloadList) { //TODO better system for this
			if (item.EndsWith(".glsl")) {
				Material.FlushAll();
				Shader.FlushAll();
			}
			if (item.EndsWith(".btex") || item.EndsWith(".bpal"))
				Texture.Flush(this, item);
		}
		Scene.FlushActive();
		HotloadList.Clear();
	}
	private void Reload() {
		Package?.Dispose();
		Package = null;
		Loose = false;

		var path = Path.Combine(Engine.Directory, Folder);
		if (File.Exists(path+".zip"))
			Package = new ZipArchive(File.Open(path+".zip", FileMode.Open, FileAccess.Read, FileShare.ReadWrite), ZipArchiveMode.Read);
		if (Directory.Exists(path))
			Loose = true;
		if (Watcher == null && Path.Exists(Path.Combine(Engine.Directory, Folder))) {
			Watcher = new(Path.Combine(Engine.Directory, Folder)) {
				IncludeSubdirectories = true,
				EnableRaisingEvents = true,
			};
			Watcher.Changed += (s,e) => HotloadList.Add(e.Name);
		}
	}
	private string Text(string path) {
		if (Loose && File.Exists(Path.Combine([Engine.Directory, Folder, path])))
			return File.ReadAllText(Path.Combine([Engine.Directory, Folder, path]));
		var entry = Package?.GetEntry(path) ?? null;
		if (entry is not null)
			return new StreamReader(entry.Open()).ReadToEnd();
		return null;
	}
	private Stream Stream(string path) {
		if (Loose && File.Exists(Path.Combine([Engine.Directory, Folder, path])))
			return File.Open(Path.Combine([Engine.Directory, Folder, path]), FileMode.Open);
		return Package?.GetEntry(path)?.Open() ?? null;
	}

	private static List<ResourceSystem> SearchPaths {get; set;} = [];
	public static void Init(Engine engine) {
		ResourceSystem core = new(engine, "core");
		core.Reload();
		SearchPaths.Add(core);
		var gameinfo = Resource.Load<GameInfo>("gameinfo.bcfg");
		if (gameinfo is null) {
			Log.Error("core/gameinfo.bcfg missing!");
			Log.Info($"using single search path 'core'");
			return;
		}
		SearchPaths.Clear();
		foreach (var path in gameinfo.Resources.SearchPaths) {
			Log.Info($"adding search path {path}");
			if (path == "core") {
				SearchPaths.Add(core);
				core = null;
			} else {
				var dir = new ResourceSystem(engine, path);
				dir.Reload();
				SearchPaths.Add(dir);
			}
		}
		if (core is not null)
			SearchPaths.Add(core);
	}
	public static void Update() {
		foreach (var dir in SearchPaths)
			dir.UpdateDir();
	}
	public static string ReadText(string path) {
		foreach (var dir in SearchPaths) {
			var str = dir.Text(path);
			if (str is not null)
				return str;
		}
		return null;
	}
	public static Stream GetStream(string path) {
		foreach (var dir in SearchPaths) {
			var str = dir.Stream(path);
			if (str is not null)
				return str;
		}
		return null;
	}
}