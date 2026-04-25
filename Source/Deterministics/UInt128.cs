// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System.Runtime.CompilerServices;

namespace Jih.Unity.Infrastructure.Deterministics
{
    /// <summary>
    /// <see cref="F64"/> supporting type. Not for common use.
    /// </summary>
    internal readonly struct UInt128
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt128 Multiply(ulong u, ulong v)
        {
            ulong uHi = u >> 32;
            ulong uLo = u & 0xFFFFFFFF;
            ulong vHi = v >> 32;
            ulong vLo = v & 0xFFFFFFFF;

            ulong t = uLo * vLo;
            ulong w0 = t & 0xFFFFFFFF;
            ulong k = t >> 32;

            t = uHi * vLo + k;
            ulong w1 = t & 0xFFFFFFFF;
            ulong w2 = t >> 32;

            t = uLo * vHi + w1;
            k = t >> 32;

            ulong hi = uHi * vHi + w2 + k;
            ulong lo = (t << 32) + w0;

            return new UInt128(hi, lo);
        }

        public readonly ulong Hi;
        public readonly ulong Lo;

        public UInt128(ulong hi, ulong lo)
        {
            Hi = hi;
            Lo = lo;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ShiftRight32()
        {
            return (Hi << 32) | (Lo >> 32);
        }
    }
}
