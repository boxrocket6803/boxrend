namespace Graphics;

public partial class Draw {
	public class Queue {
		public class Stage {
			private List<Action> Queue {get; set;} = [];
			public void Draw() {
				foreach (var a in Queue)
					a.Invoke();
			}

			public Attributes Attributes {get;} = new();
			public void Action(Action a) => Queue.Add(a);
			public void Clear() => Queue.Clear();
		}

		public Stage Depth {get;} = new();
		public Stage Color {get;} = new();
		public Stage Post  {get;} = new();
	}
}