// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jih.Unity.Infrastructure
{
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = sizeof(float))]
    public struct ScreenV : IEquatable<ScreenV>, IFormattable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV Add(ScreenV left, ScreenV right)
        {
            return new ScreenV(left.X + right.X, left.Y + right.Y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV Subtract(ScreenV left, ScreenV right)
        {
            return new ScreenV(left.X - right.X, left.Y - right.Y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV Multiply(ScreenV left, ScreenV right)
        {
            return new ScreenV(left.X * right.X, left.Y * right.Y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV Multiply(ScreenV left, float right)
        {
            return new ScreenV(left.X * right, left.Y * right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV Divide(ScreenV left, ScreenV right)
        {
            return new ScreenV(left.X / right.X, left.Y / right.Y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV Divide(ScreenV left, float right)
        {
            return new ScreenV(left.X / right, left.Y / right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV SafeDivide(ScreenV left, ScreenV right, ScreenV fallback = default)
        {
            return new ScreenV(
                right.X == 0f ? fallback.X : (left.X / right.X),
                right.Y == 0f ? fallback.Y : (left.Y / right.Y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV SafeDivide(ScreenV left, float right, ScreenV fallback = default)
        {
            if (right == 0f)
            {
                return fallback;
            }
            return Divide(left, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV Inverse(ScreenV left)
        {
            return new ScreenV(1f / left.X, 1f / left.Y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV SafeInverse(ScreenV left, ScreenV fallback = default)
        {
            return new ScreenV(
                left.X == 0f ? fallback.X : (1f / left.X),
                left.Y == 0f ? fallback.Y : (1f / left.Y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(ScreenV left, ScreenV right)
        {
            return left.X * right.X + left.Y * right.Y;
        }
        /// <summary>
        /// <c>AB × BC</c>
        /// </summary>
        /// <returns>
        /// Positive: ABC in CW order.<br/>
        /// Negative: ABC in CCW order.<br/>
        /// 0: ABC are collinear.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cross(ScreenV a, ScreenV b, ScreenV c)
        {
            return (b.X - a.X) * (c.Y - b.Y) - (b.Y - a.Y) * (c.X - b.X);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(ScreenV left, ScreenV right)
        {
            return Subtract(left, right).Length();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceSquared(ScreenV left, ScreenV right)
        {
            return Subtract(left, right).LengthSquared();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV Normalize(ScreenV left)
        {
            return Divide(left, left.Length());
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV SafeNormalize(ScreenV left, float tolerance = NormalizeTolerance, ScreenV fallback = default)
        {
            float length = left.LengthSquared();
            if (length < tolerance)
            {
                return fallback;
            }
            return Divide(left, MathF.Sqrt(length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV Negate(ScreenV left)
        {
            return new ScreenV(-left.X, -left.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV Abs(ScreenV left)
        {
            return new ScreenV(Math.Abs(left.X), Math.Abs(left.Y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV Min(ScreenV left, ScreenV right)
        {
            return new ScreenV(Math.Min(left.X, right.X), Math.Min(left.Y, right.Y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV Max(ScreenV left, ScreenV right)
        {
            return new ScreenV(Math.Max(left.X, right.X), Math.Max(left.Y, right.Y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV Clamp(ScreenV value, ScreenV min, ScreenV max)
        {
            return new ScreenV(Math.Clamp(value.X, min.X, max.X), Math.Clamp(value.Y, min.Y, max.Y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV Clamp(ScreenV value, float min, float max)
        {
            return new ScreenV(Math.Clamp(value.X, min, max), Math.Clamp(value.Y, min, max));
        }
        /// <summary>
        /// Unclampped.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV Lerp(ScreenV start, ScreenV end, ScreenV alpha)
        {
            return new ScreenV(MathEx.Lerp(start.X, end.X, alpha.X), MathEx.Lerp(start.Y, end.Y, alpha.Y));
        }
        /// <inheritdoc cref="Lerp(ScreenV, ScreenV, ScreenV)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV Lerp(ScreenV start, ScreenV end, float alpha)
        {
            return new ScreenV(MathEx.Lerp(start.X, end.X, alpha), MathEx.Lerp(start.Y, end.Y, alpha));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV Transform(ScreenV point, in ScreenM matrix)
        {
            return new ScreenV(
                point.X * matrix.M11 + point.Y * matrix.M21 + matrix.M31,
                point.X * matrix.M12 + point.Y * matrix.M22 + matrix.M32
            );
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV TransformNormal(ScreenV normal, in ScreenM matrix)
        {
            return new ScreenV(
                normal.X * matrix.M11 + normal.Y * matrix.M21,
                normal.X * matrix.M12 + normal.Y * matrix.M22
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV RotateCw90(ScreenV left)
        {
            return new ScreenV(-left.Y, left.X);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV RotateCcw90(ScreenV left)
        {
            return new ScreenV(left.Y, -left.X);
        }

        public static ScreenV Up => new(0f, -1f);
        public static ScreenV Down => new(0f, 1f);
        public static ScreenV Left => new(-1f, 0f);
        public static ScreenV Right => new(1f, 0f);

        public static ScreenV Zero => new(0f);
        public static ScreenV One => new(1f);

        [UnityEngine.SerializeField] public float X;
        [UnityEngine.SerializeField] public float Y;

        public float this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => index switch
            {
                0 => X,
                1 => Y,
                _ => throw new ArgumentOutOfRangeException(nameof(index)),
            };
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        public ScreenV(float uniform) : this(uniform, uniform)
        {
        }
        public ScreenV(float x, float y)
        {
            X = x;
            Y = y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float Length()
        {
            return MathF.Sqrt(LengthSquared());
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float LengthSquared()
        {
            return X * X + Y * Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ScreenV SafeDivide(ScreenV right, ScreenV fallback = default)
        {
            return SafeDivide(this, right, fallback);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ScreenV SafeDivide(float right, ScreenV fallback = default)
        {
            return SafeDivide(this, right, fallback);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ScreenV Inverse()
        {
            return Inverse(this);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ScreenV SafeInverse(ScreenV fallback = default)
        {
            return SafeInverse(this, fallback);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float Dot(ScreenV right)
        {
            return Dot(this, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float Distance(ScreenV right)
        {
            return Distance(this, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float DistanceSquared(ScreenV right)
        {
            return DistanceSquared(this, right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ScreenV Normalize()
        {
            return Normalize(this);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ScreenV SafeNormalize(float tolerance = NormalizeTolerance, ScreenV fallback = default)
        {
            return SafeNormalize(this, tolerance, fallback);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ScreenV Abs()
        {
            return Abs(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ScreenV Min(ScreenV right)
        {
            return Min(this, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ScreenV Max(ScreenV right)
        {
            return Max(this, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ScreenV Clamp(ScreenV min, ScreenV max)
        {
            return Clamp(this, min, max);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ScreenV Clamp(float min, float max)
        {
            return Clamp(this, min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ScreenV RotateCw90()
        {
            return RotateCw90(this);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ScreenV RotateCcw90()
        {
            return RotateCcw90(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ScreenV Transform(in ScreenM m)
        {
            return Transform(this, in m);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ScreenV TransformNormal(in ScreenM m)
        {
            return TransformNormal(this, in m);
        }

        public readonly override string ToString()
        {
            return ToString(null, null);
        }
        public readonly string ToString(string? format)
        {
            return ToString(format, null);
        }
        public readonly string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "F6";
            }

            formatProvider ??= CultureInfo.InvariantCulture.NumberFormat;

            return $"<{X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)}>";
        }

        public readonly void CopyTo(Span<float> dest)
        {
            if (dest.Length < 2)
            {
                throw new ArgumentException("Dest length is too short.");
            }
            dest[0] = X;
            dest[1] = Y;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is ScreenV coord && Equals(coord);
        }
        public readonly bool Equals(ScreenV other)
        {
            return X == other.X &&
                   Y == other.Y;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV operator +(ScreenV left, ScreenV right)
        {
            return Add(left, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV operator -(ScreenV left, ScreenV right)
        {
            return Subtract(left, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV operator *(ScreenV left, ScreenV right)
        {
            return Multiply(left, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV operator *(ScreenV left, float right)
        {
            return Multiply(left, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV operator /(ScreenV left, ScreenV right)
        {
            return Divide(left, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV operator /(ScreenV left, float right)
        {
            return Divide(left, right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV operator -(ScreenV left)
        {
            return Negate(left);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV operator +(ScreenV left)
        {
            return left;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenV operator *(ScreenV point, in ScreenM matrix)
        {
            return Transform(point, matrix);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(ScreenV left, ScreenV right)
        {
            return left.Equals(right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(ScreenV left, ScreenV right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScreenV(UnityEngine.Vector2 v)
        {
            return new ScreenV(v.x, v.y);
        }
        /// <remarks>
        /// This operator is actually loseless.<br/>
        /// <br/>
        /// By the way, the Unity's <c>Vector2</c> can be converted to <c>Vector3</c> implicitly and vice versa.<br/>
        /// Then, the <c>Vector3</c> can lose data silently when converted to <c>Vector2</c>. Because it is implicit.<br/>
        /// Therefore, preserving the 2D screen data by <c>ScreenCoord</c> as long as possible recommended.<br/>
        /// So this operator is explicit.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator UnityEngine.Vector2(ScreenV v)
        {
            return new UnityEngine.Vector2(v.X, v.Y);
        }

        const float NormalizeTolerance = 0.0001f;
    }
}
