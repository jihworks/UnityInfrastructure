// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Deterministics;
using System;

namespace Jih.Unity.Infrastructure.TileGrid
{
    public struct TileCoordF64 : IEquatable<TileCoordF64>
    {
        public static TileCoordF64 Add(TileCoordF64 left, TileCoordF64 right)
        {
            return new TileCoordF64(left.X + right.X, left.Y + right.Y);
        }
        public static TileCoordF64 Subtract(TileCoordF64 left, TileCoordF64 right)
        {
            return new TileCoordF64(left.X - right.X, left.Y - right.Y);
        }
        public static TileCoordF64 Multiply(TileCoordF64 left, F64 right)
        {
            return new TileCoordF64(left.X * right, left.Y * right);
        }

        public static TileCoord Round(TileCoordF64 left)
        {
            return new TileCoord((int)FPMath.Round(left.X), (int)FPMath.Round(left.Y));
        }

        public F64 X;
        public F64 Y;

        public TileCoordF64(F64 x, F64 y)
        {
            X = x;
            Y = y;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is TileCoordF64 f && Equals(f);
        }
        public readonly bool Equals(TileCoordF64 other)
        {
            return X == other.X &&
                   Y == other.Y;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public readonly override string ToString()
        {
            return $"<{X}, {Y}>";
        }

        public static bool operator ==(TileCoordF64 left, TileCoordF64 right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(TileCoordF64 left, TileCoordF64 right)
        {
            return !(left == right);
        }

        public static TileCoordF64 operator +(TileCoordF64 left, TileCoordF64 right)
        {
            return Add(left, right);
        }
        public static TileCoordF64 operator -(TileCoordF64 left, TileCoordF64 right)
        {
            return Subtract(left, right);
        }
        public static TileCoordF64 operator *(TileCoordF64 left, F64 right)
        {
            return Multiply(left, right);
        }

        public static implicit operator TileCoordF64(TileCoord left)
        {
            return new TileCoordF64(left.X, left.Y);
        }
        /// <summary>
        /// Truncate to integers.
        /// </summary>
        public static explicit operator TileCoord(TileCoordF64 left)
        {
            return new TileCoord((int)left.X, (int)left.Y);
        }
    }
}
