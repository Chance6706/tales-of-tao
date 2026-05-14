using UnityEngine;
using UnityEngine.UIElements;

namespace TalesOfTao.UI
{
    /// <summary>
    /// Sect Overview screen showing buildings, disciples, resources, and dissent.
    /// Uses UI Toolkit (UXML/USS). Attach to a GameObject with a UIDocument.
    /// </summary>
    public class SectOverviewScreen : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset _uxml;
        [StyleSheet]
        private StyleSheet _uss;

        private VisualElement _root;
        private Label _buildingStatusLabel;
        private Label _discipleRosterLabel;
        private Label _resourceSummaryLabel;
        private ProgressBar _dissentBar;
        private Label _zodiacLabel;
        private Button _closeButton;

        private void OnEnable()
        {
            var doc = GetComponent<UIDocument>();
            if (doc == null) return;

            _root = doc.rootVisualElement;

            // Query elements
            _buildingStatusLabel = _root.Q<Label>("building-status");
            _discipleRosterLabel = _root.Q<Label>("disciple-roster");
            _resourceSummaryLabel = _root.Q<Label>("resource-summary");
            _dissentBar = _root.Q<ProgressBar>("dissent-bar");
            _zodiacLabel = _root.Q<Label>("zodiac-label");
            _closeButton = _root.Q<Button>("close-button");

            if (_closeButton != null)
                _closeButton.clicked += Hide;

            Hide();
        }

        public void Show()
        {
            if (_root != null)
                _root.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            if (_root != null)
                _root.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// Updates all panels from SectData.
        /// </summary>
        public void Refresh(Sects.SectData sectData, int currentZodiacYear = 0)
        {
            if (sectData == null) return;

            RefreshBuildings(sectData);
            RefreshDisciples(sectData);
            RefreshResources(sectData);
            RefreshDissent(sectData);
            RefreshZodiac(currentZodiacYear);
        }

        private void RefreshBuildings(Sects.SectData sectData)
        {
            if (_buildingStatusLabel == null) return;

            var sb = new System.Text.StringBuilder(512);
            sb.AppendLine("<b>Buildings</b>");

            var buildings = sectData.GetBuildings();
            if (buildings.Count == 0)
            {
                sb.AppendLine("  (none constructed)");
            }
            else
            {
                foreach (var b in buildings)
                {
                    sb.AppendLine($"  {b.BuildingTypeId} T{b.Tier}");
                }
            }

            _buildingStatusLabel.text = sb.ToString();
        }

        private void RefreshDisciples(Sects.SectData sectData)
        {
            if (_discipleRosterLabel == null) return;

            var sb = new System.Text.StringBuilder(512);
            sb.AppendLine("<b>Disciples</b>");

            int peons = sectData.GetDiscipleCount(Sects.DiscipleRank.Peon);
            int outer = sectData.GetDiscipleCount(Sects.DiscipleRank.OuterDisciple);
            int inner = sectData.GetDiscipleCount(Sects.DiscipleRank.InnerDisciple);
            int elders = sectData.GetDiscipleCount(Sects.DiscipleRank.Elder);
            int highElders = sectData.GetDiscipleCount(Sects.DiscipleRank.HighElder);

            sb.AppendLine($"  Peons: {peons}");
            sb.AppendLine($"  Outer Disciples: {outer}");
            sb.AppendLine($"  Inner Disciples: {inner}");
            sb.AppendLine($"  Elders: {elders}");
            sb.AppendLine($"  High Elders: {highElders}");
            sb.AppendLine($"  Total: {sectData.TotalDisciples}");

            // Show ratio warnings
            if (peons > outer * 10)
                sb.AppendLine("  <color=red>⚠ Peon ratio exceeded!</color>");
            if (outer > inner * 10)
                sb.AppendLine("  <color=red>⚠ Outer Disciple ratio exceeded!</color>");

            _discipleRosterLabel.text = sb.ToString();
        }

        private void RefreshResources(Sects.SectData sectData)
        {
            if (_resourceSummaryLabel == null) return;

            var r = sectData.Stockpile;
            var sb = new System.Text.StringBuilder(512);
            sb.AppendLine("<b>Resources</b>");
            sb.AppendLine($"  Tael: {r.Tael}");
            sb.AppendLine($"  Qi: {r.Qi}");
            sb.AppendLine($"  Lumber: {r.Lumber}");
            sb.AppendLine($"  Iron Ore: {r.IronOre}");
            sb.AppendLine($"  Jade: {r.Jade}");
            sb.AppendLine($"  Medicinal Herbs: {r.MedicinalHerbs}");
            sb.AppendLine($"  Spirit Herbs: {r.SpiritHerbs}");
            sb.AppendLine($"  Tea Leaves: {r.TeaLeaves}");

            _resourceSummaryLabel.text = sb.ToString();
        }

        private void RefreshDissent(Sects.SectData sectData)
        {
            if (_dissentBar == null) return;

            _dissentBar.value = sectData.DissentLevel;
            _dissentBar.highValue = 100;
            _dissentBar.title = $"Dissent: {sectData.DissentLevel}/100";
        }

        private void RefreshZodiac(int year)
        {
            if (_zodiacLabel == null) return;

            if (year > 0)
            {
                string[] animals = { "Rat", "Ox", "Tiger", "Rabbit", "Dragon", "Snake",
                    "Horse", "Goat", "Monkey", "Rooster", "Dog", "Pig" };
                string animal = animals[year % 12];
                _zodiacLabel.text = $"Year of the {animal} ({year})";
            }
            else
            {
                _zodiacLabel.text = "";
            }
        }
    }
}
