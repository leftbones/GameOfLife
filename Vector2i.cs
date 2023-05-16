using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Numerics;

namespace GOL;

[System.Serializable]
public struct Vector2i : IEquatable<Vector2i> {
	public int X;
	public int Y;

	// Constructors
	public Vector2i(int value) {
		X = value;
		Y = value;
	}

	public Vector2i(int x, int y) {
		X = x;
		Y = y;
	}

	public Vector2i(Vector2 vec2) {
		X = (int)vec2.X;
		Y = (int)vec2.Y;
	}

	// Access
	public int this[int index] {
		get {
			if (index == 0) return X;
			if (index == 1) return Y;

			throw new IndexOutOfRangeException("Tried to access this Vector2i at index: " + index);
		}

		set {
			if (index == 0) {
				X = value;
			} else if (index == 1) {
				Y = value;
			} else {
				throw new IndexOutOfRangeException("Tried to set this Vector2i at index: " + index);
			}
		}
	}

	// Distance
	public int ManhattanDistance => Math.Abs(X) + Math.Abs(Y);
	public int EuclideanDistance => (int)Math.Sqrt((X * X) + (Y * Y));

	// Zero / One
	public static readonly Vector2i Zero = new Vector2i(0, 0);
	public static readonly Vector2i One = new Vector2i(1, 1);


	////
	//Basic Math

	// Add
	public static Vector2i Add(Vector2i a, Vector2i b) {
		a.X += b.X;
		a.Y += b.Y;
		return a;
	}

	public static Vector2i Add(Vector2i vec, int n) {
		vec.X += n;
		vec.Y += n;
		return vec;
	}

	// Subtract
	public static Vector2i Subtract(Vector2i a, Vector2i b) {
		a.X -= b.X;
		a.Y -= b.Y;
		return a;
	}

	public static Vector2i Subtract(Vector2i vec, int n) {
		vec.X -= n;
		vec.Y -= n;
		return vec;
	}

	// Multiply
	public static Vector2i Multiply(Vector2i a, Vector2i b) {
		a.X *= b.X;
		a.Y *= b.Y;
		return a;
	}

	public static Vector2i Multiply(Vector2i vec, int n) {
		vec.X *= n;
		vec.Y *= n;
		return vec;
	}

	// Divide
	public static Vector2i Divide(Vector2i a, Vector2i b) {
		a.X /= b.X;
		a.Y /= b.Y;
		return a;
	}

	public static Vector2i Divide(Vector2i vec, int n) {
		vec.X /= n;
		vec.Y /= n;
		return vec;
	}

	// Clamp
	public static Vector2i Clamp(Vector2i vec, int min, int max) {
		vec.X = Math.Clamp(vec.X, min, max);
		vec.Y = Math.Clamp(vec.Y, min, max);
		return vec;
	}

	public static Vector2i ClampX(Vector2i vec, int min, int max) {
		vec.X = Math.Clamp(vec.X, min, max);
		return vec;
	}

	public static Vector2i ClampY(Vector2i vec, int min, int max) {
		vec.Y = Math.Clamp(vec.Y, min, max);
		return vec;
	}


	///
	// Operator Math

	// Add
	[Pure]
	public static Vector2i operator +(Vector2i left, Vector2i right) {
		left.X += right.X;
		left.Y += right.Y;
		return left;
	}

	// Subtract
	[Pure]
	public static Vector2i operator -(Vector2i left, Vector2i right) {
		left.X -= right.X;
		left.Y -= right.Y;
		return left;
	}

	// Multiply
	[Pure]
	public static Vector2i operator *(Vector2i left, Vector2i right) {
		left.X *= right.X;
		left.Y *= right.Y;
		return left;
	}

	[Pure]
	public static Vector2i operator *(Vector2i vec, int scale) {
		vec.X *= scale;
		vec.Y *= scale;
		return vec;
	}

	// Divide
	[Pure]
	public static Vector2i operator /(Vector2i left, Vector2i right) {
		left.X /= right.X;
		left.Y /= right.Y;
		return left;
	}

	[Pure]
	public static Vector2i operator /(Vector2i vec, int scale) {
		vec.X /= scale;
		vec.Y /= scale;
		return vec;
	}

	// Negate
	[Pure]
	public static Vector2i operator -(Vector2i vec) {
		vec.X = -vec.X;
		vec.Y = -vec.Y;
		return vec;
	}

	// Equals
	public static bool operator ==(Vector2i left, Vector2i right) {
		return left.Equals(right);
	}

	public static bool operator !=(Vector2i left, Vector2i right) {
		return !(left == right);
	}


	////
	// Overrides + Other

	public override string ToString() {
		return string.Format("({0}, {1})", X, Y);
	}

	public override bool Equals([NotNullWhen(true)] object? obj) {
		return obj is Vector2i && base.Equals(obj);
	}

	public override int GetHashCode() {
		return HashCode.Combine(X, Y);
	}

	public bool Equals(Vector2i other) {
		return X == other.X && Y == other.Y;
	}

	public Vector2 ToVector2() {
		return new Vector2(X, Y);
	}
}