// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;

namespace Jih.Unity.Infrastructure.TileGrid
{
    public struct TileVertexIndex : IEquatable<TileVertexIndex>
    {
        public int X, Y;

        public TileVertexIndex(int x, int y)
        {
            X = x;
            Y = y;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is TileVertexIndex index && Equals(index);
        }
        public readonly bool Equals(TileVertexIndex other)
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

        public static bool operator ==(TileVertexIndex left, TileVertexIndex right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(TileVertexIndex left, TileVertexIndex right)
        {
            return !(left == right);
        }
    }
}
