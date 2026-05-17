using UnityEditor;
using UnityEngine;
using TalesOfTao.Hex;
using System.Text;

public class CreatePresetMaps : EditorWindow
{
    [MenuItem("TalesOfTao/Create Preset Maps")]
    static void ShowWindow() => GetWindow<CreatePresetMaps>("Create Preset Maps");
    
    private void OnGUI()
    {
        if (GUILayout.Button("Create All Preset Maps", GUILayout.Height(40)))
            CreateAll();
    }
    
    static void CreateAll()
    {
        string mapsPath = "Assets/_Game/Data/Maps";
        if (!AssetDatabase.IsValidFolder(mapsPath))
            AssetDatabase.CreateFolder("Assets/_Game/Data", "Maps");
        
        CreateMap(mapsPath, "PresetMap_Jianghu", "Jianghu", "A geographical representation of mainland China", 2, 120, 80);
        CreateMap(mapsPath, "PresetMap_SpiritRealm", "Spirit Realm", "A fantastical world of exaggerated terrain", 1, 100, 60);
        CreateMap(mapsPath, "PresetMap_Wasteland", "Wasteland", "A post-cataclysm world scarred by ancient battles", 1, 80, 60);
        
        AssetDatabase.SaveAssets();
        Debug.Log("=== ALL PRESET MAPS CREATED ===");
    }
    
    static void CreateMap(string path, string name, string displayName, string desc, int mapSize, int width, int height)
    {
        var map = ScriptableObject.CreateInstance<PresetMapData>();
        var so = new SerializedObject(map);
        so.FindProperty("MapName").stringValue = displayName;
        so.FindProperty("Description").stringValue = desc;
        so.FindProperty("MapSize").enumValueIndex = mapSize;
        
        int total = width * height;
        
        // Initialize terrain types array
        var terrainArr = so.FindProperty("TerrainTypes");
        terrainArr.arraySize = total;
        for (int i = 0; i < total; i++)
            terrainArr.GetArrayElementAtIndex(i).enumValueIndex = 0; // Plains
        
        // Initialize elevations array
        var elevArr = so.FindProperty("Elevations");
        elevArr.arraySize = total;
        for (int i = 0; i < total; i++)
            elevArr.GetArrayElementAtIndex(i).enumValueIndex = 0; // Low
        
        // Initialize Qi densities array
        SerializedProperty qiArr = null;
        // Find the property
        var iter = so.GetIterator();
        while (iter.NextVisible(true))
        {
            if (iter.name == "QiDensities")
            {
                qiArr = iter.Copy();
                break;
            }
        }
        qiArr.arraySize = total;
        for (int i = 0; i < total; i++)
            qiArr.GetArrayElementAtIndex(i).enumValueIndex = 0; // None
        
        so.ApplyModifiedProperties();
        
        string assetPath = path + "/" + name + ".asset";
        AssetDatabase.CreateAsset(map, assetPath);
        Debug.Log("Created: " + assetPath + " (" + total + " tiles)");
    }
}
