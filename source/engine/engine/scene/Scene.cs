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
	public class Camera {
		public Scene Scene {get; set;}
		public virtual void Update(Material program) {}

		public class Perspective : Camera {
			public Transform WorldTransform {get; set;} = Transform.Indentity;
			public float FieldOfView {get; set;} = 90;
			private int Cache;
			private Matrix4x4 View;
			private Matrix4x4 Proj;
			public override void Update(Material program) { //TODO cache matrix with state
				var hc = HashCode.Combine(WorldTransform, FieldOfView, Graphics.AspectRatio);
				if (hc != Cache) {
					View = Matrix4x4.CreateLookAt(Vector3.UnitX * 36f + Vector3.UnitZ * 64f, Vector3.UnitZ * 48f, Vector3.UnitZ); //TODO replace with real numbers
					Proj = Matrix4x4.CreatePerspectiveFieldOfView(Math.Clamp(FieldOfView * 0.0174533f, 0f, 1.57f), Graphics.AspectRatio, 3f, 4000f);
					Cache = hc;
				}
				program.Set("view", View);
				program.Set("proj", Proj);
				base.Update(program);
			}
		}
	}

	public HashSet<Object> Objects = [];
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