# Tales of the Tao — Game Design Document

**Genre:** 4X Turn-Based Strategy
**Engine:** Unity 6 (URP)
**Platform:** Windows 64-bit (initial), expandable
**Inspirations:** Civilization (hex map, tech tree, diplomacy), Stellaris (faction identity, policies), Mount & Blade (economy, troop management, world simulation)

---

## 1. Vision Statement

> *"You have finally perceived the Tao and your discovery has led you to feel Qi! You know that you cannot be alone in the world — gather the like-minded and create your Sect. Will you be noble and wise? Will you be cunning and cruel? Reveal your fate with your own hands and tell your own Tales of the Tao."*

Tales of the Tao is a wuxia-inspired 4X turn-based strategy game where players found a martial arts sect, train disciples through hierarchical ranks, research ancient techniques, and vie for dominance across a mythical landscape steeped in Chinese martial arts lore. The game blends the grand strategy of Civilization with the factional identity of Stellaris and the ground-level economy of Mount & Blade.

---

## 2. Victory Conditions

A game ends when any sect achieves one of the following:

| Victory | Condition | Flavor |
|---|---|---|
| **Domination** | Control 60%+ of all sect capitals on the map, or be the last sect standing. | Military conquest and annexation through superior martial force. |
| **Enlightenment** | Complete the final tier of all three research branches (Alchemy, Forge, Martial Techniques) and build the **Dao Sanctum** wonder. | Your sect has transcended mortal limits and unified all paths of cultivation. |
| **Influence** | Achieve 75%+ global Renown and maintain positive relations with 80%+ of independent settlements for 20 consecutive turns. | Cultural and diplomatic supremacy — the world looks to your sect as the righteous authority. |

---

## 3. Core Gameplay Loop

```
Found Sect → Recruit Peons → Train Disciples → Build Halls → Research Techniques
     ↓              ↓               ↓               ↓              ↓
  Expand       Gather Resources   Deploy Units   Unlock Abilities  Advance Era
     ↓              ↓               ↓               ↓              ↓
  Diplomacy    Trade / Manage    Combat / Explore  Specialize     Victory
```

### 3.1 Turn Structure

Each turn represents one year in a **12-year Zodiac cycle**. The Chinese Zodiac animal of each year grants thematic bonuses and influences events, creating a strategic rhythm across the cycle.

| Year | Zodiac | Effect |
|---|---|---|
| 1 | **Rat** (鼠) | +15% Tael income (cunning, resourcefulness) |
| 2 | **Ox** (牛) | +15% Labor output (diligence, strength) |
| 3 | **Tiger** (虎) | +15% combat stats for all units (ferocity, power) |
| 4 | **Rabbit** (兔) | +15% Renown gain, +10% diplomacy (grace, peace) |
| 5 | **Dragon** (龙) | +20% Qi gathering, rare events more likely (fortune, mysticism) |
| 6 | **Snake** (蛇) | +15% espionage success, +10% Insight (wisdom, cunning) |
| 7 | **Horse** (马) | +1 movement for all units, +10% training speed (vitality, travel) |
| 8 | **Goat** (羊) | +15% herb growth, +10% provisions (harmony, nurturing) |
| 9 | **Monkey** (猴) | +15% research speed, technique discovery chance up (cleverness, invention) |
| 10 | **Rooster** (鸡) | +10% defense, fortification build speed +15% (vigilance, discipline) |
| 11 | **Dog** (狗) | +15% settlement trust gain, loyalty events (loyalty, honesty) |
| 12 | **Pig** (猪) | +20% trade income, +10% provisions (abundance, generosity) |

**Turn order:**
1. **Event Phase** — random events, diplomacy responses, zodiac year effects applied
2. **Income Phase** — collect Tael, commodities, and Qi
3. **Build Phase** — construct buildings, recruit peons, queue training
4. **Research Phase** — progress on current research
5. **Action Phase** — move units, initiate combat, perform sect actions
6. **Resolution Phase** — combat resolves, training completes, AI turns execute

---

## 4. The Hex Map

### 4.1 Tile Properties

Each hex tile has the following attributes:

| Property | Description |
|---|---|
| **Terrain Type** | Plains, Mountain, Forest, River, Lake, Desert, Swamp, Sacred Peak |
| **Elevation** | Low, Medium, High, Summit — affects movement cost, visibility, and Qi density |
| **Qi Density** | None / Sparse / Moderate / Dense / Ley Line — determines cultivation speed for sects founded here |
| **Caves** | 0–6 training caves per tile; type varies (Meditation, Body Tempering, Qi Refinement, Spirit Trial) |
| **Deposits** | Resource nodes: Iron Ore, Jade, Medicinal Herbs, Spirit Herbs, Tea Leaves, Horses, Lumber |
| **Feature** | Optional special features: Ancient Ruins, Hot Spring, Spirit Vein, Bandit Camp, Wandering Master |
| **Control** | Unowned / Sect Territory / Settlement Influence / Contested |
| **Fortification** | None / Watchtower / Garrison / Fortress — built by sects for defense |

### 4.2 Terrain Effects

| Terrain | Movement Cost | Defense Bonus | Qi Modifier | Notes |
|---|---|---|---|---|
| Plains | 1 | +0% | ×1.0 | Standard tile, best for large sect compounds |
| Forest | 2 | +25% | ×1.1 | Lumber source, ambush potential |
| Mountain | 3 | +50% | ×1.5 | High Qi, caves common, difficult to assault |
| Sacred Peak | 4 | +75% | ×2.0 | Rare; ideal sect location, natural wonder |
| River | 1.5 | +0% | ×1.2 | Trade bonus, movement penalty when crossing |
| Lake | Impassable | — | ×1.3 | Fishing resource, blocks movement |
| Desert | 2 | +0% | ×0.5 | Low Qi, sparse resources, fast horse movement |
| Swamp | 3 | -10% | ×0.8 | Medicinal herbs, movement hazard, poison ingredients |

### 4.3 Map Generation

- **Map sizes:** Small (60×40), Medium (80×60), Large (120×80), Epic (160×100)
- Procedural generation using Perlin noise for elevation, Voronoi for biome regions
- Guaranteed minimum distance between starting locations
- Each starting location guaranteed at least Moderate Qi density and 1 cave
- Ley Lines connect Sacred Peaks across the map — sects on Ley Lines gain research bonuses
- Independent settlements (villages, towns, trade posts) spawn in fertile lowlands

---

## 5. Sects (Factions)

### 5.1 Playable Sects

Each sect has a unique **Affinity** (determines starting technique style), a **Sect Trait** (passive bonus), and a **Unique Hall** (replaces one standard building).

| Sect | Affinity | Sect Trait | Unique Hall | Playstyle |
|---|---|---|---|---|
| **Wu Dang** | Internal Qi (Taiji) | +20% Qi gathering, +1 meditation cave effectiveness | Taiji Pavilion (replaces Training Grounds — disciples train faster and gain defensive techniques) | Defensive, cultivation-focused |
| **Shaolin** | Body Cultivation | +15% disciple HP, immune to attrition in adverse zodiac years | Pagoda of 108 Trials (replaces Training Grounds — produces higher base combat stats) | Balanced, tanky |
| **Tang Clan** | Poison / Hidden Weapons | +20% assassination success, poison immunity for disciples | Poison Hall (replaces Medicine Hall — produces poisons and hidden weapons alongside medicine) | Espionage, indirect warfare |
| **Mount Hua** | Sword Arts | +10% sword technique damage, +1 movement in mountains | Sword Peak (replaces Library — sword techniques research 30% faster) | Aggressive, elite units |
| **Emei** | Balanced (Sword + Qi) | +15% Renown from diplomacy, female disciples gain +10% all stats | Lotus Hall (replaces External Affairs Hall — stronger settlement influence) | Diplomacy, influence |
| **Kunlun** | Ice / Elemental Qi | +20% defense in Mountain/Sacred Peak tiles, doubled bonuses in Tiger and Rooster zodiac years | Frozen Meridian Chamber (replaces cave system — unique training cave type) | Terrain control, zodiac warfare |
| **Peng Clan** | Movement / Lightness | +2 base movement for all units, +25% trade income | Courier Network (replaces External Affairs Hall — faster diplomacy and intel) | Mobility, trade, scouting |
| **Namgung** | Sword / Noble Arts | +10% Tael income, +15% Renown from victories | Hall of Prestige (replaces Elder Council — Elders provide Renown passively) | Economy, prestige |
| **Demonic Cult** | Forbidden Arts | Can sacrifice peons/disciples for instant power boosts, -50% base Renown | Blood Altar (unique building — converts captured disciples into power or resources) | Ruthless, fast growth, diplomatic penalty |
| **Imperial Palace** | Imperial Authority | Starts with 2× Tael, can levy taxes on settlements in influence range, +20% unit cap | Imperial Court (replaces Elder Council — issues edicts that affect all sects in range) | Domination, economy, edicts |

### 5.2 Custom Sect Creation

Players may create a custom sect by selecting:
- **Name and banner**
- **Affinity** (choose from the pool above or blend two at reduced effectiveness)
- **Sect Trait** (pick one from a curated list, or a weaker custom combination)
- **Starting Technique** (one weapon style + one Qi technique from a subset)
- **Origin Story** (flavor text + minor starting bonus: e.g., "Exiled Noble" = +Tael start, "Mountain Hermit" = +Qi start)

---

## 6. Sect Management

### 6.1 Founding

1. Player begins with an **Origin Disciple** (the Founder) and one **Support Unit** (a small band of followers).
2. The Founder must move to a suitable tile and use the **Found Sect** action, which constructs the **Temple** and consumes the Founder (the Founder becomes the first Sect Leader NPC, governing from the Temple).
3. The founding tile's Qi Density, Cave count, terrain, and nearby resources permanently affect the sect's base stats.
4. After founding, the Support Unit can begin **Recruiting Peons** from the local population (costs Tael per peon).

### 6.2 Sect Buildings (Halls)

Buildings are constructed in stages. Multiple buildings can be under construction simultaneously if resources and requirements allow. Each building has 3 upgrade tiers.

| Building | Prerequisite | Function | Tier Bonuses |
|---|---|---|---|
| **Temple** | None (founded with sect) | Sect headquarters; generates base Qi income; houses Sect Leader | T2: +Qi, +1 build slot. T3: +Qi, unlocks Sect Techniques |
| **Training Grounds** | Temple T1 | Converts Peons → Outer Disciples; basic combat training | T2: Faster training, +capacity. T3: Peons arrive partially trained |
| **Disciple Hall** | Training Grounds T1 | Advances Outer → Inner Disciples; assigns techniques | T2: Inner Disciples gain bonus technique slot. T3: Unlocks Dual Technique training |
| **Library** | Disciple Hall T1 | Research hub; stores technique scrolls; disciple advancement | T2: +Research speed. T3: Can copy enemy techniques from captured scrolls |
| **Elder Council** | Library T2 | Manages Inner → Elder and Elder → High Elder advancement; sect policy decisions | T2: +1 Elder slot. T3: High Elders can govern branch sects autonomously |
| **External Affairs Hall** | Temple T1 | Diplomacy with settlements and other sects; spy deployment; trade | T2: +Trade routes. T3: Can forge alliances, issue joint declarations |
| **Medicine Hall** | Training Grounds T1 | Produces healing items, cultivation pills; Alchemy research | T2: Unlocks Spirit Herbs refining. T3: Produces breakthrough pills (rank-up aid) |
| **Armory** | Training Grounds T1 | Equips disciples with weapons and armor; Forge research | T2: Unlocks refined weapons (stat bonuses). T3: Can produce Sect Treasure weapons (unique) |
| **Market Pavilion** | External Affairs Hall T1 | Enables commodity trading with settlements and sects; converts raw resources to Tael | T2: +Trade income. T3: Can set trade monopolies on regional resources |
| **Branch Sect Outpost** | Elder Council T1 + Outer Disciple settler | Founded on a new tile to extend territory; functions as a mini-Temple | T2: Can build limited halls. T3: Becomes a full sub-sect with own build queue |

**Build Costs** scale per tier and require combinations of Tael, Lumber, Iron Ore, Jade (for higher tiers), and turns.

### 6.3 Disciple Hierarchy

Disciples are managed at a **1:10 ratio** — each rank can oversee up to 10 of the rank below it.

| Rank | Role | Ratio | Promotion Path | Notes |
|---|---|---|---|---|
| **Peon** (Initiates) | Labor, resource gathering, basic tasks | 10 per Outer | Recruited from settlements via Support Unit or Market | No combat ability; can build and gather |
| **Outer Disciple** | Basic combatant, patrol, escort, gathering lead | 10 per Inner | Train at Training Grounds (costs Tael + time) | Upon completion: choose **Branch Settler** or promote to **Inner Disciple** |
| **Inner Disciple** | Core fighting force, technique users, hall assistants | 10 per Elder | Advance at Disciple Hall (costs Qi + resources + time) | Auto-assigned one weapon technique + one combat technique by sect affinity |
| **Elder** | Hall leaders, army commanders, researchers, diplomats | 10 per High Elder | Advance at Elder Council (costs significant Qi + rare resources) | Can be **specialized** for a specific Hall (see 6.4) |
| **High Elder** | Sect council, autonomous branch leaders, crisis response | — | Advance at Elder Council T3 (very rare, endgame) | Can govern Branch Sects, lead armies, or serve on the Elder Council for sect-wide bonuses |
| **Sect Leader** | Overall sect governance (the consumed Founder) | 1 per sect | — | NPC; provides passive sect-wide bonus based on origin; can be replaced upon death via Elder Council vote |

**Management cap example:** 1 Elder can manage 10 Inner Disciples who manage 100 Outer Disciples who manage 1,000 Peons. Exceeding ratios causes **Dissent** (reduced efficiency, desertion risk).

#### Promotion Costs

Promotion is queued at the appropriate hall and resolves once the labor timer elapses. Costs are paid up-front; failure paths only exist for High Elder (see Heavenly Tribulation, §14).

| Step | Hall Required | Tael | Qi | Labor (turns) | Commodities |
|---|---|---|---|---|---|
| **Peon → Outer** | Training Grounds T1 | 10 | 0 | 8 | — |
| **Outer → Inner** | Disciple Hall T1 | 25 | 15 | 15 | Iron Ore ×2 |
| **Inner → Elder** | Elder Council T1 | 60 | 40 | 30 | Jade ×3 |
| **Elder → High Elder** | Elder Council T3 | 150 | 100 | 50 | Spirit Herbs ×5 |

Inner promotion automatically grants one Weapon Technique and one Combat Technique drawn from the sect's affinity pool (see §6.5).

### 6.4 Elder Specialization

When an Inner Disciple is promoted to Elder, they can be assigned to a Hall:

| Specialization | Assigned Hall | Bonus |
|---|---|---|
| **War Elder** | Training Grounds / Armory | +combat stats for trained disciples, faster weapon production |
| **Wisdom Elder** | Library | +research speed, technique discovery chance |
| **Medicine Elder** | Medicine Hall | +pill quality, herb yield, healing effectiveness |
| **Diplomacy Elder** | External Affairs Hall | +Renown gain, trade bonus, espionage defense |
| **Discipline Elder** | Disciple Hall | +training speed, reduced Dissent |
| **Forge Elder** | Armory | +weapon/armor quality, unlocks experimental equipment |

Unspecialized Elders are **General Elders** — they can fill any role at reduced effectiveness but provide flexibility.

### 6.5 Technique Assignment

When a disciple reaches **Inner rank**, they are automatically assigned:
- **One Weapon Technique** — based on sect affinity (e.g., Mount Hua → Sword, Shaolin → Staff/Fist, Tang → Hidden Weapons)
- **One Combat Technique** — based on sect affinity (e.g., Wu Dang → Qi Defense, Demonic Cult → Blood Arts, Kunlun → Ice Qi)

Additional techniques can be:
- Researched at the Library and manually assigned
- Acquired from captured technique scrolls (enemy sects, ancient ruins)
- Taught by Wandering Masters (random map events)
- Created via Martial Techniques research (custom technique at high research tiers)

---

## 7. Economy

### 7.1 Currency

| Resource | Type | Source | Use |
|---|---|---|---|
| **Tael** | Currency | Trade, taxes, Market Pavilion, settlement tribute | Building, recruitment, diplomacy, upkeep |
| **Qi** | Sect Resource | Temple, Ley Lines, caves, meditation | Training, research, advancement, technique use |

### 7.2 Commodities

Commodities are gathered from tile deposits and used for crafting, building, trade, and sect advancement.

| Commodity | Rarity | Source Terrain | Primary Use |
|---|---|---|---|
| **Tea Leaves** | Regional | Plains, River (warm climates) | Trade (high value), settlement gifts, morale |
| **Jade** | Uncommon | Mountain, Sacred Peak | High-tier buildings, Sect Treasures, diplomacy gifts |
| **Iron Ore** | Common | Mountain, Forest (mines) | Weapons, armor, building materials |
| **Horses** | Regional | Plains, Desert | Mounted units, trade caravans, movement bonus |
| **Lumber** | Common | Forest | Buildings, siege equipment, general construction |
| **Medicinal Herbs** | Common | Forest, Swamp, River | Medicine Hall products, healing, basic pills |
| **Spirit Herbs** | Rare | Sacred Peak, Ley Line tiles, high-Qi areas | Breakthrough pills, advanced alchemy, Enlightenment victory |
| **Rare Earth** | Rare | Mountain (deep mines) | Advanced Forge research, Sect Treasure crafting |

### 7.3 Economy Model (Mount & Blade Inspiration)

- **Upkeep:** Every disciple and building has a per-turn Tael cost. Peons cost very little; Higher ranks cost progressively more. Buildings cost based on tier.
- **Income Sources:** Settlement trade, Market Pavilion, resource sales, tribute from influenced settlements, taxation (Imperial Palace special).
- **Trade Routes:** Established between your sect and settlements or other sects via the External Affairs Hall. Each route generates passive Tael based on distance and commodity differential.
- **Supply Lines:** Armies far from your territory consume extra Tael. Severed supply lines cause attrition.
- **Market Fluctuation:** Commodity prices shift based on supply/demand across all sects (simplified global market). Oversupply crashes prices; scarcity drives them up.
- **Bankruptcy:** If Tael drops below 0, disciples begin deserting (lowest rank first), buildings pause construction, and Dissent rises sharply.

#### Marketplace Pricing

The Marketplace runs at each compound with a **Market Pavilion** hall. Each commodity has a base equilibrium price; per-turn buy/sell activity shifts the local price, and the price drifts back toward equilibrium over time.

| Commodity | Base Price (Tael) |
|---|---|
| Tea Leaves | 5 |
| Lumber | 8 |
| Medicinal Herbs | 10 |
| Iron Ore | 12 |
| Horses | 15 |
| Jade | 20 |
| Spirit Herbs | 25 |
| Rare Earth | 30 |

- **Buy inflation:** +10% per unit purchased in the current turn
- **Sell deflation:** −5% per unit sold in the current turn
- **Drift:** prices drift 5% toward base each turn (auto-stabilization)
- **Sell ratio:** sell price = 60% of buy price (merchant cut)
- **Pavilion tier markup:** T1 +20%, T2 +10%, T3 +0% (T0 cannot trade)
- **Face modifier:** very-high Face buys at −10%, very-low at +10% (see §10.4)

---

## 8. Research & Technology

Research is divided into three branches, each advanced at specific buildings. Research costs Qi and time. Higher tiers require prerequisite techs.

### 8.1 Alchemy (Medicine Hall)

Focused on cultivation aids, healing, and chemical warfare.

```
Tier 1                  Tier 2                   Tier 3                    Tier 4
──────                  ──────                   ──────                    ──────
Herbal Medicine ──────► Qi Restoration Pills ──► Spirit Condensation ────► Dao Heart Pill
     │                       │                        │                        │
     ├─ Antidote Craft ──► Poison Resistance ────► Miasma Warfare ─────► Plague of Shadows
     │                       │                        │
     └─ Herb Cultivation ► Spirit Herb Farming ──► Elixir of Insight ──► Immortal Constitution
                                                      │
                                                      └─ Breakthrough Pill ► Forced Ascension
```

### 8.2 Forge (Armory)

Focused on weapons, armor, equipment, and siege.

```
Tier 1                  Tier 2                   Tier 3                    Tier 4
──────                  ──────                   ──────                    ──────
Iron Smelting ─────────► Steel Refinement ──────► Meteoric Alloy ────────► Sect Treasure Forging
     │                       │                        │
     ├─ Basic Weapons ────► Refined Weapons ────► Masterwork Weapons ───► Legendary Armament
     │                       │                        │
     ├─ Leather Armor ────► Chainmail ──────────► Qi-Infused Armor ─────► Celestial Raiment
     │                                                │
     └─ Siege Tools ──────► Battering Ram ──────► War Machines ─────────► Formation Arrays
```

### 8.3 Martial Techniques (Library)

Focused on combat styles, Qi manipulation, and body cultivation.

```
Tier 1                  Tier 2                   Tier 3                    Tier 4
──────                  ──────                   ──────                    ──────
Basic Sword Arts ──────► Flowing Blade ─────────► Myriad Sword Form ────► Heaven-Splitting Slash
     │                                                │
Basic Fist Arts ───────► Iron Body ─────────────► Diamond Sutra ────────► Unbreakable Physique
     │                       │
Basic Qi Circulation ──► Qi Projection ─────────► Domain Expansion ─────► Qi Singularity
     │                       │                        │
     ├─ Lightness Art ───► Cloud Step ──────────► Phantom Movement ─────► Void Walk
     │                                                │
     └─ Internal Defense ► Qi Shield ───────────► Heavenly Barrier ─────► Absolute Defense
                                                      │
                                                      └─ Technique Fusion ► Custom Technique Creation
```

### 8.4 Research Mechanics

- Only **one tech per branch** can be researched at a time (up to 3 simultaneous across branches)
- Research speed scales with Qi income + assigned Wisdom Elders + Library tier
- **Technique Scrolls** found in ruins or captured from enemies can grant techs for free
- **Wandering Masters** (map events) can teach one technique instantly if you pay Tael
- Completing an entire tier in all three branches unlocks an **Era Advancement** (cosmetic + balance milestone)

**Qi Cost per Tier:**

| Tier | Qi Cost |
|---|---|
| Tier 1 | 30 |
| Tier 2 | 80 |
| Tier 3 | 180 |
| Tier 4 | 400 |

**Hall Tier Gating:** T1–T2 research requires the relevant hall at T1+; T3 requires T2+; T4 requires T3+.

**Research Speed Formula:**
`Speed = (Insight yield + Library tier × 2 + Wisdom-specialized elders × 3) × (1 + 0.10 × owned ley-line tiles)`

Wisdom Elders only count at compounds that contain a Library. Difficulty modifiers (§12.3) further scale this value.

**Renown rewards on completion:** T1 +5, T2 +10, T3 +20, T4 +40. Completing all three branches at T4 grants an additional +50 Renown milestone bonus and unlocks the Dao Sanctum (Enlightenment victory).

---

## 9. Units & Combat

### 9.1 Unit Types

| Unit | Rank Required | Role | Special |
|---|---|---|---|
| **Peon Gang** | Peons (10) | Labor, basic resource transport | No combat; flees if attacked |
| **Outer Patrol** | Outer Disciples (5–10) | Basic defense, scouting | Light combat; can build watchtowers |
| **Inner Disciple Squad** | Inner Disciples (3–5) | Core fighting force | Uses assigned techniques; can duel enemy Inner+ |
| **Elder Champion** | Elder (1) + Inner retinue (up to 5) | Elite strike force, army commander | Commands nearby units; aura buffs; powerful techniques |
| **High Elder Vanguard** | High Elder (1) + Elders (up to 3) | Endgame powerhouse | Domain effects on battlefield; can solo weaker armies |
| **Support Caravan** | Peons (5) + 1 Outer overseer | Resource transport, recruitment | Carries commodities between sect and branch/settlements |
| **Settler Party** | 1 Outer Disciple (settler) + Peons (10) | Founds Branch Sect Outpost | Consumed on founding |
| **Spy** | Inner Disciple (1) | Espionage, sabotage, intelligence | Invisible to enemies without detection; see Section 11 |

### 9.2 Movement

- Base movement = **3 hexes/turn** (modified by terrain, rank, techniques, zodiac year)
- **Lightness Art** techniques increase movement
- **Horses** grant +2 movement on Plains/Desert
- **Roads** (built by peons) reduce movement cost to 0.5 per tile
- **Fog of War** — unexplored tiles are hidden; explored but unoccupied tiles show last-known state
- **Zone of Control (ZOC)** — enemy units in adjacent hexes cost +2 movement to pass; ignore with Phantom Movement technique

### 9.3 Combat System

Combat is **auto-resolved** by default with an optional **tactical view** for important battles.

#### Auto-Resolution
- Compare total **Combat Power** of each side (sum of unit stats + technique bonuses + terrain + equipment + Elder auras)
- Apply randomness (±15%) to each side
- Casualties distributed proportionally; higher-rank units have better survival odds
- Retreating army loses 25% additional troops but preserves core units

#### Tactical View (Optional)
- Zoomed hex grid (7×7) representing the battlefield terrain
- Units placed by player; AI auto-places
- Turn-based: each unit acts once per round
- **Duels:** Inner+ rank disciples can challenge enemy Inner+ to a 1v1 duel. Winner gains morale boost; loser's side takes morale penalty. Duels resolve via technique matchups and stats.
- Battles last up to 5 rounds; unresolved battles allow retreat

#### Combat Stats

| Stat | Description |
|---|---|
| **Attack** | Base damage dealt per round |
| **Defense** | Damage reduction |
| **HP** | Health points; 0 = incapacitated/killed |
| **Speed** | Turn order in tactical view; dodge chance in auto-resolve |
| **Qi Power** | Technique effectiveness multiplier |
| **Morale** | Affects combat performance; low morale = rout risk |

---

## 10. Diplomacy & Relations

### 10.1 Sect-to-Sect Relations

| Action | Effect |
|---|---|
| **Trade Agreement** | Mutual Tael bonus; requires External Affairs Hall |
| **Non-Aggression Pact** | Cannot declare war for 10 turns; reputation penalty for breaking |
| **Technique Exchange** | Trade one known technique for another |
| **Alliance** | Shared vision, military support obligations, joint war declarations |
| **Declare Rivalry** | +15% combat bonus vs rival; -relations with their allies |
| **Declare War** | Open hostilities; must wait 5 turns after pact expires |
| **Vassalization** | Defeated sect becomes vassal — pays tribute, follows foreign policy; retains internal autonomy |
| **Sect Absorption** | Annex a defeated or willing sect entirely (very high Influence cost) |

### 10.2 Independent Settlements (City-States Analog)

Settlements are NPC population centers scattered across the map. They provide trade, recruits, and strategic value.

| Settlement Type | Provides | Interaction |
|---|---|---|
| **Village** | Peon recruits, basic commodities | Low trust threshold; easy to influence |
| **Town** | Higher trade income, diverse commodities | Medium trust; may request protection quests |
| **Trade Post** | Premium trade rates, rare commodities | High trust threshold; lucrative long-term |
| **Wandering Tribe** | Horse units, mercenary disciples | Requires gifts or military reputation |
| **Hermit's Dwelling** | Random technique scroll or Wandering Master | One-time interaction; can recruit master as temporary Elder |

**Trust** is built through:
- Gifts (Tael, commodities, Tea)
- Completing requests (defend from bandits, deliver resources)
- External Affairs Hall actions
- Proximity and consistent non-aggression

**Trust Tiers:** Hostile → Wary → Neutral → Friendly → Devoted

At **Devoted**, settlements provide bonus recruits, reduced trade costs, and vote for your sect in Influence victory calculations.

### 10.3 Renown

**Renown** is the global reputation score that drives the Influence victory.

Sources:
- Winning battles (+)
- Completing settlement requests (+)
- Advancing research (+)
- Hosting martial arts tournaments (+++)
- Breaking pacts (−−)
- Attacking settlements (−−−)
- Sacrificing disciples (Demonic Cult) (−−)
- Losing territory (−)

### 10.4 Face

**Face** is a per-sect social-standing axis tracked separately from Renown. Where Renown is global fame, Face is reputation among the Jianghu — your standing in martial-world etiquette. It is measured 0–100, defaulting to 50, and influences marketplace prices, settlement disposition gain, and AI willingness to attack.

| Face Tier | Range | Effects |
|---|---|---|
| **Esteemed** | ≥80 | −10% market buy prices, +30% disposition gain, AI hesitates to attack |
| **Respected** | 60–79 | −5% market buy prices, +15% disposition gain |
| **Neutral** | 35–59 | No modifier |
| **Diminished** | 20–34 | +5% market buy prices, −10% disposition gain |
| **Disgraced** | ≤20 | +10% market buy prices, −20% disposition gain |

**Sources of Face change:**

| Event | Δ Face |
|---|---|
| Tournament victory | +15 |
| Combat victory | +8 |
| Breakthrough witnessed | +5 |
| Treaty honored | +3 |
| Bandit tribute paid | −5 |
| Disciple defected | −8 |
| Battle lost | −10 |
| Treaty broken | −20 |

**Face-Slap bonus:** when a low-Face sect defeats a high-Face sect, the winner gains an additional `8 + min(0.3 × Face difference, 15)` Face and the loser drops by 20.

**Alignment modifier (gains only):** Righteous sects gain ×1.15 Face from positive events; Demonic sects gain only ×0.50.

---

## 11. Espionage

Spies are deployed via the **External Affairs Hall** and operate covertly in enemy territory.

| Mission | Duration | Effect |
|---|---|---|
| **Gather Intelligence** | 3 turns | Reveals enemy sect's buildings, unit count, and research |
| **Steal Technique Scroll** | 5 turns | Chance to acquire one of the target's researched techniques |
| **Sabotage Building** | 4 turns | Damages or disables a target building for several turns |
| **Sow Dissent** | 6 turns | Increases Dissent in target sect; may cause desertions |
| **Assassinate** | 8 turns | Attempt to kill an Elder or key disciple; high failure risk |
| **Counter-Intelligence** | Passive | Stationed in your own sect to detect enemy spies |

Detection chance depends on the target sect's Counter-Intelligence strength, the spy's rank, and relevant techniques (Tang Clan has significant espionage bonuses).

---

## 12. AI System

### 12.1 AI Personalities

Each AI-controlled sect has a **personality** that weights its decisions:

| Personality | Priorities | Aggression | Diplomacy |
|---|---|---|---|
| **Expansionist** | Territory, branch sects, settlements | Medium | Low |
| **Militant** | Army size, combat techniques, domination | High | Very Low |
| **Scholar** | Research, Enlightenment, technique collection | Low | Medium |
| **Diplomat** | Alliances, trade, Influence, Renown | Low | Very High |
| **Opportunist** | Adapts to game state — attacks weak, allies with strong | Variable | Variable |
| **Zealot** | Sect purity, refuses alliances, aggressive conversion | Very High | Very Low |

### 12.2 AI Decision Making

The AI uses a **utility-based system** with weighted evaluations:

1. **Strategic Layer (every 5 turns):**
   - Evaluate victory progress for self and all known sects
   - Identify primary threat and primary opportunity
   - Set strategic goal: Expand / Consolidate / Attack / Research / Diplomacy

2. **Tactical Layer (every turn):**
   - Allocate resources toward strategic goal
   - Build decisions: prioritize buildings that serve current strategy
   - Unit decisions: recruit/train/deploy based on military needs
   - Research decisions: pick tech that best supports strategy

3. **Diplomatic Layer (on event / every 3 turns):**
   - Evaluate relationships based on: relative power, proximity, shared enemies, personality
   - Propose/accept/reject diplomatic actions
   - Break agreements only if strategic advantage is overwhelming (with personality modifier)

4. **Combat Layer (on encounter):**
   - Evaluate odds before engaging; retreat if unfavorable (unless Zealot)
   - Target priority: supply caravans > isolated units > defended positions
   - Use terrain advantages when possible

### 12.3 Difficulty Scaling

| Difficulty | AI Bonus | Player Bonus |
|---|---|---|
| **Novice** | -20% income/research | +20% income/research, fog of war hints |
| **Disciple** | No modifier | No modifier |
| **Master** | +20% income/research, +10% combat | None |
| **Grandmaster** | +40% income/research, +20% combat, starts with extra Outer Disciples | None |
| **Heavenly Dao** | +60% all, perfect information, coordinated AI alliances vs player | None; designed to be unfair |

---

## 13. Game Flow — Early / Mid / Late

### 13.1 Early Game (Turns 1–30)

- **Turn 1:** Place Founder, move toward ideal founding location
- **Turns 2–5:** Found sect (consume Founder). Begin recruiting peons with Support Unit
- **Turns 5–15:** Build Training Grounds. Explore surrounding hexes. Contact nearest settlements. Begin first research
- **Turns 15–30:** Train first Outer Disciples. Build Medicine Hall or Armory. Establish first trade route. First contact with rival sects

**Player focus:** Scouting, resource acquisition, building foundation, choosing specialization direction.

### 13.2 Mid Game (Turns 30–80)

- Inner Disciples trained and deployed; techniques assigned
- Branch sects established for territory expansion
- Active diplomacy: trade agreements, rivalries declared
- Research reaching Tier 2–3; meaningful technique diversity
- First major wars between sects
- Settlement influence competition intensifies

**Player focus:** Army composition, technique strategy, economic growth, choosing a victory path.

### 13.3 Late Game (Turns 80–150+)

- Elders and High Elders active; Elder specializations matter
- Tier 3–4 research; powerful techniques reshape combat
- Sect Treasures forged (legendary unique equipment)
- Alliance blocs form; world-spanning conflicts
- Victory conditions within reach; opponents attempt to block
- Wandering Masters and ancient ruins become contested flashpoints

**Player focus:** Victory push, grand strategy, countering rivals' victory conditions.

---

## 14. Events & Encounters

Random events add unpredictability and flavor. Events fire based on game state, zodiac year, and probability.

| Event | Trigger | Effect |
|---|---|---|
| **Wandering Master Appears** | Random tile, mid-game+ | Can recruit (costs Tael) for temporary Elder or learn technique |
| **Bandit Uprising** | Low settlement trust area | Bandits spawn; clearing them earns trust with nearby settlements |
| **Ancient Tomb Discovered** | Exploration near ruins | Dungeon-like encounter; rewards technique scrolls, Jade, rare herbs |
| **Spirit Beast Sighting** | High-Qi wilderness tile | Defeat for rare materials, or bond as a disciple's spirit beast (see §14.1) |
| **Martial Arts Tournament** | Any sect can host (costs Tael) | Sects send champions; winner gains massive Renown; spectacle event |
| **Plague / Famine** | Random, more likely outside Goat/Pig years | Affects region; Medicine Hall mitigates; can spread if unchecked |
| **Sect Defector** | High Dissent in any sect | A disciple from another sect offers to join yours (may be a spy) |
| **Heavenly Tribulation** | Elder → High Elder promotion | RNG trial; success = powerful High Elder; failure = injury or death |
| **Ley Line Surge** | Near Ley Lines, rare | Temporary massive Qi boost to all sects on the Ley Line |

### 14.1 Spirit Beasts

A Spirit Beast can be bonded to a unique disciple (one beast per disciple — bonding replaces any existing bond). Beasts come in five elemental archetypes and three rarity tiers; a beast's bonus is its base value multiplied by its tier scale.

| Beast | Element | Focus | Base bonuses |
|---|---|---|---|
| **Phoenix** | Fire | Offensive cultivator | +10% Attack, +20% Tribulation Mitigation |
| **Dragon** | Water | All-rounder | +10% Attack, +10% Defense, +15% HP, +10% Cultivation, +15% Tribulation Mitigation |
| **Tortoise** | Earth | Defender / tribulation tank | +15% Defense, +20% HP, +25% Tribulation Mitigation |
| **Tiger** | Metal | Striker | +15% Attack |
| **Serpent** | Wood | Cultivator | +15% Cultivation |

**Tier scale:** Common ×1.0, Rare ×1.75, Legendary ×3.0. (E.g. a Legendary Tortoise grants +60% HP and +75% Tribulation Mitigation.)

**Acquisition:** Common beasts may bond on second-recruitment events; Rare beasts are awarded by unique-disciple breakthroughs and high Secret Realm placements; Legendary beasts come from the top placement in Secret Realms during a Dragon year.

Spirit Beast bonuses apply through `UniqueDisciple.CalculateStats` and contribute to combat, Tournament matchups, and Secret Realm trial scores.

### 14.2 Secret Realms

Secret Realms are a recurring cultivation trial that opens on the **zodiac cycle boundary every 12 turns** at a random Sacred Peak or Ancient Ruins tile. The realm stays open for 3 turns, then resolves and distributes rewards.

**Difficulty** scales with the elapsed turn count (`turn / 12`), clamped to the range 1–3.

**Participation:** each sect may submit **one** disciple per opening. Eligible disciples are non-dead Cultivators (or Commanders with combat stats).

**Trial score** is computed at resolution as:

`Score = Realm × 15 + Body × 8 + RootQuality × 5 + TechAttack × 20 + TechDefense × 15 + TraitBonus + SpiritBeastBonus − Difficulty × 5 ± random ±20%`

Trait bonuses: Lucky +10, Resilient +8, Perceptive +5, Reckless −5, Fragile −8. Spirit Beast bonus: Common +5, Rare +10, Legendary +15.

**Reward placements** (rewards upgrade by one tier during a Dragon year):

| Tier | Rewards |
|---|---|
| **Legendary** (1st in Dragon year) | Cultivation root upgrade (if below Chaos), +80 Qi, +5 Spirit Herbs, a T4 technique scroll, +20 Renown, +15 Face |
| **Major** (1st normal / 2nd Dragon) | +50 Qi, +3 Spirit Herbs, T3 technique scroll, +10 Renown |
| **Standard** (2nd) | +25 Qi, +1 Spirit Herb, +5 Renown |
| **Minor** (3rd+) | +10 Qi, +3 Medicinal Herbs |

---

## 15. UI/UX Design

### 15.1 Main Game Screen

```
┌─────────────────────────────────────────────────────────┐
│  [Sect Banner]   Turn 47 — Year of the Dragon   [Tael] [Qi] [Renown]│
├─────────────────────────────────────────────────────────┤
│                                                         │
│                                                         │
│                   HEX MAP VIEW                          │
│              (rotatable, zoomable)                       │
│                                                         │
│                                                         │
├────────┬────────────────────────────────────────────────┤
│ Mini   │  Context Panel (selected tile/unit/building)   │
│ Map    │  [Actions]  [Details]  [Techniques]            │
├────────┴────────────────────────────────────────────────┤
│ [Sect] [Research] [Diplomacy] [Military] [End Turn →]   │
└─────────────────────────────────────────────────────────┘
```

### 15.2 Key Screens

- **Sect Overview:** Building status, disciple roster, resource summary, Dissent meter
- **Research Tree:** Visual node graph per branch; click to queue
- **Diplomacy Map:** Political overlay showing territories, relations, trust levels
- **Military Overview:** All units, positions, army composition, upkeep
- **Trade Screen:** Active routes, commodity prices, available deals
- **Disciple Detail:** Individual stats, techniques, equipment, promotion status

---

## 16. Audio & Visual Direction

### 16.1 Visual Style

- Leverages existing assets for architecture and terrain
- Hex tiles rendered with 3D terrain; buildings appear as sect compounds grow
- Wuxia aesthetic: flowing robes, martial arts animations, ink-wash UI elements
- Zodiac visual theming: subtle ambient effects per zodiac year — golden glow during Dragon years, swirling mist during Snake years, red lanterns during Rat years, etc.
- Qi effects rendered as visible energy flows on high-density tiles and during combat

### 16.2 Audio

- **Music:** Traditional Chinese instruments (guzheng, erhu, dizi) with dynamic layering based on game state — peaceful during building, intense during combat, ethereal during research
- **SFX:** Martial arts impacts, sword clashes, Qi discharge, ambient nature (birds, wind, water)
- **Voice:** Narrator for key events (sect founding, victory, major battles); disciples have contextual barks

---

## 17. Technical Architecture

### 17.1 Core Systems

| System | Responsibility | Notes |
|---|---|---|
| **HexGridManager** | Hex map generation, tile data, pathfinding (A*) | Cube coordinate system; chunk-based loading for large maps |
| **SectManager** | Sect state, buildings, disciples, resources | One instance per sect; serializable for save/load |
| **TurnManager** | Turn order, phase execution, event scheduling | State machine pattern; broadcasts phase events for all systems to hook into |
| **CombatResolver** | Auto-resolve and tactical combat | Strategy pattern for swappable resolution methods |
| **AIController** | AI decision-making per sect | Strategy pattern personality modules; runs during Resolution Phase |
| **DiplomacyManager** | Relations, agreements, trust, Renown | Global singleton; tracks all bilateral relations |
| **EconomyManager** | Tael flow, commodity market, trade routes | Simulates simplified global market each turn |
| **ResearchManager** | Tech tree state, research progress | Per-sect; checks prerequisites, applies unlocks |
| **UIManager** | All UI screens, context panels, tooltips | Event-driven; decoupled from game logic via SO event channels |
| **SaveLoadManager** | Serialization of full game state | JSON-based save files via Repository pattern; autosave every 5 turns |

### 17.2 Data Architecture

The project uses a strict three-layer separation to keep logic testable and data hot-reloadable without code changes:

| Layer | Type | Used For | Rationale |
|---|---|---|---|
| **Config (static)** | ScriptableObject | Terrain definitions, technique stats, building templates, sect templates, event definitions, balance constants | Designer-editable in Inspector; survives code recompiles; no runtime allocation |
| **Event Channels** | ScriptableObject | Cross-system events (PhaseChanged, UnitMoved, ResourceChanged, CombatResolved) | Decouples publisher from subscriber; wirable in Inspector; testable in isolation |
| **Runtime State** | Plain C# classes | SectData, DiscipeData, TileState, TradeRoute, ResearchProgress | No MonoBehaviour overhead; fast to allocate, clone, and serialize |
| **Scene Entities** | MonoBehaviour | HexTile visuals, Unit GameObjects, Building props, UI panels | Only components that need Update loops or Unity lifecycle |

**Decision rule:** if a class needs `Update()`, `Start()`, or a Transform, it is a MonoBehaviour. If it is pure data that designers tune, it is a ScriptableObject. Everything else is a plain C# class.

### 17.3 Design Patterns in Use

| Pattern | Applied To | Benefit |
|---|---|---|
| **State Machine** | TurnManager (phases), Unit (idle/moving/combat/dead) | Explicit enter/exit hooks; eliminates switch-on-enum spaghetti |
| **Command** | All player and AI actions (MoveUnit, BuildBuilding, DeclareWar, etc.) | Enables undo, replay, serialization into save files, and AI planning |
| **Strategy** | CombatResolver (auto vs tactical), AIPersonality (Expansionist, Militant, etc.) | Swap behaviour at runtime without conditionals |
| **Observer / Event Channel** | System-to-system communication via SO event channels | Zero coupling between systems; each system subscribes only to what it needs |
| **Object Pool** | HexTile instances, Unit GameObjects, VFX particles, combat result popups | Eliminates GC spikes from frequent Instantiate/Destroy on large maps |
| **Factory** | UnitFactory, BuildingFactory, EventFactory | Data-driven spawning from ScriptableObject configs; single creation path |
| **Repository** | SaveLoadManager | Isolates serialization from game code; swap JSON ↔ binary without touching game logic |

### 17.4 Hex Grid Implementation

- **Axial coordinates** (q, r) for storage and serialization (compact, 2D array indexable)
- **Cube coordinates** (q, r, s where q + r + s = 0) for runtime algorithms (distance, line-of-sight, A* heuristic, rotation)
- Conversion between axial and cube is trivial: `s = -q - r`
- **A\* pathfinding** with per-terrain movement costs; priority queue via a min-heap; hex neighbors computed from cube offsets
- **Chunk system** for rendering: 16×16 hex chunks as combined meshes; chunks outside view frustum are culled
- **Texture atlasing:** all terrain and building materials share a single texture atlas → one draw call per chunk regardless of tile variety
- **GPU instancing:** unit models and tile props use `Graphics.DrawMeshInstanced`; constant draw calls regardless of unit count
- **Fog of War:** per-tile `VisibilityState` enum (Hidden / Explored / Visible) per sect; shadowcasting (hex field-of-view) updates only when units move

### 17.5 Assembly Definitions

Each major system is its own `.asmdef` assembly to enforce dependency direction, reduce incremental compile times, and allow isolated unit testing:

```
TalesOfTao.Core          ← HexCoords, EventChannel, GamePhase, Command base classes
TalesOfTao.Hex           ← HexGridManager, HexTile, HexPathfinder, FogOfWar
TalesOfTao.Sects         ← SectManager, SectData, BuildingData, DiscpleData
TalesOfTao.Combat        ← CombatResolver, UnitData, TacticalBattle
TalesOfTao.Economy       ← EconomyManager, TradeRoute, MarketSimulator
TalesOfTao.Research      ← ResearchManager, TechNode, TechTree
TalesOfTao.Diplomacy     ← DiplomacyManager, RelationData, EspionageSystem
TalesOfTao.AI            ← AIController, AIPersonality strategies
TalesOfTao.UI            ← UIManager, all screen controllers
TalesOfTao.SaveLoad      ← SaveLoadManager, GameStateDTO, Repository
TalesOfTao.Tests         ← EditMode and PlayMode test assemblies
```

Dependency direction: `UI → Sects/Combat/Economy/Research/Diplomacy → Core`. `AI` depends on `Sects/Combat/Diplomacy`. `SaveLoad` depends on all data assemblies but nothing depends on `SaveLoad`.

### 17.6 Performance Targets

| Map Size | Tiles | Target frame time (AI turn) | Strategy |
|---|---|---|---|
| Small (60×40) | 2,400 | < 16 ms | Baseline |
| Medium (80×60) | 4,800 | < 25 ms | Chunk culling |
| Large (120×80) | 9,600 | < 33 ms | + Job System for pathfinding |
| Epic (160×100) | 16,000 | < 50 ms | + GPU instancing, async AI |

---

## 18. Scope & Milestones

High-level milestone summary. Full implementation detail is in §20.

| Milestone | Deliverables |
|---|---|
| **M1 — Prototype** | Hex grid rendering, tile selection, camera controls, basic UI shell |
| **M2 — Core Loop** | Sect founding, peon recruitment, building construction, turn system |
| **M3 — Disciples** | Full rank hierarchy, training, promotion, technique assignment |
| **M4 — Combat** | Unit movement, auto-resolve combat, basic tactical view |
| **M5 — Economy** | Full resource system, trade, upkeep, market |
| **M6 — Research** | Three tech branches, research UI, technique unlocks |
| **M7 — Diplomacy** | Sect relations, settlement trust, trade agreements, espionage |
| **M8 — AI** | AI personalities, utility-based decisions, difficulty scaling |
| **M9 — Content** | All 10 sects, events, balancing pass, custom sect creator |
| **M10 — Polish** | Audio, visual effects, UI polish, tutorials, save/load |

---

## 19. Appendix

### A. Glossary

| Term | Definition |
|---|---|
| **Tao** | The fundamental principle underlying reality; the path of cultivation |
| **Qi** | Life energy harnessed through cultivation; fuels techniques and advancement |
| **Sect** | A martial arts organization (the player's faction) |
| **Disciple** | A member of a sect, ranging from peon to High Elder |
| **Technique** | A learned martial or Qi ability, assigned to disciples |
| **Renown** | Global reputation score; drives Influence victory |
| **Dissent** | Internal instability caused by overstretched management or poor economy |
| **Ley Line** | A channel of concentrated Qi connecting Sacred Peaks across the map |
| **Heavenly Tribulation** | A trial that occurs during major rank advancement |
| **Tael** | Silver currency used for trade and sect operations |
| **Branch Sect** | A secondary sect compound founded on a new tile |

### B. Balance Levers

Key parameters for tuning during development:

- Peon recruitment rate and cost
- Training time per rank
- Qi income per tile density level
- Technique power scaling per tier
- Combat power formula weights
- AI aggression thresholds
- Victory condition thresholds
- Commodity base prices and fluctuation range
- Dissent accumulation and recovery rates
- Renown gain/loss rates per action

### C. Design Principles & Anti-Patterns to Avoid

#### Principles

**Incremental playability:** every development phase ends with something that can be launched, played, and verified. No phase produces code that only becomes testable in a later phase.

**Vertical slices over horizontal layers:** build one feature end-to-end (data → logic → UI → feedback) before broadening. Catching architecture problems early on a small slice is cheap; catching them after building ten parallel systems is expensive.

**Data-driven by default:** if a designer might want to tune a value, it goes in a ScriptableObject. Magic numbers in code are a design debt. This also enables balance iteration without recompilation.

**Events over direct references:** systems publish events; systems subscribe to events. No system holds a direct reference to another system's internals. This makes testing, refactoring, and future-proofing cheap.

**Commands as first-class objects:** every mutation of game state is a Command. This gives undo for free, enables replay, allows AI planning by simulating command sequences, and makes save/load trivial (serialize the command log or the resulting state).

#### Anti-Patterns to Avoid

| Anti-Pattern | Risk | Mitigation |
|---|---|---|
| **Snowballing** | Early leaders become unstoppable; game is decided by turn 40 | Escalating expansion costs, diminishing returns on territory, AI ganging up on the leader |
| **Runaway tech leader** | First to Tier 4 wins uncontested | Technique counters (each style has a weakness), espionage to steal/sabotage research |
| **Unsatisfying endgame** | Victory conditions reached but game drags on | Detect near-victory early; accelerate AI aggression; announce imminent victory to all AIs |
| **Scope creep** | Feature list expands; nothing ships | Build vertical slices; prototype each mechanic before committing to full implementation |
| **Manager singletons everywhere** | Hidden global state causes hard-to-trace bugs | Dependency injection via constructor or ScriptableObject references; avoid `FindObjectOfType` |
| **MonoBehaviour data** | Game state living in scene objects makes save/load brittle | Plain C# classes for all runtime state; MonoBehaviours are visual proxies only |
| **Frequent Instantiate/Destroy** | GC spikes on large maps during AI turns | Object pools for all frequently created/destroyed objects from Phase 1 |

---

## 20. Development Phases

This section is the authoritative implementation roadmap. Each phase follows the **vertical slice** methodology: it produces one fully functional, playable feature end-to-end, confirmed by a specific verification step before work on the next phase begins.

**Branch:** `claude/game-dev-planning-2PlkM`
**Commit convention:** one commit per completed phase (or logical sub-step within a phase).

---

### Phase 0 — Unity Project Scaffold

**Verify:** Unity 6 opens the project without errors; folder hierarchy is intact; URP renders a blank blue scene.

#### Steps

1. Create a Unity 6 (URP) project targeting Windows x64 in `/tales-of-tao/`.
2. Configure the URP pipeline asset: Lit + Unlit passes, shadows enabled at Medium quality, no HDR required for strategy camera distances.
3. Set default aspect ratio to 16:9; lock minimum resolution to 1280×720.
4. Create the following folder structure under `Assets/_Game/`:
   ```
   Scripts/
     Core/       ← EventChannel SOs, GamePhase enum, Command base, ObjectPool
     Hex/        ← HexCoords, HexGridManager, HexTile, HexPathfinder, FogOfWar
     Sects/      ← SectManager, SectData, BuildingData, DiscipleData, TrainingQueue
     Combat/     ← CombatResolver, UnitData, Unit, TacticalBattle
     Economy/    ← EconomyManager, TradeRoute, MarketSimulator
     Research/   ← ResearchManager, TechNode
     Diplomacy/  ← DiplomacyManager, RelationData, EspionageSystem
     AI/         ← AIController, IAIPersonality, personality strategies
     UI/         ← UIManager, screen controllers
     SaveLoad/   ← SaveLoadManager, GameStateDTO
   Data/         ← All ScriptableObject assets
   Prefabs/
   Materials/
   Scenes/
   Tests/
   ```
5. Create one `.asmdef` per `Scripts/` subfolder matching the assembly names in §17.5. Set explicit references (no auto-reference).
6. Create a `Tests/` folder with `TalesOfTao.Tests.asmdef` referencing `Unity.TestFramework`.
7. Add a Unity-standard `.gitignore` (exclude `Library/`, `Temp/`, `Logs/`, `obj/`, `.DS_Store`).
8. Create a `Main` scene with a default Camera and a placeholder directional light.
9. Commit and push to branch.

---

### Phase 0.5 — Core Infrastructure

**Verify:** In a test script, publish a `GamePhaseChangedEvent`; a subscriber receives it; confirm in the Console. An object pool creates 10 objects, returns 3, re-issues them without instantiating new ones.

#### Steps

1. **`GamePhase` enum** (`Core/`): `Event, Income, Build, Research, Action, Resolution`.
2. **ScriptableObject Event Channels** (`Core/EventChannels/`):
   - Generic base: `EventChannelSO<T>` with `UnityAction<T> OnEventRaised`; `Raise(T value)` and `Subscribe/Unsubscribe` methods.
   - Concrete channels: `GamePhaseEventChannelSO`, `VoidEventChannelSO`, `IntEventChannelSO`, `StringEventChannelSO`.
   - Create asset instances for: `OnPhaseChanged`, `OnTurnEnded`, `OnResourceChanged`, `OnUnitMoved`, `OnCombatResolved`.
3. **`Command` base class** (`Core/Commands/`): abstract `Execute()` and `Undo()`. `CommandQueue` executes and records commands; supports replay.
4. **`ObjectPool<T>`** (`Core/Pooling/`): generic pool; `Get()` and `Return(T obj)`; pre-warms to a configurable initial count on `Awake`.
5. **`GameManager`** MonoBehaviour singleton (`Core/`): holds references to all SO event channels; scene entry point.
6. Write EditMode unit tests for `EventChannelSO` (raise/receive) and `ObjectPool` (get/return/no-new-allocations).

---

### Phase 1 — One Fully Formed Hex Tile

**Verify:** Run the scene; see one flat-top hex with correct terrain colour; click it; a debug panel shows all tile properties.

#### Steps

1. **`HexCoords` struct** (`Hex/`): axial fields `(int Q, int R)`; cube property `S = -Q - R`; static neighbours array (6 cube direction offsets); `Distance(HexCoords a, HexCoords b)` via cube distance; `ToWorldPosition(float size)` using flat-top conversion.
2. **`TerrainTypeSO`** ScriptableObject (`Data/Terrain/`): fields `TerrainType` enum, `MovementCost`, `DefenseBonus`, `QiModifier`, `TintColor`, `DisplayName`. Create one asset per terrain type (8 total).
3. **`HexTileData`** plain C# class (`Hex/`): `HexCoords Coords`, `TerrainTypeSO Terrain`, `ElevationLevel`, `QiDensity`, `int CaveCount`, `DepositType[]`, `FeatureType`, `ControlState`, `FortificationLevel`.
4. **`HexTile`** MonoBehaviour (`Hex/`): holds `HexTileData`; on `Start()` procedurally generates a flat-top hexagonal prism mesh (6 top verts + 6 side verts) and assigns a URP Lit material tinted by `Terrain.TintColor`. Attaches a `MeshCollider`.
5. **`TileSelector`** MonoBehaviour on Camera (`Hex/`): raycasts on mouse click; calls `TileInfoPanel.Show(HexTileData)`.
6. **`TileInfoPanel`** (`UI/`): Canvas with TextMeshPro; displays terrain type, Qi density, cave count, deposits, feature, control state.
7. Place one `HexTile` prefab instance in the `Main` scene manually; assign a Plains `TerrainTypeSO`.
8. Write PlayMode test: verify tile mesh has 12 vertices and collider is present.

---

### Phase 2 — Hex Grid

**Verify:** A Small (60×40) grid generates on Play; all terrain types are visible; clicking any tile shows its data; camera pans, zooms, and rotates smoothly.

#### Steps

1. **`HexGridManager`** MonoBehaviour (`Hex/`): `GenerateGrid(int width, int height)` instantiates `HexTile` objects from the pool into 16×16 chunk parent GameObjects. Uses axial indexing for O(1) lookup by `HexCoords`.
2. **Procedural terrain**: Perlin noise on elevation (low/medium/high/summit threshold). Voronoi biome seeding (place N biome seeds, each tile assigned to nearest seed's terrain type). Ensure Sacred Peaks are rare (≤2% of tiles).
3. **Chunk mesh combining**: after generation, each 16×16 chunk calls `MeshCombiner` to merge all tile meshes sharing the same material into a single mesh. Result: one draw call per terrain type per chunk.
4. **`CameraController`** MonoBehaviour (`Core/`): pan (WASD + middle-mouse drag), zoom (scroll wheel, clamped min/max), orbit (right-mouse drag around world-up axis). Smooth via `Mathf.Lerp`.
5. **Fog of War**: `FogOfWarLayer` MonoBehaviour; per-tile `VisibilityState` (Hidden/Explored/Visible) stored in a parallel `byte[]` array. Hidden tiles render with a darkened overlay material. Initial state: all Hidden except a 3-tile radius around the player's start point.
6. Update `TileSelector` to ignore Hidden tiles.
7. Write EditMode test: `HexGridManager` produces correct tile count; axial lookup returns correct tile.

---

### Phase 3 — Turn System Shell

**Verify:** Press **End Turn**; the HUD shows phase name cycling through all 6 phases; turn counter increments; zodiac animal and year effect update in the HUD.

#### Steps

1. **`TurnStateMachine`** (`Core/`): states `EventState`, `IncomeState`, `BuildState`, `ResearchState`, `ActionState`, `ResolutionState` each as a class implementing `ITurnState` with `Enter()`, `Tick()`, `Exit()`. `TurnManager` MonoBehaviour owns the state machine and drives transitions.
2. **`ZodiacCalendar`** static class (`Core/`): `GetZodiacYear(int turn)` returns `ZodiacYearData` (animal name, Chinese character, effect description, stat modifiers as a `ZodiacBonuses` struct).
3. **`TurnManager`**: on `EnterState` raise `OnPhaseChanged` event channel. Expose `EndTurn()` public method called by the HUD button.
4. **HUD strip** (`UI/`): TextMeshPro labels for Turn number, Zodiac animal + icon (placeholder sprite), resource counters (Tael 0 / Qi 0 / Renown 0), phase name. **End Turn** button calls `TurnManager.EndTurn()`.
5. Phase name overlay: Canvas `Animator` with a fade-in/fade-out clip triggered on `OnPhaseChanged`.
6. Write PlayMode test: call `EndTurn()` six times; verify all six phase `Enter()` hooks fire in order.

---

### Phase 4 — Sect Founding

**Verify:** Control the Founder unit; move it to a Mountain tile; press **Found Sect**; Temple prop appears; Founder disappears; Qi income shows > 0 in HUD after Income Phase.

#### Steps

1. **`SectConfigSO`** ScriptableObject (`Data/Sects/`): `SectName`, `Affinity`, `SectTrait`, `StartingTael`, `StartingQi`, `BannerColor`, `UniqueHallData`.
2. **`SectData`** plain C# class (`Sects/`): `SectConfigSO Config`, `float Tael`, `float Qi`, `float Renown`, `float Face`, `List<BuildingInstance> Buildings`, `List<Disciple> Disciples`, `HexCoords FoundingTile`, `float BasQiIncomeFromTile`.
3. **`SectManager`** MonoBehaviour (`Sects/`): owns `SectData`; subscribes to `OnPhaseChanged`; on `IncomeState.Enter` runs `ProcessIncome()` which adds Qi and Tael, then raises `OnResourceChanged`.
4. **`FounderUnit`** MonoBehaviour (`Combat/`): clickable unit; on click highlights reachable tiles (BFS, 3-tile radius). On destination click, issues `MoveUnitCommand`. Has a **Found Sect** button that activates on eligible non-water tiles.
5. **`FoundSectCommand`** (`Core/Commands/`): `Execute()` — creates `SectData`, places Temple building on tile, consumes Founder, bakes founding tile bonuses (`QiDensity × base` → `SectData.BaseQiIncome`). `Undo()` reverses.
6. **`BuildingConfigSO`** for Temple (`Data/Buildings/`): tier 1 Qi income, build slots, upkeep.
7. Temple prop: a placeholder cube scaled and tinted; placed at tile world position.
8. HUD resource labels now subscribe to `OnResourceChanged` and update.

---

### Phase 5 — Peon Recruitment & Roster

**Verify:** After founding, use the Support Unit to recruit 3 peons from a nearby Village; see them in the Sect Overview screen; Tael decreases each turn from upkeep.

#### Steps

1. **`Settlement`** MonoBehaviour (`Diplomacy/`): `SettlementType`, `TrustLevel`, `AvailablePeons` (starts at 5). Visible on map as a placeholder marker.
2. **`SupportUnit`** MonoBehaviour (`Combat/`): moves like `FounderUnit`; has **Recruit Peon** action when adjacent to a Settlement. Action costs 10 Tael and decrements `Settlement.AvailablePeons`; issues `RecruitPeonCommand`.
3. **`RecruitPeonCommand`**: creates a `Disciple` instance (rank=Peon, upkeep=1 Tael/turn) and adds to `SectData.Disciples`.
4. **`Disciple`** plain C# class (`Sects/`): `DiscpleRank Rank`, `string Name`, `float UpkeepPerTurn`, technique slots (empty at Peon).
5. **Upkeep** in `SectManager.ProcessIncome()`: sum `Disciple.UpkeepPerTurn` + `BuildingInstance.UpkeepPerTurn` and subtract from `SectData.Tael`. If Tael < 0 raise `OnBankruptcy` event.
6. **Dissent check**: if `PeonCount / OuterCount > 10`, flag `SectData.DissentLevel += 0.1f` per excess ratio unit.
7. **`SectOverviewScreen`** (`UI/`): Canvas panel (toggled from HUD); groups disciples by rank in a scrollable list; shows Tael, Qi, Renown, Dissent meter (0–100 progress bar). Opens/closes via **Sect** HUD button.

---

### Phase 6 — Building Construction

**Verify:** Open the Build menu; queue Training Grounds (costs Lumber ×3, Tael 50); wait 10 turns; it completes; a visual prop appears on the tile; Training Grounds is now listed as active in Sect Overview.

#### Steps

1. **`BuildingConfigSO`** assets for all 10 buildings (`Data/Buildings/`): `BuildingName`, `PrerequisiteBuilding`, `TierCosts[3]` (Tael, Lumber, IronOre, Jade, turns), `TierEffects[3]` (upkeep, bonuses, unlocks), `PropPrefab`.
2. **`BuildingInstance`** plain C# class (`Sects/`): `BuildingConfigSO Config`, `int CurrentTier`, `bool IsUnderConstruction`, `int TurnsRemaining`, `float UpkeepPerTurn`.
3. **`BuildQueue`** on `SectManager`: `List<BuildingInstance> Queue`; `AddToQueue(BuildingConfigSO)` validates prerequisites, checks resources, deducts cost, enqueues. Enforces simultaneous build limit (= Temple tier).
4. Build Phase hook (`TurnStateMachine.BuildState.Enter`): decrement `TurnsRemaining` on all queued items; complete finished buildings, add to `SectData.Buildings`, spawn prop via `BuildingFactory`.
5. **`BuildingFactory`** (`Sects/`): `GameObject Spawn(BuildingConfigSO config, HexCoords tile)` — gets a pooled instance, positions it, returns it.
6. **Build UI** (`UI/BuildScreen`): grid of building cards (locked/available/building/complete); each card shows name, cost, turns, and a **Build** button. Available only during Action Phase.
7. Prerequisite validation: grey-out cards where prerequisites are unmet; tooltip explains requirement.

---

### Phase 7 — Units on the Map & Movement

**Verify:** A Peon Gang unit exists on the map; select it; reachable tiles highlight; click a destination 2 tiles away; the unit moves there respecting terrain costs.

#### Steps

1. **`UnitData`** plain C# class (`Combat/`): `UnitType`, `OwnerSectId`, `HexCoords Position`, `int MovementBudget`, `int MovementRemaining`, `List<Technique> Techniques`, combat stats.
2. **`Unit`** MonoBehaviour (`Combat/`): visual proxy; holds `UnitData` reference; animates position via `Vector3.MoveTowards` along path.
3. **`HexPathfinder`** (`Hex/`): A\* implementation; `FindPath(HexCoords from, HexCoords to, UnitData unit)` returns `List<HexCoords>`; uses `TerrainTypeSO.MovementCost`; respects Fog of War (cannot path through Hidden tiles); accounts for ZOC (+2 cost per enemy-adjacent tile).
4. **`ReachableTilesCalculator`** (`Hex/`): BFS flood fill from unit position up to `MovementRemaining`; returns `HashSet<HexCoords>`.
5. **Movement highlight**: on unit selection, overlay a highlight material on all reachable tiles (uses a second UV channel or a separate highlight mesh layer).
6. **`MoveUnitCommand`**: `Execute()` runs A\*, moves `UnitData.Position`, decrements `MovementRemaining`, raises `OnUnitMoved`. `Undo()` reverses.
7. **`UnitDataSO`** configs for Peon Gang, Outer Patrol, Support Caravan, Settler Party (`Data/Units/`): base stats, movement budget, upkeep, sprite/prefab reference.
8. Action Phase gate: movement commands only accepted during `ActionState`.
9. Write EditMode test: pathfinder correctly costs Mountain tiles at 3× vs Plains at 1×; returns null for unreachable targets.

---

### Phase 8 — Training & Disciple Promotion

**Verify:** Queue a Peon → Outer promotion (costs 10 Tael, 8 turns) at Training Grounds; wait 8 turns; an Outer Disciple Squad unit spawns near the sect tile; the Peon count decreases by 10.

#### Steps

1. **`TrainingQueue`** on `SectManager`: separate from `BuildQueue`; entries track `PromotionStep` (enum), resources deducted on enqueue, `TurnsRemaining`.
2. Build Phase hook: decrement training timers; on completion call `CompletePromotion(PromotionStep)`.
3. **Promotion validation**: checks hall presence, resource availability, management ratio headroom. Shows warning if ratio would be exceeded.
4. **Technique auto-assignment** on Inner promotion: `SectConfigSO.AffinityWeaponTechniques` and `AffinityQiTechniques` lists; pick first unassigned technique from each list matching affinity.
5. **`TechniqueConfigSO`** (`Data/Techniques/`): `TechniqueName`, `TechniqueType` (Weapon/Qi/Movement/Defense), `StatBonuses`, `RequiredResearchNode`.
6. On promotion completion: remove N Peons from `SectData.Disciples`, add 1 Outer/Inner/Elder; raise `OnDiscipleRankChanged` event; spawn corresponding unit via `UnitFactory`.
7. **`UnitFactory`** (`Combat/`): `UnitData Spawn(DiscpleRank rank, SectData owner, HexCoords position)`.
8. **Promotion UI** in `SectOverviewScreen`: each rank group has a **Promote** button that opens a cost/time preview and queues on confirm.

---

### Phase 9 — Auto-Resolve Combat

**Verify:** Two opposing units on adjacent tiles; select your unit; press **Attack**; `CombatResultPanel` shows both sides' power, casualties, and outcome; winning unit remains; losing unit is removed.

#### Steps

1. **`CombatPowerCalculator`** static class (`Combat/`): `float Calculate(UnitData unit, HexTileData tile)` — sums base stats, technique bonuses, terrain defense modifier, Elder aura (0 for now), equipment bonuses.
2. **`AutoResolver`** (`Combat/`): `CombatResult Resolve(UnitData attacker, UnitData defender, HexTileData attackerTile, HexTileData defenderTile)`. Applies ±15% random factor to each CP. Distributes casualties proportionally. Retreat logic: if attacker CP < 60% of defender CP, auto-retreat (−25% additional casualties). Returns `CombatResult` (winner, casualties each side, retreat flag).
3. **`AttackCommand`** (`Core/Commands/`): validates target is adjacent and enemy; calls `AutoResolver.Resolve`; applies casualties to `UnitData`; removes dead units; raises `OnCombatResolved`.
4. **`CombatResultPanel`** (`UI/`): modal showing attacker icon + CP, vs defender icon + CP, casualty numbers, outcome string ("Victory" / "Retreat" / "Defeat"). Dismisses on click.
5. Post-combat: `SectManager` updates `SectData.Face` and `SectData.Renown` via `OnCombatResolved` subscriber.
6. Attack button: appears in context panel when a unit is selected and an enemy occupies an adjacent tile during Action Phase.
7. Write EditMode tests: `AutoResolver` deterministically resolves when random seed is fixed; 2× CP attacker always wins.

---

### Phase 10 — Settlement Trust & First Trade Route

**Verify:** Use External Affairs Hall → Send Gift (10 Tael, +trust); trust reaches Friendly; open a Trade Route; Tael income increases by the expected amount in the next Income Phase.

#### Steps

1. **`SettlementData`** plain C# class (`Diplomacy/`): `SettlementType`, `TrustLevel` (enum + float 0–100), `AvailableCommodities[]`, `TradeIncomePotential`.
2. **`TrustLevelConverter`**: float 0–100 → `TrustTier` enum (Hostile/Wary/Neutral/Friendly/Devoted). Devoted: bonus recruits, −5% trade cost.
3. **`DiplomacyManager`** MonoBehaviour (`Diplomacy/`): global singleton; owns `List<SettlementData>` and `List<SectRelation>`; processes trust changes; evaluates Influence victory.
4. **`SendGiftCommand`**: deducts Tael/commodity from `SectData`, calls `DiplomacyManager.ChangeTrust(settlementId, +amount)`, raises `OnTrustChanged`.
5. **`TradeRoute`** plain C# class (`Economy/`): `SectId`, `SettlementId`, `TaelPerTurn` (distance-scaled + commodity differential formula). Created by `OpenTradeRouteCommand` (requires Friendly trust + External Affairs Hall).
6. `EconomyManager.ProcessIncome()`: sum all active `TradeRoute.TaelPerTurn` and add to Tael during Income Phase.
7. **`DiplomacyScreen`** (`UI/`): lists settlements with trust tier, commodity offering, active routes, available actions. Opens from HUD **Diplomacy** button.
8. External Affairs Hall prerequisite gate: actions greyed out if hall is absent or destroyed.

---

### Phase 11 — Research System

**Verify:** Open the Research tree; queue "Herbal Medicine" (30 Qi, 5 turns); wait; it completes; the technique becomes assignable to an Inner Disciple; Renown increases by 5.

#### Steps

1. **`TechNodeSO`** ScriptableObject (`Data/Research/`): `TechName`, `Branch` (Alchemy/Forge/Martial), `Tier`, `QiCost`, `TurnsBase`, `Prerequisite TechNodeSO[]`, `UnlockEffect` (technique SO, building unlock, or stat modifier).
2. Create all tech tree node assets matching §8.1–§8.3 (≈35 nodes total).
3. **`ResearchManager`** (`Research/`): per-sect; `ActiveResearch[3]` (one per branch); `ResearchProgress` dictionary keyed by `TechNodeSO`. Research Phase hook: adds `CalculateSpeed()` progress to active nodes; completes when progress ≥ `QiCost`.
4. **`CalculateSpeed()`**: `(QiYield + LibraryTier × 2 + WisdomElderCount × 3) × (1 + 0.10 × leyLineTileCount)`. Difficulty multiplier applied on top.
5. On completion: apply `TechNodeSO.UnlockEffect`; add +Renown per tier table; raise `OnResearchCompleted`.
6. **`ResearchScreen`** (`UI/`): three scrollable node-graph panels; each node renders as a card (name, cost, progress bar if active, locked/unlocked state); arrows show prerequisites; click to queue.
7. Hall tier gate enforced at queue time (T3 tech requires Library T2+).
8. Write EditMode tests: prerequisite check, speed formula, tier gate.

---

### Phase 12 — Economy & Market

**Verify:** Open Market Pavilion; buy 3 units of Iron Ore; price increases by 30%; next turn price drifts 5% back toward base; selling 2 units decreases price.

#### Steps

1. **Commodity inventory** on `SectData`: `Dictionary<CommodityType, int> Stockpile`.
2. **`MarketSimulator`** (`Economy/`): per-compound; `Dictionary<CommodityType, float> CurrentBuyPrice`; initialized to base prices (§7.3 table). `Buy(commodity, qty)`: price × (1 + 0.10 × qty), deducts Tael, adds to stockpile. `Sell(commodity, qty)`: sell price = 60% of buy, price × (1 − 0.05 × qty). Each turn `Drift()` moves prices 5% toward base.
3. **Face modifier**: `FaceModifier = Face ≥ 80 ? 0.9f : Face ≤ 20 ? 1.1f : …` applied to buy price in `Buy()`.
4. **Pavilion tier markup**: T1 ×1.20, T2 ×1.10, T3 ×1.00; applied to buy price.
5. **Bankruptcy handler**: `OnBankruptcy` subscriber in `SectManager` — starts deserting Peons (remove 1 per turn), pauses build queue, sets `DissentLevel += 10`.
6. **`MarketScreen`** (`UI/`): commodity list with buy price, sell price, your stockpile qty, quantity stepper, Buy/Sell buttons. Prices update in real time as quantity is adjusted (preview). Opens from the Market Pavilion building card.
7. **Supply line upkeep**: armies beyond 5 tiles from nearest friendly tile cost +1 Tael/turn per unit. Calculated in `EconomyManager.ProcessIncome()`.

---

### Phase 13 — Tactical Combat View

**Verify:** Declare a battle; choose "Tactical View"; a 7×7 hex arena loads; place your units; execute 2 rounds; retreat; return to strategic map with correct casualties applied.

#### Steps

1. **`TacticalScene`** loaded additively (`Scenes/Tactical.unity`): separate 7×7 hex grid generated from the strategic tile's terrain seed (same terrain type as battle location).
2. **`TacticalBattle`** MonoBehaviour (`Combat/`): receives `BattleSetup` struct (attacker units, defender units, terrain); instantiates unit tokens on grid.
3. **Unit placement**: player drags unit tokens to desired starting hexes; AI auto-places (spread formation).
4. **Round loop**: turn order by Speed stat (insertion sort); each unit has Move + Attack actions (optional); `TacticalUnit` state machine (Idle → Moving → Attacking → Done).
5. **Duel system**: Inner+ unit can **Challenge** adjacent enemy Inner+ (button in context panel). `DuelResolver.Resolve(a, b)` compares technique matchups (Weapon type rock-paper-scissors: Sword > Spear > Fist > Sword) + stat comparison. Winner gains +10 Morale to their side; loser's side −10.
6. **5-round limit** + **Retreat** button (appears from round 2 onwards): retreat costs 25% of remaining units; exits tactical scene.
7. On battle end: `TacticalResult` passed back to `CombatResolver`; casualties applied; strategic map restored.
8. **`MoraleSystem`**: low morale (<20) triggers rout roll each round; failed rout = unit auto-retreats.

---

### Phase 14 — AI Controller

**Verify:** Start a new game with one AI sect (Expansionist personality); watch it build Training Grounds within 15 turns, recruit units, and attempt to influence a nearby settlement by turn 30.

#### Steps

1. **`IAIPersonality`** interface (`AI/`): `StrategicGoal EvaluateGoal(SectData self, GameState world)`, `float WeightBuild(BuildingConfigSO b)`, `float WeightResearch(TechNodeSO t)`, `float WeightDiplomacy(DiplomacyAction a, SectData target)`.
2. **Concrete personalities**: `ExpansionistAI`, `MilitantAI`, `ScholarAI`, `DiplomatAI`, `OpportunistAI`, `ZealotAI` — each implements `IAIPersonality` with tuned weight tables.
3. **`AIController`** MonoBehaviour (`AI/`): runs during `ResolutionState`; holds personality instance (swappable); layers:
   - **Strategic** (every 5 turns): calls `EvaluateGoal`; sets `CurrentGoal`.
   - **Tactical** (every turn): calls weighted evaluators; issues `BuildCommand`, `TrainCommand`, `StartResearchCommand` via `CommandQueue`.
   - **Diplomatic** (every 3 turns): scores each possible diplomatic action; issues highest-value action if score > threshold.
   - **Combat** (on adjacent enemy detection): calls `CombatPowerCalculator`; attacks if win probability > 55% (Zealot: always attacks). Retreats if < 40%.
4. **Difficulty multipliers**: `AIController` reads difficulty setting; scales `SectData` income and research speed at game start.
5. AI cannot cheat fog of war except on Heavenly Dao difficulty.
6. Write EditMode tests: `MilitantAI.WeightBuild` returns highest weight for Armory and Training Grounds.

---

### Phase 15 — Espionage

**Verify:** Deploy a Spy to an enemy sect tile; assign "Gather Intelligence" mission; wait 3 turns; enemy building list is revealed in Diplomacy screen.

#### Steps

1. **`SpyUnit`** extends `Unit` (`Combat/`): `IsDetected` flag; invisible to enemies unless counter-intel threshold exceeded. Movement uses same pathfinder; can enter enemy territory.
2. **`EspionageMission`** class (`Diplomacy/`): `MissionType`, `TurnsRemaining`, `SuccessChance` (computed from `SpyRank + TechBonus vs TargetCounterIntelStrength`).
3. **Detection roll** each turn: `Random.value < DetectionChance` → spy captured (removed from map, raise `OnSpyCaptured`) or killed.
4. **Mission resolution** on `TurnsRemaining == 0`: roll success. On success apply effect:
   - *Gather Intel*: reveal target `SectData.Buildings` and unit count to player for 10 turns.
   - *Steal Scroll*: random researched tech from target → player's `ResearchManager.UnlockTech(node)`.
   - *Sabotage*: random building set `IsDisabled = true` for 5 turns.
   - *Sow Dissent*: target `SectData.DissentLevel += 20`.
   - *Assassinate*: remove a random Elder from target roster; Face penalty for attacker if discovered.
5. **Counter-Intelligence** passive: `SectData.CounterIntelStrength` = count of stationed Spy units on own tiles × 10. AI sects auto-maintain 1 counter-spy.
6. **Tang Clan bonus**: `+20%` to `SuccessChance` for all missions; `+30%` for Assassinate.
7. Espionage actions in `DiplomacyScreen`: Deploy Spy button (requires External Affairs Hall); mission assignment panel for active spies.

---

### Phase 16 — Events & Encounters

**Verify:** Advance to turn 15; a Bandit Uprising event fires near a low-trust settlement; a Bandit unit spawns; defeating it raises settlement trust; the event result is logged in the event history panel.

#### Steps

1. **`EventDefinitionSO`** ScriptableObject (`Data/Events/`): `EventType`, `TriggerCondition` (C# predicate reference via SO function), `BaseProbability`, `ZodiacModifiers[]`, `FlavorText`, `Options[]` (player choices + effect descriptors).
2. **`EventScheduler`** (`Core/`): checked at start of `EventState`; evaluates all `EventDefinitionSO` conditions + zodiac modifiers; fires up to 2 events per turn; queues them for sequential display.
3. **`EventModal`** (`UI/`): shows event art placeholder, flavor text, 1–3 option buttons; on choice executes `EventEffect` (resource change, unit spawn, trust change, etc.).
4. Implement all 9 event types from §14: Wandering Master, Bandit Uprising, Ancient Tomb, Spirit Beast, Tournament, Plague/Famine, Sect Defector, Heavenly Tribulation, Ley Line Surge.
5. **Heavenly Tribulation**: on `Elder → High Elder` promotion, roll `tribulationScore = baseStats + spiritBeastMitigation ± random`. Score < threshold → injury (−20% stats for 10 turns) or death (remove disciple).
6. **Spirit Beast bonding** (`Sects/SpiritBeast`): `SpiritBeastSO` config (archetype, tier, bonus table); `Disciple.BondedBeast`; `Disciple.CalculateStats()` multiplies base stats by beast tier bonuses.
7. **Secret Realm** (§14.2): fires every 12 turns at zodiac boundary; `SecretRealmEvent` spawns on random Sacred Peak; 3-turn window; score formula computed; rewards distributed via `OnSecretRealmResolved`.
8. **Event history panel** in `SectOverviewScreen`: scrollable log of last 20 events with turn number and outcome.

---

### Phase 17 — All 10 Sects, Zodiac Bonuses & Victory Conditions

**Verify:** Start a game with 3 AI sects using different personalities; play to turn 80; a victory condition triggers; a Victory screen displays correctly identifying the winning sect and condition type.

#### Steps

1. Create all 10 `SectConfigSO` assets (`Data/Sects/`) with correct affinities, traits, unique hall references, and starting technique pools matching §5.1.
2. **Zodiac bonus application**: `TurnManager` reads `ZodiacCalendar.GetCurrentBonuses(turn)` on `EventState.Enter`; publishes `ZodiacBonusesEvent`; all relevant systems subscribe (`SectManager` for income/research, `CombatResolver` for combat stats, `EconomyManager` for trade).
3. **`VictoryChecker`** (`Core/`): runs at end of `ResolutionState`. Checks:
   - *Domination*: sect controls ≥60% of enemy capitals or is last sect standing.
   - *Enlightenment*: all three branches at T4 complete AND Dao Sanctum building present.
   - *Influence*: Renown ≥75% of max possible AND ≥80% of settlements at Friendly+ for 20 consecutive turns.
4. Near-victory detection: when any sect is within 10 turns of a likely victory, raise `OnVictoryImminent`; all AIs receive +30% aggression toward that sect regardless of personality.
5. **Victory/Defeat screen** (`UI/`): full-screen overlay; sect banner; victory type, turn count, score breakdown. "Play Again" and "Main Menu" buttons.
6. **Custom sect creator** (`UI/SectCreatorScreen`): name input, banner color picker, affinity dropdown (single or blended ×0.75 each), trait picker (list of all traits, pick one), origin story picker (5 options + minor bonus description).

---

### Phase 18 — Polish, Audio & Save/Load

**Verify:** Save mid-game on turn 50; quit to main menu; load save; all units, buildings, research progress, and relations are exactly as they were.

#### Steps

1. **`GameStateDTO`** (`SaveLoad/`): plain C# serialization graph — captures all `SectData`, `TileState[]`, `UnitData[]`, `TradeRoute[]`, `ResearchProgress`, `DiplomacyManager` state, `TurnManager` turn/phase, `MarketSimulator` prices. No `MonoBehaviour` references; only IDs and value types.
2. **`SaveLoadRepository`** (`SaveLoad/`): `Save(string slotName)` serializes `GameStateDTO` to JSON via `Newtonsoft.Json`; `Load(string slotName)` deserializes and calls `GameManager.RestoreState(dto)`. Autosave every 5 turns via `OnTurnEnded` subscriber.
3. **State restoration**: `GameManager.RestoreState(dto)` rebuilds all runtime objects from DTO: respawns units via `UnitFactory`, rebuilds buildings via `BuildingFactory`, restores tile visibility, reconnects event channel subscriptions.
4. **Tutorial overlays**: `TutorialManager` MonoBehaviour; each phase's first-run key action triggers a `TutorialTooltip` (arrow + text). State persisted to `PlayerPrefs`.
5. **Audio integration** (`Core/Audio/`): `AudioManager` with dynamic music layer system (base layer always plays; combat layer fades in during battle; research layer during Research Phase). SFX events raised on `OnCombatResolved`, `OnBuildingCompleted`, `OnResearchCompleted`.
6. **Visual FX**: Qi density particles on Dense/Ley Line tiles (Unity particle system, pooled). Combat impact VFX. Building construction dust cloud. Zodiac year ambient overlay (full-screen post-process color grade per zodiac animal using URP volume profiles).
7. **Performance pass**: profile on Epic map with 6 AI sects. Target frame times per §17.6. Introduce `Unity.Jobs` for A\* if pathfinding > 5ms/turn. Batch AI utility evaluation across sects.
8. **Main Menu** scene: sect banner, "New Game", "Load Game", "Settings", "Exit". Settings: resolution, quality (URP quality levels), audio volume, difficulty default.

