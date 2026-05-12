using System.Collections.Generic;
using UnityEngine;

namespace TalesOfTao.Hex
{
    // Visual and physical representation of one hex tile in the scene.
    //
    // Mesh priority (Phase 2):
    //   1. Data.Terrain.BaseMesh — imported .obj from Wu_Dang_Hex_System (Bottom-Center pivot)
    //   2. GenerateHexMesh()     — procedural fallback when no .obj is assigned
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

        // Shader looked up once; materials shared per color for GPU instancing.
        private static Shader _cachedShader;
        private static readonly Dictionary<Color, Material> _sharedMaterials = new();

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

        private void ApplyWorldPosition()
        {
            if (Data.Coords != HexCoords.Zero)
                transform.position = Data.Coords.ToWorldPosition(_size);
        }

        private void BuildMesh()
        {
            // Prefer the imported .obj base mesh; fall back to procedural.
            var mesh = Data?.Terrain?.BaseMesh ?? GenerateHexMesh(_size, _height);
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
    }
}
