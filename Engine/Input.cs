using Silk.NET.Input;
using Silk.NET.Windowing;

public partial class Input {
	private IInputContext Context;

	public void Init(IWindow window) {
		Context = window.CreateInput();
		Bindings.Init();
		Mouse.Init(Context);
	}

	public void Update() {
		State.Update(Context);
		Mouse.Update(Context);
		Keyboard.Update();
	}
}