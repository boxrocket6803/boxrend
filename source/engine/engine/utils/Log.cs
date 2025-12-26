using System.Globalization;
using System.Diagnostics;
using System.Collections;

public static class Log {
	public static void Info(object message) {
		var str = String(':', message);
		Trace.WriteLine(str);
	}

	public static void Unfold(object message) { //this should probably be moved into Resource as a serializer
		var str = $"{message.GetType()}:";
		static string Value(object value, Type type, string tab) {
			if (type == typeof(string))
				return $"\"{value}\"";
			if (type.GetInterface(nameof(IEnumerable)) is not null) {
				var enumerable = value as IEnumerable;
				if (enumerable is null)
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
			if (Assets.GenericDataTypes.Contains(type))
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
		str = String(':', str);
		Trace.WriteLine(str);
	}

	public static void Error(object message) {
		var str = String('!', message);
		Trace.WriteLine(str);
	}

	public static void Exception(object message) {
		var str = String('!', message);
		Trace.WriteLine(str);
	}

	private static string String(char symbol, object message) {
		string msg = $"{Time.RealNow:000.00}{symbol} ";
		if (message == null)
			msg += "null";
		else if (message is IFormattable formattable)
			msg += formattable.ToString(null, CultureInfo.InvariantCulture);
		else
			msg += message.ToString();
		return msg;
	}
}