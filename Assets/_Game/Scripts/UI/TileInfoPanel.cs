using System.Text;
using TalesOfTao.Hex;
using TMPro;
using UnityEngine;

namespace TalesOfTao.UI
{
    // Screen-space overlay panel showing properties of the last clicked hex tile.
    // Subscribes for the full scene lifetime (Awake→OnDestroy) so clicks are
    // never missed even while the panel is hidden.
    public class TileInfoPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _contentText;

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

        private static string BuildText(HexTileData d)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>Terrain</b>    {d.Terrain?.DisplayName ?? "—"}");
            sb.AppendLine($"<b>Coords</b>     {d.Coords}");
            sb.AppendLine($"<b>Elevation</b>  {d.Elevation}");
            sb.AppendLine($"<b>Qi Density</b> {d.QiDensity}");
            sb.AppendLine($"<b>Caves</b>      {d.CaveCount}");
            sb.AppendLine($"<b>Feature</b>    {d.Feature}");
            sb.AppendLine($"<b>Control</b>    {d.Control}");
            sb.AppendLine($"<b>Fortify</b>    {d.Fortification}");

            if (d.Deposits is { Length: > 0 })
                sb.AppendLine($"<b>Deposits</b>   {string.Join(", ", d.Deposits)}");

            return sb.ToString();
        }
    }
}
