namespace Graphics;

public partial class Attributes {
	private static uint Active {get; set;}
	private uint GlobalBuffer;
	
	public void Bind(uint handle) {
		if (Active != handle)
			Manager.Instance.UseProgram(handle);
		Active = handle;
		GenGlobals();
		var i = Manager.Instance.GetUniformBlockIndex(Active, "block_GlobalParams_std140_0");
		Manager.Instance.UniformBlockBinding(Active, i, 0);
		Manager.Instance.BindBufferBase(Silk.NET.OpenGL.BufferTargetARB.UniformBuffer, 0, GlobalBuffer);
	}

	public static void Flush() {}
}