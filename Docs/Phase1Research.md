# Phase 1 Research & Refinement — Decision Document

**Branch:** `feature/phase1-research`
**Date:** 2026-05-16
**Author:** OWL (research) → Josh (decisions)

This document presents research findings and draft decisions for the 5 key design questions identified in the Production Roadmap for Phase 1: Foundation, plus the formation system concept. Each section covers: what the GDD says, what shipped games do, what the codebase currently has, and a recommended decision with rationale.

**Status:** All 7 open questions resolved by Josh on 2026-05-16. Formation system approved. Decisions incorporated below.

---

## 1. Combat System Design

### GDD Specification (§9.3)
- Auto-resolve formula: `CP = (BaseAttack × TechDmg) + (BaseDefense × TerrainDef) + (BaseHP × HPMod) + (QiPower × TechEffectiveness) + ElderAura + ZodiacBonus + Equipment − MoralePenalty`
- Random factor: `Random.Range(0.85, 1.15)` applied to both sides
- WinRatio = AttackerCP / (AttackerCP + DefenderCP)
- Casualty%: `(1 − WinRatio) × 0.40` for attacker, `WinRatio × 0.40` for defender
- Tactical view: optional 7×7 hex grid, 5-round limit, duel system, morale system
- AI always auto-resolves

### What Shipped Games Do

**Civilization VI** (the benchmark):
- Damage formula: `damage = random(0.8, 1.2) × 30 × e^(strength_diff / 25)`
- At equal strength: 24-36 damage per hit (random)
- One-shot kill at +26 strength difference (probabilistic), guaranteed at +36
- Wounded units lose CS proportional to HP lost: `penalty = -10 × (lost_HP / 100)`
- Corps/Army system: combining units gives +10/+17 CS bonus but doesn't double/triple damage — it makes units more survivable
- Healing: 5 HP/turn (enemy territory), 10 (neutral), 15 (friendly), 20 (city)
- Auto-resolve is the default; tactical view doesn't exist in Civ VI (all auto-resolve)
- Key insight: Civ VI's combat is *simple but satisfying* because the numbers are legible and the randomness creates stories

**Total War: Three Kingdoms**:
- Real-time tactical battles with auto-resolve option
- Auto-resolve considers: unit health, lord level, hero abilities, unit count
- Players can choose to fight manually or auto-resolve each battle
- Key insight: TW3K's auto-resolve is a *strategic shortcut*, not the primary experience

**Age of Empires 4**:
- Formation-based combat with stances
- Unit counters (spear > cavalry > archer > spear)
- No auto-resolve — all combat is real-time tactical
- Key insight: AoE4's depth comes from formations and counters, not from complex formulas

### Current Codebase
- Zero combat code exists. No `CombatResolver`, no `CombatPowerCalculator`, no casualty system.
- `UnitController` exists but has no combat logic.
- `HexPathfinder` exists and can be used for retreat pathing.

### Decision: AUTO-RESOLVE + DECISIVE VICTORY + FORMATIONS

**For v1, implement auto-resolve only with decisive victory threshold. Defer tactical view to Phase 2.**

**Decisive victory (>2× CP differential):** Attacker claims tile + 1 adjacent tile. Simple conditional, high impact.

**Rationale:**
1. The GDD's auto-resolve formula is well-specified and can be implemented in ~200 lines of code
2. Tactical view is a massive scope increase (7×7 grid, unit placement, initiative, duels, morale tracking) — easily 2-3 weeks of work
3. Civ VI proves that auto-resolve-only combat can be deeply satisfying for a 4X game
4. The GDD already states "AI always auto-resolves" — this signals the tactical view is optional
5. The formula's randomness (±15%) creates emergent stories without requiring player micro
6. Decisive victory adds satisfying "crushing blow" moments with minimal code cost

**Implementation approach:**
- Create `CombatPowerCalculator.Calculate(UnitData, HexTileData, GameState)` as a pure static function (per GDD dev note)
- Create `CombatResolver.Resolve(attacker, defender, attackerTile, defenderTile)` that returns a `CombatResult` struct
- `CombatResult` contains: winner, attackerCasualties%, defenderCasualties%, retreatFlag, renounceChange, faceChange, isDecisiveVictory
- Use the existing `Random` wrapper (seeded for determinism in MP)
- Casualty distribution: implement `CasualtyDistributor` with the GDD's weight system (Peon 50%, Outer 30%, Inner 20%)
- Formation bonus: multiply CP by formation modifier (see Section 6)

---

## 2. Unit Movement Model

### GDD Specification (§9.2)
- Base movement: 3 hexes/turn for all units
- Terrain costs: defined per terrain type
- Zone of Control: +2 movement cost per adjacent enemy tile
- Roads: 0.5 movement cost (built by Peon Gangs, 5 Lumber, 3 turns)
- Lightness Art tech tree modifies movement (+1 to +3, ignore terrain, ignore ZOC, invisibility)
- Fog of War: cannot path through Hidden tiles

### What Shipped Games Do

**Civilization VI:**
- 1 unit per tile (1UPT) — no stacking (but formations allow grouping)
- Movement points: 2 MP base for most units, 3 for scouts/cavalry
- Terrain: Plains/Grassland = 1 MP, Forest/Rainforest = 2 MP (unless road), Hills = 2 MP, Rivers = extra cost
- ZOC: entering an adjacent tile stops movement (can't move through), but cavalry ignores ZOC
- Roads: reduce movement cost to 0.33 per tile
- Key insight: Civ VI's movement is *legible* — players can easily calculate "can I reach that tile?"

**Age of Empires 4:**
- Formation-based movement with stances (aggressive, defensive, patrol)
- Terrain affects speed but not in a hex-grid way
- Key insight: AoE4's movement is about *formation control*, not hex math

**Total War: Three Kingdoms:**
- Army-based movement with campaign map movement points
- Terrain affects movement cost and ambush chances
- Key insight: TW3K's movement creates *strategic decisions* about where to go

### Current Codebase
- `HexPathfinder` exists with A* pathfinding
- `HexTile` has terrain data
- No movement cost system, no ZOC system, no road system
- Units spawn at mouse position with no movement logic

### Decision: BASE 3 MP + TERRAIN + ZOC + RIVERS + ROADS

**For v1, implement: base 3 MP, terrain costs, ZOC, river crossing penalty, and roads. Defer Lightness Art movement bonuses to Phase 2.**

**River crossing:** +1 MP to cross a river (unless bridge/ford). Makes rivers meaningful defensive features without adding complexity.

**Rationale:**
1. The GDD's base movement of 3 hexes/turn is generous — most tiles reachable in 1-2 turns
2. ZOC is critical for strategic depth: creates "front lines" and makes positioning matter
3. Roads are important for the economy (supply chains) and create a meaningful Peon Gang action
4. Rivers as defensive features are a genre standard (Civ VI does this well)
5. The `HexPathfinder` already exists — just need to add a `MovementCostCalculator`

**Implementation approach:**
- Create `MovementCostCalculator.GetCost(HexTileData, UnitData)` → returns float cost
- Terrain cost table: Plains=1, Forest=2, Hills=2, Mountain=3, Desert=2, Swamp=3, Road=0.5
- River crossing: +1 MP (check for river between current and target tile)
- ZOC: check adjacent tiles for enemy units, add +2 per adjacent enemy
- Integrate with `HexPathfinder` to calculate reachable tiles
- Movement points: `UnitData.MovementPoints` (default 3, modified by tech)
- Roads: add `RoadComponent` to tiles, modify cost calculation
- Formation movement: all units in a formation move at the speed of the slowest unit

---

## 3. Research Tree Structure

### GDD Specification (§8)
- 3 branches: Alchemy (Medicine Hall), Forge (Armory), Martial Techniques (Library)
- ~35+ nodes total in full game
- Tier gating: T1-2 requires Hall T1+, T3 requires Hall T2+, T4 requires Hall T3+
- Research speed: `(QiYield × 0.40) + (LibraryTier × 3) + (WisdomElder × 4) × (1 + 0.10 × LeyLineCount)`
- Qi costs: T1=30, T2=80, T3=180, T4=400
- Enlightenment Triggers: 50% progress boost from specific in-game actions
- Up to 1 tech per branch simultaneously (3 total)

### What Shipped Games Do

**Civilization VI:**
- Linear tech tree with some branching
- ~100+ technologies across the full tree (6 eras)
- Eureka moments: 50% boost from specific actions (build 2 mines → Mining 50%)
- Key insight: Eureka system creates a feedback loop — research feels *earned*, not just waited for

**Stellaris:**
- Card-based research: draw 3 options per branch, pick 1
- Cards have weights based on empire traits
- Key insight: Card system creates *meaningful choices* — can't research everything

**Age of Wonders 4:**
- Tomes unlocked by realm traits and city buildings
- Research paid in Mana/turn
- Key insight: Infrastructure gating creates natural progression

### Current Codebase
- `ResearchState` exists as a placeholder (does nothing)
- `TechNodeSO` ScriptableObjects exist but are empty
- No `ResearchManager`, no research speed calculation, no Enlightenment system

### Decision: 10 NODES (3-4 PER BRANCH) + ENLIGHTENMENT TRIGGERS + QI SENSING

**For v1, implement 3-4 nodes per branch (10 total, all T1-T2), with Enlightenment Triggers and a new Qi Sensing sub-branch. Defer T3-T4 nodes to Phase 2.**

**Math rationale:** At baseline research speed (~15 Qi/turn), T1 (30 Qi) ≈ 5 turns, T2 (80 Qi) ≈ 12-15 turns. 10 nodes × ~10 turns average = ~100 turns to complete all T1-T2. That's a meaningful chunk of a 200-300 turn game without delaying T3-T4 content.

**Enlightenment Triggers are IN for v1.** They're the single highest-value research feature — they make research feel active instead of passive. Implementation is straightforward (event subscription + 50% progress boost).

**Qi Sensing is IN for v1.** A new reconnaissance sub-branch under Martial Techniques. Thematically perfect for wuxia (cultivators sensing qi signatures). Mechanically: extends fog of war vision range. Implementation is simple (modify visibility radius per unit/player).

**⚠️ BALANCE NOTE:** Qi Sensing needs careful cost/benefit tuning for long games. If it's too cheap, players will always take it first and the fog of war becomes meaningless. If it's too expensive, nobody will research it. Key levers:
- Qi cost (T1=30 is baseline, consider 40-50 for sensing)
- Vision radius (2 hexes for T1 feels right; don't go above 3)
- Active vs. passive (passive is always-on but weaker; active is stronger but costs Qi per use)
- Late-game scaling (T3-T4 sensing in Phase 2 could be game-breaking if not carefully tuned — consider hard caps on radius)

**Recommended v1 nodes:**

| Branch | Node 1 (T1) | Node 2 (T1) | Node 3 (T2) | Node 4 (T2) |
|--------|-------------|-------------|-------------|-------------|
| Alchemy | Herbal Medicine | Antidote Craft | Qi Restoration Pills | — |
| Forge | Iron Smelting | Basic Weapons | Steel Refinement | — |
| Martial | Basic Sword Arts | Basic Fist Arts | Basic Qi Circulation | Qi Awareness (sensing) |

**Qi Sensing sub-branch (Martial Techniques):**

| Tier | Node | Type | Effect | Qi Cost | Balance Notes |
|------|------|------|--------|---------|---------------|
| T1 | **Qi Awareness** | Passive | Reveal enemy units within 2 hexes of any friendly unit (even through fog) | 40 Qi | Higher than standard T1 (30) to prevent auto-first-pick. 2-hex radius is enough to be useful without breaking fog of war. |
| T2 | **Qi Pulse** | Active | Reveal all units/tiles within 4 hexes of a target unit for 1 turn. Cooldown: 5 turns. | 80 Qi + 10 Qi/use | Active cost prevents spam. Cooldown ensures tactical timing matters. |

**T3-T4 Qi Sensing (Phase 2, for reference):**
- T3: **Spirit Sense** — Passive: reveal resource deposits and cave types within 3 hexes of any friendly unit
- T4: **Heavenly Vision** — Active: reveal entire map for 1 turn. Cooldown: 20 turns. Cost: 50 Qi
- ⚠️ T4 is extremely powerful — consider making it a rare item/scroll instead of a research node

**Implementation approach:**
- Create `ResearchManager` with `ActiveResearch` array (3 slots, one per branch)
- Create `TechNodeSO` with: branch, tier, qiCost, enlightenmentTrigger, unlocks
- Research speed: implement the GDD formula directly
- Enlightenment Triggers: create `EnlightenmentTriggerSystem` that subscribes to game events
- Hall tier gating: check `BuildingController.GetHallTier(branch)` before allowing research
- UI: simple 3-column layout (one per branch), show progress bars, highlight available nodes

---

## 4. Founder Unit Design

### GDD Specification (§6.1)
- Game begins: player controls a **Founder Unit** and a **Support Unit** (5 Peons)
- Founder has 3 movement per turn
- Founder must move to a valid founding tile and execute "Found Sect"
- Founding requirements: 50 Tael, not within 8 hexes of another sect's Temple
- On founding: Founder is removed from unit roster, becomes Sect Leader NPC
- Founding tile bonuses are baked permanently into `SectData.FoundingTileStats`

### What Shipped Games Do

**Civilization VI:**
- Settler unit: 2 MP, consumed on founding a city
- Player starts with 1 Settler + 1 Warrior (to protect the Settler)
- Key insight: Civ VI's Settler creates *tension* — protect it while exploring, choosing where to found is the most important early decision

**Total War: Three Kingdoms:**
- Lord unit: army commander, can be killed in battle
- Key insight: TW3K's Lord creates *attachment* — named character with skills

**Age of Empires 4:**
- Scout unit: fast exploration, no combat
- Key insight: AoE4's Scout creates *information advantage*

### Current Codebase
- `FoundSectCommand` exists but just places the sect directly (no Founder unit)
- No Founder unit, no movement, no founding consumption
- `SectData` exists with founding tile stats

### Decision: MOVABLE FOUNDER + "CALL FOR AID" ACTION

**For v1, implement the Founder Unit as specified in the GDD, with a "Call for Aid" defensive action.**

**Founder has NO combat ability** (preserves tension), but gets a **"Call for Aid" action**: spawns a temporary Peon Gang defender or alerts nearby friendly units. This gives the Founder a Civ Settler++ feel — vulnerable but not helpless.

**Rationale:**
1. The GDD's design creates the right kind of early-game tension
2. The Founder becoming the Sect Leader NPC is a nice narrative touch
3. "Call for Aid" reduces frustration without removing vulnerability
4. This is a HIGH severity gap — the current "sect just appears" approach removes the most important early-game decision

**Implementation approach:**
- Create `FounderUnit` as a special `UnitDataSO` with 3 MP, no combat ability
- Add `CallForAidAction`: spawns temporary Peon Gang (lasts 3 turns, costs 10 Tael)
- Add `FoundSectCommand` validation: check 50 Tael, check 8-hex distance from other sects
- On founding: remove Founder from unit roster, create `SectData`, place Temple
- Support Unit: create a `PeonGang` unit with 5 Peons, no combat, can recruit from settlements
- UI: highlight valid founding tiles when Founder is selected

---

## 5. Network Architecture

### GDD Specification (§17)
- Multiplayer: up to 8 players
- Turn-based with simultaneous movement (except during war)
- Host migration support
- JSON-based save files for async/PBEM option

### What Shipped Games Do

**Civilization VI:**
- P2P with Steam relay servers, deterministic lockstep
- Simultaneous turns, conflicts resolved by unit strength
- Key insight: P2P means *no server costs* for 2-8 player turn-based

**Stellaris:**
- P2P with Paradox relay servers, deterministic lockstep
- Key insight: Proven to work for complex 4X with hundreds of entities

**Research from Longwelwind (Swords & Ravens developer):**
- Deterministic action propagation is best for turn-based games
- Only send actions (not state) — clients apply deterministically
- Key insight: "Implement networking only once, and be done with it"

**Research from Glenn Fiedler (Más Bandwidth):**
- Deterministic lockstep ideal for low player counts (4-8) with many units
- Requires determinism (seeded RNG, no float inconsistencies)
- Use relay server to prevent lag switch attacks
- Key insight: "For turn-based strategy with many units, deterministic lockstep is a no brainer"

### Current Codebase
- `TurnCoordinator` exists with server authority
- `ReadySystem` exists for phase sync
- `NetworkCommandQueue` exists for command ordering
- All multiplayer is local-only (no actual network transport)

### Decision: DETERMINISTIC ACTION PROPAGATION + UNITY NGO

**For v1, implement deterministic action propagation with Unity Netcode for GameObjects. Defer async/PBEM to Phase 3.**

**Unity NGO** is the transport layer. It's built-in, tested, handles relay servers. Custom transport is a rabbit hole — we can swap later if needed.

**Rationale:**
1. Deterministic action propagation is the most elegant approach for turn-based games
2. The existing `TurnCoordinator` + `NetworkCommandQueue` architecture is already designed for this
3. Only actions need to be sent (not state), minimizing bandwidth
4. Relay server prevents lag switch attacks
5. Unity NGO handles transport, relay, and NAT punch-through

**Implementation approach:**
- Network transport: Unity Netcode for GameObjects (NGO)
- Relay server: Unity Relay (built into NGO)
- Determinism: use `System.Random` (seeded), avoid `UnityEngine.Random`
- Action serialization: serialize `Command` objects to JSON, send over network
- Desync detection: compute game state hash each turn, compare across clients
- Host migration: store game state on relay, allow new host to download

**Key technical decisions:**
- Use `System.Random` (deterministic) instead of `UnityEngine.Random` (non-deterministic)
- All game logic must be deterministic: no `Time.deltaTime`, no `Mathf` inconsistencies
- Float math can cause desyncs across platforms → consider fixed-point math for critical calculations
- Network tick: process all player actions at the end of each phase (not real-time)

---

## 6. Formation System (New)

### Concept

A wuxia-themed army formation system inspired by Civ VI's Corps/Army and AoE4's formations. Units on the same tile can form named formations that provide combat bonuses and visual identity. Thematic fit: martial arts novels are full of formation techniques (剑阵, 阵法).

### Decision: INCLUDE IN V1

**For v1, implement a basic formation system with 3 formation types. Estimated cost: 4-5 days.**

### Formation Types

| Formation | Requirement | Bonus | Thematic |
|-----------|-------------|-------|----------|
| **Sword Formation** (剑阵) | 2+ Inner Disciple Squads | +15% attack, can challenge duels | Offensive strike formation |
| **Commander Formation** (将阵) | 1 Elder Champion + any units | +10% all stats, Elder aura extends to 3 hexes | Army commander leadership |
| **Scout Formation** (探阵) | 3+ Outer Patrols | +2 movement, fog of war reveal in 2-hex radius | Fast reconnaissance |

### Implementation Details

- **Formation as a verb:** Player selects units → "Form Sword Formation" → units merge into a single stack with formation icon
- **Formation movement:** All units move at the speed of the slowest unit
- **Formation + ZOC:** Formation exerts ZOC as 1 unit (occupies 1 tile)
- **Casualties:** If a formation takes casualties, it auto-disbands when below minimum unit count
- **Max units per formation:** 3 in v1 (scales with Elder rank in Phase 2)
- **Visual:** Distinct formation icon overlay on unit prefab (art-dependent)

### What's Easy
- Data model: `FormationType` enum, `FormationBonus` struct, `List<Unit>` on tile
- Combat bonus: one-line multiplier in `CombatPowerCalculator`
- UI: button on unit panel with dropdown of available formations

### What's Tricky
- Splitting formations mid-campaign: auto-disband on casualties
- AI formations: small utility function for AI to know when to form/disband
- Save/load: serialize formation as list of unit IDs + formation type

---

## Summary of Recommendations

| Topic | Recommendation | Complexity | Risk |
|-------|---------------|------------|------|
| Combat | Auto-resolve + decisive victory + formation bonus | Medium | Low |
| Movement | Base 3 MP + terrain + ZOC + rivers + roads | Medium | Low |
| Research | 10 nodes (3-4 per branch) + Enlightenment Triggers + Qi Sensing | Medium | Medium |
| Founder Unit | Movable unit + "Call for Aid" action | Low | Low |
| Networking | Deterministic action propagation + Unity NGO | High | Medium |
| Formations | 3 formation types (Sword, Commander, Scout) | Medium | Low |

## Recommended Implementation Order

1. **Founder Unit** (Week 2) — lowest risk, unblocks playtesting
2. **Unit Movement** (Week 2-3) — foundational for everything else
3. **Combat Auto-Resolve** (Week 3-4) — depends on movement (retreat pathfinding)
4. **Formation System** (Week 3-4, parallel with combat) — depends on combat calculator
5. **Research System** (Week 4-5) — can be developed in parallel
6. **Save/Load** (Week 5-6) — depends on all above systems being serializable
7. **Networking** (Phase 3) — requires save/load and all core systems

## Resolved Open Questions (8 total)

| # | Question | Decision |
|---|----------|----------|
| 1 | Decisive victory threshold? | **Yes**, include in v1 |
| 2 | River movement penalty? | **Yes**, +1 MP to cross |
| 3 | Enlightenment Triggers in v1? | **Yes**, include in v1 |
| 4 | Founder combat ability? | **No combat**, but "Call for Aid" action |
| 5 | NGO vs custom transport? | **Unity NGO** for v1 |
| 6 | Research nodes per branch? | **3-4 per branch (10 total)** — Martial gets 4th node: Qi Awareness (sensing) |
| 7 | Map size target? | **~4,000 tiles** (60×60 hex grid) |
| 8 | Qi Sensing ability? | **Yes** — T1 Qi Awareness (passive, 2-hex fog reveal, 40 Qi) + T2 Qi Pulse (active, 4-hex, 80 Qi + 10 Qi/use). Balance note: higher cost than standard T1 to prevent auto-first-pick. T3-T4 deferred to Phase 2. |

---

## Deep Dive Research

The following topics were researched in depth to prevent costly refactoring during implementation:

### Save Data Structure

```csharp
[System.Serializable]
public class SaveData
{
    public int version = 1;                    // Save format version for migration
    public string timestamp;                   // ISO 8601 timestamp
    public int turnNumber;                     // Current turn
    public int activePlayerIndex;              // Whose turn it is
    public List<SectSaveData> sects;           // All sect data
    public List<TileSaveData> tiles;           // All tile data (terrain, features, visibility)
    public List<UnitSaveData> units;           // All unit data
    public List<FormationSaveData> formations; // All formation data
    public GameStateSaveData gameState;        // Global game state (zodiac, events, etc.)
}

[System.Serializable]
public class SectSaveData
{
    public string sectId;
    public string sectName;
    public int factionId;
    public float tael;
    public float qi;
    public float renown;
    public float face;
    public int dissentLevel;
    public int templeTier;
    public int hallTier;
    public List<string> completedResearch;
    public List<string> activeResearch;        // techId + progress%
    public int foundingTileX, foundingTileZ;
    public Dictionary<string, int> resources; // commodity stockpile
    public List<BuildingSaveData> buildings;
    public List<DiplomacySaveData> relations; // bilateral relation scores
}

[System.Serializable]
public class TileSaveData
{
    public int x, z;                          // Hex coordinates
    public int terrainTypeIndex;
    public int elevation;
    public int qiDensity;                     // None/Sparse/Moderate/Dense/LeyLine
    public int caveType;                      // None/Meditation/BodyTempering/QiRefinement/SpiritTrial
    public bool hasRoad;
    public int ownerSectId;                   // -1 = unclaimed
    public bool[] visibilityPerSect;          // fog of war per sect
}

[System.Serializable]
public class UnitSaveData
{
    public string unitId;
    public string unitType;                   // Prefab name for instantiation
    public int ownerSectId;
    public int tileX, tileZ;
    public float currentHP;
    public float maxHP;
    public int movementPointsRemaining;
    public int formationType;                 // -1 = no formation
    public List<string> assignedDisciples;    // disciple IDs in this unit
    public string activeAction;               // "Meditate", "QiPulse", etc.
}

[System.Serializable]
public class FormationSaveData
{
    public string formationId;
    public int formationType;                 // Sword/Commander/Scout
    public int tileX, tileZ;
    public List<string> unitIds;              // Units in this formation
}
```

### Serialization Strategy

**Use JsonUtility (Unity built-in) for v1.** It's fast, requires no external packages, and handles the data structures above. Limitations to be aware of:
- No dictionary support → use `SerializableDictionary` wrapper or parallel arrays
- No polymorphism → use type discriminator fields (`unitType` string)
- No null for value types → use nullable wrappers or sentinel values
- Float precision → JsonUtility serializes with excessive decimal places; round to 4 decimals for file size

**If JsonUtility becomes limiting, migrate to Newtonsoft JSON** (via `com.unity.nuget.newtonsoft-json` package). Plan the data structures to be compatible with both.

### File Operations (Atomic Writes)

```csharp
public static class SaveManager
{
    private static string SaveDir =>
        Path.Combine(Application.persistentDataPath, "saves");

    public static bool Save(int slot, SaveData data)
    {
        Directory.CreateDirectory(SaveDir);
        data.timestamp = System.DateTime.UtcNow.ToString("o");

        string json = JsonUtility.ToJson(data, prettyPrint: true);
        string path = SlotPath(slot);
        string tmpPath = path + ".tmp";
        string bakPath = path + ".bak";

        try
        {
            // Write to temp file first (atomic write pattern)
            File.WriteAllText(tmpPath, json);

            // Backup existing save
            if (File.Exists(path))
                File.Copy(path, bakPath, overwrite: true);

            // Atomic move (rename is atomic on all platforms)
            File.Move(tmpPath, path, overwrite: true);
            return true;
        }
        catch (IOException e)
        {
            Debug.LogError($"Save failed: {e.Message}");
            return false;
        }
    }

    public static SaveData Load(int slot)
    {
        string path = SlotPath(slot);

        // Try primary save, then backup
        if (!File.Exists(path))
        {
            string bakPath = path + ".bak";
            if (File.Exists(bakPath))
                path = bakPath;
            else
                return null;
        }

        try
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return MigrateSave(data);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Load failed: {e.Message}");
            return null;
        }
    }

    private static string SlotPath(int slot) =>
        Path.Combine(SaveDir, $"save_slot_{slot}.json");
}
```

### Save Versioning & Migration

**Always include a version field.** When save format changes between updates, migrate old saves forward:

```csharp
private static SaveData MigrateSave(SaveData data)
{
    if (data.version < 2)
    {
        // v1 → v2: formations added
        if (data.formations == null)
            data.formations = new List<FormationSaveData>();
    }

    if (data.version < 3)
    {
        // v2 → v3: Qi sensing added
        foreach (var unit in data.units)
        {
            if (unit.activeAction == null)
                unit.activeAction = "None";
        }
    }

    data.version = CurrentSaveVersion; // Always update to latest
    return data;
}
```

**Key rule:** Never remove fields from SaveData. Only add new ones with sensible defaults. Old saves will have missing fields → JsonUtility fills with defaults.

### Save File Size Estimation

For a 4,000-tile map with 6 sects and ~50 units:
- Tile data: 4,000 × ~50 bytes = ~200 KB
- Unit data: 50 × ~200 bytes = ~10 KB
- Sect data: 6 × ~500 bytes = ~3 KB
- Game state: ~5 KB
- **Total: ~220 KB per save file** (well within reasonable limits)

### Autosave Strategy

Per GDD §17.5: autosave every 5 turns. Implementation:
- Use a separate autosave slot (slot 0) that overwrites each time
- Keep 3 manual save slots (1-3) for player
- Autosave on the Income Phase (after all state changes are complete)
- Show a brief "Saving..." indicator in the UI

### Platform Considerations

| Platform | Save Location | Notes |
|----------|--------------|-------|
| Windows | `%AppData%/LocalLow/<company>/<product>/saves/` | Standard |
| macOS | `~/Library/Application Support/<company>/<product>/saves/` | Standard |
| Linux | `~/.config/unity3d/<company>/<product>/saves/` | Standard |
| Android | `/data/data/<package>/files/saves/` | No permissions needed |
| iOS | `<App>/Documents/saves/` | Included in iCloud backup |
| WebGL | IndexedDB via PlayerPrefs | Use for small saves only |

### Testing Checklist

- [ ] Save after every phase (Action, Build, Research, Income)
- [ ] Load and verify all state matches (units, tiles, resources, research)
- [ ] Test save/load mid-combat
- [ ] Test autosave every 5 turns
- [ ] Test backup recovery (delete primary save, verify backup loads)
- [ ] Test version migration (load v1 save in v2 build)
- [ ] Test on all target platforms
- [ ] Test with maximum map size (4,000 tiles)
- [ ] Test save file size stays under 1 MB

---

## Deep Dive: Procedural Map Generation

### Architecture

**Two-layer approach:**
1. **Terrain generation** — Perlin noise for height/moisture → terrain types
2. **Feature placement** — Rules-based placement of special features (Ley Lines, caves, Sacred Peaks, settlements)

This is the approach used by Catlike Coding's hex map tutorial and Felix Turner's WFC generator. WFC is overkill for v1 — Perlin noise + rules is simpler and faster.

### Terrain Generation Algorithm

```csharp
public class HexMapGenerator
{
    // Noise parameters
    public float terrainScale = 0.03f;      // Perlin noise frequency
    public float moistureScale = 0.02f;     // Second noise layer for moisture
    public int seed = 0;                    // Deterministic seed

    // Terrain thresholds
    public float waterLevel = 0.3f;
    public float sandLevel = 0.4f;
    public float grassLevel = 0.6f;
    public float forestLevel = 0.75f;
    public float mountainLevel = 0.85f;

    public void GenerateMap(int width, int height)
    {
        // Initialize noise with seed
        Unity.Mathematics.Random rng = new(seed);

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                // Sample noise (offset to avoid symmetry at origin)
                float terrain = Mathf.PerlinNoise(
                    (x + seed * 1000) * terrainScale,
                    (z + seed * 1000) * terrainScale
                );
                float moisture = Mathf.PerlinNoise(
                    (x + seed * 2000) * moistureScale + 500,
                    (z + seed * 2000) * moistureScale + 500
                );

                // Assign terrain based on height + moisture
                HexTileData tile = grid.GetCell(x, z);
                tile.terrainType = ClassifyTerrain(terrain, moisture);
                tile.elevation = Mathf.FloorToInt(terrain * 5); // 0-4 elevation levels
                tile.qiDensity = ClassifyQiDensity(terrain, moisture, rng);
            }
        }

        // Post-processing: place special features
        PlaceLeyLines(rng);
        PlaceCaves(rng);
        PlaceSacredPeaks(rng);
        PlaceSettlements(rng);

        // Ensure fair starting positions
        PlaceStartingPositions(rng);
    }

    TerrainType ClassifyTerrain(float height, float moisture)
    {
        if (height < waterLevel) return TerrainType.Water;
        if (height < sandLevel) return TerrainType.Sand;
        if (height < grassLevel) return TerrainType.Plains;
        if (height < forestLevel) return TerrainType.Forest;
        if (height < mountainLevel) return TerrainType.Hills;
        return TerrainType.Mountain;
    }

    QiDensity ClassifyQiDensity(float height, float moisture, Random rng)
    {
        // Qi density correlates with terrain quality but has randomness
        float qiBase = height * 0.6f + moisture * 0.4f;
        float qiNoise = (Mathf.PerlinNoise(height * 10, moisture * 10) - 0.5f) * 0.3f;
        float qiValue = Mathf.Clamp01(qiBase + qiNoise);

        if (qiValue > 0.9f) return QiDensity.LeyLine;      // ~2% of tiles
        if (qiValue > 0.7f) return QiDensity.Dense;         // ~10% of tiles
        if (qiValue > 0.5f) return QiDensity.Moderate;      // ~20% of tiles
        if (qiValue > 0.3f) return QiDensity.Sparse;        // ~30% of tiles
        return QiDensity.None;                              // ~38% of tiles
    }
}
```

### Feature Placement Rules

**Ley Lines (rare, high-value):**
- Place on tiles with `QiDensity.LeyLine`
- Ensure minimum 10-hex spacing between Ley Lines
- ~8-12 Ley Lines per 4,000-tile map
- Ley Lines should form natural "paths" (use noise to create curves)

**Caves (uncommon, medium-value):**
- Place on Mountain or Hills tiles with `QiDensity >= Moderate`
- Minimum 5-hex spacing between caves
- ~30-50 caves per map
- Cave type (Meditation/BodyTempering/QiRefinement/SpiritTrial) assigned by Qi density

**Sacred Peaks (very rare, highest-value):**
- Place on Mountain tiles with `QiDensity >= Dense`
- Minimum 15-hex spacing
- ~3-5 Sacred Peaks per map
- These are the most contested tiles in the game

**Settlements (common, strategic):**
- Place on Plains/Forest tiles near water
- Minimum 8-hex spacing between settlements
- ~20-30 settlements per map
- Settlement type (Village/Town/Trade Post) by terrain quality

### Starting Position Fairness

**Critical for multiplayer.** Each sect needs:
- At least 15 tiles of usable land (not water/mountain)
- Access to at least 2 different terrain types
- At least 1 Moderate+ Qi density tile within 5 hexes
- Minimum 12-hex distance from other sects
- No direct line-of-sight to another sect (fog of war at start)

```csharp
void PlaceStartingPositions(Random rng)
{
    List<HexTileData> candidates = FindValidStartTiles();

    // Score each candidate by resource diversity
    foreach (var tile in candidates)
    {
        tile.startScore = ScoreStartPosition(tile);
    }

    // Pick top N positions with maximum spacing
    List<HexTileData> starts = SelectSpacedPositions(
        candidates,
        count: 6,           // 1 player + 5 AI
        minSpacing: 12       // hexes
    );

    foreach (var start in starts)
    {
        start.ownerSectId = AssignSect(start);
        start.terrainType = TerrainType.Plains; // Ensure buildable
    }
}

float ScoreStartPosition(HexTileData center)
{
    float score = 0;
    var neighbors = grid.GetNeighbors(center, radius: 5);

    foreach (var n in neighbors)
    {
        if (n.terrainType != TerrainType.Water && n.terrainType != TerrainType.Mountain)
            score += 1;
        if (n.qiDensity >= QiDensity.Moderate)
            score += 3;
        if (n.qiDensity >= QiDensity.Dense)
            score += 5;
        if (n.caveType != CaveType.None)
            score += 2;
    }

    return score;
}
```

### Map Size & Performance

| Map Size | Tiles | Generation Time | Save File Size | Recommended Players |
|----------|-------|-----------------|----------------|-------------------|
| Small | 1,600 (40×40) | <1s | ~80 KB | 2-4 |
| Medium | 4,000 (60×60) | ~2s | ~220 KB | 4-6 |
| Large | 9,600 (80×80) | ~5s | ~500 KB | 6-8 |

**v1 target: Medium (4,000 tiles, 60×60)**

### Determinism Requirement

**Map generation must be deterministic** for multiplayer. Same seed = same map on all clients.
- Use `System.Random` with seed (not `UnityEngine.Random`)
- All noise sampling must be in a fixed order (iterate x then z)
- No `Time.deltaTime` or frame-dependent logic
- Seed is set by the host and transmitted to all clients

### Testing Checklist

- [ ] Same seed produces identical maps on all clients
- [ ] All starting positions meet fairness criteria
- [ ] Ley Lines are properly spaced (min 10 hexes)
- [ ] Caves are properly spaced (min 5 hexes)
- [ ] Sacred Peaks are properly spaced (min 15 hexes)
- [ ] Settlements are properly spaced (min 8 hexes)
- [ ] No unreachable land areas (flood fill check)
- [ ] Qi density distribution matches targets (2% LeyLine, 10% Dense, etc.)
- [ ] Generation completes under 3 seconds for 4,000 tiles
- [ ] Save/load preserves map state exactly

These concepts were discussed during Phase 1 research and deferred to Phase 2 (Engagement):

### Qi Gathering Formation (聚灵阵)

**Concept:** Place a formation of disciples on Qi-rich tiles (Ley Lines, Sacred Peaks, caves) to passively collect Qi for the sect's stockpile. Disciples in the gathering formation can't fight or move.

**Qi gathering rate example:**
- 1 Outer Disciple on Moderate Qi tile: +2 Qi/turn
- 1 Inner Disciple on Ley Line: +8 Qi/turn
- 3 Inner Disciples on Sacred Peak: +30 Qi/turn

**Why it's good:**
- Makes Qi feel like a *physical resource* on the map, not just a number
- Creates strategic decisions: gather Qi or keep disciples in the army?
- Very wuxia — "qi gathering formations" (聚灵阵) are a genre staple
- Interacts with hex map meaningfully — tile quality matters

**Implementation:** ~3-4 days. Needs: `QiGatheringArray` component, tile Qi density integration, UI for placement, AI for assignment decisions.

**Prerequisites:** Combat system (vulnerability), AI system (assignment logic), formation system (reuse from Phase 1).

### Research Array (deferred)

**Concept:** Assign disciples to a research formation that directly boosts research speed. Similar to Qi Gathering but targets research instead of Qi income.

**Status:** Lower priority than Qi Gathering. Qi Gathering is more thematic and simpler to implement. Research Array could be added later as a variant.

### Meditate Action (v1 lite alternative)

**Concept:** Any unit can be given a "Meditate" order. While meditating: +5 Qi/turn, can't move or fight. Simple toggle, no new UI.

**Estimated cost:** ~1 day. Could be added to v1 if desired.
