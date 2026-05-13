using System;
using System.Collections.Generic;
using UnityEngine;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// Generates and manages the hex grid map.
    /// Implements the 7-pass pipeline from GDD §4.3:
    ///   1. Elevation (Perlin noise)
    ///   2. Biome (Voronoi regions)
    ///   3. Sacred Peak rarity enforcement
    ///   4. Ley Line paths (A* between Sacred Peaks)
    ///   5. Starting locations (max-spread placement)
    ///   6. Settlement seeding
    ///   7. Resource deposits
    /// </summary>
    public class HexGridManager : MonoBehaviour
    {
        [Header("Map Size")]
        [SerializeField] private MapSize _mapSize = MapSize.Medium;

        [Header("Generation Seed")]
        [SerializeField] private int _seed = 0;
        [SerializeField] private bool _randomizeSeed = true;

        [Header("References")]
        [SerializeField] private TerrainTypeSO[] _terrainTypes; // 8 types in enum order

        // Generated data
        private HexTileData[] _tiles;
        private int _width;
        private int _height;
        private bool _isGenerated;

        public int Width => _width;
        public int Height => _height;
        public bool IsGenerated => _isGenerated;

        /// <summary>Total number of tiles in the generated map.</summary>
        public int TileCount => _tiles?.Length ?? 0;

        public event Action OnMapGenerated;

        private void Awake()
        {
            if (_randomizeSeed) _seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            AutoLoadTerrainTypes();
        }

        /// <summary>
        /// Auto-loads terrain type assets from Assets/_Game/Data/Terrain/ if not assigned.
        /// </summary>
        private void AutoLoadTerrainTypes()
        {
            if (_terrainTypes != null && _terrainTypes.Length > 0) return;

#if UNITY_EDITOR
            var guids = UnityEditor.AssetDatabase.FindAssets("t:TerrainTypeSO", new[] { "Assets/_Game/Data/Terrain" });
            var list = new System.Collections.Generic.List<TerrainTypeSO>();
            foreach (var guid in guids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var so = UnityEditor.AssetDatabase.LoadAssetAtPath<TerrainTypeSO>(path);
                if (so != null) list.Add(so);
            }
            if (list.Count > 0)
            {
                _terrainTypes = list.ToArray();
                Debug.Log($"[HexGrid] Auto-loaded {_terrainTypes.Length} terrain types.");
            }
            else
            {
                Debug.LogWarning("[HexGrid] No TerrainTypeSO assets found in Assets/_Game/Data/Terrain/. " +
                                 "Run 'TalesOfTao > 1 - Create Data Assets' first.");
            }
#endif
        }

        /// <summary>
        /// Generates the full map. Call this at game start or from an editor tool.
        /// </summary>
        public void GenerateMap(int? forcedSeed = null)
        {
            if (forcedSeed.HasValue) _seed = forcedSeed.Value;
            else if (_randomizeSeed) _seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

            var rng = new System.Random(_seed);

            (_width, _height) = GetDimensions(_mapSize);
            int totalTiles = _width * _height;

            Debug.Log($"[HexGrid] Generating {_width}x{_height} map ({totalTiles} tiles, seed={_seed})");

            _tiles = new HexTileData[totalTiles];

            // Initialize all tiles with default coords
            for (int i = 0; i < totalTiles; i++)
            {
                var coords = IndexToAxial(i);
                _tiles[i] = new HexTileData { Coords = coords };
            }

            Pass_Elevation(rng);
            Pass_Biome(rng);
            Pass_SacredPeakRarity(rng);
            Pass_LeyLines(rng);
            Pass_StartingLocations(rng);
            Pass_Settlements(rng);
            Pass_ResourceDeposits(rng);

            _isGenerated = true;
            OnMapGenerated?.Invoke();

            Debug.Log($"[HexGrid] Generation complete. {_tiles.Length} tiles.");
        }

        /// <summary>
        /// Gets the tile data at axial coordinates (q, r). Returns null if out of bounds.
        /// </summary>
        public HexTileData GetTile(int q, int r)
        {
            int index = AxialToIndex(q, r);
            if (index < 0 || index >= _tiles.Length) return null;
            return _tiles[index];
        }

        /// <summary>
        /// Gets the tile data by array index.
        /// </summary>
        public HexTileData GetTile(int index)
        {
            if (index < 0 || index >= _tiles.Length) return null;
            return _tiles[index];
        }

        /// <summary>
        /// Returns all generated tiles (read-only).
        /// </summary>
        public IReadOnlyList<HexTileData> GetAllTiles()
        {
            return _tiles;
        }

        // ── Coordinate Helpers ──────────────────────────────────────────────

        private int AxialToIndex(int q, int r)
        {
            // Offset coordinates: shift q by half the width to handle negatives
            int col = q + _width / 2;
            int row = r + _height / 2;
            if (col < 0 || col >= _width || row < 0 || row >= _height) return -1;
            return row * _width + col;
        }

        private HexCoords IndexToAxial(int index)
        {
            int row = index / _width;
            int col = index % _width;
            int q = col - _width / 2;
            int r = row - _height / 2;
            return new HexCoords(q, r);
        }

        // ── Pass 1: Elevation ───────────────────────────────────────────────

        private void Pass_Elevation(System.Random rng)
        {
            float scale = 0.08f;
            float offsetX = rng.Next(-10000, 10000);
            float offsetY = rng.Next(-10000, 10000);

            for (int i = 0; i < _tiles.Length; i++)
            {
                var coords = _tiles[i].Coords;
                float noise = PerlinNoise(
                    coords.Q * scale + offsetX,
                    coords.R * scale + offsetY,
                    4, 0.5f, 2.0f, rng);

                _tiles[i].Elevation = noise switch
                {
                    < 0.2f => ElevationLevel.Low,
                    < 0.5f => ElevationLevel.Medium,
                    < 0.8f => ElevationLevel.High,
                    _ => ElevationLevel.Summit
                };
            }
        }

        // ── Pass 2: Biome ───────────────────────────────────────────────────

        private void Pass_Biome(System.Random rng)
        {
            int numSeeds = Mathf.Max(8, _width / 15);
            var seedPoints = new List<(HexCoords coords, TerrainType type)>();

            for (int i = 0; i < numSeeds; i++)
            {
                int q = rng.Next(-_width / 2, _width / 2);
                int r = rng.Next(-_height / 2, _height / 2);
                var coords = new HexCoords(q, r);
                var tile = GetTile(q, r);
                if (tile == null) continue;

                TerrainType type = tile.Elevation switch
                {
                    ElevationLevel.Low => Pick3(rng, TerrainType.Plains, TerrainType.River, TerrainType.Swamp),
                    ElevationLevel.Medium => Pick(rng, TerrainType.Forest, TerrainType.Plains),
                    ElevationLevel.High => Pick(rng, TerrainType.Mountain, TerrainType.Forest),
                    ElevationLevel.Summit => Pick(rng, TerrainType.Mountain, TerrainType.SacredPeak),
                    _ => TerrainType.Plains
                };

                seedPoints.Add((coords, type));
            }

            // Assign each tile to nearest seed's biome
            for (int i = 0; i < _tiles.Length; i++)
            {
                var tileCoords = _tiles[i].Coords;
                var nearest = seedPoints[0];
                int minDist = int.MaxValue;

                foreach (var seed in seedPoints)
                {
                    int dist = HexCoords.Distance(tileCoords, seed.coords);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearest = seed;
                    }
                }

                _tiles[i].Terrain = GetTerrainTypeSO(nearest.type);
            }
        }

        // ── Pass 3: Sacred Peak Rarity ──────────────────────────────────────

        private void Pass_SacredPeakRarity(System.Random rng)
        {
            var sacredPeaks = new List<int>();
            for (int i = 0; i < _tiles.Length; i++)
            {
                if (_tiles[i].Terrain?.Type == TerrainType.SacredPeak)
                    sacredPeaks.Add(i);
            }

            int maxAllowed = Mathf.Max(2, (int)(_tiles.Length * 0.02f));

            // Downgrade excess Sacred Peaks to Mountain
            while (sacredPeaks.Count > maxAllowed)
            {
                int idx = rng.Next(sacredPeaks.Count);
                int tileIdx = sacredPeaks[idx];
                _tiles[tileIdx].Terrain = GetTerrainTypeSO(TerrainType.Mountain);
                sacredPeaks.RemoveAt(idx);
            }

            // Guarantee at least 2 Sacred Peaks
            if (sacredPeaks.Count < 2)
            {
                // Find Mountain tiles at High/Summit elevation and convert
                var candidates = new List<int>();
                for (int i = 0; i < _tiles.Length; i++)
                {
                    if (_tiles[i].Terrain?.Type == TerrainType.Mountain &&
                        (_tiles[i].Elevation == ElevationLevel.High || _tiles[i].Elevation == ElevationLevel.Summit))
                    {
                        candidates.Add(i);
                    }
                }

                while (sacredPeaks.Count < 2 && candidates.Count > 0)
                {
                    int idx = rng.Next(candidates.Count);
                    int tileIdx = candidates[idx];
                    _tiles[tileIdx].Terrain = GetTerrainTypeSO(TerrainType.SacredPeak);
                    sacredPeaks.Add(tileIdx);
                    candidates.RemoveAt(idx);
                }
            }
        }

        // ── Pass 4: Ley Lines ───────────────────────────────────────────────

        private void Pass_LeyLines(System.Random rng)
        {
            var sacredPeaks = new List<HexCoords>();
            for (int i = 0; i < _tiles.Length; i++)
            {
                if (_tiles[i].Terrain?.Type == TerrainType.SacredPeak)
                    sacredPeaks.Add(_tiles[i].Coords);
            }

            if (sacredPeaks.Count < 2) return;

            int numLines = Mathf.Min(sacredPeaks.Count - 1, 3);

            for (int i = 0; i < numLines; i++)
            {
                var start = sacredPeaks[i];
                var end = sacredPeaks[i + 1];

                // Simple path: walk toward end with some randomness
                var path = new List<HexCoords> { start };
                var current = start;

                while (current != end)
                {
                    // Find neighbor closest to end with some noise
                    HexCoords best = current;
                    int bestDist = int.MaxValue;

                    for (int dir = 0; dir < 6; dir++)
                    {
                        var neighbor = current.Neighbour(dir);
                        int dist = HexCoords.Distance(neighbor, end);
                        // Add random noise to create winding paths
                        dist += rng.Next(-2, 3);
                        if (dist < bestDist)
                        {
                            bestDist = dist;
                            best = neighbor;
                        }
                    }

                    if (best == current) break; // Stuck
                    current = best;
                    path.Add(current);
                }

                // Mark tiles within 1 hex of the path as LeyLine
                foreach (var pathTile in path)
                {
                    var tile = GetTile(pathTile.Q, pathTile.R);
                    if (tile != null)
                    {
                        tile.QiDensity = QiDensityLevel.LeyLine;
                    }

                    // Also mark neighbors within 1 hex
                    for (int dir = 0; dir < 6; dir++)
                    {
                        var neighbor = pathTile.Neighbour(dir);
                        var nTile = GetTile(neighbor.Q, neighbor.R);
                        if (nTile != null)
                        {
                            nTile.QiDensity = QiDensityLevel.LeyLine;
                        }
                    }
                }
            }
        }

        // ── Pass 5: Starting Locations ──────────────────────────────────────

        private void Pass_StartingLocations(System.Random rng)
        {
            // For now, place 1 player start. AI starts added later.
            var validStarts = new List<int>();
            for (int i = 0; i < _tiles.Length; i++)
            {
                var t = _tiles[i];
                bool validTerrain = t.Terrain?.Type == TerrainType.Plains || t.Terrain?.Type == TerrainType.Forest;
                bool validElevation = t.Elevation == ElevationLevel.Low || t.Elevation == ElevationLevel.Medium;

                if (validTerrain && validElevation)
                    validStarts.Add(i);
            }

            if (validStarts.Count > 0)
            {
                int startIdx = validStarts[rng.Next(validStarts.Count)];
                _tiles[startIdx].Control = ControlState.SectTerritory;
                Debug.Log($"[HexGrid] Player start placed at {_tiles[startIdx].Coords}");
            }
        }

        // ── Pass 6: Settlement Seeding ──────────────────────────────────────

        private void Pass_Settlements(System.Random rng)
        {
            int targetCount = Mathf.Max(1, _tiles.Length / 150);
            int placed = 0;

            // Find fertile lowland tiles
            var candidates = new List<int>();
            for (int i = 0; i < _tiles.Length; i++)
            {
                var t = _tiles[i];
                bool fertile = (t.Terrain?.Type == TerrainType.Plains || t.Terrain?.Type == TerrainType.River) &&
                               (t.Elevation == ElevationLevel.Low || t.Elevation == ElevationLevel.Medium) &&
                               t.Control == ControlState.Unowned;
                if (fertile) candidates.Add(i);
            }

            // Shuffle and pick
            for (int i = candidates.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
            }

            for (int i = 0; i < Mathf.Min(targetCount, candidates.Count); i++)
            {
                _tiles[candidates[i]].Control = ControlState.SettlementInfluence;
                placed++;
            }

            Debug.Log($"[HexGrid] Placed {placed} settlements.");
        }

        // ── Pass 7: Resource Deposits ───────────────────────────────────────

        private void Pass_ResourceDeposits(System.Random rng)
        {
            for (int i = 0; i < _tiles.Length; i++)
            {
                var t = _tiles[i];
                var deposits = new List<DepositType>();

                switch (t.Terrain?.Type)
                {
                    case TerrainType.Mountain:
                        if (rng.NextDouble() < 0.40) deposits.Add(DepositType.IronOre);
                        if (rng.NextDouble() < 0.20) deposits.Add(DepositType.Jade);
                        break;
                    case TerrainType.Forest:
                        if (rng.NextDouble() < 0.35) deposits.Add(DepositType.Lumber);
                        if (rng.NextDouble() < 0.25) deposits.Add(DepositType.MedicinalHerbs);
                        break;
                    case TerrainType.Swamp:
                        if (rng.NextDouble() < 0.60) deposits.Add(DepositType.MedicinalHerbs);
                        break;
                    case TerrainType.SacredPeak:
                        if (t.IsLeyLine && rng.NextDouble() < 0.30) deposits.Add(DepositType.SpiritHerbs);
                        break;
                }

                t.Deposits = deposits.ToArray();
            }
        }

        // ── Utility ─────────────────────────────────────────────────────────

        private static (int width, int height) GetDimensions(MapSize size) => size switch
        {
            MapSize.Small => (60, 40),
            MapSize.Medium => (80, 60),
            MapSize.Large => (120, 80),
            MapSize.Epic => (160, 100),
            _ => (80, 60)
        };

        private static TerrainType Pick(System.Random rng, TerrainType a, TerrainType b) =>
            rng.NextDouble() < 0.5 ? a : b;

        private static TerrainType Pick3(System.Random rng, TerrainType a, TerrainType b, TerrainType c) =>
            rng.Next(3) switch { 0 => a, 1 => b, _ => c };

        private TerrainTypeSO GetTerrainTypeSO(TerrainType type)
        {
            if (_terrainTypes == null) return null;
            foreach (var t in _terrainTypes)
            {
                if (t != null && t.Type == type) return t;
            }
            return null;
        }

        /// <summary>
        /// Simple 2D Perlin-like noise using multiple octaves.
        /// </summary>
        private static float PerlinNoise(float x, float y, int octaves, float persistence, float lacunarity, System.Random rng)
        {
            float total = 0;
            float frequency = 1;
            float amplitude = 1;
            float maxValue = 0;

            // Use a simple hash-based noise for deterministic results with seed
            for (int i = 0; i < octaves; i++)
            {
                total += HashNoise(x * frequency, y * frequency, rng) * amplitude;
                maxValue += amplitude;
                amplitude *= persistence;
                frequency *= lacunarity;
            }

            return (total / maxValue + 1f) * 0.5f; // Normalize to [0, 1]
        }

        private static float HashNoise(float x, float y, System.Random rng)
        {
            // Simple value noise
            int ix = Mathf.FloorToInt(x);
            int iy = Mathf.FloorToInt(y);
            float fx = x - ix;
            float fy = y - iy;

            float a = PseudoRandom(ix, iy, rng);
            float b = PseudoRandom(ix + 1, iy, rng);
            float c = PseudoRandom(ix, iy + 1, rng);
            float d = PseudoRandom(ix + 1, iy + 1, rng);

            float ux = fx * fx * (3 - 2 * fx);
            float uy = fy * fy * (3 - 2 * fy);

            return Mathf.Lerp(Mathf.Lerp(a, b, ux), Mathf.Lerp(c, d, ux), uy) * 2f - 1f;
        }

        private static float PseudoRandom(int x, int y, System.Random rng)
        {
            // Deterministic hash
            int h = x * 374761393 + y * 668265263;
            h = (h ^ (h >> 13)) * 1274126177;
            h = h ^ (h >> 16);
            return (float)(h & 0x7fffffff) / int.MaxValue;
        }
    }
}
