using UnityEngine;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// Component on each chunk GameObject holding the combined mesh and collider.
    /// </summary>
    public class HexChunkRenderer : MonoBehaviour
    {
        public Vector2Int ChunkCoord { get; set; }

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private MeshCollider _meshCollider;

        public void SetMesh(Mesh mesh, Material material)
        {
            if (_meshFilter == null)
                _meshFilter = gameObject.AddComponent<MeshFilter>();
            if (_meshRenderer == null)
                _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            if (_meshCollider == null)
                _meshCollider = gameObject.AddComponent<MeshCollider>();

            _meshFilter.sharedMesh = mesh;
            _meshRenderer.sharedMaterial = material;
            _meshCollider.sharedMesh = mesh;
        }
    }
}
