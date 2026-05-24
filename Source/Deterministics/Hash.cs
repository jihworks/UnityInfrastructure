// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

namespace Jih.Unity.Infrastructure.Deterministics
{
    public static class Hash
    {
        /// <remarks>
        /// Deterministic-safe.
        /// </remarks>
        public static uint CalculateFNV1a(int value1, int value2, int value3)
        {
            const uint fnvPrime = 16777619;

            uint hash = 2166136261;

            hash ^= (uint)value1;
            hash *= fnvPrime;

            hash ^= (uint)value2;
            hash *= fnvPrime;

            hash ^= (uint)value3;
            hash *= fnvPrime;

            return hash;
        }
    }
}
