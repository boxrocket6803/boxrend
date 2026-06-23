namespace Scene.Camera;

public abstract class Base {
	public Manager Scene {get; set;}
	public Attributes Attributes {get;} = new();
	public virtual void Update() { }
}