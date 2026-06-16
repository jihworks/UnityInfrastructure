// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Deterministics;
using System.Runtime.CompilerServices;

namespace Jih.Unity.Infrastructure.TileGrid
{
    /// <summary>
    /// CW order from Top.
    /// </summary>
    public enum TileOrthogonalPosition
    {
        Top = 0,
        Right,
        Bottom,
        Left,

        T = Top,
        R = Right,
        B = Bottom,
        L = Left,
    }

    /// <summary>
    /// CW order from Top-Left.
    /// </summary>
    public enum TileDiagonalPosition
    {
        TopLeft = 0,
        TopRight,
        BottomRight,
        BottomLeft,

        TL = TopLeft,
        TR = TopRight,
        BR = BottomRight,
        BL = BottomLeft,
    }

    /// <inheritdoc cref="TileOrthogonalPosition"/>
    public enum TileEdgePosition
    {
        Top = 0,
        Right,
        Bottom,
        Left,

        T = Top,
        R = Right,
        B = Bottom,
        L = Left,
    }

    /// <inheritdoc cref="TileDiagonalPosition"/>
    public enum TileVertexPosition
    {
        TopLeft = 0,
        TopRight,
        BottomRight,
        BottomLeft,

        TL = TopLeft,
        TR = TopRight,
        BR = BottomRight,
        BL = BottomLeft,
    }

    public static class TilePositionEx
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TileOrthogonalPosition Next(this TileOrthogonalPosition position)
        {
            return (TileOrthogonalPosition)((int)position).GetNextCircularIndex(4);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TileOrthogonalPosition Prev(this TileOrthogonalPosition position)
        {
            return (TileOrthogonalPosition)((int)position).GetPrevCircularIndex(4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TileOrthogonalPosition Flip(this TileOrthogonalPosition position)
        {
            return (TileOrthogonalPosition)(((int)position + 2) % 4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TileCoord GetOffset(this TileOrthogonalPosition position)
        {
            return _orthogonalOffsets[(int)position];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TileDiagonalPosition Next(this TileDiagonalPosition position)
        {
            return (TileDiagonalPosition)((int)position).GetNextCircularIndex(4);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TileDiagonalPosition Prev(this TileDiagonalPosition position)
        {
            return (TileDiagonalPosition)((int)position).GetPrevCircularIndex(4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TileDiagonalPosition Flip(this TileDiagonalPosition position)
        {
            return (TileDiagonalPosition)(((int)position + 2) % 4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TileCoord GetOffset(this TileDiagonalPosition position)
        {
            return _diagonalOffsets[(int)position];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TileEdgePosition Next(this TileEdgePosition position)
        {
            return (TileEdgePosition)((int)position).GetNextCircularIndex(4);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TileEdgePosition Prev(this TileEdgePosition position)
        {
            return (TileEdgePosition)((int)position).GetPrevCircularIndex(4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TileEdgePosition Flip(this TileEdgePosition position)
        {
            return (TileEdgePosition)(((int)position + 2) % 4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetRadiusDegrees(this TileEdgePosition position)
        {
            return (int)position * 90 - 90;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetRadiusDegreesF(this TileEdgePosition position)
        {
            return GetRadiusDegrees(position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TileVertexPosition Next(this TileVertexPosition position)
        {
            return (TileVertexPosition)((int)position).GetNextCircularIndex(4);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TileVertexPosition Prev(this TileVertexPosition position)
        {
            return (TileVertexPosition)((int)position).GetPrevCircularIndex(4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TileVertexPosition Flip(this TileVertexPosition position)
        {
            return (TileVertexPosition)(((int)position + 2) % 4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetRadiusDegrees(this TileVertexPosition position)
        {
            return (int)position * 90 - 135;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetRadiusDegreesF(this TileVertexPosition position)
        {
            return GetRadiusDegrees(position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TileCoordF GetOffset(this TileVertexPosition position)
        {
            return _vertexOffsets[(int)position];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TileCoordF64 GetOffsetF64(this TileVertexPosition position)
        {
            return _vertexOffsetsF64[(int)position];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TileVertexPosition ConvertToVertex(this TileDiagonalPosition position)
        {
            return (TileVertexPosition)position;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TileDiagonalPosition ConvertToDiagonal(this TileVertexPosition position)
        {
            return (TileDiagonalPosition)position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TileEdgePosition ConvertToEdge(this TileOrthogonalPosition position)
        {
            return (TileEdgePosition)position;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TileOrthogonalPosition ConvertToOrthogonal(this TileEdgePosition position)
        {
            return (TileOrthogonalPosition)position;
        }

        static readonly TileCoord[] _diagonalOffsets = new TileCoord[]
        {
            new(-1, -1),
            new(1, -1),
            new(1, 1),
            new(-1, 1),
        };
        static readonly TileCoord[] _orthogonalOffsets = new TileCoord[]
        {
            new(0, -1),
            new(1, 0),
            new(0, 1),
            new(-1, 0),
        };

        static readonly TileCoordF[] _vertexOffsets = new TileCoordF[]
        {
            new(-0.5f, -0.5f),
            new(0.5f, -0.5f),
            new(0.5f, 0.5f),
            new(-0.5f, 0.5f),
        };
        static readonly TileCoordF64[] _vertexOffsetsF64 = new TileCoordF64[]
        {
            new(F64.FromFloat(-0.5f), F64.FromFloat(-0.5f)),
            new(F64.FromFloat(0.5f), F64.FromFloat(-0.5f)),
            new(F64.FromFloat(0.5f), F64.FromFloat(0.5f)),
            new(F64.FromFloat(-0.5f), F64.FromFloat(0.5f)),
        };
    }
}
