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
        public static F64 SumF64(this IEnumerable<F64> collection)
        {
            F64 result = 0;
            foreach (var item in collection)
            {
                result += item;
            }
            return result;
        }
        public static F64 SumF64(this IEnumerable<int> collection)
        {
            F64 result = 0;
            foreach (var item in collection)
            {
                result += item;
            }
            return result;
        }
        public static F64 SumF64(this IEnumerable<long> collection)
        {
            F64 result = 0;
            foreach (var item in collection)
            {
                result += item;
            }
            return result;
        }

        public static F64 AverageF64(this IEnumerable<F64> collection)
        {
            int count = 0;
            F64 sum = 0;
            foreach (var item in collection)
            {
                sum += item;
                count++;
            }
            return sum / count;
        }
        public static F64 AverageF64(this IEnumerable<int> collection)
        {
            int count = 0;
            F64 sum = 0;
            foreach (var item in collection)
            {
                sum += item;
                count++;
            }
            return sum / count;
        }
        public static F64 AverageF64(this IEnumerable<long> collection)
        {
            int count = 0;
            F64 sum = 0;
            foreach (var item in collection)
            {
                sum += item;
                count++;
            }
            return sum / count;
        }
    }
}
