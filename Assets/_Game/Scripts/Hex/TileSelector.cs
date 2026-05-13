using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// Attach to the Main Camera. Raycasts on left-click to find hex tiles
    /// and raises TileSelected so any listener can respond without a direct reference.
    ///
    /// Works with chunk-based rendering: when a chunk collider is hit,
    /// calculates which hex tile was clicked based on world position.
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
            _gridManager = Object.FindAnyObjectByType<HexGridManager>();
        }

        private void Update()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;
            if (!mouse.leftButton.wasPressedThisFrame) return;

            var mousePos = mouse.position.ReadValue();
            var ray = _cam.ScreenPointToRay(mousePos);

            if (_gridManager == null || !_gridManager.IsGenerated)
            {
                // Fallback: try to find individual HexTile GameObjects (legacy)
                TrySelectLegacy(ray);
                return;
            }

            // Raycast against chunk colliders
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _hexLayer))
            {
                // Convert hit point to hex coordinates
                var hexCoords = WorldToHex(hit.point);
                var tile = _gridManager.GetTile(hexCoords.Q, hexCoords.R);
                if (tile != null)
                {
                    _currentSelection = tile;
                    TileSelected?.Invoke(tile);
                }
            }
        }

        private void TrySelectLegacy(Ray ray)
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

        /// <summary>
        /// Converts a world position to the nearest hex axial coordinates.
        /// Uses flat-top hex math matching HexCoords.ToWorldPosition().
        /// </summary>
        private static HexCoords WorldToHex(Vector3 worldPos)
        {
            // Inverse of: x = size * 1.5 * q, z = size * (sqrt3/2 * q + sqrt3 * r)
            float size = 1f; // Must match HexGridRenderer._hexSize
            float sqrt3 = 1.732051f;

            float q = (2f / 3f * worldPos.x) / size;
            float r = (-1f / 3f * worldPos.x + sqrt3 / 3f * worldPos.z) / size;

            // Round to nearest hex
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
