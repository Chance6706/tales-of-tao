using System.Collections.Generic;
using UnityEngine;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// Manages chunk-based visual rendering of the hex grid.
    /// Subdivides the grid into 16x16 hex chunks. Each chunk contains one sub-mesh
    /// per terrain type, each with its own URP Lit material colored by terrain.
    /// No custom shaders or vertex colors required.
    /// </summary>
    public class HexGridRenderer : MonoBehaviour
    {
        [Header("Chunk Settings")]
        [SerializeField] private int _chunkSize = 16;
        [SerializeField] private float _hexSize = 1f;
        [SerializeField] private float _hexHeight = 0.5f;

        [Header("References")]
        [SerializeField] private HexGridManager _gridManager;

        private Dictionary<Vector2Int, HexChunkRenderer> _chunks = new();
        private Transform _cameraTransform;
        private float _cullDistance = 200f;

        // Cache: terrain type index -> material with correct color
        private static readonly Dictionary<int, Material> _terrainMaterials = new();
        private static Material _fallbackMaterial;
        private static bool _materialsInitialized;

        private void Start()
        {
            _cameraTransform = Camera.main?.transform;
            InitializeMaterials();
        }

        private static void InitializeMaterials()
        {
            if (_materialsInitialized) return;
            _materialsInitialized = true;

            // Use our custom unlit terrain shader — outputs material color directly
            // with no lighting darkening. Works in any URP project.
            Shader shader = Shader.Find("TalesOfTao/HexColorPerTile");
            if (shader == null)
            {
                shader = Shader.Find("TalesOfTao_HexColorPerTile");
            }
            if (shader == null)
            {
                // Fallback: URP/Unlit (also no lighting)
                shader = Shader.Find("Universal Render Pipeline/Unlit");
            }
            if (shader == null)
            {
                // Last resort: Standard shader
                shader = Shader.Find("Standard");
            }
            if (shader == null)
            {
                Debug.LogError("[HexGridRenderer] No shader found at all. Tiles will be pink.");
                return;
            }

            // Fallback material (gray)
            _fallbackMaterial = CreateMaterial(shader, Color.gray, "HexFallback");

            // Pre-create materials for each terrain type using default tint colors
            // Colors match TerrainTypeSO defaults
            var defaultColors = new Dictionary<TerrainType, Color>
            {
                { TerrainType.Plains,     new Color(0.50f, 0.80f, 0.30f) },
                { TerrainType.Mountain,   new Color(0.55f, 0.45f, 0.35f) },
                { TerrainType.Forest,     new Color(0.12f, 0.55f, 0.12f) },
                { TerrainType.River,      new Color(0.25f, 0.60f, 0.90f) },
                { TerrainType.Lake,       new Color(0.15f, 0.40f, 0.80f) },
                { TerrainType.Desert,     new Color(0.95f, 0.85f, 0.50f) },
                { TerrainType.Swamp,      new Color(0.30f, 0.45f, 0.15f) },
                { TerrainType.SacredPeak, new Color(0.75f, 0.65f, 0.95f) },
            };

            foreach (var kvp in defaultColors)
            {
                int key = (int)kvp.Key;
                _terrainMaterials[key] = CreateMaterial(shader, kvp.Value, $"HexTerrain_{kvp.Key}");
            }
        }

        private static Material CreateMaterial(Shader shader, Color color, string name)
        {
            var mat = new Material(shader)
            {
                name = name,
                color = color
            };
            return mat;
        }

        private static Material GetTerrainMaterial(Color color, TerrainType type)
        {
            // Try cached material by terrain type
            int key = (int)type;
            if (_terrainMaterials.TryGetValue(key, out var cached))
                return cached;

            // Create on-demand
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (shader == null) return _fallbackMaterial;

            var mat = CreateMaterial(shader, color, $"HexTerrain_{type}_Runtime");
            _terrainMaterials[key] = mat;
            return mat;
        }

        private void OnEnable()
        {
            if (_gridManager != null)
                _gridManager.OnMapGenerated += OnMapGenerated;
        }

        public void SetGridManager(HexGridManager manager)
        {
            if (_gridManager != null)
                _gridManager.OnMapGenerated -= OnMapGenerated;

            _gridManager = manager;
            if (_gridManager != null)
            {
                _gridManager.OnMapGenerated += OnMapGenerated;
                if (_gridManager.IsGenerated)
                    BuildAllChunks();
            }
        }

        private void OnDisable()
        {
            if (_gridManager != null)
                _gridManager.OnMapGenerated -= OnMapGenerated;
        }

        private void LateUpdate()
        {
            UpdateChunkVisibility();
        }

        public void Initialize()
        {
            if (_gridManager == null || !_gridManager.IsGenerated)
            {
                Debug.LogWarning("[HexGridRenderer] Grid manager not ready. Call after map generation.");
                return;
            }

            BuildAllChunks();
        }

        private void OnMapGenerated()
        {
            BuildAllChunks();
        }

        private void BuildAllChunks()
        {
            InitializeMaterials();
            ClearChunks();

            int width = _gridManager.Width;
            int height = _gridManager.Height;

            int chunksX = Mathf.CeilToInt((float)width / _chunkSize);
            int chunksY = Mathf.CeilToInt((float)height / _chunkSize);

            int hexTileLayer = LayerMask.NameToLayer("HexTile");
            if (hexTileLayer < 0) hexTileLayer = 0;

            for (int cy = 0; cy < chunksY; cy++)
            {
                for (int cx = 0; cx < chunksX; cx++)
                {
                    var chunkCoord = new Vector2Int(cx, cy);
                    var chunk = CreateChunk(chunkCoord);
                    chunk.gameObject.layer = hexTileLayer;
                    BuildChunkMesh(chunk, cx, cy);
                    _chunks[chunkCoord] = chunk;
                }
            }

            Debug.Log($"[HexGridRenderer] Built {_chunks.Count} chunks ({chunksX}x{chunksY})");
        }

        private HexChunkRenderer CreateChunk(Vector2Int coord)
        {
            var go = new GameObject($"Chunk_{coord.x}_{coord.y}");
            go.transform.SetParent(transform, false);
            var chunk = go.AddComponent<HexChunkRenderer>();
            chunk.ChunkCoord = coord;
            return chunk;
        }

        private void BuildChunkMesh(HexChunkRenderer chunk, int chunkX, int chunkY)
        {
            int startQ = chunkX * _chunkSize - _gridManager.Width / 2;
            int startR = chunkY * _chunkSize - _gridManager.Height / 2;

            // Group tiles by terrain type SO — each unique terrain gets its own sub-mesh
            // Key: terrain type index -> mesh data + material color
            var meshGroups = new Dictionary<int, MeshBuildData>();

            for (int r = 0; r < _chunkSize; r++)
            {
                for (int q = 0; q < _chunkSize; q++)
                {
                    int tileQ = startQ + q;
                    int tileR = startR + r;
                    var tile = _gridManager.GetTile(tileQ, tileR);
                    if (tile == null) continue;

                    // Use terrain type as grouping key
                    int terrainKey = (int)(tile.Terrain?.Type ?? TerrainType.Plains);

                    if (!meshGroups.TryGetValue(terrainKey, out var data))
                    {
                        Color color = GetTileColor(tile);
                        data = new MeshBuildData { Color = color };
                        meshGroups[terrainKey] = data;
                    }

                    float elevationOffset = GetElevationOffset(tile.Elevation);
                    var worldPos = tile.Coords.ToWorldPosition(_hexSize);

                    // Generate hex prism mesh into this group's buffers
                    HexTile.GenerateHexMesh(_hexSize, _hexHeight, elevationOffset,
                        data.Vertices, data.Triangles, data.Normals);

                    // Offset vertices to world position (last 12 verts added)
                    int vertBase = data.Vertices.Count - 12;
                    for (int i = vertBase; i < data.Vertices.Count; i++)
                    {
                        data.Vertices[i] += worldPos;
                    }
                }
            }

            if (meshGroups.Count == 0) return;

            // Build the final mesh with sub-meshes, one per terrain type
            var mesh = new Mesh { name = $"HexChunk_{chunkX}_{chunkY}" };
            mesh.subMeshCount = meshGroups.Count;

            var allVertices = new List<Vector3>();
            var allNormals = new List<Vector3>();
            var materials = new Material[meshGroups.Count];
            var subMeshTriangles = new List<int>[meshGroups.Count];

            int matIndex = 0;
            int vertexOffset = 0;
            foreach (var kvp in meshGroups)
            {
                var data = kvp.Value;

                allVertices.AddRange(data.Vertices);
                allNormals.AddRange(data.Normals);

                // Collect triangles for this sub-mesh with vertex offset
                var tris = new List<int>();
                foreach (var tri in data.Triangles)
                {
                    tris.Add(tri + vertexOffset);
                }
                subMeshTriangles[matIndex] = tris;
                vertexOffset += data.Vertices.Count;

                TerrainType tt = (TerrainType)kvp.Key;
                materials[matIndex++] = GetTerrainMaterial(data.Color, tt);
            }

            mesh.SetVertices(allVertices);
            mesh.SetNormals(allNormals);

            // Set each sub-mesh's triangles
            for (int i = 0; i < meshGroups.Count; i++)
            {
                mesh.SetTriangles(subMeshTriangles[i], i);
            }

            mesh.RecalculateBounds();

            chunk.SetMesh(mesh, materials);
            // Mesh vertices are already in world space — do NOT offset the chunk transform.
        }

        private float GetElevationOffset(ElevationLevel elevation) => elevation switch
        {
            ElevationLevel.Low    => 0f,
            ElevationLevel.Medium => 0.5f,
            ElevationLevel.High   => 1.0f,
            ElevationLevel.Summit => 1.5f,
            _ => 0f
        };

        private Color GetTileColor(HexTileData tile)
        {
            return tile.Terrain != null
                ? tile.Terrain.TintColor
                : Color.gray;
        }

        private void UpdateChunkVisibility()
        {
            if (_cameraTransform == null) return;

            Vector3 camPos = _cameraTransform.position;

            foreach (var kvp in _chunks)
            {
                var chunk = kvp.Value;
                if (chunk == null) continue;

                float dist = Vector3.Distance(camPos, chunk.transform.position);
                bool visible = dist < _cullDistance;
                if (chunk.gameObject.activeSelf != visible)
                    chunk.gameObject.SetActive(visible);
            }
        }

        private void ClearChunks()
        {
            foreach (var chunk in _chunks.Values)
            {
                if (chunk != null)
                {
                    if (Application.isPlaying)
                        Destroy(chunk.gameObject);
                    else
                        DestroyImmediate(chunk.gameObject);
                }
            }
            _chunks.Clear();
        }

        /// <summary>
        /// Helper class for accumulating mesh data per color group.
        /// </summary>
        private class MeshBuildData
        {
            public Color Color;
            public List<Vector3> Vertices = new();
            public List<int> Triangles = new();
            public List<Vector3> Normals = new();
        }
    }
}
