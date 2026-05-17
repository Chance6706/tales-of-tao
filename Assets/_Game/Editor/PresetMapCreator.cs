using UnityEditor;
using UnityEngine;
using TalesOfTao.Hex;
using TalesOfTao.Sects;

namespace TalesOfTao.Editor
{
    /// <summary>
    /// Creates preset map data assets for the three launch maps:
    /// 1. Jianghu (Mainland China geographical representation)
    /// 2. Spirit Realm (Fantasy exaggerated terrain)
    /// 3. Wasteland (Post-cataclysm sparse map)
    /// </summary>
    public class PresetMapCreator : EditorWindow
    {
        [MenuItem("TalesOfTao/Create Preset Maps")]
        static void ShowWindow()
        {
            GetWindow<PresetMapCreator>("Preset Map Creator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Preset Map Creator", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("Create All Preset Maps", GUILayout.Height(40)))
            {
                CreateAllMaps();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Create Jianghu Map Only"))
            {
                CreateJianghuMap();
            }

            if (GUILayout.Button("Create Spirit Realm Map Only"))
            {
                CreateSpiritRealmMap();
            }

            if (GUILayout.Button("Create Wasteland Map Only"))
            {
                CreateWastelandMap();
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Delete All Preset Maps", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("Confirm Delete",
                    "Delete all preset map assets?", "Delete", "Cancel"))
                {
                    DeleteAllMaps();
                }
            }
        }

        static void CreateAllMaps()
        {
            CreateJianghuMap();
            CreateSpiritRealmMap();
            CreateWastelandMap();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[PresetMapCreator] All preset maps created successfully.");
        }

        #region Jianghu Map (Mainland China)

        static void CreateJianghuMap()
        {
            string path = "Assets/_Game/Data/Maps/PresetMap_Jianghu.asset";
            EnsureDirectory("Assets/_Game/Data/Maps");

            var map = AssetDatabase.LoadAssetAtPath<PresetMapData>(path);
            if (map == null)
            {
                map = ScriptableObject.CreateInstance<PresetMapData>();
                AssetDatabase.CreateAsset(map, path);
            }

            int width = 120; // Large map
            int height = 80;
            int totalTiles = width * height;

            map.MapName = "Jianghu";
            map.Description = "A geographical representation of mainland China. The Jianghu — the world of martial artists, wandering heroes, and ancient sects. From the fertile Central Plains to the towering Himalayas, from the Gobi Desert to the tropical south.";
            map.MapSize = MapSize.Large;

            // Initialize arrays
            map.TerrainTypes = new TerrainType[totalTiles];
            map.Elevations = new ElevationLevel[totalTiles];
            map.QiDensities = new QiDensityLevel[totalTiles];

            // Fill with default Plains/Low/Sparse
            for (int i = 0; i < totalTiles; i++)
            {
                map.TerrainTypes[i] = TerrainType.Plains;
                map.Elevations[i] = ElevationLevel.Low;
                map.QiDensities[i] = QiDensityLevel.Sparse;
            }

            // Generate China-like geography
            // Coordinate system: q = longitude (west to east), r = latitude (north to south)
            // Map covers roughly: q=0 (west/Tibet) to q=119 (east/Shanghai)
            //                    r=0 (north/Manchuria) to r=79 (south/Hainan)

            for (int r = 0; r < height; r++)
            {
                for (int q = 0; q < width; q++)
                {
                    int idx = r * width + q;
                    float x = q / (float)width;  // 0 = west, 1 = east
                    float y = r / (float)height; // 0 = north, 1 = south

                    // === ELEVATION ===
                    // Himalayas/Tibet plateau (far west, high elevation)
                    if (x < 0.2f)
                    {
                        float distFromWest = x / 0.2f;
                        map.Elevations[idx] = distFromWest < 0.3f ? ElevationLevel.Summit :
                                              distFromWest < 0.6f ? ElevationLevel.High :
                                              ElevationLevel.Medium;
                    }
                    // Central Plains (middle, low elevation)
                    else if (x >= 0.3f && x < 0.7f && y >= 0.3f && y < 0.7f)
                    {
                        map.Elevations[idx] = ElevationLevel.Low;
                    }
                    // Eastern coastal plains
                    else if (x >= 0.7f)
                    {
                        map.Elevations[idx] = ElevationLevel.Low;
                    }
                    // Northern mountains (Manchuria)
                    else if (y < 0.2f && x >= 0.5f)
                    {
                        map.Elevations[idx] = ElevationLevel.High;
                    }
                    // Southern hills
                    else if (y >= 0.7f)
                    {
                        map.Elevations[idx] = ElevationLevel.Medium;
                    }
                    // Transition zones
                    else
                    {
                        map.Elevations[idx] = ElevationLevel.Medium;
                    }

                    // === TERRAIN ===
                    // Gobi Desert (northwest)
                    if (x < 0.3f && y < 0.3f)
                    {
                        map.TerrainTypes[idx] = TerrainType.Desert;
                    }
                    // Tibetan Plateau (west, high altitude)
                    else if (x < 0.2f)
                    {
                        map.TerrainTypes[idx] = map.Elevations[idx] == ElevationLevel.Summit ? TerrainType.SacredPeak :
                                               map.Elevations[idx] == ElevationLevel.High ? TerrainType.Mountain :
                                               TerrainType.Plains;
                    }
                    // Central Plains (fertile heartland)
                    else if (x >= 0.3f && x < 0.6f && y >= 0.35f && y < 0.65f)
                    {
                        // Add some forests and rivers
                        float noise = Mathf.PerlinNoise(q * 0.1f, r * 0.1f);
                        if (noise > 0.7f)
                            map.TerrainTypes[idx] = TerrainType.Forest;
                        else if (noise < 0.15f)
                            map.TerrainTypes[idx] = TerrainType.River;
                        else
                            map.TerrainTypes[idx] = TerrainType.Plains;
                    }
                    // Yangtze River region (central-east)
                    else if (x >= 0.5f && x < 0.75f && y >= 0.4f && y < 0.6f)
                    {
                        float riverNoise = Mathf.PerlinNoise(q * 0.15f + 50, r * 0.15f + 50);
                        if (riverNoise > 0.6f)
                            map.TerrainTypes[idx] = TerrainType.River;
                        else if (riverNoise > 0.4f)
                            map.TerrainTypes[idx] = TerrainType.Forest;
                        else
                            map.TerrainTypes[idx] = TerrainType.Plains;
                    }
                    // Southern China (subtropical forests and hills)
                    else if (y >= 0.65f)
                    {
                        float noise = Mathf.PerlinNoise(q * 0.08f + 100, r * 0.08f + 100);
                        if (noise > 0.5f)
                            map.TerrainTypes[idx] = TerrainType.Forest;
                        else if (noise < 0.2f)
                            map.TerrainTypes[idx] = TerrainType.Swamp;
                        else
                            map.TerrainTypes[idx] = TerrainType.Plains;
                    }
                    // Eastern coast
                    else if (x >= 0.75f)
                    {
                        float noise = Mathf.PerlinNoise(q * 0.12f + 200, r * 0.12f + 200);
                        if (noise > 0.6f)
                            map.TerrainTypes[idx] = TerrainType.Forest;
                        else
                            map.TerrainTypes[idx] = TerrainType.Plains;
                    }
                    // Northern forests (Manchuria)
                    else if (y < 0.25f && x >= 0.4f)
                    {
                        map.TerrainTypes[idx] = TerrainType.Forest;
                    }
                    // Default: plains with some variation
                    else
                    {
                        float noise = Mathf.PerlinNoise(q * 0.1f + 300, r * 0.1f + 300);
                        if (noise > 0.65f)
                            map.TerrainTypes[idx] = TerrainType.Forest;
                        else if (noise < 0.2f)
                            map.TerrainTypes[idx] = TerrainType.Mountain;
                        else
                            map.TerrainTypes[idx] = TerrainType.Plains;
                    }

                    // === QI DENSITY ===
                    // Sacred Peaks have highest Qi
                    if (map.TerrainTypes[idx] == TerrainType.SacredPeak)
                    {
                        map.QiDensities[idx] = QiDensityLevel.LeyLine;
                    }
                    // Mountains and forests have moderate-high Qi
                    else if (map.TerrainTypes[idx] == TerrainType.Mountain)
                    {
                        map.QiDensities[idx] = QiDensityLevel.Dense;
                    }
                    else if (map.TerrainTypes[idx] == TerrainType.Forest)
                    {
                        map.QiDensities[idx] = QiDensityLevel.Moderate;
                    }
                    // Rivers have moderate Qi
                    else if (map.TerrainTypes[idx] == TerrainType.River)
                    {
                        map.QiDensities[idx] = QiDensityLevel.Moderate;
                    }
                    // Plains have sparse-moderate Qi
                    else if (map.TerrainTypes[idx] == TerrainType.Plains)
                    {
                        map.QiDensities[idx] = y >= 0.35f && y < 0.65f ? QiDensityLevel.Moderate : QiDensityLevel.Sparse;
                    }
                    // Desert and swamp have low Qi
                    else
                    {
                        map.QiDensities[idx] = QiDensityLevel.Sparse;
                    }
                }
            }

            // === SACRED PEAKS (famous mountains from Chinese mythology) ===
            map.SacredPeaks = new SacredPeakLocation[]
            {
                new() { Q = 10, R = 35, Name = "Mount Kunlun" },      // West - mother of mountains
                new() { Q = 25, R = 20, Name = "Mount Kunlun East" },  // Secondary Kunlun
                new() { Q = 55, R = 15, Name = "Mount Hua" },          // West-central - sword sect
                new() { Q = 70, R = 45, Name = "Mount Wudang" },       // Central - internal qi sect
                new() { Q = 85, R = 55, Name = "Mount Emei" },         // Southwest - balanced sect
                new() { Q = 95, R = 30, Name = "Mount Qingcheng" },     // Central-west - Taoist
                new() { Q = 45, R = 65, Name = "Mount Shaolin" },      // Central-south - martial sect
            };

            // Ensure Sacred Peak tiles are set correctly
            foreach (var peak in map.SacredPeaks)
            {
                if (peak.Q >= 0 && peak.Q < width && peak.R >= 0 && peak.R < height)
                {
                    int idx = peak.R * width + peak.Q;
                    map.TerrainTypes[idx] = TerrainType.SacredPeak;
                    map.Elevations[idx] = ElevationLevel.Summit;
                    map.QiDensities[idx] = QiDensityLevel.LeyLine;
                }
            }

            // === SETTLEMENTS (major Chinese cities/regions) ===
            map.Settlements = new SettlementLocation[]
            {
                new() { Q = 60, R = 40, Name = "Chang'an", InitialTrust = 50 },      // Central - ancient capital
                new() { Q = 80, R = 45, Name = "Luoyang", InitialTrust = 45 },       // Central-east
                new() { Q = 90, R = 50, Name = "Chengdu", InitialTrust = 40 },       // Southwest
                new() { Q = 100, R = 40, Name = "Nanjing", InitialTrust = 45 },      // East
                new() { Q = 105, R = 35, Name = "Hangzhou", InitialTrust = 40 },     // Southeast coast
                new() { Q = 75, R = 25, Name = "Beijing", InitialTrust = 35 },       // North
                new() { Q = 50, R = 55, Name = "Guangzhou", InitialTrust = 30 },     // Far south
                new() { Q = 35, R = 30, Name = "Lanzhou", InitialTrust = 35 },       // Northwest
                new() { Q = 85, R = 60, Name = "Kunming", InitialTrust = 30 },       // Far southwest
                new() { Q = 70, R = 55, Name = "Shaoguan", InitialTrust = 35 },      // South-central
                new() { Q = 95, R = 25, Name = "Qingdao", InitialTrust = 30 },       // Northeast coast
                new() { Q = 55, R = 35, Name = "Wuhan", InitialTrust = 40 },         // Central
            };

            // === STARTING LOCATIONS (player + AI sects) ===
            map.StartingLocations = new StartingLocation[]
            {
                new() { Q = 70, R = 45, PlayerIndex = 0 },   // Player - near Wudang (central)
                new() { Q = 55, R = 15, PlayerIndex = 1 },   // AI 1 - near Mount Hua (west)
                new() { Q = 95, R = 55, PlayerIndex = 2 },   // AI 2 - near Emei (southwest)
                new() { Q = 85, R = 25, PlayerIndex = 3 },   // AI 3 - near Qingcheng (north-central)
            };

            // === RESOURCE DEPOSITS ===
            var deposits = new System.Collections.Generic.List<ResourceDeposit>();
            var rng = new System.Random(42); // Fixed seed for reproducibility

            for (int r = 0; r < height; r++)
            {
                for (int q = 0; q < width; q++)
                {
                    int idx = r * width + q;
                    var terrain = map.TerrainTypes[idx];

                    // Mountain: Iron Ore (40%), Jade (20%)
                    if (terrain == TerrainType.Mountain || terrain == TerrainType.SacredPeak)
                    {
                        float roll = (float)rng.NextDouble();
                        if (roll < 0.4f)
                            deposits.Add(new ResourceDeposit { Q = q, R = r, Type = DepositType.IronOre, Abundance = rng.Next(1, 4) });
                        else if (roll < 0.6f)
                            deposits.Add(new ResourceDeposit { Q = q, R = r, Type = DepositType.Jade, Abundance = rng.Next(1, 3) });
                    }
                    // Forest: Lumber (35%), Medicinal Herbs (25%)
                    if (terrain == TerrainType.Forest)
                    {
                        float roll = (float)rng.NextDouble();
                        if (roll < 0.35f)
                            deposits.Add(new ResourceDeposit { Q = q, R = r, Type = DepositType.Lumber, Abundance = rng.Next(1, 4) });
                        else if (roll < 0.6f)
                            deposits.Add(new ResourceDeposit { Q = q, R = r, Type = DepositType.MedicinalHerbs, Abundance = rng.Next(1, 3) });
                    }
                    // Swamp: Medicinal Herbs (60%)
                    if (terrain == TerrainType.Swamp)
                    {
                        float roll = (float)rng.NextDouble();
                        if (roll < 0.6f)
                            deposits.Add(new ResourceDeposit { Q = q, R = r, Type = DepositType.MedicinalHerbs, Abundance = rng.Next(2, 4) });
                    }
                    // Sacred Peak + Ley Line: Spirit Herbs (30%)
                    if (terrain == TerrainType.SacredPeak || map.QiDensities[idx] == QiDensityLevel.LeyLine)
                    {
                        float roll = (float)rng.NextDouble();
                        if (roll < 0.3f)
                            deposits.Add(new ResourceDeposit { Q = q, R = r, Type = DepositType.SpiritHerbs, Abundance = rng.Next(1, 3) });
                    }
                    // Plains: Tea Leaves (15%)
                    if (terrain == TerrainType.Plains && r > height * 0.5f)
                    {
                        float roll = (float)rng.NextDouble();
                        if (roll < 0.15f)
                            deposits.Add(new ResourceDeposit { Q = q, R = r, Type = DepositType.TeaLeaves, Abundance = rng.Next(1, 3) });
                    }
                }
            }
            map.ResourceDeposits = deposits.ToArray();

            EditorUtility.SetDirty(map);
            Debug.Log($"[PresetMapCreator] Jianghu map created: {totalTiles} tiles, {map.SacredPeaks.Length} sacred peaks, {map.Settlements.Length} settlements, {map.ResourceDeposits.Length} resource deposits");
        }

        #endregion

        #region Spirit Realm Map

        static void CreateSpiritRealmMap()
        {
            string path = "Assets/_Game/Data/Maps/PresetMap_SpiritRealm.asset";
            EnsureDirectory("Assets/_Game/Data/Maps");

            var map = AssetDatabase.LoadAssetAtPath<PresetMapData>(path);
            if (map == null)
            {
                map = ScriptableObject.CreateInstance<PresetMapData>();
                AssetDatabase.CreateAsset(map, path);
            }

            int width = 100;
            int height = 60;
            int totalTiles = width * height;

            map.MapName = "Spirit Realm";
            map.Description = "A fantastical world of exaggerated terrain. Floating islands, crystal caverns, and spirit forests create a whimsical cultivation landscape where the rules of nature bend to the will of Qi.";
            map.MapSize = MapSize.Medium;

            map.TerrainTypes = new TerrainType[totalTiles];
            map.Elevations = new ElevationLevel[totalTiles];
            map.QiDensities = new QiDensityLevel[totalTiles];

            for (int i = 0; i < totalTiles; i++)
            {
                map.TerrainTypes[i] = TerrainType.Plains;
                map.Elevations[i] = ElevationLevel.Low;
                map.QiDensities[i] = QiDensityLevel.Moderate; // Higher baseline Qi
            }

            // Spirit Realm: more Sacred Peaks, more Forest, more varied terrain
            for (int r = 0; r < height; r++)
            {
                for (int q = 0; q < width; q++)
                {
                    int idx = r * width + q;
                    float x = q / (float)width;
                    float y = r / (float)height;

                    // Multiple terrain zones with sharp transitions
                    float noise1 = Mathf.PerlinNoise(q * 0.06f, r * 0.06f);
                    float noise2 = Mathf.PerlinNoise(q * 0.12f + 500, r * 0.12f + 500);

                    // Elevation: more dramatic variation
                    if (noise1 > 0.7f)
                        map.Elevations[idx] = ElevationLevel.Summit;
                    else if (noise1 > 0.5f)
                        map.Elevations[idx] = ElevationLevel.High;
                    else if (noise1 > 0.3f)
                        map.Elevations[idx] = ElevationLevel.Medium;
                    else
                        map.Elevations[idx] = ElevationLevel.Low;

                    // Terrain: more forests and sacred peaks
                    if (noise2 > 0.75f)
                        map.TerrainTypes[idx] = TerrainType.SacredPeak;
                    else if (noise2 > 0.55f)
                        map.TerrainTypes[idx] = TerrainType.Forest;
                    else if (noise2 > 0.4f)
                        map.TerrainTypes[idx] = TerrainType.Mountain;
                    else if (noise2 > 0.25f)
                        map.TerrainTypes[idx] = TerrainType.River;
                    else if (noise2 < 0.1f)
                        map.TerrainTypes[idx] = TerrainType.Swamp;
                    else
                        map.TerrainTypes[idx] = TerrainType.Plains;

                    // Qi: everything is higher
                    if (map.TerrainTypes[idx] == TerrainType.SacredPeak)
                        map.QiDensities[idx] = QiDensityLevel.LeyLine;
                    else if (map.TerrainTypes[idx] == TerrainType.Forest || map.TerrainTypes[idx] == TerrainType.Mountain)
                        map.QiDensities[idx] = QiDensityLevel.Dense;
                    else
                        map.QiDensities[idx] = QiDensityLevel.Moderate;
                }
            }

            // More Sacred Peaks (fantasy world has more)
            map.SacredPeaks = new SacredPeakLocation[]
            {
                new() { Q = 15, R = 15, Name = "Celestial Peak" },
                new() { Q = 85, R = 10, Name = "Dragon Spine" },
                new() { Q = 50, R = 30, Name = "Spirit Lotus Summit" },
                new() { Q = 25, R = 45, Name = "Moonlit Pinnacle" },
                new() { Q = 75, R = 50, Name = "Thunder Crown" },
                new() { Q = 90, R = 35, Name = "Phoenix Nest" },
            };

            foreach (var peak in map.SacredPeaks)
            {
                if (peak.Q >= 0 && peak.Q < width && peak.R >= 0 && peak.R < height)
                {
                    int idx = peak.R * width + peak.Q;
                    map.TerrainTypes[idx] = TerrainType.SacredPeak;
                    map.Elevations[idx] = ElevationLevel.Summit;
                    map.QiDensities[idx] = QiDensityLevel.LeyLine;
                }
            }

            map.Settlements = new SettlementLocation[]
            {
                new() { Q = 50, R = 30, Name = "Spirit Gate City", InitialTrust = 50 },
                new() { Q = 30, R = 20, Name = "Jade Market", InitialTrust = 40 },
                new() { Q = 70, R = 40, Name = "Cloud Harbor", InitialTrust = 35 },
                new() { Q = 20, R = 45, Name = "Mystic Village", InitialTrust = 45 },
                new() { Q = 80, R = 15, Name = "Crystal Town", InitialTrust = 30 },
            };

            map.StartingLocations = new StartingLocation[]
            {
                new() { Q = 50, R = 30, PlayerIndex = 0 },
                new() { Q = 20, R = 15, PlayerIndex = 1 },
                new() { Q = 80, R = 45, PlayerIndex = 2 },
                new() { Q = 35, R = 50, PlayerIndex = 3 },
            };

            // Resource deposits: more abundant
            var deposits = new System.Collections.Generic.List<ResourceDeposit>();
            var rng = new System.Random(123);

            for (int r = 0; r < height; r++)
            {
                for (int q = 0; q < width; q++)
                {
                    int idx = r * width + q;
                    var terrain = map.TerrainTypes[idx];
                    float roll = (float)rng.NextDouble();

                    if (terrain == TerrainType.Mountain && roll < 0.5f)
                        deposits.Add(new ResourceDeposit { Q = q, R = r, Type = DepositType.IronOre, Abundance = rng.Next(1, 4) });
                    if (terrain == TerrainType.Forest && roll < 0.45f)
                        deposits.Add(new ResourceDeposit { Q = q, R = r, Type = DepositType.Lumber, Abundance = rng.Next(1, 4) });
                    if ((terrain == TerrainType.Forest || terrain == TerrainType.Swamp) && roll < 0.35f)
                        deposits.Add(new ResourceDeposit { Q = q, R = r, Type = DepositType.MedicinalHerbs, Abundance = rng.Next(1, 3) });
                    if (terrain == TerrainType.SacredPeak && roll < 0.5f)
                        deposits.Add(new ResourceDeposit { Q = q, R = r, Type = DepositType.SpiritHerbs, Abundance = rng.Next(2, 4) });
                    if (terrain == TerrainType.Mountain && roll < 0.25f)
                        deposits.Add(new ResourceDeposit { Q = q, R = r, Type = DepositType.Jade, Abundance = rng.Next(1, 3) });
                }
            }
            map.ResourceDeposits = deposits.ToArray();

            EditorUtility.SetDirty(map);
            Debug.Log($"[PresetMapCreator] Spirit Realm map created: {totalTiles} tiles, {map.SacredPeaks.Length} sacred peaks");
        }

        #endregion

        #region Wasteland Map

        static void CreateWastelandMap()
        {
            string path = "Assets/_Game/Data/Maps/PresetMap_Wasteland.asset";
            EnsureDirectory("Assets/_Game/Data/Maps");

            var map = AssetDatabase.LoadAssetAtPath<PresetMapData>(path);
            if (map == null)
            {
                map = ScriptableObject.CreateInstance<PresetMapData>();
                AssetDatabase.CreateAsset(map, path);
            }

            int width = 80;
            int height = 60;
            int totalTiles = width * height;

            map.MapName = "Wasteland";
            map.Description = "A post-cataclysm world scarred by ancient battles between immortal cultivators. Resources are scarce, Qi is thin, and survival is a daily struggle. Only the strongest sects will endure.";
            map.MapSize = MapSize.Medium;

            map.TerrainTypes = new TerrainType[totalTiles];
            map.Elevations = new ElevationLevel[totalTiles];
            map.QiDensities = new QiDensityLevel[totalTiles];

            for (int i = 0; i < totalTiles; i++)
            {
                map.TerrainTypes[i] = TerrainType.Desert; // Default to harsh terrain
                map.Elevations[i] = ElevationLevel.Medium;
                map.QiDensities[i] = QiDensityLevel.None; // Very low baseline Qi
            }

            // Wasteland: mostly desert and swamp, few forests, rare oases
            for (int r = 0; r < height; r++)
            {
                for (int q = 0; q < width; q++)
                {
                    int idx = r * width + q;
                    float x = q / (float)width;
                    float y = r / (float)height;

                    float noise1 = Mathf.PerlinNoise(q * 0.05f, r * 0.05f);
                    float noise2 = Mathf.PerlinNoise(q * 0.1f + 200, r * 0.1f + 200);

                    // Elevation: mostly flat with some ridges
                    if (noise1 > 0.7f)
                        map.Elevations[idx] = ElevationLevel.High;
                    else if (noise1 > 0.5f)
                        map.Elevations[idx] = ElevationLevel.Medium;
                    else
                        map.Elevations[idx] = ElevationLevel.Low;

                    // Terrain: mostly desert, some swamp, rare forests near water
                    if (noise2 > 0.8f)
                        map.TerrainTypes[idx] = TerrainType.SacredPeak; // Rare oases of power
                    else if (noise2 > 0.6f)
                        map.TerrainTypes[idx] = TerrainType.Mountain; // Rocky ridges
                    else if (noise2 > 0.4f)
                        map.TerrainTypes[idx] = TerrainType.Swamp; // Toxic marshes
                    else if (noise2 > 0.25f)
                        map.TerrainTypes[idx] = TerrainType.Plains; // Barren flats
                    else if (noise2 < 0.1f)
                        map.TerrainTypes[idx] = TerrainType.River; // Rare water sources
                    else
                        map.TerrainTypes[idx] = TerrainType.Desert; // Default wasteland

                    // Qi: very low except near sacred peaks and rivers
                    if (map.TerrainTypes[idx] == TerrainType.SacredPeak)
                        map.QiDensities[idx] = QiDensityLevel.LeyLine;
                    else if (map.TerrainTypes[idx] == TerrainType.River)
                        map.QiDensities[idx] = QiDensityLevel.Moderate;
                    else if (map.TerrainTypes[idx] == TerrainType.Mountain)
                        map.QiDensities[idx] = QiDensityLevel.Sparse;
                    else
                        map.QiDensities[idx] = QiDensityLevel.None;
                }
            }

            // Very few Sacred Peaks (precious oases)
            map.SacredPeaks = new SacredPeakLocation[]
            {
                new() { Q = 20, R = 20, Name = "Last Sanctuary" },
                new() { Q = 60, R = 40, Name = "Remnant Peak" },
                new() { Q = 40, R = 10, Name = "Ash Crown" },
            };

            foreach (var peak in map.SacredPeaks)
            {
                if (peak.Q >= 0 && peak.Q < width && peak.R >= 0 && peak.R < height)
                {
                    int idx = peak.R * width + peak.Q;
                    map.TerrainTypes[idx] = TerrainType.SacredPeak;
                    map.Elevations[idx] = ElevationLevel.Summit;
                    map.QiDensities[idx] = QiDensityLevel.LeyLine;
                }
            }

            // Few settlements (survivor enclaves)
            map.Settlements = new SettlementLocation[]
            {
                new() { Q = 40, R = 30, Name = "Refuge", InitialTrust = 30 },
                new() { Q = 20, R = 45, Name = "Dust Village", InitialTrust = 20 },
                new() { Q = 65, R = 20, Name = "Iron Camp", InitialTrust = 25 },
            };

            map.StartingLocations = new StartingLocation[]
            {
                new() { Q = 40, R = 30, PlayerIndex = 0 },
                new() { Q = 15, R = 15, PlayerIndex = 1 },
                new() { Q = 65, R = 45, PlayerIndex = 2 },
            };

            // Sparse resources
            var deposits = new System.Collections.Generic.List<ResourceDeposit>();
            var rng = new System.Random(789);

            for (int r = 0; r < height; r++)
            {
                for (int q = 0; q < width; q++)
                {
                    int idx = r * width + q;
                    var terrain = map.TerrainTypes[idx];
                    float roll = (float)rng.NextDouble();

                    // Lower probabilities across the board
                    if (terrain == TerrainType.Mountain && roll < 0.25f)
                        deposits.Add(new ResourceDeposit { Q = q, R = r, Type = DepositType.IronOre, Abundance = rng.Next(1, 3) });
                    if (terrain == TerrainType.Swamp && roll < 0.3f)
                        deposits.Add(new ResourceDeposit { Q = q, R = r, Type = DepositType.MedicinalHerbs, Abundance = rng.Next(1, 3) });
                    if (terrain == TerrainType.SacredPeak && roll < 0.6f)
                        deposits.Add(new ResourceDeposit { Q = q, R = r, Type = DepositType.SpiritHerbs, Abundance = rng.Next(1, 4) });
                    if (terrain == TerrainType.River && roll < 0.4f)
                        deposits.Add(new ResourceDeposit { Q = q, R = r, Type = DepositType.Jade, Abundance = rng.Next(1, 2) });
                }
            }
            map.ResourceDeposits = deposits.ToArray();

            EditorUtility.SetDirty(map);
            Debug.Log($"[PresetMapCreator] Wasteland map created: {totalTiles} tiles, {map.SacredPeaks.Length} sacred peaks");
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

        static void DeleteAllMaps()
        {
            string path = "Assets/_Game/Data/Maps";
            if (AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.DeleteAsset(path);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[PresetMapCreator] All preset maps deleted.");
        }

        #endregion
    }
}
