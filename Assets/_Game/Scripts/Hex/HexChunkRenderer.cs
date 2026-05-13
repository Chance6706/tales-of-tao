using UnityEngine;

namespace TalesOfTao.Hex
{
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
