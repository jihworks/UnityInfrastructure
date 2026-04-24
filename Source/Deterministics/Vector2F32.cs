// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using UnityEngine;

namespace Jih.Unity.Infrastructure.Deterministics
{
    public struct Vector2F32 : IEquatable<Vector2F32>
    {
        public static Vector2F32 Add(in Vector2F32 left, in Vector2F32 right)
        {
            return new Vector2F32(left.X + right.X, left.Y + right.Y);
        }
        public static Vector2F32 Subtract(in Vector2F32 left, in Vector2F32 right)
        {
            return new Vector2F32(left.X - right.X, left.Y - right.Y);
        }

        public static Vector2F32 Multiply(in Vector2F32 left, in Vector2F32 right)
        {
            return new Vector2F32(left.X * right.X, left.Y * right.Y);
        }
        public static Vector2F32 Multiply(in Vector2F32 left, in F32 right)
        {
            return new Vector2F32(left.X * right, left.Y * right);
        }
        public static Vector2F32 Divide(in Vector2F32 left, in Vector2F32 right)
        {
            return new Vector2F32(left.X / right.X, left.Y / right.Y);
        }
        public static Vector2F32 Divide(in Vector2F32 left, in F32 right)
        {
            return new Vector2F32(left.X / right, left.Y / right);
        }

        public static Vector2F32 Abs(in Vector2F32 vector)
        {
            return new Vector2F32(FPMath.Abs(vector.X), FPMath.Abs(vector.Y));
        }

        public static Vector2F32 Min(in Vector2F32 left, in Vector2F32 right)
        {
            return new Vector2F32(FPMath.Min(left.X, right.X), FPMath.Min(left.Y, right.Y));
        }
        public static Vector2F32 Max(in Vector2F32 left, in Vector2F32 right)
        {
            return new Vector2F32(FPMath.Max(left.X, right.X), FPMath.Max(left.Y, right.Y));
        }

        public static Vector2F32 Clamp(in Vector2F32 value, in Vector2F32 min, in Vector2F32 max)
        {
            return new Vector2F32(FPMath.Clamp(value.X, min.X, max.X), FPMath.Clamp(value.Y, min.Y, max.Y));
        }

        public static Vector2F32 Lerp(in Vector2F32 a, in Vector2F32 b, in Vector2F32 t)
        {
            return new Vector2F32(FPMath.Lerp(a.X, b.X, t.X), FPMath.Lerp(a.Y, b.Y, t.Y));
        }
        public static Vector2F32 Lerp(in Vector2F32 a, in Vector2F32 b, in F32 t)
        {
            return new Vector2F32(FPMath.Lerp(a.X, b.X, t), FPMath.Lerp(a.Y, b.Y, t));
        }

        public static bool All(in Vector2F32 vector, F32 value)
        {
            return vector.X == value && vector.Y == value;
        }
        public static bool Any(in Vector2F32 vector, F32 value)
        {
            return vector.X == value || vector.Y == value;
        }

        public static F32 Cross(in Vector2F32 left, in Vector2F32 right)
        {
            return left.X * right.Y - left.Y * right.X; ;
        }
        public static F32 Dot(in Vector2F32 left, in Vector2F32 right)
        {
            return left.X * right.X + left.Y * right.Y;
        }

        public static F32 Distance(in Vector2F32 left, in Vector2F32 right)
        {
            return FPMath.Sqrt(DistanceSquared(left, right));
        }
        public static F32 DistanceSquared(in Vector2F32 left, in Vector2F32 right)
        {
            return Subtract(left, right).LengthSquared();
        }

        public F32 X, Y;

        public Vector2F32(ReadOnlySpan<F32> values)
        {
            if (values.Length < 2)
            {
                throw new ArgumentException("Input values must contains at least 2 values.");
            }
            X = values[0];
            Y = values[1];
        }
        public Vector2F32(F32 uniform) : this(uniform, uniform)
        {
        }
        public Vector2F32(F32 x, F32 y)
        {
            X = x;
            Y = y;
        }

        public readonly F32 Length()
        {
            return FPMath.Sqrt(LengthSquared());
        }
        public readonly F32 LengthSquared()
        {
            return Dot(this, this);
        }

        public readonly void CopyTo(Span<F32> buffer)
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
            return obj is Vector2F32 f && Equals(f);
        }
        public readonly bool Equals(Vector2F32 other)
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

        public static explicit operator Vector2F32(Vector2 vector)
        {
            return new Vector2F32((F32)vector.x, (F32)vector.y);
        }
        public static explicit operator Vector2(Vector2F32 vector)
        {
            return new Vector2((float)vector.X, (float)vector.Y);
        }

        public static Vector2F32 operator +(Vector2F32 left, Vector2F32 right)
        {
            return Add(left, right);
        }
        public static Vector2F32 operator -(Vector2F32 left, Vector2F32 right)
        {
            return Subtract(left, right);
        }
        public static Vector2F32 operator *(Vector2F32 left, Vector2F32 right)
        {
            return Multiply(left, right);
        }
        public static Vector2F32 operator *(Vector2F32 left, F32 right)
        {
            return Multiply(left, right);
        }
        public static Vector2F32 operator /(Vector2F32 left, Vector2F32 right)
        {
            return Divide(left, right);
        }
        public static Vector2F32 operator /(Vector2F32 left, F32 right)
        {
            return Divide(left, right);
        }

        public static Vector2F32 operator -(Vector2F32 vector)
        {
            return new Vector2F32(-vector.X, -vector.Y);
        }
        public static Vector2F32 operator +(Vector2F32 vector)
        {
            return vector;
        }

        public static bool operator ==(Vector2F32 left, Vector2F32 right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Vector2F32 left, Vector2F32 right)
        {
            return !(left == right);
        }

        public static Vector2F32 Zero => new(F32.Zero);
        public static Vector2F32 One => new(F32.One);

        public static Vector2F32 UnitX => new(F32.One, F32.Zero);
        public static Vector2F32 UnitY =>new(F32.Zero, F32.One);
    }
}
