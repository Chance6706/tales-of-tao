using UnityEngine;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// Visual highlight for the selected hex tile.
    /// Creates a flat ring around the selected tile.
    /// </summary>
    public class TileHighlighter : MonoBehaviour
    {
        [SerializeField] private float _hexSize = 1f;
        [SerializeField] private float _ringHeight = 0.15f;
        [SerializeField] private float _ringThickness = 0.08f;
        [SerializeField] private Color _ringColor = new Color(1f, 0.85f, 0.2f, 0.9f); // gold

        private GameObject _ringObject;
        private MeshFilter _ringFilter;
        private MeshRenderer _ringRenderer;
        private Material _ringMaterial;
        private HexCoords _currentCoords;
        private bool _visible;

        private static TileHighlighter _instance;

        private void Awake()
        {
            _instance = this;
            CreateRingMesh();
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_ringObject != null) Destroy(_ringObject);
            if (_ringMaterial != null) Destroy(_ringMaterial);
        }

        private void CreateRingMesh()
        {
            _ringObject = new GameObject("SelectionRing");
            _ringObject.transform.SetParent(transform, false);
            _ringFilter = _ringObject.AddComponent<MeshFilter>();
            _ringRenderer = _ringObject.AddComponent<MeshRenderer>();

            // Create ring mesh (hexagon with hole)
            _ringFilter.sharedMesh = GenerateRingMesh(_hexSize, _ringThickness);

            // Create unlit transparent material
            Shader shader = Shader.Find("TalesOfTao/HexColorPerTile")
                         ?? Shader.Find("Universal Render Pipeline/Unlit")
                         ?? Shader.Find("Standard");
            _ringMaterial = new Material(shader)
            {
                name = "SelectionRingMat",
                color = _ringColor
            };
            _ringRenderer.sharedMaterial = _ringMaterial;
        }

        private static Mesh GenerateRingMesh(float size, float thickness)
        {
            var mesh = new Mesh { name = "HexRing" };
            int segments = 6;
            int vertCount = segments * 2; // inner + outer ring
            var verts = new Vector3[vertCount + 1]; // +1 for center
            var tris = new int[segments * 6]; // 2 triangles per segment

            float outerR = size * 0.95f;
            float innerR = outerR - thickness;

            // Center vertex
            verts[0] = Vector3.zero;

            for (int i = 0; i < segments; i++)
            {
                float angle = Mathf.Deg2Rad * 60f * i;
                float xOuter = outerR * Mathf.Cos(angle);
                float zOuter = outerR * Mathf.Sin(angle);
                float xInner = innerR * Mathf.Cos(angle);
                float zInner = innerR * Mathf.Sin(angle);

                verts[1 + i * 2] = new Vector3(xOuter, 0f, zOuter); // outer
                verts[2 + i * 2] = new Vector3(xInner, 0f, zInner); // inner

                // Triangle: center -> outer[i] -> inner[i]
                tris[i * 6 + 0] = 0;
                tris[i * 6 + 1] = 1 + i * 2;
                tris[i * 6 + 2] = 2 + i * 2;

                // Triangle: center -> inner[i] -> outer[i+1]
                int nextOuter = 1 + ((i + 1) % segments) * 2;
                tris[i * 6 + 3] = 0;
                tris[i * 6 + 4] = 2 + i * 2;
                tris[i * 6 + 5] = nextOuter;
            }

            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.RecalculateNormals();
            return mesh;
        }

        public void Show(HexCoords coords)
        {
            _currentCoords = coords;
            var worldPos = coords.ToWorldPosition(_hexSize);
            transform.position = new Vector3(worldPos.x, _ringHeight, worldPos.z);
            gameObject.SetActive(true);
            _visible = true;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _visible = false;
        }

        /// <summary>
        /// Call this from TileSelector when a tile is selected.
        /// </summary>
        public static void SelectTile(HexTileData tile)
        {
            if (_instance != null && tile != null)
            {
                _instance.Show(tile.Coords);
            }
        }

        /// <summary>
        /// Call this to clear selection.
        /// </summary>
        public static void ClearSelection()
        {
            if (_instance != null) _instance.Hide();
        }
    }
}
