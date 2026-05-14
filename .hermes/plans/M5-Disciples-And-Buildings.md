# M5 — Disciples & Buildings: Engineering Plan

## Verify Condition
Recruit 3 peons → build Training Grounds (10-turn timer) → Outer Disciple spawns on completion

## Architecture Overview

### New Scripts to Create

1. **DiscipleData.cs** — Plain C# class (runtime state for a single disciple)
2. **BuildQueue.cs** — Construction queue system
3. **TrainingQueue.cs** — Promotion queue system  
4. **BuildingFactory.cs** — Creates building instances from BuildingConfigSO
5. **BuildingConfigSO.cs** — Extended data asset (replaces/extends BuildingDataSO)
6. **SectOverviewScreen.cs** — UI Toolkit screen controller
7. **RecruitPeonCommand.cs** — Command for recruiting peons
8. **BuildBuildingCommand.cs** — Command for starting construction
9. **M5PlayModeTest.cs** — PlayMode test for verify condition

### Assembly Dependencies
- New assembly: `TalesOfTao.Sects` (already exists from M4, add disciple/building scripts)
- References: Core, Economy, Hex, Units
- UI assembly: `TalesOfTao.UI` (add SectOverviewScreen)

## Detailed Specs

### 1. DiscipleData (plain C# class)
```
namespace TalesOfTao.Sects

public enum DiscipleRank { Peon = 0, OuterDisciple = 1, InnerDisciple = 2, Elder = 3, HighElder = 4 }

[Serializable]
public class DiscipleData
{
    public string Name;           // procedurally generated
    public DiscipleRank Rank;
    public int HP;
    public int Attack;
    public int Defense;
    public int Speed;
    public int QiPower;
    public int RootQuality;       // 0-10 cultivation potential
    public string[] Techniques;   // assigned technique IDs
    public string Trait;          // Lucky, Resilient, Perceptive, Reckless, Fragile
    public string BondedBeast;    // empty if none
    public bool IsAlive;
    
    // Stat calculation
    public void CalculateStats(SectConfigSO config, DiscipleRank tier);
}
```

### 2. BuildQueue
```
namespace TalesOfTao.Sects

public class BuildQueue : MonoBehaviour
{
    // Queue entry
    [Serializable]
    public struct BuildEntry
    {
        public string BuildingTypeId;   // e.g. "TrainingGrounds"
        public int Tier;                // 1-3
        public int TurnsRemaining;
        public int TotalTurns;
        public bool IsComplete;
    }
    
    [SerializeField] private BuildEntry[] _queue;
    [SerializeField] private int _maxConcurrent;  // = Temple tier
    
    public event Action<string> OnBuildCompleted;  // buildingTypeId
    
    public bool CanQueue(string buildingTypeId, int tier);
    public void Enqueue(string buildingTypeId, int tier, int turns);
    public void ProcessBuildPhase();  // called during Build phase
    public void Cancel(int index);
}
```

### 3. TrainingQueue
```
namespace TalesOfTao.Sects

public class TrainingQueue : MonoBehaviour
{
    [Serializable]
    public struct TrainingEntry
    {
        public string DiscipleName;
        public DiscipleRank FromRank;
        public DiscipleRank ToRank;
        public int TurnsRemaining;
        public int TotalTurns;
        public bool IsComplete;
    }
    
    [SerializeField] private TrainingEntry[] _queue;
    [SerializeField] private int _maxConcurrent;  // = Training Grounds capacity
    
    public event Action<DiscipleData> OnTrainingCompleted;
    
    public bool CanQueue(DiscipleRank from, DiscipleRank to);
    public void Enqueue(string discipleName, DiscipleRank from, DiscipleRank to, int turns);
    public void ProcessBuildPhase();  // training advances during Build phase too
    public void Cancel(int index);
}
```

### 4. BuildingFactory
```
namespace TalesOfTao.Sects

public class BuildingFactory : MonoBehaviour
{
    // Creates a building GameObject from BuildingConfigSO and places it
    public GameObject CreateBuilding(BuildingConfigSO config, int tier, Vector3 position);
    
    // Validates prerequisites
    public bool CanBuild(BuildingConfigSO config, int tier, SectData sect);
    
    // Gets build time in turns
    public int GetBuildTurns(BuildingConfigSO config, int tier);
    
    // Gets build cost
    public ResourceCost GetBuildCost(BuildingConfigSO config, int tier);
}
```

### 5. BuildingConfigSO (extends/replaces BuildingDataSO)
```
namespace TalesOfTao.Sects

[CreateAssetMenu(menuName = "TalesOfTao/Buildings/Building Config", fileName = "BuildingConfig_New")]
public class BuildingConfigSO : ScriptableObject
{
    [Header("Identity")]
    public string BuildingTypeId;       // "Temple", "TrainingGrounds", etc.
    public string DisplayName;
    
    [Header("Tier Costs (T1-T3)")]
    public ResourceCost[] TierCosts = new ResourceCost[3];
    public int[] TierBuildTurns = new int[3];
    
    [Header("Prerequisites")]
    public string PrerequisiteBuilding; // e.g. "Temple" for TrainingGrounds
    public int PrerequisiteTier;        // e.g. 1 for Temple T1
    
    [Header("Tier Effects")]
    public string[] TierEffects = new string[3];  // description
    
    [Header("Mesh")]
    public Mesh[] TierMeshes = new Mesh[3];  // T1, T2, T3
    
    [Header("Material")]
    public Material BuildingMaterial;
    
    [Header("Collider")]
    public bool RequiresCollider = true;
}

[Serializable]
public struct ResourceCost
{
    public int Tael;
    public int Qi;
    public int Lumber;
    public int IronOre;
    public int Jade;
    public int MedicinalHerbs;
    public int SpiritHerbs;
}
```

### 6. SectOverviewScreen (UI Toolkit)
```
namespace TalesOfTao.UI

public class SectOverviewScreen : MonoBehaviour
{
    // UXML template with:
    // - Building status panel (list of buildings, tier, upgrade availability, timers)
    // - Disciple roster (grouped by rank, count, Manage button)
    // - Resource summary (all commodities)
    // - Dissent meter (0-100 bar, breakdown)
    // - Zodiac year reminder
    
    public void Show();
    public void Hide();
    public void Refresh();  // update all panels from SectData
}
```

### 7. Commands
```
RecruitPeonCommand : Command
  - Cost: 10 Tael
  - Valid: during Action phase, settlement within 5 tiles, ratio check
  - Execute: deduct Tael, add Peon to SectData after 1 turn delay

BuildBuildingCommand : Command
  - Cost: from BuildingConfigSO tier cost
  - Valid: during Build phase, prerequisites met, build slot available
  - Execute: deduct resources, add to BuildQueue
```

## Building Config Data (from GDD §6.3)

| Building | Prereq | T1 Cost | T1 Turns | T2 Cost | T2 Turns | T3 Cost | T3 Turns |
|----------|--------|---------|----------|---------|----------|---------|----------|
| Temple | None (founded) | 0 | 0 | 100T 20Qi 10L | 10 | 300T 50Qi 30L 5J | 20 |
| Training Grounds | Temple T1 | 50T 5Qi 5L | 8 | 120T 15Qi 15L | 12 | 250T 30Qi 25L 2Fe | 18 |
| Disciple Hall | Training Grounds T1 | 80T 10Qi 8L 2Fe | 10 | 180T 25Qi 18L 5Fe | 15 | 350T 50Qi 30L 10Fe | 22 |
| Library | Disciple Hall T1 | 100T 15Qi 10L | 12 | 200T 35Qi 20L 3J | 18 | 400T 70Qi 35L 8J | 25 |
| Elder Council | Library T2 | 200T 40Qi 15L 5J | 20 | 400T 80Qi 30L 10J | 30 | 800T 150Qi 50L 20J 5SH | 45 |
| External Affairs Hall | Temple T1 | 60T 8Qi 8L | 8 | 140T 20Qi 15L | 12 | 280T 40Qi 25L 3J | 18 |
| Medicine Hall | Training Grounds T1 | 70T 10Qi 5L 5MH | 10 | 150T 25Qi 12L 10MH | 15 | 320T 50Qi 20L 15MH 3SH | 22 |
| Armory | Training Grounds T1 | 80T 12Qi 8L 5Fe | 10 | 160T 25Qi 15L 12Fe | 15 | 340T 45Qi 25L 20Fe | 22 |
| Market Pavilion | External Affairs Hall T1 | 90T 10Qi 10L | 10 | 180T 20Qi 18L | 14 | 360T 40Qi 30L | 20 |
| Branch Sect Outpost | Elder Council T1 | 300T 50Qi 30L 10Fe 5J | 25 | — | — | — | — |

## Integration Points

### SectManager changes:
- Add BuildQueue reference
- Add TrainingQueue reference
- Add BuildingFactory reference
- During Build phase: call BuildQueue.ProcessBuildPhase() and TrainingQueue.ProcessBuildPhase()
- On build complete: call BuildingFactory.CreateBuilding()
- On training complete: promote disciple in SectData

### TurnController changes:
- BuildQueue and TrainingQueue advance during Build phase
- Auto-advance non-critical phases for test harness (existing pattern from M3/M4)

### Event channels needed:
- EC_OnBuildingCompleted (StringEventChannelSO — buildingTypeId)
- EC_OnDiscipleTrained (StringEventChannelSO — discipleName)
- EC_OnPeonRecruited (VoidEventChannelSO)

## File Manifest

New files:
```
Assets/_Game/Scripts/Sects/DiscipleData.cs
Assets/_Game/Scripts/Sects/BuildQueue.cs
Assets/_Game/Scripts/Sects/TrainingQueue.cs
Assets/_Game/Scripts/Sects/BuildingFactory.cs
Assets/_Game/Scripts/Sects/BuildingConfigSO.cs
Assets/_Game/Scripts/Sects/RecruitPeonCommand.cs
Assets/_Game/Scripts/Sects/BuildBuildingCommand.cs
Assets/_Game/Scripts/UI/SectOverviewScreen.cs
Assets/_Game/Scripts/UI/SectOverviewScreen.uxml
Assets/_Game/Scripts/UI/SectOverviewScreen.uss
Assets/_Game/Tests/M5PlayModeTest.cs
```

Modified files:
```
Assets/_Game/Scripts/Sects/SectManager.cs (add queue/factory refs, phase handling)
Assets/_Game/Scripts/Sects/SectData.cs (add disciple list, building list)
Assets/_Game/Scripts/Core/TurnSystem/TurnController.cs (build phase integration)
```

## Test Plan

### M5 Verify PlayMode Test:
1. Start game, found sect
2. Recruit 3 peons (verify Tael deduction, peon count = 3)
3. Queue Training Grounds construction (verify resource deduction, timer starts)
4. Advance 10 turns through Build phases
5. Verify Training Grounds building appears on map
6. Verify Outer Disciple spawns (via TrainingQueue auto-promote or manual)
7. Verify HUD updates

### EditMode Tests:
- BuildQueue: enqueue, process, cancel
- TrainingQueue: enqueue, process, cancel
- BuildingFactory: create, validate prerequisites
- DiscipleData: calculate stats per rank
- ResourceCost: validate costs match GDD tables
