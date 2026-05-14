using UnityEngine;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// Component on each chunk GameObject holding the combined mesh and collider.
    /// Supports multiple sub-meshes with separate materials for terrain coloring.
    /// </summary>
    public class HexChunkRenderer : MonoBehaviour
    {
        public Vector2Int ChunkCoord { get; set; }

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private MeshCollider _meshCollider;

        /// <summary>
        /// Sets a single-material mesh (backward compatible).
        /// </summary>
        public void SetMesh(Mesh mesh, Material material)
        {
            SetMesh(mesh, material != null ? new[] { material } : null);
        }

        /// <summary>
        /// Sets a multi-submesh mesh with per-submesh materials.
        /// </summary>
        public void SetMesh(Mesh mesh, Material[] materials)
        {
            if (_meshFilter == null)
                _meshFilter = gameObject.AddComponent<MeshFilter>();
            if (_meshRenderer == null)
                _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            if (_meshCollider == null)
                _meshCollider = gameObject.AddComponent<MeshCollider>();

            _meshFilter.sharedMesh = mesh;
            _meshRenderer.sharedMaterials = materials ?? new Material[0];
            _meshCollider.sharedMesh = mesh;
            gameObject.isStatic = true;

            // Also add a box collider for reliable raycasting
            var boxCol = gameObject.GetComponent<BoxCollider>();
            if (boxCol == null) boxCol = gameObject.AddComponent<BoxCollider>();
            boxCol.center = mesh.bounds.center;
            boxCol.size = mesh.bounds.size;
        }
    }
}
