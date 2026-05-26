namespace Scene.Camera;

public abstract class Base {
	public Manager Scene {get; set;}
	public virtual void Update(Material program) {}
}