// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System.Runtime.CompilerServices;

namespace Jih.Unity.Infrastructure
{
    public static class RandomEx
    {
        /// <summary>
        /// Get random <c>double</c> in [0, 1] range from randomized <c>UInt64</c>.
        /// </summary>
        /// <param name="v">A random value in <c>UInt64</c>.</param>
        /// <returns>[0, 1]</returns>
        /// <remarks>
        /// Sacrificing very small precision to get <c>1.0</c>.<br/>
        /// The error is really tiny(approx. 1.11e-16), therefore, it can be ignored in most general cases.<br/>
        /// However, note that this approach does not provide mathematically perfect results.<br/>
        /// <br/>
        /// Guarantees deterministic results.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetDouble01(ulong v)
        {
            // Maximum value of 53 bits (2^53 - 1)
            const ulong MaxValue53 = (1UL << 53) - 1;

            // Extract upper 53 bits (64 - 53 = 11)
            ulong value53 = v >> 11;

            return value53 / (double)MaxValue53;
        }

        /// <summary>
        /// Get random <c>double</c> in [0, 1) range from randomized <c>UInt64</c>.
        /// </summary>
        /// <param name="v">A random value in <c>UInt64</c>.</param>
        /// <returns>[0, 1)</returns>
        /// <remarks>
        /// Provides mathematically perfect random numbers except <c>1.0</c>.<br/>
        /// <br/>
        /// Guarantees deterministic results.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetDouble(ulong v)
        {
            const double Divisor = 1UL << 53;

            ulong value53 = v >> 11;

            return value53 / Divisor;
        }
    }
}
