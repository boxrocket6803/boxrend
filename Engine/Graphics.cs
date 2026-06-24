namespace Graphics;

using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Diagnostics;

public class Manager(Engine game) {
	public readonly Engine Game = game;
	public static GL Instance {get; private set;}
	public static uint BoundIndexCount {get; set;}

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

	private static readonly Dictionary<int, Draw.Batch> _batch = [];
	public void Render() {
		Screen.Size = Game.Window.FramebufferSize;
		if (Screen.Size.x == 0 || Screen.Size.y == 0)
			return;
		Clear();
		if (Scene.Manager.Active.MainCamera is null)
			return;
		Scene.Manager.Active.MainCamera.Update();
		_batch.Clear();
		foreach (var o in Scene.Manager.Active.Objects)
			o.Draw.Submit(_batch);
		Depth();
		Color();
		Post();
	}

	private static void Clear() => Instance.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); //TODO get clear color from camera
	private static void Depth() {
		foreach (var b in _batch.Values) {
			b.Material.Bind(b.Attributes, true);
			b.Mesh.DrawInstanced(b.Instances);
		}
		foreach (var o in Scene.Manager.Active.Objects)
			o.Draw.Commands.Depth.Draw();
	}
	private static void Color() {
		foreach (var b in _batch.Values) {
			b.Material.Bind(b.Attributes);
			b.Mesh.DrawInstanced(b.Instances);
		}
		foreach (var o in Scene.Manager.Active.Objects)
			o.Draw.Commands.Color.Draw();
	}
	private static void Post() {
		foreach (var o in Scene.Manager.Active.Objects)
			o.Draw.Commands.Post.Draw();
	}
}