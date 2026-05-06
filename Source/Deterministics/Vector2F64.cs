// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Numerics;

namespace Jih.Unity.Infrastructure.Deterministics
{
    public partial struct Vector2F64 : IEquatable<Vector2F64>
    {
        public static Vector2F64 Add(in Vector2F64 left, in Vector2F64 right)
        {
            return new Vector2F64(left.X + right.X, left.Y + right.Y);
        }
        public static Vector2F64 Subtract(in Vector2F64 left, in Vector2F64 right)
        {
            return new Vector2F64(left.X - right.X, left.Y - right.Y);
        }

        public static Vector2F64 Multiply(in Vector2F64 left, in Vector2F64 right)
        {
            return new Vector2F64(left.X * right.X, left.Y * right.Y);
        }
        public static Vector2F64 Multiply(in Vector2F64 left, in F64 right)
        {
            return new Vector2F64(left.X * right, left.Y * right);
        }
        public static Vector2F64 Divide(in Vector2F64 left, in Vector2F64 right)
        {
            return new Vector2F64(left.X / right.X, left.Y / right.Y);
        }
        public static Vector2F64 Divide(in Vector2F64 left, in F64 right)
        {
            return new Vector2F64(left.X / right, left.Y / right);
        }

        public static Vector2F64 Abs(in Vector2F64 vector)
        {
            return new Vector2F64(FPMath.Abs(vector.X), FPMath.Abs(vector.Y));
        }

        public static Vector2F64 Min(in Vector2F64 left, in Vector2F64 right)
        {
            return new Vector2F64(FPMath.Min(left.X, right.X), FPMath.Min(left.Y, right.Y));
        }
        public static Vector2F64 Max(in Vector2F64 left, in Vector2F64 right)
        {
            return new Vector2F64(FPMath.Max(left.X, right.X), FPMath.Max(left.Y, right.Y));
        }

        public static Vector2F64 Clamp(in Vector2F64 value, in Vector2F64 min, in Vector2F64 max)
        {
            return new Vector2F64(FPMath.Clamp(value.X, min.X, max.X), FPMath.Clamp(value.Y, min.Y, max.Y));
        }

        public static Vector2F64 Lerp(in Vector2F64 a, in Vector2F64 b, in Vector2F64 t)
        {
            return new Vector2F64(FPMath.Lerp(a.X, b.X, t.X), FPMath.Lerp(a.Y, b.Y, t.Y));
        }
        public static Vector2F64 Lerp(in Vector2F64 a, in Vector2F64 b, in F64 t)
        {
            return new Vector2F64(FPMath.Lerp(a.X, b.X, t), FPMath.Lerp(a.Y, b.Y, t));
        }

        public static bool All(in Vector2F64 vector, F64 value)
        {
            return vector.X == value && vector.Y == value;
        }
        public static bool Any(in Vector2F64 vector, F64 value)
        {
            return vector.X == value || vector.Y == value;
        }

        public static F64 Cross(in Vector2F64 left, in Vector2F64 right)
        {
            return left.X * right.Y - left.Y * right.X;
        }
        public static F64 Dot(in Vector2F64 left, in Vector2F64 right)
        {
            return left.X * right.X + left.Y * right.Y;
        }

        public static F64 Distance(in Vector2F64 left, in Vector2F64 right)
        {
            return FPMath.Sqrt(DistanceSquared(left, right));
        }
        public static F64 DistanceSquared(in Vector2F64 left, in Vector2F64 right)
        {
            return Subtract(left, right).LengthSquared();
        }

        public F64 X, Y;

        public Vector2F64(F64 x, F64 y)
        {
            X = x;
            Y = y;
        }
        public Vector2F64(F64 uniform) : this(uniform, uniform)
        {
        }
        public Vector2F64(ReadOnlySpan<F64> values)
        {
            if (values.Length < 2)
            {
                throw new ArgumentException("Input values must contains at least 2 values.");
            }
            X = values[0];
            Y = values[1];
        }

        public readonly F64 Length()
        {
            return FPMath.Sqrt(LengthSquared());
        }
        public readonly F64 LengthSquared()
        {
            return Dot(this, this);
        }

        public readonly void CopyTo(Span<F64> buffer)
        {
            if (buffer.Length < 2)
            {
                throw new ArgumentException("Buffer size must be at least 2.");
            }
            buffer[0] = X;
            buffer[1] = Y;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is Vector2F64 f && Equals(f);
        }
        public readonly bool Equals(Vector2F64 other)
        {
            return X.Equals(other.X) &&
                   Y.Equals(other.Y);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public readonly override string ToString()
        {
            return $"<{X}, {Y}>";
        }

        /// <remarks>
        /// Deterministic-safe.
        /// </remarks>
        public static explicit operator Vector2F64(Vector2 vector)
        {
            return new Vector2F64((F64)vector.X, (F64)vector.Y);
        }
        /// <remarks>
        /// <b>NOT</b> deterministic-safe.
        /// </remarks>
        public static explicit operator Vector2(Vector2F64 vector)
        {
            return new Vector2((float)vector.X, (float)vector.Y);
        }

        public static Vector2F64 operator +(Vector2F64 left, Vector2F64 right)
        {
            return Add(left, right);
        }
        public static Vector2F64 operator -(Vector2F64 left, Vector2F64 right)
        {
            return Subtract(left, right);
        }
        public static Vector2F64 operator *(Vector2F64 left, Vector2F64 right)
        {
            return Multiply(left, right);
        }
        public static Vector2F64 operator *(Vector2F64 left, F64 right)
        {
            return Multiply(left, right);
        }
        public static Vector2F64 operator /(Vector2F64 left, Vector2F64 right)
        {
            return Divide(left, right);
        }
        public static Vector2F64 operator /(Vector2F64 left, F64 right)
        {
            return Divide(left, right);
        }

        public static Vector2F64 operator -(Vector2F64 vector)
        {
            return new Vector2F64(-vector.X, -vector.Y);
        }
        public static Vector2F64 operator +(Vector2F64 vector)
        {
            return vector;
        }

        public static bool operator ==(Vector2F64 left, Vector2F64 right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Vector2F64 left, Vector2F64 right)
        {
            return !(left == right);
        }

        public static Vector2F64 Zero => new(F64.Zero);
        public static Vector2F64 One => new(F64.One);

        public static Vector2F64 UnitX => new(F64.One, F64.Zero);
        public static Vector2F64 UnitY =>new(F64.Zero, F64.One);
    }
}
