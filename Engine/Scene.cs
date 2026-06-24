namespace Scene;

public class Manager(Engine game) {
	public Engine Game = game;

	public HashSet<Object> Objects = [];
	public Camera.Base MainCamera;
	public T Add<T>() where T : Object, new() {
		Context = this;
		var t = new T();
		Context = Active;
		return t;
	}

	public static Manager Active {get; set;}
	public static Manager Context {get => field ?? Active; set;}
	public static void UpdateActive() {
		Context = Active;
		foreach (var sceneobject in Active.Objects)
			sceneobject.OnUpdate();
	}
}