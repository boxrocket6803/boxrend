namespace Scene.Camera;

public abstract class Base {
	public Manager Scene {get; set;}
	public Transform WorldTransform = Transform.Zero;
	public Graphics.Attributes Attributes {get;} = new();
	public virtual void Update() { }
}