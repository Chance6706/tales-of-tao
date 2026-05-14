# Tales of the Tao — Refined Development Plan v2

**Generated:** 2026-05-13
**Based on:** GDD v2.0 (1976 lines, §1–§19) + current codebase audit + Unity best practices research
**Methodology:** Vertical slice per GDD §18 M0–M14, with architecture-first approach

---

## Changes from v1

This update incorporates research from:
- Unity 6 best practices (ScriptableObject architecture, DOTS hybrid, URP optimization)
- Game programming patterns (Command, Observer, State Machine, Object Pool, Data Locality)
- 4X strategy game architecture (Civ VI, Stellaris, Endless Legend, Mount & Blade analysis)
- Blender-to-Unity asset pipeline best practices
- Indie art direction and small-team production strategies

Key changes:
1. **Added §18 Technical Implementation Notes** to GDD (architecture patterns, performance budgets, asset pipeline)
2. **Refined milestone estimates** based on pattern complexity analysis
3. **Added architecture validation gates** between milestones
4. **Expanded risk register** with DOTS spike decision framework
5. **Added art pipeline milestones** integrated with gameplay milestones
6. **Added performance budgets** per system
7. **Added testing requirements** per milestone

---

## Current State Assessment

### What Exists (M0 + M1 partially complete)

| Area | Status | Notes |
|------|--------|-------|
| Unity project scaffold | DONE | URP, Input System, Test Framework packages configured |
| Core architecture | DONE | Namespace structure, .asmdef files, .editorconfig |
| Event channels | DONE | Generic `EventChannelSO<T>`, Void, Int, String, GamePhase variants |
| Command pattern | DONE | `Command` base + `CommandQueue` with redo support |
| Object pool | DONE | Generic `ObjectPool<T>` + `GameObjectPool` wrapper |
| Hex math | DONE | `HexCoords` (readonly struct, axial coords, operators, `IEquatable`) |
| Hex tile data model | DONE | `HexTileData`, `HexTile`, `TerrainTypeSO`, enums |
| Unit data model | DONE | `SectTier`, `UnitDataSO`, `UnitController`, `UnitRegistrySO` |
| Building data model | DONE | `BuildingDataSO`, `BuildingController` |
| UI placeholder | DONE | `TileInfoPanel` (TMP_Text-based) |
| Editor tools | DONE | `ImportSettingsValidator`, `ProjectSetupWizard`, `SceneSetupHelper` |
| Unit tests | DONE | `CommandQueueTests` (10), `HexCoordsTests` (12) |
| Module stubs | DONE | AI, Combat, Diplomacy, Research, SaveLoad, Sects README files |

### What's Missing (by system)

| System | GDD § | Priority | Architecture Pattern |
|--------|-------|----------|---------------------|
| Map generation pipeline | §4.3 | CRITICAL | 7-pass pipeline, chunk mesh combining |
| Turn state machine + Zodiac | §3.1 | CRITICAL | State pattern, event-driven |
| Sect founding + management | §5, §6 | CRITICAL | Command pattern, SO data |
| Camera controller | §15 | HIGH | Spherical coordinates orbit |
| Fog of War | §17.4 | HIGH | Shadowcasting Burst job |
| Pathfinding (A* Jobs + Burst) | §17.4 | HIGH | A* with NativeArray |
| Disciple training + promotions | §6.2 | HIGH | Queue system, ratio enforcement |
| Building system + UI | §6.3 | HIGH | SO config, build queue |
| Economy (Tael, Qi, commodities, market) | §7 | HIGH | Simulation, per-compound pricing |
| Research + Enlightenment triggers | §8 | HIGH | Tech tree, event-driven triggers |
| Combat (auto-resolve + CP formula) | §9 | HIGH | Pure function, deterministic |
| Diplomacy + settlements | §10 | HIGH | Relation score system |
| Espionage | §11 | MID | Mission system, detection rolls |
| AI system (genomes, personalities) | §12 | MID | Utility-based, time-budgeted |
| Tactical combat | §13 | MID | Additive scene, 7×7 grid |
| Narrative / events | §14 | MID | Template resolver, zodiac-modulated |
| Spirit Beasts + Secret Realms | §14.2-3 | LOW | Event-driven spawns |
| Victory checker | §2 | LOW | Score calculation |
| Save/Load | §17 | LOW | JSON DTO, GZip |
| UI screens (8+ screens) | §15 | ONGOING | UI Toolkit (UXML/USS) |
| Audio | §16 | POLISH | Dynamic layered audio |
| VFX | §16 | POLISH | URP VFX Graph |
| DOTS/ECS integration | §17 | POLISH | Hybrid approach |

---

## Architecture Validation Gates

Before each milestone is considered complete, it must pass these validation checks:

### Gate 0 — Architecture Foundation (Pre-M2)
- [ ] All asmdef references verified (no circular dependencies)
- [ ] Event channel assets created for all inter-system communication
- [ ] Command pattern interface established (ICommand with Execute/Undo)
- [ ] Object pool stress-tested (1000 alloc/release cycles, no GC spikes)
- [ ] Unit test framework operational (EditMode tests run in <5s)

### Gate 1 — Data Flow (Post-M2)
- [ ] Map generation produces valid data for all 6 map sizes
- [ ] Hex grid data accessible via both MonoBehaviour and NativeArray (DOTS-ready)
- [ ] Chunk mesh combining reduces draw calls by >80% vs individual tiles
- [ ] Camera controller maintains 60fps on Epic map

### Gate 2 — Gameplay Loop (Post-M4)
- [ ] Full turn cycle (Event→Income→Build→Research→Action→Resolution) completes without errors
- [ ] Sect founding → disciple recruitment → building construction pipeline functional
- [ ] All economy formulas produce values within GDD §7.2.3 balance table ranges
- [ ] EconomyStressTest passes (1000 iterations, median Tael at T15 > 50)

### Gate 3 — Performance (Post-M6)
- [ ] Pathfinding A* completes in <16ms for 1000-tile path on Epic map
- [ ] DOTS spike evaluation complete (go/no-go decision documented)
- [ ] AI turn time benchmark: <500ms for 10 sects on Epic map
- [ ] Memory profile: <1GB total, <32MB code

### Gate 4 — Content Complete (Post-M13)
- [ ] All 10 sect configs created and balanced
- [ ] All ~35 tech nodes created with Enlightenment triggers
- [ ] All 6 commodity types with full supply chain
- [ ] All 9 event types with zodiac modulation
- [ ] Full playthrough (T1→T80) without critical bugs

### Gate 5 — Ship Ready (Post-M14)
- [ ] Save/load round-trip verified (save at T50, load, all state intact)
- [ ] Tutorial overlays functional
- [ ] Audio system integrated
- [ ] VFX pass complete
- [ ] Final integration test: full playthrough T1→T150

---

## Refined Milestone Plan (M0–M14)

### M0 — Project Scaffold
**Status: COMPLETE**
**GDD Verify:** Unity opens without errors; blank blue scene renders at target resolution

Deliverables:
- Unity 6 project with URP, Input System, Test Framework
- Folder structure (`Assets/_Game/Scripts/`, `Art/`, `Data/`, `Prefabs/`, `Scenes/`)
- `.asmdef` assemblies (Runtime per-module, Editor, Tests)
- `.editorconfig` (4-space, file-scoped namespaces)
- `.gitignore` (Unity standard)

---

### M1 — Infrastructure
**Status: COMPLETE**
**GDD Verify:** Publish a phase-changed event; subscriber receives it; pool allocates without GC

Deliverables:
- `EventChannelSO<T>` + typed variants (Void, Int, String, GamePhase)
- `Command` base class + `CommandQueue` (undo/redo)
- `ObjectPool<T>` + `GameObjectPool`
- `GameManager` (singleton guard, event channel references, validation)
- EditMode tests for core systems

---

### M2 — Hex Grid
**Status: NEAR COMPLETE — needs CameraController, Fog of War, chunk rendering**
**GDD Verify:** Small grid generates; click any tile; data shows in context panel; camera controls work

Remaining work:

| Task | Effort | Notes | Pattern |
|------|--------|-------|---------|
| `HexGridManager` — map generation pipeline (7 passes per §4.3) | 3 days | Elevation noise, biome Voronoi, Sacred Peak enforcement, Ley Line A*, starting location, settlement seeding, resource deposits | Pipeline pattern |
| `CameraController` — isometric 3/4, rotatable, zoomable | 1 day | Clamp bounds to map edges; spherical coordinates orbit | Strategy camera pattern |
| Chunk system (16×16 hex chunks, combined mesh) | 2 days | `ChunkRenderer` MonoBehaviour; dirty-flag rebuild; LOD support | Chunk rendering pattern |
| Fog of War (shadowcasting, visibility states) | 2 days | Bresenham hex FOV; runs as Burst job on unit move | Observer + Burst job |
| Tile highlighting (reachable tiles, territory overlay) | 1 day | Uses `TileSelector` + pathfinder integration | Decorator pattern |
| **Art: Hex tile meshes** (6 terrain types) | 2 days | Low-poly with hand-painted textures; vertex color support | Modular assets |
| **Art: Basic building prefabs** (3-5 types) | 1 day | Placeholder geometry, sect-colored materials | Modular kit |

**Estimated remaining: ~12 person-days** (was 9, added art pipeline)

**Performance Gate**: Chunk mesh combining must reduce draw calls by >80% vs individual tile meshes. Camera must maintain 60fps on Epic map (16,000 tiles).

---

### M3 — Turn System
**Status: NOT STARTED**
**GDD Verify:** Press End Turn; all 6 phases cycle; zodiac label updates

Deliverables:

| Task | Effort | Notes | Pattern |
|------|--------|-------|---------|
| `TurnStateMachine` (6 `ITurnState` implementations) | 2 days | Event→Income→Build→Research→Action→Resolution; event-driven transitions | State pattern |
| `ZodiacCalendar` (12-year cycle, bonus struct) | 1 day | `ZodiacBonusesEventChannel` SO broadcast | Observer pattern |
| HUD strip (phase indicator, End Turn button, resource counters) | 2 days | UXML + USS (UI Toolkit); wuxia-themed styling | UI Toolkit |
| Phase-aware input controller | 1 day | Blocks actions outside appropriate phases; input context switching | Command pattern |
| AI turn execution in Resolution Phase | 2 days | Time-budgeted per §12.2; `AIBudgetScheduler` with Job System | Utility AI |
| **Art: HUD assets** (icons, banners, zodiac symbols) | 1 day | 12 zodiac icons, resource icons, phase indicators | 2D art |

**Estimated: ~9 person-days** (was 8, added art)

**Testing Gate**: Full turn cycle test (PlayMode) — all 6 phases execute in correct order, events fire, AI executes within budget.

---

### M4 — Sect Founding
**Status: NOT STARTED**
**GDD Verify:** Founder moves to Mountain tile; Found Sect; Temple appears; Qi income shows in HUD

Deliverables:

| Task | Effort | Notes | Pattern |
|------|--------|-------|---------|
| `SectConfigSO` (10 canonical + custom creator data) | 1 day | Affinity, trait, unique hall per §5.1 | SO data pattern |
| `SectData` plain C# class (runtime state) | 1 day | FoundingTileStats, compounds[], stockpile, command queue | Plain C# data |
| `SectManager` (income processing, compound management) | 2 days | Processes Tael/Qi income, upkeep, budget calculations | Facade pattern |
| `FoundSectCommand` (undoable action) | 0.5 days | Creates SectData, places Temple, removes Founder from roster | Command pattern |
| Founder + Support Unit creation flow | 1 day | Unit instantiation, valid founding tile validation | Factory pattern |
| Temple placement prop | 0.5 days | Uses existing `BuildingController` + `BuildingDataSO` | — |
| Qi income calculation + HUD display | 0.5 days | `BaseQiIncome + CaveBonus` per §6.1 | Observer pattern |
| 6× `SectConfigSO` data assets (start with 6 of 10) | 0.5 days | Per §5.1 table | Data authoring |
| **Art: Sect banner emblems** (6 sects) | 1 day | 64×64 icons, color-coded per sect palette | 2D art |
| **Art: Temple model** (T1 placeholder) | 1 day | Low-poly pagoda, sect-colored materials | Modular asset |

**Estimated: ~9 person-days** (was 7, added art + data assets)

**Testing Gate**: Sect founding end-to-end PlayMode test. EconomyStressTest passes (1000 iterations, median Tael at T15 > 50).

---

### M5 — Disciples & Buildings
**Status: NOT STARTED**
**GDD Verify:** Recruit 3 peons; build Training Grounds (10-turn timer); Outer Disciple spawns on completion

Deliverables:

| Task | Effort | Notes | Pattern |
|------|--------|-------|---------|
| `DiscipleData` plain C# class (stats, rank, techniques, traits) | 1 day | Rank enum, stat calculations per §6.2 | Plain C# data |
| `BuildQueue` (construction timers, building factory) | 1.5 days | Simultaneous build limit = Temple tier | Queue pattern |
| `TrainingQueue` (promotion timers, resource deduction) | 1.5 days | Peon per §6.2.1 table | Queue pattern |
| `BuildingFactory` (creates building instances from `BuildingConfigSO`) | 1 day | 11 building types × 3 tiers | Factory pattern |
| `SectOverviewScreen` (UI) | 3 days | Building status, disciple roster, resource summary, Dissent meter per §15.3 | UI Toolkit |
| Management ratio enforcement + Dissent | 1 day | Soft penalty per §6.2: +2 Dissent/turn per 10% excess | Observer pattern |
| `BuildingConfigSO` data assets (all 11 types × 3 tiers) | 1 day | Costs per §6.3 table | Data authoring |
| **Art: Building models** (5 additional types) | 2 days | Low-poly, modular, sect-colored | Modular assets |
| **Art: Disciple unit models** (3 ranks) | 2 days | Outer/Inner/Elder silhouettes, sect-colored robes | Character pipeline |

**Estimated: ~14 person-days** (was 10, added art pipeline)

---

### M6 — Units & Movement + DOTS Spike
**Status: NOT STARTED**
**GDD Verify:** Select Outer Patrol; reachable tiles highlight; unit moves along A* path respecting terrain costs

Deliverables:

| Task | Effort | Notes | Pattern |
|------|--------|-------|---------|
| `UnitData` plain C# class (position, stats, rank, morale, inventory) | 1 day | Maps to `UnitController` MonoBehaviour | Plain C# data |
| `Unit` MonoBehaviour (extends `UnitController`) | 0.5 days | Adds selection ring, sprite billboard | Component pattern |
| `HexPathfinder` (A* Jobs + Burst) | 3 days | Per §17.4 spec; movement budget; terrain costs; ZOC penalty | A* + Burst job |
| `MoveUnitCommand` (undoable movement) | 0.5 days | Validates path, deducts movement, raises `OnUnitMoved` event | Command pattern |
| Reachable tile highlight system | 1 day | Calls pathfinder for all directions; caches result during Action Phase | Decorator pattern |
| Army stack visualization | 1 day | Multiple units on same tile shown as stack summary | Composite pattern |
| **DOTS Spike** (evaluate ECS conversion) | 3 days | Convert hex grid data to ECS components; benchmark pathfinding; go/no-go decision | DOTS/ECS |
| **Art: Unit VFX** (selection rings, movement trails) | 1 day | URP particle effects, sect-colored | VFX |

**Estimated: ~11 person-days** (was 7, added DOTS spike + art)

**Performance Gate**: Pathfinding A* completes in <16ms for 1000-tile path on Epic map. DOTS spike decision documented.

---

### M7 — Combat
**Status: NOT STARTED**
**GDD Verify:** Two opposing units adjacent; attack; result panel shows correct CP and casualties

Deliverables:

| Task | Effort | Notes | Pattern |
|------|--------|-------|---------|
| `CombatPowerCalculator` (pure function per §9.3 formula) | 1.5 days | CP = f(unit stats, technique bonuses, terrain, equipment, Elder auras) | Pure function |
| `CombatResolver` (auto-resolve) | 2 days | Deterministic with seeded random per §17.6 | Strategy pattern |
| `AttackCommand` (undoable) | 0.5 days | Validates adjacency, resolves combat, applies casualties | Command pattern |
| `CombatResultPanel` (UI) | 1.5 days | Shows CP comparison, casualties, Face/Renown updates | UI Toolkit |
| Face + Renown updates on combat outcome | 0.5 days | Per §9.1 Face-Slap bonus formula | Observer pattern |
| Unit HP tracking + dead state | 0.5 days | `UnitStatsComponent` HP field, death cleanup | State pattern |
| **Art: Combat VFX** (qi bursts, impact effects) | 1.5 days | URP VFX Graph, sect-colored | VFX |

**Estimated: ~8 person-days** (was 6.5, added art)

**Testing Gate**: CombatPowerCalculator unit tests (100% coverage). Auto-resolve deterministic test (same seed → same result).

---

### M8 — Economy & Market
**Status: NOT STARTED**
**GDD Verify:** Trade route established; Tael increases next Income Phase; buy 3 Iron Ore; price inflates 30%

Deliverables:

| Task | Effort | Notes | Pattern |
|------|--------|-------|---------|
| `EconomyManager` (income/upkeep calculation per §7.2) | 2 days | Tael sources, sinks, net flow per phase | Facade pattern |
| `TradeRoute` system (establish, income formula per §7.2.1) | 1.5 days | Distance multiplier, commodity differential | Strategy pattern |
| `MarketSimulator` (per-compound pricing per §7.5) | 2 days | Base prices, 5%/turn drift, buy inflation, sell deflation | Simulation |
| `MarketScreen` (UI) | 2 days | Buy/sell, quantity stepper, price history per §15.3 | UI Toolkit |
| Bankruptcy handler + grace period | 1 day | 3-turn grace, then Peon desertions per §7.2.3 | State pattern |
| Anti-inflation mechanics | 0.5 days | Luxury upkeep scaling, §12.1 per §7.2.4 | Observer pattern |
| `EconomyStressTest` EditMode test | 0.5 days | 1,000 random iterations, assert median Tael > 50 at T15 | Stress test |
| **Art: Market/Trade icons** | 0.5 days | Commodity icons, trade route visualization | 2D art |

**Estimated: ~10 person-days** (was 9.5, added art)

**Testing Gate**: EconomyStressTest passes (1000 iterations, median Tael at T15 > 50, <5% bankruptcy before T20).

---

### M9 — Research
**Status: NOT STARTED**
**GDD Verify:** Queue Herbal Medicine; Enlightenment Trigger fires; research completes at 50% speed; Renown +5

Deliverables:

| Task | Effort | Notes | Pattern |
|------|--------|-------|---------|
| `TechNodeSO` ScriptableObject (all ~35 nodes) | 2 days | 3 branches (Alchemy, Forge, Martial); costs per §8 | SO data pattern |
| `TechTree` (cross-reference validation, prerequisite chains) | 1 day | Tier 1–4 per branch | Graph validation |
| `ResearchManager` (queue up to 3, progress per turn) | 1.5 days | Qi × 0.40 + Library × 3 + WisdomElder × 4 + LeyLine modifier | Strategy pattern |
| `EnlightenmentTriggerSystem` | 1.5 days | Subscribes to game events, fires 50% bonus per §8.1 | Observer pattern |
| `ResearchScreen` (UI) | 3 days | 3 side-by-side panels, Bezier prerequisite arrows, progress rings per §15.3 | UI Toolkit |
| ~35× `TechNodeSO` data assets | 1 day | Per §8 branch tables | Data authoring |
| **Art: Tech tree UI assets** | 1.5 days | Branch backgrounds, node icons, connection lines | 2D art |

**Estimated: ~11.5 person-days** (was 10, added art)

---

### M10 — Diplomacy & Espionage
**Status: NOT STARTED**
**GDD Verify:** Send gift to settlement; trust reaches Friendly; deploy spy; Gather Intel completes; buildings revealed

Deliverables:

| Task | Effort | Notes | Pattern |
|------|--------|-------|---------|
| `SettlementData` (trust tier, resources, disposition) | 1 day | Village/Town/Trade Post types | Plain C# data |
| `DiplomacyManager` (relations, treaties, gifts) | 2 days | Trust tiers, trade route diplomacy, AI §10 behavior | Facade pattern |
| `EspionageSystem` (deploy spy, missions, detection) | 2 days | Gather Intel, Assassinate, Sabotage per §11; detection chance −10% per Hall | Strategy pattern |
| `DiplomacyScreen` (UI) | 2 days | Sects + Settlements tabs per §15.3 | UI Toolkit |
| Settlement AI (trust drift, resource valuation) | 1 day | Trust gain/loss from gifts, broken treaties, Face modifier | Utility AI |
| Independent settlement spawning | 0.5 days | Reuse existing settlement seeding from map gen | — |
| **Art: Diplomacy UI assets** | 1 day | Relation icons, trust bar, settlement portraits | 2D art |

**Estimated: ~9.5 person-days** (was 8.5, added art)

---

### M11 — Tactical Combat
**Status: NOT STARTED**
**GDD Verify:** Battle triggers; tactical view loads; 2 rounds execute; retreat applies correct casualties to strategic map

Deliverables:

| Task | Effort | Notes | Pattern |
|------|--------|-------|---------|
| Tactical scene (additive load, 7×7 grid) | 2 days | New scene or overlay; camera reposition | Additive scene |
| `TacticalBattle` (placement + round resolution) | 2 days | State machine per §13: Placement→Running→Ended | State pattern |
| `DuelResolver` (stat + technique comparison) | 1 day | Per simplified GDD (no RPS triangle); random variance | Strategy pattern |
| `MoraleSystem` (break threshold, routing) | 1 day | Morale = base HP ratio + technique bonuses + terrain | State pattern |
| Retreat mechanic (casualty application to strategic map) | 1 day | Different from death: units survive but damaged | Command pattern |
| Duel-triggering conditions in auto-resolve | 0.5 days | Same-tile same-rank or challenged 1-rank-higher per §9.2 | Observer pattern |
| **Art: Tactical grid + terrain tiles** | 1.5 days | 7×7 hex grid, terrain-specific tiles | 2D/3D art |

**Estimated: ~9 person-days** (was 7.5, added art)

---

### M12 — AI & Narrative
**Status: NOT STARTED**
**GDD Verify:** 1 Expansionist AI builds Training Grounds within 15 turns; unique Wandering Master flavor text generates correctly

Deliverables:

| Task | Effort | Notes | Pattern |
|------|--------|-------|---------|
| `AIGenome` (8-trait weight vector, 6 personalities) | 1.5 days | §12.1 trait definitions | SO data pattern |
| `AIController` (strategic + tactical evaluation) | 3 days | Utility scoring, action selection, strategic cache per §12.2 | Utility AI |
| `GenomeDriftSystem` (generational evolution) | 1 day | Trait weight mutation on era transition | Observer pattern |
| `NarrativeTemplateResolver` (parameterized text generation) | 1.5 days | Flavor text from disciple/sect/event state | Template pattern |
| `EventScheduler` (9 event types, zodiac-modulated) | 2 days | Flat probability tables with §4 zodiac weighting | Strategy pattern |
| 1 Expansionist AI (minimum viable AI for Verify) | 0.5 days | Same as AIController with Expansionist genome preset | — |
| AI turn time budget test | 0.5 days | Assert <500ms for 10 sects on Epic map | Performance test |
| **Art: Event illustrations** (key events) | 1 day | 5-8 key event images, wuxia painting style | 2D art |

**Estimated: ~11 person-days** (was 10, added art)

**Performance Gate**: AI turn time benchmark on Epic map with 10 sects <500ms. If >500ms, profile and optimize top-2 hotspots.

---

### M13 — Content & Balance
**Status: NOT STARTED**
**GDD Verify:** 3 AI sects; play to T80; one sect achieves a victory condition; Victory screen displays correctly

Deliverables:

| Task | Effort | Notes | Pattern |
|------|--------|-------|---------|
| Remaining 4× `SectConfigSO` data assets | 1 day | Wu Dang, Shaolin, Peng Clan, Namgung remaining from M4 | Data authoring |
| All 7 commodity types (data + integration) | 1 day | Iron Ore, Jade, Lumber, Medicinal Herbs, Spirit Herbs, Tea Leaves, + High-Grade Iron flag | Data authoring |
| `VictoryChecker` (Domination/Enlightenment/Influence + turn 150 score) | 1 day | Per §2 table + score formula | Strategy pattern |
| Near-Victory alert ("Heavens Tremble") | 0.5 days | +30% aggression for 10 turns per §2 | Observer pattern |
| `VictoryScreen` (UI) | 1.5 days | Score breakdown, replay option, session narrative summary | UI Toolkit |
| Balance pass on economy tables | 2 days | Validate §7.2.3 balance table with playtesting | Playtesting |
| Balance pass on combat CP formula | 1 day | Ensure no single technique dominates | Playtesting |
| M8 balance validation stress test | 0.5 days | Run `EconomyStressTest` with tuned values | Stress test |
| **Art: Victory screen + endgame UI** | 1 day | Victory/defeat screens, score display | 2D art |

**Estimated: ~9.5 person-days** (was 8.5, added art)

---

### M14 — Polish, Save/Load, Performance
**Status: NOT STARTED**
**GDD Verify:** Save at T50; quit; load; all state intact. Epic map with 6 AI sects runs at <50ms per AI turn

Deliverables:

| Task | Effort | Notes | Pattern |
|------|--------|-------|---------|
| `GameStateDTO` (serializable state snapshot) | 1 day | Plain C# DTO with all game data | DTO pattern |
| `SaveLoadManager` + `SaveLoadRepository` | 2 days | JSON serialization; delta compression; cloud save stub | Repository pattern |
| Tutorial overlays (first-play experience) | 2 days | Context-sensitive callouts per §15 design principles | UI Toolkit |
| `AudioManager` + `AudioMixerGroup` setup | 1.5 days | Dynamic layered audio per §16.2 | Observer pattern |
| VFX pass (Qi streams, zodiac ambient, combat effects) | 2 days | URP VFX Graph; sect colors per §16.1 | VFX |
| Performance pass (DOTS spike, LOD, chunk culling) | 3 days | §17.3 draw call budgets; §17.4 chunk system; §17.6 adaptive tick rates | Optimization |
| DOTS spike decision (Risk 1 mitigation per GDD) | During M2-M6 | If DOTS spike fails, fall back to MonoBehaviour + Jobs | Architecture |
| Final integration testing | 2 days | Full playthrough; edge cases; save/load round-trip | Integration test |
| **Art: Final art pass** | 2 days | Replace placeholders, polish materials, add environmental detail | Art polish |

**Estimated: ~17.5 person-days** (was 15.5, added art)

---

## Total Effort Summary

| Milestone | Status | Est. Person-Days | Change from v1 |
|-----------|--------|-----------------|----------------|
| M0 — Project Scaffold | COMPLETE | — | — |
| M1 — Infrastructure | COMPLETE | — | — |
| M2 — Hex Grid | NEAR COMPLETE | 12 remaining | +3 (art pipeline) |
| M3 — Turn System | NOT STARTED | 9 | +1 (art) |
| M4 — Sect Founding | NOT STARTED | 9 | +2 (art + data) |
| M5 — Disciples & Buildings | NOT STARTED | 14 | +4 (art pipeline) |
| M6 — Units & Movement + DOTS Spike | NOT STARTED | 11 | +4 (DOTS spike + art) |
| M7 — Combat | NOT STARTED | 8 | +1.5 (art) |
| M8 — Economy & Market | NOT STARTED | 10 | +0.5 (art) |
| M9 — Research | NOT STARTED | 11.5 | +1.5 (art) |
| M10 — Diplomacy & Espionage | NOT STARTED | 9.5 | +1 (art) |
| M11 — Tactical Combat | NOT STARTED | 9 | +1.5 (art) |
| M12 — AI & Narrative | NOT STARTED | 11 | +1 (art) |
| M13 — Content & Balance | NOT STARTED | 9.5 | +1 (art) |
| M14 — Polish, Save/Load, Performance | NOT STARTED | 17.5 | +2 (art) |
| **TOTAL** | | **~141 person-days** | **+24 (art pipeline)** |

At 2 developers working full-time (~20 person-days/week), this is approximately **7 weeks of focused development** (was 6 weeks in v1).

---

## Critical Path

The longest dependency chain runs:

**M2 (Hex Grid + Pathfinding) → M3 (Turn System) → M4 (Sect Founding) → M5 (Disciples/Buildings) → M6 (Units/Movement + DOTS Spike) → M7 (Combat) → M12 (AI) → M13 (Balance)**

Parallel tracks that can proceed independently:
- **M8 (Economy)** can start after M4 (Sect Founding) — needs SectData but nothing else
- **M9 (Research)** can start after M4 — needs Hall building types from M5 but data can be authored in parallel
- **M10 (Diplomacy)** can start after M4 — needs SettlementData (created in map gen M2)
- **M11 (Tactical Combat)** can start after M7 — needs CP formula
- **M14 (Save/Load)** can start at any time — reads from all systems but doesn't block them
- **Art pipeline** runs in parallel with all milestones

---

## Risk Register (from GDD §19, updated)

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| DOTS integration complexity | HIGH | HIGH | 2-week spike at M6; fallback to MonoBehaviour + Jobs. Decision gate at M6. |
| Economy death spiral | MEDIUM | HIGH | Starting Tael buffer (200); 3-turn bankruptcy grace; automated stress test at M8 |
| AI turn time on Epic maps | MEDIUM | MEDIUM | Spatial indexing; staggered evaluation; strategic layer caching; <500ms gate at M12 |
| Art pipeline bottleneck | MEDIUM | MEDIUM | Stylized realism reduces asset complexity; modular kit approach; placeholder-first development |
| Scope creep from GDD detail | MEDIUM | HIGH | Kill List enforced; deferred features clearly marked; weekly scope review |

---

## Art Pipeline Summary

### Art Style Decision
**Recommended**: Stylized Realinity (Civ 6-like)
- Clean silhouettes, hand-painted textures
- Bold color choices with limited palettes per area
- Achieves 80% AAA quality with 20% effort
- Ages better than photorealism for small teams

### Art Production Phases

**Phase 1** (M2-M4 — Core Gameplay):
- Hex tile meshes (6 terrain types) — low-poly, hand-painted
- Basic building prefabs (3-5 types) — placeholder geometry
- Unit placeholders (colored capsules with icons)
- UI framework (functional, wuxia-themed)
- Sect banner emblems (6 sects)

**Phase 2** (M5-M8 — Content):
- Detailed building models (all 11 types)
- Character models (3-4 variants: Outer/Inner/Elder)
- Combat VFX (qi bursts, impact effects)
- Terrain textures (detailed, tiling)
- Tech tree UI assets
- Diplomacy UI assets

**Phase 3** (M9-M14 — Polish):
- High-quality character models (all 10 sects)
- Advanced VFX (cultivation auras, realm effects)
- Animated UI elements
- Environmental storytelling props
- Event illustrations (5-8 key events)
- Victory/defeat screens

### Blender-to-Unity Pipeline
- **Units**: Metric, 1 BU = 1 Unity meter, Y-Up
- **Characters**: 2000-5000 tris (heroes), Rigify + custom bones, hand-painted PBR
- **Environment**: Modular kit approach, texture atlases for hex tiles
- **FBX Export**: Scale 1.0, Forward -Z, Up Y, Bake Animation, no leaf bones
- **LOD**: 100% → 50% → 25% triangle count, manual creation in Blender

---

## Immediate Next Actions

1. **Complete M2** — Implement `HexGridManager` map generation pipeline (7 passes per §4.3)
2. **Build M3** — `TurnStateMachine` + `ZodiacCalendar` + HUD strip
3. **Build M4** — `SectConfigSO` + `SectManager` + `FoundSectCommand` + Founder flow
4. **Create data assets** — All `TerrainTypeSO`, `BuildingConfigSO`, `SectConfigSO` assets in parallel with M4-M5
5. **Establish art pipeline** — Set up Blender project, create first hex tile + building placeholder

---

## Deferred to Post-Launch (Kill List)

Per GDD §19, the following are explicitly cut:
- Autonomous sub-sect governance (Branch Sect T3 full autonomy)
- Custom technique creation / Technique Fusion
- Weapon-type RPS triangle in duels
- Difficulty level 5 (Grandmaster) — folded into Master/Global
- Cloud AI evaluation (prepared in architecture, not implemented)
- Multiplayer (prepared in architecture, not implemented)
