public partial struct Transform {
	public Vector3 Position;
	public Vector3 Scale;
	public Rotation Rotation;

	public Transform() : this(default) {}
	public Transform(Vector3 pos = default) {
		Position = pos;
		Rotation = Rotation.Identity;
		Scale = 1.0f;
	}
	public Transform(Vector3 position, Rotation rotation, float scale = 1.0f) {
		Position = position;
		Rotation = rotation;
		Scale = scale;
	}
	public Transform(Vector3 position, Rotation rotation, Vector3 scale) {
		Position = position;
		Rotation = rotation;
		Scale = scale;
	}

	public readonly static Transform Zero = new(Vector3.Zero);

	public readonly Vector3 Forward => Rotation.Forward;
	public readonly Vector3 Backward => Rotation.Backward;
	public readonly Vector3 Up => Rotation.Up;
	public readonly Vector3 Down => Rotation.Down;
	public readonly Vector3 Right => Rotation.Right;
	public readonly Vector3 Left => Rotation.Left;

	internal readonly Vector3 SafeScale => new(Scale.x != 0f ? Scale.x : 1f, Scale.y != 0f ? Scale.y : 1f, Scale.z != 0f ? Scale.z : 1f);
	public readonly bool IsValid {
		get {
			if (Position.IsNaN) return false;
			if (Scale.IsNaN) return false;
			if (Rotation.x == 0 && Rotation.y == 0 && Rotation.z == 0 && Rotation.w == 0) return false;
			return true;
		}
	}

	public readonly Matrix4x4 ToMatrix() {
		var sca = Matrix4x4.CreateScale(Scale);
		var rot = Matrix4x4.CreateFromQuaternion(Rotation);
		var pos = Matrix4x4.CreateTranslation(Position);
		return sca * rot * pos;
	}

	public readonly Vector3 PointToLocal(in Vector3 worldPoint) => Rotation.Inverse * (worldPoint - Position) / SafeScale;
	public readonly Vector3 NormalToLocal(in Vector3 worldNormal) => (Rotation.Inverse * worldNormal).Normal;
	public readonly Rotation RotationToLocal(in Rotation worldRot) => Rotation.Inverse * worldRot;
	public readonly Vector3 PointToWorld(in Vector3 localPoint) => Position + (Rotation * (localPoint * Scale));
	public readonly Vector3 NormalToWorld(in Vector3 localNormal) => (Rotation * localNormal).Normal;
	public readonly Rotation RotationToWorld(in Rotation localRotation) => Rotation * localRotation;

	public Transform ToLocal(in Transform child) { //TODO optimize this?
		var rotInv = Rotation.Inverse;
		var localSafeScale = new Vector3(
			Scale.x != 0f ? child.Scale.x / Scale.x : child.Scale.x,
			Scale.y != 0f ? child.Scale.y / Scale.y : child.Scale.y,
			Scale.z != 0f ? child.Scale.z / Scale.z : child.Scale.z
		);
		return new Transform {
			Position = ((child.Position - Position) * rotInv) / SafeScale,
			Rotation = rotInv * child.Rotation,
			Scale = localSafeScale
		};
	}
	public readonly Transform ToWorld(in Transform child) {
		return new Transform {
			Position = (child.Position * Scale * Rotation) + Position,
			Rotation = Rotation * child.Rotation,
			Scale = Scale * child.Scale
		};
	}

	public readonly Transform LerpTo(in Transform target, float t, bool clamp = true) => Lerp(this, target, t, clamp);
	public static Transform Lerp(in Transform a, in Transform b, float t, bool clamp) {
		return new Transform {
			Position = Vector3.Lerp(a.Position, b.Position, t, clamp),
			Rotation = Rotation.Slerp(a.Rotation, b.Rotation, t, clamp),
			Scale = Vector3.Lerp(a.Scale, b.Scale, t, clamp),
		};
	}

	public readonly Transform Add(in Vector3 position, bool worldSpace) {
		var t = this;
		if (worldSpace)
			t.Position += position;
		else
			t.Position = PointToWorld(position);
		return t;
	}

	public readonly Transform WithPosition(in Vector3 position) {
		var t = this;
		t.Position = position;
		return t;
	}
	public readonly Transform WithRotation(in Rotation rotation) {
		var t = this;
		t.Rotation = rotation;
		return t;
	}
	public readonly Transform WithScale(float scale) {
		var t = this;
		t.Scale = scale;
		return t;
	}
	public readonly Transform WithScale(in Vector3 scale) {
		var t = this;
		t.Scale = scale;
		return t;
	}
	public readonly Transform With(in Vector3 position, in Rotation rotation) {
		var t = this;
		t.Position = position;
		t.Rotation = rotation;
		return t;
	}

	public readonly Transform RotateAround(in Vector3 center, in Rotation rot) {
		var t = this;
		var dir = t.Position - center;
		dir = rot * dir;
		t.Position = center + dir;
		var myRot = t.Rotation;
		t.Rotation *= myRot.Inverse * rot * myRot;
		return t;
	}

	public static Transform Concat(Transform parent, Transform local) {
		return new Transform {
			Position = parent.Position + parent.Scale * (parent.Rotation * local.Position),
			Scale = parent.Scale * local.Scale,
			Rotation = parent.Rotation * (parent.Rotation.Distance(local.Rotation) >= 180 ? local.Rotation.Inverse : local.Rotation),
		};
	}

	public override readonly string ToString() => $"pos {Position}, rot {Rotation}, scale {Scale}";
	public static Transform Parse(string str) {
		str = str.Trim('[', ']', ' ', '\n', '\r', '\t', '"');

		var p = str.Split([' ', ',', ';', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries );
		if (p.Length != 7)
			return default;

		return new Transform(new(p[0].ToFloat(), p[1].ToFloat(), p[2].ToFloat()), new(p[3].ToFloat(), p[4].ToFloat(), p[5].ToFloat(), p[6].ToFloat()));
	}

	public static bool operator ==( Transform left, Transform right ) => left.AlmostEqual( right );
	public static bool operator !=( Transform left, Transform right ) => !left.AlmostEqual( right );
	public readonly override bool Equals( object obj ) => obj is Transform o && Equals( o );
	public readonly bool Equals( Transform o ) => Position.Equals( o.Position ) && Scale.Equals( o.Scale ) && Rotation.Equals( o.Rotation );
	public readonly override int GetHashCode() => HashCode.Combine( Position, Scale, Rotation );

	public readonly bool AlmostEqual(in Transform tx, float delta = 0.0001f) => Position.AlmostEqual(tx.Position, delta) && Scale.AlmostEqual(tx.Scale, delta) && Rotation.AlmostEqual(tx.Rotation);
}