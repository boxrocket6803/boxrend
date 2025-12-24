using System;
using System.Globalization;
using System.Diagnostics;

public static class Log {
	public static void Info(object message) {
		var str = String(':', message);
		Console.WriteLine(str);
		Trace.WriteLine(str);
	}

	public static void Error(object message) {
		var str = String('!', message);
		Console.WriteLine(str);
		Trace.WriteLine(str);
	}

	public static void Exception(object message) {
		var str = String('!', message);
		Console.WriteLine(str);
		Trace.WriteLine(str);
	}

	private static string String(char symbol, object message) {
		string msg =  Time.RealNow.ToString("F3")+symbol+' ';
		if (message == null)
			msg += "null";
		else if (message is IFormattable formattable)
			msg += formattable.ToString(null, CultureInfo.InvariantCulture);
		else
			msg += message.ToString();
		return msg;
	}
}