// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;

namespace Jih.Unity.Infrastructure.TileGrid
{
    public struct TileEdgeIndex : IEquatable<TileEdgeIndex>
    {
        public TileEdgeOrientation Orientation;
        public int X, Y;

        public TileEdgeIndex(TileEdgeOrientation orientation, int x, int y)
        {
            Orientation = orientation;
            X = x;
            Y = y;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is TileEdgeIndex index && Equals(index);
        }
        public readonly bool Equals(TileEdgeIndex other)
        {
            return Orientation == other.Orientation &&
                   X == other.X &&
                   Y == other.Y;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Orientation, X, Y);
        }

        public readonly override string ToString()
        {
            char orientationChar = Orientation switch
            {
                TileEdgeOrientation.Horizontal => 'H',
                TileEdgeOrientation.Vertical => 'V',
                _ => '?',
            };
            return $"<{orientationChar}, {X}, {Y}>";
        }

        public static bool operator ==(TileEdgeIndex left, TileEdgeIndex right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(TileEdgeIndex left, TileEdgeIndex right)
        {
            return !(left == right);
        }
    }
}
