using System.Collections.Generic;
using UnityEngine;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// Visual and physical representation of one hex tile in the scene.
    /// Uses standard URP Lit material colored per-terrain-type.
    /// No custom shaders or vertex colors needed.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class HexTile : MonoBehaviour
    {
        [SerializeField] private float         _size            = 1f;
        [SerializeField] private float         _height          = 0.3f;
        [SerializeField] private TerrainTypeSO _terrainOverride;

        private MeshFilter   _meshFilter;
        private MeshCollider _meshCollider;
        private MeshRenderer _meshRenderer;
        private GameObject   _featureInstance;

        // Cached procedural mesh shared by all tiles that don't have an imported .obj.
        private static readonly Dictionary<(float, float), Mesh> _defaultMeshes = new();

        public HexTileData Data { get; private set; }

        private void Awake()
        {
            _meshFilter   = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        public void Initialize(HexTileData data)
        {
            Data = data;
        }

        private void Start()
        {
            if (Data == null)
            {
                Data = new HexTileData
                {
                    Coords  = HexCoords.Zero,
                    Terrain = _terrainOverride,
                };
            }

            ApplyWorldPosition();
            BuildMesh();
            SpawnFeature();
            ApplyMaterial();
        }

        private void OnDestroy()
        {
            if (_featureInstance != null)
                Destroy(_featureInstance);
        }

        private void ApplyWorldPosition()
        {
            if (Data.Coords != HexCoords.Zero)
                transform.position = Data.Coords.ToWorldPosition(_size);
        }

        private void BuildMesh()
        {
            Mesh mesh = null;

            // Priority 1: imported .obj mesh from terrain data
            if (Data?.Terrain?.BaseMesh != null)
            {
                mesh = Data.Terrain.BaseMesh;
            }
            // Priority 2: cached procedural fallback
            else
            {
                var key = (_size, _height);
                if (!_defaultMeshes.TryGetValue(key, out mesh))
                {
                    mesh = GenerateHexMesh(_size, _height);
                    _defaultMeshes[key] = mesh;
                }
            }

            _meshFilter.sharedMesh   = mesh;
            _meshCollider.sharedMesh = mesh;
        }

        private void SpawnFeature()
        {
            if (_featureInstance != null) return;
            var prefab = Data?.Terrain?.FeaturePrefab;
            if (prefab == null) return;

            _featureInstance = Instantiate(prefab, transform);
            _featureInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        // ── Material ──────────────────────────────────────────────────────────

        private void ApplyMaterial()
        {
            var color = Data?.Terrain?.TintColor ?? Color.gray;
            var mat = GetOrCreateMaterial(color);
            _meshRenderer.sharedMaterial = mat;
        }

        private static Material GetOrCreateMaterial(Color color)
        {
            int key = ColorToKey(color);
            if (_materialCache.TryGetValue(key, out var cached))
                return cached;

            Shader shader = Shader.Find("TalesOfTao/HexColorPerTile")
                         ?? Shader.Find("Universal Render Pipeline/Unlit")
                         ?? Shader.Find("Standard");
            if (shader == null)
            {
                Debug.LogError("[HexTile] No shader found. Tiles will be pink.");
                return null;
            }

            var mat = new Material(shader)
            {
                name = $"HexTile_{color.r:F2}_{color.g:F2}_{color.b:F2}",
                color = color
            };
            _materialCache[key] = mat;
            return mat;
        }

        private static int ColorToKey(Color c)
        {
            int r = Mathf.RoundToInt(c.r * 31f);
            int g = Mathf.RoundToInt(c.g * 31f);
            int b = Mathf.RoundToInt(c.b * 31f);
            return (r << 24) | (g << 16) | (b << 8);
        }

        private static readonly Dictionary<int, Material> _materialCache = new();

        // ── Mesh Generation ───────────────────────────────────────────────────

        /// <summary>
        /// Procedural fallback — flat-top hex prism, 12 verts, 60 indices.
        /// </summary>
        public static Mesh GenerateHexMesh(float size, float height)
        {
            var mesh  = new Mesh { name = "HexTileMesh" };
            var verts = new Vector3[12];
            float h   = height * 0.5f;

            for (int i = 0; i < 6; i++)
            {
                float angle = Mathf.Deg2Rad * 60f * i;
                float x = size * Mathf.Cos(angle);
                float z = size * Mathf.Sin(angle);
                verts[i]     = new Vector3(x,  h, z);
                verts[i + 6] = new Vector3(x, -h, z);
            }

            var tris = new int[60];
            int ti   = 0;

            for (int i = 1; i < 5; i++) // top face
                { tris[ti++] = 0; tris[ti++] = i; tris[ti++] = i + 1; }

            for (int i = 1; i < 5; i++) // bottom face (reversed)
                { tris[ti++] = 6; tris[ti++] = 6 + i + 1; tris[ti++] = 6 + i; }

            for (int i = 0; i < 6; i++) // side quads
            {
                int n = (i + 1) % 6, b = i + 6, bn = n + 6;
                tris[ti++] = i;  tris[ti++] = n;  tris[ti++] = bn;
                tris[ti++] = i;  tris[ti++] = bn; tris[ti++] = b;
            }

            mesh.vertices  = verts;
            mesh.triangles = tris;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        /// <summary>
        /// Generates a flat-top hex prism into the provided lists.
        /// No vertex colors — color is applied via material.
        /// Used by HexGridRenderer to combine many tiles into sub-meshes.
        /// </summary>
        public static void GenerateHexMesh(float size, float height, float elevationOffset,
            List<Vector3> vertices, List<int> triangles, List<Vector3> normals)
        {
            int vertBase = vertices.Count;
            float h = height * 0.5f;
            float yBase = elevationOffset;

            Vector3[] sideNormals = new Vector3[6];
            for (int i = 0; i < 6; i++)
            {
                float angle = Mathf.Deg2Rad * 60f * (i + 0.5f);
                sideNormals[i] = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            }

            for (int i = 0; i < 6; i++)
            {
                float angle = Mathf.Deg2Rad * 60f * i;
                float x = size * Mathf.Cos(angle);
                float z = size * Mathf.Sin(angle);
                vertices.Add(new Vector3(x, yBase + h, z));
                vertices.Add(new Vector3(x, yBase - h, z));

                Vector3 topNrm = (Vector3.up + sideNormals[i] + sideNormals[(i + 5) % 6]).normalized;
                normals.Add(topNrm);
                Vector3 botNrm = (Vector3.down + sideNormals[i] + sideNormals[(i + 5) % 6]).normalized;
                normals.Add(botNrm);
            }

            // Top face (fan from vert 0)
            for (int i = 1; i < 5; i++)
            {
                triangles.Add(vertBase + 0);
                triangles.Add(vertBase + i * 2);
                triangles.Add(vertBase + (i + 1) * 2);
            }

            // Bottom face (reversed winding)
            for (int i = 1; i < 5; i++)
            {
                triangles.Add(vertBase + 1);
                triangles.Add(vertBase + (i + 1) * 2 + 1);
                triangles.Add(vertBase + i * 2 + 1);
            }

            // Side quads (2 triangles each)
            for (int i = 0; i < 6; i++)
            {
                int n = (i + 1) % 6;
                int topI = vertBase + i * 2;
                int topN = vertBase + n * 2;
                int botI = vertBase + i * 2 + 1;
                int botN = vertBase + n * 2 + 1;

                triangles.Add(topI);
                triangles.Add(topN);
                triangles.Add(botN);

                triangles.Add(topI);
                triangles.Add(botN);
                triangles.Add(botI);
            }
        }

        // ── Cache cleanup ─────────────────────────────────────────────────────

        public static void ClearCache()
        {
            foreach (var mat in _materialCache.Values)
            {
                if (mat != null)
                {
#if UNITY_EDITOR
                    DestroyImmediate(mat);
#else
                    Destroy(mat);
#endif
                }
            }
            _materialCache.Clear();
        }

        public static void ClearMeshCache()
        {
            foreach (var mesh in _defaultMeshes.Values)
            {
                if (mesh != null)
                {
#if UNITY_EDITOR
                    DestroyImmediate(mesh);
#else
                    Destroy(mesh);
#endif
                }
            }
            _defaultMeshes.Clear();
        }
    }
}
