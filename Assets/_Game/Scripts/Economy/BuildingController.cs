using UnityEngine;

namespace TalesOfTao.Economy
{
    // Applies a BuildingDataSO to a GameObject: sets mesh, material, and
    // adds a Convex MeshCollider for interactive buildings.
    //
    // Setup:
    //   1. Add this component to a building GameObject.
    //   2. Assign BuildingDataSO in the Inspector, or call Initialize() from a spawner.
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class BuildingController : MonoBehaviour
    {
        [SerializeField] private BuildingDataSO _buildingData;

        private MeshFilter   _meshFilter;
        private MeshRenderer _meshRenderer;

        public BuildingDataSO Data => _buildingData;

        private void Awake()
        {
            _meshFilter   = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            if (_buildingData != null) Apply();
        }

        // Called by spawning system before the GameObject is activated.
        public void Initialize(BuildingDataSO data)
        {
            _buildingData = data;
            Apply();
        }

        private void Apply()
        {
            if (_buildingData.Mesh != null)
                _meshFilter.sharedMesh = _buildingData.Mesh;

            if (_buildingData.Material != null)
                _meshRenderer.sharedMaterial = _buildingData.Material;

            SetupCollider();
        }

        private void SetupCollider()
        {
            // Remove any existing collider added by a previous Apply call.
            var existing = GetComponent<MeshCollider>();
            if (existing != null)
                Destroy(existing);

            if (!_buildingData.RequiresCollider || _buildingData.Mesh == null)
                return;

            var mc         = gameObject.AddComponent<MeshCollider>();
            mc.sharedMesh  = _buildingData.Mesh;
            mc.convex      = true; // required for physics interactions and triggers
        }
    }
}
