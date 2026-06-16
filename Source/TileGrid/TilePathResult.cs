// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Collections;
using System.Collections.Generic;

namespace Jih.Unity.Infrastructure.TileGrid
{
    /// <summary>
    /// This class contains caches for path finding and the result path.
    /// </summary>
    /// <remarks>
    /// <c>NOT</c> thread-safe.
    /// </remarks>
    public class TilePathResult
    {
        internal Dictionary<TileCell, TileCell> Connections { get; } = new();
        internal Dictionary<TileCell, int> Costs { get; } = new();
        internal PriorityQueue<TileCell, int> Frontiers { get; } = new();
        internal HashSet<TileCell> Visited { get; } = new();

        /// <summary>
        /// Including start cell and goal cell.
        /// </summary>
        public List<TileCell> ResultPath { get; } = new();
        public bool IsSucceed { get; internal set; }

        public void Clear()
        {
            Connections.Clear();
            Costs.Clear();
            Frontiers.Clear();
            Visited.Clear();

            ResultPath.Clear();
            IsSucceed = false;
        }
    }
}
