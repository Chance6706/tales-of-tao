using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using TalesOfTao.UI.HUD;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// Attach to the Main Camera. Raycasts on left-click to find hex tiles
    /// and raises TileSelected so any listener can respond without a direct reference.
    /// Works with both chunk-based and individual HexTile rendering.
    /// Automatically detects new vs old Input System based on Player Settings.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class TileSelector : MonoBehaviour
    {
        public static event Action<HexTileData> TileSelected;

        /// <summary>
        /// Set this to show reachable tiles from the selected position.
        /// Set to 0 to disable reachable tile display.
        /// </summary>
        public static float MovementBudget { get; set; } = 0f;

        [SerializeField] private LayerMask _hexLayer = ~0;
        [SerializeField] private float _hexSize = 1f;

        private Camera _cam;
        private HexTileData _currentSelection;
        private HexGridManager _gridManager;
        private bool _useNewInput;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            _useNewInput = Mouse.current != null;
        }

        private void Start()
        {
            // Find the grid manager and auto-generate if needed
            _gridManager = HexGridManager.Instance;
            if (_gridManager == null)
                _gridManager = UnityEngine.Object.FindAnyObjectByType<HexGridManager>();
            if (_gridManager != null && _gridManager.TileCount == 0)
            {
                Debug.Log("[TileSelector] Auto-generating map in Play mode...");
                _gridManager.GenerateMap();
            }

            // Ensure TileHighlighter exists
            if (UnityEngine.Object.FindAnyObjectByType<TileHighlighter>() == null)
            {
                var go = new GameObject("TileHighlighter");
                go.AddComponent<TileHighlighter>();
            }

            // Ensure ReachableTileOverlay exists
            if (UnityEngine.Object.FindAnyObjectByType<ReachableTileOverlay>() == null)
            {
                var go = new GameObject("ReachableTileOverlay");
                go.AddComponent<ReachableTileOverlay>();
            }
        }

        private void Update()
        {
            // Find the grid manager that has tile data
            _gridManager = HexGridManager.Instance;

            bool leftClicked;
            Vector3 mousePos;

            if (_useNewInput)
            {
                var mouse = Mouse.current;
                if (mouse == null) return;
                leftClicked = mouse.leftButton.wasPressedThisFrame;
                mousePos = mouse.position.ReadValue();
            }
            else
            {
                leftClicked = Input.GetMouseButtonDown(0);
                mousePos = Input.mousePosition;
            }

            if (!leftClicked) return;

            // Don't select tiles if clicking on the HUD button
            if (TurnTestHUD.IsMouseOverButton) return;

            var ray = _cam.ScreenPointToRay(mousePos);

            // Try chunk-based selection if grid manager has tiles (IsGenerated may not be set yet)
            if (_gridManager != null && _gridManager.TileCount > 0)
            {
                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _hexLayer))
                {
                    var hexCoords = WorldToHex(hit.point, _hexSize);
                    var tile = _gridManager.GetTile(hexCoords.Q, hexCoords.R);
                    if (tile != null)
                    {
                        _currentSelection = tile;
                        TileSelected?.Invoke(tile);
                        float elevationY = hit.point.y;
                        TileHighlighter.SelectTile(tile, elevationY);

                        // Show reachable tiles if movement budget is set
                        if (MovementBudget > 0f && _gridManager != null)
                        {
                            ReachableTileOverlay.Show(hexCoords.Q, hexCoords.R, MovementBudget, _gridManager);
                        }
                        else
                        {
                            ReachableTileOverlay.Hide();
                        }
                        return;
                    }
                    Debug.Log($"[TileSelector] Raycast hit chunk but no tile at ({hexCoords.Q},{hexCoords.R}).");
                }
                else
                {
                    Debug.Log("[TileSelector] Raycast hit nothing.");
                }
            }
            else
            {
                var hits = Physics.RaycastAll(ray, Mathf.Infinity, _hexLayer)
                    .OrderBy(h => h.distance);
                foreach (var hit in hits)
                {
                    var tile = hit.collider.GetComponent<HexTile>();
                    if (tile != null)
                    {
                        _currentSelection = tile.Data;
                        TileSelected?.Invoke(tile.Data);
                        return;
                    }
                }
            }
        }

        private static HexCoords WorldToHex(Vector3 worldPos, float size)
        {
            float sqrt3 = 1.732051f;

            float q = (2f / 3f * worldPos.x) / size;
            float r = (-1f / 3f * worldPos.x + sqrt3 / 3f * worldPos.z) / size;
            float s = -q - r;

            int rq = Mathf.RoundToInt(q);
            int rr = Mathf.RoundToInt(r);
            int rs = Mathf.RoundToInt(s);

            float dq = Mathf.Abs(rq - q);
            float dr = Mathf.Abs(rr - r);
            float ds = Mathf.Abs(rs - s);

            if (dq > dr && dq > ds)
                rq = -rr - rs;
            else if (dr > ds)
                rr = -rq - rs;

            return new HexCoords(rq, rr);
        }
    }
}
