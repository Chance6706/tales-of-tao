using System.Collections.Generic;
using UnityEngine;

namespace TalesOfTao.Hex
{
    // Visual and physical representation of one hex tile in the scene.
    //
    // Mesh priority (Phase 2):
    //   1. Data.Terrain.BaseMesh — imported .obj from Wu_Dang_Hex_System (Bottom-Center pivot)
    //   2. _defaultMesh           — cached procedural fallback (shared across all tiles)
    //
    // Feature composition:
    //   If Data.Terrain.FeaturePrefab is set, it is instantiated as a child at
    //   local (0,0,0). Bottom-Center pivots sit flush on the tile Y=0 plane.
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

        // ── Static caches ──────────────────────────────────────────────────────
        // Shader looked up once; materials shared per color for GPU instancing.
        private static Shader _cachedShader;
        private static readonly Dictionary<Color, Material> _sharedMaterials = new();

        // Cached procedural mesh shared by all tiles that don't have an imported .obj.
        // Keyed by (size, height) to support multiple grid configurations.
        private static readonly Dictionary<(float, float), Mesh> _defaultMeshes = new();

        public HexTileData Data { get; private set; }

        private void Awake()
        {
            _meshFilter   = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        // Called by HexGridManager before activation (Phase 2+).
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
            // Clean up feature instance if this tile is destroyed at runtime.
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

        // Instantiates the terrain Feature prefab (e.g. Feature_Forest) as a child.
        // Bottom-Center pivot models land flush at local Y=0 with no offset needed.
        private void SpawnFeature()
        {
            if (_featureInstance != null) return;
            var prefab = Data?.Terrain?.FeaturePrefab;
            if (prefab == null) return;

            _featureInstance = Instantiate(prefab, transform);
            _featureInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        // ── Mesh Generation ───────────────────────────────────────────────────
        //
        // Procedural fallback — flat-top hex prism, 12 verts, 60 indices.
        //   verts[0..5]  top ring (y = +height/2)
        //   verts[6..11] bottom ring (y = -height/2)
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
        /// Generates a flat-top hex prism mesh into the provided vertex/triangle/color/normal lists.
        /// Used by HexGridRenderer to combine many tiles into a single mesh.
        /// Normals are per-vertex (12 entries), not per-triangle-corner.
        /// </summary>
        public static void GenerateHexMesh(float size, float height, float elevationOffset,
            List<Vector3> vertices, List<int> triangles, List<Color> colors,
            List<Vector3> normals, Color color)
        {
            int vertBase = vertices.Count;
            float h = height * 0.5f;
            float yBase = elevationOffset;

            // 12 vertices: 6 top ring (even indices), 6 bottom ring (odd indices)
            // Top face vertices get upward normals, bottom get downward, sides get averaged.
            // For simplicity and visual quality, we use face-normal approach:
            // each vertex gets the normal of the face it primarily belongs to.
            // Top ring vertices: blend of up + side normal
            // Bottom ring vertices: blend of down + side normal

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

                // Top vertex normal: average of up + two adjacent side normals
                Vector3 topNrm = (Vector3.up + sideNormals[i] + sideNormals[(i + 5) % 6]).normalized;
                normals.Add(topNrm);
                // Bottom vertex normal: average of down + two adjacent side normals
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

            // Add colors for each vertex
            for (int i = 0; i < 12; i++)
                colors.Add(color);
        }

        /// <summary>
        /// Legacy overload without normals (for individual HexTile rendering).
        /// </summary>
        public static void GenerateHexMesh(float size, float height, float elevationOffset,
            List<Vector3> vertices, List<int> triangles, List<Color> colors, Color color)
        {
            GenerateHexMesh(size, height, elevationOffset, vertices, triangles, colors,
                new List<Vector3>(), color);
        }

        // ── Material ──────────────────────────────────────────────────────────

        private void ApplyMaterial()
        {
            var color = Data?.Terrain?.TintColor ?? Color.gray;

            if (_sharedMaterials.TryGetValue(color, out var mat))
            {
                _meshRenderer.sharedMaterial = mat;
                return;
            }

            if (_cachedShader == null)
                _cachedShader = Shader.Find("Universal Render Pipeline/Lit")
                             ?? Shader.Find("Standard");

            if (_cachedShader == null) return;

            mat = new Material(_cachedShader) { color = color };
            mat.enableInstancing = true; // required for GPU instancing at grid scale
            _sharedMaterials[color] = mat;
            _meshRenderer.sharedMaterial = mat;
        }

        // ── Static cache cleanup ──────────────────────────────────────────────
        // Call when unloading a scene or switching pipelines to prevent leaks.

        /// <summary>
        /// Clears all cached materials and the shader reference.
        /// Call this when unloading a scene or switching render pipelines.
        /// </summary>
        public static void ClearCache()
        {
            foreach (var mat in _sharedMaterials.Values)
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
            _sharedMaterials.Clear();
            _cachedShader = null;
        }

        /// <summary>
        /// Clears cached procedural meshes. Call on scene unload if tiles are
        /// frequently created/destroyed at runtime.
        /// </summary>
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
