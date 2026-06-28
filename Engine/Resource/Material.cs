namespace Resource;

public partial class Material : Config.Base<Material> {
	public readonly Guid Id = Guid.NewGuid();
	public string Shader {get; set;} = "shaders/fallback.slang";
	private Shader.Vertex Vertex {get; set;}
	private Shader.Fragment Fragment {get; set;}

	public Graphics.Attributes Attributes {get;} = new();

	public void Bind(Graphics.Attributes a, bool depth, bool shadow) {
		Vertex ??= Resource.Shader.Vertex.Load(Shader);
		Fragment ??= Resource.Shader.Fragment.Load(Shader);
		var p = GetProgram(depth, shadow);
		p.Attributes.Clear(); //this stuff runs way more than it needs to
		p.Attributes["Camera"] = Scene.Manager.Active.MainCamera.Data; //camera
		p.Attributes.Combine(Scene.Manager.Active.Attributes); //scene
		p.Attributes.Combine(Attributes); //material
		p.Attributes.Combine(a); //draw
		p.Attributes.Bind(p.Handle);
	}

	public static Material From(string s) => new() {Shader = s};
	public static Material From(string v, string f) => new() { //TODO remove this
		Vertex = Resource.Shader.Vertex.Load(v),
		Fragment = Resource.Shader.Fragment.Load(f),
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
		var f = Assets.ReadText(path);
		if (f is null)
			return false;
		Read(path, f, this);
		Shader = Shader.Replace('\\', '/');
		if (!Shader.StartsWith("shaders/"))
			Shader = $"shaders/{Shader}";
		var meta = Resource.Shader.Metadata.Load(Shader);
		return true;
	}
}