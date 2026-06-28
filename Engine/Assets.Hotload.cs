using Resource;
using System.IO;
using System.IO.Compression;

public partial class Assets {
private FileSystemWatcher Watcher;
	private readonly HashSet<string> HotloadList = [];

	private bool UpdateDir() {
		if (HotloadList.Count == 0)
			return false;
		try { //TODO this whole function SUCKS
			foreach (var item in HotloadList) {
				var p = Path.Combine([Engine.Directory, Folder, item]);
				if (!File.Exists(p)) {
					HotloadList.Remove(item);
					return false;
				}
				File.Open(p, FileMode.Open).Close(); //bad, evil
			}
			Reload();
			foreach (var item in HotloadList) {
				var path = item.Replace('\\', '/');
				foreach (var r in Resources) {
					var p = r.Key.Split(':')[1];
					if (p != path)
						continue;
					r.Value.Reload(p);
				}
				if (path == "gameinfo.bcfg" && Folder == "core") {
					Log.Info("restarting asset system");
					return true;
				}
			}
		} catch {return false;} //TODO should clear from hotload list if the file doesnt exist
		Graphics.Attributes.Flush();
		Material.Flush();
		HotloadList.Clear();
		return false;
	}

	public static void Reload(string path) {
		foreach (var p in SearchPaths)
			p.HotloadList.Add(path);
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
			Watcher.Changed += (_,e) => {
				if (!e.Name.Contains('.')) { //visual studio resaves the entire folder or some shit
					foreach (var file in Directory.GetFiles(Path.Combine([Engine.Directory, Folder, e.Name]))) {
						var f = file.Replace(Path.Combine([Engine.Directory, Folder])+"\\", null);
						HotloadList.Add(f);
					}
				} else
					HotloadList.Add(e.Name);
			};
		}
	}
}