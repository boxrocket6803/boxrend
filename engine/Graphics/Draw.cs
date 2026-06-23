public partial class Draw {
	//TODO calculate bounds from calls
	public Transform Transform {get; set;} = Transform.Indentity;
	public Attributes Attributes {get;} = new();
	public Queue Commands {get;} = new();

	public void Model(string p) => Model(Resource.Model.Load(p));
	public void Model(string p, Transform t) => Model(Resource.Model.Load(p), t);
	public void Model(Resource.Model m) => Model(m, Transform.Indentity);
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