using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TalesOfTao.Hex
{
    // Attach to the Main Camera. Raycasts on left-click to find HexTile objects
    // and raises TileSelected so any listener can respond without a direct reference.
    //
    // Uses RaycastAll to handle overlapping colliders (e.g. feature prefabs on top of
    // base hex tiles) — selects the first HexTile found from the base mesh layer.
    //
    // HexLayer default is 0 (all layers) so misconfiguration is immediately obvious.
    [RequireComponent(typeof(Camera))]
    public class TileSelector : MonoBehaviour
    {
        public static event Action<HexTileData> TileSelected;

        [SerializeField] private LayerMask _hexLayer = 0;

        private Camera _cam;
        private HexTile _currentSelection;
        private bool _mousePressed;

        private void Awake() => _cam = GetComponent<Camera>();

        private void Update()
        {
            // Read mouse state via new Input System
            var mouse = Mouse.current;
            if (mouse == null) return;

            // Check for left button press this frame
            if (mouse.leftButton.wasPressedThisFrame)
            {
                var mousePosition = mouse.position.ReadValue();
                var ray = _cam.ScreenPointToRay(mousePosition);

                // Use RaycastAll so overlapping colliders (feature prefabs, decorative meshes)
                // don't block the base hex tile hit.
                var hits = Physics.RaycastAll(ray, Mathf.Infinity, _hexLayer)
                    .OrderBy(h => h.distance);

                foreach (var hit in hits)
                {
                    var tile = hit.collider.GetComponent<HexTile>();
                    if (tile != null && tile != _currentSelection)
                    {
                        _currentSelection = tile;
                        TileSelected?.Invoke(tile.Data);
                        return;
                    }
                }
            }
        }
    }
}
