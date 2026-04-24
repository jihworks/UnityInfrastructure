// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System.Collections.Generic;

namespace Jih.Unity.Infrastructure.Deterministics
{
    public static class FPEnumerable
    {
        public static F32 SumF32(this IEnumerable<F32> collection)
        {
            F32 result = 0;
            foreach (var item in collection)
            {
                result += item;
            }
            return result;
        }
        public static F32 SumF32(this IEnumerable<int> collection)
        {
            F32 result = 0;
            foreach (var item in collection)
            {
                result += item;
            }
            return result;
        }

        public static F32 AverageF32(this IEnumerable<F32> collection)
        {
            int count = 0;
            F32 sum = 0;
            foreach (var item in collection)
            {
                sum += item;
                count++;
            }
            return sum / count;
        }
        public static F32 AverageF32(this IEnumerable<int> collection)
        {
            int count = 0;
            F32 sum = 0;
            foreach (var item in collection)
            {
                sum += item;
                count++;
            }
            return sum / count;
        }
    }
}
