// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jih.Unity.Infrastructure.Numerics
{
    /// <seealso cref="ScreenSpaceConvert"/>
    /// <inheritdoc cref="ScreenSpaceConvert"/>
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = sizeof(float))]
    public struct ScreenM : IEquatable<ScreenM>, IFormattable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenM Multiply(in ScreenM left, in ScreenM right)
        {
            return new ScreenM(
                left.M11 * right.M11 + left.M12 * right.M21,
                left.M11 * right.M12 + left.M12 * right.M22,
                left.M21 * right.M11 + left.M22 * right.M21,
                left.M21 * right.M12 + left.M22 * right.M22,
                left.M31 * right.M11 + left.M32 * right.M21 + right.M31,
                left.M31 * right.M12 + left.M32 * right.M22 + right.M32
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenM CreateTranslation(ScreenV translation)
        {
            return new ScreenM(1f, 0f, 0f, 1f, translation.X, translation.Y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenM CreateTranslation(float x, float y)
        {
            return new ScreenM(1f, 0f, 0f, 1f, x, y);
        }

        /// <summary>
        /// Creates a rotation matrix.
        /// Positive radians will rotate clockwise(CW) in a Y-down screen space.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenM CreateRotation(float radians)
        {
            float c = MathF.Cos(radians);
            float s = MathF.Sin(radians);
            return new ScreenM(c, s, -s, c, 0f, 0f);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenM CreateRotation(float radians, ScreenV centerPoint)
        {
            float c = MathF.Cos(radians);
            float s = MathF.Sin(radians);
            float x = centerPoint.X * (1f - c) + centerPoint.Y * s;
            float y = centerPoint.Y * (1f - c) - centerPoint.X * s;
            return new ScreenM(c, s, -s, c, x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenM CreateScale(ScreenV scale)
        {
            return new ScreenM(scale.X, 0f, 0f, scale.Y, 0f, 0f);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenM CreateScale(float scaleX, float scaleY)
        {
            return new ScreenM(scaleX, 0f, 0f, scaleY, 0f, 0f);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenM CreateScale(ScreenV scale, ScreenV centerPoint)
        {
            float tx = centerPoint.X * (1f - scale.X);
            float ty = centerPoint.Y * (1f - scale.Y);
            return new ScreenM(scale.X, 0f, 0f, scale.Y, tx, ty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenM CreateSRT(ScreenV translation, float rotationRadians, ScreenV scale)
        {
            float c = MathF.Cos(rotationRadians);
            float s = MathF.Sin(rotationRadians);

            return new ScreenM(
                scale.X * c, scale.X * s,
                -scale.Y * s, scale.Y * c,
                translation.X, translation.Y
            );
        }

        public static bool Invert(in ScreenM matrix, out ScreenM result)
        {
            float det = matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21;
            if (MathF.Abs(det) < float.Epsilon)
            {
                result = new ScreenM(float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN);
                return false;
            }

            float invDet = 1f / det;
            result = new ScreenM(
                matrix.M22 * invDet,
                -matrix.M12 * invDet,
                -matrix.M21 * invDet,
                matrix.M11 * invDet,
                (matrix.M21 * matrix.M32 - matrix.M22 * matrix.M31) * invDet,
                (matrix.M12 * matrix.M31 - matrix.M11 * matrix.M32) * invDet
            );
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenM SafeInvert(in ScreenM matrix)
        {
            if (!Invert(in matrix, out ScreenM result))
            {
                return Identity;
            }
            return result;
        }

        public static ScreenM Identity => new(1f, 0f, 0f, 1f, 0f, 0f);

        [UnityEngine.SerializeField] public float M11;
        [UnityEngine.SerializeField] public float M12;
        [UnityEngine.SerializeField] public float M21;
        [UnityEngine.SerializeField] public float M22;
        [UnityEngine.SerializeField] public float M31;
        [UnityEngine.SerializeField] public float M32;

        public readonly bool IsIdentity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => M11 == 1f && M22 == 1f &&
                   M12 == 0f && M21 == 0f &&
                   M31 == 0f && M32 == 0f;
        }

        public ScreenV Translation
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => new(M31, M32);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                M31 = value.X;
                M32 = value.Y;
            }
        }

        public ScreenM(float m11, float m12, float m21, float m22, float m31, float m32)
        {
            M11 = m11; M12 = m12;
            M21 = m21; M22 = m22;
            M31 = m31; M32 = m32;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ScreenM SafeInvert()
        {
            return SafeInvert(this);
        }

        public readonly bool Decompose(out ScreenV translation, out float rotation, out ScreenV scale)
        {
            translation = new ScreenV(M31, M32);

            float scaleX = MathF.Sqrt(M11 * M11 + M12 * M12);
            float scaleY = MathF.Sqrt(M21 * M21 + M22 * M22);

            if (scaleX.IsNearlyZero() || scaleY.IsNearlyZero())
            {
                scale = new ScreenV(scaleX, scaleY);
                rotation = 0f;
                return false;
            }

            float det = M11 * M22 - M12 * M21;
            if (det < 0f)
            {
                scaleY = -scaleY;
            }

            scale = new ScreenV(scaleX, scaleY);
            rotation = MathF.Atan2(M12, M11);

            return true;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is ScreenM other && Equals(other);
        }
        public readonly bool Equals(ScreenM other)
        {
            return M11 == other.M11 && M12 == other.M12 &&
                   M21 == other.M21 && M22 == other.M22 &&
                   M31 == other.M31 && M32 == other.M32;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(M11, M12, M21, M22, M31, M32);
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

            return $"[ [{M11.ToString(format, formatProvider)}, {M12.ToString(format, formatProvider)}] " +
                   $"[{M21.ToString(format, formatProvider)}, {M22.ToString(format, formatProvider)}] " +
                   $"[{M31.ToString(format, formatProvider)}, {M32.ToString(format, formatProvider)}] ]";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScreenM operator *(in ScreenM left, in ScreenM right)
        {
            return Multiply(left, right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in ScreenM left, in ScreenM right)
        {
            return left.Equals(right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in ScreenM left, in ScreenM right)
        {
            return !(left == right);
        }
    }
}
