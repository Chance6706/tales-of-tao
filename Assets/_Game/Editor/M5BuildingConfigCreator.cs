using UnityEngine;
using UnityEditor;
using TalesOfTao.Sects;

public class M5BuildingConfigCreator
{
    [MenuItem("Tales of Tao/M5/Create Building Configs")]
    static void CreateAll()
    {
        string dataPath = "Assets/_Game/Data/Buildings";
        EnsureDirectory(dataPath);

        CreateConfig(dataPath, "Temple", "Temple", new int[] { 4, 6, 8 });
        CreateConfig(dataPath, "TrainingGrounds", "Training Grounds", new int[] { 3, 5, 7 });
        CreateConfig(dataPath, "Library", "Library", new int[] { 4, 6, 8 });
        CreateConfig(dataPath, "MedicineHall", "Medicine Hall", new int[] { 3, 5, 7 });
        CreateConfig(dataPath, "Armory", "Armory", new int[] { 4, 6, 8 });
        CreateConfig(dataPath, "DiscipleHall", "Disciple Hall", new int[] { 2, 3, 5 });
        CreateConfig(dataPath, "ElderCouncil", "Elder Council", new int[] { 6, 8, 12 });
        CreateConfig(dataPath, "ExternalAffairs", "External Affairs Hall", new int[] { 3, 5, 7 });
        CreateConfig(dataPath, "MarketPavilion", "Market Pavilion", new int[] { 2, 3, 5 });
        CreateConfig(dataPath, "BranchSect", "Branch Sect Outpost", new int[] { 8, 10, 14 });
        CreateConfig(dataPath, "DaoSanctum", "Dao Sanctum", new int[] { 20, 20, 20 });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("All BuildingConfigSO assets created!");
    }

    static void EnsureDirectory(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parent = Path.GetDirectoryName(path);
            string name = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, name);
        }
    }

    static void CreateConfig(string path, string typeId, string displayName, int[] turns)
    {
        for (int tier = 1; tier <= 3; tier++)
        {
            string assetName = "BC_" + typeId + "_T" + tier;
            string fullPath = path + "/" + assetName + ".asset";

            if (AssetDatabase.LoadAssetAtPath<BuildingConfigSO>(fullPath) != null)
            {
                Debug.Log("Already exists: " + assetName);
                continue;
            }

            BuildingConfigSO config = ScriptableObject.CreateInstance<BuildingConfigSO>();

            // Set identity via SerializedObject since _buildingTypeId is private
            SerializedObject so = new SerializedObject(config);
            so.FindProperty("_buildingTypeId").stringValue = typeId;
            so.FindProperty("_displayName").stringValue = displayName + " T" + tier;
            so.ApplyModifiedProperties();

            float mult = tier;
            ResourceCost cost = new ResourceCost();
            string effect = "";

            switch (typeId)
            {
                case "Temple":
                    cost.Tael = Mathf.RoundToInt(100 * mult);
                    effect = "Qi generation: +" + (2 * tier);
                    break;
                case "TrainingGrounds":
                    cost.Tael = Mathf.RoundToInt(60 * mult);
                    effect = "Training speed: +" + (5 * tier) + "%";
                    break;
                case "Library":
                    cost.Tael = Mathf.RoundToInt(80 * mult);
                    effect = "Tech bonus: +" + (3 * tier);
                    break;
                case "MedicineHall":
                    cost.MedicinalHerbs = Mathf.RoundToInt(40 * mult);
                    effect = "Healing: +" + (5 * tier) + "%";
                    break;
                case "Armory":
                    cost.IronOre = Mathf.RoundToInt(50 * mult);
                    effect = "Combat: +" + (3 * tier);
                    break;
                case "DiscipleHall":
                    cost.Tael = Mathf.RoundToInt(40 * mult);
                    effect = "Capacity: +" + (5 * tier) + " disciples";
                    break;
                case "ElderCouncil":
                    cost.Tael = Mathf.RoundToInt(150 * mult);
                    effect = "Dissent: -" + (2 * tier);
                    break;
                case "ExternalAffairs":
                    cost.Tael = Mathf.RoundToInt(70 * mult);
                    effect = "Reputation: +" + (3 * tier);
                    break;
                case "MarketPavilion":
                    cost.Tael = Mathf.RoundToInt(50 * mult);
                    effect = "Tael income: +" + (5 * tier);
                    break;
                case "BranchSect":
                    cost.Tael = Mathf.RoundToInt(200 * mult);
                    effect = "Influence: +" + (5 * tier);
                    break;
                case "DaoSanctum":
                    cost.Jade = 100;
                    effect = "Wonder: +10 Qi, +5 Renown";
                    break;
            }

            config.SetTierCost(tier, cost);
            config.SetBuildTurns(tier, turns[tier - 1]);
            config.SetTierEffect(tier, effect);
            config.SetRequiresCollider(true);

            if (tier > 1)
            {
                config.SetPrerequisite(typeId, tier - 1);
            }

            AssetDatabase.CreateAsset(config, fullPath);
            Debug.Log("Created: " + assetName);
        }
    }
}
