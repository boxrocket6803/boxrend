public class GameManager : Scene.Object {
	public override void OnCreate() {
		Scene.Active.MainCamera = new Camera2D();
		base.OnCreate();
	}
}