namespace Scene;

public abstract class Object : Transform {
	public Manager Scene {get; private set;}
	public Graphics.Draw Draw {get;} = new();

	public Object() : this(Manager.Context) { }
	public Object(Manager scene) {
		Scene = scene;
		Scene.Objects.Add(this);
		Manager.Context = scene;
		OnCreate();
	}
	public void Update() => OnUpdate();
	public void Destroy() {
		Scene.Objects.Remove(this);
		OnDestroy();
	}

	protected virtual void OnCreate() {}
	protected virtual void OnUpdate() {}
	protected virtual void OnDestroy() {}
}