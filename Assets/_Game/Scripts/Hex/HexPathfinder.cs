using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// A* pathfinding on the hex grid. Uses NativeArray for DOTS-compatibility.
    /// Supports terrain costs, impassable tiles, and movement budget constraints.
    /// </summary>
    public static class HexPathfinder
    {
        private const int MaxPathLength = 256;

        /// <summary>
        /// Finds the shortest path from start to goal using A*.
        /// Returns null if no path exists or if the goal is unreachable within the budget.
        /// </summary>
        /// <param name="grid">The hex grid manager providing tile data.</param>
        /// <param name="startQ">Starting tile Q coordinate.</param>
        /// <param name="startR">Starting tile R coordinate.</param>
        /// <param name="goalQ">Goal tile Q coordinate.</param>
        /// <param name="goalR">Goal tile R coordinate.</param>
        /// <param name="maxCost">Maximum total movement cost allowed (movement budget). 0 = unlimited.</param>
        /// <returns>Array of HexCoords from start to goal (inclusive), or null if no path.</returns>
        public static HexCoords[] FindPath(
            HexGridManager grid,
            int startQ, int startR,
            int goalQ, int goalR,
            float maxCost = 0f)
        {
            if (grid == null || !grid.IsGenerated) return null;

            var start = new HexCoords(startQ, startR);
            var goal = new HexCoords(goalQ, goalR);

            var startTile = grid.GetTile(startQ, startR);
            var goalTile = grid.GetTile(goalQ, goalR);

            if (startTile == null || goalTile == null) return null;
            if (goalTile.IsImpassable) return null;

            // Use axial coordinate hashing for the closed set
            int width = grid.Width;
            int height = grid.Height;
            int totalTiles = width * height;

            using var gScore = new NativeArray<float>(totalTiles, Allocator.TempJob, NativeArrayOptions.ClearMemory);
            using var cameFrom = new NativeArray<int>(totalTiles, Allocator.TempJob, NativeArrayOptions.ClearMemory);
            using var inOpenSet = new NativeArray<bool>(totalTiles, Allocator.TempJob, NativeArrayOptions.ClearMemory);

            // Initialize gScore to infinity
            for (int i = 0; i < totalTiles; i++)
            {
                gScore[i] = float.MaxValue;
                cameFrom[i] = -1;
            }

            int startIdx = AxialToIndex(startQ, startR, width, height);
            int goalIdx = AxialToIndex(goalQ, goalR, width, height);

            if (startIdx < 0 || goalIdx < 0) return null;

            gScore[startIdx] = 0f;

            // Priority queue using sorted list (binary search insert)
            var openSet = new SortedSet<(float fScore, int index)>(Comparer<(float, int)>.Create((a, b) =>
            {
                int cmp = a.Item1.CompareTo(b.Item1);
                return cmp != 0 ? cmp : a.Item2.CompareTo(b.Item2);
            }));

            openSet.Add((Heuristic(start, goal), startIdx));
            inOpenSet[startIdx] = true;

            while (openSet.Count > 0)
            {
                var current = openSet.Min;
                openSet.Remove(current);
                int currentIdx = current.index;
                inOpenSet[currentIdx] = false;

                if (currentIdx == goalIdx)
                {
                    // Reconstruct path
                    return ReconstructPath(cameFrom, currentIdx, width, height, start);
                }

                var currentCoords = IndexToAxial(currentIdx, width, height);
                float currentG = gScore[currentIdx];

                // Check all 6 neighbors
                for (int dir = 0; dir < 6; dir++)
                {
                    var neighbor = currentCoords.Neighbour(dir);
                    int nIdx = AxialToIndex(neighbor.Q, neighbor.R, width, height);
                    if (nIdx < 0 || nIdx >= totalTiles) continue;

                    var neighborTile = grid.GetTile(neighbor.Q, neighbor.R);
                    if (neighborTile == null || neighborTile.IsImpassable) continue;

                    float tentativeG = currentG + neighborTile.MovementCost;

                    // Check movement budget
                    if (maxCost > 0f && tentativeG > maxCost) continue;

                    if (tentativeG < gScore[nIdx])
                    {
                        gScore[nIdx] = tentativeG;
                        cameFrom[nIdx] = currentIdx;

                        float fScore = tentativeG + Heuristic(neighbor, goal);

                        if (inOpenSet[nIdx])
                        {
                            // Remove old entry and re-add with updated fScore
                            openSet.RemoveWhere(t => t.index == nIdx);
                        }

                        openSet.Add((fScore, nIdx));
                        inOpenSet[nIdx] = true;
                    }
                }
            }

            return null; // No path found
        }

        /// <summary>
        /// Finds all tiles reachable from (startQ, startR) within the given movement budget.
        /// Returns a dictionary of tile coords to their movement cost.
        /// </summary>
        public static Dictionary<HexCoords, float> FindReachableTiles(
            HexGridManager grid,
            int startQ, int startR,
            float maxCost)
        {
            var result = new Dictionary<HexCoords, float>();
            if (grid == null || !grid.IsGenerated || maxCost <= 0f) return result;

            var startTile = grid.GetTile(startQ, startR);
            if (startTile == null || startTile.IsImpassable) return result;

            int width = grid.Width;
            int height = grid.Height;
            int totalTiles = width * height;

            using var gScore = new NativeArray<float>(totalTiles, Allocator.TempJob, NativeArrayOptions.ClearMemory);

            for (int i = 0; i < totalTiles; i++)
                gScore[i] = float.MaxValue;

            int startIdx = AxialToIndex(startQ, startR, width, height);
            if (startIdx < 0) return result;

            gScore[startIdx] = 0f;

            var openSet = new SortedSet<(float gScore, int index)>(Comparer<(float, int)>.Create((a, b) =>
            {
                int cmp = a.Item1.CompareTo(b.Item1);
                return cmp != 0 ? cmp : a.Item2.CompareTo(b.Item2);
            }));

            openSet.Add((0f, startIdx));

            while (openSet.Count > 0)
            {
                var current = openSet.Min;
                openSet.Remove(current);
                int currentIdx = current.index;
                float currentG = current.gScore;

                var currentCoords = IndexToAxial(currentIdx, width, height);

                // Add to result if within budget
                if (currentG <= maxCost)
                {
                    result[currentCoords] = currentG;
                }

                for (int dir = 0; dir < 6; dir++)
                {
                    var neighbor = currentCoords.Neighbour(dir);
                    int nIdx = AxialToIndex(neighbor.Q, neighbor.R, width, height);
                    if (nIdx < 0 || nIdx >= totalTiles) continue;

                    var neighborTile = grid.GetTile(neighbor.Q, neighbor.R);
                    if (neighborTile == null || neighborTile.IsImpassable) continue;

                    float tentativeG = currentG + neighborTile.MovementCost;
                    if (tentativeG > maxCost) continue;

                    if (tentativeG < gScore[nIdx])
                    {
                        gScore[nIdx] = tentativeG;
                        openSet.Add((tentativeG, nIdx));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Heuristic for hex grid: axial distance.
        /// Admissible and consistent for A*.
        /// </summary>
        private static float Heuristic(HexCoords a, HexCoords b)
        {
            return HexCoords.Distance(a, b);
        }

        private static HexCoords[] ReconstructPath(
            NativeArray<int> cameFrom,
            int currentIdx,
            int width, int height,
            HexCoords start)
        {
            var path = new List<HexCoords>();
            int idx = currentIdx;

            while (idx >= 0)
            {
                path.Add(IndexToAxial(idx, width, height));
                idx = cameFrom[idx];
            }

            path.Reverse();

            // Safety: limit path length
            if (path.Count > MaxPathLength)
            {
                path.RemoveRange(MaxPathLength, path.Count - MaxPathLength);
            }

            return path.ToArray();
        }

        private static int AxialToIndex(int q, int r, int width, int height)
        {
            int col = q + width / 2;
            int row = r + height / 2;
            if (col < 0 || col >= width || row < 0 || row >= height) return -1;
            return row * width + col;
        }

        private static HexCoords IndexToAxial(int index, int width, int height)
        {
            int row = index / width;
            int col = index % width;
            int q = col - width / 2;
            int r = row - height / 2;
            return new HexCoords(q, r);
        }
    }
}
