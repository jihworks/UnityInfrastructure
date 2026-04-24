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
        public static F32 Sqrt(F32 v)
        {
            if (v.RawValue <= 0)
            {
                return F32.Zero;
            }

            ulong raw = (ulong)v.RawValue << 16;
            return F32.FromRaw((int)Sqrt_Impl(raw));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong Sqrt_Impl(ulong num)
        {
            ulong res = 0;
            ulong bit = 1UL << 62;

            while (bit > num)
            {
                bit >>= 2;
            }

            while (bit != 0)
            {
                if (num >= res + bit)
                {
                    num -= res + bit;
                    res = (res >> 1) + bit;
                }
                else
                {
                    res >>= 1;
                }
                bit >>= 2;
            }
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F32 Min(F32 a, F32 b)
        {
            return a.RawValue < b.RawValue ? a : b;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Min(ref F32 a, F32 b)
        {
            a = Min(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F32 Max(F32 a, F32 b)
        {
            return a.RawValue > b.RawValue ? a : b;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Max(ref F32 a, F32 b)
        {
            a = Max(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F32 Clamp(this F32 value, F32 min, F32 max)
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
        public static void Clamp(ref F32 value, F32 min, F32 max)
        {
            value = Clamp(value, min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F32 Clamp01(this F32 value)
        {
            return Clamp(value, F32.Zero, F32.One);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clamp01(ref F32 value)
        {
            value = Clamp(value, F32.Zero, F32.One);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F32 Abs(this F32 v)
        {
            return v.RawValue < 0 ? F32.FromRaw(-v.RawValue) : v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F32 Floor(F32 value)
        {
            return F32.FromRaw(value.RawValue & F32.IntegerMask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F32 Ceiling(F32 value)
        {
            int raw = value.RawValue;
            if ((raw & F32.FractionMask) == 0)
            {
                return value;
            }
            return F32.FromRaw((raw & F32.IntegerMask) + F32.OneRaw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F32 Truncate(F32 value)
        {
            int raw = value.RawValue;
            if (raw < 0 && (raw & F32.FractionMask) != 0)
            {
                return F32.FromRaw((raw & F32.IntegerMask) + F32.OneRaw);
            }
            return F32.FromRaw(raw & F32.IntegerMask);
        }

        /// <remarks>
        /// Banker's Rounding.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F32 Round(F32 value)
        {
            int raw = value.RawValue;
            int fraction = raw & F32.FractionMask;
            int floorRaw = raw & F32.IntegerMask;

            if (fraction < 0x8000)
            {
                return F32.FromRaw(floorRaw);
            }
            if (fraction > 0x8000)
            {
                return F32.FromRaw(floorRaw + F32.OneRaw);
            }

            // 0.5
            if ((floorRaw & F32.OneRaw) != 0)
            {
                return F32.FromRaw(floorRaw + F32.OneRaw);
            }

            return F32.FromRaw(floorRaw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F32 Lerp(F32 a, F32 b, F32 t)
        {
            return a + (b - a) * t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F32 SafeDivide(this F32 left, F32 right)
        {
            return right.RawValue == 0 ? F32.Zero : left / right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2F32 RadiusVector(int degrees)
        {
            return new Vector2F32(FPLut.Cos(degrees), FPLut.Sin(degrees));
        }
    }
}
