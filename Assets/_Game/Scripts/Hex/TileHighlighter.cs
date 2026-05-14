using UnityEngine;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// Visual highlight for the selected hex tile.
    /// Creates a visible 3D ring above the tile using Unity's built-in cylinder.
    /// </summary>
    public class TileHighlighter : MonoBehaviour
    {
        [SerializeField] private float _hexSize = 1f;
        [SerializeField] private float _highlightHeight = 0.25f;

        private GameObject _highlightObj;
        private static TileHighlighter _instance;

        private void Awake()
        {
            _instance = this;
            CreateHighlight();
        }

        private void CreateHighlight()
        {
            // Use a cylinder mesh (Unity primitive)
            _highlightObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _highlightObj.name = "SelectionHighlight";
            _highlightObj.transform.SetParent(transform, false);

            // Scale to fit hex tile
            float scale = _hexSize * 0.85f;
            _highlightObj.transform.localScale = new Vector3(scale, 0.05f, scale);

            // Make it gold/yellow and transparent
            var renderer = _highlightObj.GetComponent<Renderer>();
            var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Standard"));
            mat.color = new Color(1f, 0.85f, 0.1f, 0.7f);
            mat.SetFloat("_Surface", 1); // Transparent
            mat.SetFloat("_Blend", 0);   // Alpha blend
            mat.renderQueue = 3000;
            renderer.material = mat;

            _highlightObj.SetActive(false);
        }

        public void Show(HexCoords coords)
        {
            if (_highlightObj == null) return;
            var worldPos = coords.ToWorldPosition(_hexSize);
            transform.position = new Vector3(worldPos.x, _highlightHeight, worldPos.z);
            _highlightObj.SetActive(true);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (_highlightObj != null) _highlightObj.SetActive(false);
            gameObject.SetActive(false);
        }

        public static void SelectTile(HexTileData tile)
        {
            if (_instance == null)
            {
                Debug.LogWarning("[TileHighlighter] _instance is null!");
                return;
            }
            if (tile == null) return;
            _instance.Show(tile.Coords);
        }

        public static void ClearSelection()
        {
            if (_instance != null) _instance.Hide();
        }
    }
}
