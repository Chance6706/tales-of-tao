using UnityEngine;

namespace TalesOfTao.Sects
{
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
        [Tooltip("Mesh for each tier. Assign meshes from Art/Meshes/Buildings/.")]
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

        // Editor setters (called by M5BuildingConfigCreator)
        public void SetTierCost(int tier, ResourceCost cost) { if (tier >= 1 && tier <= 3) _tierCosts[tier - 1] = cost; }
        public void SetBuildTurns(int tier, int turns) { if (tier >= 1 && tier <= 3) _tierBuildTurns[tier - 1] = turns; }
        public void SetTierEffect(int tier, string effect) { if (tier >= 1 && tier <= 3) _tierEffects[tier - 1] = effect; }
        public void SetTierMesh(int tier, Mesh mesh) { if (tier >= 1 && tier <= 3) _tierMeshes[tier - 1] = mesh; }
        public void SetBuildingMaterial(Material mat) { _buildingMaterial = mat; }
        public void SetRequiresCollider(bool val) { _requiresCollider = val; }
        public void SetPrerequisite(string building, int tier) { _prerequisiteBuilding = building; _prerequisiteTier = tier; }
    }
}
