using UnityEditor;
using UnityEngine;
using System.Text;

public class CreatePresetMapsNow : EditorWindow
{
    [MenuItem("TalesOfTao/Create Preset Maps Now")]
    static void ShowWindow() => GetWindow<CreatePresetMapsNow>("Create Preset Maps");
    
    private void OnGUI()
    {
        GUILayout.Label("Create Preset Map Assets", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("Create All Three Maps", GUILayout.Height(40)))
        {
            CreateAll();
        }
    }
    
    static void CreateAll()
    {
        var sb = new StringBuilder();
        string mapsPath = "Assets/_Game/Data/Maps";
        
        if (!AssetDatabase.IsValidFolder(mapsPath))
            AssetDatabase.CreateFolder("Assets/_Game/Data", "Maps");
        
        // Create Jianghu map (Large: 120x80 = 9600 tiles)
        CreateMap(mapsPath, "PresetMap_Jianghu", "Jianghu", 
            "A geographical representation of mainland China. The Jianghu.", 
            2, 120, 80, sb);
        
        // Create Spirit Realm map (Medium: 100x60 = 6000 tiles)
        CreateMap(mapsPath, "PresetMap_SpiritRealm", "Spirit Realm",
            "A fantastical world of exaggerated terrain.", 
            1, 100, 60, sb);
        
        // Create Wasteland map (Medium: 80x60 = 4800 tiles)
        CreateMap(mapsPath, "PresetMap_Wasteland", "Wasteland",
            "A post-cataclysm world scarred by ancient battles.", 
            1, 80, 60, sb);
        
        AssetDatabase.SaveAssets();
        sb.AppendLine("\n=== ALL PRESET MAPS CREATED ===");
        Debug.Log(sb.ToString());
    }
    
    static void CreateMap(string path, string name, string displayName, string desc, 
        int mapSize, int width, int height, StringBuilder sb)
    {
        var map = ScriptableObject.CreateInstance<PresetMapData>();
        var so = new SerializedObject(map);
        
        so.FindProperty("MapName").stringValue = displayName;
        so.FindProperty("Description").stringValue = desc;
        so.FindProperty("MapSize").enumValueIndex = mapSize;
        
        int total = width * height;
        
        // Set terrain types - all Plains for now
        var terrainProp = so.FindProperty("TerrainTypes");
        terrainProp.arraySize = total;
        for (int i = 0; i < total; i++)
            terrainProp.GetArrayElementAtIndex(i).enumValueIndex = 0;
        
        // Set elevations - all Low
        var elevProp = so.FindProperty("Elevations");
        elevProp.arraySize = total;
        for (int i = 0; i < total; i++)
            elevProp.GetArrayElementAtIndex(i).enumValueIndex = 0;
        
        // Set Qi densities
        SerializedProperty qiProp = null;
        var iter = so.GetIterator();
        while (iter.NextVisible(true))
        {
            if (iter.name == "QiDensities") { qiProp = iter.Copy(); break; }
        }
        if (qiProp != null)
        {
            qiProp.arraySize = total;
            for (int i = 0; i < total; i++)
                qiProp.GetArrayElementAtIndex(i).enumValueIndex = 0;
        }
        
        // Clear complex arrays
        SetArraySize(so, "ResourceDeposits", 0);
        SetArraySize(so, "SacredPeaks", 0);
        SetArraySize(so, "Settlements", 0);
        SetArraySize(so, "StartingLocations", 0);
        
        so.ApplyModifiedProperties();
        
        string assetPath = path + "/" + name + ".asset";
        AssetDatabase.CreateAsset(map, assetPath);
        sb.AppendLine("Created: " + name + " (" + total + " tiles, " + width + "x" + height + ")");
    }
    
    static void SetArraySize(SerializedObject so, string propName, int size)
    {
        var iter = so.GetIterator();
        while (iter.NextVisible(true))
        {
            if (iter.name == propName)
            {
                iter.arraySize = size;
                return;
            }
        }
    }
}
