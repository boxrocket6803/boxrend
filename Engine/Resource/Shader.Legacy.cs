namespace Resource;

public partial class Shader<T> { //TODO remove this
	private readonly static HashSet<string> _included = [];
	private static string[] ProcessGLSL(string path) {
		var glsl = Assets.ReadText(path);
		if (glsl is null) {
			Log.Error($"couldn't read file {path}");
			return null;
		}
		_included.Clear();
		Include(path, ref glsl);
		return Combos(glsl);
	}

	private static void Include(string path, ref string glsl) {
		foreach (var line in glsl.Split('\n')) {
			if (line.Trim().StartsWith("#include")) {
				var file = line.Trim()["#include".Length..].Trim().Trim('"');
				if (!file.StartsWith("shaders/"))
					file = $"shaders/{file}";
				if (_included.Contains(file))
					continue;
				_included.Add(file);
				var rep = Assets.ReadText(file);
				if (rep is null) {
					Log.Error($"couldn't find file {file} for #include directive in {path}");
					return;
				} else
					Include(file, ref rep);
				glsl = glsl.Replace(line, $"//{file}\n{rep}\n//");
				continue;
			}
		}
	}

	//all (or most) shaders have DEPTH and SHADOW combos, thus 3 permutations
	//DEPTH almost always exists, SHADOW sometimes does. this returns in the
	//order !DEPTH && !SHADOW, DEPTH && !SHADOW, DEPTH && SHADOW
	private static string[] Combos(string glsl) {
		var depth = false;
		var shadw = false;
		foreach (var line in glsl.Split('\n')) {
			var l = line.TrimStart();
			depth = depth || l.StartsWith("#if DEPTH") ||  l.StartsWith("#if !DEPTH") || l.StartsWith("void depth()");
			shadw = shadw || l.StartsWith("#if SHADOW") ||  l.StartsWith("#if !SHADOW") || l.StartsWith("void shadow()");
			if (depth && shadw)
				break;
		}
		var count = 1;
		if (depth || shadw)
			count++;
		if (depth && shadw)
			count++;
		var i = glsl.IndexOf('\n');
		var perms = new string[count];
		if (count > 0)
			perms[0] = glsl.Insert(i, "\n#define DEPTH 0\n#define SHADOW 0").Replace("void color()", "void main()");
		if (count > 1)
			perms[1] = glsl.Insert(i, "\n#define DEPTH 1\n#define SHADOW 0").Replace("void depth()", "void main()");
		if (count > 2)
			perms[2] = glsl.Insert(i, "\n#define DEPTH 1\n#define SHADOW 1").Replace("void depth()", "void main()");
		return perms;
	}
}