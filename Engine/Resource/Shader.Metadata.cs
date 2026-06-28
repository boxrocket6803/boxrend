namespace Resource;

public partial class Shader {
	public class Metadata : Base<Metadata> {
		public enum StandardCombos {
			NONE	= 0,
			DEPTH	= 1 << 0,
			SHADOW	= 1 << 1,
		}
		public StandardCombos Combos {get; set;}
		public HashSet<string> Dependents {get; set;} = [];
		public string Source {get; set;}

		public override bool Reload(string path) {
			var s = Assets.ReadText(path);
			if (s is null) {
				Log.Error($"couldn't read file {path}");
				return false;
			}
			if (s == Source)
				return true;
			Source = s;
			ReloadDependents();
			foreach (var line in Source.Split(['\n', ';'])) {
				if (line.TrimStart().StartsWith("import"))
					Import(path, line.Trim().Split(' ')[1]);
			}
			return true;
		}

		private void ReloadDependents() {
			foreach (var d in Dependents) {
				if (Assets.Resources.TryGetValue($"{typeof(Metadata)}:{d}", out var r) && r is Metadata m)
					m.Source = null; //so itll actually reload
				Assets.Reload(d);
			}
		}

		private void Import(string parent, string line) {
			var m = Load($"shaders/{line.Replace('.', '/')}.slang");
			Combos |= m.Combos;
			m.Dependents.Add(parent);
		}
	}
}