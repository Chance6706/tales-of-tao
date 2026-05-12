using System;
using UnityEngine;

namespace TalesOfTao.Hex
{
    // Attach to the Main Camera. Raycasts on left-click to find HexTile objects
    // and raises TileSelected so any listener can respond without a direct reference.
    //
    // HexLayer default is 0 (no layers) so misconfiguration is immediately obvious.
    [RequireComponent(typeof(Camera))]
    public class TileSelector : MonoBehaviour
    {
        public static event Action<HexTileData> TileSelected;

        [SerializeField] private LayerMask _hexLayer = 0;

        private Camera  _cam;
        private HexTile _currentSelection;

        private void Awake() => _cam = GetComponent<Camera>();

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;

            var ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _hexLayer))
            {
                var tile = hit.collider.GetComponent<HexTile>();
                if (tile != null && tile != _currentSelection)
                {
                    _currentSelection = tile;
                    TileSelected?.Invoke(tile.Data);
                }
            }
        }
    }
}
