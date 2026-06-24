namespace Scene.Camera;

public abstract class Base {
	public Manager Scene {get; set;}
	public Transform WorldTransform {get; set;} = Transform.Indentity;
	public Graphics.Attributes Attributes {get;} = new();
	public virtual void Update() { }
}