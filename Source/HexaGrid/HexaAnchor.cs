// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Jih.Unity.Infrastructure.HexaGrid
{
    public readonly struct HexaAnchor
    {
        public readonly HexaMap? Map;
        public readonly HexaCoord Coord;

        public HexaAnchor(HexaCoord coord)
        {
            Map = null;
            Coord = coord;
        }
        public HexaAnchor(HexaMap? map, HexaCoord coord)
        {
            Map = map;
            Coord = coord;
        }

        public HexaCoord GetNeighborCoord(HexaNeighborPosition position)
        {
            HexaCoord offset = position.GetOffset();
            return Coord + offset;
        }
        /// <remarks>
        /// <see cref="Map"/> required.
        /// </remarks>
        public HexaCell? GetNeighbor(HexaNeighborPosition position)
        {
            HexaMap map = CheckMap(nameof(GetNeighbor));
            return map.GetCell(GetNeighborCoord(position));
        }
        /// <remarks>
        /// <see cref="Map"/> required.
        /// </remarks>
        public bool TryGetNeighborPosition(HexaCell cell, out HexaNeighborPosition position)
        {
            for (int p = 0; p < 6; p++)
            {
                HexaNeighborPosition np = (HexaNeighborPosition)p;
                HexaCell? neighbor = GetNeighbor(np);
                if (neighbor is not null && neighbor == cell)
                {
                    position = np;
                    return true;
                }
            }

            position = default;
            return false;
        }
        /// <remarks>
        /// <see cref="Map"/> required.
        /// </remarks>
        public HexaNeighborPosition GetNeighborPosition(HexaCell cell)
        {
            if (!TryGetNeighborPosition(cell, out HexaNeighborPosition result))
            {
                throw new InvalidOperationException($"Cell {cell.Coord} is not a neighbor of the cell {Coord}.");
            }
            return result;
        }

        public HexaCoord GetDiagonalCoord(HexaDiagonalPosition position)
        {
            HexaCoord offset = position.GetOffset();
            return Coord + offset;
        }
        /// <remarks>
        /// <see cref="Map"/> required.
        /// </remarks>
        public HexaCell? GetDiagonal(HexaDiagonalPosition position)
        {
            HexaMap map = CheckMap(nameof(GetDiagonal));
            return map.GetCell(GetDiagonalCoord(position));
        }
        /// <remarks>
        /// <see cref="Map"/> required.
        /// </remarks>
        public bool TryGetDiagonalPosition(HexaCell cell, out HexaDiagonalPosition position)
        {
            for (int p = 0; p < 6; p++)
            {
                HexaDiagonalPosition dp = (HexaDiagonalPosition)p;
                HexaCell? neighbor = GetDiagonal(dp);
                if (neighbor is not null && neighbor == cell)
                {
                    position = dp;
                    return true;
                }
            }

            position = default;
            return false;
        }
        /// <remarks>
        /// <see cref="Map"/> required.
        /// </remarks>
        public HexaDiagonalPosition GetDiagonalPosition(HexaCell cell)
        {
            if (!TryGetDiagonalPosition(cell, out HexaDiagonalPosition result))
            {
                throw new InvalidOperationException($"Cell {cell.Coord} is not a diagonal of the cell {Coord}.");
            }
            return result;
        }

        /// <remarks>
        /// <see cref="Map"/> required.
        /// </remarks>
        public IEnumerable<HexaCell> EnumerateNeighbors()
        {
            for (int p = 0; p < 6; p++)
            {
                HexaCell? neighbor = GetNeighbor((HexaNeighborPosition)p);
                if (neighbor is null)
                {
                    continue;
                }
                yield return neighbor;
            }
        }

        public HexaCoordF GetVertexCoord(HexaVertexPosition position)
        {
            HexaCoordF offset = position.GetOffset();
            return Coord + offset;
        }
        public HexaCoordF64 GetVertexCoordF64(HexaVertexPosition position)
        {
            HexaCoordF64 offset = position.GetOffsetF64();
            return Coord + offset;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        HexaMap CheckMap(string context)
        {
            if (Map is null)
            {
                throw new InvalidOperationException($"Cannot '{context}' without {nameof(Map)} reference.");
            }
            return Map;
        }
    }
}
