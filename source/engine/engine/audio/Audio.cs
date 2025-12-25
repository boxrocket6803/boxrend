using NVorbis;
using Silk.NET.OpenAL;
using System.IO;

public class Audio(Engine game) {
	public Engine Game = game;

	public class Sound {
		public bool Loop;

		private AL Instance;
		private VorbisReader Stream;
		private uint Source;
		private struct Buffer {
			public int Start;
			public int Size;
		}
		private readonly List<Buffer> Buffers = [];
		private readonly List<uint> BufferRefs = [];
		private int LoadIndex = 0;
		private bool Active;
		private float FadeTime;
		private float Fade;

		public static Sound Load(AL instance, byte[] file) {
			var sound = new Sound {
				Stream = new VorbisReader(new MemoryStream(file)),
				Instance = instance,
				Source = instance.GenSource()
			};
			var chunk = sound.Stream.Channels * sound.Stream.SampleRate;
			for (int i = 0; i < sound.Stream.TotalSamples; i += chunk) {
				sound.Buffers.Add(new Buffer() {
					Start = i,
					Size = (Math.Min(i + chunk, (int)sound.Stream.TotalSamples) - i),
				});
			}
			sound.LoadIndex = -1;
			sound.Chunk();
			return sound;
		}
		private unsafe void Chunk() {
			LoadIndex++;
			Stream.SamplePosition = Buffers[LoadIndex].Start;
			var samples = new float[Buffers[LoadIndex].Size * Stream.Channels];
			Stream.ReadSamples(samples);
			var format = new short[samples.Length];
			for (int i = 0; i < samples.Length; i++)
				format[i] = (short)(samples[i] * short.MaxValue);
			var buffer = Instance.GenBuffer();
			fixed (void* ptr = format)
				Instance.BufferData(buffer, Stream.Channels == 1 ? BufferFormat.Mono16 : BufferFormat.Stereo16, ptr, format.Length * Stream.Channels, Stream.SampleRate);
			BufferRefs.Add(buffer);
			Instance.SourceQueueBuffers(Source, [buffer]);
		}
		public void Play(float fade = 0) {
			Instance.SetSourceProperty(Source, SourceBoolean.Looping, Loop);
			Instance.SourcePlay(Source);
			Active = true;
			Fade = 0;
			if (fade > 0)
				FadeTime = fade;
			else
				Fade = 1;
			Instance.SetSourceProperty(Source, SourceFloat.Gain, Fade);
		}
		public void Update() {
			Instance.GetSourceProperty(Source, GetSourceInteger.SampleOffset, out var position);
			if (position >= Buffers[LoadIndex].Start && LoadIndex + 1 < Buffers.Count)
				Chunk();
			if (Active)
				Fade += Time.Delta / FadeTime;
			else
				Fade -= Time.Delta / FadeTime;
			Fade = Math.Clamp(Fade, 0, 1);
			Instance.SetSourceProperty(Source, SourceFloat.Gain, Fade < 0.5f ? 2 * Fade * Fade : 1 - MathF.Pow(-2 * Fade + 2, 2) / 2);
		}
	}

	public AL Instance;
	public readonly List<Sound> Sounds = [];
	public string CurrentAmbience;
	public Sound Ambient;
	
	public void Init() {
		new AudioContext(null, 22050).MakeCurrent();
		Instance = AL.GetApi(true);
		Instance.SetListenerProperty(ListenerFloat.Gain, 3);
	}

	public void SetAmbient(string name, byte[] file) {
		CurrentAmbience = name;
		Ambient = Sound.Load(Instance, file);
		Ambient.Loop = true;
		Ambient.Play(3);
		Sounds.Add(Ambient);
	}

	public void Update() {
		foreach (var sound in Sounds)
			sound.Update();
	}
}