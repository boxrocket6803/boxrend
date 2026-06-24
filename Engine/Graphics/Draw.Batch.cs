namespace Graphics;

public partial class Draw {
	public struct Batch() {
		public Mesh Mesh {get; set;}
		public Attributes Attributes {get; set;}
		public Resource.Material Material {get; set;}
		public List<Transform> Instances {get; set;} = []; //replace Transform with an instance data struct
	}

	private Dictionary<int, Batch> Batches {get; set;} = [];
	public void Submit(Dictionary<int, Batch> batches) {
		foreach (var batch in Batches) {
			if (batches.TryGetValue(batch.Key, out var group))
				group.Instances.AddRange(batch.Value.Instances);
			else
				batches[batch.Key] = batch.Value;
		}
	}
}