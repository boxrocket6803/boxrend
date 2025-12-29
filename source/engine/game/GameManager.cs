public class GameManager : Scene.Object {
	public override void OnCreate() {
		Scene.Active.MainCamera = new Scene.Camera.Perspective();
		base.OnCreate();
	}

	public Model Model;
	public override void Render() {
		Model.Load("models/characters/human_base.bmdl")?.Render(Transform.Indentity);
		base.Render();
	}
}