namespace Resource.Config;

public class GameInfo : Base<GameInfo> {
	public string Title {get; set;} = "BOXREND";
	public struct ResourceBlock() {
		public List<string> SearchPaths {get; set;} = [];
	}
	public ResourceBlock Resources {get; set;} = new();
}