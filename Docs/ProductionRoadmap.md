# Tales of Tao — Production Roadmap

## Philosophy
- GDD is guidance, not gospel. When GDD conflicts with current standards/norms, we adjust.
- Vertical slice methodology: each phase produces something playable and testable.
- Research & refinement at the start of each phase — don't commit to approaches prematurely.
- Parallel tracks: gameplay code and art production can proceed concurrently.

## Current State (May 2026)
- M5 Asset Integration complete (PR #32 merged)
- Multiplayer turn system architecture in progress (feature/multiplayer-turn-system)
- 85 C# scripts, 120 art assets (meshes/materials/prefabs)
- Core systems: hex grid, pathfinding, fog of war, sect founding, build/training queues, disciple hierarchy
- Known gaps documented in Docs/GapAnalysis.md

---

## PHASE 1: FOUNDATION (Weeks 1-6)
**Goal: Minimum playable single-player game loop**

### Research & Refinement (Week 1)
Before writing code, research and decide on:
- **Combat system design**: Auto-resolve formula (GDD §9.3), tactical view scope, duel mechanics. Reference: Civ VI's simplicity vs TW3K's depth. Decision: how deep does combat need to be for v1?
- **Unit movement model**: Pathfinding integration, terrain costs, ZOC. Reference: Civ VI's one-unit-per-tile vs AoE4's formations. Decision: unit stacking rules?
- **Research tree structure**: How many techs per branch? Linear vs branching? Reference: Civ VI's tech web vs Stellaris's card system. Decision: complexity level for v1?
- **Founder unit design**: Should it be a unique unit type or a special disciple? Reference: Civ VI's Settler + Warrior start. Decision: how does founding consume the Founder?
- **Network architecture**: P2P vs client-server? Lockstep vs deterministic? Reference: Civ VI's Steam P2P, Stellaris's Paradox P2P. Decision: transport layer for v1 MP?

### Gameplay Implementation (Weeks 2-5)
1. **Unit movement system** — Integrate HexPathfinder with unit GameObjects, terrain costs, movement points
2. **Founder unit** — Placeable, movable, consumed on sect founding
3. **Combat auto-resolve** — Stat comparison formula, terrain bonuses, randomness (±15%), casualties
4. **Research system** — TechNodeSO assets, ResearchManager, 3 branches, hall tier gating
5. **Victory conditions** — VictoryChecker, Domination/Enlightenment/Influence tracking
6. **Save/load** — GameStateDTO, JSON serialization, autosave

### Art Production (Weeks 1-6, parallel)
1. **Shader foundation** (Weeks 1-2): Ink-wash terrain shader, water shader, Qi glow shader
2. **Character art** (Weeks 3-6): 5 disciple rank models with textures, LODs, basic idle animations
3. **Building art** (Weeks 3-6): T2/T3 visual upgrades for existing building meshes

### Deliverable
- Single-player game loop: Found sect → Move units → Fight battles → Research techs → Build → Win/Lose
- Playable with placeholder art (flat colors, no textures)
- Save/load working

### Open Questions to Resolve During Phase 1
- How many research nodes per branch for v1? (GDD says ~35 total — is that too many for initial release?)
- Should combat have a tactical view at launch, or auto-resolve only?
- How many unit types at launch? (GDD §9 specifies 8 — Peon Gang, Outer Patrol, Inner Squad, Elder Champion, High Elder Vanguard, Support Caravan, Settler Party, Spy)
- What's the map size target? (GDD §17.6 specifies up to 16,000 tiles — what's realistic for v1?)

---

## PHASE 2: ENGAGEMENT (Weeks 7-14)
**Goal: Game feels alive — events, diplomacy, AI opponents, progression depth**

### Research & Refinement (Week 7)
- **Events system**: Event frequency, player choice impact, narrative integration. Reference: Civ VI's events vs Stellaris's anomaly chains. Decision: how scripted vs procedural?
- **Diplomacy model**: Trust mechanics, deal types, AI personality weights. Reference: Civ VI's diplomacy screen vs Stellaris's federations. Decision: complexity level for v1?
- **AI architecture**: Utility system design, personality types, difficulty scaling. Reference: Civ VI's agendas vs Stellaris's AI weights. Decision: how sophisticated does AI need to be for v1?
- **Technique system**: Weapon vs combat techniques, assignment rules, power scaling. Reference: TW3K's skill trees vs AOW4's tomes. Decision: how many techniques per branch?
- **Settlement design**: Trust tiers, quest types, recruitment mechanics. Reference: Civ VI's city-states vs AoE4's trade posts. Decision: how many settlement types for v1?

### Gameplay Implementation (Weeks 8-13)
1. **Events system** — EventDefinitionSO, EventScheduler, EventModal UI, 9 event types
2. **Diplomacy system** — DiplomacyManager, trust tracking, trade agreements, alliances, war
3. **Settlements** — Settlement entities on map, trust tiers, quests, recruitment
4. **Technique assignment** — TechniqueSystem, weapon/combat techniques, auto-assign on promotion
5. **Elder specialization** — Hall assignment, bonus calculation
6. **AI system** — AIPersonality interface, 6 personality types, utility-based decisions, difficulty scaling
7. **Market system** — MarketSimulator, buy/sell, price fluctuation, Face modifier
8. **Trade routes** — TradeRoute class, distance-scaled income, supply line upkeep

### Art Production (Weeks 7-14, parallel)
1. **VFX** (Weeks 7-10): Qi energy flows, combat impact VFX, zodiac ambient effects
2. **UI art** (Weeks 9-12): Ink-wash UI elements, sect banners, icons, minimap
3. **Audio** (Weeks 11-14): Traditional Chinese instrument score (guzheng, erhu, dizi), martial arts SFX
4. **Building art** (Weeks 7-10): Remaining building types (T2/T3 for all 11 buildings)
5. **Prop art** (Weeks 9-12): Additional props (lanterns, statues, cultivation caves, spirit veins)

### Deliverable
- Full single-player experience: events fire, AI opponents make decisions, diplomacy works, techniques matter
- Game has narrative moments and strategic depth
- Art: textured models, VFX, UI art, audio all in place

### Open Questions to Resolve During Phase 2
- How many AI personalities are needed at launch? (GDD specifies 6 — can we start with 3?)
- Should the market be a separate screen or integrated into the sect overview?
- How important is the Face system vs simpler reputation? (GDD §10.4 is complex — can we simplify?)
- Do we need all 9 event types at launch, or can we start with 4-5 core ones?

---

## PHASE 3: MULTIPLAYER (Weeks 15-22)
**Goal: Playable 4-8 player online game**

### Research & Refinement (Week 15)
- **Network transport**: Steam P2P vs Epic OSS vs custom UDP. Reference: Civ VI's Steam P2P, Stellaris's Paradox P2P. Decision: which platform/launcher?
- **Lockstep vs deterministic**: How to handle simultaneous moves without conflicts. Reference: Civ VI's "simultaneous except during war" mode. Decision: conflict resolution model?
- **Desync prevention**: Deterministic random, command validation, state hashing. Reference: Stellaris's desync issues (known problem). Decision: how to test for desyncs?
- **PvP combat**: Auto-resolve between players, tactical battle option, retreat rules. Reference: AOW4's simultaneous battles. Decision: how to handle PvP combat fairly?
- **Player diplomacy**: Deal-making UI, alliance mechanics, betrayal penalties. Reference: Civ VI's diplomacy screen. Decision: real-time or turn-based diplomacy?

### Implementation (Weeks 16-21)
1. **Network transport layer** — Command serialization, player connection, host authority
2. **PvP combat resolution** — Conflict detection, auto-resolve, optional tactical battle
3. **Player diplomacy** — Real-time deal-making, alliances, trade agreements
4. **Host migration** — State transfer, reconnection handling
5. **AI takeover on disconnect** — Graceful degradation when players drop
6. **Async/PBEM option** — Turn submission, notification system (stretch goal)
7. **Spectator mode** — Read-only game viewing (stretch goal)

### Art Production (Weeks 15-22, parallel)
1. **Animation** (Weeks 15-18): Combat animations, disciple idle animations, building construction animations
2. **Advanced VFX** (Weeks 17-20): Weather effects, seasonal changes, zodiac year visual themes
3. **Character customization** (Weeks 19-22): Banner colors, emblem selection, sect identity
4. **Performance optimization** (Weeks 20-22): LOD optimization, texture atlasing, GPU instancing, draw call batching

### Deliverable
- 4-8 player online multiplayer working
- PvP combat, player diplomacy, host migration
- Full art pipeline: textures, shaders, animations, VFX, audio, UI art
- Performance targets met (GDD §17.6: <33ms for Medium map)

### Open Questions to Resolve During Phase 3
- Which platform/launcher? (Steam enables P2P networking; Epic has different networking OSS)
- How to handle different time zones for async play? (PBEM vs real-time with long timers)
- What's the maximum player count that's actually fun? (GDD says 4-8 — test with 4 first)
- Should we support modding from the start? (Asset bundles, mod loading)

---

## PHASE 4: POLISH & SHIP (Weeks 23-30)
**Goal: Production-ready game**

### Research & Refinement (Week 23)
- **Tutorial design**: How to teach complex systems without overwhelming. Reference: Civ VI's contextual tutorials vs Stellaris's tooltips. Decision: overlay tutorials vs guided first game?
- **Balance testing**: Win rates by strategy, game length, comeback mechanics. Reference: Civ VI's balance patches. Decision: how to gather balance data?
- **Accessibility**: Colorblind modes, text scaling, input remapping. Reference: Civ VI's accessibility options. Decision: minimum accessibility requirements?
- **Store presence**: Store page, trailer, screenshots, description. Reference: Successful indie 4X launches. Decision: which stores? (Steam, GOG, Epic?)

### Implementation (Weeks 24-29)
1. **Tutorial system** — Contextual overlays, guided first game, tooltip system
2. **UI/UX polish** — All screens finalized, minimap, tooltips, hotkeys, settings
3. **Balance pass** — Economy tuning, combat balance, research pacing, victory condition thresholds
4. **Accessibility** — Colorblind modes, text scaling, input remapping
5. **Performance pass** — Profiling, optimization, memory management
6. **Bug fixing** — Playtesting, issue tracking, regression testing
7. **Store preparation** — Store page, trailer, screenshots, demo build

### Art Production (Weeks 23-30, parallel)
1. **Final art polish** — All assets reviewed, consistency pass, quality bar enforcement
2. **Cinematic/trailer** — Gameplay trailer, store page visuals
3. **Localization prep** — String extraction, font support for Chinese characters
4. **DLC planning** — Additional sects, maps, events (post-launch content)

### Deliverable
- Shippable game on target platform(s)
- Tutorial, polish, balance, accessibility all complete
- Store page, trailer, demo ready
- Post-launch content plan

### Open Questions to Resolve During Phase 4
- Release scope: All 10 sects at launch or 5-6 with DLC?
- Price point? (Reference: Civ VI $60, Old World $40, Northgard $30 — where does ToT fit?)
- Early access or full release? (Early access gives feedback but risks first impressions)
- Post-launch support plan? (Free updates vs paid DLC vs expansion packs)

---

## PARALLEL TRACKS (Throughout All Phases)

### Audio Production
- Traditional Chinese instrument score (guzheng, erhu, dizi)
- Dynamic music layering (peaceful/building/combat/research)
- Martial arts SFX (sword clashes, Qi discharge, impacts)
- Ambient nature (birds, wind, water)
- Voice: Narrator for key events, disciple barks

### Quality Assurance
- EditMode tests for all new systems (ongoing)
- Playtest sessions at end of each phase
- Balance testing with AI vs AI games
- Multiplayer stress testing (Phase 3+)
- Performance profiling (Phase 2+)

### Documentation
- Update GDD as design decisions deviate from original vision
- Technical documentation for all systems
- Player-facing documentation (wiki, tooltips, tutorial text)

---

## RISK REGISTER

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| Scope creep | HIGH | Delays all phases | Strict phase gates, cut features to next phase |
| Art production bottleneck | HIGH | Visual quality lags behind gameplay | Start art early, use placeholders, consider asset store for v1 |
| Network desync in MP | MEDIUM | Unplayable multiplayer | Deterministic design from start, extensive testing |
| AI too weak/strong | MEDIUM | Unfun single-player | Iterative balance testing, difficulty scaling |
| Performance on large maps | MEDIUM | Can't support target map sizes | Profile early, LOD system, chunk culling |
| GDD conflicts with norms | LOW | Design confusion | Treat GDD as guidance, research benchmarks, adjust |

---

## KEY DECISIONS LOG

| Decision | Status | Notes |
|----------|--------|-------|
| GDD as guidance vs hard rule | RESOLVED | GDD is guidance. Adjust when it conflicts with current standards/norms. |
| 4-phase vs 6-phase turn system | RESOLVED | 4-phase for multiplayer (Event/Management/Action/Resolution). Merge Income into Management, Research into Management. |
| Simultaneous vs sequential MP turns | RESOLVED | Simultaneous with ready system. "Simultaneous except during war" as fallback. |
| Art style: ink-wash stylized realism | REFINED | Hybrid: TW3K characters + Jade Dynasty VFX + Civ VI UI clarity. Expressed through shaders, not mesh topology. |
| Network transport | PENDING | Research in Phase 3. Likely Steam P2P. |
| Combat depth | PENDING | Research in Phase 1. Auto-resolve first, tactical view later. |
| Research complexity | PENDING | Research in Phase 1. Start simple, add complexity. |
| AI sophistication | PENDING | Research in Phase 2. Start with 3 personalities, expand to 6. |
| Release scope | PENDING | Research in Phase 4. Likely 5-6 sects at launch, rest as DLC. |
| Platform/launcher | PENDING | Research in Phase 3. Steam most likely. |
| Early access vs full release | PENDING | Research in Phase 4. |

---

## SUCCESS CRITERIA

### Phase 1 Gate
- [ ] Can found a sect, move units, fight a battle, research a tech, and win/lose
- [ ] Save/load works reliably
- [ ] No critical bugs in core loop

### Phase 2 Gate
- [ ] Events fire regularly and create interesting choices
- [ ] AI opponents provide challenge on Normal difficulty
- [ ] Diplomacy system allows meaningful player-AI interaction
- [ ] Techniques create visible build diversity
- [ ] Art: all models textured, VFX in place, audio working

### Phase 3 Gate
- [ ] 4-player online game works without desyncs
- [ ] PvP combat resolves fairly
- [ ] Player diplomacy allows deals and alliances
- [ ] Host migration works
- [ ] Performance targets met

### Phase 4 Gate
- [ ] New player can learn the game within 30 minutes
- [ ] Game is balanced (no dominant strategy)
- [ ] All accessibility features working
- [ ] Store page and trailer ready
- [ ] Demo build stable
