namespace Resource;

using System.Diagnostics;
using System.IO;

public class Model : Base<Model> {
	//TODO skeleton state, seperate object binds seperately
	[Flags] public enum Flags {
		Skeleton	= 1 << 0,
		Color		= 1 << 1,
		Morphs		= 1 << 2,
		Weights		= 1 << 3,
		BigIndicies = 1 << 4,
	}
	public struct Bone {
		public string Name {get; set;}
		public int Parent {get; set;}
		public Transform Bind {get; set;}
	}
	public class Mesh {
		public struct Vertex() {
			public Vector3 Position {get; set;}
			public Vector3 Normal {get; set;}
			public Vector2 TexCoord0 {get; set;}
			public int Color {get; set;} = int.MaxValue;
			public ushort[] Bones {get; set;} = [];
			public float[] Weights {get; set;} = [];
		}
		public struct Morph() {
			public uint[] Vertices {get; set;} = [];
			public Vector3[] Offset {get; set;} = [];
		}
		public string Name {get; set;}
		public global::Mesh GpuMesh {get; set;} //TODO this should just be part of the class
		public Material Material {get; set;}
		public uint[] Indices {get; set;} = [];
		public Vertex[] Vertices {get; set;} = [];
		public Morph[] Morphs {get; set;} = [];
	}
	public Bone[] Skeleton {get; set;} = [];
	public Mesh[] Meshes {get; set;} = [];

	public override bool Reload(string path) { //TODO calculate bounds somewhere in here
		var timer = Stopwatch.StartNew();
		var r = Assets.GetStream(path);
		if (r is null) {
			if (path == "models/error.bmdl")
				return false; //shit
			Log.Error($"using fallback for missing {path}");
			return Reload("models/error.bmdl");
		}
		var f = new BinaryReader(r);
		f.ReadBytes(4); //bmdl
		var flags = f.ReadByte();
		if ((flags & (byte)Flags.Skeleton) != 0) {
			//TODO load skeleton
		}
		Meshes = new Mesh[f.ReadByte()];
		for (var i = 0; i < Meshes.Length; i++) {
			Mesh mesh = new();
			mesh.Name = f.ReadString();
			mesh.Material = Material.Load(f.ReadString()) ?? new();
			mesh.Indices = new uint[f.ReadInt32()];
			for (var j = 0; j < mesh.Indices.Length; j++) {
				if ((flags & (byte)Flags.BigIndicies) != 0)
					mesh.Indices[j] = f.ReadUInt32();
				else
					mesh.Indices[j] = f.ReadUInt16();
			}
			mesh.Vertices = new Mesh.Vertex[f.ReadInt32()];
			for (var j = 0; j < mesh.Vertices.Length; j++) {
				Mesh.Vertex vertex = new();
				vertex.Position = new((float)f.ReadHalf(), (float)f.ReadHalf(), (float)f.ReadHalf());
				vertex.Normal = new((float)f.ReadHalf(), (float)f.ReadHalf(), (float)f.ReadHalf());
				var l = vertex.Normal.Length();
				if (l > 0) vertex.Normal /= l;
				vertex.TexCoord0 = new((float)f.ReadHalf(), (float)f.ReadHalf());
				if ((flags & (byte)Flags.Color) != 0)
					vertex.Color = f.ReadInt32();
				if ((flags & (byte)Flags.Weights) != 0) {
					vertex.Bones = new ushort[f.ReadByte()];
					for (var k = 0; k < vertex.Bones.Length; k++)
						vertex.Bones[k] = f.ReadUInt16();
					vertex.Weights = new float[vertex.Bones.Length];
					for (var k = 0; k < vertex.Weights.Length; k++)
						vertex.Weights[k] = (float)f.ReadHalf();
				}
				mesh.Vertices[j] = vertex;
			}
			if ((flags & (byte)Flags.Morphs) != 0) {
				//TODO load morphs
			}

			//gen vertex array //TODO rewrite this
			var vdesc = new float[mesh.Vertices.Length * 8];
			var w = 0;
			foreach (var v in mesh.Vertices) {
				vdesc[w++] = v.Position.X;
				vdesc[w++] = v.Position.Y;
				vdesc[w++] = v.Position.Z;
				vdesc[w++] = v.Normal.X;
				vdesc[w++] = v.Normal.Y;
				vdesc[w++] = v.Normal.Z;
				vdesc[w++] = v.TexCoord0.X;
				vdesc[w++] = v.TexCoord0.Y;
			}
			mesh.GpuMesh = new();
			mesh.GpuMesh.Load(vdesc, mesh.Indices);

			Meshes[i] = mesh;
		}
		f.Close();
		Log.Info($"{path} load in {Math.Round(timer.Elapsed.TotalSeconds * 1000, 2)}ms");
		return true;
	}
}