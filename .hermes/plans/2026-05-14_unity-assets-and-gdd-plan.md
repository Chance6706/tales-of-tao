# Tales of the Tao — Unity Asset Audit & GDD Phase Plan

**Date:** 2026-05-14
**Scope:** Full repo audit → asset inventory → gap analysis → prioritized production plan
**References:** GDD v2.0 (§1–§19), DevelopmentPlan v2 (M0–M14), TechnicalNotes (§18)

---

## 1. EXISTING ASSET INVENTORY

### 1.1 What Already Exists (Placeholder/Basic)

| Category | Asset | Format | Status |
|----------|-------|--------|--------|
| **Hex Tiles** | Hex_Base_Flat.obj | OBJ | Placeholder — single flat hex, no terrain variants |
| **Terrain Features** | Feature_Forest_Tree.obj | OBJ | Placeholder tree |
| | Feature_Mountain.obj | OBJ | Placeholder mountain |
| | Feature_Sect_Gate.obj | OBJ | Placeholder gate |
| | Feature_Water_Stream.obj | OBJ | Placeholder water |
| **Buildings (5 of 11)** | Building_Temple.obj | OBJ | Placeholder |
| | Building_Training_Grounds.obj | OBJ | Placeholder |
| | Building_Library.obj | OBJ | Placeholder |
| | Building_Medicine_Hall.obj | OBJ | Placeholder |
| | Building_Armory.obj | OBJ | Placeholder |
| **Units (5 of 8)** | T1_Labor_Disciple.obj | OBJ | Placeholder |
| | T2_Outer_Disciple.obj | OBJ | Placeholder |
| | T3_Core_Disciple.obj | OBJ | Placeholder |
| | T4_Sect_Master.obj | OBJ | Placeholder |
| | T5_Grand_Patriarch.obj | OBJ | Placeholder |
| **Materials** | HexTileVertexColor.mat | URP Material | Basic vertex color |
| **Shaders** | HexVertexColor.shader | URP Shader | Basic |
| | TalesOfTao_HexColorPerTile.shader | URP Shader | Per-tile coloring |
| | TalesOfTao_HexVertexColor.shader | URP Shader | Vertex color variant |
| **Prefabs** | 5 building prefabs | Unity Prefab | Linked to OBJs above |
| | 4 unit prefabs | Unity Prefab | Linked to OBJs above |
| **Data Assets** | 5 BuildingDataSO | SO | Temple, Training Grounds, Library, Medicine Hall, Armory |
| | 5 EventChannels | SO | Combat, Phase, Resource, Turn, +1 |
| **TerrainTypeSO** | (in Data/Terrain/) | SO | Exists per folder |

### 1.2 What's Missing — Full Asset Gap Analysis

#### A. HEX TILES & TERRAIN (GDD §4, §16, §17)

| Needed Asset | Count | GDD Ref | Notes |
|---|---|---|---|
| Hex tile meshes per terrain type | 8 | §4.1 | Plains, Mountain, Forest, River, Lake, Desert, Swamp, Sacred Peak — currently only 1 flat hex exists |
| Hex tile LOD variants (LOD0/1/2) | 8 × 3 = 24 | §17.3 | Full prism → simplified flat → billboard |
| Terrain texture atlas | 1 (2048²) | §17.3 | 8 terrain types × 256² tiles in one atlas |
| Terrain feature props | 12–16 | §4.1 | Trees (×3 variants), rocks, ruins, hot spring, spirit vein, bandit camp, wandering master spawn — currently 4 exist |
| Ley Line particle effect | 1 VFX | §4.3, §16.1 | URP VFX Graph — particle stream connecting Sacred Peaks |
| Zodiac ambient VFX | 12 | §16.1 | One particle emitter type per zodiac year (lanterns, gold rim-light, red vignette, mist, etc.) |
| Fog of War overlay material | 1 | §17.4 | Dark overlay for Hidden tiles, 50% brightness for Explored |
| Territory color overlay material | 1 | §17.4 | Per-sect colored territory borders |
| Road segment mesh | 1 | §9.2 | Peon-built road visual |
| Fortification meshes | 3 | §4.1 | Watchtower, Garrison, Fortress |
| Cave entrance meshes | 4 | §4.1 | Meditation, Body Tempering, Qi Refinement, Spirit Trial |
| Water mesh (River/Lake) | 2 | §4.1 | Animated water surface for River and Lake tiles |
| Elevation visual variants | 4 | §4.2 | Low, Medium, High, Summit — affects tile height/color |

**Subtotal: ~60 terrain/hex assets**

#### B. BUILDINGS (GDD §5, §6, §16)

11 building types × 3 tiers = 33 building visual states. Currently 5 types exist at T1 only.

| Building | T1 | T2 | T3 | Unique Sect Variants | GDD Ref |
|----------|----|----|----|----------------------|---------|
| Temple | ✅ placeholder | ❌ | ❌ | — | §6.3 |
| Training Grounds | ✅ placeholder | ❌ | ❌ | Wu Dang: Taiji Pavilion, Shaolin: Pagoda of 108 Trials | §6.3, §5.1 |
| Disciple Hall | ❌ | ❌ | ❌ | — | §6.3 |
| Library | ✅ placeholder | ❌ | ❌ | Hua Shan: Sword Peak | §6.3, §5.1 |
| Elder Council | ❌ | ❌ | ❌ | Namgung: Hall of Prestige, Imperial Palace: Imperial Court | §6.3, §5.1 |
| External Affairs Hall | ❌ | ❌ | ❌ | Emei: Lotus Hall, Peng Clan: Courier Network | §6.3, §5.1 |
| Medicine Hall | ✅ placeholder | ❌ | ❌ | Tang Clan: Poison Hall | §6.3, §5.1 |
| Armory | ✅ placeholder | ❌ | ❌ | — | §6.3 |
| Market Pavilion | ❌ | ❌ | ❌ | — | §6.3 |
| Branch Sect Outpost | ❌ | ❌ | ❌ | — | §6.3.1 |
| Dao Sanctum (wonder) | ❌ (T3 only) | — | — | — | §2, §6.3 |

**Unique sect building variants (6):**
- Taiji Pavilion (Wu Dang)
- Pagoda of 108 Trials (Shaolin)
- Sword Peak (Hua Shan)
- Lotus Hall (Emei)
- Poison Hall (Tang Clan)
- Frozen Meridian Chamber (Kunlun)
- Courier Network (Peng Clan)
- Hall of Prestige (Namgung)
- Blood Altar (Demonic Cult)
- Imperial Court (Imperial Palace)

**Subtotal: ~45 building assets (33 tier variants + 10 unique + 2 shared)**

#### C. UNITS (GDD §6, §9, §16)

| Unit Type | Existing | Needed Variants | GDD Ref |
|-----------|----------|-----------------|---------|
| Peon Gang | ✅ T1 placeholder | Sect-colored robe variant × 10 sects | §9.1 |
| Outer Patrol | ✅ T2 placeholder | Sect-colored robe variant × 10 sects | §9.1 |
| Inner Disciple Squad | ✅ T3 placeholder | Sect-colored robe variant × 10 sects | §9.1 |
| Elder Champion | ✅ T4 placeholder | Sect-colored + unique Elder visual × 10 sects | §9.1 |
| High Elder Vanguard | ✅ T5 placeholder | Sect-colored + unique High Elder visual × 10 sects | §9.1 |
| Support Caravan | ❌ | 1 base model + sect color | §9.1 |
| Settler Party | ❌ | 1 base model + sect color | §9.1 |
| Spy | ❌ | 1 base model (hooded/stealth visual) | §9.1, §11 |

**Unit equipment variants (per sect affinity):**
- Sword users (Hua Shan, Namgung): sword visible on model
- Fist/Body (Shaolin): bare-arms visual
- Hidden weapons (Tang Clan): subtle blade props
- Staff (Wu Dang): staff prop
- Ice/Elemental (Kunlun): frost aura VFX

**Subtotal: ~15 unit assets (8 base + 7 variants/equipment)**

#### D. CHARACTER ART (GDD §6.2, §16, §18.8)

| Asset | Count | Notes |
|-------|-------|-------|
| Disciple rank silhouettes | 5 | Peon, Outer, Inner, Elder, High Elder — distinct readable silhouettes |
| Sect emblem/banner icons | 10 | 64×64 icons per §18.8 color palette |
| Sect color palettes | 10 | Primary/Secondary/Accent per §18.8 table |
| Cultivation realm visual progression | 5 | Aura effects, glow intensity, robe quality per realm |
| Spirit Beast models | 5 | Phoenix, Dragon, Tortoise, Tiger, Serpent |
| Spirit Beast rarity variants | 5 × 3 = 15 | Common/Rare/Legendary visual tiers |
| Wandering Master NPC | 1 | Generic wandering cultivator |
| Bandit unit model | 1 | Generic bandit for events |

**Subtotal: ~50 character/creature assets**

#### E. VFX (GDD §9, §14, §16)

| VFX | Count | Notes |
|-----|-------|-------|
| Combat technique VFX | ~20 | One per technique tier per branch (sword slash, fist impact, Qi beam, ice crystal, poison cloud, etc.) |
| Combat impact/hit VFX | 5 | Generic hit, block, dodge, critical, death |
| Qi stream particles | 3 | Dense Qi, Ley Line, ambient cultivation |
| Cultivation aura VFX | 5 | Per cultivation realm (peaceful glow → reality distortion) |
| Heavenly Tribulation VFX | 1 | Lightning/storm effect for Elder→High Elder promotion |
| Building construction VFX | 1 | Dust/hammer particles during build |
| Building completion VFX | 1 | Completion fanfare particles |
| Secret Realm portal VFX | 1 | Glowing portal on Sacred Peak |
| Spirit Beast encounter VFX | 5 | One per beast elemental type |
| Zodiac year transition VFX | 12 | Per §16.1 ambient effects |
| Selection ring VFX | 1 | Unit selection highlight |
| Movement trail VFX | 1 | Unit movement path indicator |
| Territory capture VFX | 1 | Flash/border effect on tile claim |
| Face-Slap VFX | 1 | Dramatic effect for underdog victory |
| Plague/Famine VFX | 1 | Dark particles over affected tiles |

**Subtotal: ~60 VFX assets**

#### F. UI ART (GDD §15, §16)

| Asset | Count | Notes |
|-------|-------|-------|
| HUD icons (resources) | 6 | Tael, Qi, Renown, Face, Dissent, Turn |
| Zodiac animal icons | 12 | One per year, wuxia-styled |
| Phase indicator icons | 6 | Event, Income, Build, Research, Action, Resolution |
| Sect banner emblems | 10 | Per §18.8 — already counted above |
| Building icons | 11 | One per building type |
| Unit type icons | 8 | One per unit type |
| Commodity icons | 6 | Iron Ore, Jade, Lumber, Medicinal Herbs, Spirit Herbs, Tea Leaves |
| Tech tree node icons | ~35 | One per tech node |
| Tech tree branch backgrounds | 3 | Alchemy, Forge, Martial |
| Diplomacy relation icons | 6 | War, Rivalry, Neutral, NAP, Trade, Alliance |
| Trust tier icons | 5 | Hostile, Wary, Neutral, Friendly, Devoted |
| Button/frame assets | ~20 | Wuxia-themed UI frames, buttons, panels |
| Minimap assets | 5 | Territory colors, fog overlay, icons |
| Event illustration cards | 8–10 | Key events (Wandering Master, Tournament, Tribulation, etc.) |
| Victory/defeat screen art | 3 | Domination, Enlightenment, Influence + generic defeat |
| Tutorial overlay assets | 10 | Callout boxes, highlight rings, arrow indicators |
| Font assets | 2–3 | Chinese-inspired display font, body font, monospace for numbers |

**Subtotal: ~150 UI assets**

#### G. AUDIO (GDD §16.2)

| Category | Count | Notes |
|----------|-------|-------|
| Music layers (base) | 4 | Exploration, tension, endgame, menu |
| Music layers (combat) | 3 | Light battle, heavy battle, boss/tribulation |
| Music layers (research) | 1 | Contemplative dizi/xiao layer |
| Zodiac transition stings | 12 | One per zodiac year |
| Victory stinger | 1 | Orchestral swell |
| UI SFX | 6 | Click, hover, panel open/close, error, confirmation |
| Combat SFX | 5 | Sword clash, staff impact, Qi discharge, death, rout |
| Building SFX | 2 | Construction complete, upgrade fanfare |
| Disciple SFX | 3 | Footstep loop, selection bark, movement |
| Marketplace SFX | 3 | Coin clink, price rise, price fall |
| Event SFX | 5 | Event arrival, tournament, plague, tribulation, Ley Line surge |
| Ambient loops | 5 | Wind, river, forest, mountain, swamp |
| Narrator VO | 3 | Sect founding, milestone, game over |

**Subtotal: ~50 audio assets**

#### H. SHADERS & MATERIALS (GDD §17, §18)

| Shader/Material | Count | Notes |
|-----------------|-------|-------|
| Hex tile shader (terrain) | 1 | Currently exists — needs atlas support |
| Hex tile shader (Fog of War) | 1 | Dark overlay + explored dimming |
| Hex tile shader (territory) | 1 | Per-sect color overlay |
| Building material (instanced) | 1 | With sect color property block |
| Unit material (instanced) | 1 | With sect color + equipment variants |
| Outline shader (ink-wash style) | 1 | Custom URP outline pass for stylized look |
| Water shader | 1 | Animated river/lake surface |
| Particle shader (Qi) | 1 | Additive glow for cultivation effects |
| LOD cross-fade shader | 1 | For smooth LOD transitions |

**Subtotal: ~9 shader/material assets**

---

## 2. TOTAL ASSET COUNT SUMMARY

| Category | Count |
|----------|-------|
| Hex Tiles & Terrain | ~60 |
| Buildings (all tiers + unique) | ~45 |
| Units & Characters | ~15 base + ~50 character/creature |
| VFX | ~60 |
| UI Art | ~150 |
| Audio | ~50 |
| Shaders & Materials | ~9 |
| **TOTAL** | **~380 assets** |

**Already exist:** ~30 placeholder assets (OBJs, basic materials, shaders, data assets)
**Need to create:** ~350 assets

---

## 3. GDD PHASE-TO-ASSET MAPPING

### M2 — Hex Grid (NEAR COMPLETE — needs art pipeline)
**Assets needed:**
- 8 terrain hex meshes (LOD0/1/2)
- Terrain texture atlas (2048²)
- 12–16 terrain feature props
- Fog of War + territory overlay materials
- Road segment mesh
- 4 cave entrance meshes
- 2 water meshes (River, Lake)
- Hex tile shader updates (atlas support)
- 4 fortification meshes

**GDD refs:** §4.1–4.3, §16.1, §17.3–17.4, §18.6
**Est. art effort:** 12 person-days (from DevPlan)

### M3 — Turn System
**Assets needed:**
- 12 zodiac animal icons
- 6 phase indicator icons
- HUD frame/button assets (wuxia-themed)
- 6 resource icons (Tael, Qi, Renown, Face, Dissent, Turn)
- Zodiac transition sting audio (12 stings)

**GDD refs:** §3.1, §15.2, §16.1–16.2
**Est. art effort:** 2 person-days

### M4 — Sect Founding
**Assets needed:**
- 10 sect banner emblems (64×64)
- 10 sect color palette definitions (ScriptableObjects)
- Temple T2/T3 visual upgrade
- Temple completion VFX
- Narrator VO (sect founding)

**GDD refs:** §5.1, §6.1, §16.1, §18.8
**Est. art effort:** 2 person-days

### M5 — Disciples & Buildings
**Assets needed:**
- 6 remaining building type meshes (T1): Disciple Hall, Elder Council, External Affairs Hall, Medicine Hall (T2/T3), Armory (T2/T3), Market Pavilion, Branch Sect Outpost
- 5 building tier upgrade visuals (T2/T3 for existing 5)
- 3 disciple rank models (refined: Outer, Inner, Elder — T1/T2/T3 placeholders already exist)
- Building construction VFX
- Building completion VFX
- Building construction/upgrade SFX

**GDD refs:** §6.2–6.3, §16.1–16.2
**Est. art effort:** 5 person-days

### M6 — Units & Movement + DOTS Spike
**Assets needed:**
- Unit selection ring VFX
- Unit movement trail VFX
- Support Caravan model
- Settler Party model
- Spy model (hooded)
- Unit LOD variants (LOD0/1/2 per unit type)

**GDD refs:** §9.1–9.2, §17.3
**Est. art effort:** 3 person-days

### M7 — Combat
**Assets needed:**
- Combat VFX (sword slash, fist impact, Qi beam, ice crystal, poison cloud, hit, block, dodge, critical, death)
- Combat SFX (sword clash, staff impact, Qi discharge, death, rout)
- Combat music layer (3 variants)
- Face-Slap VFX
- Combat result UI panel art

**GDD refs:** §9.3–9.4, §16.1–16.2
**Est. art effort:** 4 person-days

### M8 — Economy & Market
**Assets needed:**
- 6 commodity icons
- Market screen UI art
- Marketplace ambient SFX
- Coin transaction SFX
- Price change SFX

**GDD refs:** §7.1–7.5, §15.3
**Est. art effort:** 2 person-days

### M9 — Research
**Assets needed:**
- ~35 tech node icons
- 3 tech tree branch backgrounds
- Tech tree connection line art
- Research progress ring UI
- Research completion VFX + SFX
- Research music layer

**GDD refs:** §8.1–8.3, §15.3, §16.1–16.2
**Est. art effort:** 4 person-days

### M10 — Diplomacy & Espionage
**Assets needed:**
- 6 diplomacy relation icons
- 5 trust tier icons
- Diplomacy screen UI art
- Settlement portrait placeholders (5 types)
- Espionage UI icons
- Event arrival SFX

**GDD refs:** §10.1–10.2, §11, §15.3, §16.2
**Est. art effort:** 3 person-days

### M11 — Tactical Combat
**Assets needed:**
- 7×7 tactical hex grid tiles
- Tactical terrain variants (elevated, cliff, dense forest, sparse)
- Tactical combat camera setup
- Duel VFX
- Morale break VFX
- Retreat VFX

**GDD refs:** §13, §16.1
**Est. art effort:** 3 person-days

### M12 — AI & Narrative
**Assets needed:**
- 8–10 event illustration cards (Wandering Master, Tournament, Tribulation, Ancient Tomb, Spirit Beast, Plague, Defector, Ley Line Surge)
- Wandering Master NPC model
- Bandit unit model
- Narrative template data assets (NarrativeTemplateSO)

**GDD refs:** §12.2, §14.1, §16.1
**Est. art effort:** 3 person-days

### M13 — Content & Balance
**Assets needed:**
- Remaining 4 sect banner emblems (completing all 10)
- 10 unique sect building variants (Taiji Pavilion, Pagoda, Sword Peak, Lotus Hall, Poison Hall, Frozen Meridian Chamber, Courier Network, Hall of Prestige, Blood Altar, Imperial Court)
- Spirit Beast models (5 types × 3 rarity tiers = 15)
- Victory screen art (3 victory types + defeat)
- Dao Sanctum wonder model
- Heavenly Tribulation VFX
- Secret Realm portal VFX

**GDD refs:** §2, §5.1, §6.3, §14.2–14.3, §16.1
**Est. art effort:** 8 person-days

### M14 — Polish, Save/Load, Performance
**Assets needed:**
- Replace all placeholder OBJs with final stylized models
- Ink-wash outline shader (URP custom pass)
- Water shader (animated)
- Particle shader (Qi glow)
- LOD cross-fade shader
- Final ambient loops (5 terrain types)
- Final music layers (base + combat + research)
- Tutorial overlay assets (10 callout/arrow/ring assets)
- Final narrator VO (milestone + game over)

**GDD refs:** §15, §16.1–16.2, §17.3, §18.6, §18.8
**Est. art effort:** 10 person-days

---

## 4. CRITICAL PATH FOR ASSET PRODUCTION

The asset production critical path follows the M2→M3→M4→M5→M6→M7 chain, since each milestone's gameplay depends on the previous milestone's art being in place for verification.

**Phase 1 — Core Visuals (M2-M4, ~16 art-days):**
Terrain → HUD → Sect Identity. This establishes the game's visual language.

**Phase 2 — Gameplay Content (M5-M7, ~12 art-days):**
Buildings → Units → Combat VFX. This makes the game playable and satisfying.

**Phase 3 — Systems Content (M8-M10, ~9 art-days):**
Economy UI → Research UI → Diplomacy UI. This completes the information layer.

**Phase 4 — Advanced Content (M11-M12, ~6 art-days):**
Tactical grid → Events/Narrative. This adds depth and story.

**Phase 5 — Full Content + Polish (M13-M14, ~18 art-days):**
Sect variants → Spirit Beasts → Shaders → Audio → Final pass. This ships the game.

**Total estimated art production: ~61 person-days** (overlaps with ~141 person-days of engineering)

---

## 5. ART PIPELINE RECOMMENDATIONS

### 5.1 Blender-to-Unity USD Pipeline (per GDD §17.8)

The GDD specifies USD as the export format. Current assets are OBJ — they need to be migrated.

**Recommended workflow:**
1. Author in Blender 4.x (metric, Y-up)
2. Export as `.usda` (ASCII USD for version control)
3. Import via `com.unity.formats.usd` package
4. USD variant sets → Unity LODGroup
5. USD Preview Surface → URP Lit shader remapping

**Naming convention (from §17.8):**
- `terrain/hex_{type}_LOD.usd`
- `buildings/bld_{name}_T{tier}.usd`
- `units/unit_{rank}[_{variant}].usd`
- `props/prop_{name}.usd`
- `fx/fx_{effect}.usd`

### 5.2 Material Batching Rules (per §17.8)

1. All terrain tiles of same type → 1 material instance (texture atlas)
2. All building instances of same type + tier → 1 material instance
3. Unit figurines → shared material + MaterialPropertyBlock for sect color
4. Max 4 unique materials per draw call batch group

### 5.3 LOD Strategy (per §17.3, §18.6)

| LOD | Distance | Tri Count | Use Case |
|-----|----------|-----------|----------|
| LOD0 | 0–20 units | 100% (800–5000 tris for heroes) | Close-up, tactical combat |
| LOD1 | 20–60 units | 50% | Standard gameplay |
| LOD2 | 60–120 units | 25% (or billboard) | Distant units on strategy map |
| Culled | >120 units | — | Not rendered |

### 5.4 Texture Budget (per §18.6)

| Asset Type | Resolution |
|------------|-----------|
| Hero characters | 2048×2048 |
| Standard units | 1024×1024 |
| Buildings | 1024–2048 |
| Hex tiles | 512–1024 (atlas) |
| Props | 512–1024 |
| UI elements | 256–512 |

---

## 6. RISK ASSETS (Highest Priority to Create First)

These assets are on the critical path for the most milestones and should be created first:

1. **8 terrain hex meshes** — blocks M2 verification, M6 movement, M7 combat terrain
2. **Terrain texture atlas** — blocks all terrain rendering
3. **10 sect banner emblems** — blocks M4 verification, M10 diplomacy, M13 content complete
4. **Sect color palette SOs** — blocks all sect-colored rendering (units, buildings, territory)
5. **Combat VFX (5 core)** — blocks M7 verification
6. **12 zodiac icons** — blocks M3 verification
7. **6 resource icons** — blocks M3 HUD
8. **Building meshes (6 missing types)** — blocks M5 verification
9. **Unit LOD variants** — blocks M6 DOTS spike evaluation
10. **Event illustration cards (8)** — blocks M12 narrative verification

---

## 7. DEFERRED ASSETS (Post-Launch / Kill List)

Per GDD §19 Kill List, these assets should NOT be created for launch:

| Deferred Feature | Assets Saved | Reason |
|-----------------|-------------|--------|
| Technique Fusion UI | ~5 UI assets | Replaced by "Grandmaster's Legacy" passive |
| Weapon-type RPS icons | 3 icons | Duel triangle removed |
| Grandmaster difficulty badge | 1 icon | Folded into Master+ |
| Cloud AI UI indicators | 2–3 UI assets | Architecture only, not implemented |
| Multiplayer lobby UI | ~10 UI assets | Deferred entirely |
| Autonomous sub-sect UI | ~5 UI assets | Branch Sect T3 is player-controlled only |

---

## 8. SUMMARY

- **~380 total assets** needed for a complete ship
- **~30 exist** as placeholders (OBJs, basic shaders, data assets)
- **~350 need creation** across meshes, textures, VFX, UI, audio, shaders
- **~61 person-days** of art production (can parallelize with engineering)
- **Critical first assets:** terrain meshes, texture atlas, sect emblems, combat VFX, zodiac icons
- **Pipeline:** Blender → USD → Unity USD Plugin → URP materials with instancing
- **Style:** Stylized realism / Chinese ink-wash (§18.8) — hand-painted textures, clean silhouettes, bold sect color palettes
