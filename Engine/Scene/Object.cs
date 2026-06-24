namespace Scene;

public abstract class Object {
	public Manager Scene {get; private set;}
	public Draw Draw {get;} = new();

	public Object() : this(Manager.Context) { }
	public Object(Manager scene) {
		Scene = scene;
		Scene.Objects.Add(this);
		Manager.Context = scene;
		OnCreate();
	}

	protected virtual void OnCreate() {}
	public virtual void OnUpdate() {}
	public virtual void Destroy() => Scene.Objects.Remove(this);
}