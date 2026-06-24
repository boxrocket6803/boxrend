using System.Diagnostics;
using System.IO;
using System.IO.Compression;

public class Assets(Engine engine, string folder) {
	public readonly Engine Engine = engine;
	public readonly string Folder = folder;
	public static Dictionary<string, Resource.Base> Resources {get; set;} = [];
	private static List<Assets> SearchPaths {get; set;} = [];
	private bool Loose;
	private ZipArchive Package;
	private FileSystemWatcher Watcher;
	private readonly HashSet<string> HotloadList = [];

	private bool UpdateDir() {
		if (HotloadList.Count == 0)
			return false;
		try {
			foreach (var item in HotloadList) //this is messy and terrible, only way to do it though
				File.Open(Path.Combine([Engine.Directory, Folder, item]), FileMode.Open).Close();
		} catch {return false;}
		Reload();
		foreach (var item in HotloadList) {
			var path = item.Replace('\\', '/');
			if (Resources.TryGetValue(path, out var r))
				r.Reload(path);
			if (path == "gameinfo.bcfg" && Folder == "core") {
				Log.Info("restarting asset system");
				return true;
			}
		}
		Graphics.Attributes.Flush();
		HotloadList.Clear();
		return false;
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

	public static void Init(Engine engine) {
		var timer = Stopwatch.StartNew();
		Resources.Clear();
		SearchPaths.Clear();
		Assets core = new(engine, "core");
		core.Reload();
		SearchPaths.Add(core);
		var gameinfo = Resource.Config.GameInfo.Load("gameinfo.bcfg");
		if (gameinfo is null || gameinfo.Resources.SearchPaths is null) {
			Log.Error("core/gameinfo.bcfg missing or Resources.SearchPaths invalid!");
			Log.Info($"mounting\n + 'core'");
			return;
		}
		Log.Info("mounting");
		SearchPaths.Clear();
		foreach (var path in gameinfo.Resources.SearchPaths) {
			if (path == "core") {
				SearchPaths.Add(core);
				core = null;
			} else {
				var dir = new Assets(engine, path);
				dir.Reload();
				SearchPaths.Add(dir);
			}
			Log.Info($"+ '{path}'");
		}
		if (core is not null) {
			SearchPaths.Add(core);
			Log.Info($"+ 'core'");
		}
		Log.Info($"assets init in {Math.Round(timer.Elapsed.TotalSeconds * 1000, 2)}ms");
	}

	public static void Update() {
		var reset = false;
		foreach (var dir in SearchPaths)
			reset = reset || dir.UpdateDir();
		if (!reset)
			return;
		var engine = SearchPaths.First().Engine;
		Init(engine);
		engine.Window.Title = Resource.Config.GameInfo.Load("gameinfo.bcfg").Title;
		foreach (var r in Resources)
			r.Value.Reload(r.Key);
		Graphics.Attributes.Flush();
	}

	public static string ReadText(string path) { //TODO check if these are used
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