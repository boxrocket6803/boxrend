namespace Graphics;

public partial class Attributes : Dictionary<string,object> {
	private static uint Active {get; set;}

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

	public void Bind(uint handle, Resource.Shader.Metadata meta) {
		if (Active != handle)
			Manager.Instance.UseProgram(handle);
		Active = handle;
		UpdateGlobal();
		foreach (var p in meta.Reflection.Parameters) {
			if (p.Type.Name == "_Texture" && TryGetValue(p.Name, out var o) && o is Resource.Texture tex) {
				Manager.Instance.BindTextureUnit(p.BindingIndex, tex.Handle);
				Manager.Instance.Uniform1((int)p.BindingIndex, (int)p.BindingIndex);
			}
		}
		//TODO cache the state of this per program, only need to set it once
		Manager.Instance.UniformBlockBinding(Active, meta.Reflection.GlobalConstantBufferBinding, 0);
		Manager.Instance.BindBufferBase(Silk.NET.OpenGL.BufferTargetARB.UniformBuffer, 0, GlobalBuffer);
	}

	public static void Flush() {}
}