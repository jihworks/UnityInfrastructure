// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Runtime.CompilerServices;

namespace Jih.Unity.Infrastructure.Deterministics
{
    public static class FPMath
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F64 Sqrt(this F64 value)
        {
            if (value.RawValue <= 0)
            {
                return F64.Zero;
            }

            ulong numHi = (ulong)value.RawValue;

            ulong res = 0;
            ulong rem = 0;
            for (int i = 0; i < 48; i++)
            {
                rem = (rem << 2) | (numHi >> 62);

                numHi <<= 2;

                ulong test = (res << 2) | 1UL;
                if (rem >= test)
                {
                    rem -= test;
                    res = (res << 1) | 1UL;
                }
                else
                {
                    res <<= 1;
                }
            }

            return F64.FromRaw((long)res);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F64 Min(F64 left, F64 right)
        {
            return left.RawValue < right.RawValue ? left : right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Min(ref F64 left, F64 right)
        {
            left = Min(left, right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F64 Max(F64 left, F64 right)
        {
            return left.RawValue > right.RawValue ? left : right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Max(ref F64 left, F64 right)
        {
            left = Max(left, right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F64 Clamp(this F64 value, F64 min, F64 max)
        {
            if (value.RawValue < min.RawValue)
            {
                return min;
            }
            if (value.RawValue > max.RawValue)
            {
                return max;
            }
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clamp(ref F64 value, F64 min, F64 max)
        {
            value = Clamp(value, min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F64 Clamp01(this F64 value)
        {
            return Clamp(value, F64.Zero, F64.One);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clamp01(ref F64 value)
        {
            value = Clamp(value, F64.Zero, F64.One);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F64 Abs(this F64 value)
        {
            return value.RawValue < 0 ? F64.FromRaw(checked(-value.RawValue)) : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F64 Floor(this F64 value)
        {
            return F64.FromRaw(value.RawValue & F64.IntegerMask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F64 Ceiling(this F64 value)
        {
            long raw = value.RawValue;
            if ((raw & F64.FractionMask) == 0)
            {
                return value;
            }
            return F64.FromRaw((raw & F64.IntegerMask) + F64.OneRaw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F64 Truncate(this F64 value)
        {
            long raw = value.RawValue;
            if (raw < 0 && (raw & F64.FractionMask) != 0)
            {
                return F64.FromRaw((raw & F64.IntegerMask) + F64.OneRaw);
            }
            return F64.FromRaw(raw & F64.IntegerMask);
        }

        /// <remarks>
        /// Banker's Rounding.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F64 Round(this F64 value)
        {
            long raw = value.RawValue;
            long fraction = raw & F64.FractionMask;
            long floorRaw = raw & F64.IntegerMask;

            long halfRaw = F64.Half.RawValue;
            if (fraction < halfRaw)
            {
                return F64.FromRaw(floorRaw);
            }
            if (fraction > halfRaw)
            {
                return F64.FromRaw(floorRaw + F64.OneRaw);
            }

            // 0.5
            if ((floorRaw & F64.OneRaw) != 0)
            {
                return F64.FromRaw(floorRaw + F64.OneRaw);
            }

            return F64.FromRaw(floorRaw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F64 Lerp(F64 left, F64 right, F64 alpha)
        {
            return left + (right - left) * alpha;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F64 Sq(this F64 value)
        {
            return value * value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F64 SafeDivide(this F64 left, F64 right)
        {
            return right.RawValue == 0 ? F64.Zero : left / right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F64 SafeInverse(this F64 right)
        {
            return right.RawValue == 0 ? F64.Zero : F64.One / right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2F64 RadiusVector(int degrees)
        {
            return new Vector2F64(FPLut.Cos(degrees), FPLut.Sin(degrees));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearly(this F64 left, F64 right, F64 tolerance)
        {
            return Abs(left - right) <= tolerance;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearly(this Vector2F64 left, Vector2F64 right, F64 tolerance)
        {
            return
                Abs(left.X - right.X) <= tolerance &&
                Abs(left.Y - right.Y) <= tolerance;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearly(this Vector3F64 left, Vector3F64 right, F64 tolerance)
        {
            return
                Abs(left.X - right.X) <= tolerance &&
                Abs(left.Y - right.Y) <= tolerance &&
                Abs(left.Z - right.Z) <= tolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearly(this F64 left, F64 right)
        {
            return IsNearly(left, right, F64.LogicalTolerance);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearly(this Vector2F64 left, Vector2F64 right)
        {
            return IsNearly(left, right, F64.LogicalTolerance);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearly(this Vector3F64 left, Vector3F64 right)
        {
            return IsNearly(left, right, F64.LogicalTolerance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearlyZero(this F64 value, F64 tolerance)
        {
            return Abs(value) <= tolerance;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearlyZero(this Vector2F64 value, F64 tolerance)
        {
            return
                Abs(value.X) <= tolerance &&
                Abs(value.Y) <= tolerance;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearlyZero(this Vector3F64 value, F64 tolerance)
        {
            return
                Abs(value.X) <= tolerance &&
                Abs(value.Y) <= tolerance &&
                Abs(value.Z) <= tolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearlyZero(this F64 value)
        {
            return IsNearlyZero(value, F64.LogicalTolerance);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearlyZero(this Vector2F64 value)
        {
            return IsNearlyZero(value, F64.LogicalTolerance);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearlyZero(this Vector3F64 value)
        {
            return IsNearlyZero(value, F64.LogicalTolerance);
        }

        /// <inheritdoc cref="MathEx.GetClosestPointOnLine(UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector3)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3F64 GetClosestPointOnLine(Vector3F64 lineStart, Vector3F64 lineEnd, Vector3F64 point)
        {
            F64 t = GetClosestPointPositionOnLine(lineStart, lineEnd, point);
            return lineStart + (lineEnd - lineStart) * t;
        }

        /// <inheritdoc cref="MathEx.GetClosestPointPositionOnLine(UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector3)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F64 GetClosestPointPositionOnLine(Vector3F64 lineStart, Vector3F64 lineEnd, Vector3F64 point)
        {
            Vector3F64 lineDirection = lineEnd - lineStart;
            F64 lineLengthSq = lineDirection.LengthSquared();

            if (lineLengthSq.IsNearlyZero())
            {
                return F64.Zero;
            }

            F64 dotProduct = Vector3F64.Dot(point - lineStart, lineDirection) / lineLengthSq;

            return Clamp01(dotProduct);
        }
    }
}
