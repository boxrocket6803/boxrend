namespace Resource;

public partial class Material : Config.Base<Material> {
	public readonly Guid Id = Guid.NewGuid();
	public Shader.Vertex Vertex {get; set;}
	public Shader.Fragment Fragment {get; set;}

	public Graphics.Attributes Attributes {get;} = new();

	public void Bind(Graphics.Attributes a, bool depth, bool shadow) {
		Vertex ??= Shader.Vertex.Load("shaders/vs_static.glsl");
		Fragment ??= Shader.Fragment.Load("shaders/fs_fallback.glsl");
		var p = GetProgram(depth, shadow);
		p.Attributes.Clear(); //this stuff runs way more than it needs to
		p.Attributes.Combine(Scene.Manager.Active.Attributes); //scene
		p.Attributes.Combine(Scene.Manager.Active.MainCamera.Attributes); //camera
		p.Attributes.Combine(Attributes); //material
		p.Attributes.Combine(a); //draw
		p.Attributes.Bind(p.Handle);
	}

	public static Material From(string v, string f) => new() {
		Vertex = Shader.Vertex.Load(v),
		Fragment = Shader.Fragment.Load(f),
	};
	public static Material Load(string model, string path) {
		var m = Load($"{path}.bmat");
		if (m is not null)
			return m;
		model = model.Split('.')[0];
		m = Load($"{model}/{path}.bmat");
		if (m is not null)
			return m;
		model = string.Join('/', model.Split('/').SkipLast(1));
		m = Load($"{model}/{path}.bmat");
		if (m is not null)
			return m;
		Log.Error($"couldn't find material {path} for {model}");
		return m;
	}

	public override bool Reload(string path) {
		if (!base.Reload(path))
			return false;
		//TODO load params from bmat, maybe just manually load everything
		return true;
	}
}