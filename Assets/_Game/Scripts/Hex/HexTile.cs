using UnityEngine;

namespace TalesOfTao.Hex
{
    // Visual and physical representation of one hex tile in the scene.
    // Generates a flat-top hexagonal prism mesh at runtime from HexTileData.
    //
    // Phase 1 scene setup:
    //   1. Create a GameObject, add this component (MeshFilter, MeshRenderer,
    //      MeshCollider are auto-added via RequireComponent).
    //   2. Assign a TerrainTypeSO to _terrainOverride in the Inspector.
    //   3. Set the GameObject's layer to "HexTile" so TileSelector can filter it.
    //
    // Phase 2+: HexGridManager calls Initialize(HexTileData) before the object
    //           is activated, bypassing the fallback data in Start().
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class HexTile : MonoBehaviour
    {
        [SerializeField] private float        _size            = 1f;
        [SerializeField] private float        _height          = 0.3f;
        [SerializeField] private TerrainTypeSO _terrainOverride;  // for manual scene placement

        public HexTileData Data { get; private set; }

        // Called by HexGridManager before the GameObject is activated (Phase 2+).
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
            ApplyMaterial();
        }

        // Snap the tile's world position to its axial coordinates.
        // Phase 1: the tile is at (0,0) so this is a no-op unless overridden.
        private void ApplyWorldPosition()
        {
            if (Data.Coords != HexCoords.Zero)
                transform.position = Data.Coords.ToWorldPosition(_size);
        }

        private void BuildMesh()
        {
            var mesh = GenerateHexMesh(_size, _height);
            GetComponent<MeshFilter>().sharedMesh    = mesh;
            GetComponent<MeshCollider>().sharedMesh  = mesh;
        }

        // ── Mesh Generation ───────────────────────────────────────────────────
        //
        // Produces a flat-top hexagonal prism with exactly 12 vertices:
        //   verts[0..5]  — top ring (y = +height/2)
        //   verts[6..11] — bottom ring (y = -height/2)
        //
        // 20 triangles (60 indices):
        //   4 top face  +  4 bottom face  +  12 side (6 quads × 2)
        //
        // Shared vertices give flat-shaded appearance after RecalculateNormals(),
        // which is correct for a strategy-game terrain tile.
        internal static Mesh GenerateHexMesh(float size, float height)
        {
            var mesh = new Mesh { name = "HexTileMesh" };

            var verts = new Vector3[12];
            float h = height * 0.5f;

            // Flat-top: vertex i at angle 60° × i, starting at 0° (right).
            for (int i = 0; i < 6; i++)
            {
                float angle = Mathf.Deg2Rad * 60f * i;
                float x = size * Mathf.Cos(angle);
                float z = size * Mathf.Sin(angle);
                verts[i]     = new Vector3(x,  h, z); // top ring
                verts[i + 6] = new Vector3(x, -h, z); // bottom ring
            }

            // 20 triangles × 3 indices = 60 entries.
            var tris = new int[60];
            int ti = 0;

            // Top face — CCW fan from vertex 0 → normal points +Y.
            // Fan covers triangles (0,1,2), (0,2,3), (0,3,4), (0,4,5).
            for (int i = 1; i < 5; i++)
            {
                tris[ti++] = 0;
                tris[ti++] = i;
                tris[ti++] = i + 1;
            }

            // Bottom face — reversed winding → normal points -Y.
            // Fan from vertex 6: (6,8,7), (6,9,8), (6,10,9), (6,11,10).
            for (int i = 1; i < 5; i++)
            {
                tris[ti++] = 6;
                tris[ti++] = 6 + i + 1;
                tris[ti++] = 6 + i;
            }

            // Side faces — 6 quads, each split into 2 CW triangles (normals outward).
            for (int i = 0; i < 6; i++)
            {
                int n  = (i + 1) % 6;
                int b  = i + 6;
                int bn = n + 6;

                tris[ti++] = i;  tris[ti++] = n;  tris[ti++] = bn; // upper triangle
                tris[ti++] = i;  tris[ti++] = bn; tris[ti++] = b;  // lower triangle
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
            var color  = Data?.Terrain?.TintColor ?? Color.gray;
            var shader = Shader.Find("Universal Render Pipeline/Lit")
                      ?? Shader.Find("Standard");

            var renderer = GetComponent<MeshRenderer>();

            if (shader == null)
            {
                // Test / headless environments may have no shaders loaded.
                // Keep the default material; tint won't apply but mesh will render.
                if (renderer.sharedMaterial != null)
                    renderer.sharedMaterial.color = color;
                return;
            }

            renderer.sharedMaterial = new Material(shader) { color = color };
        }
    }
}
