namespace Resource.Config;

using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;

public class Base<T> : Resource.Base<T> where T : Resource.Base, new() {
	public override bool Reload(string path) { //TODO need a better solution for this SHIT
		var timer = Stopwatch.StartNew();
		var f = Assets.ReadText(path);
		if (f is null)
			return false;
		Read(path, f, this);
		Log.Info($"{path} load in {Math.Round(timer.Elapsed.TotalSeconds * 1000, 2)}ms");
		return true;
	}

	protected object Read(string path, string chunk, object target) {
		if (string.IsNullOrEmpty(chunk))
			return target;
		var (key,value,next) = ReadKV(chunk);
		var property = target.GetType().GetProperty(key);
		property?.SetValue(target, Value(path, property.PropertyType, value));
		if (next is not null)
			Read(path, next, target);
		return target;
	}
	protected object Read(string path, string chunk, Dictionary<string,Type> schema, Dictionary<string,object> target) {
		if (string.IsNullOrEmpty(chunk))
			return target;
		var (key,value,next) = ReadKV(chunk);
		if (schema.TryGetValue(key, out var type))
			target[key] = Value(path, type, value);
		if (next is not null)
			Read(path, next, schema, target);
		return target;
	}

	private static (string,string,string) ReadKV(string chunk) {
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
		return (key, value, next);
	}

	private object Value(string path, Type type, string value) {
		if (value.Last() == ',')
			value = value[0..^1];
		if (value == "null")
			return null;
		if (type == typeof(string))
			return Regex.Replace(value, "^\"|\"$", "");
		if (type.IsAssignableTo(typeof(Resource.Base))) {
			value = Regex.Replace(value, "^\"|\"$", "");
			return Load(type, value) ?? Load(type, $"{string.Join('/', path.Split('/').SkipLast(1))}/{value}") ?? Load(type, $"{path.Split('.')[0]}/{value}");
		}
		if (Base.GenericDataTypes.Contains(type))
			return Convert.ChangeType(value, type);
		var instance = type.GetConstructor([]).Invoke([]);
		value = value[1..^1].Trim();
		if (type.GetInterface(nameof(IEnumerable)) is not null) {
			if (instance is IList list) {
				foreach (var item in value.Split(',')) {
					if (string.IsNullOrEmpty(item))
						continue;
					list.Add(Value(path, type.GenericTypeArguments[0], item.Trim()));
				}
			} else if (instance is IDictionary dict && type.GenericTypeArguments[0] == typeof(string)) {
				foreach (var item in value.Split(',')) {
					if (string.IsNullOrEmpty(item))
						continue;
					var pair = item.Split('=');
					dict.Add(pair[0].Trim(), Value(path, type.GenericTypeArguments[1], pair[1].Trim()));
				}
			} else
				Log.Error($"unkown enumerable type {type} encountered while loading {path}");
			return instance;
		}
		return Read(path, value, instance);
	}
}

public class Base {
	public static readonly HashSet<Type> GenericDataTypes = [ //TODO seems like thered be a better way to do this
		typeof(bool), typeof(char), typeof(sbyte), typeof(byte),
		typeof(short), typeof(ushort), typeof(int), typeof(uint),
		typeof(long), typeof(ulong), typeof(float), typeof(double),
		typeof(decimal), typeof(DateTime), typeof(Enum)
	];

	public static void Serialilze(object message) {
		var str = $"{message.GetType()}:";
		static string Value(object value, Type type, string tab) {
			if (type == typeof(string))
				return $"\"{value}\"";
			if (type.GetInterface(nameof(IEnumerable)) is not null) {
				if (value is not IEnumerable enumerable)
					return "null";
				var any = false;
				var str = "[";
				foreach (var item in enumerable) {
					any = true;
					str += $"\n{tab}\t{Value(item, item.GetType(), tab+'\t')},";
				}
				if (any)
					str = $"{str[..^1]}\n{tab}";
				return $"{str}]";
			}
			if (GenericDataTypes.Contains(type))
				return value.ToString();
			return $"{{{Pair(value, tab+'\t')}\n{tab}}}";
		}
		static string Pair(object message, string tab) {
			var str = "";
			var array = message.GetType().GetProperties();
			for (int i = 0; i < array.Length; i++) {
				var property = array[i];
				str += $"\n{tab}{property.Name} = ";
				str += Value(property.GetValue(message), property.PropertyType, tab);
				if (i + 1 < array.Length)
					str += ',';
			}
			return str;
		}
		str += Pair(message, "\t\t");
		Trace.WriteLine(str);
	}
}