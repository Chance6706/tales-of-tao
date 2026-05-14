using UnityEngine;

namespace TalesOfTao.Sects
{
    /// <summary>
    /// Static configuration for one sect type. Create via Assets > Create > TalesOfTao > Sect Config.
    /// Defines identity: Affinity, Trait, Unique Hall, banner appearance.
    /// </summary>
    [CreateAssetMenu(menuName = "TalesOfTao/Sect Config", fileName = "SectConfig_New")]
    public class SectConfigSO : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string _displayName = "New Sect";
        [SerializeField] private SectAffinity _affinity = SectAffinity.InternalQi;
        [SerializeField] private SectTrait _trait = SectTrait.None;

        [Header("Banner")]
        [SerializeField] private Color _primaryColor = Color.white;
        [SerializeField] private Color _secondaryColor = Color.black;
        [SerializeField] private string _emblemId = "default";

        [Header("Unique Hall")]
        [Tooltip("Building type this sect's unique hall replaces (e.g. TrainingGrounds). None = no unique hall.")]
        [SerializeField] private string _replacesBuilding = "";
        [Tooltip("Bonus description for the unique hall.")]
        [SerializeField] private string _uniqueHallBonus = "";

        [Header("Starting Bonuses")]
        [SerializeField] private int _startingTael = 100;
        [SerializeField] private int _startingQi = 20;
        [SerializeField] private int _startingPeons = 5;

        public string DisplayName => _displayName;
        public SectAffinity Affinity => _affinity;
        public SectTrait Trait => _trait;
        public Color PrimaryColor => _primaryColor;
        public Color SecondaryColor => _secondaryColor;
        public string EmblemId => _emblemId;
        public string ReplacesBuilding => _replacesBuilding;
        public string UniqueHallBonus => _uniqueHallBonus;
        public int StartingTael => _startingTael;
        public int StartingQi => _startingQi;
        public int StartingPeons => _startingPeons;
    }
}
