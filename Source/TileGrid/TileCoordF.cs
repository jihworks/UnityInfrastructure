// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;

namespace Jih.Unity.Infrastructure.TileGrid
{
    public struct TileCoordF : IEquatable<TileCoordF>
    {
        public static TileCoordF Add(TileCoordF left, TileCoordF right)
        {
            return new TileCoordF(left.X + right.X, left.Y + right.Y);
        }
        public static TileCoordF Subtract(TileCoordF left, TileCoordF right)
        {
            return new TileCoordF(left.X - right.X, left.Y - right.Y);
        }
        public static TileCoordF Multiply(TileCoordF left, float right)
        {
            return new TileCoordF(left.X * right, left.Y * right);
        }

        public static TileCoord Round(TileCoordF left)
        {
            return new TileCoord((int)Math.Round(left.X), (int)Math.Round(left.Y));
        }

        public float X;
        public float Y;

        public TileCoordF(float x, float y)
        {
            X = x;
            Y = y;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is TileCoordF f && Equals(f);
        }
        public readonly bool Equals(TileCoordF other)
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

        public static bool operator ==(TileCoordF left, TileCoordF right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(TileCoordF left, TileCoordF right)
        {
            return !(left == right);
        }

        public static TileCoordF operator +(TileCoordF left, TileCoordF right)
        {
            return Add(left, right);
        }
        public static TileCoordF operator -(TileCoordF left, TileCoordF right)
        {
            return Subtract(left, right);
        }
        public static TileCoordF operator *(TileCoordF left, float right)
        {
            return Multiply(left, right);
        }

        public static implicit operator TileCoordF(TileCoord left)
        {
            return new TileCoordF(left.X, left.Y);
        }
        /// <summary>
        /// Truncate to integers.
        /// </summary>
        public static explicit operator TileCoord(TileCoordF left)
        {
            return new TileCoord((int)left.X, (int)left.Y);
        }
    }
}
