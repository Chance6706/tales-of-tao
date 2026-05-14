# AUDIT REPORT

## Step 1 — Reference Benchmarking & Gap Analysis

---

### 1.1 Civilization VI — What Made It the Gold Standard

**The three mechanics that define Civ VI:**

**1. District System (Unstacking Cities)**
Rather than stacking all yield bonuses on a single city tile, Civ VI forces players to build districts on specific terrain tiles, creating meaningful geographic decisions. Placing a Campus near mountains gives bonus Science; a Harbor must be coastal. Every city becomes a spatial puzzle. The consequence: players engage with the hex map as a strategic resource, not just a movement grid.

**2. Eureka / Inspiration Moments**
Research and civics have contextual "boost" conditions — build two Mines and Mining research is 50% done instantly. This creates a feedback loop between what you're doing and what you're learning, making the tech tree feel earned rather than arbitrary. It also gives players of all styles a path to research efficiency.

**3. Agenda + Personality Consistency in AI**
Each AI leader has a public agenda (known to the player) and a hidden agenda (discoverable). This makes AI behavior legible and creates diplomatic texture — you know Gilgamesh will always value allies and punish lone wolves. Predictable-but-varied AI generates stories players retell.

**Cross-reference with Tales of the Tao:**
- ToT has buildings placed at the sect compound tile (single tile), not distributed across the map. This means the hex map is primarily a movement/combat space, not a yield-optimization puzzle. **Gap:** building placement has no spatial consequence.
- ToT has no Eureka equivalent. Research is pure Qi-over-time. **Gap:** research completion feels like waiting, not playing.
- ToT's AI personalities are weight tables, not legible agendas players can learn. **Gap:** AI behavior is opaque, reducing diplomatic engagement.

---

### 1.2 Stellaris — The X-Factor

**The three mechanics that define Stellaris:**

**1. Narrative Event Chains**
Stellaris generates story through parameterized event chains that respond to empire state. The same "derelict ship" event plays differently if you're a xenophile vs. a militarist. Events aren't random flavor — they're modulated by dozens of conditions. The game feels like it's telling *your* story.

**2. Pop System (Dynamic Empire Identity)**
Pops grow, migrate, gain jobs, adopt ethics, and rebel. Your empire's character emerges from population dynamics, not just your policy selections. A conquered militarist pop will always create friction in a pacifist empire. This makes internal management feel alive.

**3. Crisis Escalation**
Stellaris uses escalating late-game crises (Unbidden, Prethoryn Scourge) that force inter-empire cooperation, dissolving rivalries and creating narrative climax. The late game has a *reason* to exist beyond "finish the win condition."

**Cross-reference with Tales of the Tao:**
- ToT has a 9-event table with flat probability triggers. Events fire randomly without meaningful state-parameterization. **Gap:** events don't respond to your sect's identity or current story.
- ToT's disciple system is hierarchical HR, not a living population. Peons don't have cultural identity or ethical drift. **Gap:** no emergent internal politics.
- ToT has no equivalent late-game crisis or convergence mechanic. Three victory conditions race independently without collision forcing. **Gap:** late game can feel like executing a checklist rather than navigating a climax.

---

### 1.3 Mount & Blade: Bannerlord — Economy That Feels Alive

**The three mechanics that define Bannerlord:**

**1. Village Supply Chains**
Villages produce specific goods based on their terrain. Those goods flow to towns as raw materials, get processed into finished goods, and those finished goods get traded across regions. When you raid a village, downstream workshops in towns starve. The economy has *physical causality*.

**2. Dynamic Pricing with Inventory Memory**
Town prosperity, garrison size, recent battles, and caravan traffic all shift commodity prices. A player who floods a market with grain sees prices crash. Prices have memory — they recover over time. Players learn to read prices as signals about the world, not just numbers.

**3. Lord Loyalty and Clan Politics**
Lords have individual relationships with you and with your kingdom. Their loyalty fluctuates based on whether you honor promises, distribute fiefs fairly, and win battles. A disloyal lord defects, taking their clan wealth with them. Internal politics is a second front you must manage.

**Cross-reference with Tales of the Tao:**
- ToT has a commodity market with price drift (5%/turn) and buy inflation/sell deflation. This is directionally correct but lacks physical causality — commodities don't flow between nodes along supply chains. **Gap:** the economy is a price simulation, not a supply network.
- ToT's "Trade Routes" generate passive Tael but don't model what goods travel on them or what happens if a route is disrupted militarily. **Gap:** trade feels like a toggle, not a strategic system.
- ToT has Dissent as an internal stability mechanic but no individual NPC politics analogous to lord loyalty. Elders are statistics, not political actors. **Gap:** internal management lacks character.

---

### 1.4 Gap Analysis Summary

#### Orphaned Mechanics
Features present in ToT that sound compelling but don't currently reinforce the core loop (Found Sect → Cultivate Disciples → Dominate):

| Orphaned Mechanic | Why It's Orphaned | Recommended Action |
|---|---|---|
| **Face System (0–100 axis)** | Face modifies market prices and settlement disposition, but players have no direct actions to build Face — it's a side effect of other actions. There's no "Face play" strategy. | **Merge** Face into Renown as a secondary axis on the same score. Simplify to three tiers only. |
| **Rare Earth Commodity** | Only used in "advanced Forge research" and "Sect Treasure crafting." Not referenced in any building, trade route, or event with specific mechanics. Its source terrain (Mountain deep mines) has no mining mechanic defined. | **Merge** Rare Earth into Iron Ore with a quality flag. Reduces commodity count from 8 to 7 without losing design intent. |
| **Wandering Tribe Settlement Type** | Provides "Horse units, mercenary disciples" but no mechanic for deploying mercenary disciples is defined. They can't be in your disciple roster (different affinity) and you can't train them. | **Simplify** to: Wandering Tribes provide Horse commodity bonuses and one-time Outer Disciple mercenary hire (fight one battle, then disband). |
| **Branch Sect Outpost T3 (Autonomous Sub-Sect)** | At T3, Branch Sects become "full sub-sects with own build queue" governed by High Elders. But the UI, build queue logic, and High Elder AI governance are never specified. This is an entire second game within the game. | **Defer** autonomous sub-sect governance to post-launch DLC. T3 Branch Sect = expanded build list only, still controlled by player. |
| **Formation Arrays (Forge Tier 4)** | Listed as a Tier 4 Forge research unlock with no mechanical definition. What does a Formation Array do in auto-resolve? In tactical view? | **Define or cut.** Specified in the refined GDD as: Formation Arrays grant a passive combat CP bonus (+15%) to all friendly units fighting within 3 hexes of a designated Formation Elder. |

#### Feature Creep
Elements that add complexity without proportional strategic value at current scope:

| Feature | Complexity Cost | Value Delivered | Decision |
|---|---|---|---|
| **8 Commodities** | High (8 price curves, 8 supply sources, 8 craft uses to balance) | Medium (only Iron Ore, Jade, Spirit Herbs, and Lumber appear in hard mechanical gates) | **Reduce to 6** by merging Rare Earth→Iron Ore and removing Horses as a commodity (make Horses a terrain movement bonus only) |
| **Duel Rock-Paper-Scissors (Sword > Spear > Fist > Sword)** | Medium (players must learn and remember the triangle; adds UI complexity to tactical view) | Low (duels are optional; the triangle adds a meta-game layer that conflicts with technique investment decisions) | **Simplify** duels to stat + technique tier comparison; remove the weapon-type triangle |
| **5 Difficulty Levels** | Medium (requires 5 sets of tuned parameters; the jump from Disciple to Master to Grandmaster must all feel distinctly different) | Medium-Low (2–3 difficulty levels adequately serve the audience; 5 levels dilute balancing effort) | **Reduce to 4**: Initiate, Disciple, Master, Heavenly Dao. Remove Grandmaster as a distinct tier; its parameters fold into Master+ |
| **Technique Fusion / Custom Technique Creation (Library T4)** | Very High (requires a technique composition system, validation of combinations, balance testing of player-created abilities) | Low at launch (this is modding-adjacent functionality; the rest of the research tree must be complete first) | **Defer** to post-launch. Replace the T4 Martial unlock slot with a passive: "Grandmaster's Legacy — all existing techniques gain +10% effectiveness" |
| **18-Phase Development Roadmap** | Administrative overhead is fine, but some phases (0, 0.5) are sub-phases disguised as phases | Phases 0 and 0.5 are setup tasks, not deliverable features | **Consolidate** into 14 milestone phases |

#### Unclear Directions
Areas where a developer or artist must currently guess the author's intent:

| Unclear Area | Current State | Resolution in v2.0 |
|---|---|---|
| **"1:10 management ratio" enforcement** | The ratio is stated but it's unclear whether exceeding it is hard-blocked or soft-penalized with Dissent. What is the Dissent accumulation rate? Per turn? Per exceeded unit? | Specified: soft penalty. Each 10% excess above the ratio adds +2 Dissent/turn. Hard cap is 200% of ratio — any recruits above that are auto-rejected. |
| **"Supply Lines" army attrition** | States armies far from territory "consume extra Tael." What is far? What is the cost formula? Does it apply per unit, per army, per tile? | Specified: armies > 5 tiles from nearest friendly-controlled tile pay +0.5 Tael/unit/turn. Severed supply line (all paths blocked by enemies): +1.5 Tael/unit/turn. |
| **"Combat Power" formula** | Auto-resolve "compares total CP." CP is described as "sum of unit stats + technique bonuses + terrain + equipment + Elder auras" but no weights are given. | Full formula specified in §9.3 of refined GDD. |
| **"Insight" in research formula** | Research speed uses "Insight yield" but Insight is never defined as a resource or stat anywhere in the document. | Insight removed from the formula. Research speed uses Qi yield instead (which is defined). |
| **Ley Line rendering** | "Ley Lines connect Sacred Peaks across the map" — no specification of how Ley Lines are visually rendered, whether they are tiles or overlay elements, or how many exist. | Specified: Ley Lines are a procedurally generated overlay of 2–4 paths connecting Sacred Peaks. They are rendered as a particle-streamed line on a URP decal projector. Tiles within 1 hex of a Ley Line path receive the Ley Line Qi bonus. |
| **"Very-high Face buys at −10%"** | Face modifier to market uses "very-high" as a tier label but Face tiers in §10.4 use "Esteemed/Respected" etc. The mapping is inconsistent. | Resolved: Face modifier in market uses the Face Tier enum directly. Esteemed = −10%, Respected = −5%, Neutral = 0%, Diminished = +5%, Disgraced = +10%. |
| **Demonic Cult "sacrifice" mechanic** | "Can sacrifice peons/disciples for instant power boosts" — no formula for what power is gained, what the Tael/Qi equivalent is, or what the max sacrifice rate is. | Fully specified in refined §5.1: Sacrifice a Peon → +5 Qi, +1 Renown loss. Sacrifice an Outer Disciple → +20 Qi, −5 Face. Sacrifice an Inner Disciple → +60 Qi, −15 Face, triggers Jianghu Outcry event if witnessed. Max once per turn per rank tier. |
| **"Elder Council vote" for new Sect Leader** | On Sect Leader death, "Elder Council vote" happens. No mechanics for this vote are specified. | Specified: on Sect Leader death, all Elders and High Elders vote. Each High Elder = 3 votes, each Elder = 1 vote. The disciple with most votes becomes interim Sect Leader. Ties broken by highest Qi Power stat. Player participates by selecting one candidate to support (+5 votes). |
| **Neural LOD / GPU LOD** | Not mentioned anywhere in the original GDD. | Fully specified in §17 of refined GDD (DOTS-First architecture section). |
| **Carbon-Aware Development** | Not mentioned. | Fully specified in §17.6 of refined GDD. |

---
# Tales of the Tao — Game Design Document v2.0

**Genre:** 4X Turn-Based Strategy
**Engine:** Unity 6 (URP + DOTS hybrid)
**Platform:** Windows 64-bit (primary); Linux and macOS via IL2CPP (future); no mobile at launch
**Target Resolution:** 1920×1080 minimum; 4K UI scaling supported
**Inspirations:** Civilization VI (spatial districts, research moments), Stellaris (narrative events, faction identity), Mount & Blade: Bannerlord (supply chains, dynamic pricing, NPC loyalty)
**Document Version:** 2.0 — Production Ready
**Last Revised:** 2026-05-12

---

## §1. Vision Statement

> *"You have finally perceived the Tao and your discovery has led you to feel Qi. You know that you cannot be alone in the world — gather the like-minded and create your Sect. Will you be noble and wise? Will you be cunning and cruel? Reveal your fate with your own hands and tell your own Tales of the Tao."*

Tales of the Tao is a wuxia-inspired 4X turn-based strategy game where players found a martial arts sect, cultivate disciples through hierarchical ranks, research ancient techniques, and vie for dominance across a procedurally generated mythical landscape steeped in Chinese martial arts lore.

**The singular design goal:** every mechanical decision — where you found your sect, which disciples you cultivate, which techniques you research, which sects you befriend or destroy — should feel like it is expressing a coherent *identity* for your sect. The player should be able to describe their run in a sentence: "I was a ruthless Demonic Cult that sacrificed its weak to forge an elite corps of blade-masters and crushed every sect before the Dragon Year."

**The three pillars that serve this goal:**

1. **Meaningful Geography** — the hex map is not a movement grid; it is a resource allocation puzzle. Where you settle and where you expand determines what you can build, cultivate, and defend.
2. **Emergent Narrative** — events, disciple traits, zodiac cycles, and faction relationships generate stories the player didn't script. The game is a story engine, not a story delivery vehicle.
3. **Legible Consequence** — every action has a traceable causal chain. Bankruptcy cascades from upkeep. Overstretched management causes Dissent. A broken treaty triggers AI aggression. Players should never say "I don't know why that happened."

---

## §2. Victory Conditions

A game ends when any sect achieves one of three victory conditions, or at turn 150 (score-based fallback). Near-victory detection: when any sect is within 15 turns of a likely victory, the game broadcasts a **"Heavens Tremble" alert** to all players and AIs, triggering a +30% aggression modifier toward that sect from all non-allied factions for 10 turns.

| Victory | Primary Condition | Fallback Condition | Flavor |
|---|---|---|---|
| **Domination** | Control 60%+ of all active sect capitals on the map | Be the last sect with an intact Temple | Military conquest through superior martial force |
| **Enlightenment** | Complete Tier 4 of all three research branches AND construct the **Dao Sanctum** wonder at your main compound | N/A — all conditions are hard requirements | Your sect has transcended mortal limits and unified all paths of cultivation |
| **Influence** | Achieve Renown ≥ 75% of the global maximum AND maintain Friendly+ trust with ≥ 80% of all active independent settlements for 20 consecutive turns | N/A | Cultural and diplomatic supremacy — the Jianghu recognizes your sect as the righteous authority |

**Score Calculation (turn 150 fallback):**

```
Score = (Renown × 1.0) + (Controlled Tiles × 5) + (Research Nodes Completed × 15)
      + (Settlements at Friendly+ × 20) + (Active Trade Routes × 10)
      + (Elder Count × 25) + (High Elder Count × 75)
```

**Developer note:** The score formula is a balance lever (see §B). These weights should be validated against mid-game playtests at turns 60, 90, and 120 to ensure no single path creates an insurmountable score lead before turn 100.

---

## §3. Core Gameplay Loop

```
Found Sect ──► Recruit Peons ──► Train Disciples ──► Build Halls ──► Research Techniques
     │               │                  │                  │                 │
     │         Gather Resources    Deploy Units       Unlock Abilities    Advance Era
     │               │                  │                  │                 │
     └──────────────►└──────────────────►└──────────────────►└─────────────────►
                                         │
                              Expand / Diplomacy / Combat / Specialize
                                         │
                                     Victory
```

### §3.1 Turn Structure

Each turn represents one year in a **12-Year Zodiac Cycle**. The Zodiac animal of each year grants thematic strategic bonuses and modulates event probability, creating a recurring 12-turn rhythm that experienced players learn to anticipate and plan around.

**Design intent:** the Zodiac cycle is Tales of the Tao's equivalent of Civilization's Eureka system — it rewards players who align their actions with the calendar (e.g., rushing Iron Ore stockpiling before the Tiger year when combat bonuses apply). A developer should implement Zodiac bonuses as multiplier structs applied at `EventState.Enter` each turn, broadcast via the `ZodiacBonusesEventChannel` SO. All subscribing systems (SectManager, CombatResolver, EconomyManager, ResearchManager) read from this struct for the duration of the turn.

| Year | Zodiac | Zh. | Primary Effect | Secondary Effect |
|---|---|---|---|---|
| 1 | **Rat** | 鼠 | +15% Tael income | Spy detection chance −10% (rats slip through) |
| 2 | **Ox** | 牛 | +15% building construction speed | Peon gathering yield +10% |
| 3 | **Tiger** | 虎 | +15% all unit combat stats | Heavenly Tribulation failure chance −10% |
| 4 | **Rabbit** | 兔 | +15% Renown gain from all sources | Diplomatic action cooldowns −1 turn |
| 5 | **Dragon** | 龙 | +20% Qi income; rare events probability ×2 | Secret Realm rewards upgrade by one tier |
| 6 | **Snake** | 蛇 | +15% espionage success; +10% research speed | Counter-intelligence effectiveness −15% globally |
| 7 | **Horse** | 马 | +1 base movement for all units | Supply line attrition costs −25% |
| 8 | **Goat** | 羊 | +15% Medicinal and Spirit Herb yield | Medicine Hall production speed +20% |
| 9 | **Monkey** | 猴 | +15% research speed | Technique scroll drop rate from events ×1.5 |
| 10 | **Rooster** | 鸡 | +10% defense for all units | Fortification build speed +15% |
| 11 | **Dog** | 狗 | +15% settlement trust gain from all sources | Disciple loyalty — Dissent recovery rate ×2 |
| 12 | **Pig** | 猪 | +20% trade route income | Market price drift toward base ×2 per turn |

**Turn Phases (6 sequential):**

Each phase executes fully before the next begins. The TurnStateMachine drives transitions; each phase is an `ITurnState` with `Enter()`, `Tick()`, and `Exit()`.

| # | Phase | What Happens | Player Agency |
|---|---|---|---|
| 1 | **Event** | Zodiac bonuses applied; random events evaluated and fired (up to 2/turn); diplomatic notifications delivered | Respond to event modals; choose options |
| 2 | **Income** | Tael collected from trade routes + taxation; Qi collected from Temple + caves + Ley Lines; commodity yields added to stockpile; upkeep deducted | None (automatic) |
| 3 | **Build** | Construction timers tick; completed buildings finalize; training queue ticks; promotions complete | None (automatic; queue managed in Action) |
| 4 | **Research** | Research progress advances on active nodes (up to 3, one per branch); completions apply unlock effects | None (automatic; queue managed in Action) |
| 5 | **Action** | Player moves units, initiates combat, issues diplomatic actions, manages build/research queues, uses espionage | Full player control |
| 6 | **Resolution** | Combat resolves; AI sects execute their turns; trade routes rebalance; market prices drift; victory check runs | None (watch AI) |

**Developer note:** AI turn execution in Resolution Phase must be time-budgeted (see §12.2). On large maps with 9 AI sects, Resolution Phase must complete in under 500ms total. Use the `AIBudgetScheduler` to distribute inference time across sects using Unity's Job System.

---

## §4. The Hex Map

### §4.1 Tile Properties

Each hex tile has the following data attributes, stored in `HexTileData` (plain C# class, not MonoBehaviour):

| Property | Type | Description |
|---|---|---|
| **Terrain Type** | Enum (8 values) | Plains, Mountain, Forest, River, Lake, Desert, Swamp, Sacred Peak |
| **Elevation** | Enum (4 values) | Low, Medium, High, Summit — affects movement cost, visibility range, and Qi density |
| **Qi Density** | Enum (5 values) | None / Sparse / Moderate / Dense / Ley Line |
| **Cave Count** | int (0–6) | Number of training caves; type randomized at generation time |
| **Cave Types** | Enum[] | Per cave: Meditation, Body Tempering, Qi Refinement, Spirit Trial |
| **Deposits** | ResourceType[] | 0–3 resource nodes: Iron Ore, Jade, Medicinal Herbs, Spirit Herbs, Tea Leaves, Lumber |
| **Feature** | Enum | None / Ancient Ruins / Hot Spring / Spirit Vein / Bandit Camp / Wandering Master Spawn |
| **Control** | Enum | Unowned / Sect Territory / Settlement Influence / Contested |
| **Fortification** | Enum | None / Watchtower / Garrison / Fortress |
| **Ley Line** | bool | True if this tile lies within 1 hex of a generated Ley Line path |
| **Settlement ID** | int (nullable) | If occupied by an independent settlement |

**Horses removed as a commodity.** Horses are now a terrain-linked movement bonus: Plains and Desert tiles within a player's territory that are unoccupied by buildings passively grant +1 mounted unit movement if the sect has researched "Cavalry Techniques" (Martial Techniques Tier 2). No inventory slot required. This eliminates commodity management overhead with no strategic loss.

### §4.2 Terrain Effects

| Terrain | Move Cost | Defense Bonus | Qi Modifier | Special Rule |
|---|---|---|---|---|
| Plains | 1 | +0% | ×1.0 | Best for large sect compounds; farms grant +2 Tael/turn |
| Forest | 2 | +25% | ×1.1 | Lumber deposit source; ambush: attacking from Forest is undetected until melee range |
| Mountain | 3 | +50% | ×1.5 | Caves most common (avg 2–4 per tile); ideal for Kunlun and Wu Dang |
| Sacred Peak | 4 | +75% | ×2.0 | Rare (≤2% of map); natural Ley Line anchor; Dao Sanctum can only be built here |
| River | 1.5 | +0% | ×1.2 | +15% trade route income if route crosses this tile; penalty: +0.5 cost to cross |
| Lake | Impassable | — | ×1.3 | Fishing: adjacent tiles gain +3 Tael/turn; blocks line of sight |
| Desert | 2 | +0% | ×0.5 | Low Qi; mounted units move at cost 1 here; harsh attrition: +1 Tael/unit/turn upkeep |
| Swamp | 3 | −10% | ×0.8 | Medicinal Herbs guaranteed deposit; poison effects in combat from ambush |

**Elevation modifiers** (stacked on top of terrain):

| Elevation | Visibility Range Bonus | Additional Move Cost | Qi Modifier |
|---|---|---|---|
| Low | +0 | +0 | ×1.0 |
| Medium | +1 tile | +0 | ×1.1 |
| High | +2 tiles | +1 | ×1.3 |
| Summit | +3 tiles | +2 | ×1.5 (stacked with terrain) |

### §4.3 Map Generation

**Map sizes and tile counts:**

| Size | Dimensions | Tiles | Max Sects | Target AI Turn Budget |
|---|---|---|---|---|
| Small | 60×40 | 2,400 | 4 | 16ms |
| Medium | 80×60 | 4,800 | 6 | 25ms |
| Large | 120×80 | 9,600 | 8 | 33ms |
| Epic | 160×100 | 16,000 | 10 | 50ms |

**Generation algorithm (developer specification):**

A developer should implement map generation as a sequential pipeline of passes, each operating on the full tile array:

1. **Elevation Pass:** 2D Perlin noise (octaves: 4, persistence: 0.5, lacunarity: 2.0, scale: 0.08). Normalize to [0,1]. Quantize: <0.2 = Low, 0.2–0.5 = Medium, 0.5–0.8 = High, >0.8 = Summit.

2. **Biome Pass:** Place N Voronoi seed points (N = map width / 15, minimum 8). Each seed is randomly assigned a terrain type weighted by elevation: Low→Plains/River/Swamp, Medium→Forest/Plains, High→Mountain/Forest, Summit→Mountain/Sacred Peak. Each tile is assigned to the nearest seed's terrain type.

3. **Sacred Peak Rarity Enforcement:** After biome assignment, count Sacred Peak tiles. If >2% of total tiles, randomly downgrade excess Sacred Peaks to Mountain. Guarantee at least 2 Sacred Peaks on any map.

4. **Ley Line Pass:** For each pair of Sacred Peaks (if 2 peaks: 1 line; if 3: 2 lines; if 4+: 3 lines, maximum), generate a winding path using A* with randomized cost noise. Mark all tiles within 1 hex of the path as `LeyLine = true` and `QiDensity = LeyLine`.

5. **Starting Location Pass:** For each player/AI start, use maximum-spread placement (iterative furthest-point sampling). Guarantee: starting tile is Plains or Forest, elevation Low or Medium, QiDensity ≥ Moderate, CaveCount ≥ 1. If no valid tile exists within 5 tiles of the spread point, relax constraints in order.

6. **Settlement Seeding:** Place independent settlements in fertile lowlands (Plains, River, elevation Low/Medium, not adjacent to player starts). Settlement density: 1 per 150 tiles (Small map: ~16 settlements).

7. **Resource Deposit Pass:** Each terrain type has a deposit probability table. Apply per tile: Mountain → 40% Iron Ore, 20% Jade; Forest → 35% Lumber, 25% Medicinal Herbs; Swamp → 60% Medicinal Herbs; Sacred Peak + Ley Line tiles → 30% Spirit Herbs.

**Chunk system:** The grid is subdivided into 16×16 hex chunks. Each chunk is a single combined mesh for rendering. Chunks outside the camera frustum are culled. Chunks at the edge of the visible range use LOD meshes (see §17.3).

---

## §5. Sects

### §5.1 Playable Sects

Ten canonical sects plus the custom sect creator. Each sect has an **Affinity** (starting technique style and pool), a **Sect Trait** (passive bonus), and a **Unique Hall** (replaces one standard building with an upgraded variant). All sect stats are defined in `SectConfigSO` ScriptableObjects and are hot-reloadable.

**Design intent:** each sect should have a clearly legible identity that shapes the player's entire strategy. A developer building the UI should ensure that on the Sect Selection screen, a first-time player can read the three fields (Affinity, Trait, Unique Hall) and form a mental model of how they will play. No two sects should share the same dominant strategy.

| Sect | Affinity | Sect Trait | Unique Hall | Dominant Strategy |
|---|---|---|---|---|
| **Wu Dang** | Internal Qi (Taiji) | +20% Qi income; Meditation caves yield ×1.5 Qi | **Taiji Pavilion** (replaces Training Grounds) — disciples train 25% faster; all trained disciples begin with +10% defensive technique effectiveness | Cultivation depth; defensive endgame |
| **Shaolin** | Body Cultivation | +15% all disciple HP; immune to attrition during adverse terrain movement | **Pagoda of 108 Trials** (replaces Training Grounds) — disciples gain a permanent +5% HP per completed cave trial, up to 3 stacks | Tanky front line; war of attrition |
| **Tang Clan** | Poison / Hidden Weapons | +20% assassination success; own disciples immune to all poison effects | **Poison Hall** (replaces Medicine Hall) — produces poisons AND standard medicine; unique Hidden Weapon items assignable to any disciple as a third equipment slot | Espionage; assassination economy |
| **Mount Hua** | Sword Arts | +10% sword technique damage; +1 movement on Mountain tiles | **Sword Peak** (replaces Library) — all Martial Technique research costs −30% Qi; one additional Sword technique slot per Inner Disciple | Elite melee; aggressive expansion |
| **Emei** | Balanced (Sword + Qi) | +15% Renown from diplomacy and settlement requests | **Lotus Hall** (replaces External Affairs Hall) — settlement trust gain ×1.5; can send Influence Emissaries (costs 20 Tael; +10 trust at target settlement) | Influence victory; peaceful expansion |
| **Kunlun** | Ice / Elemental Qi | +20% defense on Mountain and Sacred Peak tiles; all bonuses ×2 during Tiger (year 3) and Rooster (year 10) zodiac years | **Frozen Meridian Chamber** (replaces cave system — all caves on founding tile become Qi Refinement type) | Zodiac timing; fortified territories |
| **Peng Clan** | Movement / Lightness | +2 base movement for all units; +25% trade route income | **Courier Network** (replaces External Affairs Hall) — diplomatic cooldowns reduced by 2 turns; can establish trade routes with Wary-trust settlements (normally requires Friendly) | Mobility; trade economy; early scouting |
| **Namgung** | Sword / Noble Arts | +10% Tael income; +15% Renown from combat victories | **Hall of Prestige** (replaces Elder Council) — each Elder passively generates +3 Renown/turn; Elder specialization bonuses are +25% stronger | Economy + prestige compounding |
| **Demonic Cult** | Forbidden Arts | Sacrifice actions available (see below); base Renown gains ×0.5; cannot establish Alliances (can form Pacts and Rivalries) | **Blood Altar** (unique — cannot be built by any other sect) — converts captured disciples into Qi or resources; enables Forbidden Technique research branch (5 nodes, Demonic Cult exclusive) | Fast early power; diplomatic pariah |
| **Imperial Palace** | Imperial Authority | Starts with ×2 Tael; can levy Settlement Tax (15% of settlement trade income, requires Friendly trust) | **Imperial Court** (replaces Elder Council) — issues Edicts (cooldown: 5 turns) that apply a chosen effect to all sects within 10 tiles for 3 turns | Domination via economic leverage |

**Demonic Cult Sacrifice Actions** (fully specified):

| Sacrifice Target | Qi Gained | Renown Change | Face Change | Additional Effect |
|---|---|---|---|---|
| Peon (10 peons) | +5 Qi | −1 | 0 | None |
| Outer Disciple | +20 Qi | −3 | −2 | None |
| Inner Disciple | +60 Qi | −8 | −8 | If witnessed by another sect's spy or unit: triggers **Jianghu Outcry** — all non-allied sects gain +20 aggression toward Demonic Cult for 5 turns |
| Captured Enemy Disciple | +15 Qi per rank tier | −2 | −5 | Converts enemy to Qi resource without imprisonment cost |

Limit: one sacrifice action per rank tier per turn (so maximum: 1×Peon batch + 1×Outer + 1×Inner per turn, costing the sacrificed disciples from the roster).

### §5.2 Custom Sect Creator

**Developer note:** implement the custom sect creator as a separate `SectCreatorScreen` with live preview of sect stats. All choices produce a `SectConfigSO` asset at runtime (or a runtime equivalent struct if SO creation at runtime is not desired — use a `CustomSectConfig` plain C# class that implements the same `ISectConfig` interface as `SectConfigSO`).

Players configure:

1. **Name and Banner** — text input (max 24 chars); color picker (HSV) for banner primary/secondary color; emblem selector (16 preset emblems).

2. **Affinity** — single pick from the 10 canonical affinities, OR blend two at ×0.75 effectiveness each. Developer note: blended affinity means the technique pool is the union of both affinities' pools, but all technique stat bonuses are multiplied by 0.75.

3. **Sect Trait** — choose one from a curated list of 12 traits (each canonical sect's trait is available, minus Demonic Cult's sacrifice and Imperial Palace's Edict traits, which are exclusive to those sects).

4. **Unique Hall** — choose one replacement from the available pool (again minus Blood Altar and Imperial Court, which are exclusive).

5. **Origin Story** — choose one of 6 origins; each grants a minor starting bonus and a flavor paragraph used in the session's opening narrative event:

| Origin | Starting Bonus | Narrative Theme |
|---|---|---|
| Exiled Noble | +100 Tael, −5 starting Face | Disgraced but determined; reclaim honor |
| Mountain Hermit | +30 Qi, +1 cave at founding tile | Alone with nature; emerging to reshape the world |
| Wandering Martial Artist | +1 Outer Disciple at start, +5 Face | Already known; reputation precedes you |
| Merchant House | +2 trade routes unlocked at start | Wealth before power; buy your way to supremacy |
| Fallen Sect | +1 technique scroll (random Tier 1–2) | The last survivor; rebuild from ruins |
| Holy Shrine | +10 Spirit Herbs, +5 Renown | Sacred mandate; destiny is already written |

---

## §6. Sect Management

### §6.1 Founding

1. Game begins: player controls a **Founder Unit** and a **Support Unit** (5 Peons). The Founder has 3 movement per turn.
2. The Founder must move to a valid founding tile (any non-Lake, non-impassable tile) and execute **Found Sect** (available in the Action Phase context menu).
3. Founding requirements: the player has at least 50 Tael and the tile is not within 8 hexes of another sect's Temple.
4. On founding: the `FoundSectCommand` executes — creates `SectData`, places the Temple building instance on the tile, removes the Founder from the unit roster (Founder becomes Sect Leader NPC), and bakes the following permanent founding bonuses into `SectData`:
   - `BaseQiIncome = tile.QiDensity.ToFloat() × 10` where None=0, Sparse=5, Moderate=10, Dense=15, LeyLine=25
   - `CaveBonus = tile.CaveCount × 2` (added to Qi income)
   - `TerrainDefenseBonus = tile.Terrain.DefenseBonus` (permanent fortification bonus to the founding tile)
5. After founding, the Support Unit can begin recruiting Peons from nearby settlements (within 5 tiles). Each Peon costs 10 Tael and takes 1 turn to arrive.

**Developer note:** founding tile baked bonuses are stored in `SectData.FoundingTileStats` (a plain C# struct) and never recalculated — they represent permanent advantages that cannot be taken away by tile control changes. This is intentional: founding location is the most important long-term decision in the game and should have lasting weight.

### §6.2 Disciple Hierarchy

Disciples are the sect's core resource. The hierarchy enforces a **1:10 management ratio** — each rank can manage up to 10 of the rank below it. This ratio is a **soft cap enforced by Dissent**, not a hard block.

**Management Ratio Enforcement:**
- For each 10% excess above the ratio, `SectData.DissentLevel += 2` per turn.
- Hard cap: when any rank is at 200% of its ratio (twice as many subordinates as the cap allows), the Build Phase rejects all new recruitment of that rank until the ratio is resolved.
- Dissent recovery: 5 points per turn when all ratios are in bounds; 10 points per turn during Dog year (§3.1).

| Rank | Role | Ratio | Promotion Path | Upkeep/Turn |
|---|---|---|---|---|
| **Peon** | Labor, resource gathering, construction | 10 per Outer Disciple | Recruited from settlements (10 Tael each, 1 turn) | 1 Tael |
| **Outer Disciple** | Basic combat, patrol, scouting, settler | 10 per Inner Disciple | Training Grounds T1 (see §6.2.1) | 3 Tael |
| **Inner Disciple** | Core fighting force, technique users | 10 per Elder | Disciple Hall T1 (see §6.2.1) | 8 Tael |
| **Elder** | Hall specialists, army commanders, diplomats | 10 per High Elder | Elder Council T1 (see §6.2.1) | 20 Tael |
| **High Elder** | Branch sect governance, army leadership | No ratio cap | Elder Council T3 (see §6.2.1) | 50 Tael |
| **Sect Leader** | Passive sect-wide bonus; NPC | 1 per sect | Elected by Elder Council on death | 0 Tael (covered by Temple) |

#### §6.2.1 Promotion Costs

All costs are paid upfront at queue time. Resources are deducted immediately; timers begin on the next Build Phase. Failure paths exist only at the Elder → High Elder transition (Heavenly Tribulation, §14).

| Transition | Hall Required | Tael | Qi | Lumber | Iron Ore | Jade | Spirit Herbs | Build Turns |
|---|---|---|---|---|---|---|---|---|
| Peon → Outer | Training Grounds T1 | 10 | 0 | 0 | 0 | 0 | 0 | 8 |
| Outer → Inner | Disciple Hall T1 | 25 | 15 | 0 | 2 | 0 | 0 | 15 |
| Inner → Elder | Elder Council T1 | 60 | 40 | 0 | 0 | 3 | 0 | 30 |
| Elder → High Elder | Elder Council T3 | 150 | 100 | 0 | 0 | 0 | 5 | 50 |

**Management cap example:** 1 Sect Leader governs 5 High Elders who govern 50 Elders who command 500 Inner Disciples who lead 5,000 Outer Disciples who direct 50,000 Peons. In practice, early-to-mid game sects operate at 10–100 total disciples. Late game elite sects may reach 200–500.

### §6.3 Sect Buildings

All buildings are constructed at the sect's **main compound** or **Branch Sect Outpost** (see §6.3.1). Multiple buildings can be under simultaneous construction; the simultaneous build limit equals the Temple's current tier (T1=1, T2=2, T3=3).

Build costs scale by tier. Costs are cumulative (upgrading T1→T2 costs the T2 cost only; the T1 cost was already paid). All costs defined in `BuildingConfigSO` assets.

| Building | Prerequisite | Core Function | T1 Specifics | T2 Specifics | T3 Specifics |
|---|---|---|---|---|---|
| **Temple** | None (founded with sect) | HQ; Qi income; build slot cap | Qi +10/turn; 1 build slot; houses Sect Leader | Qi +20/turn; 2 build slots; +5% all income | Qi +35/turn; 3 build slots; unlocks Dao Sanctum wonder option |
| **Training Grounds** | Temple T1 | Peon→Outer promotion; basic training | 8-turn training; capacity 5/batch | 6-turn training; capacity 8/batch; +5% combat stats | 5-turn training; capacity 10/batch; all recruits begin with 1 free Tier 1 technique |
| **Disciple Hall** | Training Grounds T1 | Outer→Inner promotion; technique assignment | 15-turn promotion; 1 technique slot per Inner | 12-turn promotion; 2 technique slots per Inner | 10-turn promotion; enables Dual-Path technique (both slots from one affinity tree) |
| **Library** | Disciple Hall T1 | Research hub; scroll storage; Wisdom Elder home | Unlocks research queue; stores 5 scrolls | Research speed +30%; stores 15 scrolls | Can copy captured enemy scrolls (1-time use → permanent unlock); stores 30 scrolls |
| **Elder Council** | Library T2 | Inner→Elder and Elder→High Elder promotion; sect policy | Unlocks Elder promotion; 1 Elder Council seat | +1 Elder Council seat; Elders generate +2 Renown/turn | Unlocks High Elder promotion; High Elders can govern Branch Sects autonomously |
| **External Affairs Hall** | Temple T1 | Diplomacy; spy deployment; trade negotiation | Basic diplomacy; 1 active spy | +2 trade route limit; spy detection resistance +15% | Alliance-level diplomacy unlocked; joint declarations; multi-sect trade networks |
| **Medicine Hall** | Training Grounds T1 | Healing items; cultivation pills; Alchemy research | Herbal medicine production; basic Alchemy research | Spirit Herb refining; advanced pill production | Breakthrough pills (aid Elder promotions); Alchemy Tier 4 unlocked |
| **Armory** | Training Grounds T1 | Weapons; armor; Forge research | Basic weapon production; Forge research | Refined weapons (+8% attack stat); chainmail (+8% defense) | Masterwork weapon production; Sect Treasure Forging unlocked |
| **Market Pavilion** | External Affairs Hall T1 | Commodity trading; Tael conversion | Base market with T1 markups | +2 trade route income; market markup reduced to T2 | Trade monopoly option (one commodity per compound); removes all markups |
| **Branch Sect Outpost** | Elder Council T1 + Settler Party | Territory extension; mini-compound | Basic gathering; Peon housing | Can build Training Grounds and Medicine Hall | Can build all non-wonder buildings; full independent build queue |

#### §6.3.1 Branch Sect Outposts

A Branch Sect is founded by a **Settler Party** (1 Outer Disciple settler + 10 Peons, consumed on founding). The Settler Party is created in the Action Phase from the main compound and takes 3 turns to assemble. The party can then move to a valid tile and execute **Found Branch Sect**.

**Developer note:** at T3, a Branch Sect gets its own build queue managed by the player directly — there is no autonomous AI governance. This was listed as a deferred feature in the Kill List. The player selects the Branch Sect from the map and opens its build panel just as they would the main compound. Each Branch Sect is a separate `SectCompound` instance in `SectData.Compounds[]`.

### §6.4 Elder Specialization

When promoted to Elder, a disciple is assigned to one Hall (or left as a General Elder). Specialization is permanent unless an Elder is re-assigned at the cost of 30 Tael and 5 turns of transition time.

| Specialization | Hall | Bonus |
|---|---|---|
| **War Elder** | Training Grounds or Armory | Trained disciples +10% combat stats; weapon production −20% turns |
| **Wisdom Elder** | Library | Research speed +3 per Wisdom Elder at this compound; technique scroll analysis: once per 10 turns, identify an unidentified scroll for free |
| **Medicine Elder** | Medicine Hall | Pill quality tier +1 (Common→Uncommon→Rare); herb yield +15% |
| **Diplomacy Elder** | External Affairs Hall | Renown gain +5/turn; trade negotiation success chance +20% |
| **Discipline Elder** | Disciple Hall | Training speed +15% for all queued promotions at this compound; Dissent recovery +3/turn |
| **Forge Elder** | Armory | Weapon/armor stat bonus +5%; unlocks experimental equipment prototypes (one per Forge Elder, testable in tactical combat) |

**General Elder (unspecialized):** provides no hall bonus but can be deployed as an army commander (+5% combat stats aura to all friendly units in the same army stack), or as a travelling diplomat (counts as +10 Tael gift equivalent when sent to a settlement as an envoy).

### §6.5 Technique Assignment

**Assignment occurs automatically** when a disciple reaches Inner rank: the system draws from the sect's `SectConfigSO.AffinityWeaponTechniques` and `AffinityQiTechniques` lists in priority order (first unresearched technique is skipped; the first researched and unassigned technique is assigned).

**Developer note:** `TechniqueAssigner.Assign(Disciple d, SectData sect)` should be called from `CompletePromotion(PromotionStep.OuterToInner)`. It must check that the technique is both researched (exists in `ResearchManager.UnlockedTechs`) and available (not over the assignment cap). If no technique is available in the affinity pool, the disciple receives a generic "Basic Qi Circulation" technique as a fallback.

Technique slots per Inner Disciple by Disciple Hall tier:
- **T1 Disciple Hall:** 1 Weapon Technique + 1 Combat Technique
- **T2 Disciple Hall:** 1 Weapon + 1 Combat + 1 Auxiliary (movement or defense technique)
- **T3 Disciple Hall:** enables Dual-Path (both slots can be from the same affinity tree; can hold 1 Weapon + 2 Combat or 2 Weapon + 1 Combat)

Additional technique acquisition:
- Researched at Library and manually assigned via Disciple Detail screen (Action Phase only)
- Acquired from captured technique scrolls (enemy ruins, ancient tombs, espionage)
- Taught by Wandering Masters (event: costs 50 Tael; grants one technique to up to 5 Inner+ disciples present at the Master's tile)
- Purchased from Hermit's Dwelling settlements (one-time; technique tier corresponds to settlement trust level)

---

## §7. Economy

### §7.1 Resources Overview

| Resource | Type | Unit | Primary Source | Primary Sink |
|---|---|---|---|---|
| **Tael** | Currency | Silver coins | Trade routes, market, taxation, tribute | Building costs, upkeep, diplomacy, recruitment |
| **Qi** | Cultivation energy | Abstract units | Temple, caves, Ley Lines, meditation | Promotion costs, research, technique use in combat |
| **Lumber** | Commodity | Bundles | Forest deposits, peon gathering | Building construction (T1–T2) |
| **Iron Ore** | Commodity | Units (includes Rare Earth quality flag) | Mountain deposits | Building (Armory), promotion (Outer→Inner), weapons |
| **Jade** | Commodity | Pieces | Mountain/Sacred Peak deposits | High-tier buildings, diplomacy gifts, Inner→Elder promotion |
| **Medicinal Herbs** | Commodity | Bundles | Forest/Swamp deposits, Goat year bonus | Medicine Hall, healing items, basic pills |
| **Spirit Herbs** | Commodity | Rare bundles | Sacred Peak/Ley Line deposits | Breakthrough pills, Alchemy T3–T4, Elder→High Elder promotion |
| **Tea Leaves** | Commodity | Boxes | Plains/River (warm climate) deposits | Trade (highest Tael/unit value), settlement gifts, morale boost |

**Rare Earth removed as a separate commodity.** Iron Ore deposits in Mountain tiles at High or Summit elevation have a 30% chance of being flagged as "High-Grade Iron" (Rare Earth equivalent). High-Grade Iron counts as ×2 Iron Ore for Armory T3 recipes and Sect Treasure forging. This distinction is tracked as a `ResourceQuality` enum on the stockpile entry, not a separate commodity type.

### §7.2 Economy Sources and Sinks — Mathematical Framework

The economy is designed with three distinct phases: early growth, mid-game equilibrium, and late-game pressure. Every Tael source and sink is defined below with its formula so that the `EconomyManager` implementation is unambiguous.

#### §7.2.1 Tael Sources

| Source | Formula | Notes |
|---|---|---|
| **Base Temple Income** | `TempleIncome = TemplateTier × 5` | T1=5, T2=10, T3=15 Tael/turn |
| **Trade Route** | `RouteIncome = BaseValue × DistanceMultiplier × CommodityDifferential` where `BaseValue = 10`, `DistanceMultiplier = 1 + (distance_in_hexes / 20)`, `CommodityDifferential = 1 + (count of unique commodities partner has that you don't × 0.1)` | Capped at 3× base value per route |
| **Market Pavilion Sales** | `SaleIncome = qty × CurrentSellPrice` where `CurrentSellPrice = BaseBuyPrice × 0.60 × (1 − 0.05 × qty_sold_this_turn)` | Per transaction |
| **Settlement Tribute** | `Tribute = TrustMultiplier × SettlementTier × 5` where Friendly=×1, Devoted=×1.5; Village=1, Town=2, Trade Post=3 | Collected during Income Phase; requires Friendly+ trust |
| **Settlement Tax** (Imperial Palace only) | `Tax = TradeRouteIncome × 0.15` per connected settlement | Requires Friendly trust; reduces trust gain rate by −3/turn |
| **Pig Year Bonus** | `PigBonus = TotalTradeRouteIncome × 0.20` | Applied during Income Phase in Pig year only |
| **Namgung Base Bonus** | `+10%` applied to `TempleIncome + TradeRouteIncome` | Sect trait |

#### §7.2.2 Tael Sinks

| Sink | Formula | Notes |
|---|---|---|
| **Disciple Upkeep** | `Upkeep = sum(Disciple.UpkeepPerTurn)` across all disciples in all compounds | Peon=1, Outer=3, Inner=8, Elder=20, High Elder=50 Tael/turn |
| **Building Upkeep** | `BuildUpkeep = sum(BuildingInstance.UpkeepPerTurn)` | Per building per compound; see `BuildingConfigSO.TierUpkeep[]` |
| **Supply Line Surcharge** | `SupplyExtra = max(0, distance_from_nearest_friendly_tile − 5) × 0.5 × unitCount` | Applied per army stack during Income Phase |
| **Severed Supply Line** | `SeveredExtra = 1.5 × unitCount` per turn | When all paths to friendly tiles are blocked |
| **Building Construction** | One-time cost at queue time | Deducted from `SectData.Tael` when construction is queued |
| **Promotion Costs** | One-time at queue time | Per §6.2.1 table |
| **Diplomatic Gifts** | Player-chosen amount | Sent via `SendGiftCommand` |
| **Spy Deployment** | 20 Tael per spy per mission activation | Covers mission expenses; recurs if spy is re-assigned |
| **Tournament Hosting** | 100 Tael flat cost to host | See §14 events |

#### §7.2.3 Balancing Table — Tael Economy by Phase

The following table assumes a standard Medium map, 1 player + 5 AI sects, Disciple difficulty. Numbers represent a representative mid-strength single compound player.

| Phase | Typical Income/Turn | Typical Upkeep/Turn | Net Tael Flow | Design Intent |
|---|---|---|---|---|
| **Early (T1–30)** | 25–60 Tael | 10–25 Tael | +15 to +35 surplus | Player feels momentum; can invest in buildings |
| **Mid (T30–80)** | 60–150 Tael | 55–140 Tael | ±5 to ±20 | Trade-off zone; must choose expansion vs. army vs. research |
| **Late (T80+)** | 150–350 Tael | 140–400 Tael | −50 to +50 | Sink pressure; must expand trade or specialize to remain solvent |

**Breakdown of typical early-game (T15) player:**
- Temple T1: +5
- 1 trade route (distance 8): +10 × 1.4 = +14
- Settlement tribute (1 Friendly village): +5
- **Total income: 24 Tael/turn**
- 20 Peons: −20 Tael
- 5 Outer Disciples: −15 Tael
- Temple upkeep: −3 Tael
- **Total upkeep: −38 Tael/turn** ← THIS IS A KNOWN EARLY SQUEEZE

**Early-game squeeze mitigation:** the player starts with 200 Tael (or 400 for Imperial Palace). The intent is that during T1–15, players are spending their reserves while building their first trade route. By T15–20, the first trade route + settlement tribute should flip the player to net-positive. The `EconomyManager` should log a warning in debug builds when a new player reaches T10 with Tael < 50, as this indicates the starting configuration is too punishing.

#### §7.2.4 Anti-Inflation Mechanics

When a player accumulates extreme Tael surplus (>1,000 Tael above upkeep for more than 5 consecutive turns), the following automatic mechanisms activate:

| Mechanism | Trigger | Effect | Cap |
|---|---|---|---|
| **Luxury Upkeep Scaling** | Tael surplus > 500 | High Elder upkeep +10 Tael/turn per 500 surplus (representing political demands) | +50 Tael/turn max |
| **Sect Renown Taxation** | Tael surplus > 1,000 AND Renown > 50% | Nearby sects (Diplomat AI first) demand "charitable contributions" — a diplomatic event offering 100 Tael for +15 relations, or refusal for −10 relations | Fires once per 10 turns |
| **Settlement Price Inflation** | Player controls >40% of all resource deposits | Settlement buy prices rise 15% (they resent the monopoly) | Returns to normal when <35% control |
| **Market Saturation** | Player sells >5 units of the same commodity in one turn | Sell price floor: each unit beyond the 5th sells for base price ×0.3 instead of ×0.6 | Resets each turn |

### §7.3 Qi Economy

Qi is the cultivation resource. Unlike Tael, Qi does not have runaway inflation — it is consumed at promotion and research, creating a natural flow.

#### §7.3.1 Qi Sources

| Source | Formula | Notes |
|---|---|---|
| **Temple** | `TempleQi = FoundingTileQiBase + TempleTier × 10` | Founding baked bonus + tier bonus |
| **Ley Line Tiles** | `LeyLineBonus = LeyLineTileCount × 5 Qi/turn` | Tiles within 1 hex of a Ley Line path that are within your territory |
| **Caves (per cave)** | Meditation=3, Body Tempering=2, Qi Refinement=5, Spirit Trial=8 Qi/turn | Only caves in your territory count |
| **Dragon Year** | `×1.20` multiplier on all Qi income | Applied during Income Phase |
| **Wu Dang Trait** | `×1.20` on Temple + Meditation cave Qi | Sect trait; stacks with Dragon year |

#### §7.3.2 Qi Sinks

| Sink | Cost | Trigger |
|---|---|---|
| **Research Progress** | Consumed per turn at `CalculateResearchSpeed()` | Continuous during Research Phase |
| **Promotion Costs** | Upfront per promotion (§6.2.1) | At queue time |
| **Technique Scroll Analysis** | 10 Qi | To identify a captured scroll's content |
| **Breakthrough Pills** | Requires Qi stockpile (pill consumed, Qi effectively stored as an item) | Produced at Medicine Hall T3 |

#### §7.3.3 Qi Balancing Table

| Phase | Typical Qi Income/Turn | Typical Qi Spend/Turn | Net | Design Intent |
|---|---|---|---|---|
| **Early (T1–30)** | 15–35 | 5–20 (research only) | +10 to +20 | Build stockpile for first promotions |
| **Mid (T30–80)** | 35–80 | 30–70 (research + promotions) | ±5 to ±15 | Qi is the binding constraint — players must choose: research OR promote, rarely both at full speed |
| **Late (T80+)** | 80–200 | 100–250 (Tier 3–4 research + High Elder promotions) | −20 to +50 | Qi becomes a strategic resource requiring active management |

### §7.4 Commodity Economy — Supply Network

**Design intent:** commodities should feel like physical goods that flow through space, not abstract sliders. A forest settlement producing Lumber is a strategic asset. Losing control of Iron Ore deposits mid-war should hurt.

**Commodity flow model:**
1. Deposits on tiles within a sect's territory produce commodities automatically during Income Phase. Production rate: 2 units/turn for Common deposits (Iron Ore, Lumber, Medicinal Herbs), 1 unit/turn for Uncommon (Jade, Tea Leaves), 0.5 units/turn for Rare (Spirit Herbs).
2. Deposits on tiles connected to your territory by an unbroken road (built by Peons) produce at ×1.5 rate (efficient supply chain).
3. Deposits on contested tiles produce 0.
4. Commodities stockpile in `SectData.Stockpile`. There is no spoilage mechanic (this was considered and cut for complexity).
5. Trade routes carry the commodity differential — if your partner has Jade and you don't, the route automatically trades Jade for Tael (you don't track the commodity transfer; only the Tael income is computed). This simplification is intentional at launch.

### §7.5 Marketplace Pricing

The Marketplace runs at each compound with an active Market Pavilion. Each commodity has a base equilibrium price. Prices drift toward equilibrium by 5% per turn.

| Commodity | Base Buy Price (Tael) | Base Sell Price (60%) | Common Sources |
|---|---|---|---|
| Tea Leaves | 5 | 3 | Plains/River plains |
| Lumber | 8 | 4.8 | Forest deposits |
| Medicinal Herbs | 10 | 6 | Forest, Swamp |
| Iron Ore | 12 | 7.2 | Mountain deposits |
| Jade | 20 | 12 | Mountain/Sacred Peak |
| Spirit Herbs | 25 | 15 | Sacred Peak, Ley Line tiles |

**Price adjustment formulas:**
- **Buy transaction:** `NewPrice = CurrentPrice × (1 + 0.10 × qty_purchased_this_turn)`
- **Sell transaction:** `NewPrice = CurrentPrice × (1 − 0.05 × qty_sold_this_turn)`
- **Turn drift:** `CurrentPrice = lerp(CurrentPrice, BasePrice, 0.05f)`
- **Pavilion tier markup:** T1: `BuyPrice × 1.20`; T2: `BuyPrice × 1.10`; T3: `BuyPrice × 1.00`
- **Face modifier (buy only):** Esteemed: `×0.90`; Respected: `×0.95`; Neutral: `×1.00`; Diminished: `×1.05`; Disgraced: `×1.10`

**Developer note:** the MarketSimulator is per-compound (not global). Each compound with a Market Pavilion runs its own price simulation. The "simplified global market" described in the original GDD is replaced by this per-compound model, which creates locational price differences that reward trade route planning. If a developer wishes to implement a global price signal (e.g., when Iron Ore is scarce globally, all markets see a mild uptick), this can be added post-launch as a `GlobalMarketPressureSystem` that applies a ±10% modifier to all local base prices.

---

## §8. Research & Technology

Research divides into three branches. Up to one tech per branch can be actively researched simultaneously (three total across the sect). Research cost is paid in Qi over time; there is no upfront Qi cost — Qi is consumed from income each turn until the node is funded.

**Research speed formula:**
```
Speed (Qi/turn consumed toward research) =
  (QiYieldPerTurn × 0.40)           // 40% of Qi income dedicated to research
  + (LibraryTier × 3)               // Library tier bonus
  + (WisdomElderCount × 4)          // per Wisdom-specialized Elder at this compound
  × (1 + 0.10 × LeyLineTileCount)  // Ley Line bonus
  × DifficultyModifier              // see §12.3
```

**"Insight" removed:** the original formula referenced "Insight yield" which was undefined. The revised formula uses QiYieldPerTurn directly, which is a defined, displayed stat.

**Eureka Moments (new — see §1.1 gap analysis):** each research node has one optional **Enlightenment Trigger** — a specific in-game action that, when completed, grants 50% research progress toward that node instantly. Examples:

| Tech Node | Enlightenment Trigger |
|---|---|
| Iron Smelting | Control a tile with an Iron Ore deposit |
| Basic Sword Arts | Win 3 tactical combat rounds using the default sword technique |
| Herbal Medicine | Collect Medicinal Herbs from a Swamp tile |
| Qi Projection | Have a disciple complete a Qi Refinement cave trial |
| Cavalry Techniques | Have a mounted unit traverse 5 Desert tiles in one turn |
| Formation Arrays | Field an army of 3+ Inner Disciple Squads simultaneously |

**Developer note:** Enlightenment Triggers are checked by the `EnlightenmentTriggerSystem`, which subscribes to relevant game events (unit movement, resource collection, combat resolution). When a trigger fires, it calls `ResearchManager.ApplyEnlightenmentBonus(techNode, 0.50f)`. Triggers can only fire once per node. This system is an observable addition — it does not replace the continuous Qi expenditure model; it accelerates it.

**Hall Tier Gating:**
- Tier 1–2 research: requires relevant Hall at T1+
- Tier 3 research: requires relevant Hall at T2+
- Tier 4 research: requires relevant Hall at T3+

**Renown awarded on completion:** T1 = +5, T2 = +10, T3 = +20, T4 = +40. Completing all three branches at T4 awards an additional +50 Renown and unlocks the **Dao Sanctum** building option (buildable only at Temple T3 on a Sacred Peak tile; see §2 Enlightenment victory).

**Qi cost per tier (total node cost, consumed over time):**

| Tier | Total Qi Cost per Node | Approx. turns at baseline speed |
|---|---|---|
| Tier 1 | 30 Qi | 5–8 turns |
| Tier 2 | 80 Qi | 12–18 turns |
| Tier 3 | 180 Qi | 25–35 turns |
| Tier 4 | 400 Qi | 50–70 turns |

### §8.1 Alchemy Branch (Medicine Hall)

```
Tier 1                  Tier 2                    Tier 3                     Tier 4
──────                  ──────                    ──────                     ──────
Herbal Medicine ───────► Qi Restoration Pills ───► Spirit Condensation ─────► Dao Heart Pill
  [Trigger: collect        [Trigger: heal a           [Unlocks: advanced         [Unlocks: Dao Heart
   Medicinal Herbs          disciple with pills]        pill crafting]             Pill item; +10%
   from Swamp]                    │                          │                    all disciple
        │                         ├─ Poison Resistance ─► Miasma Warfare ────► Plague of Shadows
        ├─ Antidote Craft ────────┘  [Unlocks: poison       [Unlocks: poison      [Unlock: area
        │                             immunity item]          attack in combat]     poison event]
        └─ Herb Cultivation ─────► Spirit Herb Farming ──► Elixir of Insight ──► Immortal Constitution
           [Trigger: control          [Trigger: harvest       [Unlocks: +15%        [Passive: disciples
            a Ley Line tile]           Spirit Herbs ×5]        research speed        cannot die from
                                                               consumable]           natural causes
                                            │                                        during Goat year]
                                            └─ Breakthrough Pill ─► Forced Ascension
                                               [Unlocks: pill that    [Unlocks: force
                                                reduces Elder→High     Inner→Elder with
                                                Elder failure chance]  doubled risk but
                                                                        no time cost]
```

### §8.2 Forge Branch (Armory)

```
Tier 1                  Tier 2                    Tier 3                     Tier 4
──────                  ──────                    ──────                     ──────
Iron Smelting ─────────► Steel Refinement ────────► Meteoric Alloy ──────────► Sect Treasure Forging
  [Trigger: control        [Trigger: produce          [Requires: High-Grade      [Unlocks: 1 unique
   Iron Ore deposit]        10 weapons at Armory]      Iron Ore in stockpile]    legendary weapon
        │                         │                          │                    per Sect Treasure]
        ├─ Basic Weapons ────────► Refined Weapons ─────► Masterwork Weapons ───► Legendary Armament
        │   [+5% attack]           [+8% attack]            [+12% attack, +5%      [+20% attack;
        │                                                    HP to wielder]         unique effect]
        ├─ Leather Armor ────────► Chainmail ──────────► Qi-Infused Armor ─────► Celestial Raiment
        │   [+5% defense]          [+8% defense]           [+12% def, +5%         [+20% def;
        │                                                    Qi Power]              immunity: 1 hit]
        └─ Siege Tools ──────────► Battering Ram ──────► War Machines ─────────► Formation Arrays
           [enables siege           [enables gate          [siege range +1;        [passive: +15% CP
            actions on forts]        breaching]             fortification dmg]      to all friendlies
                                                                                    in 3 hex radius of
                                                                                    a Formation Elder]
```

### §8.3 Martial Techniques Branch (Library)

```
Tier 1                  Tier 2                    Tier 3                     Tier 4
──────                  ──────                    ──────                     ──────
Basic Sword Arts ──────► Flowing Blade ───────────► Myriad Sword Form ────────► Heaven-Splitting Slash
  [Trigger: win 3          [+15% sword dmg]          [+25% sword dmg;           [+40% sword dmg;
   tactical rounds                                     ignores 10% defense]       AoE duel effect]
   with sword tech]

Basic Fist Arts ───────► Iron Body ──────────────► Diamond Sutra ────────────► Unbreakable Physique
  [Trigger: win a          [+15% HP]                 [+25% HP;                  [passive: survive 1
   duel with fist tech]                               +10% Qi Power]             lethal blow/battle]

Basic Qi Circulation ──► Qi Projection ──────────► Domain Expansion ──────────► Qi Singularity
  [Trigger: complete       [enables ranged Qi         [AoE Qi attack;             [passive: Qi
   a Qi Refinement         attack in tactical]         +10% all Qi effects]        abilities cost
   cave trial]                   │                          │                       −30% Qi Power]
                                 │
        ┌────────────────────────┴────────────────────┐
        │                                             │
        ├─ Lightness Art ─────► Cloud Step ─────────► Phantom Movement ─────► Void Walk
        │   [+1 movement]        [+2 movement;          [ignores ZOC;            [invisible to
        │   [Trigger: traverse    ignore Forest          +3 movement]             non-adjacent
        │    5 tiles in 1 turn]   terrain cost]                                   enemies]
        │
        └─ Internal Defense ──► Qi Shield ─────────► Heavenly Barrier ─────── ► Absolute Defense
            [+10% defense]        [+15% defense;         [+25% defense;            [passive: reduce
                                   reflects 5% dmg]       immune to Tier 1–2        all incoming dmg
                                                           technique effects]        by 30%]
```

**Technique Fusion removed.** The T4 Martial Techniques final unlock node ("Technique Fusion / Custom Technique Creation") is replaced by **"Grandmaster's Legacy"** — a passive that grants +10% effectiveness to all currently researched techniques. This eliminates the unmechanized custom-technique-creation system while preserving meaningful T4 payoff.

---

## §9. Units & Combat

### §9.1 Unit Types

All unit types are defined in `UnitDataSO` ScriptableObjects. Units are not individual disciples — they are stacks representing groups of disciples acting as a single operational unit. The exceptions are Elder Champion and High Elder Vanguard, which are individual named disciples.

| Unit | Rank Composition | Role | Special Rules |
|---|---|---|---|
| **Peon Gang** | 10 Peons | Labor, resource transport | No combat ability; automatically flees when attacked; can build roads and watchtowers |
| **Outer Patrol** | 5–10 Outer Disciples | Scouting, basic defense, escort | Light combat; can build watchtowers; eligible for Settler Party conversion |
| **Inner Disciple Squad** | 3–5 Inner Disciples | Core combat force | Uses assigned techniques; can challenge enemy Inner+ to duel |
| **Elder Champion** | 1 Elder + 0–5 Inner Disciples (retinue) | Elite strike force, army commander | Aura: +10% CP to all friendly units within 2 hexes; can command adjacent stacks |
| **High Elder Vanguard** | 1 High Elder + 0–3 Elders (honor guard) | Endgame powerhouse | Domain Effect: once per battle, activate the High Elder's signature technique (defined by their highest-tier technique); targets a 2-hex radius |
| **Support Caravan** | 5 Peons + 1 Outer overseer | Resource transport | Carries commodity stockpile between compounds; generates +5% trade route income if active on a route |
| **Settler Party** | 1 Outer (settler) + 10 Peons | Founds Branch Sect Outpost | Consumed on founding; assembles over 3 turns at main compound |
| **Spy** | 1 Inner Disciple | Espionage missions | Invisible to enemies without counter-intel detection; see §11 |

### §9.2 Movement

| Factor | Effect |
|---|---|
| **Base movement** | 3 hexes/turn (all units) |
| **Lightness Art T1** | +1 movement |
| **Cloud Step T2** | +2 movement; ignore Forest movement penalty |
| **Phantom Movement T3** | +3 movement; ignore ZOC entirely |
| **Void Walk T4** | +3 movement; invisible to non-adjacent enemies |
| **Horse year bonus** | +1 movement for all units (year 7 only) |
| **Mounted on Plains/Desert** | Peng Clan trait: +2 base movement for all units |
| **Roads (Peon-built)** | Movement cost on road tile = 0.5 (any terrain) |
| **Zone of Control (ZOC)** | Adjacent enemy unit: +2 movement cost per adjacent enemy tile |
| **Fog of War** | Cannot path through Hidden tiles; pathfinder treats them as impassable |

**Roads:** Peon Gangs can build a road segment on an adjacent tile during the Action Phase (costs 5 Lumber; takes 3 turns). Roads persist even if tile changes control. Road segments are destroyed by enemy siege actions.

### §9.3 Combat System

#### §9.3.1 Combat Power Formula

```
CP(unit, tile) =
  (BaseAttack × TechniqueDamageBonus)
  + (BaseDefense × TerrainDefenseBonus)
  + (BaseHP × HPModifier)
  + (QiPower × TechniqueEffectivenessBonus)
  + ElderAura                            // +10% to all stats if Elder Champion within 2 hexes
  + ZodiacBonus                          // e.g., +15% in Tiger year
  + EquipmentBonus                       // from Armory-produced weapons/armor
  − MoralePenalty                        // Morale < 50: −(50 − Morale) × 0.5% to CP
```

Where `TerrainDefenseBonus = 1.0 + tile.Terrain.DefenseBonus` (e.g., Mountain = 1.50).

**Developer note:** `CombatPowerCalculator.Calculate(UnitData unit, HexTileData tile, GameState state)` is a pure static function with no side effects. It reads from `UnitData`, `HexTileData`, and the current `ZodiacBonuses` struct. All bonuses are multiplicative unless specified as additive above. When debugging combat results, all CP components should be loggable individually via a `CombatPowerBreakdown` struct returned alongside the float value.

#### §9.3.2 Auto-Resolution

```
AttackerCP = CP(attacker, attackerTile) × Random.Range(0.85f, 1.15f)
DefenderCP = CP(defender, defenderTile) × Random.Range(0.85f, 1.15f)

WinRatio = AttackerCP / (AttackerCP + DefenderCP)

AttackerCasualties = defenderCasualties = 0 (if decisive)

// Casualty model:
AttackerCasualties% = (1 − WinRatio) × 0.40    // 40% loss at 50/50 fight
DefenderCasualties% = WinRatio × 0.40

// Winner: whoever has more CP after casualties
// If AttackerCP < 0.60 × DefenderCP: auto-retreat
//   Retreat: additional +25% casualties for retreating side
```

Higher-rank units within a stack have a survival advantage: within the casualty pool, Peons die first (50% weight), then Outer Disciples (30%), then Inner Disciples (20%). This is resolved by the `CasualtyDistributor` static class.

#### §9.3.3 Tactical View (Optional)

The tactical view is a 7×7 hex grid loaded as an additive scene. It is offered as an option whenever the player initiates combat during the Action Phase. The AI always uses auto-resolve for its own attacks.

**Battlefield terrain:** generated from the strategic tile's terrain type. A Mountain tile generates a tactical battlefield with 40% elevated hexes and 20% impassable cliff hexes. Plains = flat with scattered cover. Forest = alternating dense and sparse tiles. Use a seeded random generator (seed = `combatID hash`) to ensure replays produce identical battlefields.

**Turn order:** initiative order is determined by each unit's Speed stat (insertion sort descending). Ties broken by HP descending.

**Actions per unit per round:** Move (up to movement budget) + one of: Attack, Use Technique, Duel Challenge, or Hold.

**Duel System (revised):**
The weapon-type triangle (Sword > Spear > Fist) is removed. Duels are resolved as:
```
DuelScore(challenger) = challenger.Attack × TechniqueBonus + rand(0, challenger.QiPower)
DuelScore(target)     = target.Attack × TechniqueBonus + rand(0, target.QiPower)
```
Winner: higher DuelScore. Tie: both take 15% HP damage; no morale effect; duel ends.
- Winner: +15 Morale to winner's side.
- Loser: −15 Morale to loser's side; loser unit takes 30% HP damage.
- Duel is valid between any two Inner+ units. Each unit can participate in at most 1 duel per battle.

**5-round limit:** if neither side has routed or been eliminated after 5 rounds, both sides may retreat. The side with higher remaining CP is declared the strategic winner (for Face/Renown calculation purposes).

**Morale System:**
- Starting morale: 100 for all units.
- Morale loss: −10 per round if taking casualties; −15 if an Elder Champion in the army is killed; −15 from losing a duel.
- Morale gain: +15 from winning a duel; +10 if an enemy Elder Champion is killed.
- Rout threshold: Morale < 20 → roll `Random.value < 0.40f` each round. On success, unit auto-retreats.

#### §9.3.4 Post-Combat Consequences

| Outcome | Renown Change | Face Change | Territory |
|---|---|---|---|
| Victory | +8 Renown | +8 Face | Attacker may claim defender's tile |
| Decisive Victory (>2× CP differential) | +15 Renown | +12 Face | Attacker claims tile + 1 adjacent tile |
| Retreat (attacker retreats) | −3 Renown | −5 Face | Defender keeps tile |
| Defeat (defender eliminates attacker) | −8 Renown | −10 Face | Defender may pursue |
| Face-Slap bonus | +`8 + min(0.3 × FaceDiff, 15)` Face | Applied to winner | — |

---

## §10. Diplomacy & Relations

### §10.1 Sect-to-Sect Relations

Relations between sects are tracked as a **bilateral relation score** (−100 to +100) and a **relation state** (the active formal status between two sects). The score influences AI decision-making; the state dictates what actions are legal.

**Relation State Ladder:**

| State | Required Score Range | Mutual Requirements | Effect |
|---|---|---|---|
| **War** | Any | Declared via Action Phase | Open hostilities; units can attack; trade suspended |
| **Rivalry** | −40 to +100 | Mutual declaration | +15% combat CP vs rival; −20 to rival's allies' relation scores |
| **Neutral** | −40 to +40 | Default | No special rules |
| **Non-Aggression Pact** | +10 or higher | Both parties agree | Cannot declare war for 10 turns; breaking costs −30 Face and +40 aggression from both parties' AI allies |
| **Trade Agreement** | +20 or higher | Both parties agree | Mutual +15% trade route income on shared routes; requires External Affairs Hall on both sides |
| **Alliance** | +50 or higher | Both parties agree; requires External Affairs Hall T3 | Shared vision (remove Fog of War for allied territory); military support obligation; cannot ally with a rival of your ally |

**Diplomatic Actions** (issued via External Affairs Hall, Action Phase):

| Action | Relation Score Effect | Tael Cost | Notes |
|---|---|---|---|
| **Send Gift** | +5 to +25 (depending on gift value) | 20–200 Tael | Immediately shifts score; no cooldown |
| **Propose Trade Agreement** | — | 0 | Target AI uses DiplomacyAI.WeightDiplomacy() to accept/reject |
| **Propose Non-Aggression Pact** | — | 0 | Cooldown: 5 turns after prior expiry |
| **Propose Alliance** | — | 0 | Requires score ≥ 50 and no active war or rivalry |
| **Declare Rivalry** | Sets state to Rivalry | 0 | Can be proposed; opponent may or may not reciprocate. Unreciprocated rivalry still applies player's +15% CP bonus |
| **Declare War** | Sets state to War | 0 | Requires 5-turn waiting period after any pact expires |
| **Demand Tribute** | −10 to relationship | 0 | Only available if your CP is > 2× theirs. AI pays 50% of the time; refusing triggers war option |
| **Vassalization Offer** | Sets state to Vassal (if accepted) | 0 | Target must be significantly weakened; AI accepts if CP ratio < 0.3 and no alliance options |
| **Absorption Offer** | — | 500 Tael + high Influence | Target sect ceases to exist; their disciples are distributed proportionally; requires score ≥ 80 with vassal sect |
| **Technique Exchange** | +10 | 0 | Both parties receive one researched technique from the other; must be agreed; each party's AI weights technique value |

**Vassalization mechanics:**
- Vassal pays 20% of their Tael income as tribute each turn.
- Vassal follows the suzerain's foreign policy (cannot declare war independently, cannot form alliances without permission).
- Vassal retains internal autonomy (buildings, disciples, research).
- Vassals can rebel if: tribute has been collected for 20+ turns AND their CP grows above 70% of suzerain's CP. Rebellion is a diplomatic event with a rebellion roll.

### §10.2 Independent Settlements

Independent settlements are NPC-controlled population centers. They do not expand, declare war, or form diplomatic agreements. They respond only to trust-building actions and threats.

**Settlement Types:**

| Type | Pop | Provides at Friendly+ | Difficulty to Influence |
|---|---|---|---|
| **Village** | 50–200 | Peon recruits (2/turn), basic commodities (Lumber, Herbs) | Easy (trust scale compressed) |
| **Town** | 200–1,000 | Higher trade income, diverse commodities, Outer Disciple recruits | Medium |
| **Trade Post** | 50–300 | Premium trade rates (×1.3 base), rare commodities (Jade, Tea) | Hard (requires Friendly before routes open) |
| **Wandering Tribe** | Mobile (moves 1 tile/turn randomly) | Provides one-time Outer Disciple mercenary hire (hired: fights 1 battle, then disbands); passive Horse terrain movement bonus if within territory | Hard (requires combat reputation or Horses commodity gift) |
| **Hermit's Dwelling** | 1 NPC | One technique scroll (Tier matches trust level; Neutral=T1, Friendly=T2, Devoted=T3) OR one-time temporary Elder hire (10 turns, costs 80 Tael) | Very Hard (one-time; trust degrades −5/turn after reaching Devoted if no action taken) |

**Trust Tier System:**

| Tier | Range | Effect |
|---|---|---|
| **Hostile** | 0–19 | Settlement refuses all contact; may spawn Bandit units that attack your territory |
| **Wary** | 20–39 | Basic observation; no trade; Peng Clan can open trade routes here |
| **Neutral** | 40–59 | Basic gifting allowed; no trade routes without Trade Agreement action |
| **Friendly** | 60–79 | Trade routes allowed; peon recruitment allowed; tribute collection begins |
| **Devoted** | 80–100 | Bonus recruits (+1 Outer/turn); −5% trade cost; settlement votes for you in Influence victory |

**Trust Building Actions:**

| Action | Trust Gain | Tael Cost | Cooldown |
|---|---|---|---|
| Send Tael Gift (50 Tael) | +5 | 50 | 3 turns |
| Send Commodity Gift (e.g., Tea Leaves ×5) | +8 | 0 + commodity | 3 turns |
| Complete Defence Request (defeat bandit spawns) | +15 | 0 | Event-driven |
| Complete Delivery Request | +10 | 0 + commodity | Event-driven |
| External Affairs Hall Influence action | +5 | 20 | 2 turns |
| Emei Lotus Hall Emissary | +10 | 20 | 2 turns |
| Dog Year passive bonus | +15% to all trust gains | 0 | Year 11 only |

**Trust decay:** trust decreases −1/turn passively if no action has been taken toward the settlement in 10+ turns. Attacking a settlement causes −40 trust immediately and triggers a Bandit Uprising event (§14).

### §10.3 Renown System

**Renown** is the global reputation score (0 to ∞ but practical range 0–5,000 in a standard game). It drives the Influence victory and affects AI perception of the player's threat level.

**Renown Sources:**

| Event | Renown Gain |
|---|---|
| Win a combat against an equal or stronger foe | +8 |
| Decisive victory (CP ratio > 2×) | +15 |
| Complete a settlement protection request | +10 |
| Advance a research tier | +5 (T1), +10 (T2), +20 (T3), +40 (T4) |
| Host a Martial Arts Tournament | +30 (host) |
| Win a Tournament | +50 (winner) |
| Ally assists in war victory | +20 |
| Rabbit Year passive | ×1.15 all Renown gains |
| Emei Sect Trait | +15% from diplomacy-based Renown sources |
| Namgung Sect Trait | +15% from combat victories |

**Renown Losses:**

| Event | Renown Loss |
|---|---|
| Break a Non-Aggression Pact | −20 |
| Attack an allied settlement | −30 |
| Disciple defects publicly | −10 |
| Lose territory to an enemy | −5 per tile |
| Demonic Cult sacrifice (witnessed) | −15 |

**Renown thresholds for AI perception:**
- > 25% of global max Renown: AI Diplomat personality considers alliance proposals.
- > 50%: AI sects with Militant or Expansionist personality increase aggression toward you.
- > 75%: Influence victory conditions enter check range; "Heavens Tremble" alert can trigger.

### §10.4 Face System

**Face** is the sect's Jianghu social standing (0–100, starting at 50). It is distinct from Renown: Renown is broad global fame; Face is specific to martial-world etiquette — how other cultivators regard your conduct.

**Face effects:**

| Tier | Range | Market | Disposition Gain | AI Attack Hesitation |
|---|---|---|---|---|
| **Esteemed** | ≥80 | Buy prices ×0.90 | +30% | AI threshold to attack: +20% CP advantage required |
| **Respected** | 60–79 | Buy prices ×0.95 | +15% | AI threshold: +10% |
| **Neutral** | 35–59 | No modifier | +0% | Standard threshold |
| **Diminished** | 20–34 | Buy prices ×1.05 | −10% | AI threshold: −10% |
| **Disgraced** | ≤20 | Buy prices ×1.10 | −20% | AI actively seeks to attack |

**Face Change Events:**

| Event | ΔFace |
|---|---|
| Tournament victory | +15 |
| Combat victory vs equal+ foe | +8 |
| Disciple completes Heavenly Tribulation | +5 |
| Treaty honored at expiry | +3 |
| Paying bandit tribute | −5 |
| Disciple publicly defects to another sect | −8 |
| Battle lost | −10 |
| Non-Aggression Pact broken | −20 |
| Demonic Cult sacrifice witnessed by another sect | −15 |
| High Elder killed in battle | −12 |

**Face-Slap Bonus:** when a Disgraced or Diminished sect defeats an Esteemed or Respected sect:
```
FaceGain = 8 + min(0.30 × (WinnerFace − LoserFace), 15)
LoserFaceLoss = −20
```

**Alignment modifier (Face gains only):**
- Righteous alignment (Renown > 200, no Demonic Cult, no broken pacts in last 20 turns): Face gains ×1.15
- Demonic Cult: Face gains ×0.50; Face losses ×1.20 (the Jianghu views them with additional scorn)

---

## §11. Espionage

Spies are Inner Disciple units deployed via the External Affairs Hall (T1 required). The spy unit is invisible to enemies unless their Counter-Intelligence strength exceeds the spy's detection threshold.

**Spy Detection Threshold:**
```
TargetDetectionStrength = (CounterSpyCount × 10) + (ExternalAffairsHallTier × 5)
SpyEvasion = SpyRank × 15 + TechniqueStealth + SectBonus (Tang Clan: +20)
Detected if: TargetDetectionStrength > SpyEvasion + Random.Range(0, 30)
```
Detection roll happens every turn the spy is in enemy territory. On detection, the spy is captured (removed from map) and the owner sect receives an `OnSpyCaptured` notification. The captor's sect gains +5 Face and +5 Renown.

**Missions:**

| Mission | Duration (turns) | Base Success Chance | Effect on Success | Effect on Failure |
|---|---|---|---|---|
| **Gather Intelligence** | 3 | 70% | Reveals target `SectData.Buildings`, unit positions, and research progress to the player for 10 turns | Nothing; spy stays (50% chance) or flees (50%) |
| **Steal Technique Scroll** | 5 | 45% | Acquires a random researched technique from target; adds to player's `UnlockedTechs` | Nothing; detection roll +10% |
| **Sabotage Building** | 4 | 55% | Disables a random target building for 5 turns (`IsDisabled = true`); disabled buildings cannot produce output or advance queues | Nothing; detection roll +10% |
| **Sow Dissent** | 6 | 40% | Target `SectData.DissentLevel += 20` | Nothing; detection roll +15% |
| **Assassinate** | 8 | 25% (base) | Removes a random Elder from target sect; Face penalty if discovered | Detection roll +25%; if discovered: −15 Face for attacker sect, +20 aggression from target |
| **Counter-Intelligence** | Passive (stationed) | — | Each stationed spy adds +10 to own sect's `CounterIntelStrength` | N/A |

**Tang Clan bonuses:** +20% to all mission success chances. +30% specifically to Assassinate.

**Mission Success Chance Full Formula:**
```
SuccessChance = BaseMissionChance
              + (SpyRank − 1) × 10%       // Inner=+0%, Elder spy=+10%, High Elder spy=+20%
              + TechniqueBonus             // e.g., Snake year: +15% to all espionage
              + SectBonus                  // Tang Clan: +20%
              − CounterIntelPenalty        // −5% per 10 points of target's counter-intel above 20
```

**Developer note:** `EspionageMission` is a plain C# class with `Tick()` called during Resolution Phase. `Tick()` decrements `TurnsRemaining`, runs the detection roll, and on `TurnsRemaining == 0` calls `Resolve()`. `Resolve()` calls `Random.value < SuccessChance` to determine outcome and invokes the appropriate effect through the `EspionageEffectApplier` static class. All espionage effects must be Command objects (not direct mutations) so they appear in the replay log.

---

## §12. AI System

### §12.1 Behavioral DNA System

Each AI-controlled sect has a **Behavioral Genome** — 8 weighted traits, each a float in [0.0, 1.0]. These weights drive the utility evaluations in every decision layer. Crucially, the genome **drifts** in response to game events, making AI behavior dynamic and adaptive rather than static.

```csharp
public struct AIGenome
{
    public float Aggression;     // Weight toward military actions
    public float Expansion;      // Weight toward territory and branch sects
    public float Research;       // Weight toward tech tree advancement
    public float Diplomacy;      // Weight toward alliances and trade
    public float Economy;        // Weight toward income-generating actions
    public float Espionage;      // Weight toward spy missions
    public float Zealotry;       // Weight toward sect identity preservation (low = adapts freely)
    public float Adaptability;   // Meta-weight: how quickly other traits drift
}
```

**Starting Genomes by Personality:**

| Personality | Aggr | Expan | Resrch | Diplo | Econ | Espy | Zeal | Adapt |
|---|---|---|---|---|---|---|---|---|
| **Expansionist** | 0.45 | 0.90 | 0.40 | 0.30 | 0.60 | 0.30 | 0.30 | 0.60 |
| **Militant** | 0.90 | 0.50 | 0.35 | 0.10 | 0.40 | 0.35 | 0.60 | 0.30 |
| **Scholar** | 0.15 | 0.35 | 0.95 | 0.55 | 0.45 | 0.20 | 0.50 | 0.50 |
| **Diplomat** | 0.10 | 0.40 | 0.45 | 0.95 | 0.65 | 0.25 | 0.25 | 0.70 |
| **Opportunist** | 0.50 | 0.55 | 0.50 | 0.50 | 0.55 | 0.55 | 0.15 | 0.90 |
| **Zealot** | 0.85 | 0.40 | 0.30 | 0.05 | 0.35 | 0.50 | 0.95 | 0.10 |

**Genome Drift Rules (applied at end of each turn):**

```
DriftRate = BaseRate × Adaptability
BaseRate = 0.03 per triggering event

// Events and their drift direction:
Lost a battle:               Aggression += DriftRate × 0.5
                             Diplomacy  += DriftRate × 0.3

Won a decisive battle:       Aggression += DriftRate × 0.3
                             Expansion  += DriftRate × 0.4

Completed a trade deal:      Diplomacy  += DriftRate × 0.6
                             Economy    += DriftRate × 0.2

Espionage mission succeeded: Espionage  += DriftRate × 0.5

Research tier completed:     Research   += DriftRate × 0.4

Went bankrupt (Tael < 0):   Economy    += DriftRate × 0.8
                             Aggression -= DriftRate × 0.3

Ally betrayed them:          Diplomacy  -= DriftRate × 0.6
                             Zealotry   += DriftRate × 0.4

// All trait values are clamped to [0.05, 0.95] after drift.
// Zealotry acts as a brake: high Zealotry (>0.70) halves all drift rates.
```

**Developer note:** `AIGenome` is stored in `AIController.Genome` and serialized in the save file. `GenomeDriftSystem.Apply(AIController ai, GameEvent ev)` is called once per relevant event during Resolution Phase. Drift is deterministic given the same event sequence — this ensures replays produce identical AI behavior.

**Legibility for players:** the top three genome weights (by value) are displayed on the Diplomacy screen as public "known agenda traits" for AIs whose sects the player has spied on (via Gather Intelligence). Example display: "Expansionist tendencies, Economic focus, Low diplomatic priority." This makes AI behavior legible without requiring players to read internal numbers.

### §12.2 Decision Making & Inference Budgets

The AI runs four decision layers in sequence during Resolution Phase. Each layer has a per-turn CPU time budget. When a layer exceeds its budget, lower-priority evaluations are pruned in a defined order.

**Per-Sect Inference Budgets (milliseconds):**

| Difficulty | Budget/Sect | Notes |
|---|---|---|
| **Initiate** | 2ms | Only Combat and Build layers run; Diplomatic and Strategic pruned |
| **Disciple** | 5ms | All layers run; diplomatic evaluations limited to 3 options |
| **Master** | 10ms | All layers run fully; strategic re-evaluation every 3 turns |
| **Heavenly Dao** | 20ms + async offload | Full evaluation every turn; excess computation offloaded to Unity Job threads |

**Pruning Priority (when over budget):**
1. Prune **Diplomatic** evaluations first (delay to next turn)
2. Prune **Strategic Layer** re-evaluation (use cached goal from last evaluation)
3. Never prune **Combat-Immediate** decisions (attack/retreat when adjacent to enemy)

**Decision Layers:**

**Layer 1 — Strategic (every 3–5 turns, depending on difficulty):**
```csharp
StrategicGoal EvaluateGoal(SectData self, WorldState world)
{
    // Score each goal by genome weights × opportunity score
    float expandScore   = Genome.Expansion × EvaluateExpansionOpportunity(self, world);
    float attackScore   = Genome.Aggression × EvaluateThreatsAndTargets(self, world);
    float researchScore = Genome.Research   × EvaluateResearchGap(self, world);
    float diplomScore   = Genome.Diplomacy  × EvaluateDiplomaticOpportunity(self, world);
    float econScore     = Genome.Economy    × EvaluateEconomicGap(self, world);

    return ArgMax(expandScore, attackScore, researchScore, diplomScore, econScore);
}
```

**Layer 2 — Tactical (every turn):**
Uses genome weights as coefficients to score available actions. The top-3 scoring actions are executed.

```csharp
float WeightBuildAction(BuildingConfigSO building, AIGenome genome, SectData self)
{
    // Example: Militant AI weights Armory very high
    return building.MilitaryValue    × genome.Aggression
         + building.EconomyValue     × genome.Economy
         + building.ResearchValue    × genome.Research
         + building.DiplomacyValue   × genome.Diplomacy;
}
```

Each `BuildingConfigSO` has pre-authored `MilitaryValue`, `EconomyValue`, `ResearchValue`, `DiplomacyValue` floats [0,1] set by the designer.

**Layer 3 — Diplomatic (every 3 turns or on receiving a diplomatic proposal):**
Evaluates each possible diplomatic action against all known sects. Scores `DiplomacyWeight × RelationScore × StrategicAlignment`. Executes actions scoring above a threshold (base 0.6, modified by Zealotry: high Zealotry raises threshold).

AI breaks agreements only when: `(StrategicGain > 0.80)` AND `(Genome.Zealotry < 0.50)`. High-Zealotry AIs never break agreements they make — they simply refuse to make them in the first place.

**Layer 4 — Combat (on adjacent enemy detection):**
```
AttackOdds = EstimatedPlayerCP / (EstimatedPlayerCP + EstimatedEnemyCP)
AttackThreshold = 0.55 − (Genome.Aggression × 0.20)   // Militant: attacks at ~35% odds
RetreatThreshold = 0.40 − (Genome.Aggression × 0.10)

if AttackOdds > AttackThreshold: execute attack
if AttackOdds < RetreatThreshold: execute retreat
```

**CP Estimation:** AIs use last-known unit data plus research-level inference. They do not have perfect information (except Heavenly Dao difficulty).

**Contextual Narrative Generation for Wandering Master and Sect Defector Events:**

Rather than scripted text, these events use **parameterized flavor text templates** that are populated with context from the current game state.

```
// Wandering Master template:
"A figure {MasterAdjective} approaches your {CompoundName} gates. He introduces himself as
{MasterName}, once a {FormattedRank} of the {FormerSectName} who {ReasonForLeaving}. He
{OfferVerb} to share the secrets of {TechniqueName} in exchange for {RequestType}."

// Parameters:
MasterAdjective:  ["weathered", "dignified", "mysterious", "battle-scarred", "serene"]
FormattedRank:    player's highest current rank or one rank above if late-game
FormerSectName:   randomly selected from known sects (or "unnamed mountain sect")
ReasonForLeaving: ["was exiled after questioning the sect leader's motives",
                   "lost everything in a raid by a rival faction",
                   "voluntarily left seeking a worthier path",
                   "fled after a technique scroll was stolen"]
OfferVerb:        ["offers", "grudgingly agrees", "eagerly proposes", "quietly asks"]
TechniqueName:    actual technique name from the player's researchable pool
RequestType:      ["50 Tael", "safe passage for 5 turns", "a rare Spirit Herb", "shelter for the winter"]
```

```
// Sect Defector template:
"A {Rank} from {SourceSectName} has been sighted near your borders. {PronounThey} calls
{PronounThemself} {DefectorName} and claims {MotivationString}. {OpportunityString}."

// Parameters:
Rank:             the actual rank of the defecting disciple
SourceSectName:   the sect name (which player knows from map, regardless of Fog of War)
MotivationString: ["to have suffered under an unjust Elder Council decision",
                   "the sect's brutal training has broken {PronounThem}",
                   "a failed assassination plot against {PronounThem} by {SourceSectName} spies",
                   "philosophical disagreements with the sect's cultivation path"]
OpportunityString:["Will you offer shelter?",
                   "The defector may be a spy.",
                   "Their techniques could prove valuable.",
                   "Taking them in will strain your relations with {SourceSectName}."]
```

**Developer note:** template resolution is handled by `NarrativeTemplateResolver.Resolve(template, GameState state, EventContext context)`. Templates are stored as `NarrativeTemplateSO` assets (plain text with `{Parameter}` markers). The resolver uses a `ParameterProvider` that reads game state to produce contextually accurate substitutions. This is not AI-generated text — it is deterministic data-driven narrative assembly, which is important for localization.

### §12.3 Difficulty Scaling

**Four difficulty levels** (reduced from 5; Grandmaster parameters folded into the Master+ range):

| Difficulty | AI Income/Research Modifier | AI Combat Modifier | Player Assistance | AI Starting Bonus |
|---|---|---|---|---|
| **Initiate** | −20% | −15% | Fog of War hints on; event tooltips explain all consequences | None |
| **Disciple** | No modifier | No modifier | No assistance | None |
| **Master** | +20% income; +20% research | +10% CP | None | Starts with 1 extra Outer Disciple squad |
| **Heavenly Dao** | +50% income; +50% research | +25% CP | None; designed to be uncomfortable | Starts with 2 Outer Disciple squads; perfect information (no Fog of War for AI) |

**Anti-snowball mechanics (all difficulties):**
- **Comeback Mechanic:** any sect at <25% of the leading sect's score receives a +20% to all income and research for 10 turns. This prevents runaway leaders by compressing the strategic field.
- **Near-Victory Aggression:** when any sect is within 15 turns of Influence or Enlightenment victory, all non-allied sects gain +30% aggression toward them for 10 turns.
- **Domination Resistance:** when a sect controls >45% of all sect capitals, every remaining independent sect gains a passive +15% CP when fighting that sect.

---

## §13. Game Flow — Early, Mid, Late

### §13.1 Early Game (Turns 1–30)

**Core loop focus:** scouting, resource acquisition, first infrastructure decisions, choosing specialization direction.

**Expected player actions by milestone:**

| Turns | Expected Actions | Key Decisions |
|---|---|---|
| T1–5 | Move Founder; scout starting area; identify 3 potential founding sites | Choose founding tile: Qi density vs. proximity to settlements vs. resource deposits |
| T5–10 | Found Sect; begin recruiting Peons; build first non-Temple building | Training Grounds (military path) vs. External Affairs Hall (trade/diplomacy path) vs. Medicine Hall (cultivation path) |
| T10–20 | Establish first trade route; train first Outer Disciples; contact nearest settlements; begin first research | Which tech branch to start: Alchemy (survival), Forge (military), Martial (techniques) |
| T20–30 | First Outer Disciples deployed; scouting map 60% complete; first contact with rival sects; begin building second hall | Alliance opportunity? Rivalry declaration? Settle a branch sect? |

**Common early-game failure modes (developer note — design for these):**
- Peon recruitment too fast → Dissent spike from ratio imbalance. Mitigate by displaying Dissent projection in the Recruit UI.
- Trade route not established by T15 → net-negative Tael; spiral. Mitigate by Tutorial overlay at T12 if no active trade route.
- Founding tile too remote → isolation, slow growth. Valid strategic choice; do not penalize, but ensure there are AI sects close enough to create pressure by T25.

### §13.2 Mid Game (Turns 30–80)

**Core loop focus:** army composition, technique strategy, economic optimization, victory path selection.

**Expected player actions by milestone:**

| Turns | Expected Actions | Key Decisions |
|---|---|---|
| T30–45 | First Inner Disciples trained; techniques assigned; Library built; second research node completing | Which Elder specialization to prioritize? War Elder vs. Wisdom Elder? |
| T45–60 | Branch Sect founded or scouted; first major diplomatic event; Tier 2 research completing | Expand territory or consolidate current compound? Trade Agreement or Rivalry? |
| T60–80 | Elder Council built; first Elder promoted; serious military threat from at least one AI sect | Prepare for war or negotiate? Which victory condition to commit to? |

**Mid-game strategic pressure:** by T50, the map should feel "full" — most Plains/River tiles within the starting area should be claimed or contested. If the map feels sparse at T50, map generation seeded too few AI sects.

### §13.3 Late Game (Turns 80–150)

**Core loop focus:** victory push, grand strategy, countering rivals' conditions, Heavenly Tribulations.

| Turns | Expected Actions | Key Decisions |
|---|---|---|
| T80–100 | Elders specialized; Tier 3 research; Sect Treasures in development; alliance blocs forming | Which AI is closest to a victory? Counter them or push your own? |
| T100–120 | High Elder promotions; Heavenly Tribulations; world-spanning conflicts; Near-Victory alerts firing | Full commitment to one victory condition |
| T120–150 | Endgame crisis: "Heavens Tremble" alerts concentrate all aggression; final push | Alliance or final war |

**Endgame tension design:** the Near-Victory mechanic (§2) ensures the late game is a sprint under pressure, not a slow checkmark. Developer note: playtest specifically for "quiet endgame" syndrome — if a player reaches T120 without being challenged, increase the AI aggression scaling curve in `AIAggression.EvaluateNearVictoryThreat()`.

---

## §14. Events & Encounters

Events fire at the start of the Event Phase. Up to 2 events can fire per turn (3 during Dragon year, due to the Dragon year's rare-event modifier). Events are evaluated by `EventScheduler` against all active `EventDefinitionSO` conditions plus zodiac modifiers.

**Developer note:** `EventDefinitionSO.TriggerCondition` is a C# predicate (`Func<GameState, bool>`) stored as a delegate reference in a static `EventConditionLibrary` class. The SO stores an enum key that maps to the correct predicate in the library. This avoids serializing executable code while keeping conditions designer-configurable.

### §14.1 Event Catalog

| Event | Base Probability/Turn | Trigger Conditions | Player Options | Outcome |
|---|---|---|---|---|
| **Wandering Master** | 3% | Turn > 15; at least one Sacred Peak or Mountain within 10 tiles of any sect | Recruit (50 Tael: +1 temporary Elder for 10 turns; OR teaches 1 technique to up to 5 present Inner+ disciples); Ignore | Narrative template (§12.2) generates unique flavor text |
| **Bandit Uprising** | 5% | Any settlement within 5 tiles with trust < Neutral | Player dispatches nearest army to defeat; OR ignores | Defeat: +15 trust with nearest settlement, +5 Renown. Ignore: trust −10 with that settlement, bandits persist for 3 turns |
| **Ancient Tomb** | 2% | Exploration near Ruins tiles (a unit moves adjacent to a Ruins feature) | Investigate (costs 1 turn of unit movement); Ignore | Investigate: dungeon encounter — auto-resolve against Bandit-tier opponents; success = reward table: Jade ×2–5, T1–T2 technique scroll, Medicinal Herbs ×5 |
| **Spirit Beast Sighting** | 2% | High-Qi tile (Dense or Ley Line) within 5 tiles of any sect | Pursue and defeat (combat encounter against Spirit Beast); Bond (requires Inner+ disciple to be adjacent) | Defeat: rare materials (Spirit Herbs ×3, Jade ×1). Bond: see §14.2 |
| **Martial Arts Tournament** | Player-triggered | Player spends 100 Tael to host; OR any AI sect triggers it | Host (send champion: one Inner+ disciple); Attend (send champion); Boycott | Winner: +50 Renown, +15 Face. Host: +30 Renown regardless of result. Boycott: −5 Face |
| **Plague / Famine** | 2% (×2 if outside Goat/Pig year) | Random; slightly more likely in Swamp/Desert adjacent tiles | Apply Medicine Hall resources to treat (costs 5 Medicinal Herbs: limits spread); Ignore | Untreated: spreads to adjacent tiles; −10% disciple HP in affected region for 5 turns. Treated: contained; +5 Renown |
| **Sect Defector** | 4% | Any sect has DissentLevel > 40 | Accept (+1 disciple of target sect's rank to your roster; +5 Qi); Investigate (55% chance: genuine addition, 45% chance: spy — planted counter-intel agent; if spy detected, return to owner and gain Face); Refuse | If accepted: target sect −5 Renown, −8 Face; your sect +10 Renown. Narrative template (§12.2) |
| **Heavenly Tribulation** | Triggered (not random) | Elder → High Elder promotion queues | Player can spend 1 Breakthrough Pill to reduce failure chance by −30% | Success: High Elder with full stats. Injury (partial failure): High Elder at −20% stats for 10 turns. Death (full failure): Elder removed from roster; 150 Tael, 100 Qi consumed, no refund. |
| **Ley Line Surge** | 1% | Near Ley Line tiles (any sect within 5 tiles of Ley Line) | No player choice; automatic | All sects with territory within 5 tiles of the Ley Line gain +50 Qi; if it's Dragon year, +100 Qi. Lasts 1 turn. |

### §14.2 Spirit Beasts

A Spirit Beast can be **bonded** to one unique disciple. One beast per disciple; bonding overwrites any existing bond. Beasts have five elemental archetypes and three rarity tiers.

| Beast | Element | Stat Focus | Base Bonuses (×Tier Multiplier) |
|---|---|---|---|
| **Phoenix** | Fire | Offensive Qi cultivator | +10% Attack, +20% Heavenly Tribulation mitigation |
| **Dragon** | Water | Balanced | +10% Attack, +10% Defense, +15% HP, +10% Qi income, +15% Tribulation mitigation |
| **Tortoise** | Earth | Defensive / Tribulation tank | +15% Defense, +20% HP, +25% Tribulation mitigation |
| **Tiger** | Metal | Pure striker | +15% Attack |
| **Serpent** | Wood | Cultivation specialist | +15% Qi income to owning sect (passive), +10% research speed |

**Tier Multipliers:** Common ×1.0; Rare ×1.75; Legendary ×3.0.
Example: Legendary Tortoise = +45% Defense, +60% HP, +75% Tribulation mitigation.

**Acquisition:**
- **Common:** 20% chance to bond during any Spirit Beast Sighting event where the bonding option is chosen and the disciple survives the encounter.
- **Rare:** awarded as a reward for unique-disciple breakthroughs (completing 3 cave trials of the same type on one disciple) or 2nd–3rd placement in a Secret Realm.
- **Legendary:** 1st placement in a Secret Realm during a Dragon year only.

**Developer note:** `UniqueDisciple.CalculateStats()` must check `BondedBeast` and apply the beast's bonuses as multipliers to base stats before returning the stat block. Spirit Beast bonuses stack with technique bonuses multiplicatively. Ensure that the Legendary Dragon Beast's Qi income bonus is routed through `SectManager.ProcessIncome()` as an event subscription, not hardcoded in `CalculateStats()`.

### §14.3 Secret Realms

Secret Realms are recurring cultivation trials opening at the **zodiac cycle boundary every 12 turns** on a random Sacred Peak or Ancient Ruins tile. The realm stays open for 3 turns, then auto-resolves.

**Each sect may submit exactly one eligible disciple per opening.** Eligible: any Inner Disciple or above who is not dead, not on an active espionage mission, and not in mid-combat.

**Realm Difficulty:** `Difficulty = clamp(floor(CurrentTurn / 12), 1, 3)`.

**Trial Score Formula:**
```
Score = Realm × 15
      + Body × 8                     // disciple's base HP stat
      + RootQuality × 5              // cultivation potential (0–10 scale per disciple)
      + TechniqueAttack × 20         // sum of offensive technique stat bonuses
      + TechniqueDefense × 15        // sum of defensive technique stat bonuses
      + TraitBonus                   // Lucky +10, Resilient +8, Perceptive +5, Reckless −5, Fragile −8
      + SpiritBeastBonus             // Common +5, Rare +10, Legendary +15
      − Difficulty × 5
      × Random(0.80, 1.20)           // ±20% variance
```

**Reward Tiers (upgraded by one tier during Dragon year):**

| Placement | Normal Rewards | Dragon Year Upgrade |
|---|---|---|
| **1st — Legendary** (Dragon year only) | Cultivation root upgrade + 80 Qi + 5 Spirit Herbs + T4 technique scroll + 20 Renown + 15 Face | N/A (this IS the Dragon year reward) |
| **1st — Major** (normal) | 50 Qi + 3 Spirit Herbs + T3 technique scroll + 10 Renown | Becomes Legendary tier |
| **2nd — Standard** | 25 Qi + 1 Spirit Herb + 5 Renown | Becomes Major tier |
| **3rd+ — Minor** | 10 Qi + 3 Medicinal Herbs | Becomes Standard tier |

**Developer note:** `SecretRealmEvent` is resolved in `EventScheduler` during the Resolution Phase on turn 12, 24, 36, etc. (turns divisible by 12). The `SecretRealmResolver.Resolve(participants, difficulty, isDragonYear)` is a pure function: it scores all participating disciples, ranks them, distributes rewards via `RewardApplier`, and raises `OnSecretRealmResolved`. The result is deterministic given a fixed random seed, ensuring consistent replays.

---

## §15. UI/UX Design

### §15.1 Design Principles

1. **Information without overload:** all critical numbers (Tael, Qi, Renown, Dissent, current phase) are always visible. Secondary numbers (commodity stockpile, individual disciple stats) are one click deep. Tertiary numbers (technique formulas, exact CP breakdowns) are one tooltip away.

2. **Context-sensitive actions:** the Context Panel shows only actions available for the currently selected tile/unit/building in the current phase. Actions greyed out show a tooltip explaining the prerequisite (e.g., "Requires External Affairs Hall T1").

3. **Phase clarity:** the current phase name and a brief description of what is happening ("Action Phase — move units, build, research, and issue commands") is displayed prominently during each phase. Phases the player cannot act in (Income, Build, Research) show an "auto-advancing" animation so players don't feel stuck.

4. **Non-blocking AI turns:** during Resolution Phase, the camera remains free to pan and zoom. AI unit movements are animated on the map in real-time (speed-scaled for large maps). A "Skip AI Animations" toggle is available in Settings.

### §15.2 Main Game Screen Layout

```
┌──────────────────────────────────────────────────────────────────────────────┐
│  [SECT BANNER]  Turn 47 — Year of the Dragon  │ Tael: 284 │ Qi: 156 │ Renown: 340  │
│  Phase: Action Phase — Move units, issue commands              [Zodiac Icon]  │
├──────────────────────────────────────────────────────────────────────────────┤
│                                                              │ MINI MAP       │
│                                                              │ [16:9 scaled]  │
│                    HEX MAP VIEW                              │                │
│              (rotatable, zoomable, pannable)                 │ [Fog overlay]  │
│                                                              │                │
│         [Unit selection highlight rings]                     │ Legend:        │
│         [Territory color overlays]                           │ ■ Your sect    │
│         [Ley Line particle streams]                          │ ■ AI sects     │
│                                                              │ ■ Neutral      │
├──────────────────────────────────────────────────────────────┴────────────────┤
│ CONTEXT PANEL (selected object)                                               │
│ [Object Icon] [Name]      [Stat Block]         [Available Actions]            │
│ Mountain Tile │ Qi: Dense │ Cave: 2 (Qi Ref.)  │ [Found Sect] [Scout Cave]   │
│ Iron Ore: ×1  │ Elev: High│ Control: Yours     │ [Build Watchtower]          │
├───────────────────────────────────────────────────────────────────────────────┤
│ [SECT ▼]  [RESEARCH ▼]  [DIPLOMACY ▼]  [MILITARY ▼]  [MARKET]  [END TURN →] │
└───────────────────────────────────────────────────────────────────────────────┘
```

**Persistent HUD elements:**
- Sect banner (click to open Sect Overview)
- Turn counter and Zodiac animal icon (click for full zodiac year effects tooltip)
- Tael / Qi / Renown counters (click each to open detailed income breakdown panel)
- Dissent bar: only visible when Dissent > 0; turns red when > 50; pulses when > 75
- Phase indicator with phase name

### §15.3 Key Screens

**Sect Overview:**
- Building status panel: each building card shows name, current tier, upgrade availability, any active construction timer, and a one-line current-turn production summary.
- Disciple roster: grouped by rank; each group shows count and a "Manage" button to drill into individual disciples.
- Resource summary: current stockpile of all commodities.
- Dissent meter: 0–100 progress bar with breakdown of Dissent sources (ratio imbalance, bankruptcy, etc.).
- Zodiac year reminder: small banner showing current year's bonuses.

**Research Tree:**
- Three panels side-by-side (Alchemy / Forge / Martial Techniques).
- Each tech node renders as a card: name, flavor text (one sentence), Qi cost, estimated turns at current speed, locked/unlocked/researching status, Enlightenment Trigger description.
- Prerequisite arrows rendered as Bezier curves.
- Active research node shows a progress ring.
- Clicking a locked node shows a tooltip: "Requires [Prerequisite Tech]" or "Requires [Hall] at T[n]".

**Diplomacy Screen:**
- Two tabs: Sects and Settlements.
- **Sects tab:** list of all known sects with relation state (color-coded), relation score, last diplomatic action, available actions for this turn. Top 3 AI genome traits shown for sects that have been successfully spied on.
- **Settlements tab:** list of all settlements with trust tier, distance, available commodities, trust-building options, active trade routes.

**Military Overview:**
- All active units listed with position, composition, movement remaining, upkeep, and current orders.
- Army stacks: if multiple units are on the same tile, shown as a stack summary with combined CP estimate.
- Supply line status: color-coded icon per army (green = in range; yellow = extended supply; red = severed).

**Disciple Detail:**
- Individual disciple stats: rank, name (procedurally generated from a Chinese name table), HP, Attack, Defense, Speed, Qi Power.
- Assigned techniques: slots displayed with technique name, stat bonus, and tier.
- Special traits (Lucky, Resilient, etc.): shown with tooltip explanation.
- Bonded Spirit Beast: beast type, tier, and bonuses applied.
- Promotion status: if eligible, show promotion cost/timer preview.

**Market Screen:**
- Commodity list: buy price (with tier markup shown), sell price, your stockpile quantity, global supply indicator (relative bar — high supply → downward arrow, scarce → upward arrow).
- Quantity stepper with live preview of price after the transaction.
- Price history chart (last 10 turns) per commodity — one-click toggle.

---

## §16. Audio & Visual Direction

### §16.1 Visual Style

**Overall aesthetic:** Chinese ink-wash painting (水墨画, shuǐmò huà) translated to 3D. Terrain tiles use muted, naturalistic colors with ink-outline shading via a custom URP outline pass. Buildings are stylized wuxia architecture — curved eaves, red lacquer, bamboo and stone materials. UI elements use aged parchment texture with calligraphic brushstroke borders.

**Camera:** isometric 3/4 perspective, rotatable 360°. Zoom range: city-level (individual disciple visible) to regional (full sector of map visible). No free-fly camera (this is a strategy game, not an RPG).

**Terrain rendering:**
- Hex tiles: 3D prism geometry with terrain-specific surface detail (grass for plains, snow-cap for Sacred Peaks, dark mud for swamps).
- Buildings: placed on the compound tile as a cluster of era-appropriate structures that grow with tier upgrades. T1 = humble wooden hall. T2 = stone-reinforced with a courtyard. T3 = grand compound with gates and towers.
- Units: stylized figurines (not photorealistic); color-coded by sect banner color with small affinity icon.

**Zodiac ambient effects** (applied as URP Volume Profile overrides, blended over 3 turns):
- Rat year: warm amber tint; red lanterns animated as overlaid particles on all compound tiles.
- Dragon year: gold rim-lighting on all Ley Line tiles; periodic divine light shafts.
- Tiger year: subtle red vignette at screen edges; unit aura effects pulse orange.
- Snake year: swirling mist on Forest and Mountain tiles; moonlight hue.
- (Remaining years: each has a subtle ambient palette shift and one particle emitter type. Full list in `ZodiacAmbientConfig` ScriptableObject, authored by the art team.)

**Qi visualization:** Dense and Ley Line tiles emit upward-flowing particle streams in sect-color when in player territory; neutral light blue when uncontrolled. Combat techniques render as URP VFX Graph effects — Qi projection as a visible beam; Ice Qi as crystallizing frost; Sword Arts as light-trail slashes.

### §16.2 Audio Direction

**Music system:** dynamic layered audio (implemented in the `AudioManager` using AudioMixerGroups and additive loading):
- **Base layer:** always playing; Chinese traditional instruments (guzheng, erhu) in a meditative arrangement matched to the zodiac year's mood.
- **Combat layer:** fades in when a combat is initiated or a tactical view opens; adds percussion (taiko-inspired), accelerating tempo, dissonant erhu tension notes.
- **Research layer:** fades in during Research Phase; replaces combat layer; adds dizi flute and xiao (end-blown flute) in a contemplative, ascending mode.
- **Victory stinger:** one-shot dramatic orchestral swell when a victory condition triggers.
- **Zodiac year transition:** 3-second ambient sound sting when the year changes (unique audio asset per zodiac animal).

**Sound Effects catalog (minimum required for shipping):**
- UI: click, hover, panel open/close, error, confirmation
- Combat: sword clash, staff impact, Qi discharge, unit death, army routing sound
- Building: construction completion chime, building upgrade fanfare
- Disciples: soft footstep loops (movement), short vocal bark per rank tier on selection
- Marketplace: coin clink on transaction, price rise/fall tones
- Events: event arrival chime (distinct from combat); individual stings for Tournament, Plague, Ley Line Surge
- Ambient: terrain loops (wind, river, forest birds, mountain wind, swamp insects)

**Voice:** narrator VO for key moments only (sect founding, first victory progress milestone, game over). A single authoritative voice actor in English; localization text strings provided for all subtitled events.

---

## §17. Technical Architecture

### §17.1 Architecture Philosophy — DOTS-First Hybrid

**Decision Rule for ECS vs. MonoBehaviour:**

```
Use DOTS (ECS Entities + Jobs + Burst) when:
  - The system processes > 100 identical or structurally similar objects per frame
  - The computation is data-parallel (no dependency on specific object identity)
  - The system runs on a fixed schedule (not event-driven)
  - Memory layout matters for performance (tight inner loops)

Use MonoBehaviour when:
  - The object has a unique visual identity, animation, or interaction (hero units, buildings)
  - The object requires Unity's existing lifecycle (coroutines, physics callbacks, UI events)
  - The component is authored in the Editor via Inspector
  - The system is event-driven (fires occasionally, not every frame)
```

**System Classification:**

| System | Architecture | Rationale |
|---|---|---|
| Hex tile grid (data) | **ECS Entities** | 2,400–16,000 tiles; tile data is homogeneous; spatial queries are data-parallel |
| Hex tile grid (visuals) | **Hybrid: DOTS rendering** via GPU Instancing + DrawMeshInstanced | Tile meshes share the same mesh; instancing collapses to a few draw calls per material |
| Unit crowds (Outer Disciples, Peons) | **ECS Entities** | High unit counts; movement and combat are data-parallel batch operations |
| Hero units (Elders, High Elders) | **MonoBehaviour** | Unique animations, named characters, complex interaction UIs; identity matters |
| Buildings | **MonoBehaviour prefabs** | Unique per building type; authored in Editor; Inspector-configurable |
| A* pathfinding | **Unity Jobs + Burst** | CPU-intensive; path queries for all units in a turn should complete in parallel |
| Combat auto-resolve | **Unity Jobs + Burst** | Resolve all battles in a turn simultaneously in parallel jobs |
| AI utility evaluation | **Unity Jobs** | One job per AI sect; all sects evaluated simultaneously |
| Market simulation | **Unity Jobs** | Per-compound price drift and buy/sell transactions are data-parallel |
| UI | **MonoBehaviour + Unity UI Toolkit** | Event-driven; screen-space; performance is UI-draw-call bound, not CPU-logic bound |
| Save/Load | **Plain C#** | JSON serialization; no Unity lifecycle needed |

### §17.2 Assembly Definitions

Each major system is an `.asmdef` assembly. Dependency direction is strictly enforced:

```
TalesOfTao.Core              ← GamePhase, Command base, EventChannels, ObjectPool, HexCoords
TalesOfTao.Hex               ← HexGridManager, HexTile, HexPathfinder, FogOfWar, ChunkSystem
TalesOfTao.Sects             ← SectManager, SectData, BuildingData, DiscipleData, TrainingQueue
TalesOfTao.Combat            ← CombatResolver, UnitData, TacticalBattle, CombatPowerCalculator
TalesOfTao.Economy           ← EconomyManager, TradeRoute, MarketSimulator
TalesOfTao.Research          ← ResearchManager, TechNode, TechTree, EnlightenmentTriggerSystem
TalesOfTao.Diplomacy         ← DiplomacyManager, RelationData, EspionageSystem, SettlementData
TalesOfTao.AI                ← AIController, AIGenome, GenomeDriftSystem, IAIPersonality strategies
TalesOfTao.Narrative         ← NarrativeTemplateResolver, EventScheduler, EventDefinitionSO
TalesOfTao.UI                ← UIManager, all screen controllers
TalesOfTao.SaveLoad          ← SaveLoadManager, GameStateDTO, SaveLoadRepository
TalesOfTao.DOTS              ← ECS components for hex tiles and unit crowds; Jobs for pathfinding/combat/AI/market
TalesOfTao.Tests             ← EditMode and PlayMode test assemblies
```

**Dependency direction:**
- `UI → Sects / Combat / Economy / Research / Diplomacy / Narrative → Core`
- `AI → Sects / Combat / Diplomacy / Narrative`
- `DOTS → Core / Hex / Combat / Economy / AI` (DOTS assembly bridges ECS and managed systems)
- `SaveLoad → all data assemblies`; nothing depends on `SaveLoad`
- `Narrative → Core / Sects / Diplomacy`

### §17.3 Neural LOD Strategy (GPU-Driven Mesh Reduction)

"Neural LOD" in this context refers to distance-based LOD switching driven by the GPU instance buffer, not a machine-learning system. The strategy uses Unity's `LODGroup` component in hybrid with GPU instancing for unit crowds, and a custom `StrategyLODSystem` ECS system for tile instances.

**LOD Tiers per Unit Type:**

| LOD | Distance from Camera | Mesh Detail | Technique Effects | Shadow |
|---|---|---|---|---|
| **LOD0 (Full)** | 0–20 units | Full mesh (~800 polys); all texture maps; animated | Full VFX Graph effects | Full shadow |
| **LOD1 (Medium)** | 20–60 units | Reduced mesh (~200 polys); diffuse only; simplified animation (2-bone only) | Billboard sprite effects only | Simple shadow |
| **LOD2 (Distant)** | 60–120 units | Quad billboard sprite; single diffuse texture | No effects | No shadow |
| **Culled** | >120 units | Not rendered | N/A | N/A |

**LOD Switching Implementation:**

For MonoBehaviour hero units: standard Unity `LODGroup` component. Transition is cross-faded over 0.5 seconds to avoid popping.

For DOTS unit crowds:
```csharp
// In StrategyLODSystem (ISystem, Burst-compiled):
// Each entity has a UnitRenderData component with LODLevel (byte).
// The system runs a spatial hash query per entity:
//   distanceSq = lengthsq(entity.WorldPosition - cameraPosition)
//   newLOD = distanceSq < 400 ? 0 : distanceSq < 3600 ? 1 : distanceSq < 14400 ? 2 : 3
// When LODLevel changes, set a LODChanged flag.
// A separate RenderSwitchSystem reads LODChanged flags and updates the shared
// RenderMeshArray component on each entity.
// GPU instancing: all entities at the same LOD level and same mesh share an instance buffer.
// DrawMeshInstanced called once per (mesh, material, LOD) combination.
```

**Tile LOD:**
- LOD0: full hex prism with surface detail and normal map (within 3 chunks of camera)
- LOD1: simplified flat hex with color-only material (3–6 chunks)
- LOD2: combined chunk billboard (> 6 chunks from camera)
- Chunk culling: Unity frustum culling applied to each 16×16 chunk's AABB

**Texture atlasing:** all terrain surface textures share one 2048×2048 atlas (8 terrain types × 256×256 tiles). All building diffuse textures share one 4096×4096 atlas. Atlases are authored using Unity's `SpriteAtlas` or a custom URP texture array shader.

**Draw call budget:**

| Map Size | Max Draw Calls (strategic view) | Max Draw Calls (tactical view) |
|---|---|---|
| Small | 80 | 40 |
| Medium | 120 | 40 |
| Large | 200 | 40 |
| Epic | 300 | 40 |

Tactical view is always a fixed 7×7 grid — draw calls are bounded regardless of map size.

### §17.4 Hex Grid Implementation

**Coordinate systems:**
- **Axial (q, r):** used for storage and serialization. Array indexable: `tiles[q * height + r]`.
- **Cube (q, r, s):** used for runtime algorithms where `s = −q − r`. Distance: `max(|q1-q2|, |r1-r2|, |s1-s2|)`.
- **World position (flat-top):** `x = size × (3/2 × q)`, `z = size × (√3/2 × q + √3 × r)`.

**A\* pathfinding (Jobs + Burst):**
```csharp
[BurstCompile]
public struct HexPathfinderJob : IJob
{
    [ReadOnly] public NativeArray<HexTileData> Tiles;
    [ReadOnly] public int Width, Height;
    [ReadOnly] public HexCoords Start, Goal;
    [ReadOnly] public float MovementBudget;
    public NativeList<HexCoords> ResultPath;
    // Priority queue via NativeMinHeap (custom or use Unity.Collections.NativeHeap)
    // Heuristic: cube distance
    // Per-tile cost: TerrainMovementCost + ZOCPenalty
    // Terminates early on budget exhaustion (returns partial path)
    public void Execute() { /* ... */ }
}
```

Multiple pathfinding jobs can run in parallel using `JobHandle.CombineDependencies()`. One job per unit that needs pathing; scheduled at the start of Resolution Phase; results collected before combat evaluation begins.

**Chunk system:**
- 16×16 hex chunks as `NativeArray<HexTileData>` slices for DOTS; corresponding `ChunkRenderer` MonoBehaviour for visual combining.
- Chunk dirty flag: when any tile in a chunk changes (control state, fortification, Fog of War), the chunk's combined mesh is rebuilt on the next frame (not synchronously).
- Chunk LOD: `ChunkRenderer` uses a `LODGroup` with the three tile LOD levels described above.

**Fog of War:**
- Per-sect visibility stored as `NativeArray<VisibilityState>` (Hidden=0, Explored=1, Visible=2). Indexed by axial coordinates.
- Shadowcasting (Bresenham hex field-of-view) runs as a Burst job when a unit moves. All tiles within the unit's visibility range (base: 2 tiles + elevation bonus) are marked Visible.
- On unit movement, tiles that fall outside all units' ranges revert from Visible to Explored after 1 turn.
- Hidden tiles: rendered with a dark overlay material. Explored tiles: rendered at 50% brightness with no unit visibility. Visible tiles: full brightness.

### §17.5 DOTS Specifics — ECS Component Layout

```csharp
// Hex tile entity components:
struct HexTileComponent : IComponentData
{
    public int Q, R;                    // Axial coords
    public TerrainType Terrain;
    public ElevationLevel Elevation;
    public QiDensity QiDensity;
    public byte CaveCount;
    public ControlState Control;
    public int OwnerSectId;             // -1 = unowned
    public bool IsLeyLine;
}

// Unit entity components (for Outer Disciple / Peon crowd units):
struct UnitPositionComponent : IComponentData { public float3 WorldPosition; }
struct UnitStatsComponent : IComponentData
{
    public float Attack, Defense, HP, MaxHP, Speed, QiPower;
    public float Morale;
    public int SectId;
    public UnitRank Rank;
    public int AssignedTechniqueFlags;  // Bitmask of unlocked technique IDs
}
struct UnitMovementComponent : IComponentData
{
    public HexCoords TargetHex;
    public float MovementRemaining;
    public bool IsMoving;
}

// LOD rendering:
struct UnitLODComponent : IComponentData { public byte LODLevel; }
```

**Systems running on the DOTS world:**
- `HexTileInitSystem`: creates tile entities from `HexGridManager`'s generation pass.
- `UnitMovementSystem` (Burst): processes `UnitMovementComponent` each frame during Action Phase; moves `WorldPosition` toward target.
- `PathfindingDispatchSystem`: schedules `HexPathfinderJob` for each unit with a pending path request.
- `CombatAutoResolveSystem` (Burst): batch-resolves all queued combats in parallel during Resolution Phase.
- `MarketSimulatorSystem` (Jobs): updates all `MarketSimulator` instances in parallel during Income Phase.
- `AIUtilityEvaluationSystem` (Jobs): evaluates utility scores for all AI sects simultaneously.
- `StrategyLODSystem` (Burst): updates LOD levels based on camera distance.

### §17.6 Carbon-Aware Development

**Design goal:** Tales of the Tao targets 20–30% lower battery drain compared to comparable 4X titles (e.g., Civ VI on the same hardware) through the following specific techniques.

**Adaptive Tick Rates:**
- During the player's Action Phase (human thinking time), the game loop runs at full framerate (target 60fps).
- During Resolution Phase (AI processing), the game loop drops to 30fps target. AI calculations use Burst-scheduled jobs that complete asynchronously; the main thread waits without spinning.
- During the opponent sects' turns between animations, the game loop enters a **low-power tick**: `Application.targetFrameRate = 15`. This is restored to 60fps when the next player interaction is detected.
- When the game window loses focus (Alt-Tab), `Application.targetFrameRate = 5` and all particle emitters pause.

```csharp
// CarbonAwareTickController (MonoBehaviour):
void OnApplicationFocus(bool hasFocus)
{
    Application.targetFrameRate = hasFocus ? _activeTargetFPS : 5;
    _particleManager.SetAllPaused(!hasFocus);
}

void OnPhaseChanged(GamePhase newPhase)
{
    _activeTargetFPS = newPhase == GamePhase.Action ? 60 :
                       newPhase == GamePhase.Resolution ? 30 : 15;
    Application.targetFrameRate = _activeTargetFPS;
}
```

**Burst-Scheduled Jobs:**
All AI evaluation, pathfinding, and market simulation jobs are scheduled using `IJobParallelFor` with Unity's Job System. This distributes work across available cores without spinning the main thread. Jobs are scheduled at the beginning of Resolution Phase and collected with a `Complete()` call before the next phase requires their results.

**Sleep-Aware Platform APIs:**
- On Windows: `WaitForSingleObjectEx` timeout set to match the target frame interval during low-power mode. Do not use `Thread.Sleep(0)` in hot paths; use job completion handles instead.
- On future Linux/macOS ports: use equivalent platform sleep APIs via `Platform.DeviceSleepRequest()` (Unity's adaptive performance API for supported platforms).

**Network efficiency (future multiplayer proofing):**
- Game state delta compression: only changed `HexTileData` fields are transmitted; full state sync only on join.
- Turn bundling: in multiplayer, all actions in a turn are queued locally and transmitted as a single `TurnCommandBundle` at "End Turn," not individually.
- AI computation: on Grandmaster+ single-player, long AI evaluations (>10ms per sect) are offloaded to a background Unity Job thread. Replay-critical computations (combat resolution, market prices) remain on the deterministic main-thread path with a fixed random seed derived from the turn number and sect ID, ensuring consistent replays.

**Determinism guarantee for replay:**
```
RandomSeed(turn, sectId) = Hash(gameStartSeed XOR (turn × 31337) XOR (sectId × 9973))
```
Combat resolution, market drift, and event triggers all use `Unity.Mathematics.Random` initialized with this seed. AI utility weights (non-deterministic genome drift) are serialized with the save file and replayed directly from the log.

### §17.7 Hybrid Cloud Considerations

At launch, Tales of the Tao is single-player only with a local deterministic simulation. However, the architecture is designed to support future cloud offload:

| Computation | Cloud Eligible | Rationale |
|---|---|---|
| AI utility evaluation at Heavenly Dao difficulty | **Yes** — async offload | Long evaluation chains (20ms+ budget) can be offloaded to a cloud worker; result arrives before the next phase begins; latency is not player-visible since Resolution Phase already has animation delay |
| Procedural map generation | **Yes** — session start only | Map generation on Epic size takes 200–400ms; offloading to cloud at session start is acceptable if result is cached locally |
| Combat auto-resolve | **No** — must stay local | Deterministic for replay; any network latency would break replay consistency |
| Market simulation | **No** — must stay local | Deterministic; also runs in <1ms per compound via Burst |
| Save file sync | **Yes** — cloud save | Standard cloud save pattern; save to `Application.persistentDataPath` locally and optionally sync to a cloud storage API |

**If cloud AI is implemented:** the `AIController` must have a `ICloudAIEvaluator` interface. On Heavenly Dao difficulty, when `AIBudget > 10ms`, dispatch the evaluation payload (genome, world state snapshot, available actions) to the cloud endpoint as a lightweight JSON payload. Deserialize the returned `AIDecisionResult` and apply it. Fall back to local evaluation if the cloud response exceeds a 2-second timeout.

### §17.8 USD Asset Pipeline

**Source Authoring:** all 3D assets (terrain tiles, buildings, unit figurines, props) are authored in Blender 4.x or Maya 2026. Source files are stored in `/art-source/` (not in the Unity project folder; kept in a separate art repository or LFS).

**Export Format:** assets are exported as **USD (Universal Scene Description)** `.usd` / `.usda` / `.usdc` files. Maya and Blender both have native USD exporters. The USD file carries:
- Geometry (all LOD variants as USD variant sets)
- UV maps (up to 2 UV channels: diffuse + lightmap)
- Material bindings (USD Preview Surface for interoperability)
- Skeleton and bind pose for animated units (USD Skeletal Animation)

**Naming Convention:**
```
/art-source/
  terrain/
    hex_plains_LOD.usd          ← terrain tile with LOD variant set
    hex_mountain_LOD.usd
    [terrain type]_LOD.usd      ← pattern: {type}_{qualifier}.usd
  buildings/
    bld_temple_T1.usd           ← {prefix bld}_{building}_{tier}
    bld_temple_T2.usd
    bld_training_grounds_T1.usd
  units/
    unit_outer_disciple.usd     ← {prefix unit}_{rank}[_{variant}]
    unit_inner_disciple.usd
    unit_elder_champion.usd
  props/
    prop_watchtower.usd         ← {prefix prop}_{name}
    prop_road_segment.usd
  fx/
    fx_qi_dense_particles.usd   ← {prefix fx}_{effect}
```

**Unity Import via USD Plugin:**
Import using the `com.unity.formats.usd` package (Unity USD plugin). Configure:
- `Import Scale`: 0.01 (convert cm to meters)
- `Import Materials`: use USD Preview Surface → URP Lit shader remapping
- `Import Variants`: import all LOD variants; map to Unity `LODGroup`
- `Animation Clip Import`: USD Skeletal Animation → Unity `AnimationClip`

**Unity folder structure for imported USD assets:**
```
Assets/_Game/
  Art/
    Terrain/         ← imported USD terrain meshes and materials
    Buildings/       ← imported USD building prefabs (per type, per tier)
    Units/           ← imported USD unit prefabs (per rank)
    Props/           ← imported USD props
    VFX/             ← VFX Graph assets (authored in Unity; reference USD textures)
    UI/              ← UI art (sprites, icons; NOT from USD pipeline; authored in 2D tools)
```

**Material Batching Rules:**
1. All terrain tiles of the same type share one material instance (diffuse color from texture atlas, no unique material instances per tile).
2. All building instances of the same building + tier share one material instance.
3. Unit figurines sharing the same sect color use material property blocks (`MaterialPropertyBlock.SetColor("_SectColor", sectColor)`) on a shared material — no separate material per sect.
4. No more than 4 unique materials per draw call batch group. Exceeding this breaks instancing.

**Draw Call Enforcement:** the `DrawCallAuditor` editor tool (custom Unity Editor window) should be run as part of the CI pipeline. It fails the build if draw calls in the strategic view exceed the budgets in §17.3. Implement `DrawCallAuditor` as an `EditorWindow` that captures a `FrameDebugger` snapshot programmatically and counts draw calls in the `Camera.RenderSkybox` through `Camera.RenderTransparent` passes.

### §17.9 Design Patterns in Use

| Pattern | Applied To | Benefit |
|---|---|---|
| **State Machine** | TurnManager (6 phases), Unit (idle/moving/combat/dead), TacticalBattle (placement/running/ended) | Explicit Enter/Exit hooks; eliminates switch-on-enum spaghetti |
| **Command** | All player and AI actions (`MoveUnitCommand`, `BuildBuildingCommand`, `DeclareWarCommand`, etc.) | Enables replay, undo, serialization, and AI planning by simulating command sequences |
| **Strategy** | `CombatResolver` (auto vs. tactical), `IAIPersonality` (6 implementations) | Swap behavior at runtime without conditionals |
| **Observer (SO Event Channels)** | All cross-system communication | Zero coupling; each system subscribes only to what it needs; testable in isolation |
| **Object Pool** | HexTile instances, Unit GameObjects, VFX particles, combat result popups | Eliminates GC spikes on large maps during AI turns |
| **Factory** | `UnitFactory`, `BuildingFactory`, `EventFactory` | Data-driven spawning from SO configs; single creation path |
| **Repository** | `SaveLoadRepository` | Isolates serialization from game code; swap JSON ↔ binary without touching game logic |
| **Template Method** | `NarrativeTemplateResolver` | Consistent narrative generation from parameterized templates |
| **Facade** | `GameManager` | Single entry point for scene setup; hides system initialization order |

---

## §18. Scope & Milestones

Consolidated from 18 phases to 14 milestone phases, each following the **vertical slice** methodology. Each milestone ends with a specific **Verify** condition — a playable, testable state. No phase produces code that only becomes testable in a later phase.

| Milestone | Core Deliverables | Verify Condition |
|---|---|---|
| **M0 — Project Scaffold** | Unity 6 project setup; folder structure; .asmdef assemblies; URP configured | Unity opens without errors; blank blue scene renders at target resolution |
| **M1 — Infrastructure** | EventChannelSO; Command base + CommandQueue; ObjectPool; GameManager; EditMode tests | Publish a phase-changed event; subscriber receives it; pool allocates without GC |
| **M2 — Hex Grid** | HexCoords; HexTile; HexGridManager; CameraController; Fog of War; TileSelector | Small grid generates; click any tile; data shows in context panel; camera controls work |
| **M3 — Turn System** | TurnStateMachine (6 phases); ZodiacCalendar; HUD strip; End Turn button | Press End Turn; all 6 phases cycle; zodiac label updates |
| **M4 — Sect Founding** | SectConfigSO; SectData; SectManager; FoundSectCommand; Temple prop | Founder moves to Mountain tile; Found Sect; Temple appears; Qi income shows in HUD |
| **M5 — Disciples & Buildings** | Full disciple hierarchy; BuildQueue; BuildingFactory; SectOverviewScreen | Recruit 3 peons; build Training Grounds (10-turn timer); Outer Disciple spawns on completion |
| **M6 — Units & Movement** | UnitData; Unit; HexPathfinder (Burst); MoveUnitCommand; highlight system | Select Outer Patrol; reachable tiles highlight; unit moves along A* path respecting terrain costs |
| **M7 — Combat** | CombatPowerCalculator; AutoResolver; AttackCommand; CombatResultPanel; Face/Renown update | Two opposing units adjacent; attack; result panel shows correct CP and casualties |
| **M8 — Economy & Market** | EconomyManager; TradeRoute; MarketSimulator; MarketScreen; bankruptcy handler | Trade route established; Tael increases next Income Phase; buy 3 Iron Ore; price inflates 30% |
| **M9 — Research** | TechNodeSO (all ~35 nodes); ResearchManager; EnlightenmentTriggerSystem; ResearchScreen | Queue Herbal Medicine; Enlightenment Trigger fires; research completes at 50% speed; Renown +5 |
| **M10 — Diplomacy & Espionage** | SettlementData; DiplomacyManager; trust system; EspionageSystem; DiplomacyScreen | Send gift to settlement; trust reaches Friendly; deploy spy; Gather Intel completes; buildings revealed |
| **M11 — Tactical Combat** | TacticalScene (additive); TacticalBattle; DuelResolver; MoraleSystem | Battle triggers; tactical view loads; 2 rounds execute; retreat applies correct casualties to strategic map |
| **M12 — AI & Narrative** | AIController; AIGenome; GenomeDriftSystem; 6 personality strategies; NarrativeTemplateResolver; EventScheduler | 1 Expansionist AI: builds Training Grounds within 15 turns; unique Wandering Master flavor text generates correctly |
| **M13 — Content & Balance** | All 10 SectConfigSO assets; all 9 event types; Spirit Beasts; Secret Realms; VictoryChecker | 3 AI sects; play to T80; one sect achieves a victory condition; Victory screen displays correctly |
| **M14 — Polish, Save/Load, Performance** | GameStateDTO; SaveLoadRepository; Tutorial overlays; AudioManager; VFX; performance pass | Save at T50; quit; load; all state intact. Epic map with 6 AI sects runs at <50ms per AI turn |

---

## §19. Appendix

### §A. Glossary

| Term | Definition |
|---|---|
| **Tao** | The fundamental principle underlying reality; the path of cultivation |
| **Qi** | Life energy harnessed through cultivation; fuels techniques, advancement, and research |
| **Sect** | A martial arts organization (the player's faction) |
| **Disciple** | Any member of a sect, from Peon to High Elder |
| **Technique** | A learned martial or Qi ability, assigned to Inner+ rank disciples |
| **Renown** | Global reputation score (0–∞); drives the Influence victory condition |
| **Face** | Jianghu social standing (0–100); influences market prices, settlement disposition, and AI aggression thresholds |
| **Dissent** | Internal instability caused by management ratio imbalance, bankruptcy, or broken promises |
| **Ley Line** | A channel of concentrated Qi connecting Sacred Peaks; tiles within 1 hex receive Ley Line Qi density |
| **Heavenly Tribulation** | A trial that occurs when promoting Elder → High Elder; can result in success, injury, or death |
| **Tael** | Silver currency used for all sect operations |
| **Branch Sect** | A secondary sect compound founded by a Settler Party |
| **Enlightenment Trigger** | A contextual in-game action that grants 50% instant progress toward a specific research node |
| **Behavioral Genome** | The 8-trait weight vector that defines an AI sect's decision-making priorities |
| **Inference Budget** | The per-turn CPU time allocated to an AI sect's decision evaluation |
| **Compound** | Any sect-controlled installation: main compound or Branch Sect Outpost |
| **High-Grade Iron** | Iron Ore from High/Summit elevation Mountains; counts as ×2 Iron Ore for Armory T3 and Sect Treasure forging |

### §B. Balance Levers (Priority-Ordered)

These are the variables most likely to require adjustment during playtesting. Listed in priority order — change earlier items before later ones, as earlier items have broader effects.

| Priority | Lever | Current Value | Effect of Increase | Effect of Decrease |
|---|---|---|---|---|
| 1 | Peon upkeep | 1 Tael/turn | Higher early-game financial pressure | Lower pressure; easier early game |
| 2 | Trade route base income | 10 Tael × distance multiplier | More economic incentive for expansion | Forces military or cultivation path |
| 3 | Qi cost per research tier | T1: 30, T2: 80, T3: 180, T4: 400 | Slower tech progression; technique decisions more weighted | Faster tech; technique diversity increases |
| 4 | Dissent accumulation rate | +2/turn per 10% ratio excess | More punishing overstretching | More forgiving expansion |
| 5 | Inner Disciple upkeep | 8 Tael/turn | Smaller mid-game armies | Larger armies; combat more decisive |
| 6 | Near-Victory aggression boost | +30% for 10 turns | More dramatic final sprint | Less catch-up pressure |
| 7 | Face-Slap bonus cap | min(0.3 × FaceDiff, 15) | More social mobility for low-Face sects | More stable social hierarchy |
| 8 | Enlightenment Trigger bonus | 50% research progress | Faster research via active play | Less reward for trigger completion |
| 9 | Spirit Herb rarity | 0.5 units/turn per deposit | Slower Alchemy T3+ progress | Faster Enlightenment victory path |
| 10 | AI Comeback Mechanic threshold | <25% of leader score → +20% bonus | Stronger catch-up | More decisive early snowballing |

### §C. Design Principles & Anti-Patterns

#### Principles

**Incremental playability:** every milestone ends with a build that can be launched, played, and verified. No milestone produces code only testable in a subsequent milestone.

**Vertical slices over horizontal layers:** build one feature end-to-end (data → logic → UI → feedback loop) before broadening. Architecture mistakes on a single vertical slice cost days; on ten parallel systems, they cost months.

**Data-driven by default:** if a designer might tune a value during balance passes, it lives in a ScriptableObject. Magic numbers in code are balance debt. Hot-reloading SOs during Play Mode means no recompile required for balance iteration.

**Events over direct references:** systems publish to SO Event Channels; systems subscribe. No system holds a direct reference to another system's internals. The result: any system can be removed or replaced without modifying its dependencies.

**Commands as first-class objects:** every mutation of game state is a Command. This gives replay for free, enables AI planning by simulating command sequences, and makes save/load trivial (serialize the game state snapshot + delta command log).

**Determinism by design:** all game logic that affects win/lose or AI decisions uses a seeded random generator derived from `(gameStartSeed, turn, eventIndex)`. This guarantees reproducible results for bug reports and replays.

#### Anti-Patterns to Avoid

| Anti-Pattern | Risk | Mitigation |
|---|---|---|
| **Snowballing** | Early leaders become unstoppable | Comeback mechanic; escalating upkeep; Near-Victory aggression spike |
| **Runaway tech leader** | First to T4 wins uncontested | Technique counters; espionage-based research theft; Enlightenment victory requires *all* three T4 branches (hard to achieve secretly) |
| **Quiet endgame** | Victory conditions reached with no tension | Near-Victory alert; AI aggression burst; playtest specifically for "nothing happened after T100" |
| **Scope creep** | Feature list grows; launch never comes | Kill List enforced; prototyping each mechanic before full implementation (vertical slice methodology) |
| **Manager singletons everywhere** | Hidden global state → untraceable bugs | SO event channels for communication; constructor injection for explicit dependencies; no `FindObjectOfType` in production code |
| **MonoBehaviour data** | Game state in scene objects → save/load brittleness | Plain C# classes for all runtime state; MonoBehaviours are visual proxies only |
| **Frequent Instantiate/Destroy** | GC spikes on large maps during AI turns | Object pools for all frequently created/destroyed objects from M1 onwards |
| **Undefined "later" features** | Technical debt accumulates | Kill List specifies exactly what is deferred and what replaces it; no feature is "TBD" without a replacement |

---

# Kill List

The following features from the original GDD v1.0 have been removed, merged, simplified, or deferred. Every decision is driven by one of three criteria: (O) Orphaned — doesn't reinforce the core loop; (C) Complexity cost exceeds value at current scope; (U) Undefined — would require unspecified sub-systems to implement.

| Feature | Original Location | Why Removed or Combined | What Replaces It |
|---|---|---|---|
| **Face as a separate 0–100 axis** | §10.4 | (O) Face and Renown tracked the same social concept through different lenses but neither was independently actionable; players had no "Face strategy" distinct from their Renown strategy. Two parallel scores with overlapping effects add UI and balance overhead without strategic differentiation. | **Face is retained** but reframed as a mechanical modifier tier (5 tiers, not a continuous 0–100 scale) derived from Renown + behavior flags. Its effects on market prices and AI aggression thresholds are preserved. The continuous 0–100 tracker is removed; Face tier is re-computed from game events at the end of each turn. |
| **Horses as a commodity** | §7.2 | (O) Horses had only one mechanical use (mounted unit movement) and no crafting, building, or research gate. Managing Horses as a stockpile item added inventory slots and market entries without meaningful strategic choice. | Horses become a **terrain-linked movement bonus**: Plains and Desert tiles in your territory grant +1 mounted movement to units after "Cavalry Techniques" is researched (Martial T2). No inventory entry needed. |
| **Rare Earth as a separate commodity** | §7.2 | (O) Rare Earth was referenced in exactly two places (advanced Forge research, Sect Treasure crafting) with no defined source mechanic (what is a "deep mine"?). It was a placeholder that would require a separate mining mechanic to justify. | Folded into Iron Ore as a **quality flag**: Iron Ore deposits at High/Summit Mountain elevation have a 30% chance of being High-Grade Iron (counts as ×2 Iron Ore for Armory T3 and Sect Treasure recipes). Zero new inventory slot; all existing Iron Ore logic reused. |
| **Branch Sect T3 — Autonomous Sub-Sect Governance** | §6.2 (Building Table, Branch Sect Outpost) | (C) + (U) Autonomous sub-sect governance (High Elder AI managing a mini-economy independently) is essentially a second game embedded in the first. It requires its own AI decision layer, separate UI panels, autonomous build queues, and a political relationship system between sub-sects and the player. This is post-launch DLC scope. | At T3, Branch Sect Outposts gain access to a full building list (all non-wonder buildings) but remain **directly player-controlled**. The player selects the Branch Sect from the map and manages its build queue the same way as the main compound. High Elders assigned to a Branch Sect provide a passive +10% to that compound's output rather than autonomous governance. |
| **Technique Fusion / Custom Technique Creation** | §8.3 (Martial Techniques T4 node) | (C) Custom technique creation requires a technique composition system, balance validation of all possible combos, and a UI for designing techniques. This is modding-level functionality that cannot be balanced without extensive playtesting of the complete technique tree. | The T4 Martial Techniques final node is replaced by **"Grandmaster's Legacy"** — a passive that grants +10% effectiveness to all currently researched techniques sect-wide. Clear mechanical payoff; no new sub-system required. |
| **Weapon-Type Triangle (Sword > Spear > Fist > Sword)** | §9.3 (Duel System, Phase 13) | (C) The RPS triangle requires players to memorize a counter-hierarchy that conflicts with the existing technique investment system (why research Fist Arts deeply if Sword always counters it?). It adds a memory burden without proportional strategic depth. | Duels are resolved via **stat + technique tier comparison** with a random variance component. Technique tier (T1–T4) still matters — a T4 technique gives a significant duel advantage. Weapon type identity is preserved through technique names and visual effects, just not as a hard counter mechanic. |
| **5 Difficulty Levels** | §12.3 | (C) Five difficulty levels dilute balancing effort. The delta between Grandmaster and Master was undefined in v1.0 (both gave AI bonuses; the "extra Outer Disciples" starting bonus for Grandmaster is a minor cosmetic difference). | **4 difficulty levels**: Initiate, Disciple, Master, Heavenly Dao. Grandmaster parameters fold into the Master–Heavenly Dao range. Each level has distinct, clearly stated differences (§12.3). |
| **18-Phase Development Roadmap** | §20 | (C) Phases 0 and 0.5 were setup tasks (project scaffold + core infrastructure) that do not deliver playable features. Splitting them into named phases overstated their milestone significance and created false confidence in early "phase completions." | **14 Milestone Phases** (M0–M14). Project scaffold and infrastructure are M0 and M1 respectively. The remaining 12 milestones each deliver a verifiable playable feature slice. |
| **"Insight" as a resource in the research formula** | §8.4 | (U) "Insight yield" appeared in the research speed formula but was never defined as a resource anywhere in the document. No source, no sink, no UI representation. A developer implementing this formula would have to invent a system from scratch. | Removed from the formula. **Research speed = (QiYieldPerTurn × 0.40) + (LibraryTier × 3) + (WisdomElderCount × 4) × (1 + 0.10 × LeyLineTileCount) × DifficultyModifier**. Qi is the defined, displayed resource. |
| **"Elder Council vote" (unspecified mechanics)** | §6.3 | (U) The Sect Leader death → Elder Council vote mechanic was referenced without any specification of how the vote works, what determines candidate eligibility, or how the player participates. | **Fully specified in §6.2**: on Sect Leader death, all Elders vote (High Elder = 3 votes, Elder = 1 vote). Player supports one candidate (+5 votes to their total). Highest vote count wins; ties broken by highest Qi Power. |
| **Wandering Tribe: "mercenary disciples" mechanic** | §10.2 | (U) "Mercenary disciples" was listed as a Wandering Tribe output but no mechanic for deploying them in the disciple roster was defined. Non-affinity disciples can't be trained through the normal hall system. | **Simplified**: Wandering Tribes provide one-time **Outer Disciple mercenary hire** — a temporary unit (lasts 1 battle, then disbands) that can be recruited for 40 Tael when the tribe is at Friendly trust. No roster entry; not a permanent disciple. |
| **"Formation Arrays" (Forge T4, undefined effect)** | §8.2 | (U) Formation Arrays were listed as a Tier 4 Forge unlock but had no specified mechanical effect in auto-resolve or tactical combat. A developer or artist implementing this would have to invent its function. | **Fully specified in §8.2**: Formation Arrays grant a passive combat CP bonus of **+15% to all friendly units fighting within 3 hexes of a designated Formation Elder**. Clear, implementable, balanced. |
| **"Global market" (shared across all sects)** | §7.3 | (C) A single global market where all sects' buy/sell activity shifts the same price curve requires a global commodity flow simulation that isn't supported by the rest of the economy model (which is per-compound). The original description said "simplified global market" — so simplified it was undefined. | **Per-compound markets**: each compound with a Market Pavilion runs its own price simulation. Price differences between compounds create locational trade incentives naturally. A future `GlobalMarketPressureSystem` can add a ±10% modifier to all local base prices post-launch without changing any local market logic. |
| **Demonic Cult "sacrifice" (no formula)** | §5.1 | (U) "Can sacrifice peons/disciples for instant power boosts" — what power? What formula? What cap? Without a formula, this is unimplementable. | **Fully specified sacrifice table in §5.1**: Peon batch → +5 Qi; Outer → +20 Qi; Inner → +60 Qi. Face/Renown penalties defined. Max once per rank tier per turn. Jianghu Outcry event trigger specified. |
| **Imperial Palace Edict system** | §5.1 | (U) "Issues edicts that affect all sects in range" — what edicts? What effects? What constitutes "in range"? The Imperial Court building description gives no mechanical content. | **Specified in §5.1**: Imperial Court issues one of three Edicts per 5-turn cooldown: (1) Trade Mandate — all sects within 10 tiles pay +5% on commodity trades to the Imperial Palace for 3 turns; (2) Military Draft — all sects within 10 tiles suffer −10% Tael income for 3 turns while the Imperial Palace gains +20% CP; (3) Cultural Decree — all sects within 10 tiles gain +5 Renown for 3 turns (goodwill play). Edicts are passive broadcasts; the affected sects cannot refuse but can declare war on the Imperial Palace in retaliation. |

---

# Risk Matrix

## Risk 1 — DOTS Integration Complexity

**Description:** The proposed DOTS-First architecture requires the team to work simultaneously with ECS entities (for tile and crowd data), MonoBehaviours (for hero units and buildings), and the hybrid rendering bridge (DOTS entities → GPU instancing → Unity Renderer). In Unity 6, the DOTS packages are stable but the hybrid ECS-MonoBehaviour bridge (`CompanionLink`, `EntityManager.AddComponentObject`) is known to have non-obvious constraints around scene loading, prefab instantiation, and inspector discoverability. A team unfamiliar with DOTS may spend 40–60% of early milestone time fighting framework confusion rather than building game features. Additionally, if ECS is abandoned mid-project (a documented risk in previous Unity DOTS projects), the refactor cost to revert to pure MonoBehaviours is high.

| Attribute | Value |
|---|---|
| **Likelihood** | High |
| **Impact** | High |

**Mitigation Strategy (specific and actionable):**
1. **Before M0 begins:** run a 2-week DOTS spike. One senior developer builds a standalone proof-of-concept: a 16,000-tile hex grid with DOTS entities + GPU instancing, a pathfinding job running on all tiles, and a hybrid MonoBehaviour building placed on a DOTS tile. This spike answers: "Can this team ship DOTS without the architecture collapsing?" If the answer is no, the architecture falls back to the MonoBehaviour-first approach described in §17.9 with Jobs-only (no ECS) for pathfinding and AI.
2. **Fallback decision point:** if the DOTS spike takes >3 weeks, scrap ECS. Keep Unity Jobs + Burst for pathfinding, AI evaluation, and market simulation (these are high-value, low-integration-risk). Use standard MonoBehaviours + `Graphics.DrawMeshInstanced` for crowd rendering. Draw call budget increases to ~500 on Epic maps, which is acceptable at 30fps.
3. **Canary test:** at M6 (Units & Movement), if the `HexPathfinderJob` takes >10ms average on a Medium map in the Editor, the Jobs architecture is not working correctly. Do not proceed to M7 without resolving this.

---

## Risk 2 — Economy Balance (Tael Squeeze Death Spiral)

**Description:** The economy model has a known structural risk: during early game (T1–15), upkeep can exceed income before the first trade route is established, creating a death spiral where Peon desertions from bankruptcy prevent the player from ever building their first Training Grounds. This is not a theoretical risk — the §7.2.3 balancing table explicitly notes the early squeeze ("THIS IS A KNOWN EARLY SQUEEZE"). A mistreated early economy is the #1 cause of new player abandonment in 4X games. If the squeeze is too punishing (player reaches negative Tael by T10), the tutorial mitigation (prompt to build a trade route) is insufficient because the player may not have External Affairs Hall yet.

| Attribute | Value |
|---|---|
| **Likelihood** | Medium |
| **Impact** | High |

**Mitigation Strategy (specific and actionable):**
1. **Starting Tael buffer:** player starts with 200 Tael (400 for Imperial Palace). At 38 Tael upkeep by T15 and ~24 Tael income, the player burns through 200 Tael in approximately T1–T12 if they make no income. The first trade route should be reachable by T10–12 (External Affairs Hall is T1; requires only Temple T1 which is founding). Playtest this path.
2. **Trade route early unlock:** the External Affairs Hall prerequisites only require Temple T1 (which exists from T0). The build cost must be set to be reachable by T5 with starting Tael. In M8 balancing pass: if External Affairs Hall T1 costs more than 80 Tael, reduce it. The first trade route should be established no later than T12 under normal play.
3. **Bankruptcy grace period:** implement a 3-turn grace period before Peon desertions begin. During the grace period, construction queues pause but no disciples are lost. This window gives the player time to sell commodities or establish a trade route. The grace period is displayed as a "Debt Warning" state (red Tael counter, no immediate loss) distinct from "Bankruptcy" (desertions begin).
4. **Automated playtest:** in M8, add an `EconomyStressTest` EditMode test that simulates a standard T1–T30 game with no player input (all decisions random-but-valid) across 1,000 iterations. Assert: median Tael at T15 > 50. Assert: <5% of runs reach bankruptcy before T20. If assertions fail, reduce Peon upkeep or increase starting Tael until they pass.

---

## Risk 3 — AI Turn Time Budget Violation on Epic Maps

**Description:** On an Epic map (16,000 tiles, up to 10 AI sects) with all AI sects at Heavenly Dao difficulty, the total AI evaluation budget is 10 × 20ms = 200ms per turn minimum. However, the AI evaluation chain includes: strategic goal evaluation (world state scan), tactical action scoring (50+ available actions per sect), diplomatic evaluation (10 × 9 = 90 bilateral evaluations), and combat resolution batching. In practice, world state scans on Epic maps (scanning 16,000 tile entities for strategic opportunities) without DOTS optimization can easily exceed 100ms per sect on mid-range hardware. This means the Resolution Phase could take 1–2 seconds per turn on Epic maps — tolerable once, but repeated every turn across a 150-turn game, it compounds to 5–8 minutes of AI wait time in a typical session.

| Attribute | Value |
|---|---|
| **Likelihood** | Medium |
| **Impact** | Medium |

**Mitigation Strategy (specific and actionable):**
1. **Spatial indexing for AI world scans:** the AI should never scan all 16,000 tiles for opportunities. Implement a `SpatialIndex` (grid-aligned hash map updated incrementally when tiles change control) that provides pre-filtered tile lists: "tiles within 10 hexes of sect X's territory" returned in O(1). AI strategic evaluation only queries the spatial index, not the full tile array.
2. **Staggered AI evaluation:** don't evaluate all 10 AI sects in the same frame. Distribute evaluation across the Resolution Phase sub-ticks: Sect 1–3 evaluate in tick 1, Sect 4–6 in tick 2, etc. Player sees AI unit movements animating while the next batch evaluates in background Jobs. Total wall-clock time is the same but the UI remains responsive.
3. **Strategic layer caching:** the strategic layer only re-evaluates every 3–5 turns (per §12.2). Cache the `StrategicGoal` and the top-10 candidate actions between re-evaluations. The tactical layer only needs to score cached candidates against the current resource state — not re-scan the world. This reduces per-turn AI work by ~60% on turns where strategic re-evaluation doesn't trigger.
4. **Performance gate at M12:** when the AI Controller milestone (M12) is complete, run a benchmark: `BenchmarkAITurn()` on an Epic map with 10 Heavenly Dao AI sects. Measure total Resolution Phase time. If >500ms, profile with Unity Profiler and address the top-2 hotspots before proceeding to M13. Do not ship M13 content without passing the <500ms gate.
5. **Fallback — Epic map AI downgrade:** if Heavenly Dao AI on Epic maps provably cannot meet budget, apply a transparent rule: on Epic maps with 8+ sects, Heavenly Dao AI uses 10ms budget (equivalent to Master) for strategic/diplomatic layers, but retains 20ms budget for combat-immediate decisions. This is surfaced to the player as a tooltip: "On Epic maps, AI processing is optimized for performance."

