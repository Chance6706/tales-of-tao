using UnityEditor;
using UnityEngine;
using System.Text;

public class RunBuildingConfig : EditorWindow
{
    [MenuItem("TalesOfTao/Run Building Config")]
    static void ShowWindow() => GetWindow<RunBuildingConfig>("Run Building Config");
    
    private void OnGUI()
    {
        if (GUILayout.Button("Configure All Building Configs", GUILayout.Height(40)))
        {
            ConfigureAll();
        }
    }
    
    static void ConfigureAll()
    {
        var sb = new StringBuilder();
        int configured = 0;
        int errors = 0;
        
        // Temple (3 tiers) - no prerequisite
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_Temple_T1.asset", "Temple", "Temple T1", "", 0, 
            default, default, default, 0, 0, 0,
            "Qi +10/turn; 1 build slot", "Qi +20/turn; 2 build slots; +5% income", "Qi +35/turn; 3 build slots; Dao Sanctum option", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_Temple_T2.asset", "Temple", "Temple T2", "", 0,
            default, default, default, 0, 0, 0,
            "Qi +10/turn; 1 build slot", "Qi +20/turn; 2 build slots; +5% income", "Qi +35/turn; 3 build slots; Dao Sanctum option", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_Temple_T3.asset", "Temple", "Temple T3", "", 0,
            default, default, default, 0, 0, 0,
            "Qi +10/turn; 1 build slot", "Qi +20/turn; 2 build slots; +5% income", "Qi +35/turn; 3 build slots; Dao Sanctum option", ref configured, ref errors, sb);
        
        // Training Grounds (3 tiers) - requires Temple T1
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_TrainingGrounds_T1.asset", "TrainingGrounds", "Training Grounds T1", "Temple", 1,
            new ResourceCost(50, 0, 20, 0, 0, 0, 0), new ResourceCost(120, 0, 50, 10, 0, 0, 0), new ResourceCost(250, 20, 100, 30, 0, 0, 0),
            4, 6, 8, "8-turn training; cap 5/batch", "6-turn training; cap 8/batch; +5% combat", "5-turn training; cap 10/batch; free T1 technique", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_TrainingGrounds_T2.asset", "TrainingGrounds", "Training Grounds T2", "Temple", 1,
            new ResourceCost(50, 0, 20, 0, 0, 0, 0), new ResourceCost(120, 0, 50, 10, 0, 0, 0), new ResourceCost(250, 20, 100, 30, 0, 0, 0),
            4, 6, 8, "8-turn training; cap 5/batch", "6-turn training; cap 8/batch; +5% combat", "5-turn training; cap 10/batch; free T1 technique", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_TrainingGrounds_T3.asset", "TrainingGrounds", "Training Grounds T3", "Temple", 1,
            new ResourceCost(50, 0, 20, 0, 0, 0, 0), new ResourceCost(120, 0, 50, 10, 0, 0, 0), new ResourceCost(250, 20, 100, 30, 0, 0, 0),
            4, 6, 8, "8-turn training; cap 5/batch", "6-turn training; cap 8/batch; +5% combat", "5-turn training; cap 10/batch; free T1 technique", ref configured, ref errors, sb);
        
        // Disciple Hall (3 tiers) - requires Training Grounds T1
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_DiscipleHall_T1.asset", "DiscipleHall", "Disciple Hall T1", "TrainingGrounds", 1,
            new ResourceCost(80, 10, 30, 0, 0, 0, 0), new ResourceCost(180, 25, 60, 15, 0, 0, 0), new ResourceCost(350, 50, 120, 40, 0, 0, 0),
            6, 10, 14, "15-turn promotion; 1 technique slot", "12-turn promotion; 2 technique slots", "10-turn promotion; Dual-Path technique", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_DiscipleHall_T2.asset", "DiscipleHall", "Disciple Hall T2", "TrainingGrounds", 1,
            new ResourceCost(80, 10, 30, 0, 0, 0, 0), new ResourceCost(180, 25, 60, 15, 0, 0, 0), new ResourceCost(350, 50, 120, 40, 0, 0, 0),
            6, 10, 14, "15-turn promotion; 1 technique slot", "12-turn promotion; 2 technique slots", "10-turn promotion; Dual-Path technique", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_DiscipleHall_T3.asset", "DiscipleHall", "Disciple Hall T3", "TrainingGrounds", 1,
            new ResourceCost(80, 10, 30, 0, 0, 0, 0), new ResourceCost(180, 25, 60, 15, 0, 0, 0), new ResourceCost(350, 50, 120, 40, 0, 0, 0),
            6, 10, 14, "15-turn promotion; 1 technique slot", "12-turn promotion; 2 technique slots", "10-turn promotion; Dual-Path technique", ref configured, ref errors, sb);
        
        // Library (3 tiers) - requires Disciple Hall T1
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_Library_T1.asset", "Library", "Library T1", "DiscipleHall", 1,
            new ResourceCost(100, 20, 40, 0, 0, 0, 0), new ResourceCost(200, 45, 80, 0, 0, 0, 0), new ResourceCost(400, 90, 150, 0, 5, 0, 0),
            8, 12, 18, "Research queue; 5 scrolls", "Research +30%; 15 scrolls", "Copy enemy scrolls; 30 scrolls", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_Library_T2.asset", "Library", "Library T2", "DiscipleHall", 1,
            new ResourceCost(100, 20, 40, 0, 0, 0, 0), new ResourceCost(200, 45, 80, 0, 0, 0, 0), new ResourceCost(400, 90, 150, 0, 5, 0, 0),
            8, 12, 18, "Research queue; 5 scrolls", "Research +30%; 15 scrolls", "Copy enemy scrolls; 30 scrolls", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_Library_T3.asset", "Library", "Library T3", "DiscipleHall", 1,
            new ResourceCost(100, 20, 40, 0, 0, 0, 0), new ResourceCost(200, 45, 80, 0, 0, 0, 0), new ResourceCost(400, 90, 150, 0, 5, 0, 0),
            8, 12, 18, "Research queue; 5 scrolls", "Research +30%; 15 scrolls", "Copy enemy scrolls; 30 scrolls", ref configured, ref errors, sb);
        
        // Elder Council (3 tiers) - requires Library T2
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_ElderCouncil_T1.asset", "ElderCouncil", "Elder Council T1", "Library", 2,
            new ResourceCost(150, 30, 50, 0, 2, 0, 0), new ResourceCost(300, 60, 100, 0, 5, 0, 0), new ResourceCost(600, 120, 200, 0, 10, 0, 5),
            10, 16, 24, "Elder promotion; 1 seat", "+1 seat; +2 Renown/turn", "High Elder promotion; Branch Sect governance", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_ElderCouncil_T2.asset", "ElderCouncil", "Elder Council T2", "Library", 2,
            new ResourceCost(150, 30, 50, 0, 2, 0, 0), new ResourceCost(300, 60, 100, 0, 5, 0, 0), new ResourceCost(600, 120, 200, 0, 10, 0, 5),
            10, 16, 24, "Elder promotion; 1 seat", "+1 seat; +2 Renown/turn", "High Elder promotion; Branch Sect governance", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_ElderCouncil_T3.asset", "ElderCouncil", "Elder Council T3", "Library", 2,
            new ResourceCost(150, 30, 50, 0, 2, 0, 0), new ResourceCost(300, 60, 100, 0, 5, 0, 0), new ResourceCost(600, 120, 200, 0, 10, 0, 5),
            10, 16, 24, "Elder promotion; 1 seat", "+1 seat; +2 Renown/turn", "High Elder promotion; Branch Sect governance", ref configured, ref errors, sb);
        
        // External Affairs (3 tiers) - requires Temple T1
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_ExternalAffairs_T1.asset", "ExternalAffairs", "External Affairs Hall T1", "Temple", 1,
            new ResourceCost(60, 0, 25, 0, 0, 0, 0), new ResourceCost(140, 10, 55, 0, 0, 0, 0), new ResourceCost(300, 25, 110, 20, 0, 0, 0),
            5, 8, 12, "Basic diplomacy; 1 spy", "+2 trade routes; spy resist +15%", "Alliance diplomacy; joint declarations", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_ExternalAffairs_T2.asset", "ExternalAffairs", "External Affairs Hall T2", "Temple", 1,
            new ResourceCost(60, 0, 25, 0, 0, 0, 0), new ResourceCost(140, 10, 55, 0, 0, 0, 0), new ResourceCost(300, 25, 110, 20, 0, 0, 0),
            5, 8, 12, "Basic diplomacy; 1 spy", "+2 trade routes; spy resist +15%", "Alliance diplomacy; joint declarations", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_ExternalAffairs_T3.asset", "ExternalAffairs", "External Affairs Hall T3", "Temple", 1,
            new ResourceCost(60, 0, 25, 0, 0, 0, 0), new ResourceCost(140, 10, 55, 0, 0, 0, 0), new ResourceCost(300, 25, 110, 20, 0, 0, 0),
            5, 8, 12, "Basic diplomacy; 1 spy", "+2 trade routes; spy resist +15%", "Alliance diplomacy; joint declarations", ref configured, ref errors, sb);
        
        // Medicine Hall (3 tiers) - requires Training Grounds T1
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_MedicineHall_T1.asset", "MedicineHall", "Medicine Hall T1", "TrainingGrounds", 1,
            new ResourceCost(70, 5, 20, 0, 0, 10, 0), new ResourceCost(160, 15, 45, 0, 0, 25, 0), new ResourceCost(350, 35, 90, 0, 0, 50, 5),
            5, 9, 14, "Herbal medicine; basic Alchemy", "Spirit Herb refining; advanced pills", "Breakthrough pills; Alchemy T4", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_MedicineHall_T2.asset", "MedicineHall", "Medicine Hall T2", "TrainingGrounds", 1,
            new ResourceCost(70, 5, 20, 0, 0, 10, 0), new ResourceCost(160, 15, 45, 0, 0, 25, 0), new ResourceCost(350, 35, 90, 0, 0, 50, 5),
            5, 9, 14, "Herbal medicine; basic Alchemy", "Spirit Herb refining; advanced pills", "Breakthrough pills; Alchemy T4", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_MedicineHall_T3.asset", "MedicineHall", "Medicine Hall T3", "TrainingGrounds", 1,
            new ResourceCost(70, 5, 20, 0, 0, 10, 0), new ResourceCost(160, 15, 45, 0, 0, 25, 0), new ResourceCost(350, 35, 90, 0, 0, 50, 5),
            5, 9, 14, "Herbal medicine; basic Alchemy", "Spirit Herb refining; advanced pills", "Breakthrough pills; Alchemy T4", ref configured, ref errors, sb);
        
        // Armory (3 tiers) - requires Training Grounds T1
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_Armory_T1.asset", "Armory", "Armory T1", "TrainingGrounds", 1,
            new ResourceCost(60, 0, 15, 15, 0, 0, 0), new ResourceCost(140, 0, 35, 40, 0, 0, 0), new ResourceCost(300, 10, 70, 80, 3, 0, 0),
            5, 9, 14, "Basic weapons; Forge research", "Refined weapons +8%; chainmail +8%", "Masterwork weapons; Sect Treasure Forging", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_Armory_T2.asset", "Armory", "Armory T2", "TrainingGrounds", 1,
            new ResourceCost(60, 0, 15, 15, 0, 0, 0), new ResourceCost(140, 0, 35, 40, 0, 0, 0), new ResourceCost(300, 10, 70, 80, 3, 0, 0),
            5, 9, 14, "Basic weapons; Forge research", "Refined weapons +8%; chainmail +8%", "Masterwork weapons; Sect Treasure Forging", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_Armory_T3.asset", "Armory", "Armory T3", "TrainingGrounds", 1,
            new ResourceCost(60, 0, 15, 15, 0, 0, 0), new ResourceCost(140, 0, 35, 40, 0, 0, 0), new ResourceCost(300, 10, 70, 80, 3, 0, 0),
            5, 9, 14, "Basic weapons; Forge research", "Refined weapons +8%; chainmail +8%", "Masterwork weapons; Sect Treasure Forging", ref configured, ref errors, sb);
        
        // Market Pavilion (3 tiers) - requires External Affairs T1
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_MarketPavilion_T1.asset", "MarketPavilion", "Market Pavilion T1", "ExternalAffairs", 1,
            new ResourceCost(80, 0, 30, 0, 0, 0, 0), new ResourceCost(180, 5, 65, 0, 0, 0, 0), new ResourceCost(380, 15, 130, 15, 0, 0, 0),
            6, 10, 15, "Base market; T1 markups", "+2 trade routes; T2 markups", "Trade monopoly option; no markups", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_MarketPavilion_T2.asset", "MarketPavilion", "Market Pavilion T2", "ExternalAffairs", 1,
            new ResourceCost(80, 0, 30, 0, 0, 0, 0), new ResourceCost(180, 5, 65, 0, 0, 0, 0), new ResourceCost(380, 15, 130, 15, 0, 0, 0),
            6, 10, 15, "Base market; T1 markups", "+2 trade routes; T2 markups", "Trade monopoly option; no markups", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_MarketPavilion_T3.asset", "MarketPavilion", "Market Pavilion T3", "ExternalAffairs", 1,
            new ResourceCost(80, 0, 30, 0, 0, 0, 0), new ResourceCost(180, 5, 65, 0, 0, 0, 0), new ResourceCost(380, 15, 130, 15, 0, 0, 0),
            6, 10, 15, "Base market; T1 markups", "+2 trade routes; T2 markups", "Trade monopoly option; no markups", ref configured, ref errors, sb);
        
        // Branch Sect Outpost (3 tiers) - requires Elder Council T1
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_BranchSect_T1.asset", "BranchSect", "Branch Sect Outpost T1", "ElderCouncil", 1,
            new ResourceCost(200, 10, 60, 10, 0, 0, 0), new ResourceCost(400, 25, 120, 25, 0, 0, 0), new ResourceCost(800, 50, 250, 50, 5, 0, 0),
            12, 18, 26, "Basic gathering; Peon housing", "Can build Training Grounds + Medicine Hall", "Can build all non-wonder buildings", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_BranchSect_T2.asset", "BranchSect", "Branch Sect Outpost T2", "ElderCouncil", 1,
            new ResourceCost(200, 10, 60, 10, 0, 0, 0), new ResourceCost(400, 25, 120, 25, 0, 0, 0), new ResourceCost(800, 50, 250, 50, 5, 0, 0),
            12, 18, 26, "Basic gathering; Peon housing", "Can build Training Grounds + Medicine Hall", "Can build all non-wonder buildings", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_BranchSect_T3.asset", "BranchSect", "Branch Sect Outpost T3", "ElderCouncil", 1,
            new ResourceCost(200, 10, 60, 10, 0, 0, 0), new ResourceCost(400, 25, 120, 25, 0, 0, 0), new ResourceCost(800, 50, 250, 50, 5, 0, 0),
            12, 18, 26, "Basic gathering; Peon housing", "Can build Training Grounds + Medicine Hall", "Can build all non-wonder buildings", ref configured, ref errors, sb);
        
        // Dao Sanctum (3 tiers) - requires Temple T3 (wonder)
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_DaoSanctum_T1.asset", "DaoSanctum", "Dao Sanctum", "Temple", 3,
            new ResourceCost(500, 200, 100, 50, 20, 0, 10), default, default,
            20, 0, 0, "Wonder: Enlightenment victory path; +50% all Qi income", "", "", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_DaoSanctum_T2.asset", "DaoSanctum", "Dao Sanctum T2", "Temple", 3,
            new ResourceCost(500, 200, 100, 50, 20, 0, 10), default, default,
            20, 0, 0, "Wonder: Enlightenment victory path; +50% all Qi income", "", "", ref configured, ref errors, sb);
        ConfigureConfig("Assets/_Game/Data/Buildings/BC_DaoSanctum_T3.asset", "DaoSanctum", "Dao Sanctum T3", "Temple", 3,
            new ResourceCost(500, 200, 100, 50, 20, 0, 10), default, default,
            20, 0, 0, "Wonder: Enlightenment victory path; +50% all Qi income", "", "", ref configured, ref errors, sb);
        
        AssetDatabase.SaveAssets();
        sb.AppendLine("\n=== DONE: " + configured + " configured, " + errors + " errors ===");
        Debug.Log(sb.ToString());
    }
    
    static void ConfigureConfig(string path, string typeId, string displayName, string prereq, int prereqTier,
        ResourceCost t1Cost, ResourceCost t2Cost, ResourceCost t3Cost,
        int t1Turns, int t2Turns, int t3Turns,
        string t1Effect, string t2Effect, string t3Effect,
        ref int configured, ref int errors, StringBuilder sb)
    {
        var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.ScriptableObject>(path);
        if (obj == null)
        {
            sb.AppendLine("SKIP: " + path);
            errors++;
            return;
        }
        
        var so = new SerializedObject(obj);
        so.FindProperty("_buildingTypeId").stringValue = typeId;
        so.FindProperty("_displayName").stringValue = displayName;
        so.FindProperty("_prerequisiteBuilding").stringValue = prereq;
        so.FindProperty("_prerequisiteTier").intValue = prereqTier;
        so.FindProperty("_requiresCollider").boolValue = true;
        
        var tierCosts = so.FindProperty("_tierCosts");
        SetCost(tierCosts.GetArrayElementAtIndex(0), t1Cost);
        SetCost(tierCosts.GetArrayElementAtIndex(1), t2Cost);
        SetCost(tierCosts.GetArrayElementAtIndex(2), t3Cost);
        
        var tierTurns = so.FindProperty("_tierBuildTurns");
        tierTurns.GetArrayElementAtIndex(0).intValue = t1Turns;
        tierTurns.GetArrayElementAtIndex(1).intValue = t2Turns;
        tierTurns.GetArrayElementAtIndex(2).intValue = t3Turns;
        
        var tierEffects = so.FindProperty("_tierEffects");
        tierEffects.GetArrayElementAtIndex(0).stringValue = t1Effect;
        tierEffects.GetArrayElementAtIndex(1).stringValue = t2Effect;
        tierEffects.GetArrayElementAtIndex(2).stringValue = t3Effect;
        
        so.ApplyModifiedProperties();
        configured++;
        sb.AppendLine("OK: " + displayName);
    }
    
    static void SetCost(SerializedProperty elem, ResourceCost cost)
    {
        elem.FindPropertyRelative("Tael").intValue = cost.Tael;
        elem.FindPropertyRelative("Qi").intValue = cost.Qi;
        elem.FindPropertyRelative("Lumber").intValue = cost.Lumber;
        elem.FindPropertyRelative("IronOre").intValue = cost.IronOre;
        elem.FindPropertyRelative("Jade").intValue = cost.Jade;
        elem.FindPropertyRelative("MedicinalHerbs").intValue = cost.MedicinalHerbs;
        elem.FindPropertyRelative("SpiritHerbs").intValue = cost.SpiritHerbs;
    }
}

public struct ResourceCost
{
    public int Tael, Qi, Lumber, IronOre, Jade, MedicinalHerbs, SpiritHerbs;
    public ResourceCost(int tael, int qi, int lumber, int iron, int jade, int med, int spirit)
    {
        Tael = tael; Qi = qi; Lumber = lumber; IronOre = iron; Jade = jade; MedicinalHerbs = med; SpiritHerbs = spirit;
    }
}
