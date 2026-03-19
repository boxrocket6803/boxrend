public static class Trivia {
	public static readonly HashSet<Type> GenericDataTypes = [ //TODO seems like thered be a better way to do this
		typeof(bool), typeof(char), typeof(sbyte), typeof(byte),
		typeof(short), typeof(ushort), typeof(int), typeof(uint),
		typeof(long), typeof(ulong), typeof(float), typeof(double),
		typeof(decimal), typeof(DateTime), typeof(Enum)
	];
}