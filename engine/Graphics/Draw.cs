public class Draw {
	public class BoundsAccessor {
		private BBox? Bounds {get; set;}

		public void BBox(BBox b) => Bounds = b;
		//public void Model(Resource.Model m, Transform t) {
		//	//TODO set bounds from model
		//}
		public void Clear() => Bounds = null;
	}

	public class SubmitAccessor {
		public struct MeshBatch() {
			public Mesh Mesh {get; set;}
			public Attributes Attributes {get; set;}
			public Resource.Material Material {get; set;}
			public List<Transform> Instances {get; set;} = []; //replace Transform with an instance data struct
		}
		private Dictionary<int, MeshBatch> Batches {get; set;} = [];
		public void Submit(Dictionary<int, MeshBatch> batches) {
			foreach (var batch in Batches) {
				if (batches.TryGetValue(batch.Key, out var group))
					group.Instances.AddRange(batch.Value.Instances);
				else
					batches[batch.Key] = batch.Value;
			}
		}

		public Attributes Attributes {get;} = new();
		public void Model(Resource.Model m, Transform t) {
			foreach (var mesh in m.Meshes)
				Mesh(mesh.GpuMesh, mesh.Material, t);
		}
		public void Mesh(Mesh m, Resource.Material s, Transform t) {
			var h = HashCode.Combine(m.Id, s.Id, Attributes);
			if (!Batches.TryGetValue(h, out var b))
				Batches[h] = b = new() {Mesh = m, Attributes = Attributes, Material = s};
			b.Instances.Add(t);
		}
		public void Clear() => Batches.Clear();
	}

	public class QueueAccessor {
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

	//TODO base transform
	public BoundsAccessor Bounds {get;} = new();
	public SubmitAccessor Submit {get;} = new();
	public QueueAccessor  Queue  {get;} = new();
}