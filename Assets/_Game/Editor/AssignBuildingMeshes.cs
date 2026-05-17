using UnityEditor;
using UnityEngine;
using TalesOfTao.Sects;
using System.Collections.Generic;

public class AssignBuildingMeshes : EditorWindow
{
    [MenuItem("TalesOfTao/Assign Building Meshes")]
    static void ShowWindow() => GetWindow<AssignBuildingMeshes>("Assign Building Meshes");
    
    private void OnGUI()
    {
        GUILayout.Label("Assign Meshes to Building Configs", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("Assign All Meshes", GUILayout.Height(40)))
        {
            AssignAllMeshes();
        }
    }
    
    static void AssignAllMeshes()
    {
        var meshMap = new Dictionary<string, string>
        {
            {"Armory", "Assets/_Game/Art/Meshes/Buildings/Building_Armory_T1.obj"},
            {"BranchSect", "Assets/_Game/Art/Meshes/Buildings/Building_BranchSect_T1.obj"},
            {"DaoSanctum", "Assets/_Game/Art/Meshes/Buildings/Building_DaoSanctum.obj"},
            {"DiscipleHall", "Assets/_Game/Art/Meshes/Buildings/Building_DiscipleHall_T1.obj"},
            {"ElderCouncil", "Assets/_Game/Art/Meshes/Buildings/Building_ElderCouncil_T1.obj"},
            {"ExternalAffairs", "Assets/_Game/Art/Meshes/Buildings/Building_ExternalAffairs_T1.obj"},
            {"Library", "Assets/_Game/Art/Meshes/Buildings/Building_Library_T1.obj"},
            {"MarketPavilion", "Assets/_Game/Art/Meshes/Buildings/Building_MarketPavilion_T1.obj"},
            {"MedicineHall", "Assets/_Game/Art/Meshes/Buildings/Building_MedicineHall_T1.obj"},
            {"Temple", "Assets/_Game/Art/Meshes/Buildings/Building_Temple_T1.obj"},
            {"TrainingGrounds", "Assets/_Game/Art/Meshes/Buildings/Building_TrainingGrounds_T1.obj"},
        };
        
        var matMap = new Dictionary<string, string>
        {
            {"Armory", "M_BuildingStone"},
            {"BranchSect", "M_BuildingWood"},
            {"DaoSanctum", "M_BuildingStone"},
            {"DiscipleHall", "M_BuildingWood"},
            {"ElderCouncil", "M_BuildingStone"},
            {"ExternalAffairs", "M_BuildingWood"},
            {"Library", "M_BuildingWood"},
            {"MarketPavilion", "M_BuildingWood"},
            {"MedicineHall", "M_BuildingWood"},
            {"Temple", "M_BuildingWood"},
            {"TrainingGrounds", "M_BuildingWood"},
        };
        
        int assigned = 0;
        int errors = 0;
        
        var guids = AssetDatabase.FindAssets("t:BuildingConfigSO");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var config = AssetDatabase.LoadAssetAtPath<BuildingConfigSO>(path);
            if (config == null) continue;
            
            string typeId = config.BuildingTypeId;
            
            if (meshMap.ContainsKey(typeId))
            {
                var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshMap[typeId]);
                if (mesh != null)
                {
                    for (int tier = 1; tier <= 3; tier++)
                    {
                        config.SetTierMesh(tier, mesh);
                    }
                    assigned++;
                    Debug.Log("[AssignMeshes] " + config.DisplayName + ": assigned " + mesh.name);
                }
                else
                {
                    Debug.LogWarning("[AssignMeshes] Mesh not found: " + meshMap[typeId]);
                    errors++;
                }
            }
            else
            {
                Debug.LogWarning("[AssignMeshes] No mesh mapping for: " + typeId);
                errors++;
            }
            
            if (matMap.ContainsKey(typeId))
            {
                var mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Game/Art/Materials/" + matMap[typeId] + ".mat");
                if (mat != null)
                {
                    config.SetBuildingMaterial(mat);
                }
            }
            
            EditorUtility.SetDirty(config);
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log("[AssignMeshes] Done: " + assigned + " configs updated, " + errors + " errors");
    }
}
