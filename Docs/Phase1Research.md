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

### Decision: 9 NODES (3 PER BRANCH) + ENLIGHTENMENT TRIGGERS

**For v1, implement 3 nodes per branch (9 total, all T1-T2), with Enlightenment Triggers. Defer T3-T4 nodes to Phase 2.**

**Math rationale:** At baseline research speed (~15 Qi/turn), T1 (30 Qi) ≈ 5 turns, T2 (80 Qi) ≈ 12-15 turns. 9 nodes × ~10 turns average = ~90 turns to complete all T1-T2. That's a meaningful chunk of a 200-300 turn game without delaying T3-T4 content.

**Enlightenment Triggers are IN for v1.** They're the single highest-value research feature — they make research feel active instead of passive. Implementation is straightforward (event subscription + 50% progress boost).

**Recommended v1 nodes (3 per branch):**

| Branch | Node 1 (T1) | Node 2 (T1) | Node 3 (T2) |
|--------|-------------|-------------|-------------|
| Alchemy | Herbal Medicine | Antidote Craft | Qi Restoration Pills |
| Forge | Iron Smelting | Basic Weapons | Steel Refinement |
| Martial | Basic Sword Arts | Basic Fist Arts | Basic Qi Circulation |

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
| Research | 9 nodes (3 per branch) + Enlightenment Triggers | Medium | Medium |
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

## Resolved Open Questions

| # | Question | Decision |
|---|----------|----------|
| 1 | Decisive victory threshold? | **Yes**, include in v1 |
| 2 | River movement penalty? | **Yes**, +1 MP to cross |
| 3 | Enlightenment Triggers in v1? | **Yes**, include in v1 |
| 4 | Founder combat ability? | **No combat**, but "Call for Aid" action |
| 5 | NGO vs custom transport? | **Unity NGO** for v1 |
| 6 | Research nodes per branch? | **3 per branch (9 total)** |
| 7 | Map size target? | **~4,000 tiles** (60×60 hex grid) |
