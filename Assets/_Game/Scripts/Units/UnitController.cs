using UnityEngine;

namespace TalesOfTao.Units
{
    // Controls a unit's mesh, material, and collider based on its cultivation tier.
    //
    // Setup:
    //   1. Add this component to a unit GameObject.
    //   2. Assign a UnitDataSO in the Inspector.
    //   3. Call Initialize(data, tier) from the spawning system, or set _unitData
    //      in the Inspector and let Start() call SetTier(SectTier.Novice).
    //
    // Collider: CapsuleCollider — sized per tier for navigation/pathfinding accuracy.
    //   Height and radius come from UnitDataSO so designers can tune per archetype.
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class UnitController : MonoBehaviour
    {
        [SerializeField] private UnitDataSO _unitData;

        private MeshFilter      _meshFilter;
        private MeshRenderer    _meshRenderer;
        private CapsuleCollider _capsule;

        public SectTier  CurrentTier { get; private set; } = SectTier.Novice;
        public UnitDataSO Data       => _unitData;

        private void Awake()
        {
            _meshFilter   = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _capsule      = GetComponent<CapsuleCollider>();
        }

        private void Start()
        {
            if (_unitData != null)
                SetTier(SectTier.Novice);
        }

        // Called by the spawning system before the unit is activated.
        public void Initialize(UnitDataSO data, SectTier startingTier = SectTier.Novice)
        {
            _unitData = data;
            SetTier(startingTier);
        }

        // Swaps the mesh, material, and collider size for the given tier.
        // Promotes to GrandPatriarch automatically triggers the emission material.
        public void SetTier(SectTier tier)
        {
            if (_unitData == null)
            {
                Debug.LogWarning($"[UnitController] No UnitDataSO assigned on '{name}'.");
                return;
            }

            CurrentTier = tier;

            var mesh = _unitData.GetMesh(tier);
            if (mesh != null)
                _meshFilter.sharedMesh = mesh;

            _meshRenderer.sharedMaterial = _unitData.GetMaterial(tier);

            // Resize capsule for navigation/pathfinding — designers tune values in UnitDataSO.
            _capsule.height = _unitData.GetCapsuleHeight(tier);
            _capsule.radius = _unitData.CapsuleRadius;
            _capsule.center = new Vector3(0f, _capsule.height * 0.5f, 0f);
        }

        // Advances to the next tier. Returns false if already at GrandPatriarch.
        public bool TryPromote()
        {
            if (CurrentTier == SectTier.GrandPatriarch) return false;
            SetTier((SectTier)((int)CurrentTier + 1));
            return true;
        }
    }
}
