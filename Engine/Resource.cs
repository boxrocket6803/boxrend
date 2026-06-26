namespace Resource;

public abstract class Base<T> : Base where T : Base, new() {
	public static T Load(string path) {
		path = path.Replace('\\', '/');
		if (Assets.Resources.TryGetValue(path, out var r) && r is T resource)
			return resource;
		resource = new T();
		if (!resource.Reload(path))
			resource = null;
		Assets.Resources[path] = resource;
		return resource;
	}
}

public abstract class Base {
	public virtual bool Reload(string path) => false;

	public static Base Load(Type type, string path) {
		path = path.Replace('\\', '/');
		if (Assets.Resources.TryGetValue(path, out var r) && r?.GetType() == type)
			return r;
		r = type.GetConstructor([]).Invoke([]) as Base;
		if (!r.Reload(path))
			r = null;
		Assets.Resources[path] = r;
		return r;
	}
}