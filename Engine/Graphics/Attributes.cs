namespace Graphics;

public partial class Attributes { //TODO custom hash
	private readonly Dictionary<string,object> Current = [];

	public void Set(string property, float value) => Current[property] = value;
	public void Set(string property, Vector2 value) => Current[property] = value;
	public void Set(string property, Resource.Texture value) => Current[property] = value.Handle;
	public void Set(string property, Matrix4x4 value) => Current[property] = value;
	
	public void Combine(Attributes a) {
		foreach (var attr in a.Current)
			Current[attr.Key] = attr.Value;
	}
	public void Clear() => Current.Clear(); //TODO do we need to clear uniforms too? (or more likely set dummy values)
}