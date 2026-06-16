// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System.Collections.Generic;

namespace Jih.Unity.Infrastructure.TileGrid
{
    public class TileVertex
    {
        public TileMap Map { get; }

        public TileVertexIndex Index { get; }
        public TileCoordF Coord { get; }
        public TileCoordF64 Coord64 { get; }

        internal TileCell?[] CellsInternal { get; } = new TileCell?[4];
        /// <summary>
        /// Index: <see cref="TileVertexPosition"/><br/>
        /// Can contain <c>null</c> when this vertex at map border.
        /// </summary>
        public IReadOnlyList<TileCell?> Cells => CellsInternal;

        public TileCell? TopLeftCell => CellsInternal[(int)TileVertexPosition.TL];
        public TileCell? TopRightCell => CellsInternal[(int)TileVertexPosition.TR];
        public TileCell? BottomRightCell => CellsInternal[(int)TileVertexPosition.BR];
        public TileCell? BottomLeftCell => CellsInternal[(int)TileVertexPosition.BL];

        internal TileEdge?[] EdgesInternal { get; } = new TileEdge?[4];
        /// <summary>
        /// Index: <see cref="TileEdgePosition"/><br/>
        /// Means direction of another vertex from this vertex on the edge.<br/>
        /// Can contain <c>null</c> when this vertex at map border.
        /// </summary>
        public IReadOnlyList<TileEdge?> Edges => EdgesInternal;

        public TileEdge? TopEdge => EdgesInternal[(int)TileEdgePosition.T];
        public TileEdge? RightEdge => EdgesInternal[(int)TileEdgePosition.R];
        public TileEdge? BottomEdge => EdgesInternal[(int)TileEdgePosition.B];
        public TileEdge? LeftEdge => EdgesInternal[(int)TileEdgePosition.L];

        public TileVertex(TileMap map, TileVertexIndex index, TileCoordF coord, TileCoordF64 coord64)
        {
            Map = map;
            Index = index;
            Coord = coord;
            Coord64 = coord64;
        }

        public IEnumerable<TileCell> EnumerateCells()
        {
            foreach (var cell in CellsInternal)
            {
                if (cell is not null)
                {
                    yield return cell;
                }
            }
        }
    }
}
