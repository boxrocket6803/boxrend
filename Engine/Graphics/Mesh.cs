using Silk.NET.OpenGL;

public class Mesh { //should this even be seperate from model?
	public readonly Guid Id = Guid.NewGuid();
	public uint Handle;
	public uint Count;

	public void Bind() {
		Graphics.Manager.Instance.BindVertexArray(Handle);
		Graphics.Manager.BoundIndexCount = Count;
	}
	public unsafe void Draw(Transform transform) {
		Graphics.Manager.Instance.BindVertexArray(Handle);
		Graphics.Manager.Instance.DrawElements(PrimitiveType.Triangles, Count, DrawElementsType.UnsignedInt, (void*)0);
	}
	public void DrawInstanced(List<Transform> transforms) {
		foreach (var t in transforms)
			Draw(t);
		//TODO bind data, actually instance
		//Graphics.Instance.BindVertexArray(Handle);
		//Graphics.Instance.DrawElementsInstanced(PrimitiveType.Triangles, Count, DrawElementsType.UnsignedInt, (void*)0, (uint)transforms.Count);
	}

	public unsafe void Load(float[] verticies, uint[] indicies) {
		Handle = Graphics.Manager.Instance.GenVertexArray();
		Count = (uint)indicies.Length;
		Graphics.Manager.Instance.BindVertexArray(Handle);

		var vbo = Graphics.Manager.Instance.GenBuffer();
		Graphics.Manager.Instance.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
		fixed (float* buf = verticies)
			Graphics.Manager.Instance.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(verticies.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);

		var ebo = Graphics.Manager.Instance.GenBuffer();
		Graphics.Manager.Instance.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
		fixed (uint* buf = indicies)
			Graphics.Manager.Instance.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indicies.Length * sizeof(uint)), buf, BufferUsageARB.StaticDraw);

		//TODO this should should be dynamic somehow
		Graphics.Manager.Instance.EnableVertexAttribArray(0); //position
		Graphics.Manager.Instance.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)0);
		Graphics.Manager.Instance.EnableVertexAttribArray(1); //normal
		Graphics.Manager.Instance.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)(3 * sizeof(float)));
		Graphics.Manager.Instance.EnableVertexAttribArray(2); //uv
		Graphics.Manager.Instance.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)(6 * sizeof(float)));

		Graphics.Manager.Instance.BindVertexArray(0);
		Graphics.Manager.Instance.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		Graphics.Manager.Instance.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
	}

	public static Mesh Sprite {get; set;}
	public static void Init() {
		Sprite ??= new();
		Sprite.Load([
		 1, 1,0,  0,0,1,  1,0,
		 1,-1,0,  0,0,1,  1,1,
		-1,-1,0,  0,0,1,  0,1,
		-1, 1,0,  0,0,1,  0,0,
		], [0,1,3,1,2,3]);
	}
}