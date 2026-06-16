// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;

namespace Jih.Unity.Infrastructure.TileGrid
{
    public struct TileCoord : IEquatable<TileCoord>
    {
        public static TileCoord Add(TileCoord left, TileCoord right)
        {
            return new TileCoord(left.X + right.X, left.Y + right.Y);
        }
        public static TileCoord Subtract(TileCoord left, TileCoord right)
        {
            return new TileCoord(left.X - right.X, left.Y - right.Y);
        }
        public static TileCoord Multiply(TileCoord left, int right)
        {
            return new TileCoord(left.X * right, left.Y * right);
        }

        public int X, Y;

        public TileCoord(int x, int y)
        {
            X = x;
            Y = y;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is TileCoord index && Equals(index);
        }
        public readonly bool Equals(TileCoord other)
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

        public static bool operator ==(TileCoord left, TileCoord right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(TileCoord left, TileCoord right)
        {
            return !(left == right);
        }

        public static TileCoord operator +(TileCoord left, TileCoord right)
        {
            return Add(left, right);
        }
        public static TileCoord operator -(TileCoord left, TileCoord right)
        {
            return Subtract(left, right);
        }
        public static TileCoord operator *(TileCoord left, int right)
        {
            return Multiply(left, right);
        }
    }
}
