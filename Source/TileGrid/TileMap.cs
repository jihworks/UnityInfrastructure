// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Collections;
using Jih.Unity.Infrastructure.Deterministics;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Jih.Unity.Infrastructure.TileGrid
{
    /// <summary>
    /// Grid structure of square tiles in 2D euclidean space.
    /// </summary>
    /// <remarks>
    /// Using Screen Coordinate System<br/>
    /// +X is right, +Y is down.<br/>
    /// Left-Top corner is origin(0, 0).<br/>
    /// Positive rotation is CW.<br/>
    /// <br/>
    /// Assuming all of cells are filled in the grid. Hole is not supported.
    /// </remarks>
    public class TileMap
    {
        public int Width { get; }
        public int Height { get; }

        public TileCell this[TileCoord coord] => GetCell(coord) ?? throw new ArgumentOutOfRangeException(nameof(coord), $"Invalid cell coordinate: {coord}");
        public TileVertex this[TileVertexIndex index] => GetVertex(index) ?? throw new ArgumentOutOfRangeException(nameof(index), $"Invalid vertex coordinate: {index}");
        public TileEdge this[TileEdgeIndex index] => GetEdge(index) ?? throw new ArgumentOutOfRangeException(nameof(index), $"Invalid edge coordinate: {index}");

        readonly TileCell[,] _cells;
        readonly TileVertex[,] _vertices;
        readonly TileEdge[,] _horizontalEdges, _verticalEdges;

        public TileMap(int width, int height, CreateTileCellDelegate? createTileCell, CreateTileVertexDelegate? createTileVertex, CreateTileEdgeDelegate? createTileEdge)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive.");
            }
            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive.");
            }

            Width = width;
            Height = height;

            createTileCell ??= ((map, coord) => new TileCell(map, coord));
            createTileVertex ??= ((map, index, coord, coordF64) => new TileVertex(map, index, coord, coordF64));
            createTileEdge ??= ((index, v0, v1, rCell) => new TileEdge(index, v0, v1, rCell));

            _cells = new TileCell[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    _cells[y, x] = createTileCell(this, new TileCoord(x, y));
                }
            }

            _vertices = new TileVertex[height + 1, width + 1];
            TileCoordF vertexCoordOffset = new(-0.5f, -0.5f);
            TileCoordF64 vertexCoordOffsetF64 = new(F64.FromFloat(-0.5f), F64.FromFloat(-0.5f));
            for (int y = 0; y <= height; y++)
            {
                for (int x = 0; x <= width; x++)
                {
                    TileCoordF coord = new(x, y);
                    TileCoordF64 coordF64 = new(x, y);
                    _vertices[y, x] = createTileVertex(this, new TileVertexIndex(x, y), coord + vertexCoordOffset, coordF64 + vertexCoordOffsetF64);
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    TileCell cell = _cells[y, x];

                    TileCoord cellCoord = cell.Coord;

                    TileVertex tl = _vertices[cellCoord.Y, cellCoord.X];
                    TileVertex tr = _vertices[cellCoord.Y, cellCoord.X + 1];
                    TileVertex br = _vertices[cellCoord.Y + 1, cellCoord.X + 1];
                    TileVertex bl = _vertices[cellCoord.Y + 1, cellCoord.X];

                    cell.VerticesInternal[(int)TileVertexPosition.TL] = tl;
                    cell.VerticesInternal[(int)TileVertexPosition.TR] = tr;
                    cell.VerticesInternal[(int)TileVertexPosition.BR] = br;
                    cell.VerticesInternal[(int)TileVertexPosition.BL] = bl;

                    tl.CellsInternal[(int)TileVertexPosition.BR] = cell;
                    tr.CellsInternal[(int)TileVertexPosition.BL] = cell;
                    br.CellsInternal[(int)TileVertexPosition.TL] = cell;
                    bl.CellsInternal[(int)TileVertexPosition.TR] = cell;
                }
            }

            _horizontalEdges = new TileEdge[height + 1, width];
            _verticalEdges = new TileEdge[height, width + 1];
            // Horizontal edges.
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    TileVertex v0 = _vertices[y, x];
                    TileVertex v1 = _vertices[y, x + 1];
                    TileCell cell = _cells[y, x];
                    TileEdge top = createTileEdge(new TileEdgeIndex(TileEdgeOrientation.TB, x, y), v0, v1, cell);
                    _horizontalEdges[y, x] = top;

                    if (y > 0)
                    {
                        TileCell cellAbove = _cells[y - 1, x];
                        top.LeftCell = cellAbove;
                    }

                    v0.EdgesInternal[(int)TileEdgePosition.R] = top;
                    v1.EdgesInternal[(int)TileEdgePosition.L] = top;
                }
            }
            for (int x = 0; x < width; x++)
            {
                TileVertex v0 = _vertices[height, x + 1];
                TileVertex v1 = _vertices[height, x];
                TileCell cell = _cells[height - 1, x];
                TileEdge bottom = createTileEdge(new TileEdgeIndex(TileEdgeOrientation.TB, x, height), v0, v1, cell);
                _horizontalEdges[height, x] = bottom;

                v0.EdgesInternal[(int)TileEdgePosition.L] = bottom;
                v1.EdgesInternal[(int)TileEdgePosition.R] = bottom;
            }
            // Vertical edges.
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    TileVertex v0 = _vertices[y + 1, x];
                    TileVertex v1 = _vertices[y, x];
                    TileCell cell = _cells[y, x];
                    TileEdge left = createTileEdge(new TileEdgeIndex(TileEdgeOrientation.LR, x, y), v0, v1, cell);
                    _verticalEdges[y, x] = left;

                    if (x > 0)
                    {
                        TileCell cellLeft = _cells[y, x - 1];
                        left.LeftCell = cellLeft;
                    }

                    v0.EdgesInternal[(int)TileEdgePosition.T] = left;
                    v1.EdgesInternal[(int)TileEdgePosition.B] = left;
                }
            }
            for (int y = 0; y < height; y++)
            {
                TileVertex v0 = _vertices[y, width];
                TileVertex v1 = _vertices[y + 1, width];
                TileCell cell = _cells[y, width - 1];
                TileEdge right = createTileEdge(new TileEdgeIndex(TileEdgeOrientation.LR, width, y), v0, v1, cell);
                _verticalEdges[y, width] = right;

                v0.EdgesInternal[(int)TileEdgePosition.B] = right;
                v1.EdgesInternal[(int)TileEdgePosition.T] = right;
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    TileCell cell = _cells[y, x];

                    TileEdge top = _horizontalEdges[y, x];
                    TileEdge bottom = _horizontalEdges[y + 1, x];
                    cell.EdgesInternal[(int)TileEdgePosition.T] = top;
                    cell.EdgesInternal[(int)TileEdgePosition.B] = bottom;

                    TileEdge left = _verticalEdges[y, x];
                    TileEdge right = _verticalEdges[y, x + 1];
                    cell.EdgesInternal[(int)TileEdgePosition.L] = left;
                    cell.EdgesInternal[(int)TileEdgePosition.R] = right;
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    TileCell cell = _cells[y, x];

                    int l = x - 1, t = y - 1;
                    int r = x + 1, b = y + 1;

                    bool vl = 0 <= l, vr = r < width;
                    bool vt = 0 <= t, vb = b < height;

                    if (vl)
                    {
                        TileCell other = _cells[y, l];
                        cell.OrthogonalsInternal[(int)TileOrthogonalPosition.L] = _cells[y, l];
                    }
                    if (vr)
                    {
                        cell.OrthogonalsInternal[(int)TileOrthogonalPosition.R] = _cells[y, r];
                    }
                    if (vt)
                    {
                        cell.OrthogonalsInternal[(int)TileOrthogonalPosition.T] = _cells[t, x];
                    }
                    if (vb)
                    {
                        cell.OrthogonalsInternal[(int)TileOrthogonalPosition.B] = _cells[b, x];
                    }

                    if (vt && vl)
                    {
                        cell.DiagonalsInternal[(int)TileDiagonalPosition.TL] = _cells[t, l];
                    }
                    if (vt && vr)
                    {
                        cell.DiagonalsInternal[(int)TileDiagonalPosition.TR] = _cells[t, r];
                    }
                    if (vb && vr)
                    {
                        cell.DiagonalsInternal[(int)TileDiagonalPosition.BR] = _cells[b, r];
                    }
                    if (vb && vl)
                    {
                        cell.DiagonalsInternal[(int)TileDiagonalPosition.BL] = _cells[b, l];
                    }
                }
            }
        }

        public TileCell? GetCell(TileCoord coord)
        {
            return GetCell(coord.X, coord.Y);
        }
        public TileCell? GetCell(int coordX, int coordY)
        {
            if (0 <= coordX && coordX < Width && 0 <= coordY && coordY < Height)
            {
                return _cells[coordY, coordX];
            }
            else
            {
                return null;
            }
        }

        public TileVertex? GetVertex(TileVertexIndex index)
        {
            return GetVertex(index.X, index.Y);
        }
        public TileVertex? GetVertex(int indexX, int indexY)
        {
            if (0 <= indexX && indexX <= Width && 0 <= indexY && indexY <= Height)
            {
                return _vertices[indexY, indexX];
            }
            else
            {
                return null;
            }
        }

        public TileEdge? GetEdge(TileEdgeIndex index)
        {
            return GetEdge(index.Orientation, index.X, index.Y);
        }
        public TileEdge? GetEdge(TileEdgeOrientation orientation, int indexX, int indexY)
        {
            switch (orientation)
            {
                case TileEdgeOrientation.TB:
                    if (0 <= indexX && indexX < Width && 0 <= indexY && indexY <= Height)
                    {
                        return _horizontalEdges[indexY, indexX];
                    }
                    return null;

                case TileEdgeOrientation.LR:
                    if (0 <= indexX && indexX <= Width && 0 <= indexY && indexY < Height)
                    {
                        return _verticalEdges[indexY, indexX];
                    }
                    return null;

                default: throw new ArgumentException($"Invalid edge orientation: {orientation}", nameof(orientation));
            }
        }

        public IEnumerable<TileCell> EnumerateCells()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return _cells[y, x];
                }
            }
        }

        public IEnumerable<TileVertex> EnumerateVertices()
        {
            for (int y = 0; y <= Height; y++)
            {
                for (int x = 0; x <= Width; x++)
                {
                    yield return _vertices[y, x];
                }
            }
        }

        public IEnumerable<TileEdge> EnuerateEdges()
        {
            for (int y = 0; y <= Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return _horizontalEdges[y, x];
                }
            }
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x <= Width; x++)
                {
                    yield return _verticalEdges[y, x];
                }
            }
        }

        /// <remarks>
        /// The method itself is thread-safe. But the <paramref name="result"/> is not. Do not share it between threads.<br/>
        /// Also, do not modify the objects involved with this map while the method is running. It may cause undefined behavior.
        /// </remarks>
        public bool FindPath(TileCell start, TileCell goal, TilePathResult result, TilePathAccess? access, TilePathCost? cost, TilePathHeuristic? heuristic)
        {
            if (start.Map != this)
            {
                throw new InvalidOperationException($"The start cell '{start.Coord}' does not exist in the map.");
            }
            if (goal.Map != this)
            {
                throw new InvalidOperationException($"The goal cell '{goal.Coord}' does not exist in the map.");
            }

            result.Clear();

            Dictionary<TileCell, TileCell> connections = result.Connections;

            Dictionary<TileCell, int> costs = result.Costs;
            costs[start] = 0;

            PriorityQueue<TileCell, int> frontiers = result.Frontiers;
            frontiers.Enqueue(start, 0);

            HashSet<TileCell> visited = result.Visited;

            access ??= (current) => current.EnumerateOrthogonals();
            cost ??= ((current, next) => 1);
            heuristic ??= ((goal, next) => 0);

            while (frontiers.Count > 0)
            {
                TileCell current = frontiers.Dequeue();

                if (!visited.Add(current))
                {
                    continue;
                }

                if (current == goal)
                {
                    List<TileCell> resultPath = result.ResultPath;

                    TileCell currentTrack = goal;
                    while (currentTrack is not null)
                    {
                        resultPath.Add(currentTrack);

                        if (!connections.TryGetValue(currentTrack, out TileCell prev))
                        {
                            break;
                        }

                        currentTrack = prev;
                    }

                    if (resultPath[^1] != start)
                    {
                        throw new InvalidOperationException();
                    }

                    resultPath.Reverse();

                    result.IsSucceed = true;
                    return true;
                }

                foreach (var next in access(current))
                {
                    if (next == current)
                    {
                        continue;
                    }

                    if (next.Map != this)
                    {
                        throw new InvalidOperationException($"The next accessing cell '{next.Coord}' does not exist in the map.");
                    }

                    int newCost = costs[current] + Math.Max(1, cost(current, next))/*Must not smaller than 1*/;

                    if (costs.TryGetValue(next, out int prevCost) && newCost >= prevCost)
                    {
                        continue;
                    }

                    costs[next] = newCost;
                    frontiers.Enqueue(next, newCost + Math.Max(0, heuristic(goal, next))/*Must not smaller than 0*/);
                    connections[next] = current;
                }
            }

            result.IsSucceed = false;
            return false;
        }
    }
}
