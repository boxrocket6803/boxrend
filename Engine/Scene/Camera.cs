namespace Scene.Camera;

public abstract class Base : Transform {
	public struct CameraData {
		public Matrix4x4 Projection;
		public Matrix4x4 View;
		public Vector3 Position;
		public Vector3 Forward;
	}
	public Manager Scene {get; set;}
	public CameraData Data {get; protected set;}

	public virtual void Update() { }
}