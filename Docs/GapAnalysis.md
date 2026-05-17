# Tales of Tao — Gap Analysis: Codebase vs. Vision vs. Genre Benchmarks

## Methodology
- Audited 85 C# scripts across 7 system categories
- Audited 120 art assets (meshes, materials, prefabs) across 5 categories
- Compared implemented systems against GDD §1-§18 vision
- Benchmarked against: Civ VI, Stellaris, Total War: Three Kingdoms, Age of Wonders 4, Northgard, Old World, Jade Dynasty
- Identified gaps from player experience, code architecture, and visual/art perspectives

---

## CATEGORY 1: CORE GAMEPLAY LOOP

### GDD Vision
Found Sect → Recruit Peons → Train Disciples → Build Halls → Research Techniques → Expand → Diplomacy → Combat → Victory

### What's Implemented
- Sect founding (FoundSectCommand) ✅
- Peon recruitment (RecruitPeonCommand) ✅
- Building construction (BuildQueue) ✅
- Training queue (TrainingQueue) ✅
- Disciple promotion (DiscipleData.TryPromote) ✅
- Management ratio + Dissent ✅
- Income/upkeep calculation ✅

### GAPS (Player Perspective)

| Gap | Severity | Description | Benchmark Comparison |
|-----|----------|-------------|---------------------|
| **No Founder unit** | HIGH | GDD §13.1: "Turn 1: Place Founder, move toward ideal founding location." Current: sect just appears on a tile. No unit to move, no consumption on founding. | Civ VI: Settler unit is the most important early-game unit. Total War: Lord unit is the army commander. Both are tangible, movable, consumable. |
| **No unit movement** | HIGH | GDD §9.2: Base movement = 3 hexes/turn, modified by terrain, rank, techniques. Current: units spawn at mouse position, no movement system. | Civ VI: Unit movement is the core of the Action phase. AoE4: Unit stances, formations, terrain modifiers. |
| **No combat system** | HIGH | GDD §9.3: Auto-resolve + tactical view with duels, morale, terrain. Current: nothing. | Civ VI: Simple but satisfying auto-resolve with unit cards. AoE4: Tactical battles with formations. Total War: Real-time tactical battles. |
| **No research system** | HIGH | GDD §8: Three tech branches (Alchemy/Forge/Martial), 35+ nodes, hall tier gating. Current: ResearchState is a placeholder that does nothing. | Civ VI: Tech tree with era progression. Stellaris: Research cards with weights. Both create meaningful choices. |
| **No diplomacy** | HIGH | GDD §10: Sect-to-sect relations, trust, trade agreements, alliances, war declarations. Current: nothing. | Civ VI: Diplomacy screen with deals, grievances, alliances. Stellaris: Federations, rivalries, galactic community. |
| **No settlements** | HIGH | GDD §10.2: Independent settlements (Village/Town/Trade Post) with trust tiers. Current: nothing. | Civ VI: City-states with envoys. AoE4: Trade posts, relics. Both create map objectives. |
| **No events system** | HIGH | GDD §14: 9 event types (Wandering Master, Bandit Uprising, Ancient Tomb, etc.). Current: EventState is a placeholder. | Civ VI: Random events with choices. Stellaris: Anomaly chains, crisis events. Both create narrative moments. |
| **No victory conditions** | HIGH | GDD §2: Domination, Enlightenment, Influence victories. Current: nothing. | All benchmarks: Victory conditions drive player motivation and end-game urgency. |
| **No save/load** | HIGH | GDD §17.5: JSON-based save files, autosave every 5 turns. Current: nothing. | All benchmarks: Essential for any strategy game. |
| **No tutorial** | HIGH | GDD §18: Tutorial overlays for first-run key actions. Current: nothing. | Civ VI: Excellent contextual tutorials. Stellaris: Improved but still complex. Both reduce onboarding friction. |

### GAPS (Code Perspective)

| Gap | Severity | Description |
|-----|----------|-------------|
| **No CombatResolver** | HIGH | GDD §9.3 specifies auto-resolve formula. No code exists. |
| **No ResearchManager** | HIGH | GDD §8 specifies research speed formula, tech nodes. No code exists. |
| **No DiplomacyManager** | HIGH | GDD §10 specifies trust, relations, agreements. No code exists. |
| **No EventScheduler** | HIGH | GDD §14 specifies event triggers, probability. No code exists. |
| **No VictoryChecker** | HIGH | GDD §2 specifies 3 victory conditions. No code exists. |
| **No SaveLoadManager** | HIGH | GDD §17.5 specifies JSON serialization. No code exists. |
| **BuildingFactory is thin** | MEDIUM | Creates GOs but no upgrade system, no visual state changes. |
| **No unit stances/formations** | MEDIUM | GDD §9 mentions unit stances. No code exists. |
| **No morale system** | MEDIUM | GDD §9.3 mentions morale affecting combat. No code exists. |
| **No technique assignment** | MEDIUM | GDD §6.5 mentions weapon/combat techniques. No code exists. |

---

## CATEGORY 2: MULTIPLAYER

### What's Implemented
- TurnCoordinator (server authority) ✅
- ReadySystem (phase sync) ✅
- NetworkCommandQueue (command ordering) ✅
- PlayerSlot (player registration) ✅
- TurnDriver multiplayer mode ✅
- MultiplayerTurnTestHUD ✅

### GAPS (Player Perspective)

| Gap | Severity | Description | Benchmark Comparison |
|-----|----------|-------------|---------------------|
| **No network transport** | HIGH | All multiplayer is local-only. No actual networking. | Civ VI: Steam P2P with relay servers. Stellaris: Paradox P2P with host migration. |
| **No combat between players** | HIGH | Simultaneous moves can result in conflicts, but no resolution system. | Civ VI: "Simultaneous except during war" mode. AOW4: Simultaneous with manual battle option. |
| **No diplomacy between players** | HIGH | No way for players to make deals, trade, or form alliances. | Civ VI: Extensive diplomacy screen. Stellaris: Federations, trade deals. |
| **No host migration** | HIGH | If host drops, game is lost. | Civ VI: Host migration support. Stellaris: Host migration with reconnection. |
| **No async/PBEM option** | HIGH | All players must be online simultaneously. | Civ VI: Play-by-Cloud. Old World: Async multiplayer. Both allow flexible scheduling. |
| **No spectator mode** | MEDIUM | Can't watch ongoing games. | Civ VI: Spectator mode for multiplayer. |

### GAPS (Code Perspective)

| Gap | Severity | Description |
|-----|----------|-------------|
| **No network serialization** | HIGH | Commands need to be serialized for network transport. |
| **No client-side prediction** | HIGH | Players will experience latency without prediction. |
| **No desync detection** | HIGH | Simultaneous MP requires deterministic execution and desync detection. |
| **No reconnection handling** | MEDIUM | Players who drop should be able to reconnect. |
| **No AI takeover on disconnect** | MEDIUM | Dropped human players should have AI take over. |

---

## CATEGORY 3: ECONOMY & RESOURCES

### GDD Vision
Dual resource system: Tael (currency) + Qi (cultivation). Commodities: Lumber, Iron Ore, Jade, Medicinal Herbs, Spirit Herbs, Tea Leaves, Horses. Market with price fluctuation. Trade routes. Upkeep costs.

### What's Implemented
- ResourceStockpile struct (Tael, Qi, Lumber, IronOre, Jade, MedicinalHerbs, SpiritHerbs, TeaLeaves) ✅
- Income calculation with terrain bonuses ✅
- Upkeep calculation per disciple rank ✅
- BuildingConfigSO costs ✅
- Peon recruitment cost (10 Tael) ✅

### GAPS (Player Perspective)

| Gap | Severity | Description | Benchmark Comparison |
|-----|----------|-------------|---------------------|
| **No market system** | HIGH | GDD §12: Market with buy/sell, price fluctuation, Face modifier. Current: nothing. | Civ VI: Market for buying/selling units, districts. Stellaris: Galactic market with price fluctuation. |
| **No trade routes** | HIGH | GDD §10: Trade routes between sect and settlements. Current: nothing. | Civ VI: Trade routes are a core income source. AoE4: Trade routes with gold generation. |
| **No commodity gathering** | HIGH | GDD §7: Resource nodes on map (Iron Ore, Jade, Herbs, etc.). Current: map has terrain but no resource deposits. | Civ VI: Strategic/luxury resources on map. Stellaris: Mining/research stations. |
| **No Face system** | MEDIUM | GDD §10.4: Face (0-100) affects market prices, settlement disposition. Current: nothing. | TW3K: Renown/Face system affects diplomacy. Creates role-playing tension. |
| **No Renown system** | MEDIUM | GDD §10.3: Global reputation score for Influence victory. Current: nothing. | Civ VI: Fame/Score. TW3K: Renown. Both drive victory progress. |
| **No bankruptcy handler** | MEDIUM | GDD §12: Bankruptcy causes peon desertion, build queue pause, dissent. Current: nothing. | Stellaris: Energy credit bankruptcy causes severe penalties. Creates economic tension. |

### GAPS (Code Perspective)

| Gap | Severity | Description |
|-----|----------|-------------|
| **No MarketSimulator** | HIGH | GDD §12 specifies buy/sell/drift mechanics. |
| **No TradeRoute class** | HIGH | GDD §10 specifies distance-scaled income. |
| **No resource node system** | HIGH | GDD §7 specifies deposits on tiles. |
| **No Face/Renown tracking** | MEDIUM | GDD §10.3-10.4 specify sources and effects. |

---

## CATEGORY 4: DISCIPLES & PROGRESSION

### GDD Vision
6 ranks (Peon → Outer → Inner → Elder → High Elder → Sect Leader). 1:10 management ratio. Promotion costs in Tael/Qi/commodities. Technique assignment. Elder specialization. Spirit Beast bonding. Heavenly Tribulation.

### What's Implemented
- 6 disciple ranks ✅
- Promotion costs ✅
- Management ratio enforcement ✅
- Dissent from ratio violations ✅
- DiscipleData with stats ✅
- Training queue ✅
- Build queue ✅

### GAPS (Player Perspective)

| Gap | Severity | Description | Benchmark Comparison |
|-----|----------|-------------|---------------------|
| **No technique assignment** | HIGH | GDD §6.5: Weapon + Combat techniques auto-assigned on promotion. Current: nothing. | TW3K: Skill trees for generals. AOW4: Tomes of magic. Both create build diversity. |
| **No Elder specialization** | HIGH | GDD §6.4: Elders assigned to specific Halls for bonuses. Current: nothing. | TW3K: Minister appointments. Stellaris: Leader specialization. Both create strategic depth. |
| **No Spirit Beast bonding** | HIGH | GDD §14.1: Spirit Beasts bond to disciples for stat bonuses. Current: nothing. | TW3K: Mount system. AOW4: Mounts and familiars. Both add character progression. |
| **No Heavenly Tribulation** | HIGH | GDD §14: Elder→High Elder promotion has RNG trial (injury/death). Current: nothing. | Creates dramatic tension. TW3K: Character death events. |
| **No Sect Leader mechanics** | MEDIUM | GDD §6.3: Sect Leader provides passive bonus, can be replaced via Elder Council vote. Current: nothing. | TW3K: Faction leader mechanics. Creates succession drama. |
| **No disciple naming/personality** | MEDIUM | GDD §6: Disciples have names, traits (Lucky, Resilient, etc.). Current: GenerateName() exists but traits are cosmetic only. | TW3K: Character personalities affect gameplay. Stellaris: Leader traits matter. |

### GAPS (Code Perspective)

| Gap | Severity | Description |
|-----|----------|-------------|
| **No TechniqueSystem** | HIGH | GDD §6.5 specifies weapon/combat techniques. |
| **No ElderSpecialization** | HIGH | GDD §6.4 specifies Hall assignments. |
| **No SpiritBeastSystem** | HIGH | GDD §14.1 specifies bonding mechanics. |
| **No TribulationSystem** | MEDIUM | GDD §14 specifies promotion trial. |

---

## CATEGORY 5: MAP & WORLD

### GDD Vision
Hex map with 8 terrain types, elevation, Qi density, caves, deposits, features. 3 preset maps. Fog of war. Zone of control. Roads. Fortifications.

### What's Implemented
- Hex grid with coordinates ✅
- 8 terrain types ✅
- Map generation with presets ✅
- Fog of war system ✅
- Pathfinding (A*) ✅
- Tile selection/highlighting ✅
- Reachable tile overlay ✅

### GAPS (Player Perspective)

| Gap | Severity | Description | Benchmark Comparison |
|-----|----------|-------------|---------------------|
| **No resource deposits on map** | HIGH | GDD §4.1: Deposits (Iron Ore, Jade, Herbs, etc.) on tiles. Current: terrain only, no resources. | Civ VI: Strategic/luxury resources drive expansion. Stellaris: Mining stations. |
| **No map features** | HIGH | GDD §4.1: Ancient Ruins, Hot Spring, Spirit Vein, Bandit Camp, Wandering Master. Current: nothing. | Civ VI: Goody huts, natural wonders. Both create exploration rewards. |
| **No roads** | MEDIUM | GDD §9.2: Roads reduce movement cost to 0.5. Current: nothing. | Civ VI: Roads built by builders. AoE4: Trade routes create roads. |
| **No fortifications** | MEDIUM | GDD §4.1: Watchtower/Garrison/Fortress built by sects. Current: nothing. | Civ VI: Encampments, city walls. Both create defensive strategy. |
| **No zone of control** | MEDIUM | GDD §9.2: ZOC costs +2 movement. Current: nothing. | Civ VI: ZOC from military units. Creates tactical positioning. |
| **No settlement entities** | HIGH | GDD §10.2: Independent settlements on map. Current: nothing. | Civ VI: City-states. AoE4: Trade posts. Both create map objectives. |

### GAPS (Code Perspective)

| Gap | Severity | Description |
|-----|----------|-------------|
| **No resource deposit system** | HIGH | GDD §4.1 specifies deposits per tile. |
| **No map feature system** | HIGH | GDD §4.1 specifies features. |
| **No road building** | MEDIUM | GDD §9.2 specifies road construction by peons. |
| **No fortification building** | MEDIUM | GDD §4.1 specifies fortification types. |

---

## CATEGORY 6: UI/UX

### GDD Vision
Sect overview screen, research tree, diplomacy screen, market screen, tactical battle view, event modals, tutorial overlays, HUD with phase/turn/resources.

### What's Implemented
- TurnTestHUD (phase/turn/resources) ✅
- SectOverviewScreen (exists but minimal) ✅
- MultiplayerTurnTestHUD ✅
- TileInfoPanel ✅

### GAPS (Player Perspective)

| Gap | Severity | Description | Benchmark Comparison |
|-----|----------|-------------|---------------------|
| **No research tree UI** | HIGH | GDD §8: Three scrollable node-graph panels. Current: nothing. | Civ VI: Excellent tech tree UI. Stellaris: Research card picker. Both make choices clear. |
| **No diplomacy screen** | HIGH | GDD §10: Diplomacy screen with trust, deals, actions. Current: nothing. | Civ VI: Comprehensive diplomacy screen. Essential for MP. |
| **No market screen** | HIGH | GDD §12: Commodity list with buy/sell prices. Current: nothing. | Civ VI: Market for unit purchases. Stellaris: Galactic market. |
| **No event modal** | HIGH | GDD §14: Event modal with flavor text, choices, effects. Current: nothing. | Civ VI: Event pop-ups with choices. Creates narrative moments. |
| **No tactical battle view** | HIGH | GDD §9.3: 7×7 hex arena with unit placement. Current: nothing. | AoE4: Tactical battles are core to the experience. Total War: Real-time battles. |
| **No minimap** | MEDIUM | Standard for strategy games. Current: nothing. | Civ VI: Minimap with resource indicators. Essential for navigation. |
| **No tooltip system** | MEDIUM | GDD §15: Contextual tooltips for all UI. Current: nothing. | Civ VI: Excellent tooltips. Reduces cognitive load. |

### GAPS (Code Perspective)

| Gap | Severity | Description |
|-----|----------|-------------|
| **No ResearchScreen** | HIGH | GDD §8 specifies three scrollable panels. |
| **No DiplomacyScreen** | HIGH | GDD §10 specifies trust tiers, actions. |
| **No MarketScreen** | HIGH | GDD §12 specifies commodity trading. |
| **No EventModal** | HIGH | GDD §14 specifies event display. |
| **No TacticalBattle view** | HIGH | GDD §9.3 specifies 7×7 arena. |
| **No minimap** | MEDIUM | Standard strategy game feature. |

---

## CATEGORY 7: AI

### GDD Vision
Utility-based AI with 4 decision layers (Strategic/Tactical/Diplomatic/Combat). 6 personality types. Difficulty scaling. No cheating except Heavenly Dao.

### What's Implemented
- AIController stub (exists but empty) ⚠️

### GAPS (Player Perspective)

| Gap | Severity | Description | Benchmark Comparison |
|-----|----------|-------------|---------------------|
| **No AI decision making** | HIGH | AIController.ExecuteTurn() is empty. | Civ VI: Sophisticated AI with agendas. Stellaris: AI with personality weights. Both create believable opponents. |
| **No AI personalities** | HIGH | GDD §12: 6 personality types (Expansionist, Militant, Scholar, Diplomat, Opportunist, Zealot). | Civ VI: Leader agendas create distinct personalities. Stellaris: AI personalities affect diplomacy. |
| **No difficulty scaling** | HIGH | GDD §12.3: 5 difficulty levels with income/research/combat modifiers. | Civ VI: Difficulty levels with bonuses/penalties. Essential for accessibility. |

### GAPS (Code Perspective)

| Gap | Severity | Description |
|-----|----------|-------------|
| **No AIPersonality interface** | HIGH | GDD §12 specifies personality weights. |
| **No AIController implementation** | HIGH | Stub only. |
| **No difficulty scaling** | HIGH | GDD §12.3 specifies modifiers. |

---

## SUMMARY: TOP 15 CRITICAL GAPS (by priority)

### Tier 1: Core Gameplay (must have for playable game)
1. **No combat system** — The entire "X" (eXplore, eXpand, eXploit, eXterminate) loop is broken without combat
2. **No unit movement** — Units can't move, making the map decorative
3. **No research system** — No progression gate, no technique unlocks
4. **No Founder unit** — First player action is missing entirely
5. **No victory conditions** — No win/loss states, no game end

### Tier 2: Player Engagement (needed for retention)
6. **No events system** — No narrative moments, no random events
7. **No diplomacy** — No player interaction, no deals, no alliances
8. **No settlements** — No map objectives, no trade partners
9. **No technique assignment** — Disciples have no abilities, no build diversity
10. **No AI** — No opponents, no challenge

### Tier 3: Multiplayer (needed for social play)
11. **No network transport** — MP is local-only
12. **No player-vs-player combat** — Simultaneous moves with no resolution
13. **No diplomacy between players** — Can't make deals or alliances

### Tier 4: Polish (needed for release)
14. **No save/load** — Can't persist games
15. **No tutorial** — New players have no guidance

---

## RECOMMENDED RESOLUTION ORDER

Based on genre benchmarks and the GDD's vertical slice methodology:

### Phase A: Minimum Playable Game (2-4 weeks)
1. Unit movement system (GDD §9.2)
2. Combat auto-resolve (GDD §9.3)
3. Founder unit placement (GDD §13.1)
4. Research system stub (GDD §8)
5. Victory condition check (GDD §2)
6. Save/load (GDD §17.5)

### Phase B: Engagement Layer (4-8 weeks)
7. Events system (GDD §14)
8. Diplomacy system (GDD §10)
9. Settlements (GDD §10.2)
10. Technique assignment (GDD §6.5)
11. AI system (GDD §12)

### Phase C: Multiplayer (4-8 weeks)
12. Network transport layer
13. PvP combat resolution
14. Player diplomacy
15. Host migration

### Phase D: Polish (2-4 weeks)
16. Tutorial system
17. UI/UX polish
18. Balance pass

---

## CATEGORY 8: VISUAL & ART PIPELINE

### GDD Vision (§16)
- Wuxia aesthetic: flowing robes, martial arts animations, ink-wash UI elements
- Hex tiles rendered with 3D terrain; buildings appear as sect compounds grow
- Zodiac visual theming: golden glow (Dragon), swirling mist (Snake), red lanterns (Rat)
- Qi effects as visible energy flows on high-density tiles and during combat
- Traditional Chinese instruments (guzheng, erhu, dizi) with dynamic music layering
- Martial arts SFX: sword clashes, Qi discharge, ambient nature
- Narrator for key events; disciple contextual barks

### What's Implemented
- 11 building OBJ meshes (T1 variants) ✅
- 5 unit OBJ meshes (T1-T5 disciples) ✅
- 9 terrain LOD meshes (8 biomes × 3 LODs, minus Desert LOD2) ✅
- 7 prop meshes (trees, rocks, crystal, water plane) ✅
- 18 materials (5 disciple tiers, 13 terrain/building types) ✅
- 47 prefabs (buildings, terrain LODs, props, units) ✅
- Hex grid rendering with chunk system ✅
- Basic camera controller ✅

### GAPS (Visual/Art Perspective)

| Gap | Severity | Description | Benchmark Comparison |
|-----|----------|-------------|---------------------|
| **No textures on meshes** | CRITICAL | All OBJ meshes are untextured. Materials use flat colors. | TW3K: Hand-painted textures on all character models. Jade Dynasty: Detailed texture maps. Civ VI: Stylized textures on all assets. |
| **No shaders** | CRITICAL | No custom shaders exist. GDD specifies ink-wash aesthetic. | TW3K: Custom toon shaders for painterly look. Jade Dynasty: Custom water/ink shaders. Both define the visual identity. |
| **No animations** | HIGH | GDD §16: flowing robes, martial arts animations. Current: static meshes only. | TW3K: Full character animations for combat. Jade Dynasty: Animation-driven combat. Both bring characters to life. |
| **No VFX/particles** | HIGH | GDD §16: Qi energy flows, zodiac ambient effects, combat VFX. Current: nothing. | Jade Dynasty: Extensive Qi/energy VFX. TW3K: Battle VFX for abilities. Both create visual spectacle. |
| **No UI art assets** | HIGH | GDD §15: Ink-wash UI elements, sect banners, icons. Current: IMGUI debug UI only. | TW3K: Beautiful UI with Chinese painting motifs. Jade Dynasty: Themed UI elements. Both reinforce the aesthetic. |
| **No audio** | HIGH | GDD §16: Traditional instruments, SFX, voice. Current: nothing. | TW3K: Excellent Chinese instrument score. Jade Dynasty: Atmospheric audio. Both create immersion. |
| **No LOD system** | MEDIUM | LOD meshes exist but no LODGroup components or switching logic. | Civ VI: LOD system for all models. Essential for performance on large maps. |
| **No USD pipeline** | MEDIUM | GDD §17.8: Blender → USD → Unity USD Plugin. Current: OBJ import. | Pixar's USD is the industry standard for complex asset pipelines. Enables non-destructive editing. |
| **Building meshes are T1 only** | MEDIUM | GDD §6.3: 3 tiers per building with visual upgrades. Current: only T1 meshes. | Civ VI: District visuals upgrade with era. TW3K: Building upgrades are visually distinct. |
| **No terrain blending** | MEDIUM | GDD §4: 8 terrain types should blend at edges. Current: flat color per tile. | Civ VI: Terrain blending with texture transitions. Creates natural-looking maps. |
| **No water shader** | MEDIUM | GDD §4: River/Lake terrain with water effects. Current: flat blue material. | TW3K: Animated water with reflections. Jade Dynasty: Stylized water. Both are visually striking. |
| **No character customization** | LOW | GDD §17: Banner color picker, emblem selection. Current: fixed materials. | TW3K: Character customization. AOW4: Army customization. Both increase player attachment. |

### GAPS (Art Pipeline Perspective)

| Gap | Severity | Description |
|-----|----------|-------------|
| **No texture atlasing** | HIGH | GDD §17.4 specifies texture atlasing for draw call batching. |
| **No GPU instancing setup** | HIGH | GDD §17.4 specifies Graphics.DrawMeshInstanced for units. |
| **No material property blocks** | MEDIUM | Needed for per-instance color variation (sect colors). |
| **No asset bundle system** | MEDIUM | For downloadable content and mod support. |
| **No Blender automation** | MEDIUM | GDD §17.8 specifies Blender → USD pipeline. No automation scripts exist. |

### Art Style Recommendations (based on genre research)

The GDD specifies "Chinese ink-wash stylized realism" — this is a strong direction, but needs refinement based on what works in shipped games:

1. **Reference: Total War: Three Kingdoms** — Stylized realism with painterly textures. Characters are recognizable but idealized. This is the closest benchmark for ToT's disciple models.

2. **Reference: Jade Dynasty / Storm Age** — Wuxia aesthetic with flowing robes, glowing Qi effects, dramatic poses. More stylized than TW3K. Good reference for combat VFX and character silhouettes.

3. **Reference: Civ VI's approach to stylized strategy** — Clear readability over photorealism. Players need to distinguish unit types at a glance. This matters more than visual fidelity for a 4X game.

4. **Recommendation: Hybrid approach** — Use TW3K's character style (stylized realism) for disciples, Jade Dynasty's VFX style for Qi effects, and Civ VI's clarity for UI/readability. The ink-wash aesthetic should be expressed through shaders and UI, not necessarily through mesh topology.

5. **Critical insight from shipped games**: The visual style needs to work at both zoom levels — close-up for tactical battles and zoomed-out for strategic map. TW3K handles this with LODs and a consistent art direction. ToT should plan for this dual-view requirement from the start.

### Recommended Art Production Order

1. **Shader foundation** (week 1-2): Ink-wash terrain shader, water shader, Qi glow shader
2. **Character art** (week 3-6): 5 disciple rank models with textures, LODs, basic animations
3. **Building art** (week 5-8): 11 building types × 3 tiers with visual upgrades
4. **VFX** (week 7-10): Qi effects, combat VFX, zodiac ambient effects
5. **UI art** (week 9-12): Ink-wash UI elements, icons, banners
6. **Audio** (week 11-14): Music, SFX, voice
7. **Polish** (week 13-16): LOD optimization, texture atlasing, GPU instancing
