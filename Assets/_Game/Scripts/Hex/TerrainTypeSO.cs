using UnityEngine;

namespace TalesOfTao.Hex
{
    // Static configuration for one terrain type. Create one asset per type via:
    //   Assets > Create > TalesOfTao > Hex > Terrain Type
    // and save all 8 into Assets/_Game/Data/Terrain/.
    [CreateAssetMenu(menuName = "TalesOfTao/Hex/Terrain Type", fileName = "TerrainType_Plains")]
    public class TerrainTypeSO : ScriptableObject
    {
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
