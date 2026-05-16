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
            
            string name = config.Name;
            
            foreach (var kvp in meshMap)
            {
                if (name.Contains(kvp.Key))
                {
                    var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(kvp.Value);
                    if (mesh != null)
                    {
                        // Use SerializedObject to set the mesh field
                        var so = new SerializedObject(config);
                        var meshProp = so.FindProperty("_mesh");
                        if (meshProp != null)
                        {
                            meshProp.objectReferenceValue = mesh;
                            so.ApplyModifiedProperties();
                            assigned++;
                            Debug.Log("[AssignUnitMeshes] " + name + ": assigned " + mesh.name);
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
