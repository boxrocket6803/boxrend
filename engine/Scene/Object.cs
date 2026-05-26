namespace Scene;

public abstract class Object {
	public Manager Scene;
	public Object() : this(Manager.Context) { }
	public Object(Manager scene) {
		Scene = scene;
		Scene.Objects.Add(this);
		Manager.Context = scene;
		OnCreate();
	}

	public virtual void OnCreate() {}
	public virtual void OnUpdate() {}
	public virtual void Render() {}
	public virtual void Destroy() => Scene.Objects.Remove(this);
}