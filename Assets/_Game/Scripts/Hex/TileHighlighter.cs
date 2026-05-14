using UnityEngine;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// Visual highlight for the selected hex tile.
    /// Uses a simple quad mesh that's easy to see.
    /// </summary>
    public class TileHighlighter : MonoBehaviour
    {
        [SerializeField] private float _hexSize = 1f;
        [SerializeField] private float _highlightHeight = 0.3f;
        [SerializeField] private Color _highlightColor = new Color(1f, 0.9f, 0.2f, 0.85f);

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private Material _material;
        private static TileHighlighter _instance;

        private void Awake()
        {
            _instance = this;
            CreateHighlightMesh();
            gameObject.SetActive(false);
        }

        private void CreateHighlightMesh()
        {
            var meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshFilter = meshFilter;

            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _meshRenderer = meshRenderer;

            // Create a simple hexagon mesh (flat, facing up)
            var mesh = new Mesh { name = "HexHighlight" };
            int segments = 6;
            var verts = new Vector3[segments + 1];
            var tris = new int[segments * 3];
            var colors = new Color[segments + 1];

            float radius = _hexSize * 0.9f;
            verts[0] = Vector3.zero;
            colors[0] = _highlightColor;

            for (int i = 0; i < segments; i++)
            {
                float angle = Mathf.Deg2Rad * 60f * i;
                verts[1 + i] = new Vector3(radius * Mathf.Cos(angle), 0f, radius * Mathf.Sin(angle));
                colors[1 + i] = _highlightColor;

                tris[i * 3 + 0] = 0;
                tris[i * 3 + 1] = 1 + i;
                tris[i * 3 + 2] = 1 + ((i + 1) % segments);
            }

            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.colors = colors;
            mesh.RecalculateNormals();
            _meshFilter.sharedMesh = mesh;

            // Use unlit shader so it's always visible
            Shader shader = Shader.Find("TalesOfTao/HexColorPerTile")
                         ?? Shader.Find("Universal Render Pipeline/Unlit")
                         ?? Shader.Find("Standard");
            _material = new Material(shader)
            {
                name = "HighlightMat",
                color = _highlightColor
            };
            _material.SetFloat("_ColorMask", 15);
            _meshRenderer.sharedMaterial = _material;
        }

        public void Show(HexCoords coords)
        {
            var worldPos = coords.ToWorldPosition(_hexSize);
            transform.position = new Vector3(worldPos.x, _highlightHeight, worldPos.z);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public static void SelectTile(HexTileData tile)
        {
            if (_instance == null)
            {
                Debug.LogWarning("[TileHighlighter] _instance is null!");
                return;
            }
            if (tile == null)
            {
                Debug.LogWarning("[TileHighlighter] tile is null!");
                return;
            }
            _instance.Show(tile.Coords);
            Debug.Log($"[TileHighlighter] Show at ({tile.Coords.Q},{tile.Coords.R}), worldPos=({tile.Coords.ToWorldPosition(1f)})");
        }

        public static void ClearSelection()
        {
            if (_instance != null) _instance.Hide();
        }
    }
}
