public class GameInfo : Resource {
	public string Title {get; set;} = "BOX_DRAW";
	public struct ResourceBlock() {
		public List<string> SearchPaths {get; set;} = [];
	}
	public ResourceBlock Resources {get; set;}
}