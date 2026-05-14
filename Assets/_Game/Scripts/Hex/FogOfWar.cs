using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// Visibility states for fog of war.
    /// </summary>
    public enum VisibilityState
    {
        Hidden = 0,      // Never explored
        Explored = 1,    // Was visible, now out of sight
        Visible = 2,     // Currently in line of sight
    }

    /// <summary>
    /// Fog of War system using shadowcasting on hex grid.
    /// Maintains visibility state per tile and updates when units move.
    /// Uses Burst-compatible shadowcasting algorithm adapted for hex grids.
    /// </summary>
    public class FogOfWar : MonoBehaviour
    {
        [SerializeField] private int _defaultVisionRange = 4;

        private HexGridManager _grid;
        private VisibilityState[] _visibility;
        private int _width;
        private int _height;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// Event raised when visibility changes for a tile.
        /// Parameters: (q, r, newState)
        /// </summary>
        public event Action<int, int, VisibilityState> OnVisibilityChanged;

        public void Initialize(HexGridManager grid)
        {
            _grid = grid;
            if (_grid == null || !_grid.IsGenerated) return;

            _width = _grid.Width;
            _height = _grid.Height;
            _visibility = new VisibilityState[_width * _height];

            // All tiles start as Hidden
            Array.Fill(_visibility, VisibilityState.Hidden);
            _isInitialized = true;

            Debug.Log($"[FogOfWar] Initialized for {_width}x{_height} grid.");
        }

        /// <summary>
        /// Updates visibility from a single observer position.
        /// Tiles within vision range become Visible, previously visible tiles become Explored.
        /// </summary>
        public void UpdateVisibility(int observerQ, int observerR, int? visionRange = null)
        {
            if (!_isInitialized || _grid == null) return;

            int range = visionRange ?? _defaultVisionRange;

            // First: mark all currently visible tiles as Explored
            for (int i = 0; i < _visibility.Length; i++)
            {
                if (_visibility[i] == VisibilityState.Visible)
                {
                    _visibility[i] = VisibilityState.Explored;
                }
            }

            // Run shadowcasting from observer position
            var newlyVisible = Shadowcast(observerQ, observerR, range);

            foreach (var (q, r) in newlyVisible)
            {
                int idx = AxialToIndex(q, r);
                if (idx < 0 || idx >= _visibility.Length) continue;

                var oldState = _visibility[idx];
                _visibility[idx] = VisibilityState.Visible;

                if (oldState != VisibilityState.Visible)
                {
                    OnVisibilityChanged?.Invoke(q, r, VisibilityState.Visible);
                }
            }
        }

        /// <summary>
        /// Updates visibility from multiple observer positions (e.g. all units of a sect).
        /// </summary>
        public void UpdateVisibilityMulti(IEnumerable<(int q, int r, int range)> observers)
        {
            if (!_isInitialized || _grid == null) return;

            // Mark all currently visible as Explored
            for (int i = 0; i < _visibility.Length; i++)
            {
                if (_visibility[i] == VisibilityState.Visible)
                {
                    _visibility[i] = VisibilityState.Explored;
                }
            }

            foreach (var (q, r, range) in observers)
            {
                var visible = Shadowcast(q, r, range);
                foreach (var (vq, vr) in visible)
                {
                    int idx = AxialToIndex(vq, vr);
                    if (idx < 0 || idx >= _visibility.Length) continue;

                    var oldState = _visibility[idx];
                    _visibility[idx] = VisibilityState.Visible;

                    if (oldState != VisibilityState.Visible)
                    {
                        OnVisibilityChanged?.Invoke(vq, vr, VisibilityState.Visible);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the visibility state of a tile.
        /// </summary>
        public VisibilityState GetVisibility(int q, int r)
        {
            int idx = AxialToIndex(q, r);
            if (idx < 0 || idx >= _visibility.Length) return VisibilityState.Hidden;
            return _visibility[idx];
        }

        /// <summary>
        /// Checks if a tile is currently visible (in line of sight).
        /// </summary>
        public bool IsVisible(int q, int r)
        {
            return GetVisibility(q, r) == VisibilityState.Visible;
        }

        /// <summary>
        /// Checks if a tile has ever been explored.
        /// </summary>
        public bool IsExplored(int q, int r)
        {
            var state = GetVisibility(q, r);
            return state == VisibilityState.Visible || state == VisibilityState.Explored;
        }

        /// <summary>
        /// Reveals the entire map (debug/cheat).
        /// </summary>
        public void RevealAll()
        {
            if (!_isInitialized) return;
            for (int i = 0; i < _visibility.Length; i++)
            {
                _visibility[i] = VisibilityState.Visible;
            }
        }

        /// <summary>
        /// Hex shadowcasting algorithm.
        /// Returns all tiles visible from (centerQ, centerR) within the given range.
        /// Uses a simplified radial sweep suitable for hex grids.
        /// </summary>
        private List<(int q, int r)> Shadowcast(int centerQ, int centerR, int range)
        {
            var visible = new List<(int, int)>();

            // Center is always visible
            visible.Add((centerQ, centerR));

            if (range <= 0) return visible;

            // For each ring from 1 to range, check line of sight
            for (int ring = 1; ring <= range; ring++)
            {
                // Walk the ring starting from the "northwest" position
                var ringCoords = GetRing(centerQ, centerR, ring);

                foreach (var (q, r) in ringCoords)
                {
                    if (HasLineOfSight(centerQ, centerR, q, r, range))
                    {
                        visible.Add((q, r));
                    }
                }
            }

            return visible;
        }

        /// <summary>
        /// Checks if there's a clear line of sight from (q1,r1) to (q2,r2).
        /// Uses hex Bresenham-like line drawing to check for blocking tiles.
        /// </summary>
        private bool HasLineOfSight(int q1, int r1, int q2, int r2, int range)
        {
            // Use hex line drawing to check each tile along the path
            var line = HexLine(q1, r1, q2, r2);

            for (int i = 1; i < line.Count - 1; i++) // Skip start and end
            {
                var (lq, lr) = line[i];
                var tile = _grid.GetTile(lq, lr);
                if (tile == null) return false;

                // High elevation and certain features block vision
                if (tile.Terrain?.Type == TerrainType.Mountain && tile.Elevation >= ElevationLevel.High)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets all hex coordinates at exactly `range` distance from center.
        /// </summary>
        private List<(int q, int r)> GetRing(int centerQ, int centerR, int range)
        {
            var result = new List<(int, int)>();

            // Start at the "north" position of the ring
            int q = centerQ;
            int r = centerR - range;

            // Walk the 6 edges of the ring
            // Directions: E, SE, SW, W, NW, NE
            int[][] edgeDirs = new int[][]
            {
                new[] { 1, 0 },  // E
                new[] { 0, 1 },  // SE
                new[] { -1, 1 }, // SW
                new[] { -1, 0 }, // W
                new[] { 0, -1 }, // NW
                new[] { 1, -1 }, // NE
            };

            for (int edge = 0; edge < 6; edge++)
            {
                for (int step = 0; step < range; step++)
                {
                    // Check bounds
                    var tile = _grid.GetTile(q, r);
                    if (tile != null)
                    {
                        result.Add((q, r));
                    }

                    q += edgeDirs[edge][0];
                    r += edgeDirs[edge][1];
                }
            }

            return result;
        }

        /// <summary>
        /// Hex line drawing algorithm (Bresenham-style).
        /// Returns all hex coordinates on the line from (q1,r1) to (q2,r2).
        /// </summary>
        private List<(int q, int r)> HexLine(int q1, int r1, int q2, int r2)
        {
            var result = new List<(int, int)>();
            int distance = HexCoords.Distance(new HexCoords(q1, r1), new HexCoords(q2, r2));

            if (distance == 0)
            {
                result.Add((q1, r1));
                return result;
            }

            for (int i = 0; i <= distance; i++)
            {
                float t = (float)i / distance;
                // Lerp in cube coordinates
                float cq = q1 + (q2 - q1) * t;
                float cr = r1 + (r2 - r1) * t;
                float cs = -cq - cr;

                // Round to nearest hex
                int rq = Mathf.RoundToInt(cq);
                int rr = Mathf.RoundToInt(cr);
                int rs = Mathf.RoundToInt(cs);

                float dq = Mathf.Abs(rq - cq);
                float dr = Mathf.Abs(rr - cr);
                float ds = Mathf.Abs(rs - cs);

                if (dq > dr && dq > ds)
                    rq = -rr - rs;
                else if (dr > ds)
                    rr = -rq - rs;

                result.Add((rq, rr));
            }

            return result;
        }

        private int AxialToIndex(int q, int r)
        {
            int col = q + _width / 2;
            int row = r + _height / 2;
            if (col < 0 || col >= _width || row < 0 || row >= _height) return -1;
            return row * _width + col;
        }
    }
}
