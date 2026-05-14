using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class M5AssetImporter : EditorWindow
{
    [MenuItem("Tales of Tao/M5/Import All Assets")]
    static void ImportAll()
    {
        string artRoot = "Assets/_Game/Art";
        string materialsPath = artRoot + "/Materials";
        string prefabsPath = artRoot + "/Prefabs";
        string buildingsPath = prefabsPath + "/Buildings";
        string unitsPath = prefabsPath + "/Units";
        string tilesPath = prefabsPath + "/HexTiles";
        string featuresPath = prefabsPath + "/Features";

        EnsureDirectory(artRoot + "/Materials");
        EnsureDirectory(prefabsPath);
        EnsureDirectory(buildingsPath);
        EnsureDirectory(unitsPath);
        EnsureDirectory(tilesPath);
        EnsureDirectory(featuresPath);

        // Create all materials
        CreateMaterial(materialsPath, "M_Plains", new Color(0.45f, 0.65f, 0.3f));
        CreateMaterial(materialsPath, "M_Mountain", new Color(0.55f, 0.5f, 0.45f));
        CreateMaterial(materialsPath, "M_Forest", new Color(0.25f, 0.5f, 0.2f));
        CreateMaterial(materialsPath, "M_River", new Color(0.3f, 0.55f, 0.75f));
        CreateMaterial(materialsPath, "M_Lake", new Color(0.25f, 0.45f, 0.7f));
        CreateMaterial(materialsPath, "M_Desert", new Color(0.85f, 0.75f, 0.5f));
        CreateMaterial(materialsPath, "M_Swamp", new Color(0.35f, 0.4f, 0.25f));
        CreateMaterial(materialsPath, "M_SacredPeak", new Color(0.7f, 0.6f, 0.85f));
        CreateMaterial(materialsPath, "M_BuildingWood", new Color(0.55f, 0.35f, 0.2f));
        CreateMaterial(materialsPath, "M_BuildingStone", new Color(0.6f, 0.58f, 0.55f));
        CreateMaterial(materialsPath, "M_BuildingRoof", new Color(0.4f, 0.15f, 0.1f));
        CreateMaterial(materialsPath, "M_DiscipleT1", new Color(0.7f, 0.7f, 0.65f));
        CreateMaterial(materialsPath, "M_DiscipleT2", new Color(0.6f, 0.65f, 0.8f));
        CreateMaterial(materialsPath, "M_DiscipleT3", new Color(0.8f, 0.7f, 0.3f));
        CreateMaterial(materialsPath, "M_DiscipleT4", new Color(0.9f, 0.5f, 0.2f));
        CreateMaterial(materialsPath, "M_DiscipleT5", new Color(0.95f, 0.85f, 0.4f));
        CreateMaterial(materialsPath, "M_Crystal", new Color(0.6f, 0.8f, 1.0f));
        CreateMaterial(materialsPath, "M_Water", new Color(0.3f, 0.5f, 0.8f));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Create prefabs
        CreateTilePrefabs(artRoot + "/Meshes/HexTiles", tilesPath, materialsPath);
        CreateBuildingPrefabs(artRoot + "/Meshes/Buildings", buildingsPath, materialsPath);
        CreateUnitPrefabs(artRoot + "/Meshes/Units", unitsPath, materialsPath);
        CreateFeaturePrefabs(artRoot + "/Meshes/HexTiles/Features", featuresPath, materialsPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("M5 Asset Import Complete!");
    }

    static void EnsureDirectory(string path)
    {
        if (string.IsNullOrEmpty(path) || path == "Assets") return;
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = Path.GetDirectoryName(path);
        string name = Path.GetFileName(path);
        if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(name)) return;
        EnsureDirectory(parent);
        AssetDatabase.CreateFolder(parent, name);
    }

    static void CreateMaterial(string path, string name, Color color)
    {
        string fullPath = path + "/" + name + ".mat";
        if (AssetDatabase.LoadAssetAtPath<Material>(fullPath) != null) return;

        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = color;
        mat.name = name;
        AssetDatabase.CreateAsset(mat, fullPath);
        Debug.Log("Created material: " + name);
    }

    static Material LoadMaterial(string materialsPath, string name)
    {
        return AssetDatabase.LoadAssetAtPath<Material>(materialsPath + "/" + name + ".mat");
    }

    static void CreateTilePrefabs(string meshPath, string prefabPath, string matPath)
    {
        string[] terrainTypes = { "Plains", "Mountain", "Forest", "River", "Lake", "Desert", "Swamp", "SacredPeak" };
        for (int i = 0; i < terrainTypes.Length; i++)
        {
            for (int lod = 0; lod < 3; lod++)
            {
                string objName = terrainTypes[i] + "_LOD" + lod;
                string objPath = meshPath + "/" + objName + ".obj";
                GameObject prefab = CreatePrefabFromMesh(objPath, "M_" + terrainTypes[i], matPath, prefabPath + "/" + objName + ".prefab");
            }
        }
    }

    static void CreateBuildingPrefabs(string meshPath, string prefabPath, string matPath)
    {
        string[] buildings = {
            "Building_Temple_T1", "Building_TrainingGrounds_T1", "Building_Library_T1",
            "Building_MedicineHall_T1", "Building_Armory_T1", "Building_DiscipleHall_T1",
            "Building_ElderCouncil_T1", "Building_ExternalAffairs_T1", "Building_MarketPavilion_T1",
            "Building_BranchSect_T1", "Building_DaoSanctum"
        };
        foreach (string b in buildings)
        {
            CreatePrefabFromMesh(meshPath + "/" + b + ".obj", "M_BuildingWood", matPath, prefabPath + "/" + b + ".prefab");
        }
    }

    static void CreateUnitPrefabs(string meshPath, string prefabPath, string matPath)
    {
        string[] units = { "T1_Labor_Disciple", "T2_Outer_Disciple", "T3_Core_Disciple", "T4_Sect_Master", "T5_Grand_Patriarch" };
        string[] mats = { "M_DiscipleT1", "M_DiscipleT2", "M_DiscipleT3", "M_DiscipleT4", "M_DiscipleT5" };
        for (int i = 0; i < units.Length; i++)
        {
            CreatePrefabFromMesh(meshPath + "/" + units[i] + ".obj", mats[i], matPath, prefabPath + "/" + units[i] + ".prefab");
        }
    }

    static void CreateFeaturePrefabs(string meshPath, string prefabPath, string matPath)
    {
        string[] features = { "Prop_Bamboo_Tree", "Prop_Pine_Tree", "Prop_Dead_Tree", "Prop_Rock_Mountain", "Prop_Rock_River", "Prop_Crystal_Qi", "Prop_Water_Plane" };
        string[] mats = { "M_Forest", "M_Forest", "M_Swamp", "M_Mountain", "M_River", "M_Crystal", "M_Water" };
        for (int i = 0; i < features.Length; i++)
        {
            CreatePrefabFromMesh(meshPath + "/" + features[i] + ".obj", mats[i], matPath, prefabPath + "/" + features[i] + ".prefab");
        }
    }

    static GameObject CreatePrefabFromMesh(string objPath, string materialName, string matPath, string prefabPath)
    {
        GameObject loadedObj = AssetDatabase.LoadAssetAtPath<GameObject>(objPath);
        if (loadedObj == null)
        {
            Debug.LogWarning("Could not load OBJ: " + objPath);
            return null;
        }

        GameObject instance = Object.Instantiate(loadedObj);
        instance.name = Path.GetFileNameWithoutExtension(prefabPath);

        Material mat = LoadMaterial(matPath, materialName);
        if (mat != null)
        {
            Renderer[] renderers = instance.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
            {
                r.sharedMaterial = mat;
            }
        }

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        Object.DestroyImmediate(instance);
        Debug.Log("Created prefab: " + prefabPath);
        return prefab;
    }
}
