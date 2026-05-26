using Silk.NET.Windowing;
using System.IO;

public class Engine {
	public IWindow Window {get; set;}
	public string Directory {get; set;}
	public Graphics Graphics {get; set;}
	public Audio Audio {get; set;}
	public Input Input {get; set;}

	public Scene Scene {get; set;}

	public static void Main() {
		var e = new Engine();
		e.Init();
		e.Run();
		e.Destroy();
	}

	private static string GetDirectory() {
		var dir = Path.GetDirectoryName(Environment.ProcessPath).Split(Path.DirectorySeparatorChar);
		if (dir.Length > 3 && dir[^1] == "bin" && dir[^3] == "source") //debugging
			dir = dir[0..^3];
		return Path.Combine(dir);
	}

	public void Init() {
		Log.Info("init boxrend");
		Directory = GetDirectory();
		Time.Update();

		Assets.Init(this);
		Window = Silk.NET.Windowing.Window.Create(WindowOptions.Default with {
			Size = new(300, 1),
			Title = Resource.Load<GameInfo>("gameinfo.bcfg")?.Title ?? "BOXREND",
			VSync = false,
			Samples = 8,
		});
		Window.Initialize();
		Graphics = new(this);
		Graphics.Init(Window);
		Audio = new(this);
		Audio.Init();
		Input = new();
		Input.Init(Window);
		Scene.Active = Scene = new(this);

		Window.Update += Update;
		Window.FramebufferResize += (size) => Graphics.Instance?.Viewport(size);
		Window.Render += (d) => Graphics.Render();
		Window.Size = new(640, 480);
		Time.Update();
		Log.Info($"engine init in {Math.Round(Time.RealNow * 1000, 2)}ms");
	}

	private void Update(double delta) {
		Assets.Update();
		Time.Update();
		Input.Update();
		Scene.UpdateActive();
		Audio.Update();
	}

	public void Run() => Window.Run();
	private void Destroy() => Window.Dispose();
}
