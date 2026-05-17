using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class AssignUnitMeshes : EditorWindow
{
    [MenuItem("TalesOfTao/Assign Unit Meshes")]
    static void ShowWindow() => GetWindow<AssignUnitMeshes>("Assign Unit Meshes");
    
    private void OnGUI()
    {
        GUILayout.Label("Assign Meshes to Unit Configs", EditorStyles.boldLabel);
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
            {"T1_Labor", "Assets/_Game/Art/Meshes/Units/T1_Labor_Disciple.obj"},
            {"T2_Outer", "Assets/_Game/Art/Meshes/Units/T2_Outer_Disciple.obj"},
            {"T3_Core", "Assets/_Game/Art/Meshes/Units/T3_Core_Disciple.obj"},
            {"T4_Sect", "Assets/_Game/Art/Meshes/Units/T4_Sect_Master.obj"},
            {"T5_Grand", "Assets/_Game/Art/Meshes/Units/T5_Grand_Patriarch.obj"},
        };
        
        int assigned = 0;
        
        // Find all UnitDataSO assets
        var guids = AssetDatabase.FindAssets("t:UnitDataSO");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var config = AssetDatabase.LoadAssetAtPath<UnityEngine.ScriptableObject>(path);
            if (config == null) continue;
            
            string name = config.name;
            
            foreach (var kvp in meshMap)
            {
                if (name.Contains(kvp.Key))
                {
                    var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(kvp.Value);
                    if (mesh != null)
                    {
                        // UnitDataSO uses _tierMeshes array, not _mesh
                        var so = new SerializedObject(config);
                        var meshesProp = so.FindProperty("_tierMeshes");
                        if (meshesProp != null && meshesProp.isArray)
                        {
                            // Find the right tier index based on the key
                            int tierIdx = 0;
                            if (kvp.Key.Contains("T1")) tierIdx = 0;
                            else if (kvp.Key.Contains("T2")) tierIdx = 1;
                            else if (kvp.Key.Contains("T3")) tierIdx = 2;
                            else if (kvp.Key.Contains("T4")) tierIdx = 3;
                            else if (kvp.Key.Contains("T5")) tierIdx = 4;
                            
                            if (tierIdx < meshesProp.arraySize)
                            {
                                meshesProp.GetArrayElementAtIndex(tierIdx).objectReferenceValue = mesh;
                                so.ApplyModifiedProperties();
                                assigned++;
                                Debug.Log("[AssignUnitMeshes] " + name + ": assigned " + mesh.name + " to T" + (tierIdx+1));
                            }
                        }
                    }
                    break;
                }
            }
            
            EditorUtility.SetDirty(config);
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log("[AssignUnitMeshes] Done: " + assigned + " configs updated");
    }
}
