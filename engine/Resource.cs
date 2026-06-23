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
}