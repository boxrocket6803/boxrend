using Resource;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

public partial class Assets(Engine engine, string folder) {
	public readonly Engine Engine = engine;
	public readonly string Folder = folder;
	public static Dictionary<string, Base> Resources {get; set;} = [];
	public static List<Assets> SearchPaths {get; set;} = [];
	private bool Loose;
	private ZipArchive Package;

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
	private bool Exists(string path) {
		if (Loose && File.Exists(Path.Combine([Engine.Directory, Folder, path])))
			return true;
		return Package?.GetEntry(path) is not null;
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
		Material.Flush();
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
	public static string GetFullPath(string path) { //TODO figure out how to handle zipped files
		foreach (var dir in SearchPaths) {
			if (dir.Exists(path))
				return Path.Combine([dir.Engine.Directory, dir.Folder, path]);
		}
		return null;
	}
}