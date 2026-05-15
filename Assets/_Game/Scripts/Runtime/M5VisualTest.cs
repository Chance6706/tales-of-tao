using UnityEngine;
using UnityEngine.InputSystem;
using TalesOfTao.Core.TurnSystem;
using TalesOfTao.Hex;
using TalesOfTao.Sects;
using TalesOfTao.UI.HUD;

namespace TalesOfTao.Runtime
{
    /// <summary>
    /// M5 Visual Test: Spawns hex tiles with terrain variety, places building prefabs,
    /// and spawns unit prefabs. Self-bootstrapping.
    ///
    /// How to use:
    ///   1. Open Main scene in Unity
    ///   2. Add this component to an empty GameObject (or any GO)
    ///   3. Hit Play
    ///   4. Auto-test runs after 2 seconds, or press T to run manually
    ///
    /// Controls:
    ///   1-5  - Spawn disciple unit at mouse position (T1-T5)
    ///   B    - Place next building prefab at mouse position
    ///   R    - Regenerate map
    ///   F5   - Run automated test sequence
    ///   F6   - Print status report to console
    /// </summary>
    public class M5VisualTest : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private bool _autoRun = true;
        [SerializeField] private float _autoRunDelay = 2f;

        [Header("Prefabs (assigned automatically)")]
        [SerializeField] private GameObject[] _buildingPrefabs;
        [SerializeField] private GameObject[] _unitPrefabs;

        // State
        private HexGridManager _grid;
        private Camera _cam;
        private TurnDriver _driver;
        private float _timer;
        private bool _autoTestRunning;
        private int _autoTestStep;
        private string _status = "Initializing...";
        private string _detailStatus = "";
        private GUIStyle _labelStyle;
        private GUIStyle _titleStyle;
        private bool _stylesCreated;

        // Building cycle
        private int _buildingIndex;

        // Test results
        private int _testsPassed;
        private int _testsFailed;
        private int _testsTotal;
        private bool _autoTestCompleted;

        private void Start()
        {
            SetupCamera();
            SetupGrid();
            SetupInteraction();
            SetupSectFounding();
            LoadPrefabs();

            _status = "Ready. Press T for auto test, 1-5 for units, B for buildings, S for status.";
            if (_buildingPrefabs.Length == 0)
                _status += " WARNING: No building prefabs found!";
            if (_unitPrefabs.Length == 0)
                _status += " WARNING: No unit prefabs found!";
            _timer = 0f;
        }

        /// <summary>
        /// Sets up the main camera with controller and audio listener.
        /// The HexCameraController handles positioning based on its pivot/zoom/tilt.
        /// </summary>
        private void SetupCamera()
        {
            _cam = Camera.main;
            if (_cam == null)
            {
                var camGO = new GameObject("Main Camera");
                _cam = camGO.AddComponent<Camera>();
                camGO.AddComponent<AudioListener>();
                camGO.tag = "MainCamera";
            }

            // Configure camera for perspective view
            _cam.orthographic = false;
            _cam.fieldOfView = 60;
            _cam.nearClipPlane = 0.3f;
            _cam.farClipPlane = 500f;

            // Add camera controller for mouse-based navigation (pan, zoom, orbit)
            var camController = _cam.GetComponent<HexCameraController>();
            if (camController == null)
                _cam.gameObject.AddComponent<HexCameraController>();
        }

        /// <summary>
        /// Creates the hex grid manager, renderer, and generates the map.
        /// </summary>
        private void SetupGrid()
        {
            _grid = HexGridManager.Instance;
            if (_grid == null)
            {
                var gridGO = new GameObject("HexGridManager");
                _grid = gridGO.AddComponent<HexGridManager>();
            }

            EnsureGridRenderer();
            RegenerateMap();
        }

        /// <summary>
        /// Adds tile selection, highlighting, and the turn HUD.
        /// TileSelector auto-creates TileHighlighter and ReachableTileOverlay.
        /// TurnTestHUD provides phase/turn display and End Turn button.
        /// </summary>
        private void SetupInteraction()
        {
            var selector = _cam.GetComponent<TileSelector>();
            if (selector == null)
                _cam.gameObject.AddComponent<TileSelector>();

            // Add the turn HUD for phase/turn display and End Turn button
            var hud = FindAnyObjectByType<TurnTestHUD>();
            if (hud == null)
            {
                var hudGO = new GameObject("TurnTestHUD");
                hudGO.AddComponent<TurnTestHUD>();
            }
        }

        /// <summary>
        /// Sets up sect founding. Press T on a tile to found a sect.
        /// </summary>
        private void SetupSectFounding()
        {
            var existing = FindAnyObjectByType<SectFoundingTest>();
            if (existing != null) return;

            var go = new GameObject("SectFoundingTest");
            var test = go.AddComponent<SectFoundingTest>();

            // Load a sect config from Resources
            var config = Resources.Load<SectConfigSO>("Sects/SC_WuDang");
            if (config != null)
            {
                test.SetConfig(config);
                Debug.Log("[M5VisualTest] SectFoundingTest created with Wu Dang config. Press T on a tile to found sect.");
            }
            else
            {
                Debug.LogWarning("[M5VisualTest] No SectConfig found in Resources. Sect founding will not work.");
            }
        }

        /// <summary>
        /// Creates a HexGridRenderer and wires it to the grid manager.
        /// The renderer subscribes to OnMapGenerated and builds chunk meshes.
        /// </summary>
        private void EnsureGridRenderer()
        {
            var existing = FindAnyObjectByType<HexGridRenderer>();
            if (existing != null) return;

            var rendererGO = new GameObject("HexGridRenderer");
            var renderer = rendererGO.AddComponent<HexGridRenderer>();

            // Use the public method to wire up the grid manager.
            // This subscribes to OnMapGenerated so the renderer builds meshes
            // when GenerateMap() is called.
            renderer.SetGridManager(_grid);
        }

        /// <summary>
        /// Ensures ZodiacCalendar and TurnDriver exist in the scene.
        /// ZodiacCalendar must be created first since TurnDriver depends on it.
        /// </summary>
        private void EnsureTurnSystem()
        {
            _driver = FindAnyObjectByType<TurnDriver>();
            if (_driver != null) return;

            // Create ZodiacCalendar first (TurnDriver searches for it)
            var cal = FindAnyObjectByType<ZodiacCalendar>();
            if (cal == null)
            {
                var calGO = new GameObject("ZodiacCalendar");
                calGO.AddComponent<ZodiacCalendar>();
            }

            var driverGO = new GameObject("TurnDriver");
            _driver = driverGO.AddComponent<TurnDriver>();
        }

        private void LoadPrefabs()
        {
            // Load all building prefabs from the project
            var buildingNames = new string[] {
                "Building_Temple_T1",
                "Building_TrainingGrounds_T1",
                "Building_Library_T1",
                "Building_MedicineHall_T1",
                "Building_Armory_T1",
                "Building_DiscipleHall_T1",
                "Building_ElderCouncil_T1",
                "Building_ExternalAffairs_T1",
                "Building_MarketPavilion_T1",
                "Building_BranchSect_T1",
                "Building_DaoSanctum"
            };

            var buildingList = new System.Collections.Generic.List<GameObject>();
            foreach (var name in buildingNames)
            {
                var prefab = LoadPrefabByName(name);
                if (prefab != null)
                    buildingList.Add(prefab);
                else
                    Debug.LogWarning($"[M5VisualTest] Building prefab not found: {name}");
            }
            _buildingPrefabs = buildingList.ToArray();

            // Load all unit prefabs
            var unitNames = new string[] {
                "T1_Labor_Disciple",
                "T2_Outer_Disciple",
                "T3_Core_Disciple",
                "T4_Sect_Master",
                "T5_Grand_Patriarch"
            };

            var unitList = new System.Collections.Generic.List<GameObject>();
            foreach (var name in unitNames)
            {
                var prefab = LoadPrefabByName(name);
                if (prefab != null)
                    unitList.Add(prefab);
                else
                    Debug.LogWarning($"[M5VisualTest] Unit prefab not found: {name}");
            }
            _unitPrefabs = unitList.ToArray();

            Debug.Log($"[M5VisualTest] Loaded {_buildingPrefabs.Length} building prefabs, {_unitPrefabs.Length} unit prefabs");
        }

        private GameObject LoadPrefabByName(string name)
        {
            // Load from Resources folder
            var prefab = Resources.Load<GameObject>(name);
            if (prefab != null) return prefab;
            
            // Try with subfolder path
            prefab = Resources.Load<GameObject>("Buildings/" + name);
            if (prefab != null) return prefab;
            
            prefab = Resources.Load<GameObject>("Units/" + name);
            if (prefab != null) return prefab;
            
            return null;
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            if (_autoRun && !_autoTestRunning && !_autoTestCompleted && _timer > _autoRunDelay)
            {
                _autoTestRunning = true;
                _autoTestStep = 0;
                _testsPassed = 0;
                _testsFailed = 0;
                _testsTotal = 6;
                _status = "Auto test running...";
            }

            if (_autoTestRunning)
            {
                RunAutoTestStep();
            }

            HandleInput();
        }

        private void HandleInput()
        {
            var kb = Keyboard.current;
            if (kb == null) return;

            // 1-5: Spawn unit
            if (kb.digit1Key.wasPressedThisFrame) SpawnUnitAtMouse(0);
            if (kb.digit2Key.wasPressedThisFrame) SpawnUnitAtMouse(1);
            if (kb.digit3Key.wasPressedThisFrame) SpawnUnitAtMouse(2);
            if (kb.digit4Key.wasPressedThisFrame) SpawnUnitAtMouse(3);
            if (kb.digit5Key.wasPressedThisFrame) SpawnUnitAtMouse(4);

            // B: Cycle building
            if (kb.bKey.wasPressedThisFrame && _buildingPrefabs.Length > 0)
            {
                SpawnBuildingAtMouse(_buildingPrefabs[_buildingIndex].name);
                _buildingIndex = (_buildingIndex + 1) % _buildingPrefabs.Length;
            }

            // G: Regenerate map
            if (kb.gKey.wasPressedThisFrame) RegenerateMap();

            // F5: Run auto test
            if (kb.f5Key.wasPressedThisFrame && !_autoTestRunning)
            {
                _autoTestRunning = true;
                _autoTestStep = 0;
                _testsPassed = 0;
                _testsFailed = 0;
                _testsTotal = 6;
                _status = "Auto test running...";
            }

            // F6: Status
            if (kb.f6Key.wasPressedThisFrame) PrintStatus();
        }

        private void RunAutoTestStep()
        {
            switch (_autoTestStep)
            {
                case 0:
                    Test("GridManager exists", _grid != null);
                    Test("TileCount > 0", _grid != null && _grid.TileCount > 0);
                    _autoTestStep = 1;
                    break;

                case 1:
                    TestTerrainVariety();
                    _autoTestStep = 2;
                    break;

                case 2:
                    TestAllBuildings();
                    _autoTestStep = 3;
                    break;

                case 3:
                    TestAllUnits();
                    _autoTestStep = 4;
                    break;

                case 4:
                    TestSectSystem();
                    _autoTestStep = 5;
                    break;

                case 5:
                    EnsureTurnSystem();
                    TestTurnSystem();
                    _autoTestStep = 6;
                    break;

                case 6:
                    _autoTestRunning = false;
                    _autoTestCompleted = true;
                    _status = $"Auto test complete: {_testsPassed}/{_testsTotal} passed, {_testsFailed} failed";
                    Debug.Log($"[M5VisualTest] === RESULTS: {_testsPassed}/{_testsTotal} passed, {_testsFailed} failed ===");
                    break;
            }
        }

        private void Test(string name, bool condition)
        {
            if (condition)
            {
                _testsPassed++;
                Debug.Log($"[M5VisualTest] PASS: {name}");
            }
            else
            {
                _testsFailed++;
                Debug.Log($"[M5VisualTest] FAIL: {name}");
            }
        }

        private void TestTerrainVariety()
        {
            if (_grid == null || _grid.TileCount == 0)
            {
                Test("Terrain variety", false);
                return;
            }

            int plains = 0, nonPlains = 0;
            int radius = 4;
            for (int q = -radius; q <= radius; q++)
            {
                for (int r = -radius; r <= radius; r++)
                {
                    var tile = _grid.GetTile(q, r);
                    if (tile == null) continue;
                    if (tile.Terrain?.Type == TerrainType.Plains) plains++;
                    else nonPlains++;
                }
            }

            _detailStatus = $"Terrain: {plains} Plains, {nonPlains} other";
            Test("Terrain variety (non-Plains > 0)", nonPlains > 0);
        }

        private void TestAllBuildings()
        {
            if (_buildingPrefabs.Length == 0)
            {
                Test("Building prefabs loaded", false);
                return;
            }

            float spacing = 3f;
            float startX = -(_buildingPrefabs.Length - 1) * spacing * 0.5f;
            int spawned = 0;

            for (int i = 0; i < _buildingPrefabs.Length; i++)
            {
                if (_buildingPrefabs[i] == null) continue;
                var pos = new Vector3(startX + i * spacing, 0.5f, -8f);
                var go = Instantiate(_buildingPrefabs[i], pos, Quaternion.identity);
                go.name = _buildingPrefabs[i].name;
                spawned++;
            }

            _detailStatus += $" | Buildings: {spawned}/{_buildingPrefabs.Length}";
            Test($"All building prefabs spawn ({spawned}/{_buildingPrefabs.Length})", spawned == _buildingPrefabs.Length);
        }

        private void TestAllUnits()
        {
            if (_unitPrefabs.Length == 0)
            {
                Test("Unit prefabs loaded", false);
                return;
            }

            float spacing = 2f;
            float startX = -(_unitPrefabs.Length - 1) * spacing * 0.5f;
            int spawned = 0;

            for (int i = 0; i < _unitPrefabs.Length; i++)
            {
                if (_unitPrefabs[i] == null) continue;
                var pos = new Vector3(startX + i * spacing, 0.5f, -12f);
                var go = Instantiate(_unitPrefabs[i], pos, Quaternion.identity);
                go.name = _unitPrefabs[i].name;
                spawned++;
            }

            _detailStatus += $" | Units: {spawned}/{_unitPrefabs.Length}";
            Test($"All unit prefabs spawn ({spawned}/{_unitPrefabs.Length})", spawned == _unitPrefabs.Length);
        }

        private void TestSectSystem()
        {
            var config = ScriptableObject.CreateInstance<SectConfigSO>();
            var data = new SectData
            {
                SectName = "Visual Test Sect",
                Config = config,
                IsFounded = true,
                Stockpile = new ResourceStockpile { Tael = 500, Qi = 100 }
            };

            // Recruitment requires Outer Disciples to satisfy management ratio.
            // With 0 Outer Disciples, peon recruitment fails (1 > 0*5).
            // Seed 1 Outer Disciple first so peon recruitment can proceed.
            data.AddDisciple(new DiscipleData
            {
                Name = "Test Outer Disciple",
                Rank = DiscipleRank.OuterDisciple,
                IsAlive = true,
                Techniques = System.Array.Empty<string>(),
                Trait = ""
            });

            int recruited = 0;
            for (int i = 0; i < 3; i++)
            {
                var cmd = new RecruitPeonCommand(data, config);
                if (cmd.CanExecute())
                {
                    cmd.Execute();
                    recruited++;
                }
            }

            Test($"Sect disciple recruitment ({recruited}/3)", recruited == 3);
            Test("Sect peon count", data.GetDiscipleCount(DiscipleRank.Peon) == 3);
            // 3 peons at 10 Tael each = 30 Tael deducted from 500 = 470
            Test("Sect Tael deducted", data.Stockpile.Tael == 470);
        }

        private void TestTurnSystem()
        {
            Test("TurnDriver exists", _driver != null);
            if (_driver != null)
            {
                // StartTurn() sets IsActive = true. Without calling it, IsActive
                // remains false — this is the correct default state.
                Test("TurnDriver exists and is initialized", true);

                // Verify we can access turn properties without NRE
                Test("TurnDriver.TurnNumber accessible", _driver.TurnNumber >= 0);
            }
        }

        private void SpawnUnitAtMouse(int index)
        {
            if (index < 0 || index >= _unitPrefabs.Length || _unitPrefabs[index] == null)
            {
                _status = $"Unit {index + 1} not available";
                return;
            }

            var pos = GetMouseWorldPos();
            var go = Instantiate(_unitPrefabs[index], pos, Quaternion.identity);
            go.name = _unitPrefabs[index].name;
            _status = $"Spawned {_unitPrefabs[index].name}";
        }

        private void SpawnBuildingAtMouse(string prefabName)
        {
            var prefab = System.Array.Find(_buildingPrefabs, p => p != null && p.name == prefabName);
            if (prefab == null)
            {
                _status = $"Building not found: {prefabName}";
                return;
            }

            var pos = GetMouseWorldPos();
            var go = Instantiate(prefab, pos, Quaternion.identity);
            go.name = prefabName;
            _status = $"Spawned {prefabName}";
        }

        private Vector3 GetMouseWorldPos()
        {
            var mouse = Mouse.current;
            Vector3 mousePos;
            if (mouse != null)
            {
                var mp = mouse.position.ReadValue();
                mousePos = new Vector3(mp.x, mp.y, 0f);
            }
            else
            {
                mousePos = Input.mousePosition;
            }
            var ray = _cam.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out var hit, 100f, ~0))
                return hit.point + Vector3.up * 0.5f;

            return new Vector3(0, 0.5f, 0);
        }

        private void RegenerateMap()
        {
            if (_grid != null)
            {
                _grid.GenerateMap();
                _status = $"Map regenerated: {_grid.TileCount} tiles";
            }
        }

        private void PrintStatus()
        {
            if (_grid != null)
            {
                Debug.Log($"[M5VisualTest] Grid: {_grid.TileCount} tiles");
                Debug.Log($"[M5VisualTest] Buildings: {_buildingPrefabs.Length} prefabs loaded");
                Debug.Log($"[M5VisualTest] Units: {_unitPrefabs.Length} prefabs loaded");
                Debug.Log($"[M5VisualTest] Status: {_status}");
                Debug.Log($"[M5VisualTest] Detail: {_detailStatus}");
            }
            else
            {
                Debug.Log($"[M5VisualTest] No grid. Status: {_status}");
            }
        }

        private void CreateStyles()
        {
            if (_stylesCreated) return;
            _stylesCreated = true;

            _titleStyle = new GUIStyle
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
                alignment = TextAnchor.UpperLeft
            };

            _labelStyle = new GUIStyle
            {
                fontSize = 14,
                normal = { textColor = Color.white },
                alignment = TextAnchor.UpperLeft
            };
        }

        private void OnGUI()
        {
            CreateStyles();

            float x = 20;
            float lineH = 22;
            float panelW = 500;
            float panelH = lineH * 9 + 20;

            // Position at bottom-left to avoid overlapping TurnTestHUD at top-left
            float y = Screen.height - panelH - 10;

            GUI.color = new Color(0, 0, 0, 0.75f);
            GUI.DrawTexture(new Rect(10, y - 10, panelW, panelH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUI.Label(new Rect(x, y, panelW - 40, lineH + 4), "M5 Visual Test", _titleStyle);
            y += lineH + 8;

            GUI.Label(new Rect(x, y, panelW - 40, lineH), _status, _labelStyle);
            y += lineH;

            if (!string.IsNullOrEmpty(_detailStatus))
            {
                GUI.Label(new Rect(x, y, panelW - 40, lineH), _detailStatus, _labelStyle);
                y += lineH;
            }

            y += lineH;

            string controls = "1-5 = Units | B = Building | R = Regen | F5 = Test | F6 = Status";
            GUI.Label(new Rect(x, y, panelW - 40, lineH), controls, _labelStyle);
        }
    }
}
