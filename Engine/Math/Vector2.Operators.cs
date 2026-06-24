using System.Runtime.CompilerServices;

public partial struct Vector2 {
	public float this[int index] {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get => _vec[index];
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => _vec[index] = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 operator +(Vector2 c1, Vector2 c2) => c1._vec + c2._vec;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 operator -(Vector2 c1, Vector2 c2) => c1._vec - c2._vec;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 operator -(Vector2 c1) => System.Numerics.Vector2.Negate(c1._vec);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 operator *(Vector2 c1, float f) => c1._vec * f;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 operator *(float f, Vector2 c1) => c1._vec * f;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 operator *(Vector2 c1, Vector2 c2) => c1._vec * c2._vec;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 operator /(Vector2 c1, Vector2 c2) => c1._vec / c2._vec;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 operator /(Vector2 c1, float c2) => c1._vec / c2;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Vector2(System.Numerics.Vector2 value) => new(value);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator System.Numerics.Vector2(Vector2 value) => new(value.x, value.y);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Vector2(double value) => new((float)value, (float)value);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Vector2(Vector3 value) => new(value);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Vector2(Vector4 value) => new(value);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Vector2(Silk.NET.Maths.Vector2D<int> value) => new(value.X, value.Y);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Vector2 left, Vector2 right) => left.Equals(right);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Vector2 left, Vector2 right) => !(left == right);
	public override readonly bool Equals(object obj) => obj is Vector2 o && Equals(o);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(Vector2 o) => (_vec) == (o._vec);
	public readonly override int GetHashCode() => _vec.GetHashCode();
}