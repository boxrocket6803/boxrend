public class Scene(Engine game) {
	public Engine Game = game;

	public abstract class Object {
		public Scene Scene;
		public Object() : this(Context) { }
		public Object(Scene scene) {
			Scene = scene;
			Scene.Objects.Add(this);
			Context = scene;
			OnCreate();
		}

		public virtual void OnCreate() {}
		public virtual void OnUpdate() {}
		public virtual void Flush() {}
		public virtual void Render() {}
		public virtual void Dispose() => Scene.Objects.Remove(this);
	}
	public abstract class Camera {
		public Scene Scene;
		public virtual void Update(Material program) {}
	}

	public List<Object> Objects = [];
	public Camera MainCamera;

	public static Scene Active {get; set;}
	public static Scene Context {get => field ?? Active; set => field = value;}
	public static void UpdateActive() {
		Context = Active;
		foreach (var sceneobject in Active.Objects)
			sceneobject.OnUpdate();
	}
	public static void RenderActive() {
		Context = Active;
		if (Active.MainCamera == null)
			return;
		Active.MainCamera.Scene = Active;
		foreach (var sceneobject in Active.Objects)
			sceneobject.Render();
	}
	public static void FlushActive() {
		Context = Active;
		foreach (var sceneobject in Active.Objects)
			sceneobject.Flush();
	}
}