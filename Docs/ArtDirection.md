# Wuxia Architecture & Art Direction Research
## Tales of the Tao — Visual Asset Guide

### 1. Art Direction Summary

**Target style:** Chinese ink-wash painting (水墨画) translated to 3D — stylized realism, NOT photorealistic. Think Civ 6's approach: clean silhouettes, hand-painted textures, bold color choices with limited palettes per area. This achieves ~80% AAA quality with ~20% effort and ages better than photorealism.

### 2. Historical Chinese Architecture Reference

#### 2.1 Core Architectural Elements (ALL building models need these)

**Roof (屋顶) — The most important visual identifier:**
- **Hip roof (庑殿顶):** 4 slopes + ridge — used for T3 grand buildings
- **Gable-and-hip (歇山顶):** 9-ridge roof — T2 buildings
- **Overhanging gable (悬山顶):** 2 slopes, eaves extend past walls — T1 humble buildings
- **Curved eaves (飞檐):** Upward sweep at corners — ESSENTIAL for wuxia feel. Even T1 buildings should have subtle curves
- **Ridge ornaments (脊兽):** Small animal figures on roof ridges — T2+ buildings get 3-5 ornaments, T3 gets 7+

**Platform/Base (台基):**
- Stone platform, minimum 1-2 steps for T1
- T2: Raised platform with carved balustrades
- T3: Multi-tiered white marble platform (须弥座 style)

**Columns & Beams (柱梁):**
- Red lacquer columns for all tiers
- T1: Simple wooden columns, no decoration
- T2: Carved bases, painted brackets (斗拱)
- T3: Full bracket sets, gold accents

**Walls & Enclosure:**
- T1: Wooden walls or bamboo fencing
- T2: Stone lower walls, wooden upper
- T3: Full stone walls with decorative gates (山门)

**Courtyards (院落):**
- T1: Single open area
- T2: Central courtyard with path
- T3: Multiple courtyards in axial arrangement (中轴线)

#### 2.2 Building Type Visual Identifiers

Each building type needs a UNIQUE silhouette so players can identify it at a glance:

| Building | Key Visual | Roof Style | Special Elements |
|----------|-----------|------------|------------------|
| **Temple** | Pagoda form | Multi-tiered hip | Incense burner, prayer flags |
| **Training Grounds** | Open pavilion + yard | Simple gable | Training dummies, weapon racks |
| **Disciple Hall** | Long dormitory | Overhanging gable | Rows of doors, laundry lines |
| **Library** | Tower form | Hip roof | Scroll cases visible, reading desks |
| **Elder Council** | Grand hall | Gable-and-hip | Throne-like seats, incense |
| **External Affairs Hall** | Gatehouse + reception | Gable-and-hip | Guest reception area, tea service |
| **Medicine Hall** | Workshop + garden | Simple gable | Herb drying racks, mortar/pestle |
| **Armory** | Forge + storage | Simple gable (smoke vent) | Anvil, forge chimney, weapon racks |
| **Market Pavilion** | Open-air stalls | Minimal roof | Stalls, awnings, coin counter |
| **Branch Sect Outpost** | Walled compound | Mixed | Watchtower, gate |
| **Dao Sanctum** (wonder) | Sacred structure | Multi-tiered | Glowing elements, qi particles |

#### 2.3 Tier Progression Visual Language

**T1 — Humble (简陋):**
- Wooden construction, unpainted or natural wood stain
- Simple roofs, no ornaments
- Single story, small footprint
- Bamboo or rope fencing
- ~200-400 polys

**T2 — Established (建成):**
- Stone foundation, red-painted wood above
- Curved eaves with small ridge ornaments
- Two stories or expanded single story
- Courtyard with stone paths
- Decorative gate
- ~400-600 polys

**T3 — Grand (宏伟):**
- White marble platform base
- Full red lacquer columns with gold accents
- Multi-tiered roofs with full ornament sets
- Multiple courtyards or large hall
- Guardian statues at entrance
- ~600-800 polys

### 3. Cultural & Historical Reference by Sect

Each of the 10 sects should have subtle architectural variations:

| Sect | Origin Region | Architectural Influence | Color Palette |
|------|--------------|------------------------|---------------|
| **Wu Dang** | Hubei (Wudang Mountains) | Taoist temple style, curved roofs, yin-yang motifs | Azure + White |
| **Shaolin** | Henan (Song Mountain) | Buddhist monastery, pagoda forms, martial austerity | Saffron + Brown |
| **Tang Clan** | Sichuan | Fortress-like, hidden passages, poison garden | Black + Green |
| **Mount Hua** | Shaanxi (Hua Mountain) | Cliff-integrated, narrow bridges, sword motifs | Silver + Blue |
| **Emei** | Sichuan (Emei Mountain) | Buddhist-Taoist blend, lotus motifs, elegant | Pink + Gold |
| **Kunlun** | Xinjiang/Tibet | High-altitude, ice-themed, fortress monastery | Ice Blue + White |
| **Peng Clan** | Jiangnan (water towns) | Open, airy, lightweight structures, bridge networks | Teal + Grey |
| **Namgung** | Korean-influenced | Palace-style, formal symmetry, noble aesthetics | Purple + Gold |
| **Demonic Cult** | Remote/wilderness | Twisted forms, dark materials, blood motifs | Crimson + Black |
| **Imperial Palace** | Northern Chinese | Imperial palace style, yellow accents, grand scale | Imperial Yellow + Red |

### 4. Unit Visual Design

#### 4.1 Disciple Rank Silhouettes

Units are stylized figurines (NOT photorealistic). Key: each rank must be instantly recognizable by silhouette.

| Rank | Silhouette | Robe Color | Key Accessory |
|------|-----------|------------|---------------|
| **Peon** | Hunched, carrying tool | Undyed linen (grey/brown) | Basket, hoe, or broom |
| **Outer Disciple** | Upright, basic stance | Sect primary color (light) | Simple belt, no weapon visible |
| **Inner Disciple** | Confident stance | Sect primary color (full) | Weapon on back or hip |
| **Elder** | Commanding presence | Sect primary + secondary trim | Hall-specific accessory (scroll, pill gourd, sword) |
| **High Elder** | Aura of power | Sect colors + gold trim | Floating/crowning element, glowing hands |
| **Grand Patriarch** | Transcendent | White + sect colors | Full emission glow, floating slightly |

#### 4.2 Unit Model Specifications

- **Poly budget:** LOD0 = ~800 polys, LOD1 = ~200 polys, LOD2 = billboard
- **Material:** Single material per unit, GPU instanced
- **Sect color:** Tint via material property block (not unique materials)
- **Height variation:** Peon=1.6u, Outer=1.7u, Inner=1.8u, Elder=1.9u, High Elder=2.1u

### 5. Terrain & Environment Art Notes

- **Sacred Peaks:** Snow-capped, qi particle streams (upward flowing)
- **Mountains:** Rocky with cave entrances, pine trees
- **Forests:** Bamboo groves (not European oaks), mist particles
- **Plains:** Grassland with wildflowers, gentle hills
- **Rivers/Water:** Stylized blue-white, not photorealistic
- **Swamps:** Dark mud, dead trees, fog particles

### 6. Common Pitfalls to Avoid (from previous models)

1. **Gaps between base and building** — Building base must be a solid platform that sits flush with the ground plane. No floating buildings.
2. **No backfaces on roofs** — Roofs must be solid volumes, not single-plane surfaces
3. **Consistent scale** — All buildings must use the same unit scale (1 unit = 1 meter in Unity)
4. **Manifold geometry** — No non-manifold edges, no inverted normals, no internal faces
5. **Pivot point** — All building pivots at the base center (0,0,0) for consistent placement
6. **LOD naming** — LOD0 = full name, LOD1 = `_LOD1` suffix, LOD2 = `_LOD2` suffix
7. **Export as OBJ** — Consistent with existing pipeline, Y-up, meters

### 7. Reference Images to Study

Search for these reference terms when modeling:
- "Chinese temple architecture curved eaves"
- "Wudang Mountain temple complex"
- "Song Dynasty building reconstruction"
- "Chinese pagoda structure"
- "Traditional Chinese courtyard layout"
- "Chinese martial arts movie set design"
- "Jianghu architecture concept art"
- "Chinese ink painting architecture"

### 8. Production Priority for M5

**Must have for M5 verify condition:**
1. Training Grounds model (T1) — needed for "build Training Grounds" verify
2. Temple model (T1) — already exists but needs quality pass
3. Outer Disciple unit model — needed for "Outer Disciple spawns" verify
4. Peon unit model — needed for "recruit 3 peons" verify

**Should have for M5 completeness:**
5. All 5 existing building models — quality pass (fix gaps, add details)
6. Disciple Hall, Library, Elder Council, External Affairs Hall, Medicine Hall, Armory, Market Pavilion models (T1)
7. Inner Disciple, Elder unit models

**Nice to have:**
8. T2/T3 variants of all buildings
9. T3 Core Disciple, T4 Sect Master, T5 Grand Patriarch models
10. Sect-specific architectural variants
