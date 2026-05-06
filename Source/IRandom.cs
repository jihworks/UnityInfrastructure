// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Deterministics;
using System;
using System.Buffers.Binary;

namespace Jih.Unity.Infrastructure
{
    /// <seealso cref="SystemRandom"/>
    /// <seealso cref="RandomStream"/>
    public interface IRandomInt32
    {
        /// <summary>
        /// Gets a random <c>int</c> that is greater than or equal to <paramref name="minInclusive"/> and <b>less</b> than <paramref name="maxExclusive"/>.
        /// </summary>
        /// <remarks>
        /// The distribution of returned values must be approximately uniform across the range of possible values.
        /// </remarks>
        /// <param name="minInclusive">Inclusive minimum value.</param>
        /// <param name="maxExclusive"><b>Exclusive</b> maximum value.</param>
        /// <returns>[<paramref name="minInclusive"/>, <paramref name="maxExclusive"/>)</returns>
        int NextInt32(int minInclusive, int maxExclusive);
    }

    /// <seealso cref="SystemRandom"/>
    /// <seealso cref="RandomStream"/>
    public interface IRandomDouble
    {
        /// <summary>
        /// Gets a random <c>double</c> in [0, 1) range.
        /// </summary>
        /// <remarks>
        /// This approach never returns <c>1.0</c> but provides mathematically perfect random numbers.<br/>
        /// Check <see cref="RandomEx"/> for more details.
        /// </remarks>
        /// <returns>[0, 1)</returns>
        double NextDouble();
    }

    /// <seealso cref="SystemRandom"/>
    /// <seealso cref="RandomStream"/>
    public interface IRandomDouble01
    {
        /// <summary>
        /// Gets a random <c>double</c> in [0, 1] range.
        /// </summary>
        /// <remarks>
        /// This approach <b>cannot</b> provide mathematically perfect random numbers but the error is negligible.<br/>
        /// Check <see cref="RandomEx"/> for more details.
        /// </remarks>
        /// <returns>[0, 1]</returns>
        double NextDouble01();
    }

    /// <seealso cref="RandomStream"/>
    public interface IRandomF64
    {
        /// <summary>
        /// Gets a random <see cref="F64"/> in [0, 1) range.
        /// </summary>
        /// <remarks>
        /// Deterministic-safe.
        /// </remarks>
        /// <returns>[0, 1)</returns>
        F64 NextF64();
    }

    public class SystemRandom : Random, IRandomInt32, IRandomDouble, IRandomDouble01
    {
        public SystemRandom()
        {
        }
        public SystemRandom(int seed) : base(seed)
        {
        }

        public int NextInt32(int minInclusive, int maxExclusive)
        {
            return Next(minInclusive, maxExclusive);
        }

        public double NextDouble01()
        {
            Span<byte> buffer = stackalloc byte[8];
            NextBytes(buffer);
            ulong v = BinaryPrimitives.ReadUInt64LittleEndian(buffer);
            return RandomEx.GetDouble01(v);
        }
    }
}
