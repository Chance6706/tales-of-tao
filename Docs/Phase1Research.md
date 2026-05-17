# Phase 1 Research & Refinement — Decision Document

**Branch:** `feature/phase1-research`
**Date:** 2026-05-16
**Author:** OWL (research) → Josh (decisions)

This document presents research findings and draft decisions for the 5 key design questions identified in the Production Roadmap for Phase 1: Foundation. Each section covers: what the GDD says, what shipped games do, what the codebase currently has, and a recommended decision with rationale.

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

### Recommended Decision

**For v1, implement auto-resolve only. Defer tactical view to Phase 2 or later.**

Rationale:
1. The GDD's auto-resolve formula is well-specified and can be implemented in ~200 lines of code
2. Tactical view is a massive scope increase (7×7 grid, unit placement, initiative, duels, morale tracking) — easily 2-3 weeks of work
3. Civ VI proves that auto-resolve-only combat can be deeply satisfying for a 4X game
4. The GDD already states "AI always auto-resolves" — this signals the tactical view is optional
5. The formula's randomness (±15%) creates emergent stories without requiring player micro

**Implementation approach:**
- Create `CombatPowerCalculator.Calculate(UnitData, HexTileData, GameState)` as a pure static function (per GDD dev note)
- Create `CombatResolver.Resolve(attacker, defender, attackerTile, defenderTile)` that returns a `CombatResult` struct
- `CombatResult` contains: winner, attackerCasualties%, defenderCasualties%, retreatFlag, renounceChange, faceChange
- Use the existing `Random` wrapper (seeded for determinism in MP)
- Casualty distribution: implement `CasualtyDistributor` with the GDD's weight system (Peon 50%, Outer 30%, Inner 20%)

**Open question for Josh:** Should we include a "decisive victory" threshold (>2× CP differential) in v1, or keep it simple? The GDD specifies it but it adds complexity.

---

## 2. Unit Movement Model

### GDD Specification (§9.2)
- Base movement: 3 hexes/turn for all units
- Terrain costs: defined per terrain type (not fully specified in the excerpt, but implied)
- Zone of Control: +2 movement cost per adjacent enemy tile
- Roads: 0.5 movement cost (built by Peon Gangs, 5 Lumber, 3 turns)
- Lightness Art tech tree modifies movement (+1 to +3, ignore terrain, ignore ZOC, invisibility)
- Fog of War: cannot path through Hidden tiles

### What Shipped Games Do

**Civilization VI:**
- 1 unit per tile (1UPT) — no stacking
- Movement points: 2 MP base for most units, 3 for scouts/cavalry
- Terrain: Plains/Grassland = 1 MP, Forest/Rainforest = 2 MP (unless road), Hills = 2 MP, Rivers = extra cost unless bridge/ford
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
- Key insight: TW3K's movement creates *strategic decisions* about where to go, not micro about how to get there

### Current Codebase
- `HexPathfinder` exists with A* pathfinding
- `HexTile` has terrain data
- No movement cost system, no ZOC system, no road system
- Units spawn at mouse position with no movement logic

### Recommended Decision

**For v1, implement a simplified movement system: base 3 MP, terrain costs, ZOC, and roads. Defer Lightness Art movement bonuses to Phase 2 (they require the research system).**

Rationale:
1. The GDD's base movement of 3 hexes/turn is generous — this means most tiles are reachable in 1-2 turns
2. ZOC is critical for strategic depth: it creates "front lines" and makes positioning matter
3. Roads are important for the economy (supply chains) and create a meaningful Peon Gang action
4. The `HexPathfinder` already exists — we just need to add a `MovementCostCalculator` and integrate it
5. Lightness Art bonuses can be added later as modifiers to the base system

**Implementation approach:**
- Create `MovementCostCalculator.GetCost(HexTileData, UnitData)` → returns float cost
- Terrain cost table: Plains=1, Forest=2, Hills=2, Mountain=3, Desert=2, Swamp=3, Road=0.5
- ZOC: check adjacent tiles for enemy units, add +2 per adjacent enemy
- Integrate with `HexPathfinder` to calculate reachable tiles
- Movement points: `UnitData.MovementPoints` (default 3, modified by tech)
- Roads: add `RoadComponent` to tiles, modify cost calculation

**Open question for Josh:** Should rivers have a movement cost penalty in v1? The GDD mentions ZOC doesn't apply across rivers but doesn't specify river movement cost. Civ VI makes rivers significant; should we?

---

## 3. Research Tree Structure

### GDD Specification (§8)
- 3 branches: Alchemy (Medicine Hall), Forge (Armory), Martial Techniques (Library)
- ~35+ nodes total (based on the tree diagrams: ~12 per branch)
- Tier gating: T1-2 requires Hall T1+, T3 requires Hall T2+, T4 requires Hall T3+
- Research speed: `(QiYield × 0.40) + (LibraryTier × 3) + (WisdomElder × 4) × (1 + 0.10 × LeyLineCount)`
- Qi costs: T1=30, T2=80, T3=180, T4=400
- Enlightenment Triggers: 50% progress boost from specific in-game actions
- Up to 1 tech per branch simultaneously (3 total)
- Eureka-style "Enlightenment Triggers" for 50% progress boost

### What Shipped Games Do

**Civilization VI:**
- Linear tech tree with some branching (especially in later eras)
- ~100+ technologies across the full tree
- Eureka moments: 50% boost from specific actions (build 2 mines → Mining 50%)
- Research is paid in Science/turn; no upfront cost
- Key insight: Civ VI's Eureka system creates a feedback loop between playing and researching — it makes research feel *earned*, not just waited for

**Stellaris:**
- Card-based research system: draw 3 options per branch, pick 1
- Cards have weights (some are more likely based on your empire's traits)
- Research is paid in Science/turn
- Key insight: Stellaris's card system creates *meaningful choices* — you can't research everything, so you must prioritize

**Age of Wonders 4:**
- Tomes (research) are unlocked by realm traits and city buildings
- Each tome has 3-4 spells/units
- Research is paid in Mana/turn
- Key insight: AOW4's tome system gates research behind *infrastructure* — you need the right buildings

### Current Codebase
- `ResearchState` exists as a placeholder (does nothing)
- `TechNodeSO` ScriptableObjects exist but are empty
- No `ResearchManager`, no research speed calculation, no Enlightenment system

### Recommended Decision

**For v1, implement a simplified research system: 3 branches, ~8-10 nodes total (3-4 per branch), Enlightenment Triggers, and hall tier gating. Defer the full ~35-node tree to Phase 2.**

Rationale:
1. The GDD's full tree (~35 nodes) is too complex for v1 — Civ VI has ~100 but that's across 6 eras; our 35 nodes are all in one era
2. Starting with 3-4 nodes per branch (T1-T2 only) gives players meaningful choices without overwhelming them
3. Enlightenment Triggers are a high-value, low-cost feature — they make research feel active
4. Hall tier gating creates a natural progression: build infrastructure → unlock better research
5. The research speed formula is well-specified and can be implemented directly
6. We can add T3-T4 nodes in Phase 2 when the content system is more mature

**Implementation approach:**
- Create `ResearchManager` with `ActiveResearch` array (3 slots, one per branch)
- Create `TechNodeSO` with: branch, tier, qiCost, enlightenmentTrigger, unlocks
- Research speed: implement the GDD formula directly
- Enlightenment Triggers: create `EnlightenmentTriggerSystem` that subscribes to game events
- Hall tier gating: check `BuildingController.GetHallTier(branch)` before allowing research
- UI: simple 3-column layout (one per branch), show progress bars, highlight available nodes

**Recommended v1 node count:**
- Alchemy: Herbal Medicine, Antidote Craft, Herb Cultivation, Qi Restoration Pills (4 nodes)
- Forge: Iron Smelting, Basic Weapons, Leather Armor, Steel Refinement (4 nodes)
- Martial: Basic Sword Arts, Basic Fist Arts, Basic Qi Circulation, Lightness Art (4 nodes)
- Total: 12 nodes (all T1-T2)

**Open question for Josh:** Should we include Enlightenment Triggers in v1, or defer them? They're high-value but require event system integration.

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
- Settler is the most important early-game unit — losing it is devastating
- Player starts with 1 Settler + 1 Warrior (to protect the Settler)
- Key insight: Civ VI's Settler creates *tension* — you need to protect it while exploring, and choosing where to found is the most important early decision

**Total War: Three Kingdoms:**
- Lord unit: army commander, can be killed in battle
- Player starts with 1 Lord + 1 army
- Key insight: TW3K's Lord creates *attachment* — players care about their Lord because they're a named character with skills

**Age of Empires 4:**
- Scout unit: fast exploration, no combat
- Player starts with 1 Scout + villagers
- Key insight: AoE4's Scout creates *information advantage* — knowing the map is critical

### Current Codebase
- `FoundSectCommand` exists but just places the sect directly (no Founder unit)
- No Founder unit, no movement, no founding consumption
- `SectData` exists with founding tile stats

### Recommended Decision

**For v1, implement the Founder Unit as specified in the GDD: a movable unit with 3 MP, consumed on founding, accompanied by a Support Unit (5 Peons).**

Rationale:
1. The GDD's design is well-thought-out and creates the right kind of early-game tension
2. The Founder becoming the Sect Leader NPC is a nice narrative touch
3. The founding tile bonus system is already partially implemented in `SectData`
4. This is a HIGH severity gap (per Gap Analysis) — the current "sect just appears" approach removes the most important early-game decision
5. Implementation is straightforward: create a `FounderUnit` prefab, add movement logic, add founding command

**Implementation approach:**
- Create `FounderUnit` as a special `UnitDataSO` with 3 MP, no combat ability
- Add `FoundSectCommand` validation: check 50 Tael, check 8-hex distance from other sects
- On founding: remove Founder from unit roster, create `SectData`, place Temple
- Support Unit: create a `PeonGang` unit with 5 Peons, no combat, can recruit from settlements
- UI: highlight valid founding tiles when Founder is selected

**Open question for Josh:** Should the Founder have any combat ability? The GDD says no, but it might be frustrating to lose the Founder to a random bandit. Civ VI's Settler can't fight either, but it starts with a Warrior escort.

---

## 5. Network Architecture

### GDD Specification (§17)
- Multiplayer: up to 8 players
- Turn-based with simultaneous movement (except during war)
- Host migration support
- JSON-based save files for async/PBEM option

### What Shipped Games Do

**Civilization VI:**
- Architecture: P2P with Steam relay servers
- Deterministic lockstep: each client runs the same simulation, only inputs are sent
- Simultaneous turns: all players move at the same time, conflicts resolved by unit strength
- Host migration: supported via Steam networking
- Key insight: Civ VI's P2P approach means *no server costs* and works well for 2-8 player turn-based games

**Stellaris:**
- Architecture: P2P with Paradox relay servers
- Deterministic lockstep (same as Civ VI)
- Host migration: supported
- Key insight: Stellaris proves deterministic lockstep works for complex 4X games with hundreds of entities

**Old World:**
- Architecture: Client-server (async multiplayer)
- Turns are sequential (not simultaneous)
- Key insight: Old World's async approach allows *play-by-email* style, which is great for busy adults

**Research from Longwelwind (Swords & Ravens developer):**
- Deterministic action propagation is the best approach for turn-based games
- Only send player actions (not state updates) — clients apply actions deterministically
- Secret state can be handled by filtering actions and reconciling differences
- Bandwidth is minimal (only actions, not state)
- Key insight: "Having to implement the networking only once, and be done with it, is quite elegant"

**Research from Glenn Fiedler (Más Bandwidth):**
- Deterministic lockstep is ideal for low player counts (4-8) with many units
- Requires determinism (seeded RNG, no floating point inconsistencies)
- Vulnerable to lag switch attacks → use relay server instead of pure P2P
- Not suitable for large open worlds or non-deterministic physics
- Key insight: "For a turn-based strategy game with many units, deterministic lockstep is a no brainer"

### Current Codebase
- `TurnCoordinator` exists with server authority
- `ReadySystem` exists for phase sync
- `NetworkCommandQueue` exists for command ordering
- All multiplayer is local-only (no actual network transport)
- No serialization, no client-side prediction, no desync detection

### Recommended Decision

**For v1, implement deterministic action propagation with a relay server architecture. Use the existing `TurnCoordinator` and `NetworkCommandQueue` as the foundation. Defer async/PBEM to Phase 3.**

Rationale:
1. Deterministic action propagation is the most elegant approach for turn-based games (per Longwelwind's research)
2. The existing `TurnCoordinator` + `NetworkCommandQueue` architecture is already designed for this
3. Only actions need to be sent (not state), minimizing bandwidth
4. Relay server prevents lag switch attacks (per Fiedler's research)
5. Unity's Netcode for GameObjects or a custom TCP/UDP layer can handle the transport
6. Async/PBEM is a Phase 3 feature — it requires save/load (Phase 1) and a different turn model

**Implementation approach:**
- Network transport: use Unity Netcode for GameObjects (NGO) or custom TCP layer
- Relay server: use a simple relay (Unity Relay, Steam Networking, or custom)
- Determinism: use seeded RNG (`System.Random` with seed), avoid `UnityEngine.Random`
- Action serialization: serialize `Command` objects to JSON, send over network
- Desync detection: compute game state hash each turn, compare across clients
- Host migration: store game state on relay, allow new host to download

**Key technical decisions:**
- Use `System.Random` (deterministic) instead of `UnityEngine.Random` (non-deterministic)
- All game logic must be deterministic: no `Time.deltaTime`, no `Mathf` inconsistencies
- Float math can cause desyncs across platforms → consider fixed-point math for critical calculations
- Network tick: process all player actions at the end of each phase (not real-time)

**Open question for Josh:** Should we use Unity Netcode for GameObjects (built-in) or a custom transport layer? NGO is easier but less flexible. Custom is more work but gives us full control.

---

## Summary of Recommendations

| Topic | Recommendation | Complexity | Risk |
|-------|---------------|------------|------|
| Combat | Auto-resolve only, defer tactical view | Medium | Low |
| Movement | Base 3 MP + terrain costs + ZOC + roads | Medium | Low |
| Research | 12 nodes (3-4 per branch), Enlightenment Triggers | Medium | Medium |
| Founder Unit | Movable unit, consumed on founding | Low | Low |
| Networking | Deterministic action propagation + relay server | High | Medium |

## Recommended Implementation Order

1. **Founder Unit** (Week 2) — lowest risk, unblocks playtesting
2. **Unit Movement** (Week 2-3) — foundational for everything else
3. **Combat Auto-Resolve** (Week 3-4) — depends on movement (retreat pathfinding)
4. **Research System** (Week 4-5) — can be developed in parallel with combat
5. **Save/Load** (Week 5-6) — depends on all above systems being serializable
6. **Networking** (Phase 3) — requires save/load and all core systems

## Open Questions for Josh

1. Should we include "decisive victory" threshold in combat v1?
2. Should rivers have a movement cost penalty in v1?
3. Should Enlightenment Triggers be in v1 or deferred?
4. Should the Founder have any combat ability?
5. Unity Netcode for GameObjects vs custom transport layer?
6. How many research nodes per branch for v1? (I recommend 3-4)
7. What's the map size target for v1? (GDD says up to 16,000 tiles — what's realistic?)
