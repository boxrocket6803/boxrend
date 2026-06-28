using Slang.Sdk;
using Slang.Sdk.Interop;
using System.Diagnostics;

namespace Resource;

public partial class Shader {
	public class Metadata : Base<Metadata> {
		public enum StandardCombos {
			NONE	= 0,
			DEPTH	= 1 << 0,
			SHADOW	= 1 << 1,
		}
		public StandardCombos Combos {get; private set;}
		public ShaderReflection Reflection {get; private set;}
		public HashSet<string> Dependents {get;} = [];
		public bool HasVertex {get; private set;}
		public bool HasFragment {get; private set;}
		public string Source {get; private set;}

		public override bool Reload(string path) {
			var s = Assets.ReadText(path);
			if (s is null) {
				Log.Error($"couldn't read file {path}");
				return false;
			}
			var timer = Stopwatch.StartNew();
			if (s == Source)
				return true;
			Source = s;
			ReloadDependents();
			FindEntryPoints();
			if (HasVertex || HasFragment)
				GetReflectionData(path);
			WalkImports(path);
			if (HasVertex || HasFragment)
				Log.Info($"{path} load in {Math.Round(timer.Elapsed.TotalSeconds * 1000, 2)}ms");
			return true;
		}

		private void FindEntryPoints() {
			HasVertex = Source.Contains("Vertex(");
			HasFragment = Source.Contains("Fragment(");
		}

		private void GetReflectionData(string path) {
			Reflection = OpenSlangProgram(path, Source)?.GetReflection();
			if (Reflection is null)
				return;
			foreach (var p in Reflection.Parameters)
				Log.Info($"{p.Name} - {p.Type.Name} at {p.BindingIndex}");
		}

		private void ReloadDependents() {
			foreach (var d in Dependents) {
				if (Assets.Resources.TryGetValue($"{typeof(Metadata)}:{d}", out var r) && r is Metadata m)
					m.Source = null; //so itll actually reload
				Assets.Reload(d);
			}
		}

		private void WalkImports(string path) {
			foreach (var line in Source.Split(['\n', ';'])) {
				if (line.TrimStart().StartsWith("import")) {
					var m = Load($"shaders/{line.Trim().Split(' ')[1].Replace('.', '/')}.slang");
					Combos |= m.Combos;
					m.Dependents.Add(path);
				}
			}
		}
	}
}