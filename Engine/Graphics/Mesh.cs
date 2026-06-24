using Silk.NET.OpenGL;

public class Mesh { //should this even be seperate from model?
	public readonly Guid Id = Guid.NewGuid();
	public uint Handle;
	public uint Count;

	public void Bind() {
		Graphics.Instance.BindVertexArray(Handle);
		Graphics.BoundIndexCount = Count;
	}
	public unsafe void Draw(Transform transform) {
		Graphics.Instance.BindVertexArray(Handle);
		Graphics.Instance.DrawElements(PrimitiveType.Triangles, Count, DrawElementsType.UnsignedInt, (void*)0);
	}
	public void DrawInstanced(List<Transform> transforms) {
		foreach (var t in transforms)
			Draw(t);
		//TODO bind data, actually instance
		//Graphics.Instance.BindVertexArray(Handle);
		//Graphics.Instance.DrawElementsInstanced(PrimitiveType.Triangles, Count, DrawElementsType.UnsignedInt, (void*)0, (uint)transforms.Count);
	}

	public unsafe void Load(float[] verticies, uint[] indicies) {
		Handle = Graphics.Instance.GenVertexArray();
		Count = (uint)indicies.Length;
		Graphics.Instance.BindVertexArray(Handle);

		var vbo = Graphics.Instance.GenBuffer();
		Graphics.Instance.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
		fixed (float* buf = verticies)
			Graphics.Instance.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(verticies.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);

		var ebo = Graphics.Instance.GenBuffer();
		Graphics.Instance.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
		fixed (uint* buf = indicies)
			Graphics.Instance.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indicies.Length * sizeof(uint)), buf, BufferUsageARB.StaticDraw);

		Graphics.Instance.EnableVertexAttribArray(0); //position
		Graphics.Instance.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)0);
		Graphics.Instance.EnableVertexAttribArray(1); //normal
		Graphics.Instance.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)(3 * sizeof(float)));
		Graphics.Instance.EnableVertexAttribArray(2); //uv
		Graphics.Instance.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)(6 * sizeof(float)));

		Graphics.Instance.BindVertexArray(0);
		Graphics.Instance.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		Graphics.Instance.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
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