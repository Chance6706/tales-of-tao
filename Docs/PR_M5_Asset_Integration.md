# PR: M5 Asset Integration

## Branch
`feature/m5-asset-integration` → `main`

## Summary
Completes the M5 asset integration milestone. All building/unit/terrain/sect data assets are configured with meshes, materials, and prefabs. The M5VisualTest scene loads and runs a full integration test (11/11 checks passing).

## What's Included

### Data Assets (configured from GDD data)
- **33 BuildingConfigSO** assets (11 building types × 3 tiers) — meshes, materials, costs, build times assigned
- **10 SectConfigSO** assets — all 10 canonical sects with affinities, traits, starting resources
- **8 TerrainTypeSO** assets — Plains, Forest, Mountain, Sacred Peak, River, Lake, Desert, Swamp
- **5 UnitDataSO** assets — T1 Labor Disciple through T5 Grand Patriarch
- **3 PresetMapData** assets — Jianghu, SpiritRealm, Wasteland

### Prefabs (copied to Resources/ for runtime loading)
- **11 building prefabs** in `Assets/Resources/Buildings/`
- **5 unit prefabs** in `Assets/Resources/Units/`
- **1 sect config** (SC_WuDang) in `Assets/Resources/Sects/`

### Editor Tools
- `M5AssetConfigurator.cs` — configures all M5 assets from GDD data (menu: TalesOfTao > Configure M5 Assets)
- `BuildingMeshMaterialAssigner.cs` — assigns meshes and materials to BuildingConfigSO assets
- `AssignBuildingMeshes.cs`, `AssignUnitMeshes.cs` — mesh assignment utilities
- `CreatePresetMaps.cs`, `PresetMapCreator.cs` — map generation tools

### Code Changes
- `M5VisualTest.cs` — updated to load prefabs from Resources, spawn peons as visible GameObjects, test sect recruitment with Tael deduction
- `RecruitPeonCommand.cs` — updated to spawn visual peon prefabs at founding tile
- `SectManager.cs` — Training Grounds completion now auto-enqueues peon training (8 turns → Outer Disciple)
- `FoundSectCommand.cs` — Temple building placement on founding tile
- `TurnTestHUD.cs` — build queue and training queue display
- Various event channel and turn system fixes

### Integration Test Results (11/11 passing)
- GridManager exists: PASS
- TileCount > 0: PASS
- Terrain variety: PASS
- All building prefabs spawn (11/11): PASS
- All unit prefabs spawn (5/5): PASS
- Sect disciple recruitment (3/3): PASS
- Sect peon count: PASS
- Sect Tael deducted: PASS
- TurnDriver exists: PASS
- TurnDriver initialized: PASS
- TurnDriver.TurnNumber accessible: PASS

## What's NOT Included (known gaps)
- No UI to start building construction (BuildBuildingCommand exists but no UI calls it)
- No UI to recruit peons (only auto-test flow)
- Building completion doesn't create visible building GameObjects
- Resource income/upkeep not wired to UI display
- Founder unit doesn't exist (GDD says Turn 1 should place a Founder)
- Support Unit for peon recruitment doesn't exist yet

These are M2/M4 player-facing UI gaps, not M5 asset integration gaps.

## Stats
- 428 files changed
- ~58,000 insertions, ~7,200 deletions
- 8 commits on branch
