using System.Drawing;
using System.Runtime.CompilerServices;

public partial struct Vector3 {
	public float this[int index] {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get => _vec[index];
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => _vec[index] = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 operator +(Vector3 c1, Vector3 c2) => System.Numerics.Vector3.Add(c1._vec, c2._vec);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 operator -(Vector3 c1, Vector3 c2) => System.Numerics.Vector3.Subtract(c1._vec, c2._vec);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 operator *(Vector3 c1, float f) => System.Numerics.Vector3.Multiply(c1._vec, f);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 operator *(Vector3 c1, Rotation f) => System.Numerics.Vector3.Transform(c1._vec, f._quat);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 operator *(Vector3 c1, Vector3 c2) => System.Numerics.Vector3.Multiply(c1._vec, c2._vec);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 operator *(float f, Vector3 c1) => System.Numerics.Vector3.Multiply(f, c1._vec);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 operator /(Vector3 c1, float f) => System.Numerics.Vector3.Divide(c1._vec, f);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 operator /(Vector3 c1, in Vector3 c2) => System.Numerics.Vector3.Divide(c1._vec, c2._vec);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 operator -(Vector3 value) => System.Numerics.Vector3.Negate(value._vec);

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//TODO public static implicit operator Vector3(Color value) => new(value.r, value.g, value.b);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Vector3(float value) => new(value, value, value);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Vector3(Vector2 value) => new(value.x, value.y, 0.0f);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Vector3(System.Numerics.Vector3 value) => new() {_vec = value};
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator System.Numerics.Vector3(Vector3 value) => new(value.x, value.y, value.z);
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//TODO public static implicit operator Vector3(Vector4 vec) => new((float)vec.x, (float)vec.y, (float)vec.z);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Vector3 left, Vector3 right) => left.AlmostEqual(right);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Vector3 left, Vector3 right) => !left.AlmostEqual(right);
	public override readonly bool Equals(object obj) => obj is Vector3 o && Equals(o);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(Vector3 o) => _vec.Equals(o._vec);
	public readonly override int GetHashCode() => _vec.GetHashCode();
}