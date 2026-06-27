namespace Scene.Light;

public abstract class Base : Transform {
	public Manager Scene {get; set;}
	public int Index {get; set;}

	private static int _widx;
	public Base() : this(Manager.Context) { }
	public Base(Manager scene) {
		Scene = scene;
		Scene.Lights.Add(this);
		if (Scene.Lights.Count >= 1024)
			Log.Exception("too many lights!");
		for (var i = 0; i < 1024; i++) {
			if (!Scene.ActiveLights.Contains(_widx))
				break;
			if (++_widx >= 1024)
				_widx = 0;
		}
		Index = _widx;
		Scene.ActiveLights.Add(Index);
		//TODO clear existing light struct
		Manager.Context = scene;
	}
	public void Update() {
		//TODO Scene.Attributes[$"Lights[{Index}].Position"] = WorldPosition;
	}
	public void Destroy() {
		//TODO Scene.Attributes[$"Lights[{Index}].Color"] = Vector3.Zero;
		Scene.ActiveLights.Remove(Index);
		Scene.Lights.Remove(this);
	}
}