using System.Collections.Generic;
using UnityEngine;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// Manages chunk-based visual rendering of the hex grid.
    /// Subdivides the grid into 16x16 hex chunks, each rendered as a single combined mesh.
    /// Chunks outside the camera frustum are disabled for performance.
    /// </summary>
    public class HexGridRenderer : MonoBehaviour
    {
        [Header("Chunk Settings")]
        [SerializeField] private int _chunkSize = 16;
        [SerializeField] private float _hexSize = 1f;
        [SerializeField] private float _hexHeight = 0.5f;

        [Header("References")]
        [SerializeField] private HexGridManager _gridManager;
        [SerializeField] private Material _defaultMaterial;

        private Dictionary<Vector2Int, HexChunkRenderer> _chunks = new();
        private Transform _cameraTransform;
        private float _cullDistance = 200f;

        private static Material _cachedMaterial;

        private void Start()
        {
            _cameraTransform = Camera.main?.transform;
            if (_defaultMaterial == null)
                _defaultMaterial = GetOrCreateMaterial();
        }

        private static Material GetOrCreateMaterial()
        {
            if (_cachedMaterial != null) return _cachedMaterial;

            // Try our custom vertex-color shader first, then fall back to URP/Unlit
            // which also supports vertex colors and is guaranteed to exist.
            Shader shader = Shader.Find("Custom/HexVertexColor");
            if (shader == null)
            {
                Debug.Log("[HexGridRenderer] Custom/HexVertexColor not found, using URP/Unlit.");
                shader = Shader.Find("Universal Render Pipeline/Unlit");
            }
            if (shader == null)
            {
                Debug.LogError("[HexGridRenderer] No usable shader found. Hex tiles will be pink.");
                return null;
            }

            _cachedMaterial = new Material(shader)
            {
                name = "HexTileVertexColor_Auto",
                color = Color.white
            };
            return _cachedMaterial;
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
            var mesh = new Mesh { name = $"HexChunk_{chunkX}_{chunkY}" };
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var colors = new List<Color>();
            var normals = new List<Vector3>();

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
                    var worldPos = tile.Coords.ToWorldPosition(_hexSize);

                    HexTile.GenerateHexMesh(_hexSize, _hexHeight, elevationOffset,
                        vertices, triangles, colors, normals, color);

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
            mesh.SetNormals(normals);
            mesh.RecalculateBounds();

            chunk.SetMesh(mesh, _defaultMaterial);

            // Position chunk at its world-space center for correct frustum culling
            float centerX = (chunkX * _chunkSize + _chunkSize * 0.5f - _gridManager.Width * 0.5f) * _hexSize * 1.5f;
            float centerZ = (chunkY * _chunkSize + _chunkSize * 0.5f - _gridManager.Height * 0.5f) * _hexSize * 1.732051f;
            chunk.transform.position = new Vector3(centerX, 0f, centerZ);
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
            Color baseColor = tile.Terrain != null
                ? tile.Terrain.TintColor
                : Color.gray;

            float elevationBrightness = tile.Elevation switch
            {
                ElevationLevel.Low    => 1.0f,
                ElevationLevel.Medium => 1.05f,
                ElevationLevel.High   => 1.1f,
                ElevationLevel.Summit => 1.2f,
                _ => 1.0f
            };

            if (tile.IsLeyLine)
                baseColor = Color.Lerp(baseColor, new Color(0.4f, 0.7f, 1.0f), 0.35f);

            if (tile.Control == ControlState.SectTerritory)
                baseColor = Color.Lerp(baseColor, new Color(0.2f, 0.8f, 0.2f), 0.2f);
            else if (tile.Control == ControlState.SettlementInfluence)
                baseColor = Color.Lerp(baseColor, new Color(0.9f, 0.8f, 0.2f), 0.15f);

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
    }
}
