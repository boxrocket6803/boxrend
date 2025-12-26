using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;

public class Resource {
	private static Dictionary<string,object> Cache {get; set;} = [];
	public static T Load<T>(string path) where T : Resource, new() { //TODO cache this, need to update the classes on reload not replace them so refs dont break
		if (Cache.TryGetValue(path, out var r) && r is T resource)
			return resource;
		Cache[path] = new T();
		Reload(path);
		return (T)Cache[path];
	}
	public static void Reload(string path) {
		var timer = Stopwatch.StartNew();
		static object Value(Type type, string value) {
			if (value == "null")
				return null;
			if (type == typeof(string))
				return Regex.Replace(value, "^\"|\"$", "");
			if (Assets.GenericDataTypes.Contains(type))
				return Convert.ChangeType(value, type);
			var instance = type.GetConstructor([]).Invoke([]);
			value = value[1..^1].Trim();
			if (type.GetInterface(nameof(IEnumerable)) is not null) {
				if (instance is IList list) {
					foreach (var item in value.Split(',')) {
						if (string.IsNullOrEmpty(item))
							continue;
						list.Add(Value(type.GenericTypeArguments[0], item.Trim()));
					}
				} else
					Log.Error($"unkown enumerable type {type} encountered in Resource.Reload");
				return instance;
			}
			return Read(value, instance);
		}
		static object Read(string chunk, object target) {
			if (string.IsNullOrEmpty(chunk))
				return target;
			var key = chunk.Split('=')[0].Trim();
			var value = chunk.Trim()[(key.Length + 1)..].Trim()[1..].Trim();
			var next = value;
			var depth = 0;
			for (int i = 0; i < value.Length; i++) {
				char c = value[i];
				if (c == '{' || c == '[')
					depth++;
				if (c == '}' || c == ']')
					depth--;
				if (i + 1 >= value.Length) {
					next = null;
					break;
				}
				if (depth > 0)
					continue;
				if (c != ',' && c != '\n')
					continue;
				value = value[..i].Trim();
				next = next[(value.Length + 1)..].Trim();
				break;
			}
			//Log.Info($"k = {key}");
			//Log.Info($"v = {value}");
			var property = target.GetType().GetProperty(key);
			if (property is not null)
				property.SetValue(target, Value(property.PropertyType, value));
			if (next is not null)
				Read(next, target);
			return target;
		}
		Read(Assets.ReadText(path), Cache[path]);
		Log.Info($"{path} load in {Math.Round(timer.Elapsed.TotalSeconds * 1000, 2)}ms");
		//Log.Unfold(Cache[path]);
	}
}