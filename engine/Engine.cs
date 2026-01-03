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

	public void Init() {
		Directory = Path.GetDirectoryName(Environment.ProcessPath);
		var test = Directory.EndsWith("source\\engine\\bin");
		if (test) Directory = Directory.Replace("\\source\\engine\\bin", null);
		Time.Update();

		Assets.Init(this);
		Window = Silk.NET.Windowing.Window.Create(WindowOptions.Default with {
			Size = new(300, 1),
			Title = Resource.Load<GameInfo>("gameinfo.bcfg")?.Title ?? "BOX_DRAW",
			VSync = false
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
		if (!test)
			Window.WindowState = WindowState.Maximized;
		Time.Update();
		Log.Info($"engine init in {Math.Round(Time.RealNow * 1000, 2)}ms");
	}

	public void Run() {
		Window.Run();
		Destroy();
	}

	private void Update(double delta) {
		Assets.Update();
		Time.Update();
		Input.Update();
		Scene.UpdateActive();
		Audio.Update();
	}

	private void Destroy() => Window.Dispose();
}
