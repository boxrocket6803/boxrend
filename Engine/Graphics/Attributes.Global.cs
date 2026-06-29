namespace Graphics;

using Silk.NET.OpenGL;
using Slang.Sdk;

public partial class Attributes {
	private byte[] Globals;
	private uint GlobalBuffer;

	//TODO this only needs to be run once per frame, with the caveat that different shaders might use
	//different global buffer layouts, probably hash by names in layout and keep static dict of buffers
	//with hash key, then second hashset reset after update so it only updates the buffer once
	private void UpdateGlobal() {
		Globals ??= new byte[160];
		Array.Fill<byte>(Globals, 0);
		var w = 0;

		//TODO get the layout dynamically
		if (TryGetValue("Camera", out var d) && d is Scene.Camera.Base.CameraData camera) {
			Write(Globals, ref w, camera.Projection);
			Write(Globals, ref w, camera.View);
			Write(Globals, ref w, camera.Position);
			Write(Globals, ref w, camera.Forward);
		} else w += 160;
		if (GlobalBuffer == 0)
			GlobalBuffer = Manager.Instance.GenBuffer();
		Manager.Instance.BindBuffer(BufferTargetARB.UniformBuffer, GlobalBuffer);
		Manager.Instance.BufferData(BufferTargetARB.UniformBuffer, Globals, BufferUsageARB.StaticDraw);
		Manager.Instance.BindBuffer(BufferTargetARB.UniformBuffer, 0);
	}

	private static void Write(byte[] t, ref int w, Vector3 vec) {
		BitConverter.TryWriteBytes(new(t, w, 4), vec.x); w += 4;
		BitConverter.TryWriteBytes(new(t, w, 4), vec.y); w += 4;
		BitConverter.TryWriteBytes(new(t, w, 4), vec.z); w += 4;
		w += 4; //pad to vec4 length
	}
	private static void Write(byte[] t, ref int w, Matrix4x4 mat) {
		BitConverter.TryWriteBytes(new(t, w, 4), mat.M11); w += 4;
		BitConverter.TryWriteBytes(new(t, w, 4), mat.M12); w += 4;
		BitConverter.TryWriteBytes(new(t, w, 4), mat.M13); w += 4;
		BitConverter.TryWriteBytes(new(t, w, 4), mat.M14); w += 4;
		BitConverter.TryWriteBytes(new(t, w, 4), mat.M21); w += 4;
		BitConverter.TryWriteBytes(new(t, w, 4), mat.M22); w += 4;
		BitConverter.TryWriteBytes(new(t, w, 4), mat.M23); w += 4;
		BitConverter.TryWriteBytes(new(t, w, 4), mat.M24); w += 4;
		BitConverter.TryWriteBytes(new(t, w, 4), mat.M31); w += 4;
		BitConverter.TryWriteBytes(new(t, w, 4), mat.M32); w += 4;
		BitConverter.TryWriteBytes(new(t, w, 4), mat.M33); w += 4;
		BitConverter.TryWriteBytes(new(t, w, 4), mat.M34); w += 4;
		BitConverter.TryWriteBytes(new(t, w, 4), mat.M41); w += 4;
		BitConverter.TryWriteBytes(new(t, w, 4), mat.M42); w += 4;
		BitConverter.TryWriteBytes(new(t, w, 4), mat.M43); w += 4;
		BitConverter.TryWriteBytes(new(t, w, 4), mat.M44); w += 4;
	}
}