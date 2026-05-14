using System.Collections.Generic;
using UnityEngine;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// Visual overlay for reachable tiles. Creates semi-transparent hex markers
    /// on all tiles reachable within a unit's movement budget.
    /// </summary>
    public class ReachableTileOverlay : MonoBehaviour
    {
        [SerializeField] private float _hexSize = 1f;
        [SerializeField] private Color _reachableColor = new Color(0.2f, 0.6f, 1f, 0.35f);
        [SerializeField] private Color _expensiveColor = new Color(1f, 0.6f, 0.1f, 0.35f);

        private readonly List<GameObject> _markers = new();
        private Material _reachableMat;
        private Material _expensiveMat;
        private static ReachableTileOverlay _instance;

        private void Awake()
        {
            _instance = this;
            CreateMaterials();
        }

        private void CreateMaterials()
        {
            var shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Standard");

            _reachableMat = new Material(shader);
            _reachableMat.color = _reachableColor;
            _reachableMat.SetFloat("_Surface", 1);
            _reachableMat.SetFloat("_Blend", 0);
            _reachableMat.renderQueue = 3000;

            _expensiveMat = new Material(shader);
            _expensiveMat.color = _expensiveColor;
            _expensiveMat.SetFloat("_Surface", 1);
            _expensiveMat.SetFloat("_Blend", 0);
            _expensiveMat.renderQueue = 3000;
        }

        /// <summary>
        /// Shows reachable tile markers from the given position within the movement budget.
        /// Tiles costing > 50% of budget are shown in the "expensive" color.
        /// </summary>
        public void ShowReachable(int startQ, int startR, float movementBudget, HexGridManager grid)
        {
            Clear();

            if (grid == null || !grid.IsGenerated || movementBudget <= 0f) return;

            var reachable = HexPathfinder.FindReachableTiles(grid, startQ, startR, movementBudget);
            float expensiveThreshold = movementBudget * 0.5f;

            foreach (var kvp in reachable)
            {
                var coords = kvp.Key;
                float cost = kvp.Value;

                // Skip the starting tile
                if (coords.Q == startQ && coords.R == startR) continue;

                var marker = CreateMarker(coords, cost > expensiveThreshold ? _expensiveMat : _reachableMat);
                _markers.Add(marker);
            }
        }

        /// <summary>
        /// Clears all reachable tile markers.
        /// </summary>
        public void Clear()
        {
            foreach (var marker in _markers)
            {
                if (marker != null) Destroy(marker);
            }
            _markers.Clear();
        }

        private GameObject CreateMarker(HexCoords coords, Material material)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = $"Reachable_{coords.Q}_{coords.R}";
            go.transform.SetParent(transform, false);

            float scale = _hexSize * 0.75f;
            var worldPos = coords.ToWorldPosition(_hexSize);
            go.transform.position = new Vector3(worldPos.x, 0.1f, worldPos.z);
            go.transform.localScale = new Vector3(scale, 0.03f, scale);

            var renderer = go.GetComponent<Renderer>();
            renderer.material = material;

            // Remove collider — these are visual only
            var col = go.GetComponent<Collider>();
            if (col != null) Destroy(col);

            return go;
        }

        /// <summary>
        /// Static helper to show reachable tiles from a position.
        /// </summary>
        public static void Show(int startQ, int startR, float movementBudget, HexGridManager grid)
        {
            if (_instance != null)
                _instance.ShowReachable(startQ, startR, movementBudget, grid);
        }

        /// <summary>
        /// Static helper to clear reachable tile display.
        /// </summary>
        public static void Hide()
        {
            if (_instance != null)
                _instance.Clear();
        }

        private void OnDestroy()
        {
            if (_reachableMat != null) Destroy(_reachableMat);
            if (_expensiveMat != null) Destroy(_expensiveMat);
            Clear();
        }
    }
}
