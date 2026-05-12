using UnityEngine;

namespace TalesOfTao.Hex
{
    // Static configuration for one terrain type.
    // Create one asset per type: Assets > Create > TalesOfTao > Hex > Terrain Type
    // Save all 8 into Assets/_Game/Data/Terrain/.
    //
    // Phase 2 additions:
    //   _baseMesh     — imported .obj hex base mesh (overrides procedural mesh when set)
    //   _featurePrefab — Feature_ child prefab (Forest, Mountain, etc.) spawned at tile origin
    [CreateAssetMenu(menuName = "TalesOfTao/Hex/Terrain Type", fileName = "TerrainType_New")]
    public class TerrainTypeSO : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private TerrainType _type         = TerrainType.Plains;
        [SerializeField] private string      _displayName  = "Plains";

        [Header("Gameplay")]
        [SerializeField] private float _movementCost = 1f;

        [Tooltip("Additive percentage bonus to defending unit's combat power.")]
        [SerializeField] private float _defenseBonus = 0f;

        [Tooltip("Multiplier applied to raw Qi income on this tile type.")]
        [SerializeField] private float _qiModifier   = 1f;

        [Tooltip("Units cannot enter this tile (e.g. Lake).")]
        [SerializeField] private bool  _isImpassable = false;

        [Header("Visuals")]
        [SerializeField] private Color      _tintColor    = new Color(0.55f, 0.76f, 0.29f);

        [Tooltip("Imported .obj hex base mesh. Overrides procedural mesh when assigned.")]
        [SerializeField] private Mesh       _baseMesh;

        [Tooltip("Feature prefab (Forest, Mountain, etc.) instantiated as a child at local (0,0,0).")]
        [SerializeField] private GameObject _featurePrefab;

        public TerrainType Type         => _type;
        public string      DisplayName  => _displayName;
        public float       MovementCost => _movementCost;
        public float       DefenseBonus => _defenseBonus;
        public float       QiModifier   => _qiModifier;
        public bool        IsImpassable => _isImpassable;
        public Color       TintColor    => _tintColor;
        public Mesh        BaseMesh     => _baseMesh;
        public GameObject  FeaturePrefab => _featurePrefab;
    }
}
