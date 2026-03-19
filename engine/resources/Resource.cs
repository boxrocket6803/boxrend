public abstract class Resource {
	public virtual bool Load(string path) => false;

	public static T Load<T>(string path) where T : Resource, new() {
		path = path.Replace('\\', '/');
		if (Assets.Resources.TryGetValue(path, out var r) && r is T resource)
			return resource;
		resource = new T();
		if (!resource.Load(path))
			resource = null;
		Assets.Resources[path] = resource;
		return resource;
	}
}