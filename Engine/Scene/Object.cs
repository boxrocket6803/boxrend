namespace Scene;

public abstract class Object {
	public Manager Scene {get; private set;}
	public Transform WorldTransform {get; set;} = Transform.Indentity; //TODO this is shared with camera, put it in a base class or interface or something
	public Graphics.Draw Draw {get;} = new();

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