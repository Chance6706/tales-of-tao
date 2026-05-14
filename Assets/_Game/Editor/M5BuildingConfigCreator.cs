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

        // 11 building types x 3 tiers (DaoSanctum is single-tier wonder)
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
            config._buildingTypeId = typeId;
            config._displayName = displayName + " T" + tier;

            // Tier costs (index 0 = T1)
            config._tierCosts = new ResourceCost[3];
            config._tierBuildTurns = new int[3];
            config._tierEffects = new string[3];
            config._tierMeshes = new Mesh[3];

            float mult = tier;

            switch (typeId)
            {
                case "Temple":
                    config._tierCosts[tier - 1] = new ResourceCost { Tael = Mathf.RoundToInt(100 * mult) };
                    config._tierBuildTurns[tier - 1] = turns[tier - 1];
                    config._tierEffects[tier - 1] = "Qi generation: +" + (2 * tier);
                    break;
                case "TrainingGrounds":
                    config._tierCosts[tier - 1] = new ResourceCost { Tael = Mathf.RoundToInt(60 * mult) };
                    config._tierBuildTurns[tier - 1] = turns[tier - 1];
                    config._tierEffects[tier - 1] = "Training speed: +" + (5 * tier) + "%";
                    break;
                case "Library":
                    config._tierCosts[tier - 1] = new ResourceCost { Tael = Mathf.RoundToInt(80 * mult) };
                    config._tierBuildTurns[tier - 1] = turns[tier - 1];
                    config._tierEffects[tier - 1] = "Tech bonus: +" + (3 * tier);
                    break;
                case "MedicineHall":
                    config._tierCosts[tier - 1] = new ResourceCost { MedicinalHerbs = Mathf.RoundToInt(40 * mult) };
                    config._tierBuildTurns[tier - 1] = turns[tier - 1];
                    config._tierEffects[tier - 1] = "Healing: +" + (5 * tier) + "%";
                    break;
                case "Armory":
                    config._tierCosts[tier - 1] = new ResourceCost { IronOre = Mathf.RoundToInt(50 * mult) };
                    config._tierBuildTurns[tier - 1] = turns[tier - 1];
                    config._tierEffects[tier - 1] = "Combat: +" + (3 * tier);
                    break;
                case "DiscipleHall":
                    config._tierCosts[tier - 1] = new ResourceCost { Tael = Mathf.RoundToInt(40 * mult) };
                    config._tierBuildTurns[tier - 1] = turns[tier - 1];
                    config._tierEffects[tier - 1] = "Capacity: +" + (5 * tier) + " disciples";
                    break;
                case "ElderCouncil":
                    config._tierCosts[tier - 1] = new ResourceCost { Tael = Mathf.RoundToInt(150 * mult) };
                    config._tierBuildTurns[tier - 1] = turns[tier - 1];
                    config._tierEffects[tier - 1] = "Dissent: -" + (2 * tier);
                    break;
                case "ExternalAffairs":
                    config._tierCosts[tier - 1] = new ResourceCost { Tael = Mathf.RoundToInt(70 * mult) };
                    config._tierBuildTurns[tier - 1] = turns[tier - 1];
                    config._tierEffects[tier - 1] = "Reputation: +" + (3 * tier);
                    break;
                case "MarketPavilion":
                    config._tierCosts[tier - 1] = new ResourceCost { Tael = Mathf.RoundToInt(50 * mult) };
                    config._tierBuildTurns[tier - 1] = turns[tier - 1];
                    config._tierEffects[tier - 1] = "Tael income: +" + (5 * tier);
                    break;
                case "BranchSect":
                    config._tierCosts[tier - 1] = new ResourceCost { Tael = Mathf.RoundToInt(200 * mult) };
                    config._tierBuildTurns[tier - 1] = turns[tier - 1];
                    config._tierEffects[tier - 1] = "Influence: +" + (5 * tier);
                    break;
                case "DaoSanctum":
                    config._tierCosts[tier - 1] = new ResourceCost { Jade = 100 };
                    config._tierBuildTurns[tier - 1] = turns[tier - 1];
                    config._tierEffects[tier - 1] = "Wonder: +10 Qi, +5 Renown";
                    break;
            }

            config._requiresCollider = true;
            config._prerequisiteTier = (tier > 1) ? tier - 1 : 0;

            AssetDatabase.CreateAsset(config, fullPath);
            Debug.Log("Created: " + assetName);
        }
    }
}
