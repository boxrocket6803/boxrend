public class GameManager : Scene.Object {
	public override void OnCreate() {
		Scene.Active.MainCamera = new Scene.Camera();
		base.OnCreate();
	}
}