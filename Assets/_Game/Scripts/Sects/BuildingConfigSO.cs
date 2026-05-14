using UnityEngine;

namespace TalesOfTao.Sects
{
    /// <summary>
    /// Resource cost for building construction or promotion.
    /// </summary>
    [System.Serializable]
    public struct ResourceCost
    {
        public int Tael;
        public int Qi;
        public int Lumber;
        public int IronOre;
        public int Jade;
        public int MedicinalHerbs;
        public int SpiritHerbs;

        public bool CanAfford(ResourceCost available)
        {
            return available.Tael >= Tael
                && available.Qi >= Qi
                && available.Lumber >= Lumber
                && available.IronOre >= IronOre
                && available.Jade >= Jade
                && available.MedicinalHerbs >= MedicinalHerbs
                && available.SpiritHerbs >= SpiritHerbs;
        }

        public static ResourceCost operator -(ResourceCost a, ResourceCost b)
        {
            return new ResourceCost
            {
                Tael = a.Tael - b.Tael,
                Qi = a.Qi - b.Qi,
                Lumber = a.Lumber - b.Lumber,
                IronOre = a.IronOre - b.IronOre,
                Jade = a.Jade - b.Jade,
                MedicinalHerbs = a.MedicinalHerbs - b.MedicinalHerbs,
                SpiritHerbs = a.SpiritHerbs - b.SpiritHerbs
            };
        }

        public static ResourceCost FromTael(int tael)
        {
            return new ResourceCost { Tael = tael };
        }

        public static ResourceCost FromTaelQi(int tael, int qi)
        {
            return new ResourceCost { Tael = tael, Qi = qi };
        }

        public static ResourceCost FromTaelQiLumber(int tael, int qi, int lumber)
        {
            return new ResourceCost { Tael = tael, Qi = qi, Lumber = lumber };
        }

        public static ResourceCost FromTaelQiLumberIron(int tael, int qi, int lumber, int iron)
        {
            return new ResourceCost { Tael = tael, Qi = qi, Lumber = lumber, IronOre = iron };
        }

        public static ResourceCost FromTaelQiLumberJade(int tael, int qi, int lumber, int jade)
        {
            return new ResourceCost { Tael = tael, Qi = qi, Lumber = lumber, Jade = jade };
        }

        public static ResourceCost FromTaelQiLumberMedHerbs(int tael, int qi, int lumber, int medHerbs)
        {
            return new ResourceCost { Tael = tael, Qi = qi, Lumber = lumber, MedicinalHerbs = medHerbs };
        }

        public static ResourceCost FromTaelQiLumberSpiritHerbs(int tael, int qi, int lumber, int spiritHerbs)
        {
            return new ResourceCost { Tael = tael, Qi = qi, Lumber = lumber, SpiritHerbs = spiritHerbs };
        }
    }

    /// <summary>
    /// Static configuration for one building type across all 3 tiers.
    /// Create via Assets > Create > TalesOfTao > Buildings > Building Config.
    /// Save into Assets/_Game/Data/Buildings/.
    /// </summary>
    [CreateAssetMenu(menuName = "TalesOfTao/Buildings/Building Config", fileName = "BuildingConfig_New")]
    public class BuildingConfigSO : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Unique ID for this building type (e.g. TrainingGrounds, Library)")]
        [SerializeField] private string _buildingTypeId = "NewBuilding";

        [SerializeField] private string _displayName = "New Building";

        [Header("Tier Costs (T1-T3)")]
        [Tooltip("Resource cost for each tier. Index 0 = T1.")]
        [SerializeField] private ResourceCost[] _tierCosts = new ResourceCost[3];

        [Header("Build Turns (T1-T3)")]
        [Tooltip("Number of Build phases to complete construction. Index 0 = T1.")]
        [SerializeField] private int[] _tierBuildTurns = new int[3];

        [Header("Prerequisites")]
        [Tooltip("Building type ID that must be built first. Empty = no prerequisite.")]
        [SerializeField] private string _prerequisiteBuilding = "";

        [Tooltip("Tier of prerequisite building required.")]
        [SerializeField] private int _prerequisiteTier = 0;

        [Header("Tier Effects")]
        [Tooltip("Short description of what each tier provides.")]
        [SerializeField] private string[] _tierEffects = new string[3];

        [Header("Meshes")]
        [Tooltip("Mesh for each tier. Assign .obj meshes from Art/Meshes/Buildings/.")]
        [SerializeField] private Mesh[] _tierMeshes = new Mesh[3];

        [Header("Material")]
        [Tooltip("Material for this building. Enable GPU Instancing in the Material Inspector.")]
        [SerializeField] private Material _buildingMaterial;

        [Header("Collider")]
        [Tooltip("True for interactive buildings (spawns Convex MeshCollider). False for decorative.")]
        [SerializeField] private bool _requiresCollider = true;

        // Public accessors
        public string BuildingTypeId => _buildingTypeId;
        public string DisplayName => _displayName;
        public ResourceCost GetTierCost(int tier) => tier >= 1 && tier <= 3 ? _tierCosts[tier - 1] : default;
        public int GetBuildTurns(int tier) => tier >= 1 && tier <= 3 ? _tierBuildTurns[tier - 1] : 0;
        public string PrerequisiteBuilding => _prerequisiteBuilding;
        public int PrerequisiteTier => _prerequisiteTier;
        public string GetTierEffect(int tier) => tier >= 1 && tier <= 3 ? _tierEffects[tier - 1] : "";
        public Mesh GetTierMesh(int tier) => tier >= 1 && tier <= 3 ? _tierMeshes[tier - 1] : null;
        public Material BuildingMaterial => _buildingMaterial;
        public bool RequiresCollider => _requiresCollider;
    }
}
