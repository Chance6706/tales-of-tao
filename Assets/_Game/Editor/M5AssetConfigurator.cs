using UnityEditor;
using UnityEngine;
using TalesOfTao.Sects;
using TalesOfTao.Hex;

namespace TalesOfTao.Editor
{
    /// <summary>
    /// Configures all M5 assets from GDD data.
    /// Run via: TalesOfTao > Configure M5 Assets
    /// </summary>
    public class M5AssetConfigurator : EditorWindow
    {
        [MenuItem("TalesOfTao/Configure M5 Assets")]
        static void ShowWindow()
        {
            GetWindow<M5AssetConfigurator>("M5 Asset Configurator");
        }

        private void OnGUI()
        {
            GUILayout.Label("M5 Asset Configuration", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("Configure All Assets", GUILayout.Height(40)))
            {
                ConfigureAll();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Configure Building Configs Only"))
            {
                ConfigureBuildingConfigs();
            }

            if (GUILayout.Button("Configure Sect Configs Only"))
            {
                ConfigureSectConfigs();
            }

            if (GUILayout.Button("Configure Terrain Types Only"))
            {
                ConfigureTerrainTypes();
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Delete All Generated Assets", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("Confirm Delete",
                    "Delete all generated M5 assets?", "Delete", "Cancel"))
                {
                    DeleteAllAssets();
                }
            }
        }

        static void ConfigureAll()
        {
            ConfigureTerrainTypes();
            ConfigureBuildingConfigs();
            ConfigureSectConfigs();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[M5AssetConfigurator] All assets configured successfully.");
        }

        #region Terrain Types

        static void ConfigureTerrainTypes()
        {
            string path = "Assets/_Game/Data/Terrain";
            EnsureDirectory(path);

            CreateTerrainTypeSO($"{path}/TerrainType_Plains.asset", "Plains", TerrainType.Plains,
                moveCost: 1f, defenseBonus: 0f, qiModifier: 1f, isImpassable: false,
                tintColor: new Color(0.55f, 0.76f, 0.29f));

            CreateTerrainTypeSO($"{path}/TerrainType_Forest.asset", "Forest", TerrainType.Forest,
                moveCost: 2f, defenseBonus: 0.25f, qiModifier: 1.1f, isImpassable: false,
                tintColor: new Color(0.13f, 0.55f, 0.13f));

            CreateTerrainTypeSO($"{path}/TerrainType_Mountain.asset", "Mountain", TerrainType.Mountain,
                moveCost: 3f, defenseBonus: 0.5f, qiModifier: 1.2f, isImpassable: false,
                tintColor: new Color(0.5f, 0.5f, 0.5f));

            CreateTerrainTypeSO($"{path}/TerrainType_River.asset", "River", TerrainType.River,
                moveCost: 2f, defenseBonus: 0f, qiModifier: 1.0f, isImpassable: false,
                tintColor: new Color(0.2f, 0.4f, 0.8f));

            CreateTerrainTypeSO($"{path}/TerrainType_Lake.asset", "Lake", TerrainType.Lake,
                moveCost: 999f, defenseBonus: 0f, qiModifier: 1.3f, isImpassable: true,
                tintColor: new Color(0.1f, 0.3f, 0.7f));

            CreateTerrainTypeSO($"{path}/TerrainType_Desert.asset", "Desert", TerrainType.Desert,
                moveCost: 2f, defenseBonus: 0f, qiModifier: 0.5f, isImpassable: false,
                tintColor: new Color(0.76f, 0.7f, 0.5f));

            CreateTerrainTypeSO($"{path}/TerrainType_Swamp.asset", "Swamp", TerrainType.Swamp,
                moveCost: 3f, defenseBonus: 0.1f, qiModifier: 0.8f, isImpassable: false,
                tintColor: new Color(0.3f, 0.4f, 0.2f));

            CreateTerrainTypeSO($"{path}/TerrainType_SacredPeak.asset", "Sacred Peak", TerrainType.SacredPeak,
                moveCost: 4f, defenseBonus: 0.6f, qiModifier: 2.0f, isImpassable: false,
                tintColor: new Color(0.8f, 0.7f, 0.3f));

            Debug.Log("[M5AssetConfigurator] Terrain types configured.");
        }

        static void CreateTerrainTypeSO(string path, string displayName, TerrainType type,
            float moveCost, float defenseBonus, float qiModifier, bool isImpassable, Color tintColor)
        {
            var so = AssetDatabase.LoadAssetAtPath<TerrainTypeSO>(path);
            if (so == null)
            {
                so = ScriptableObject.CreateInstance<TerrainTypeSO>();
                AssetDatabase.CreateAsset(so, path);
            }

            var serializedObject = new SerializedObject(so);
            serializedObject.FindProperty("_displayName").stringValue = displayName;
            serializedObject.FindProperty("_type").enumValueIndex = (int)type;
            serializedObject.FindProperty("_movementCost").floatValue = moveCost;
            serializedObject.FindProperty("_defenseBonus").floatValue = defenseBonus;
            serializedObject.FindProperty("_qiModifier").floatValue = qiModifier;
            serializedObject.FindProperty("_isImpassable").boolValue = isImpassable;
            serializedObject.FindProperty("_tintColor").colorValue = tintColor;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        #endregion

        #region Building Configs

        static void ConfigureBuildingConfigs()
        {
            string path = "Assets/_Game/Data/Buildings";
            EnsureDirectory(path);

            // Temple - no prerequisite (founded with sect)
            CreateBuildingConfig($"{path}/BC_Temple_T1.asset", "Temple", "Temple T1",
                prereq: "", prereqTier: 0,
                costs: new[] {
                    new ResourceCost { Tael = 0, Qi = 0 },
                    new ResourceCost { Tael = 0, Qi = 0 },
                    new ResourceCost { Tael = 0, Qi = 0 }
                },
                turns: new[] { 0, 0, 0 },
                effects: new[] { "Qi +10/turn; 1 build slot", "Qi +20/turn; 2 build slots; +5% income", "Qi +35/turn; 3 build slots; Dao Sanctum option" });

            // Training Grounds - requires Temple T1
            CreateBuildingConfig($"{path}/BC_TrainingGrounds_T1.asset", "TrainingGrounds", "Training Grounds T1",
                prereq: "Temple", prereqTier: 1,
                costs: new[] {
                    new ResourceCost { Tael = 50, Qi = 0, Lumber = 20 },
                    new ResourceCost { Tael = 120, Qi = 0, Lumber = 50, IronOre = 10 },
                    new ResourceCost { Tael = 250, Qi = 20, Lumber = 100, IronOre = 30 }
                },
                turns: new[] { 4, 6, 8 },
                effects: new[] { "8-turn training; cap 5/batch", "6-turn training; cap 8/batch; +5% combat", "5-turn training; cap 10/batch; free T1 technique" });

            // Disciple Hall - requires Training Grounds T1
            CreateBuildingConfig($"{path}/BC_DiscipleHall_T1.asset", "DiscipleHall", "Disciple Hall T1",
                prereq: "TrainingGrounds", prereqTier: 1,
                costs: new[] {
                    new ResourceCost { Tael = 80, Qi = 10, Lumber = 30 },
                    new ResourceCost { Tael = 180, Qi = 25, Lumber = 60, IronOre = 15 },
                    new ResourceCost { Tael = 350, Qi = 50, Lumber = 120, IronOre = 40 }
                },
                turns: new[] { 6, 10, 14 },
                effects: new[] { "15-turn promotion; 1 technique slot", "12-turn promotion; 2 technique slots", "10-turn promotion; Dual-Path technique" });

            // Library - requires Disciple Hall T1
            CreateBuildingConfig($"{path}/BC_Library_T1.asset", "Library", "Library T1",
                prereq: "DiscipleHall", prereqTier: 1,
                costs: new[] {
                    new ResourceCost { Tael = 100, Qi = 20, Lumber = 40 },
                    new ResourceCost { Tael = 200, Qi = 45, Lumber = 80 },
                    new ResourceCost { Tael = 400, Qi = 90, Lumber = 150, Jade = 5 }
                },
                turns: new[] { 8, 12, 18 },
                effects: new[] { "Research queue; 5 scrolls", "Research +30%; 15 scrolls", "Copy enemy scrolls; 30 scrolls" });

            // Elder Council - requires Library T2
            CreateBuildingConfig($"{path}/BC_ElderCouncil_T1.asset", "ElderCouncil", "Elder Council T1",
                prereq: "Library", prereqTier: 2,
                costs: new[] {
                    new ResourceCost { Tael = 150, Qi = 30, Lumber = 50, Jade = 2 },
                    new ResourceCost { Tael = 300, Qi = 60, Lumber = 100, Jade = 5 },
                    new ResourceCost { Tael = 600, Qi = 120, Lumber = 200, Jade = 10, SpiritHerbs = 5 }
                },
                turns: new[] { 10, 16, 24 },
                effects: new[] { "Elder promotion; 1 seat", "+1 seat; +2 Renown/turn", "High Elder promotion; Branch Sect governance" });

            // External Affairs Hall - requires Temple T1
            CreateBuildingConfig($"{path}/BC_ExternalAffairs_T1.asset", "ExternalAffairs", "External Affairs Hall T1",
                prereq: "Temple", prereqTier: 1,
                costs: new[] {
                    new ResourceCost { Tael = 60, Qi = 0, Lumber = 25 },
                    new ResourceCost { Tael = 140, Qi = 10, Lumber = 55 },
                    new ResourceCost { Tael = 300, Qi = 25, Lumber = 110, IronOre = 20 }
                },
                turns: new[] { 5, 8, 12 },
                effects: new[] { "Basic diplomacy; 1 spy", "+2 trade routes; spy resist +15%", "Alliance diplomacy; joint declarations" });

            // Medicine Hall - requires Training Grounds T1
            CreateBuildingConfig($"{path}/BC_MedicineHall_T1.asset", "MedicineHall", "Medicine Hall T1",
                prereq: "TrainingGrounds", prereqTier: 1,
                costs: new[] {
                    new ResourceCost { Tael = 70, Qi = 5, Lumber = 20, MedicinalHerbs = 10 },
                    new ResourceCost { Tael = 160, Qi = 15, Lumber = 45, MedicinalHerbs = 25 },
                    new ResourceCost { Tael = 350, Qi = 35, Lumber = 90, MedicinalHerbs = 50, SpiritHerbs = 5 }
                },
                turns: new[] { 5, 9, 14 },
                effects: new[] { "Herbal medicine; basic Alchemy", "Spirit Herb refining; advanced pills", "Breakthrough pills; Alchemy T4" });

            // Armory - requires Training Grounds T1
            CreateBuildingConfig($"{path}/BC_Armory_T1.asset", "Armory", "Armory T1",
                prereq: "TrainingGrounds", prereqTier: 1,
                costs: new[] {
                    new ResourceCost { Tael = 60, Qi = 0, Lumber = 15, IronOre = 15 },
                    new ResourceCost { Tael = 140, Qi = 0, Lumber = 35, IronOre = 40 },
                    new ResourceCost { Tael = 300, Qi = 10, Lumber = 70, IronOre = 80, Jade = 3 }
                },
                turns: new[] { 5, 9, 14 },
                effects: new[] { "Basic weapons; Forge research", "Refined weapons +8%; chainmail +8%", "Masterwork weapons; Sect Treasure Forging" });

            // Market Pavilion - requires External Affairs Hall T1
            CreateBuildingConfig($"{path}/BC_MarketPavilion_T1.asset", "MarketPavilion", "Market Pavilion T1",
                prereq: "ExternalAffairs", prereqTier: 1,
                costs: new[] {
                    new ResourceCost { Tael = 80, Qi = 0, Lumber = 30 },
                    new ResourceCost { Tael = 180, Qi = 5, Lumber = 65 },
                    new ResourceCost { Tael = 380, Qi = 15, Lumber = 130, IronOre = 15 }
                },
                turns: new[] { 6, 10, 15 },
                effects: new[] { "Base market; T1 markups", "+2 trade routes; T2 markups", "Trade monopoly option; no markups" });

            // Branch Sect Outpost - requires Elder Council T1
            CreateBuildingConfig($"{path}/BC_BranchSect_T1.asset", "BranchSect", "Branch Sect Outpost T1",
                prereq: "ElderCouncil", prereqTier: 1,
                costs: new[] {
                    new ResourceCost { Tael = 200, Qi = 10, Lumber = 60, IronOre = 10 },
                    new ResourceCost { Tael = 400, Qi = 25, Lumber = 120, IronOre = 25 },
                    new ResourceCost { Tael = 800, Qi = 50, Lumber = 250, IronOre = 50, Jade = 5 }
                },
                turns: new[] { 12, 18, 26 },
                effects: new[] { "Basic gathering; Peon housing", "Can build Training Grounds + Medicine Hall", "Can build all non-wonder buildings" });

            // Dao Sanctum - requires Temple T3 (wonder)
            CreateBuildingConfig($"{path}/BC_DaoSanctum_T1.asset", "DaoSanctum", "Dao Sanctum",
                prereq: "Temple", prereqTier: 3,
                costs: new[] {
                    new ResourceCost { Tael = 500, Qi = 200, Lumber = 100, IronOre = 50, Jade = 20, SpiritHerbs = 10 },
                    new ResourceCost(),
                    new ResourceCost()
                },
                turns: new[] { 20, 0, 0 },
                effects: new[] { "Wonder: Enlightenment victory path; +50% all Qi income", "", "" });

            Debug.Log("[M5AssetConfigurator] Building configs configured.");
        }

        static void CreateBuildingConfig(string path, string typeId, string displayName,
            string prereq, int prereqTier,
            ResourceCost[] costs, int[] turns, string[] effects)
        {
            var so = AssetDatabase.LoadAssetAtPath<BuildingConfigSO>(path);
            if (so == null)
            {
                so = ScriptableObject.CreateInstance<BuildingConfigSO>();
                AssetDatabase.CreateAsset(so, path);
            }

            var serializedObject = new SerializedObject(so);
            serializedObject.FindProperty("_buildingTypeId").stringValue = typeId;
            serializedObject.FindProperty("_displayName").stringValue = displayName;
            serializedObject.FindProperty("_prerequisiteBuilding").stringValue = prereq;
            serializedObject.FindProperty("_prerequisiteTier").intValue = prereqTier;
            serializedObject.FindProperty("_requiresCollider").boolValue = true;

            var tierCosts = serializedObject.FindProperty("_tierCosts");
            for (int i = 0; i < 3; i++)
            {
                if (i < costs.Length)
                {
                    var element = tierCosts.GetArrayElementAtIndex(i);
                    element.FindPropertyRelative("Tael").intValue = costs[i].Tael;
                    element.FindPropertyRelative("Qi").intValue = costs[i].Qi;
                    element.FindPropertyRelative("Lumber").intValue = costs[i].Lumber;
                    element.FindPropertyRelative("IronOre").intValue = costs[i].IronOre;
                    element.FindPropertyRelative("Jade").intValue = costs[i].Jade;
                    element.FindPropertyRelative("MedicinalHerbs").intValue = costs[i].MedicinalHerbs;
                    element.FindPropertyRelative("SpiritHerbs").intValue = costs[i].SpiritHerbs;
                }
            }

            var tierTurns = serializedObject.FindProperty("_tierBuildTurns");
            for (int i = 0; i < 3; i++)
            {
                if (i < turns.Length)
                    tierTurns.GetArrayElementAtIndex(i).intValue = turns[i];
            }

            var tierEffects = serializedObject.FindProperty("_tierEffects");
            for (int i = 0; i < 3; i++)
            {
                if (i < effects.Length)
                    tierEffects.GetArrayElementAtIndex(i).stringValue = effects[i];
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        #endregion

        #region Sect Configs

        static void ConfigureSectConfigs()
        {
            string path = "Assets/_Game/Data/Sects";
            EnsureDirectory(path);

            CreateSectConfig($"{path}/SC_WuDang.asset", "Wu Dang", SectAffinity.InternalQi, SectTrait.QiBonus,
                startTael: 100, startQi: 30, startPeons: 5,
                primaryColor: new Color(0.2f, 0.3f, 0.8f), secondaryColor: Color.white,
                replacesBuilding: "TrainingGrounds", uniqueHallBonus: "Taiji Pavilion: disciples train 25% faster; +10% defensive technique effectiveness");

            CreateSectConfig($"{path}/SC_Shaolin.asset", "Shaolin", SectAffinity.BodyCultivation, SectTrait.HpBonus,
                startTael: 100, startQi: 20, startPeons: 5,
                primaryColor: new Color(0.8f, 0.6f, 0.1f), secondaryColor: new Color(0.2f, 0.2f, 0.2f),
                replacesBuilding: "TrainingGrounds", uniqueHallBonus: "Pagoda of 108 Trials: +5% HP per cave trial, up to 3 stacks");

            CreateSectConfig($"{path}/SC_TangClan.asset", "Tang Clan", SectAffinity.Poison, SectTrait.PoisonImmunity,
                startTael: 120, startQi: 15, startPeons: 4,
                primaryColor: new Color(0.6f, 0.1f, 0.6f), secondaryColor: new Color(0.9f, 0.9f, 0.2f),
                replacesBuilding: "", uniqueHallBonus: "+20% assassination success; immune to poison");

            CreateSectConfig($"{path}/SC_MountHua.asset", "Mount Hua", SectAffinity.SwordArts, SectTrait.SwordDamage,
                startTael: 100, startQi: 20, startPeons: 5,
                primaryColor: new Color(0.9f, 0.9f, 0.9f), secondaryColor: new Color(0.8f, 0.1f, 0.1f),
                replacesBuilding: "Library", uniqueHallBonus: "Sword Peak: Martial research -30% Qi; +1 Sword technique slot per Inner Disciple");

            CreateSectConfig($"{path}/SC_Emei.asset", "Emei", SectAffinity.Balanced, SectTrait.RenownDiplomacy,
                startTael: 110, startQi: 20, startPeons: 5,
                primaryColor: new Color(0.9f, 0.5f, 0.7f), secondaryColor: Color.white,
                replacesBuilding: "ExternalAffairs", uniqueHallBonus: "Lotus Hall: settlement trust gain x1.5; Influence Emissaries (20 Tael, +10 trust)");

            CreateSectConfig($"{path}/SC_Kunlun.asset", "Kunlun", SectAffinity.IceElemental, SectTrait.MountainDefense,
                startTael: 100, startQi: 25, startPeons: 4,
                primaryColor: new Color(0.7f, 0.8f, 0.95f), secondaryColor: new Color(0.2f, 0.4f, 0.6f),
                replacesBuilding: "", uniqueHallBonus: "Frozen Meridian Chamber: all caves become Qi Refinement; +20% defense on Mountain/Sacred Peak");

            CreateSectConfig($"{path}/SC_PengClan.asset", "Peng Clan", SectAffinity.Movement, SectTrait.MovementBonus,
                startTael: 100, startQi: 20, startPeons: 5,
                primaryColor: new Color(0.3f, 0.7f, 0.4f), secondaryColor: new Color(0.9f, 0.9f, 0.5f),
                replacesBuilding: "", uniqueHallBonus: "+2 base movement for all disciples");

            CreateSectConfig($"{path}/SC_Namgung.asset", "Namgung", SectAffinity.NobleArts, SectTrait.TaelBonus,
                startTael: 150, startQi: 15, startPeons: 5,
                primaryColor: new Color(0.8f, 0.7f, 0.2f), secondaryColor: new Color(0.2f, 0.2f, 0.6f),
                replacesBuilding: "ElderCouncil", uniqueHallBonus: "Hall of Prestige: Elders generate +3 Renown/turn; specialization bonuses +25%");

            CreateSectConfig($"{path}/SC_DemonicCult.asset", "Demonic Cult", SectAffinity.Forbidden, SectTrait.Sacrifice,
                startTael: 80, startQi: 30, startPeons: 3,
                primaryColor: new Color(0.6f, 0.0f, 0.0f), secondaryColor: new Color(0.1f, 0.1f, 0.1f),
                replacesBuilding: "", uniqueHallBonus: "Blood Altar: sacrifice disciples for Qi; Forbidden Technique research branch");

            CreateSectConfig($"{path}/SC_ImperialPalace.asset", "Imperial Palace", SectAffinity.Imperial, SectTrait.ImperialTax,
                startTael: 200, startQi: 20, startPeons: 6,
                primaryColor: new Color(0.9f, 0.8f, 0.2f), secondaryColor: new Color(0.8f, 0.0f, 0.0f),
                replacesBuilding: "ElderCouncil", uniqueHallBonus: "Imperial Court: Edicts every 5 turns; levy Settlement Tax (15% trade income)");

            Debug.Log("[M5AssetConfigurator] Sect configs configured.");
        }

        static void CreateSectConfig(string path, string displayName, SectAffinity affinity, SectTrait trait,
            int startTael, int startQi, int startPeons,
            Color primaryColor, Color secondaryColor,
            string replacesBuilding, string uniqueHallBonus)
        {
            var so = AssetDatabase.LoadAssetAtPath<SectConfigSO>(path);
            if (so == null)
            {
                so = ScriptableObject.CreateInstance<SectConfigSO>();
                AssetDatabase.CreateAsset(so, path);
            }

            var serializedObject = new SerializedObject(so);
            serializedObject.FindProperty("_displayName").stringValue = displayName;
            serializedObject.FindProperty("_affinity").enumValueIndex = (int)affinity;
            serializedObject.FindProperty("_trait").enumValueIndex = (int)trait;
            serializedObject.FindProperty("_startingTael").intValue = startTael;
            serializedObject.FindProperty("_startingQi").intValue = startQi;
            serializedObject.FindProperty("_startingPeons").intValue = startPeons;
            serializedObject.FindProperty("_primaryColor").colorValue = primaryColor;
            serializedObject.FindProperty("_secondaryColor").colorValue = secondaryColor;
            serializedObject.FindProperty("_replacesBuilding").stringValue = replacesBuilding;
            serializedObject.FindProperty("_uniqueHallBonus").stringValue = uniqueHallBonus;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        #endregion

        #region Utilities

        static void EnsureDirectory(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = System.IO.Path.GetDirectoryName(path);
                string folder = System.IO.Path.GetFileName(path);
                AssetDatabase.CreateFolder(parent, folder);
            }
        }

        static void DeleteAllAssets()
        {
            string[] paths = {
                "Assets/_Game/Data/Terrain",
                "Assets/_Game/Data/Buildings",
                "Assets/_Game/Data/Sects"
            };

            foreach (var path in paths)
            {
                if (AssetDatabase.IsValidFolder(path))
                {
                    string[] guids = AssetDatabase.FindAssets("", new[] { path });
                    foreach (var guid in guids)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                        AssetDatabase.DeleteAsset(assetPath);
                    }
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[M5AssetConfigurator] All generated assets deleted.");
        }

        #endregion
    }
}
