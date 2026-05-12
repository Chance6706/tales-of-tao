using UnityEngine;

namespace TalesOfTao.Hex
{
    // Static configuration for one terrain type. Create one asset per type via:
    //   Assets > Create > TalesOfTao > Hex > Terrain Type
    // and save all 8 into Assets/_Game/Data/Terrain/.
    [CreateAssetMenu(menuName = "TalesOfTao/Hex/Terrain Type", fileName = "TerrainType_Plains")]
    public class TerrainTypeSO : ScriptableObject
    {
        [SerializeField] private TerrainType _type         = TerrainType.Plains;
        [SerializeField] private string      _displayName  = "Plains";
        [SerializeField] private float       _movementCost = 1f;

        [Tooltip("Additive percentage bonus to defending unit's combat power.")]
        [SerializeField] private float _defenseBonus = 0f;

        [Tooltip("Multiplier applied to raw Qi income on this tile type.")]
        [SerializeField] private float _qiModifier   = 1f;

        [SerializeField] private Color _tintColor    = new Color(0.55f, 0.76f, 0.29f);

        [Tooltip("Units cannot enter this tile (e.g. Lake).")]
        [SerializeField] private bool  _isImpassable = false;

        public TerrainType Type         => _type;
        public string      DisplayName  => _displayName;
        public float       MovementCost => _movementCost;
        public float       DefenseBonus => _defenseBonus;
        public float       QiModifier   => _qiModifier;
        public Color       TintColor    => _tintColor;
        public bool        IsImpassable => _isImpassable;
        [field: SerializeField] public TerrainType Type         { get; private set; } = TerrainType.Plains;
        [field: SerializeField] public string      DisplayName  { get; private set; } = "Plains";
        [field: SerializeField] public float       MovementCost { get; private set; } = 1f;

        [Tooltip("Additive percentage bonus to defending unit's combat power.")]
        [field: SerializeField] public float DefenseBonus  { get; private set; } = 0f;

        [Tooltip("Multiplier applied to raw Qi income on this tile type.")]
        [field: SerializeField] public float QiModifier   { get; private set; } = 1f;

        [field: SerializeField] public Color TintColor    { get; private set; } = new Color(0.55f, 0.76f, 0.29f);

        [Tooltip("Units cannot enter this tile (e.g. Lake).")]
        [field: SerializeField] public bool  IsImpassable { get; private set; } = false;
    }
}
