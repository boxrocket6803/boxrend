using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

public class Assets(Engine engine, string folder) {
	public readonly Engine Engine = engine;
	public readonly string Folder = folder;
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
		foreach (var item in HotloadList) { //TODO better system for this
			if (item.EndsWith(".glsl")) {
				Material.FlushAll();
				Shader.FlushAll();
			}
			if (item.EndsWith(".btex") || item.EndsWith(".bpal"))
				Texture.Flush(this, item);
			if (item.EndsWith(".bcfg"))
				Resource.Reload(item);
			if (item == "gameinfo.bcfg" && Folder == "core") {
				Log.Info("restarting asset system");
				Material.FlushAll();
				Shader.FlushAll();
				Texture.FlushAll();
				return true;
			}
		}
		Scene.FlushActive();
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

	public static readonly HashSet<Type> GenericDataTypes = [ //TODO seems like thered be a better way to do this
		typeof(bool), typeof(char), typeof(sbyte), typeof(byte),
		typeof(short), typeof(ushort), typeof(int), typeof(uint),
		typeof(long), typeof(ulong), typeof(float), typeof(double),
		typeof(decimal), typeof(DateTime), typeof(Enum)
	];
	private static List<Assets> SearchPaths {get; set;} = [];
	public static void Init(Engine engine) {
		var timer = Stopwatch.StartNew();
		SearchPaths.Clear();
		Assets core = new(engine, "core");
		core.Reload();
		SearchPaths.Add(core);
		var gameinfo = Resource.Load<GameInfo>("gameinfo.bcfg");
		if (gameinfo is null || gameinfo.Resources.SearchPaths is null) {
			Log.Error("core/gameinfo.bcfg missing/invalid!");
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
		engine.Window.Title = Resource.Load<GameInfo>("gameinfo.bcfg").Title;
	}
	public static string ReadText(string path) {
		foreach (var dir in SearchPaths) {
			var str = dir.Text(path);
			if (str is not null)
				return str;
		}
		Log.Error($"couldn't read file at {path}");
		return null;
	}
	public static Stream GetStream(string path) {
		foreach (var dir in SearchPaths) {
			var str = dir.Stream(path);
			if (str is not null)
				return str;
		}
		Log.Error($"couldn't read file at {path}");
		return null;
	}
}