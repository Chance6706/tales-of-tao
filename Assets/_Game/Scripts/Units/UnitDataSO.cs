using UnityEngine;

namespace TalesOfTao.Units
{
    // Per-unit-type configuration: one asset per unit archetype (e.g. Swordsman, Archer).
    // Holds all five tier meshes and the materials used for normal and Grand Patriarch states.
    //
    // Asset creation: Assets > Create > TalesOfTao > Units > Unit Data
    // Save into Assets/_Game/Data/Units/.
    //
    // Mesh slots:  index 0 = T1 (Novice) … index 4 = T5 (GrandPatriarch)
    // Assign .obj meshes from Assets/_Game/Art/Meshes/Units/ in the Inspector.
    //
    // Grand Patriarch material:
    //   Pre-configure this material with URP Lit + Emission enabled in the Material
    //   Inspector. UnitController swaps to it on promotion to T5. Keeping a separate
    //   pre-built material preserves GPU instancing (EnableKeyword on sharedMaterial
    //   would dirty the asset; doing it on .material creates a per-instance copy).
    [CreateAssetMenu(menuName = "TalesOfTao/Units/Unit Data", fileName = "UnitData_New")]
    public class UnitDataSO : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string _unitName = "Unit";

        [Header("Tier Meshes (T1–T5)")]
        [Tooltip("Element 0 = T1 Novice, element 4 = T5 Grand Patriarch. " +
                 "Assign .obj meshes from Assets/_Game/Art/Meshes/Units/.")]
        [SerializeField] private Mesh[] _tierMeshes = new Mesh[5];

        [Header("Materials")]
        [Tooltip("Base material used for T1–T4. Enable GPU Instancing in the Material Inspector.")]
        [SerializeField] private Material _baseMaterial;

        [Tooltip("Material used exclusively for T5. Pre-configure Emission in the Material Inspector.")]
        [SerializeField] private Material _grandPatriarchMaterial;

        [Header("Collider")]
        [Tooltip("Capsule collider height per tier (T1–T5). Used by UnitController.")]
        [SerializeField] private float[] _capsuleHeights = { 1.6f, 1.7f, 1.8f, 1.9f, 2.1f };

        [Tooltip("Capsule collider radius (same for all tiers).")]
        [SerializeField] private float _capsuleRadius = 0.35f;

        public string UnitName => _unitName;
        public float CapsuleRadius => _capsuleRadius;

        public Mesh GetMesh(SectTier tier)
        {
            int idx = Mathf.Clamp((int)tier - 1, 0, 4);
            return _tierMeshes != null && idx < _tierMeshes.Length ? _tierMeshes[idx] : null;
        }

        public Material GetMaterial(SectTier tier) =>
            tier == SectTier.GrandPatriarch ? _grandPatriarchMaterial : _baseMaterial;

        public float GetCapsuleHeight(SectTier tier)
        {
            int idx = Mathf.Clamp((int)tier - 1, 0, 4);
            return _capsuleHeights != null && idx < _capsuleHeights.Length ? _capsuleHeights[idx] : 1.8f;
        }
    }
}
