using UnityEngine;

namespace TalesOfTao.Sects
{
    /// <summary>
    /// Creates building GameObjects from BuildingConfigSO data.
    /// Handles mesh assignment, material setup, collider creation, and positioning.
    /// </summary>
    public class BuildingFactory : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _buildingParent; // Parent transform for all buildings

        /// <summary>
        /// Creates a building GameObject from config and places it at the given position.
        /// Returns the created GameObject, or null if creation failed.
        /// </summary>
        public GameObject CreateBuilding(BuildingConfigSO config, int tier, Vector3 position)
        {
            if (config == null)
            {
                Debug.LogError("[BuildingFactory] Config is null.");
                return null;
            }

            if (tier < 1 || tier > 3)
            {
                Debug.LogError($"[BuildingFactory] Invalid tier {tier}.");
                return null;
            }

            string objectName = $"Building_{config.BuildingTypeId}_T{tier}";
            var go = new GameObject(objectName);

            // Set parent
            if (_buildingParent != null)
            {
                go.transform.SetParent(_buildingParent, false);
            }

            go.transform.position = position;

            // Add MeshFilter + MeshRenderer
            var meshFilter = go.AddComponent<MeshFilter>();
            var meshRenderer = go.AddComponent<MeshRenderer>();

            // Assign mesh
            Mesh mesh = config.GetTierMesh(tier);
            if (mesh != null)
            {
                meshFilter.sharedMesh = mesh;
            }
            else
            {
                Debug.LogWarning($"[BuildingFactory] No mesh for {config.BuildingTypeId} T{tier}. Using placeholder cube.");
                meshFilter.sharedMesh = CreatePlaceholderMesh();
            }

            // Assign material
            if (config.BuildingMaterial != null)
            {
                meshRenderer.sharedMaterial = config.BuildingMaterial;
            }

            // Add collider if required
            if (config.RequiresCollider && mesh != null)
            {
                var collider = go.AddComponent<MeshCollider>();
                collider.sharedMesh = mesh;
                collider.convex = true;
            }

            Debug.Log($"[BuildingFactory] Created {objectName} at {position}.");
            return go;
        }

        /// <summary>
        /// Validates that a building can be constructed (prerequisites met).
        /// </summary>
        public bool CanBuild(BuildingConfigSO config, int tier, SectData sect)
        {
            if (config == null || sect == null) return false;
            if (tier < 1 || tier > 3) return false;

            // Check prerequisites
            string prereq = config.PrerequisiteBuilding;
            if (!string.IsNullOrEmpty(prereq))
            {
                int prereqTier = config.PrerequisiteTier;
                if (!sect.HasBuilding(prereq, prereqTier))
                {
                    Debug.Log($"[BuildingFactory] Prerequisite not met: {prereq} T{prereqTier}.");
                    return false;
                }
            }

            // Check if already built
            if (sect.HasBuilding(config.BuildingTypeId, tier))
            {
                Debug.Log($"[BuildingFactory] Already built: {config.BuildingTypeId} T{tier}.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the build time in turns for a building tier.
        /// </summary>
        public int GetBuildTurns(BuildingConfigSO config, int tier)
        {
            if (config == null || tier < 1 || tier > 3) return 0;
            return config.GetBuildTurns(tier);
        }

        /// <summary>
        /// Gets the resource cost for a building tier.
        /// </summary>
        public ResourceCost GetBuildCost(BuildingConfigSO config, int tier)
        {
            if (config == null || tier < 1 || tier > 3) return default;
            return config.GetTierCost(tier);
        }

        /// <summary>
        /// Creates a placeholder cube mesh when no mesh is assigned.
        /// </summary>
        private Mesh CreatePlaceholderMesh()
        {
            var mesh = new Mesh { name = "Placeholder_Cube" };

            Vector3[] vertices = {
                new(-0.5f, 0f, -0.5f), new(0.5f, 0f, -0.5f), new(0.5f, 0f, 0.5f), new(-0.5f, 0f, 0.5f),
                new(-0.5f, 1f, -0.5f), new(0.5f, 1f, -0.5f), new(0.5f, 1f, 0.5f), new(-0.5f, 1f, 0.5f)
            };

            int[] triangles = {
                0,2,1, 0,3,2, // bottom
                4,5,6, 4,6,7, // top
                0,1,5, 0,5,4, // front
                2,3,7, 2,7,6, // back
                0,4,7, 0,7,3, // left
                1,2,6, 1,6,5  // right
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}
