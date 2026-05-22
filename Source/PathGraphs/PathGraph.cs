// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Collections;
using System;
using System.Collections.Generic;

namespace Jih.Unity.Infrastructure.PathGraphs
{
    public static class PathGraph
    {
        /// <remarks>
        /// The method itself is thread-safe. But the <paramref name="result"/> is not. Do not share it between threads.<br/>
        /// Also, do not modify the objects involved with this map while the method is running. It may cause undefined behavior.
        /// </remarks>
        public static bool FindPath<TPathNode>(TPathNode start, TPathNode goal, PathResult<TPathNode> result, PathAccess<TPathNode> access, PathCost<TPathNode>? cost, PathHeuristic<TPathNode>? heuristic) where TPathNode : class
        {
            result.Clear();

            Dictionary<TPathNode, TPathNode> connections = result.Connections;

            Dictionary<TPathNode, int> costs = result.Costs;
            costs[start] = 0;

            PriorityQueue<TPathNode, int> frontiers = result.Frontiers;
            frontiers.Enqueue(start, 0);

            HashSet<TPathNode> visited = result.Visited;

            cost ??= ((current, next) => 1);
            heuristic ??= ((goal, next) => 0);

            while (frontiers.Count > 0)
            {
                TPathNode current = frontiers.Dequeue();

                if (!visited.Add(current))
                {
                    continue;
                }

                if (current == goal)
                {
                    List<TPathNode> resultPath = result.ResultPath;

                    TPathNode currentTrack = goal;
                    while (currentTrack is not null)
                    {
                        resultPath.Add(currentTrack);

                        if (!connections.TryGetValue(currentTrack, out TPathNode prev))
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

    /// <remarks>
    /// Should return next path candidate cells except not accessable from the <paramref name="current"/>.
    /// </remarks>
    public delegate IEnumerable<TPathNode> PathAccess<TPathNode>(TPathNode current) where TPathNode : class;
    /// <remarks>
    /// Default implmentation is <c>1</c>.<br/>
    /// Should be bigger or eqaul than 1. It will be maximized by 1. Lower value is better.
    /// </remarks>
    public delegate int PathCost<TPathNode>(TPathNode current, TPathNode next) where TPathNode : class;
    /// <remarks>
    /// Default implmentation is <c>0</c>.<br/>
    /// Should be bigger or eqaul than 0. It will be maximized by 0. Lower value is better.
    /// </remarks>
    public delegate int PathHeuristic<TPathNode>(TPathNode goal, TPathNode next) where TPathNode : class;

    /// <summary>
    /// This class contains caches for path finding and the result path.
    /// </summary>
    /// <remarks>
    /// <c>NOT</c> thread-safe.
    /// </remarks>
    public class PathResult<TPathNode> where TPathNode : class
    {
        internal Dictionary<TPathNode, TPathNode> Connections { get; } = new();
        internal Dictionary<TPathNode, int> Costs { get; } = new();
        internal PriorityQueue<TPathNode, int> Frontiers { get; } = new();
        internal HashSet<TPathNode> Visited { get; } = new();

        /// <summary>
        /// Including start node and goal node.
        /// </summary>
        public List<TPathNode> ResultPath { get; } = new();
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
