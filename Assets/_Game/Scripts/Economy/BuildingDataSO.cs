using UnityEngine;

namespace TalesOfTao.Economy
{
    // Static configuration for one building type.
    // Create one asset per building: Assets > Create > TalesOfTao > Buildings > Building Data
    // Save into Assets/_Game/Data/Buildings/.
    //
    // Collider policy (per GDD):
    //   Interactive buildings (markets, shrines, etc.): MeshCollider Convex = true
    //   Decorative structures: no collider (set _requiresCollider = false)
    [CreateAssetMenu(menuName = "TalesOfTao/Buildings/Building Data", fileName = "BuildingData_New")]
    public class BuildingDataSO : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string _buildingName = "Building";

        [Header("Mesh")]
        [Tooltip("Assign .obj mesh from Assets/_Game/Art/Meshes/Buildings/.")]
        [SerializeField] private Mesh _mesh;

        [Header("Material")]
        [Tooltip("Enable GPU Instancing on this material for draw-call efficiency.")]
        [SerializeField] private Material _material;

        [Header("Collider")]
        [Tooltip("True for interactive buildings (spawns Convex MeshCollider). False for decorative.")]
        [SerializeField] private bool _requiresCollider = true;

        public string   BuildingName     => _buildingName;
        public Mesh     Mesh             => _mesh;
        public Material Material         => _material;
        public bool     RequiresCollider => _requiresCollider;
    }
}
