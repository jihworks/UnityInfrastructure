// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using System.Collections.Generic;

namespace Jih.Unity.Infrastructure.TileGrid
{
    public class TileCell
    {
        public TileMap Map { get; }

        public TileCoord Coord { get; }

        internal TileVertex[] VerticesInternal { get; } = new TileVertex[4];
        /// <summary>
        /// Index: <see cref="TileVertexPosition"/>
        /// </summary>
        public IReadOnlyList<TileVertex> Vertices => VerticesInternal;

        public TileVertex TopLeftVertex => VerticesInternal[(int)TileVertexPosition.TL];
        public TileVertex TopRightVertex => VerticesInternal[(int)TileVertexPosition.TR];
        public TileVertex BottomRightVertex => VerticesInternal[(int)TileVertexPosition.BR];
        public TileVertex BottomLeftVertex => VerticesInternal[(int)TileVertexPosition.BL];

        internal TileEdge[] EdgesInternal { get; } = new TileEdge[4];
        /// <summary>
        /// Index: <see cref="TileEdgePosition"/>
        /// </summary>
        public IReadOnlyList<TileEdge> Edges => EdgesInternal;

        public TileEdge TopEdge => EdgesInternal[(int)TileEdgePosition.T];
        public TileEdge RightEdge => EdgesInternal[(int)TileEdgePosition.R];
        public TileEdge BottomEdge => EdgesInternal[(int)TileEdgePosition.B];
        public TileEdge LeftEdge => EdgesInternal[(int)TileEdgePosition.L];

        internal TileCell?[] OrthogonalsInternal { get; } = new TileCell?[4];
        /// <summary>
        /// Index: <see cref="TileOrthogonalPosition"/>
        /// </summary>
        public IReadOnlyList<TileCell?> Orthogonals => OrthogonalsInternal;

        public TileCell? TopCell => OrthogonalsInternal[(int)TileOrthogonalPosition.T];
        public TileCell? RightCell => OrthogonalsInternal[(int)TileOrthogonalPosition.R];
        public TileCell? BottomCell => OrthogonalsInternal[(int)TileOrthogonalPosition.B];
        public TileCell? LeftCell => OrthogonalsInternal[(int)TileOrthogonalPosition.L];

        internal TileCell?[] DiagonalsInternal { get; } = new TileCell?[4];
        /// <summary>
        /// Index: <see cref="TileDiagonalPosition"/>
        /// </summary>
        public IReadOnlyList<TileCell?> Diagonals => DiagonalsInternal;

        public TileCell? TopLeftCell => DiagonalsInternal[(int)TileDiagonalPosition.TL];
        public TileCell? TopRightCell => DiagonalsInternal[(int)TileDiagonalPosition.TR];
        public TileCell? BottomRightCell => DiagonalsInternal[(int)TileDiagonalPosition.BR];
        public TileCell? BottomLeftCell => DiagonalsInternal[(int)TileDiagonalPosition.BL];

        public TileCell(TileMap map, TileCoord coord)
        {
            Map = map;
            Coord = coord;
        }

        public TileCell? GetOrthogonal(TileOrthogonalPosition position)
        {
            return OrthogonalsInternal[(int)position];
        }

        public TileCell? GetDiagonal(TileDiagonalPosition position)
        {
            return DiagonalsInternal[(int)position];
        }

        public IEnumerable<TileCell> EnumerateOrthogonals()
        {
            foreach (var cell in OrthogonalsInternal)
            {
                if (cell is not null)
                {
                    yield return cell;
                }
            }
        }
        public IEnumerable<TileCell> EnumerateDiagonals()
        {
            foreach (var cell in DiagonalsInternal)
            {
                if (cell is not null)
                {
                    yield return cell;
                }
            }
        }

        public IEnumerable<TileCell> EnumerateAllAdjacents()
        {
            for (int i = 0; i < 4; i++)
            {
                TileCell? cell = DiagonalsInternal[i];
                if (cell is not null)
                {
                    yield return cell;
                }

                cell = OrthogonalsInternal[i];
                if (cell is not null)
                {
                    yield return cell;
                }
            }
        }
    }
}
