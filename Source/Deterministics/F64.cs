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
    /// 64-bit Fixed-Point Number (Q31.32 format).<br/>
    /// Provides deterministic arithmetics.
    /// </summary>
    /// <remarks>
    /// * Deterministic-safe operations for standard floating-points (<c>float</c>, <c>double</c>):<br/>
    /// - Guaranteed by the IEEE 754 standard.<br/>
    /// 1. Basic arithmetic operations (<c>+</c>, <c>-</c>, <c>*</c>, <c>/</c>)<br/>
    /// 2. Comparison operations (<c>==</c>, <c>!=</c>, <c>&gt;</c>, <c>&lt;</c>, etc.)<br/>
    /// 3. Type casting (Precision loss may occur, but the loss itself is deterministic.)<br/>
    /// 4. Rounding operations (<c>Round</c>, <c>Floor</c>, <c>Truncate</c>, <c>Ceiling</c>)<br/>
    /// 5. Bit-level/Sign operations (<c>Abs</c>, <c>Sign</c>, etc.)<br/>
    /// <br/>
    /// * <b>NOT</b> deterministic-safe operations:<br/>
    /// - Generally hardware/OS/library dependent.<br/>
    /// 1. Trigonometric functions (<c>Sin</c>, <c>Cos</c>, <c>Atan</c>, etc.)<br/>
    /// 2. Exponential and logarithmic functions (<c>Pow</c>, <c>Exp</c>, <c>Log</c>, etc.)<br/>
    /// 3. Square root (<c>Sqrt</c>) function (While IEEE 754 mandates correctness, real-world implementations often vary across platforms.)<br/>
    /// 4. Any other complex transcendental functions.
    /// </remarks>
    public readonly struct F64 : IEquatable<F64>, IComparable<F64>
    {
        /// <remarks>
        /// Deterministic-safe.
        /// </remarks>
        /// <param name="v">A value for integer part.</param>
        public static F64 FromInt(int v)
        {
            return new F64(((long)v) << FractionalBits);
        }

        /// <remarks>
        /// Deterministic-safe.<br/>
        /// <br/>
        /// But possibly losing precision. The loss itself is also deterministic-safe.<br/>
        /// <br/>
        /// Even though, strongly recommend isolating calculations in deterministic-safe states.
        /// </remarks>
        public static F64 FromFloat(float v)
        {
            if (float.IsNaN(v) || float.IsInfinity(v))
            {
                throw new NotFiniteNumberException($"Cannot convert {v} to {nameof(F64)}.");
            }
            return new F64(checked((long)MathF.Round(v * OneRaw)));
        }
        /// <inheritdoc cref="FromFloat(float)"/>
        public static F64 FromDouble(double v)
        {
            if (double.IsNaN(v) || double.IsInfinity(v))
            {
                throw new NotFiniteNumberException($"Cannot convert {v} to {nameof(F64)}.");
            }
            return new F64(checked((long)Math.Round(v * OneRaw)));
        }

        /// <summary>
        /// Probably need <see cref="FromLong(long)"/> instead of this.
        /// </summary>
        /// <remarks>
        /// Do not use this if don't know exactly what it doing.
        /// </remarks>
        public static F64 FromRaw(long rawValue)
        {
            return new F64(rawValue);
        }

        public readonly long RawValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private F64(long rawValue)
        {
            RawValue = rawValue;
        }

        /// <inheritdoc cref="FromInt(int)"/>
        public static implicit operator F64(int v)
        {
            return FromInt(v);
        }

        /// <inheritdoc cref="FromFloat(float)"/>
        public static explicit operator F64(float v)
        {
            return FromFloat(v);
        }
        /// <inheritdoc cref="FromDouble(double)"/>
        public static explicit operator F64(double v)
        {
            return FromDouble(v);
        }

        /// <remarks>
        /// Deterministic-safe.<br/>
        /// <br/>
        /// However, <c>int</c> can be converted to <c>float</c> or <c>double</c> implicitly, then, it will break deterministics.<br/>
        /// Therefore, this casting is <c>explicit</c>.
        /// </remarks>
        public static explicit operator int(F64 v)
        {
            return (int)(v.RawValue >> FractionalBits);
        }
        /// <remarks>
        /// Deterministic-safe.<br/>
        /// <br/>
        /// However, <c>long</c> can be converted to <c>float</c> or <c>double</c> implicitly, then, it will break deterministics.<br/>
        /// Therefore, this casting is <c>explicit</c>.
        /// </remarks>
        public static explicit operator long(F64 v)
        {
            return v.RawValue >> FractionalBits;
        }

        /// <remarks>
        /// <c>NOT</c> deterministic-safe.<br/>
        /// <br/>
        /// The returned value itself is deterministic-safe.<br/>
        /// And, converting back to <see cref="F64"/> <b>without modification</b> is also deterministic-safe. But losing precision.<br/>
        /// The loss itself is also deterministic-safe.<br/>
        /// <br/>
        /// Even though, strongly recommend isolating calculations in deterministic-safe states.
        /// </remarks>
        public static explicit operator float(F64 v)
        {
            return v.RawValue / (float)OneRaw;
        }
        /// <inheritdoc cref="explicit operator float"/>
        public static explicit operator double(F64 v)
        {
            return v.RawValue / (double)OneRaw;
        }

        /// <exception cref="OverflowException">Throws when arithmetic overflow is detected.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F64 operator +(F64 l, F64 r)
        {
            return new F64(checked(l.RawValue + r.RawValue));
        }
        /// <inheritdoc cref="operator +"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F64 operator -(F64 l, F64 r)
        {
            return new F64(checked(l.RawValue - r.RawValue));
        }
        /// <inheritdoc cref="operator +"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F64 operator *(F64 l, F64 r)
        {
            bool isNegative = (l.RawValue < 0L) ^ (r.RawValue < 0L);
            ulong x = (ulong)(l.RawValue >= 0L ? l.RawValue : -l.RawValue);
            ulong y = (ulong)(r.RawValue >= 0L ? r.RawValue : -r.RawValue);

            UInt128 multResult = UInt128.Multiply(x, y);
            ulong shifted = multResult.ShiftRight32();

            long finalRaw = (long)shifted;
            if (finalRaw < 0L)
            {
                throw new OverflowException($"{nameof(F64)} Multiplication overflowed.");
            }

            return isNegative ? new F64(-finalRaw) : new F64(finalRaw);
        }
        /// <exception cref="DivideByZeroException">Throws when the divisor is 0.</exception>
        /// <inheritdoc cref="operator +"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static F64 operator /(F64 l, F64 r)
        {
            if (r.RawValue == 0L)
            {
                throw new DivideByZeroException();
            }

            bool isNegative = (l.RawValue < 0) ^ (r.RawValue < 0);
            ulong num = (ulong)(l.RawValue >= 0 ? l.RawValue : -l.RawValue);
            ulong den = (ulong)(r.RawValue >= 0 ? r.RawValue : -r.RawValue);

            // Shift-and-subtract division algorithm.
            ulong result = Div64(num, den);

            long finalRaw = (long)result;
            if (finalRaw < 0)
            {
                throw new OverflowException($"{nameof(F64)} Division overflowed.");
            }

            return isNegative ? new F64(-finalRaw) : new F64(finalRaw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong Div64(ulong num, ulong den)
        {
            // (num << 32) / den
            ulong remainder = num >> 32;
            ulong quotient = 0ul;
            ulong num_lo = num << 32;

            for (int i = 0; i < 64; i++)
            {
                ulong bit = num_lo >> 63;
                remainder = (remainder << 1) | bit;
                num_lo <<= 1;
                quotient <<= 1;

                if (remainder >= den)
                {
                    remainder -= den;
                    quotient |= 1;
                }
            }
            return quotient;
        }

        /// <inheritdoc cref="operator +"/>
        public static F64 operator -(F64 v)
        {
            return new(checked(-v.RawValue));
        }
        public static F64 operator +(F64 v)
        {
            return v;
        }

        public static bool operator ==(F64 l, F64 r)
        {
            return l.RawValue == r.RawValue;
        }
        public static bool operator !=(F64 l, F64 r)
        {
            return l.RawValue != r.RawValue;
        }
        public static bool operator >(F64 l, F64 r)
        {
            return l.RawValue > r.RawValue;
        }
        public static bool operator <(F64 l, F64 r)
        {
            return l.RawValue < r.RawValue;
        }
        public static bool operator >=(F64 l, F64 r)
        {
            return l.RawValue >= r.RawValue;
        }
        public static bool operator <=(F64 l, F64 r)
        {
            return l.RawValue <= r.RawValue;
        }

        public readonly bool Equals(F64 other)
        {
            return RawValue == other.RawValue;
        }
        public readonly override bool Equals(object? obj)
        {
            return obj is F64 other && Equals(other);
        }

        public readonly int CompareTo(F64 other)
        {
            return RawValue.CompareTo(other.RawValue);
        }

        public readonly override int GetHashCode()
        {
            return RawValue.GetHashCode();
        }

        public override string ToString()
        {
            return ((double)this).ToString("0.#########");
        }

        public static readonly F64 Zero = new(0L);
        public static readonly F64 One = new(OneRaw);
        /// <summary>
        /// <c>0.5</c>
        /// </summary>
        public static readonly F64 Half = new(OneRaw >> 1);
        /// <summary>
        /// <c>0.25</c>
        /// </summary>
        public static readonly F64 Quarter = new(OneRaw >> 2);

        /// <summary>
        /// <c>-2147483648.0</c>
        /// </summary>
        public static readonly F64 MinValue = new(long.MinValue);
        /// <summary>
        /// Approx. <c>2147483647.9999999997</c>
        /// </summary>
        public static readonly F64 MaxValue = new(long.MaxValue);

        /// <summary>
        /// Detectable minimum difference value.
        /// </summary>
        public static readonly F64 Epsilon = FromRaw(1L);

        /// <summary>
        /// Approx. <c>0.0001</c>
        /// </summary>
        public static readonly F64 LogicalTolerance = FromRaw(429497L);

        internal const int FractionalBits = 32;
        /// <summary>
        /// <c>4294967296</c>
        /// </summary>
        internal const long OneRaw = 1L << FractionalBits;

        /// <summary>
        /// Lower 32 bits. Frational part.
        /// </summary>
        internal const long FractionMask = 0xFFFFFFFFL;
        /// <summary>
        /// Upper 32 bits. Integer part.
        /// </summary>
        internal const long IntegerMask = ~0xFFFFFFFFL;
    }
}
