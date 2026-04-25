// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System.Runtime.CompilerServices;

namespace Jih.Unity.Infrastructure.Deterministics
{
    public static class FPMath
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F64 Sqrt(this F64 v)
        {
            if (v.RawValue <= 0)
            {
                return F64.Zero;
            }

            ulong numHi = (ulong)v.RawValue;

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
        public static F64 Min(F64 a, F64 b)
        {
            return a.RawValue < b.RawValue ? a : b;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Min(ref F64 a, F64 b)
        {
            a = Min(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F64 Max(F64 a, F64 b)
        {
            return a.RawValue > b.RawValue ? a : b;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Max(ref F64 a, F64 b)
        {
            a = Max(a, b);
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
        public static F64 Abs(this F64 v)
        {
            return v.RawValue < 0 ? F64.FromRaw(-v.RawValue) : v;
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
        public static F64 Lerp(F64 a, F64 b, F64 t)
        {
            return a + (b - a) * t;
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
        public static Vector2F64 RadiusVector(int degrees)
        {
            return new Vector2F64(FPLut.Cos(degrees), FPLut.Sin(degrees));
        }
    }
}
