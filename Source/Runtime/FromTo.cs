// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;

namespace Jih.Unity.Runtime
{
    public struct FromTo : IEquatable<FromTo>
    {
        public float From, To;

        public FromTo(float from, float to)
        {
            From = from;
            To = to;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is FromTo pair && Equals(pair);
        }
        public readonly bool Equals(FromTo other)
        {
            return From == other.From &&
                   To == other.To;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(From, To);
        }

        public static bool operator ==(FromTo left, FromTo right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(FromTo left, FromTo right)
        {
            return !(left == right);
        }
    }
}
