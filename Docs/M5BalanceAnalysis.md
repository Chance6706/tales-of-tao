# M5 Balance Analysis & Proposed Changes

## Research Sources
- Civilization VI (district/building cost scaling)
- Stellaris (pop/job tier management, soft caps)
- Total War: Three Kingdoms (character hierarchy, promotion satisfaction)
- Northgard (territory control, expansion pacing)
- Age of Wonders 4 (city tier gating, unit upkeep)

---

## 1. Disciple Hierarchy Balance

### Current GDD Values
| Rank | Upkeep | Promotion Cost (Tael/Qi) | Build Turns |
|------|--------|--------------------------|-------------|
| Peon | 1 Tael | 10 Tael (recruitment) | 1 turn |
| Outer Disciple | 3 Tael | 10 Tael | 8 turns |
| Inner Disciple | 8 Tael | 25 Tael + 15 Qi + 2 Iron | 15 turns |
| Elder | 20 Tael | 60 Tael + 40 Qi + 3 Jade | 30 turns |
| High Elder | 50 Tael | 150 Tael + 100 Qi + 5 Spirit Herbs | 50 turns |

### Analysis
Comparing to TW3K character promotions (2000-5000 gold per rank) and Stellaris job tiers (1:10:100 ratio), the current disciple costs are reasonable but the **scaling factor is too linear**:

- Peon → Outer: 10 Tael (1x)
- Outer → Inner: ~40 Tael equivalent (4x)
- Inner → Elder: ~100 Tael equivalent (10x)
- Elder → High Elder: ~250 Tael equivalent (25x)

Civ VI uses ~2-3x scaling between tiers. The current 4x/10x/25x scaling means mid-game (Inner→Elder) is a huge wall, while early game is too easy.

### Proposed Changes
| Rank | Upkeep | Promotion Cost (Tael/Qi) | Build Turns | Scaling |
|------|--------|--------------------------|-------------|---------|
| Peon | 1 Tael | 10 Tael | 1 turn | — |
| Outer Disciple | 2 Tael | 10 Tael | 6 turns | 1x |
| Inner Disciple | 5 Tael | 30 Tael + 10 Qi + 2 Iron | 10 turns | 3x |
| Elder | 12 Tael | 80 Tael + 40 Qi + 2 Jade | 18 turns | 6x |
| High Elder | 30 Tael | 200 Tael + 80 Qi + 3 Spirit Herbs | 30 turns | 12x |

**Rationale:**
- Reduced build turns across the board (8→6, 15→10, 30→18, 50→30) — promotions should feel meaningful but not glacial
- Smoother cost scaling (1x → 3x → 6x → 12x) instead of exponential jumps
- Upkeep reduced at higher ranks to prevent "death spiral" where you can't afford your own disciples
- Total cost to reach High Elder: ~320 Tael + 130 Qi + resources (was ~245 Tael + 155 Qi) — slightly more expensive but much faster

---

## 2. Building Cost Balance

### Current GDD Values (T1 costs)
| Building | Tael | Qi | Lumber | Iron | Turns |
|----------|------|-----|--------|------|-------|
| Training Grounds | 50 | 0 | 20 | 0 | 4 |
| Disciple Hall | 80 | 10 | 30 | 0 | 6 |
| Library | 100 | 20 | 40 | 0 | 8 |
| Elder Council | 150 | 30 | 50 | 0 | 10 |
| External Affairs | 60 | 0 | 25 | 0 | 5 |
| Medicine Hall | 70 | 5 | 20 | 0 | 5 |
| Armory | 60 | 0 | 15 | 15 | 5 |
| Market Pavilion | 80 | 0 | 30 | 0 | 6 |
| Branch Sect Outpost | 200 | 10 | 60 | 10 | 12 |

### Analysis
Comparing to Civ VI district costs (54-100 production for early game), the current costs are reasonable but **lack resource diversity**. Most buildings only cost Tael + one resource. Stellaris uses multiple resource types per building to create interesting trade-offs.

### Proposed Changes
| Building | T1 Cost | T2 Cost | T3 Cost | Turns (T1/T2/T3) |
|----------|---------|---------|---------|-------------------|
| Training Grounds | 50T + 20L | 120T + 50L + 10Fe | 250T + 20L + 30Fe | 4/6/8 |
| Disciple Hall | 80T + 10Q + 30L | 180T + 25Q + 60L + 15Fe | 350T + 50Q + 120L + 40Fe | 6/10/14 |
| Library | 100T + 20Q + 40L | 200T + 45Q + 80L | 400T + 90Q + 150L + 5J | 8/12/18 |
| Elder Council | 150T + 30Q + 50L + 2J | 300T + 60Q + 100L + 5J | 600T + 120Q + 200L + 10J + 5SH | 10/16/24 |
| External Affairs | 60T + 25L | 140T + 10Q + 55L | 300T + 25Q + 110L + 20Fe | 5/8/12 |
| Medicine Hall | 70T + 5Q + 20L + 10MH | 160T + 15Q + 45L + 25MH | 350T + 35Q + 90L + 50MH + 5SH | 5/9/14 |
| Armory | 60T + 15L + 15Fe | 140T + 35L + 40Fe | 300T + 10Q + 70L + 80Fe + 3J | 5/9/14 |
| Market Pavilion | 80T + 30L | 180T + 5Q + 65L | 380T + 15Q + 130L + 15Fe | 6/10/15 |
| Branch Sect Outpost | 200T + 10Q + 60L + 10Fe | 400T + 25Q + 120L + 25Fe | 800T + 50Q + 250L + 50Fe + 5J | 12/18/26 |
| Dao Sanctum | 500T + 200Q + 100L + 50Fe + 20J + 10SH | — | — | 20 |

**Key changes:**
- Added Qi costs to more buildings (creates tension between disciple promotion and building)
- Added Iron Ore to military buildings (Training Grounds T2/T3, Armory)
- Added Jade to late-game buildings (Library T3, Elder Council, Branch Sect T3)
- Added Spirit Herbs to high-tier special buildings (Elder Council T3, Medicine Hall T3, Dao Sanctum)
- Turn times slightly reduced for early buildings, increased for late-game

---

## 3. Exploration & Expansion Balance

### Current State
- Branch Sect Outpost requires Elder Council T1 + Settler Party (1 Outer + 10 Peons, 3 turns to assemble)
- No explicit exploration mechanics defined

### Proposed Additions

**Exploration Scouting:**
- Outer Disciples can be sent to "Scout" a tile (costs 5 Tael, takes 2 turns)
- Scouting reveals terrain type, Qi density, and any special features (caves, deposits)
- Unscouted tiles are fogged — you can't see terrain details until scouted
- This creates a meaningful early-game decision: spend Tael on scouting vs. recruiting

**Settler Party Changes:**
- Current: 1 Outer + 10 Peons, 3 turns
- Proposed: 1 Outer + 5 Peons, 2 turns (faster, cheaper)
- Branch Sect starts with T1 Temple automatically (no need to build it)
- This makes expansion feel rewarding rather than punishing

---

## 4. Management Ratio Enforcement

### Current GDD
- 1:10 ratio (each rank manages 10 of the rank below)
- Soft cap: Dissent +2 per turn per 10% excess
- Hard cap: 200% of ratio (recruitment rejected)

### Analysis
The 1:10 ratio is very generous compared to Stellaris (1:1 for specialist/ruler ratio). This means players can have 100 peons with just 10 outer disciples. The soft cap at 200% means you can have 200 peons before hard-blocking.

### Proposed Changes
- Keep 1:10 ratio (it's thematic for a cultivation sect — one master, many disciples)
- Reduce hard cap to 150% (tighter control, forces meaningful decisions)
- Add "Dissent per turn" scaling: +1 per 5% excess (more granular than +2 per 10%)
- Add "Dissent recovery" bonus: +5/turn during Dog year (already in GDD)

---

## 5. Starting Resources & Early Game Pacing

### Current GDD
- Starting Tael: 100 (from SectConfigSO)
- Starting Qi: 20
- Starting Peons: 5

### Analysis
With 100 Tael, a player can immediately recruit 10 more peons (10 Tael each). This is too much early-game momentum. Comparing to Civ VI (start with 1 warrior, 1 settler, 100 gold), the starting resources should be more constrained.

### Proposed Changes
| Resource | Current | Proposed | Rationale |
|----------|---------|----------|-----------|
| Starting Tael | 100 | 60 | Enough for 6 peon recruits, not 10 |
| Starting Qi | 20 | 15 | Enough for 1 Outer promotion, not 2 |
| Starting Peons | 5 | 3 | Forces early recruitment decisions |
| Starting Outer Disciples | 0 | 1 | Gives the player a combat unit from the start |

**Early game pacing impact:**
- Turn 1: 3 peons, 1 outer disciple, 60 Tael, 15 Qi
- Player must choose: recruit peons (10 Tael each) or save for Outer→Inner promotion (30 Tael + 10 Qi)
- First building (Training Grounds) costs 50 Tael + 20 Lumber — achievable by turn 3-4
- First disciple promotion achievable by turn 6-8

---

## 6. Summary of Key Balance Principles

1. **Cost scaling**: 2-3x between tiers (not exponential)
2. **Time gates**: Promotions take 6-30 turns (meaningful but not glacial)
3. **Resource diversity**: Each building should cost 2-3 resource types
4. **Soft caps**: Management ratios create natural growth limits
5. **Meaningful choices**: Every rank/tier should feel like a visible power jump
6. **Early game constraint**: Starting resources should force interesting decisions
7. **Exploration incentive**: Scouting costs create tension between expansion and consolidation