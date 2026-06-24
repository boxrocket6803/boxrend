namespace Scene.Camera;

public abstract class Base : Transform {
	public Manager Scene {get; set;}
	public Graphics.Attributes Attributes {get;} = new();
	public virtual void Update() { }
}