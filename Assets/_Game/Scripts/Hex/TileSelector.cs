using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// Attach to the Main Camera. Raycasts on left-click to find hex tiles
    /// and raises TileSelected so any listener can respond without a direct reference.
    /// Works with both chunk-based and individual HexTile rendering.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class TileSelector : MonoBehaviour
    {
        public static event Action<HexTileData> TileSelected;

        [SerializeField] private LayerMask _hexLayer = 0;

        private Camera _cam;
        private HexTileData _currentSelection;
        private HexGridManager _gridManager;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            _gridManager = UnityEngine.Object.FindAnyObjectByType<HexGridManager>();
        }

        private void Update()
        {
            bool leftClicked = false;

            // Try new Input System first
            var mouse = Mouse.current;
            if (mouse != null)
            {
                leftClicked = mouse.leftButton.wasPressedThisFrame;
            }
            else
            {
                // Fallback to old Input
                leftClicked = Input.GetMouseButtonDown(0);
            }

            if (!leftClicked) return;

            Vector3 mousePos;
            if (mouse != null)
                mousePos = mouse.position.ReadValue();
            else
                mousePos = Input.mousePosition;

            var ray = _cam.ScreenPointToRay(mousePos);

            if (_gridManager != null && _gridManager.IsGenerated)
            {
                // Chunk-based raycast
                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _hexLayer))
                {
                    var hexCoords = WorldToHex(hit.point);
                    var tile = _gridManager.GetTile(hexCoords.Q, hexCoords.R);
                    if (tile != null)
                    {
                        _currentSelection = tile;
                        TileSelected?.Invoke(tile);
                    }
                }
            }
            else
            {
                // Legacy: individual HexTile GameObjects
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

        private static HexCoords WorldToHex(Vector3 worldPos)
        {
            float size = 1f;
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
