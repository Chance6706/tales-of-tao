using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class AssignMeshes : EditorWindow
{
    [MenuItem("TalesOfTao/Assign Meshes")]
    static void ShowWindow() => GetWindow<AssignMeshes>("Assign Meshes");
    
    private void OnGUI()
    {
        GUILayout.Label("Mesh Assignment", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("Assign Building Meshes", GUILayout.Height(30)))
            AssignBuildingMeshes();
        
        if (GUILayout.Button("Assign Unit Meshes", GUILayout.Height(30)))
            AssignUnitMeshes();
        
        if (GUILayout.Button("Create Missing Prefabs", GUILayout.Height(30)))
            CreateMissingPrefabs();
        
        if (GUILayout.Button("Fix All Materials", GUILayout.Height(30)))
            FixAllMaterials();
    }
    
    static void AssignBuildingMeshes()
    {
        var sb = new StringBuilder();
        int assigned = 0;
        
        var buildingMeshes = new Dictionary<string, string>
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
        
        var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] {"Assets/_Game/Art/Prefabs/Buildings"});
        foreach (var guid in prefabGuids)
        {
            var prefabPath = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null) continue;
            
            string buildingType = null;
            foreach (var kvp in buildingMeshes)
            {
                if (prefab.name.Contains(kvp.Key)) { buildingType = kvp.Key; break; }
            }
            if (buildingType == null) { sb.AppendLine("SKIP: " + prefab.name); continue; }
            
            var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(buildingMeshes[buildingType]);
            if (mesh == null) { sb.AppendLine("NO MESH: " + buildingType); continue; }
            
            var meshFilters = prefab.GetComponentsInChildren<MeshFilter>(true);
            foreach (var mf in meshFilters) mf.sharedMesh = mesh;
            
            var renderers = prefab.GetComponentsInChildren<MeshRenderer>(true);
            foreach (var r in renderers)
            {
                var woodMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Game/Art/Materials/M_BuildingWood.mat");
                var stoneMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Game/Art/Materials/M_BuildingStone.mat");
                var useStone = (buildingType == "Armory" || buildingType == "ElderCouncil");
                var mats = new Material[r.sharedMaterials.Length];
                for (int i = 0; i < mats.Length; i++) mats[i] = useStone ? stoneMat : woodMat;
                r.sharedMaterials = mats;
            }
            
            EditorUtility.SetDirty(prefab);
            assigned++;
            sb.AppendLine("OK: " + prefab.name);
        }
        
        AssetDatabase.SaveAssets();
        sb.AppendLine("\n=== DONE: " + assigned + " building prefabs ===");
        Debug.Log(sb.ToString());
    }
    
    static void AssignUnitMeshes()
    {
        var sb = new StringBuilder();
        int assigned = 0;
        
        var unitMeshes = new Dictionary<string, string>
        {
            {"T1_Labor", "Assets/_Game/Art/Meshes/Units/T1_Labor_Disciple.obj"},
            {"T2_Outer", "Assets/_Game/Art/Meshes/Units/T2_Outer_Disciple.obj"},
            {"T3_Core", "Assets/_Game/Art/Meshes/Units/T3_Core_Disciple.obj"},
            {"T4_Sect", "Assets/_Game/Art/Meshes/Units/T4_Sect_Master.obj"},
            {"T5_Grand", "Assets/_Game/Art/Meshes/Units/T5_Grand_Patriarch.obj"},
        };
        
        var unitMats = new Dictionary<string, string>
        {
            {"T1_Labor", "Assets/_Game/Art/Materials/M_DiscipleT1.mat"},
            {"T2_Outer", "Assets/_Game/Art/Materials/M_DiscipleT2.mat"},
            {"T3_Core", "Assets/_Game/Art/Materials/M_DiscipleT3.mat"},
            {"T4_Sect", "Assets/_Game/Art/Materials/M_DiscipleT4.mat"},
            {"T5_Grand", "Assets/_Game/Art/Materials/M_DiscipleT5.mat"},
        };
        
        var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] {"Assets/_Game/Art/Prefabs/Units"});
        foreach (var guid in prefabGuids)
        {
            var prefabPath = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null) continue;
            
            string unitType = null;
            foreach (var kvp in unitMeshes)
            {
                if (prefab.name.Contains(kvp.Key)) { unitType = kvp.Key; break; }
            }
            if (unitType == null) { sb.AppendLine("SKIP: " + prefab.name); continue; }
            
            var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(unitMeshes[unitType]);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(unitMats[unitType]);
            
            foreach (var mf in prefab.GetComponentsInChildren<MeshFilter>(true))
                if (mesh != null) mf.sharedMesh = mesh;
            
            foreach (var r in prefab.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (mat != null)
                {
                    var mats = new Material[r.sharedMaterials.Length];
                    for (int i = 0; i < mats.Length; i++) mats[i] = mat;
                    r.sharedMaterials = mats;
                }
            }
            
            EditorUtility.SetDirty(prefab);
            assigned++;
            sb.AppendLine("OK: " + prefab.name);
        }
        
        AssetDatabase.SaveAssets();
        sb.AppendLine("\n=== DONE: " + assigned + " unit prefabs ===");
        Debug.Log(sb.ToString());
    }
    
    static void CreateMissingPrefabs()
    {
        var sb = new StringBuilder();
        var oldPrefabs = new Dictionary<string, string>
        {
            {"Building_Armory.prefab", "Building_Armory_T1.obj"},
            {"Building_Library.prefab", "Building_Library_T1.obj"},
            {"Building_Medicine_Hall.prefab", "Building_MedicineHall_T1.obj"},
            {"Building_Temple.prefab", "Building_Temple_T1.obj"},
            {"Building_Training_Grounds.prefab", "Building_TrainingGrounds_T1.obj"},
        };
        
        foreach (var kvp in oldPrefabs)
        {
            var path = "Assets/_Game/Prefabs/Buildings/" + kvp.Key;
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) { sb.AppendLine("NOT FOUND: " + path); continue; }
            
            var meshPath = "Assets/_Game/Art/Meshes/Buildings/" + kvp.Value;
            var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
            
            foreach (var mf in prefab.GetComponentsInChildren<MeshFilter>(true))
            {
                if (mf.sharedMesh != null && mf.sharedMesh.name == "default" && mesh != null)
                    mf.sharedMesh = mesh;
            }
            
            EditorUtility.SetDirty(prefab);
            sb.AppendLine("OK: " + kvp.Key);
        }
        
        AssetDatabase.SaveAssets();
        sb.AppendLine("\n=== DONE ===");
        Debug.Log(sb.ToString());
    }
    
    static void FixAllMaterials()
    {
        int fixed_count = 0;
        var allPrefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] {"Assets/_Game"});
        foreach (var guid in allPrefabGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;
            
            foreach (var r in prefab.GetComponentsInChildren<Renderer>(true))
            {
                var mats = r.sharedMaterials;
                bool changed = false;
                for (int i = 0; i < mats.Length; i++)
                {
                    if (mats[i] != null && !mats[i].shader.name.Contains("Universal"))
                    {
                        var urpMat = AssetDatabase.LoadAssetAtPath<Material>(
                            "Assets/_Game/Art/Materials/" + mats[i].name + ".mat");
                        if (urpMat != null) { mats[i] = urpMat; changed = true; }
                    }
                }
                if (changed) { r.sharedMaterials = mats; EditorUtility.SetDirty(r); fixed_count++; }
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log("Fixed materials on " + fixed_count + " renderers");
    }
}
