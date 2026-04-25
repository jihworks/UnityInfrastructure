// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using UnityEngine;

namespace Jih.Unity.Infrastructure.Deterministics
{
    public struct Vector3F64
    {
        public static Vector3F64 Add(in Vector3F64 left, in Vector3F64 right)
        {
            return new Vector3F64(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }
        public static Vector3F64 Subtract(in Vector3F64 left, in Vector3F64 right)
        {
            return new Vector3F64(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static Vector3F64 Multiply(in Vector3F64 left, in Vector3F64 right)
        {
            return new Vector3F64(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
        }
        public static Vector3F64 Multiply(in Vector3F64 left, in F64 right)
        {
            return new Vector3F64(left.X * right, left.Y * right, left.Z * right);
        }
        public static Vector3F64 Divide(in Vector3F64 left, in Vector3F64 right)
        {
            return new Vector3F64(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
        }
        public static Vector3F64 Divide(in Vector3F64 left, in F64 right)
        {
            return new Vector3F64(left.X / right, left.Y / right, left.Z / right);
        }

        public static Vector3F64 Abs(in Vector3F64 vector)
        {
            return new Vector3F64(FPMath.Abs(vector.X), FPMath.Abs(vector.Y), FPMath.Abs(vector.Z));
        }

        public static Vector3F64 Min(in Vector3F64 left, in Vector3F64 right)
        {
            return new Vector3F64(FPMath.Min(left.X, right.X), FPMath.Min(left.Y, right.Y), FPMath.Min(left.Z, right.Z));
        }
        public static Vector3F64 Max(in Vector3F64 left, in Vector3F64 right)
        {
            return new Vector3F64(FPMath.Max(left.X, right.X), FPMath.Max(left.Y, right.Y), FPMath.Max(left.Z, right.Z));
        }

        public static Vector3F64 Clamp(in Vector3F64 value, in Vector3F64 min, in Vector3F64 max)
        {
            return new Vector3F64(FPMath.Clamp(value.X, min.X, max.X), FPMath.Clamp(value.Y, min.Y, max.Y), FPMath.Clamp(value.Z, min.Z, max.Z));
        }

        public static Vector3F64 Lerp(in Vector3F64 a, in Vector3F64 b, in Vector3F64 t)
        {
            return new Vector3F64(FPMath.Lerp(a.X, b.X, t.X), FPMath.Lerp(a.Y, b.Y, t.Y), FPMath.Lerp(a.Z, b.Z, t.Z));
        }
        public static Vector3F64 Lerp(in Vector3F64 a, in Vector3F64 b, in F64 t)
        {
            return new Vector3F64(FPMath.Lerp(a.X, b.X, t), FPMath.Lerp(a.Y, b.Y, t), FPMath.Lerp(a.Z, b.Z, t));
        }

        public static bool All(in Vector3F64 vector, F64 value)
        {
            return vector.X == value && vector.Y == value && vector.Z == value;
        }
        public static bool Any(in Vector3F64 vector, F64 value)
        {
            return vector.X == value || vector.Y == value || vector.Z == value;
        }

        public static Vector3F64 Cross(in Vector3F64 left, in Vector3F64 right)
        {
            return new Vector3F64(
                left.Y * right.Z - left.Z * right.Y,
                left.Z * right.X - left.X * right.Z,
                left.X * right.Y - left.Y * right.X);
        }
        public static F64 Dot(in Vector3F64 left, in Vector3F64 right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }

        public static F64 Distance(in Vector3F64 left, in Vector3F64 right)
        {
            return FPMath.Sqrt(DistanceSquared(left, right));
        }
        public static F64 DistanceSquared(in Vector3F64 left, in Vector3F64 right)
        {
            return Subtract(left, right).LengthSquared();
        }

        public F64 X, Y, Z;

        public Vector3F64(F64 x, F64 y, F64 z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Vector3F64(F64 uniform) : this(uniform, uniform, uniform)
        {
        }
        public Vector3F64(ReadOnlySpan<F64> values)
        {
            if (values.Length < 3)
            {
                throw new ArgumentException("Input values must contains at least 3 values.");
            }
            X = values[0];
            Y = values[1];
            Z = values[2];
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
            if (buffer.Length < 3)
            {
                throw new ArgumentException("Buffer size must be at least 3.");
            }
            buffer[0] = X;
            buffer[1] = Y;
            buffer[2] = Z;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is Vector3F64 f && Equals(f);
        }
        public readonly bool Equals(Vector3F64 other)
        {
            return X.Equals(other.X) &&
                   Y.Equals(other.Y) &&
                   Z.Equals(other.Z);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public readonly override string ToString()
        {
            return $"<{X}, {Y}, {Z}>";
        }

        /// <remarks>
        /// Deterministic-safe.
        /// </remarks>
        public static explicit operator Vector3F64(Vector3 vector)
        {
            return new Vector3F64((F64)vector.x, (F64)vector.y, (F64)vector.z);
        }
        /// <remarks>
        /// <b>NOT</b> deterministic-safe.
        /// </remarks>
        public static explicit operator Vector3(Vector3F64 vector)
        {
            return new Vector3((float)vector.X, (float)vector.Y, (float)vector.Z);
        }

        public static Vector3F64 operator +(Vector3F64 left, Vector3F64 right)
        {
            return Add(left, right);
        }
        public static Vector3F64 operator -(Vector3F64 left, Vector3F64 right)
        {
            return Subtract(left, right);
        }
        public static Vector3F64 operator *(Vector3F64 left, Vector3F64 right)
        {
            return Multiply(left, right);
        }
        public static Vector3F64 operator *(Vector3F64 left, F64 right)
        {
            return Multiply(left, right);
        }
        public static Vector3F64 operator /(Vector3F64 left, Vector3F64 right)
        {
            return Divide(left, right);
        }
        public static Vector3F64 operator /(Vector3F64 left, F64 right)
        {
            return Divide(left, right);
        }

        public static Vector3F64 operator -(Vector3F64 vector)
        {
            return new Vector3F64(-vector.X, -vector.Y, -vector.Z);
        }
        public static Vector3F64 operator +(Vector3F64 vector)
        {
            return vector;
        }

        public static bool operator ==(Vector3F64 left, Vector3F64 right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Vector3F64 left, Vector3F64 right)
        {
            return !(left == right);
        }

        public static Vector3F64 Zero => new(F64.Zero);
        public static Vector3F64 One => new(F64.One);

        public static Vector3F64 UnitX => new(F64.One, F64.Zero, F64.Zero);
        public static Vector3F64 UnitY => new(F64.Zero, F64.One, F64.Zero);
        public static Vector3F64 UnitZ => new(F64.Zero, F64.Zero, F64.One);
    }
}
