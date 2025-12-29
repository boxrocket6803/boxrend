using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Diagnostics;

public class Graphics(Engine game) {
	public readonly Engine Game = game;
	public static GL Instance {get; private set;}

	public static void Init(IWindow window) {
		var timer = Stopwatch.StartNew();
		Instance = window.CreateOpenGL();
		Instance.ClearColor(System.Drawing.Color.Black);
		Instance.Enable(EnableCap.DepthTest);
		Instance.DepthFunc(DepthFunction.Less);

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

	public static void Draw(Material material, Mesh mesh, Transform transform)  {
		var hc = HashCode.Combine(material.Id, mesh.Id);
		if (!Frame.TryGetValue(hc, out var batch))
			batch = Frame[hc] = new() {Material = material, Mesh = mesh};
		batch.Instances.Add(transform);
	}

	public static float AspectRatio {get; private set;}
	public void Render() {
		if (Game.Window.FramebufferSize.X == 0 || Game.Window.FramebufferSize.Y == 0)
			return;
		AspectRatio = Game.Window.FramebufferSize.X / (float)Game.Window.FramebufferSize.Y;
		//TODO get clear color from camera
		Instance.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		Scene.RenderActive();
		foreach (var batch in Frame.Values)
			batch.Render();
		Frame.Clear();
	}
}