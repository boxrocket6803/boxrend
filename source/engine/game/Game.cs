public static class Game {
	public static Engine Engine = new();

	public static void Main() {
		Engine.Init("BOXDRAW_TEST");
		Scene.Active.Objects.Add(new GameManager());
		Engine.Run();
	}
}