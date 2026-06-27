namespace Graphics;

public partial class Attributes : Dictionary<string,object> {
	public void Combine(Attributes a) {
		foreach (var attr in a)
			this[attr.Key] = attr.Value;
	}

	public override int GetHashCode() {
		var h = new HashCode();
		foreach (var a in this) {
			h.Add(a.Key);
			h.Add(a.Value);
		}
		return h.ToHashCode();
	}
}