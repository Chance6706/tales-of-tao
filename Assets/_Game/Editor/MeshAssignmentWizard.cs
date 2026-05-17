using UnityEditor;
using UnityEngine;
using TalesOfTao.Hex;
using TalesOfTao.Economy;
using TalesOfTao.Units;

namespace TalesOfTao.Editor
{
    public static class MeshAssignmentWizard
    {
        private const string MeshDir = "Assets/_Game/Art/Meshes";
        private const string TerrainDir = "Assets/_Game/Data/Terrain";

        [MenuItem("TalesOfTao/4 - Assign Meshes to Terrain Types")]
        public static void AssignMeshes()
        {
            // Load all terrain type assets
            var terrainGuids = AssetDatabase.FindAssets("t:TerrainTypeSO", new[] { TerrainDir });
            int assigned = 0;

            foreach (var guid in terrainGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var terrainSO = AssetDatabase.LoadAssetAtPath<TerrainTypeSO>(path);
                if (terrainSO == null) continue;

                var ser = new SerializedObject(terrainSO);
                bool modified = false;

                // Assign base mesh (Hex_Base_Flat) to all terrain types that don't have one
                var baseMeshProp = ser.FindProperty("_baseMesh");
                if (baseMeshProp.objectReferenceValue == null)
                {
                    var baseMesh = AssetDatabase.LoadAssetAtPath<Mesh>($"{MeshDir}/HexTiles/Base/Hex_Base_Flat.obj");
                    if (baseMesh != null)
                    {
                        baseMeshProp.objectReferenceValue = baseMesh;
                        modified = true;
                    }
                }

                // Assign feature prefabs based on terrain type
                var featurePrefabProp = ser.FindProperty("_featurePrefab");
                if (featurePrefabProp.objectReferenceValue == null)
                {
                    string featureName = terrainSO.Type switch
                    {
                        TerrainType.Forest => "Feature_Forest_Tree",
                        TerrainType.Mountain => "Feature_Mountain",
                        TerrainType.SacredPeak => "Feature_Sect_Gate",
                        TerrainType.River => "Feature_Water_Stream",
                        TerrainType.Lake => "Feature_Water_Stream",
                        _ => null
                    };

                    if (featureName != null)
                    {
                        // Load the feature mesh and create a prefab
                        var featureMesh = AssetDatabase.LoadAssetAtPath<Mesh>($"{MeshDir}/HexTiles/Features/{featureName}.obj");
                        if (featureMesh != null)
                        {
                            // Create a prefab with the feature mesh
                            var featureGo = new GameObject(featureName);
                            var mf = featureGo.AddComponent<MeshFilter>();
                            mf.sharedMesh = featureMesh;
                            var mr = featureGo.AddComponent<MeshRenderer>();
                            // Use default material for now
                            mr.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");

                            string prefabPath = $"{MeshDir}/HexTiles/Features/{featureName}_prefab.prefab";
                            var prefab = PrefabUtility.SaveAsPrefabAsset(featureGo, prefabPath);
                            if (prefab != null)
                            {
                                featurePrefabProp.objectReferenceValue = prefab;
                                modified = true;
                            }
                            Object.DestroyImmediate(featureGo);
                        }
                    }
                }

                if (modified)
                {
                    ser.ApplyModifiedPropertiesWithoutUndo();
                    assigned++;
                    Debug.Log($"[TalesOfTao] Assigned meshes to {terrainSO.Type}");
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[TalesOfTao] Mesh assignment complete. {assigned} terrain types updated.");
        }

        [MenuItem("TalesOfTao/5 - Create Building Prefabs")]
        public static void CreateBuildingPrefabs()
        {
            var buildingGuids = AssetDatabase.FindAssets("t:BuildingDataSO", new[] { "Assets/_Game/Data/Buildings" });
            int created = 0;

            // Create building data assets if they don't exist
            EnsureDirectory("Assets/_Game/Data/Buildings");

            string[] buildingNames = { "Armory", "Library", "Medicine_Hall", "Temple", "Training_Grounds" };

            foreach (var name in buildingNames)
            {
                string dataPath = $"Assets/_Game/Data/Buildings/BuildingData_{name}.asset";
                var existingData = AssetDatabase.LoadAssetAtPath<BuildingDataSO>(dataPath);
                if (existingData != null) continue;

                var data = ScriptableObject.CreateInstance<BuildingDataSO>();
                AssetDatabase.CreateAsset(data, dataPath);
                var ser = new SerializedObject(data);
                ser.FindProperty("_displayName").stringValue = name.Replace("_", " ");

                // Assign mesh
                var meshPath = $"{MeshDir}/Buildings/Building_{name}.obj";
                var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
                if (mesh != null)
                {
                    ser.FindProperty("_mesh").objectReferenceValue = mesh;
                }

                ser.ApplyModifiedPropertiesWithoutUndo();
                created++;
                Debug.Log($"[TalesOfTao] Created BuildingDataSO for {name}");
            }

            // Create building prefabs
            EnsureDirectory("Assets/_Game/Prefabs/Buildings");
            foreach (var name in buildingNames)
            {
                string prefabPath = $"Assets/_Game/Prefabs/Buildings/Building_{name}.prefab";
                if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null) continue;

                var go = new GameObject($"Building_{name}");
                go.AddComponent<BuildingController>();

                var meshPath = $"{MeshDir}/Buildings/Building_{name}.obj";
                var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
                if (mesh != null)
                {
                    var mf = go.AddComponent<MeshFilter>();
                    mf.sharedMesh = mesh;
                    var mr = go.AddComponent<MeshRenderer>();
                    mr.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
                }

                PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                Object.DestroyImmediate(go);
                Debug.Log($"[TalesOfTao] Created building prefab: {name}");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[TalesOfTao] Created {created} building data assets and prefabs.");
        }

        [MenuItem("TalesOfTao/6 - Create Unit Prefabs")]
        public static void CreateUnitPrefabs()
        {
            EnsureDirectory("Assets/_Game/Data/Units");
            EnsureDirectory("Assets/_Game/Prefabs/Units");

            string[] unitNames = { "Labor_Disciple", "Outer_Disciple", "Core_Disciple", "Sect_Master", "Grand_Patriarch" };

            for (int i = 0; i < unitNames.Length; i++)
            {
                string tier = $"T{i + 1}";
                string name = unitNames[i];

                // Create UnitDataSO
                string dataPath = $"Assets/_Game/Data/Units/UnitData_{tier}_{name}.asset";
                if (AssetDatabase.LoadAssetAtPath<UnitDataSO>(dataPath) != null) continue;

                var data = ScriptableObject.CreateInstance<UnitDataSO>();
                AssetDatabase.CreateAsset(data, dataPath);
                var ser = new SerializedObject(data);
                ser.FindProperty("_unitName").stringValue = $"{tier} {name.Replace("_", " ")}";

                // Assign tier mesh
                var meshPath = $"{MeshDir}/Units/{tier}_{name}.obj";
                var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
                if (mesh != null)
                {
                    var tierMeshes = ser.FindProperty("_tierMeshes");
                    if (tierMeshes.isArray && tierMeshes.arraySize > 0)
                    {
                        tierMeshes.GetArrayElementAtIndex(i).objectReferenceValue = mesh;
                    }
                }

                ser.ApplyModifiedPropertiesWithoutUndo();

                // Create prefab
                string prefabPath = $"Assets/_Game/Prefabs/Units/Unit_{tier}_{name}.prefab";
                if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null) continue;

                var go = new GameObject($"Unit_{tier}_{name}");
                go.AddComponent<UnitController>();

                if (mesh != null)
                {
                    var mf = go.AddComponent<MeshFilter>();
                    mf.sharedMesh = mesh;
                    var mr = go.AddComponent<MeshRenderer>();
                    mr.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
                    go.AddComponent<CapsuleCollider>();
                }

                PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                Object.DestroyImmediate(go);
                Debug.Log($"[TalesOfTao] Created unit data + prefab: {tier} {name}");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[TalesOfTao] Unit data assets and prefabs created.");
        }

        private static void EnsureDirectory(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
