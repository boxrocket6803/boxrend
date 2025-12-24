using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Diagnostics;

public class Graphics(Game game) {
	public readonly Game Game = game;
	public static GL Instance {get; private set;}

	public static void Init(IWindow window) {
		var timer = Stopwatch.StartNew();
		Instance = window.CreateOpenGL();
		Instance.ClearColor(System.Drawing.Color.DarkGray);
		Instance.Enable(EnableCap.DepthTest);
		Instance.DepthFunc(DepthFunction.Less);

		Mesh.Init();
		Log.Info($"graphics init in {Math.Round(timer.Elapsed.TotalSeconds * 1000, 2)}ms");
	}

	//TODO functions to instance render meshes

	public void Render() {
		if (Game.Window.FramebufferSize.X == 0 || Game.Window.FramebufferSize.Y == 0)
			return;
		Instance.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		Scene.RenderActive();
	}
}