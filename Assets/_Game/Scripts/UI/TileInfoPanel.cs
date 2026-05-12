using System.Text;
using TalesOfTao.Hex;
using TMPro;
using UnityEngine;

namespace TalesOfTao.UI
{
    // Screen-space overlay panel showing properties of the last clicked hex tile.
    // Subscribes for the full scene lifetime (Awake -> OnDestroy) so clicks are
    // never missed even while the panel is hidden.
    public class TileInfoPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _contentText;

        // Reused across clicks to avoid per-frame StringBuilder allocation.
        private readonly StringBuilder _sb = new(256);

        private void Awake()
        {
            gameObject.SetActive(false);
            TileSelector.TileSelected += Show;
        }

        private void OnDestroy() => TileSelector.TileSelected -= Show;

        public void Show(HexTileData data)
        {
            if (data == null) { gameObject.SetActive(false); return; }
            if (_contentText == null) return;

            _contentText.text = BuildText(data);
            gameObject.SetActive(true);
        }

        public void Hide() => gameObject.SetActive(false);

        private string BuildText(HexTileData d)
        {
            _sb.Clear();
            _sb.AppendLine($"<b>Terrain</b>    {d.Terrain?.DisplayName ?? "—"}");
            _sb.AppendLine($"<b>Coords</b>     {d.Coords}");
            _sb.AppendLine($"<b>Elevation</b>  {d.Elevation}");
            _sb.AppendLine($"<b>Qi Density</b> {d.QiDensity}");
            _sb.AppendLine($"<b>Caves</b>      {d.CaveCount}");
            _sb.AppendLine($"<b>Feature</b>    {d.Feature}");
            _sb.AppendLine($"<b>Control</b>    {d.Control}");
            _sb.AppendLine($"<b>Fortify</b>    {d.Fortification}");

            if (d.Deposits is { Length: > 0 })
                _sb.AppendLine($"<b>Deposits</b>   {string.Join(", ", d.Deposits)}");

            return _sb.ToString();
        }
    }
}
