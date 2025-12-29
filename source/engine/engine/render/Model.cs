using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

public class Model {
	//TODO skeleton
	private struct MeshChunk {
		public string Name {get; set;}
		public Material Material {get; set;}
		public Mesh Mesh {get; set;}
	}
	private List<MeshChunk> Meshes {get; set;} = [];
	public void Render(Transform transform) {
		foreach (var chunk in Meshes)
			Graphics.Draw(chunk.Material, chunk.Mesh, transform);
	}

	private static Dictionary<string,Model> Resident {get; set;} = [];
	public static Model Load(string path) {
		if (Resident.TryGetValue(path, out var model))
			return model;
		var timer = Stopwatch.StartNew();
		var r = Assets.GetStream(path);
		if (r is null)
			return null;
		var f = new BinaryReader(r);
		model = new Model();
		var bonecount = f.ReadUInt16();
		for (int i = 0; i < bonecount; i++) {
			Encoding.ASCII.GetString(f.ReadBytes(f.ReadByte())); //name
			f.ReadSingle(); f.ReadSingle(); f.ReadSingle(); //pos
			f.ReadSingle(); f.ReadSingle(); f.ReadSingle(); //ang
		}
		var meshcount = f.ReadByte();
		for (int i = 0; i < meshcount; i++) {
			var chunk = new MeshChunk {Name = Encoding.ASCII.GetString(f.ReadBytes(f.ReadByte()))};
			var mat = $"{Encoding.ASCII.GetString(f.ReadBytes(f.ReadByte()))}.bmat";
			var full = string.Join('/', path.Split('/').SkipLast(1));
			chunk.Material = Material.From(mat) ?? Material.From($"{full}/{mat}") ?? Material.From($"{full}/{path.Split('/').Last().Split('.')[0]}/{mat}");
			if (chunk.Material is null) {
				chunk.Material = Material.From("shaders/vs_model.glsl", "shaders/fs_fallback.glsl");
				Log.Error($"using fallback for missing {mat} (referenced in {path})");
			}
			f.ReadByte(); //uv channel count
			f.ReadByte(); //vertex color channel count
			var indicies = new uint[f.ReadUInt32()];
			for (int i2 = 0; i2 < indicies.Length; i2++)
				indicies[i2] = f.ReadUInt32();
			var vertexcount = f.ReadUInt32();
			var vertices = new float[vertexcount * 8];
			for (int i2 = 0; i2 < vertexcount; i2++) {
				for (int i3 = 0; i3 < 8; i3++)
					vertices[i2 * 8 + i3] = f.ReadSingle();
				var groupcount = f.ReadByte();
				for (int i3 = 0; i3 < groupcount; i3++) {
					f.ReadUInt16(); //bone index
					f.ReadSingle(); //weight
				}
			}
			chunk.Mesh = Mesh.From(vertices, indicies);
			model.Meshes.Add(chunk);
		}
		f.Close();
		Resident.Add(path, model);
		Log.Info($"{path} load in {Math.Round(timer.Elapsed.TotalSeconds * 1000, 2)}ms");
		return model;
	}
	public static void FlushAll() => Resident.Clear();
}