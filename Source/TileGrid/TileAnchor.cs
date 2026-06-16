// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Jih.Unity.Infrastructure.TileGrid
{
    public readonly struct TileAnchor
    {
        public readonly TileMap? Map;
        public readonly TileCoord Coord;

        public TileAnchor(TileCoord coord)
        {
            Map = null;
            Coord = coord;
        }
        public TileAnchor(TileMap? map, TileCoord coord)
        {
            Map = map;
            Coord = coord;
        }

        public TileCoord GetOrthogonalCoord(TileOrthogonalPosition position)
        {
            return Coord + position.GetOffset();
        }

        /// <remarks>
        /// <see cref="Map"/> required.
        /// </remarks>
        public TileCell? GetOrthogonal(TileOrthogonalPosition position)
        {
            return CheckMap().GetCell(GetOrthogonalCoord(position));
        }

        public TileCoord GetDiagonalCoord(TileDiagonalPosition position)
        {
            return Coord + position.GetOffset();
        }

        /// <remarks>
        /// <see cref="Map"/> required.
        /// </remarks>
        public TileCell? GetDiagonal(TileDiagonalPosition position)
        {
            return CheckMap().GetCell(GetDiagonalCoord(position));
        }

        /// <remarks>
        /// <see cref="Map"/> required.
        /// </remarks>
        public IEnumerable<TileCell> EnumerateOrthogonals()
        {
            for (int p = 0; p < 4; p++)
            {
                TileOrthogonalPosition op = (TileOrthogonalPosition)p;
                TileCell? cell = GetOrthogonal(op);
                if (cell is null)
                {
                    continue;
                }
                yield return cell;
            }
        }
        /// <remarks>
        /// <see cref="Map"/> required.
        /// </remarks>
        public IEnumerable<TileCell> EnumerateDiagonals()
        {
            for (int p = 0; p < 4; p++)
            {
                TileDiagonalPosition dp = (TileDiagonalPosition)p;
                TileCell? cell = GetDiagonal(dp);
                if (cell is null)
                {
                    continue;
                }
                yield return cell;
            }
        }
        /// <remarks>
        /// <see cref="Map"/> required.
        /// </remarks>
        public IEnumerable<TileCell> EnumerateAllAdjacents()
        {
            for (int p = 0; p < 4; p++)
            {
                TileDiagonalPosition dp = (TileDiagonalPosition)p;
                TileCell? d = GetDiagonal(dp);
                if (d is null)
                {
                    continue;
                }
                yield return d;

                TileOrthogonalPosition op = (TileOrthogonalPosition)p;
                TileCell? o = GetOrthogonal(op);
                if (o is null)
                {
                    continue;
                }
                yield return o;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TileMap CheckMap([CallerMemberName] string context = nameof(TileAnchor))
        {
            if (Map is null)
            {
                throw new InvalidOperationException($"Cannot '{context}' without {nameof(Map)} reference.");
            }
            return Map;
        }
    }
}
