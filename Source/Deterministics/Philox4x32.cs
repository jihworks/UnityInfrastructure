// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Jih.Unity.Infrastructure.Deterministics
{
    /// <summary>
    /// Supports random access to the random numbers sequence by position value.
    /// </summary>
    /// <remarks>
    /// This implementation does not use caching. Generating occurs on demand.<br/>
    /// Thread-safe.
    /// <br/>
    /// Guarantees deterministic results. Even <c>double</c>s.
    /// </remarks>
    public readonly struct Philox4x32
    {
        readonly Philox4x32Key _key;

        public Philox4x32(int seed) : this(unchecked((uint)seed))
        {
        }
        public Philox4x32(uint seed)
        {
            _key = Philox4x32Key.FromSeed(seed);
        }
        public Philox4x32(uint key0, uint key1)
        {
            _key = new Philox4x32Key(key0, key1);
        }

        public uint GenerateUInt32(long position)
        {
            CheckPosition(position);

            return GenerateUInt32((ulong)position);
        }
        /// <returns>[0, <see cref="uint.MaxValue"/>]</returns>
        public uint GenerateUInt32(ulong position)
        {
            return Generate(position).V0;
        }

        public ulong GenerateUInt64(long position)
        {
            CheckPosition(position);

            return GenerateUInt64((ulong)position);
        }
        public ulong GenerateUInt64(ulong position)
        {
            Philox4x32Result result = Generate(position);
            return ((ulong)result.V1 << 32) | result.V0;
        }

        /// <returns>[<paramref name="minInclusive"/>, <paramref name="maxExclusive"/>)</returns>
        public int GenerateInt32(long position, int minInclusive, int maxExclusive)
        {
            CheckPosition(position);

            return GenerateInt32((ulong)position, minInclusive, maxExclusive);
        }
        /// <returns>[<paramref name="minInclusive"/>, <paramref name="maxExclusive"/>)</returns>
        public int GenerateInt32(ulong position, int minInclusive, int maxExclusive)
        {
            if (minInclusive >= maxExclusive)
            {
                throw new ArgumentException("Min must be less than max.");
            }

            uint range = (uint)(maxExclusive - minInclusive);
            uint value = GenerateUInt32(position);

            return minInclusive + (int)(((ulong)value * range) >> 32);
        }

        /// <returns>[0, 1)</returns>
        public double GenerateDouble(long position)
        {
            CheckPosition(position);

            return GenerateDouble((ulong)position);
        }
        /// <returns>[0, 1)</returns>
        public double GenerateDouble(ulong position)
        {
            ulong v = GenerateUInt64(position);
            return RandomEx.GetDouble(v);
        }

        /// <returns>[0, 1]</returns>
        public double GenerateDouble01(long position)
        {
            CheckPosition(position);

            return GenerateDouble01((ulong)position);
        }
        /// <returns>[0, 1]</returns>
        public double GenerateDouble01(ulong position)
        {
            ulong v = GenerateUInt64(position);
            return RandomEx.GetDouble01(v);
        }

        /// <returns><see cref="F32"/> in range [0, 1)</returns>
        public F32 GenerateF32(long position)
        {
            CheckPosition(position);

            return GenerateF32((ulong)position);
        }
        /// <returns><see cref="F32"/> in range [0, 1)</returns>
        public F32 GenerateF32(ulong position)
        {
            uint rawRandom = GenerateUInt32(position);

            // Extract lower 16 bits (0 ~ 65535)
            int value16 = (int)(rawRandom & 0xFFFF);

            return F32.FromRaw(value16);
        }

        public bool GenerateBoolean(long position)
        {
            CheckPosition(position);

            return GenerateBoolean((ulong)position);
        }
        public bool GenerateBoolean(ulong position)
        {
            return (GenerateUInt32(position) & 1) != 0;
        }

        public void GenerateBytes(long position, Span<byte> destination)
        {
            CheckPosition(position);

            GenerateBytes((ulong)position, destination);
        }
        public void GenerateBytes(ulong position, Span<byte> destination)
        {
            if (destination.Length < 16)
            {
                throw new ArgumentException("Destination must be at least 16 bytes length.");
            }

            Philox4x32Result result = Generate(position);

            BinaryPrimitives.WriteUInt32LittleEndian(destination[..4], result.V0);
            BinaryPrimitives.WriteUInt32LittleEndian(destination.Slice(4, 4), result.V1);
            BinaryPrimitives.WriteUInt32LittleEndian(destination.Slice(8, 4), result.V2);
            BinaryPrimitives.WriteUInt32LittleEndian(destination.Slice(12, 4), result.V3);
        }

        public Philox4x32Result Generate(ulong position)
        {
            uint ctr0 = (uint)(position & 0xFFFFFFFF);
            uint ctr1 = (uint)((position >> 32) & 0xFFFFFFFF);
            uint ctr2 = 0;
            uint ctr3 = 0;

            return PhiloxRound(ctr0, ctr1, ctr2, ctr3, _key.K0, _key.K1);
        }

        static Philox4x32Result PhiloxRound(uint x0, uint x1, uint x2, uint x3, uint k0, uint k1)
        {
            for (int i = 0; i < R; i++)
            {
                // Feistel network rounds
                ulong prod0 = (ulong)x0 * M0;
                ulong prod1 = (ulong)x2 * M1;

                uint hi0 = (uint)(prod0 >> 32);
                uint lo0 = (uint)(prod0 & 0xFFFFFFFF);
                uint hi1 = (uint)(prod1 >> 32);
                uint lo1 = (uint)(prod1 & 0xFFFFFFFF);

                // Calculate new state
                uint newX0 = hi1 ^ x1 ^ k0;
                uint newX1 = lo1;
                uint newX2 = hi0 ^ x3 ^ k1;
                uint newX3 = lo0;

                x0 = newX0;
                x1 = newX1;
                x2 = newX2;
                x3 = newX3;

                // Key schedule
                if (i < R - 1)
                {
                    unchecked
                    {
                        k0 += W0;
                        k1 += W1;
                    }
                }
            }

            return new Philox4x32Result(x0, x1, x2, x3);
        }

        readonly struct Philox4x32Key
        {
            public static Philox4x32Key FromSeed(uint seed)
            {
                return new Philox4x32Key(seed, 0);
            }

            public readonly uint K0, K1;

            public Philox4x32Key(uint k0, uint k1)
            {
                K0 = k0;
                K1 = k1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void CheckPosition(long position)
        {
            if (position < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(position), "Position must be non-negative.");
            }
        }

        const uint R = 10;
        const uint M0 = 0xD2511F53;
        const uint M1 = 0xCD9E8D57;
        const uint W0 = 0x9E3779B9;
        const uint W1 = 0xBB67AE85;
    }

    public readonly struct Philox4x32Result
    {
        public readonly uint V0, V1, V2, V3;

        public Philox4x32Result(uint v0, uint v1, uint v2, uint v3)
        {
            V0 = v0;
            V1 = v1;
            V2 = v2;
            V3 = v3;
        }
    }
}
