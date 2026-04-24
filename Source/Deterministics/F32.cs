// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Runtime.CompilerServices;

namespace Jih.Unity.Infrastructure.Deterministics
{
    /// <summary>
    /// 32-bit Fixed-Point Number (Q16.16 format).<br/>
    /// Provides deterministic arithmetics.
    /// </summary>
    public readonly struct F32 : IEquatable<F32>, IComparable<F32>
    {
        public static F32 FromInt(int v)
        {
            return new F32(v << FractionalBits);
        }
        /// <remarks>
        /// <b>DO NOT</b> use this for a value in middle of calculations. Because will break deterministics.<br/>
        /// Use this to get constant values only. Specifically, hard-coded literal values or their copies.
        /// </remarks>
        public static F32 FromFloat(float v)
        {
            return new F32((int)MathF.Round(v * OneRaw));
        }
        /// <inheritdoc cref="FromInt(int)"/>
        public static F32 FromDouble(double v)
        {
            return new F32((int)Math.Round(v * OneRaw));
        }

        /// <summary>
        /// Probably need <see cref="FromInt(int)"/> instead of this.
        /// </summary>
        /// <remarks>
        /// Do not use this if don't know exactly what it doing.
        /// </remarks>
        internal static F32 FromRaw(int rawValue)
        {
            return new F32(rawValue);
        }

        public readonly int RawValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private F32(int rawValue)
        {
            RawValue = rawValue;
        }

        public static implicit operator F32(int v)
        {
            return FromInt(v);
        }
        public static explicit operator F32(float v)
        {
            return FromFloat(v);
        }
        public static explicit operator F32(double v)
        {
            return FromDouble(v);
        }

        public static explicit operator int(F32 v)
        {
            return v.RawValue >> FractionalBits;
        }
        public static explicit operator float(F32 v)
        {
            return (float)v.RawValue / OneRaw;
        }
        public static explicit operator double(F32 v)
        {
            return (double)v.RawValue / OneRaw;
        }

        public static F32 operator +(F32 l, F32 r)
        {
            return new F32(checked(l.RawValue + r.RawValue));
        }
        public static F32 operator -(F32 l, F32 r)
        {
            return new F32(checked(l.RawValue - r.RawValue));
        }
        public static F32 operator *(F32 l, F32 r)
        {
            long t = ((long)l.RawValue * r.RawValue) >> FractionalBits;
            return new F32(checked((int)t));
        }
        public static F32 operator /(F32 l, F32 r)
        {
            long t = (((long)l.RawValue) << FractionalBits) / r.RawValue;
            return new F32(checked((int)t));
        }

        public static F32 operator -(F32 v)
        {
            return new F32(checked(-v.RawValue));
        }
        public static F32 operator +(F32 v)
        {
            return v;
        }

        public static bool operator ==(F32 l, F32 r)
        {
            return l.RawValue == r.RawValue;
        }
        public static bool operator !=(F32 l, F32 r)
        {
            return l.RawValue != r.RawValue;
        }
        public static bool operator >(F32 l, F32 r)
        {
            return l.RawValue > r.RawValue;
        }
        public static bool operator <(F32 l, F32 r)
        {
            return l.RawValue < r.RawValue;
        }
        public static bool operator >=(F32 l, F32 r)
        {
            return l.RawValue >= r.RawValue;
        }
        public static bool operator <=(F32 l, F32 r)
        {
            return l.RawValue <= r.RawValue;
        }

        public readonly bool Equals(F32 other)
        {
            return RawValue == other.RawValue;
        }
        public readonly override bool Equals(object? obj)
        {
            return obj is F32 other && Equals(other);
        }

        public readonly int CompareTo(F32 other)
        {
            return RawValue.CompareTo(other.RawValue);
        }

        public override int GetHashCode()
        {
            return RawValue;
        }

        public override string ToString()
        {
            return ((double)this).ToString("0.#####");
        }

        public static readonly F32 Zero = new(0);
        public static readonly F32 One = new(OneRaw);
        /// <summary>
        /// <c>0.5</c>
        /// </summary>
        public static readonly F32 Half = new(OneRaw >> 1);
        /// <summary>
        /// <c>0.25</c>
        /// </summary>
        public static readonly F32 Quarter = new(OneRaw >> 2);

        /// <summary>
        /// Approx. <c>-32768.0</c>
        /// </summary>
        public static readonly F32 MinValue = new(int.MinValue);
        /// <summary>
        /// Approx. <c>32767.99998...</c>
        /// </summary>
        public static readonly F32 MaxValue = new(int.MaxValue);

        internal const int FractionalBits = 16;
        internal const int OneRaw = 1 << FractionalBits; // 65536

        internal const int FractionMask = 0xFFFF; // Lower 16 bits. Frational part.
        internal const int IntegerMask = ~0xFFFF; // Upper 16 bits. Integer part.
    }
}
