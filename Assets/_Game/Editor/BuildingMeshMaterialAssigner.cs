using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using TalesOfTao.Sects;

namespace TalesOfTao.Editor
{
    public class BuildingMeshMaterialAssigner
    {
        [MenuItem("TalesOfTao/Assign Building Meshes & Materials")]
        public static string AssignAllMeshesAndMaterials()
        {
            int meshes = AssignAllMeshes();
            int mats = AssignAllMaterials();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            string result = "[BuildingMeshMaterialAssigner] Complete. Meshes: " + meshes + ", Materials: " + mats;
            Debug.Log(result);
            return result;
        }

        static int AssignAllMeshes()
        {
            var meshMap = new Dictionary<string, string[]>
            {
                { "Temple", new[] { "Assets/_Game/Art/Meshes/Buildings/Building_Temple_T1.obj", "Assets/_Game/Art/Meshes/Buildings/Building_Temple_T1.obj", "Assets/_Game/Art/Meshes/Buildings/Building_Temple_T1.obj" }},
                { "TrainingGrounds", new[] { "Assets/_Game/Art/Meshes/Buildings/Building_TrainingGrounds_T1.obj", "Assets/_Game/Art/Meshes/Buildings/Building_TrainingGrounds_T1.obj", "Assets/_Game/Art/Meshes/Buildings/Building_TrainingGrounds_T1.obj" }},
                { "DiscipleHall", new[] { "Assets/_Game/Art/Meshes/Buildings/Building_DiscipleHall_T1.obj", "Assets/_Game/Art/Meshes/Buildings/Building_DiscipleHall_T1.obj", "Assets/_Game/Art/Meshes/Buildings/Building_DiscipleHall_T1.obj" }},
                { "Library", new[] { "Assets/_Game/Art/Meshes/Buildings/Building_Library_T1.obj", "Assets/_Game/Art/Meshes/Buildings/Building_Library_T1.obj", "Assets/_Game/Art/Meshes/Buildings/Building_Library_T1.obj" }},
                { "ElderCouncil", new[] { "Assets/_Game/Art/Meshes/Buildings/Building_ElderCouncil_T1.obj", "Assets/_Game/Art/Meshes/Buildings/Building_ElderCouncil_T1.obj", "Assets/_Game/Art/Meshes/Buildings/Building_ElderCouncil_T1.obj" }},
                { "ExternalAffairs", new[] { "Assets/_Game/Art/Meshes/Buildings/Building_ExternalAffairs_T1.obj", "Assets/_Game/Art/Meshes/Buildings/Building_ExternalAffairs_T1.obj", "Assets/_Game/Art/Meshes/Buildings/Building_ExternalAffairs_T1.obj" }},
                { "MedicineHall", new[] { "Assets/_Game/Art/Meshes/Buildings/Building_MedicineHall_T1.obj", "Assets/_Game/Art/Meshes/Buildings/Building_MedicineHall_T1.obj", "Assets/_Game/Art/Meshes/Buildings/Building_MedicineHall_T1.obj" }},
                { "Armory", new[] { "Assets/_Game/Art/Meshes/Buildings/Building_Armory_T1.obj", "Assets/_Game/Art/Meshes/Buildings/Building_Armory_T1.obj", "Assets/_Game/Art/Meshes/Buildings/Building_Armory_T1.obj" }},
                { "MarketPavilion", new[] { "Assets/_Game/Art/Meshes/Buildings/Building_MarketPavilion_T1.obj", "Assets/_Game/Art/Meshes/Buildings/Building_MarketPavilion_T1.obj", "Assets/_Game/Art/Meshes/Buildings/Building_MarketPavilion_T1.obj" }},
                { "BranchSect", new[] { "Assets/_Game/Art/Meshes/Buildings/Building_BranchSect_T1.obj", "Assets/_Game/Art/Meshes/Buildings/Building_BranchSect_T1.obj", "Assets/_Game/Art/Meshes/Buildings/Building_BranchSect_T1.obj" }},
                { "DaoSanctum", new[] { "Assets/_Game/Art/Meshes/Buildings/Building_DaoSanctum.obj", "Assets/_Game/Art/Meshes/Buildings/Building_DaoSanctum.obj", "Assets/_Game/Art/Meshes/Buildings/Building_DaoSanctum.obj" }},
            };

            string buildingsPath = "Assets/_Game/Data/Buildings";
            string[] guids = AssetDatabase.FindAssets("t:BuildingConfigSO", new[] { buildingsPath });
            int assignedCount = 0;

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var config = AssetDatabase.LoadAssetAtPath<BuildingConfigSO>(assetPath);
                if (config == null) continue;

                string typeId = config.BuildingTypeId;
                if (!meshMap.ContainsKey(typeId)) continue;

                var serializedObject = new SerializedObject(config);
                var tierMeshes = serializedObject.FindProperty("_tierMeshes");
                string[] meshPaths = meshMap[typeId];

                for (int i = 0; i < 3 && i < meshPaths.Length; i++)
                {
                    GameObject objAsset = AssetDatabase.LoadAssetAtPath<GameObject>(meshPaths[i]);
                    if (objAsset == null) continue;
                    MeshFilter mf = objAsset.GetComponent<MeshFilter>();
                    if (mf == null) mf = objAsset.GetComponentInChildren<MeshFilter>();
                    if (mf != null && mf.sharedMesh != null)
                        tierMeshes.GetArrayElementAtIndex(i).objectReferenceValue = mf.sharedMesh;
                }

                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                assignedCount++;
                Debug.Log("[BuildingMeshMaterialAssigner] Assigned meshes for " + typeId);
            }

            return assignedCount;
        }

        static int AssignAllMaterials()
        {
            var materialMap = new Dictionary<string, string>
            {
                { "Temple", "Assets/_Game/Art/Materials/M_BuildingWood.mat" },
                { "TrainingGrounds", "Assets/_Game/Art/Materials/M_BuildingStone.mat" },
                { "DiscipleHall", "Assets/_Game/Art/Materials/M_BuildingWood.mat" },
                { "Library", "Assets/_Game/Art/Materials/M_BuildingWood.mat" },
                { "ElderCouncil", "Assets/_Game/Art/Materials/M_BuildingStone.mat" },
                { "ExternalAffairs", "Assets/_Game/Art/Materials/M_BuildingWood.mat" },
                { "MedicineHall", "Assets/_Game/Art/Materials/M_BuildingWood.mat" },
                { "Armory", "Assets/_Game/Art/Materials/M_BuildingStone.mat" },
                { "MarketPavilion", "Assets/_Game/Art/Materials/M_BuildingWood.mat" },
                { "BranchSect", "Assets/_Game/Art/Materials/M_BuildingWood.mat" },
                { "DaoSanctum", "Assets/_Game/Art/Materials/M_Crystal.mat" },
            };

            string buildingsPath = "Assets/_Game/Data/Buildings";
            string[] guids = AssetDatabase.FindAssets("t:BuildingConfigSO", new[] { buildingsPath });
            int assignedCount = 0;

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var config = AssetDatabase.LoadAssetAtPath<BuildingConfigSO>(assetPath);
                if (config == null) continue;

                string typeId = config.BuildingTypeId;
                if (!materialMap.ContainsKey(typeId)) continue;

                var serializedObject = new SerializedObject(config);
                var buildingMaterial = serializedObject.FindProperty("_buildingMaterial");
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(materialMap[typeId]);
                if (mat != null)
                {
                    buildingMaterial.objectReferenceValue = mat;
                    serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    assignedCount++;
                    Debug.Log("[BuildingMeshMaterialAssigner] Assigned material " + mat.name + " for " + typeId);
                }
            }

            return assignedCount;
        }
    }
}