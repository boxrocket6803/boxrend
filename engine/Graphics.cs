using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Diagnostics;

public class Graphics(Engine game) {
	public enum RenderStage {
		Idle	= 1 << 0, //not rendering
		Submit	= 1 << 1, //collect instances
		Depth	= 1 << 2, //light shadows, depth prepass
		Forward	= 1 << 3, //color, transparent sorted last
		Post	= 1 << 4, //post processing, interface
	}

	public readonly Engine Game = game;
	public static GL Instance {get; private set;}

	public static void Init(IWindow window) {
		var timer = Stopwatch.StartNew();
		Instance = window.CreateOpenGL();
		Instance.ClearColor(System.Drawing.Color.Black);
		Instance.Enable(EnableCap.DepthTest);
		Instance.Enable(EnableCap.Multisample);
		Instance.DepthFunc(DepthFunction.Lequal);

		Mesh.Init();
		Log.Info($"graphics init in {Math.Round(timer.Elapsed.TotalSeconds * 1000, 2)}ms");
	}

	private struct Batch() {
		public Mesh Mesh {get; set;}
		public Material Material {get; set;}
		public List<Transform> Instances {get; set;} = [];
		public readonly void Render() {
			Material.Bind();
			Mesh.DrawInstanced(Instances);
		}
	}
	private static Dictionary<int,Batch> Frame {get; set;} = [];

	public static void Draw(Material material, Mesh mesh, Transform transform) {
		if (Stage != RenderStage.Submit)
			Log.Exception("Graphics.Draw must be called in submit stage");
		var hc = HashCode.Combine(material.Id, mesh.Id);
		if (!Frame.TryGetValue(hc, out var batch))
			batch = Frame[hc] = new() {Material = material, Mesh = mesh};
		batch.Instances.Add(transform);
		//TODO might want to make more informed decisions as to what gets depth prepassed, might be faster without skinned meshes
	}

	public static float AspectRatio {get; private set;}
	public static RenderStage Stage {get; private set;} = RenderStage.Idle;
	public void Render() {
		if (Game.Window.FramebufferSize.X == 0 || Game.Window.FramebufferSize.Y == 0)
			return;
		AspectRatio = Game.Window.FramebufferSize.X / (float)Game.Window.FramebufferSize.Y;
		//TODO get clear color from camera
		Instance.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		
		Stage = RenderStage.Submit;
		Frame.Clear();
		Scene.Manager.RenderActive();
		//TODO sort batches by distance

		Stage = RenderStage.Depth;
		//TODO shadows
		foreach (var batch in Frame.Values)
			batch.Render();
		Scene.Manager.RenderActive();

		Stage = RenderStage.Forward;
		foreach (var batch in Frame.Values)
			batch.Render();
		Scene.Manager.RenderActive();

		Stage = RenderStage.Post;
		Scene.Manager.RenderActive();
		//TODO collect and render post process layers

		Stage = RenderStage.Idle;
	}
}