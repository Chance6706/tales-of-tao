using System.Collections.Generic;
using UnityEngine;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// Manages chunk-based visual rendering of the hex grid.
    /// Subdivides the grid into 16×16 hex chunks, each rendered as a single combined mesh.
    /// Chunks outside the camera frustum are disabled for performance.
    /// </summary>
    public class HexGridRenderer : MonoBehaviour
    {
        [Header("Chunk Settings")]
        [SerializeField] private int _chunkSize = 16;
        [SerializeField] private float _hexSize = 1f;
        [SerializeField] private float _hexHeight = 0.3f;

        [Header("References")]
        [SerializeField] private HexGridManager _gridManager;
        [SerializeField] private Material _defaultMaterial;

        private Dictionary<Vector2Int, HexChunkRenderer> _chunks = new();
        private Transform _cameraTransform;
        private float _cullDistance = 50f;

        private void Start()
        {
            _cameraTransform = Camera.main?.transform;
        }

        private void OnEnable()
        {
            if (_gridManager != null)
                _gridManager.OnMapGenerated += OnMapGenerated;
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

        /// <summary>
        /// Call after the grid manager has generated the map.
        /// </summary>
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
            ClearChunks();

            int width = _gridManager.Width;
            int height = _gridManager.Height;

            int chunksX = Mathf.CeilToInt((float)width / _chunkSize);
            int chunksY = Mathf.CeilToInt((float)height / _chunkSize);

            for (int cy = 0; cy < chunksY; cy++)
            {
                for (int cx = 0; cx < chunksX; cx++)
                {
                    var chunkCoord = new Vector2Int(cx, cy);
                    var chunk = CreateChunk(chunkCoord);
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
            var mesh = new Mesh { name = $"HexChunk_{chunkX}_{chunkY}" };
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var colors = new List<Color>();

            int startQ = chunkX * _chunkSize - _gridManager.Width / 2;
            int startR = chunkY * _chunkSize - _gridManager.Height / 2;

            for (int r = 0; r < _chunkSize; r++)
            {
                for (int q = 0; q < _chunkSize; q++)
                {
                    int tileQ = startQ + q;
                    int tileR = startR + r;
                    var tile = _gridManager.GetTile(tileQ, tileR);
                    if (tile == null) continue;

                    float elevationOffset = GetElevationOffset(tile.Elevation);
                    Color color = GetTileColor(tile);

                    // Calculate world position for this tile
                    var worldPos = tile.Coords.ToWorldPosition(_hexSize);

                    // Generate hex prism mesh at this position
                    HexTile.GenerateHexMesh(_hexSize, _hexHeight, elevationOffset,
                        vertices, triangles, colors, color);

                    // Offset vertices to world position
                    int vertBase = vertices.Count - 12;
                    for (int i = vertBase; i < vertices.Count; i++)
                    {
                        vertices[i] += worldPos;
                    }
                }
            }

            if (vertices.Count == 0) return;

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.SetColors(colors);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            chunk.SetMesh(mesh, _defaultMaterial);
            chunk.transform.position = Vector3.zero;
        }

        private float GetElevationOffset(ElevationLevel elevation) => elevation switch
        {
            ElevationLevel.Low => 0f,
            ElevationLevel.Medium => 0.5f,
            ElevationLevel.High => 1.0f,
            ElevationLevel.Summit => 1.5f,
            _ => 0f
        };

        private Color GetTileColor(HexTileData tile)
        {
            // Use terrain tint color, modified by elevation
            var baseColor = tile.Terrain?.TintColor ?? Color.gray;
            float elevationBrightness = tile.Elevation switch
            {
                ElevationLevel.Low => 1.0f,
                ElevationLevel.Medium => 1.05f,
                ElevationLevel.High => 1.1f,
                ElevationLevel.Summit => 1.2f,
                _ => 1.0f
            };

            // Highlight ley lines
            if (tile.IsLeyLine)
                baseColor = Color.Lerp(baseColor, new Color(0.5f, 0.8f, 1f), 0.3f);

            // Highlight controlled tiles
            if (tile.Control == ControlState.SectTerritory)
                baseColor = Color.Lerp(baseColor, Color.green, 0.2f);
            else if (tile.Control == ControlState.SettlementInfluence)
                baseColor = Color.Lerp(baseColor, Color.yellow, 0.15f);

            return new Color(
                Mathf.Clamp01(baseColor.r * elevationBrightness),
                Mathf.Clamp01(baseColor.g * elevationBrightness),
                Mathf.Clamp01(baseColor.b * elevationBrightness),
                baseColor.a);
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
    }

    /// <summary>
    /// Component on each chunk GameObject holding the combined mesh.
    /// </summary>
    public class HexChunkRenderer : MonoBehaviour
    {
        public Vector2Int ChunkCoord { get; set; }

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        public void SetMesh(Mesh mesh, Material material)
        {
            if (_meshFilter == null)
                _meshFilter = gameObject.AddComponent<MeshFilter>();
            if (_meshRenderer == null)
                _meshRenderer = gameObject.AddComponent<MeshRenderer>();

            _meshFilter.sharedMesh = mesh;
            _meshRenderer.sharedMaterial = material;
        }
    }
}
