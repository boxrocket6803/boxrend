namespace Scene;

public class Manager(Engine game) {
	public Engine Game = game;

	public Graphics.Attributes Attributes {get;} = new();

	public HashSet<Object> Objects {get;} = [];
	public Camera.Base MainCamera {get; set;}

	public HashSet<Light.Base> Lights {get;} = [];
	public HashSet<int> ActiveLights {get;} = [];

	public T Add<T>() where T : Transform, new() {
		Context = this;
		var t = new T();
		Context = Active;
		return t;
	}

	public static Manager Active {get; set;}
	public static Manager Context {get => field ?? Active; set;}
	public static void UpdateActive() {
		Context = Active;
		foreach (var o in Active.Objects)
			o.Update();
		foreach (var o in Active.Lights)
			o.Update();
	}
}