public static class Game {
	public static Engine Engine {get; private set;} = new();

	public static void Main() {
		Engine.Init();
		Scene.Active.Objects.Add(new GameManager());
		Engine.Run();
	}
}