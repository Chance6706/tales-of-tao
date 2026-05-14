
---

## §18. Technical Implementation Notes

*This section provides architecture and implementation guidance for the development team. It translates the design specifications in §1–§17 into concrete technical patterns and performance budgets.*

### §18.1 Architecture Patterns

#### Event-Driven Architecture
All inter-system communication MUST use ScriptableObject event channels. Direct system-to-system references are prohibited.

**Event Channel Naming**: `On[Subject][Action]` — e.g., `OnUnitMoved`, `OnSectFounded`, `OnTurnPhaseChanged`.

**Event Channel Locations**: Create assets in `Assets/_Game/Data/EventChannels/`. Group by domain: `OnUnitMoved` in `Hex/`, `OnSectFounded` in `Economy/`, etc.

**Listener Lifecycle**: Every MonoBehaviour that registers a listener MUST unregister in `OnDestroy()`:
```csharp
private void OnEnable() => channel.RegisterListener(Handler);
private void OnDisable() => channel.UnregisterListener(Handler);
```

#### Command Pattern
All player actions MUST implement `ICommand` with `Execute()` and `Undo()`. The `CommandQueue` maintains a stack of executed commands for the current turn.

**Rule**: Commands must not directly mutate other systems. They raise event channels that other systems subscribe to.

**Compound Commands**: Multi-step actions (e.g., FoundSect = create data + place building + remove founder) use `CompoundCommand` which executes/undoes sub-commands in order/reverse order.

#### State Pattern
The turn system uses `ITurnState` with `Enter()`, `Exit()`, and `Tick()`. The `TurnStateMachine` drives transitions and broadcasts phase changes via event channels.

**Sub-states**: The Action Phase may have sub-states (UnitSelected, UnitMoving, UnitAttacking) managed by a nested state machine.

#### Data Separation
- **ScriptableObjects** = immutable configuration data (SectConfigSO, BuildingConfigSO, TechNodeSO)
- **Plain C# classes** = mutable runtime state (SectData, UnitData, DiscipleData)
- **MonoBehaviours** = visual representation and input handling only

**Rule**: Never store mutable game state in ScriptableObjects. They persist between play sessions and cause save/load bugs.

### §18.2 Performance Budgets

#### Rendering Budgets (from GDD §17.3)

| Category | Target | Measurement |
|----------|--------|-------------|
| Draw calls (zoomed in) | < 500 | Unity Frame Debugger |
| Draw calls (zoomed out) | < 200 | Unity Frame Debugger |
| Frame time (gameplay) | < 16ms (60fps) | Unity Profiler |
| Frame time (AI turn) | < 500ms | Custom benchmark |
| Texture memory | < 512 MB | Unity Memory Profiler |
| Mesh memory | < 128 MB | Unity Memory Profiler |
| Total memory | < 1 GB | Unity Memory Profiler |

#### AI Budgets (from GDD §12.2)

| Map Size | Per-Sect Budget | Total AI Budget |
|----------|----------------|-----------------|
| Small (2,400 tiles) | 16ms | 64ms |
| Medium (4,800 tiles) | 25ms | 150ms |
| Large (9,600 tiles) | 33ms | 264ms |
| Epic (16,000 tiles) | 50ms | 500ms |

**Budget Enforcement**: The `AIBudgetScheduler` uses `Stopwatch` to track elapsed time. When a sect's budget is exhausted, remaining action evaluations are skipped (pruned). Lower-priority evaluations are pruned first: diplomatic > espionage > economic > military > expansion.

#### Pathfinding Budget
- A* pathfinding for a single unit: < 5ms on Medium map
- Batch pathfinding (all units): < 16ms on Medium map
- Use Burst-compiled jobs with `NativeArray<HexTileData>` for cache-friendly access
- Cache paths for the duration of the Action Phase; invalidate on unit move

### §18.3 Memory Management

#### GC Allocation Rules
- Zero allocations in `Update()` methods (use cached references)
- Zero allocations in pathfinding jobs (use NativeArray)
- Zero allocations in AI evaluation (use pre-allocated buffers)
- Acceptable: allocations during turn phase transitions, event processing

#### Object Pooling Requirements
These object types MUST use object pools:
- Unit selection rings (pool size: 20)
- Combat VFX (pool size: 50)
- UI popup text (pool size: 30)
- Pathfinding debug visualizers (pool size: 100)
- Territory overlay tiles (pool size: 500)

### §18.4 Save/Load Architecture

#### GameStateDTO Pattern
The entire game state is captured in a single `GameStateDTO` plain C# class. Each system contributes its state via a `CaptureState()` method.

**Serialization**: JSON via `JsonUtility.ToJson()` for debugging. GZip compression for production saves.

**Versioning**: Include `int Version` field in DTO. On load, check version and run migration if needed.

**Auto-save**: At end of each Resolution Phase. Keep last 3 auto-saves + manual saves.

### §18.5 Testing Requirements

#### EditMode Tests (Required per Milestone)
- M2: HexCoords tests (distance, neighbors, conversion), map generation validation
- M3: Turn state machine transition tests
- M4: Sect founding validation, economy formula tests
- M5: Building cost validation, disciple promotion tests
- M6: Pathfinding correctness tests (100 random paths)
- M7: CombatPowerCalculator tests (100% coverage), auto-resolve deterministic tests
- M8: EconomyStressTest (1000 iterations, median Tael at T15 > 50)
- M9: Tech tree prerequisite validation
- M10: Diplomacy relation score tests
- M12: AI genome drift tests, AI turn time benchmark

#### PlayMode Tests (Required per Milestone)
- M3: Full turn cycle test (all 6 phases)
- M4: Sect founding end-to-end
- M7: Combat auto-resolve integration test
- M14: Save/load round-trip test, full playthrough test

### §18.6 Asset Pipeline

#### Blender-to-Unity Settings
- **Units**: Metric, 1 BU = 1 Unity meter
- **Up Axis**: Y-Up (Blender default)
- **Apply Transforms**: Always Ctrl+A before export
- **FBX Export**: Scale 1.0, Forward -Z, Up Y, Bake Animation, no leaf bones
- **Characters**: 2000-5000 tris (heroes), Rigify + custom bones
- **Environment**: Modular kit approach, texture atlases for hex tiles

#### LOD Strategy
| LOD Level | Distance | Tri Count | Use Case |
|-----------|----------|-----------|----------|
| LOD0 | 0-10 units | 100% | Close-up, tactical combat |
| LOD1 | 10-30 units | 50% | Standard gameplay |
| LOD2 | 30+ units | 25% | Distant units on strategy map |

#### Texture Budget
| Asset Type | Resolution | Notes |
|-----------|-----------|-------|
| Hero characters | 2048×2048 | Main characters, elders |
| Standard units | 1024×1024 | Disciples, generic units |
| Buildings | 1024-2048 | Depending on screen presence |
| Hex tiles | 512-1024 | Tiling textures |
| Props | 512-1024 | Smaller for distant props |
| UI elements | 256-512 | Icons, banners |

### §18.7 DOTS Integration Strategy

#### Hybrid Approach (Confirmed)
- **MonoBehaviour**: Visual representation, input handling, UI integration, editor tools
- **DOTS Jobs**: Pathfinding (A*), AI evaluation (utility scoring), economy simulation (bulk calculations), fog of War (shadowcasting)

#### DOTS Spike at M6
**Goal**: Evaluate whether converting hex grid data and pathfinding to pure ECS provides >2x performance improvement.

**Success Criteria**:
- Pathfinding on Epic map: <8ms (vs <16ms target with MonoBehaviour)
- AI evaluation on Epic map: <25ms per sect (vs <50ms target)
- No regression in gameplay behavior

**Fallback**: If DOTS spike fails, maintain MonoBehaviour + Burst Jobs hybrid. This is an acceptable outcome — the architecture supports both.

### §18.8 Art Direction

#### Style: Stylized Realism
- Clean, readable silhouettes
- Hand-painted textures (not photorealistic)
- Exaggerated proportions (slightly larger heads, expressive features)
- Bold color choices with limited palettes per area
- Stylized lighting (not physically accurate)

#### Sect Color Palettes
| Sect | Primary | Secondary | Accent |
|------|---------|-----------|--------|
| Wu Dang | Deep blue (#1565C0) | White (#FFFFFF) | Gold (#FFD700) |
| Shaolin | Saffron orange (#FF9800) | Brown (#795548) | Gold (#FFD700) |
| Peng Clan | Black (#212121) | Dark purple (#4A148C) | Silver (#BDBDBD) |
| Namgung | Green (#2E7D32) | Brown (#795548) | Bronze (#CD7F32) |
| Hua Shan | Red (#C62828) | Black (#212121) | Gold (#FFD700) |
| Emei | Pink (#EC407A) | White (#FFFFFF) | Light blue (#81D4FA) |

#### Cultivation Realm Visual Progression
Each realm adds visual indicators: better robes, aura effects, glow intensity, levation, reality distortion. See §6.2 for gameplay effects; art team should reference the visual progression guide in the project style guide document.

### §18.9 Difficulty Levels

| Level | AI Budget | Starting Bonus | Description |
|-------|-----------|---------------|-------------|
| **Initiate** | 50% of normal | +100 Tael, +20 Qi | Forgiving; learning the game |
| **Disciple** | 75% of normal | Standard | Balanced experience |
| **Master** | 100% of normal | Standard | Full challenge |
| **Heavenly Dao** | 100% of normal | -50 Tael, AI gets +10% CP | Hardcore; for experienced players |

**Removed**: Grandmaster difficulty (folded into Master+). 4 levels instead of 5 to reduce balancing overhead.

### §18.10 Multiplayer Architecture (Deferred)

The architecture is designed to support future multiplayer:
- Command pattern enables deterministic replay (required for networked lockstep)
- GameStateDTO enables state synchronization
- Event channels enable local/remote event routing
- No direct system references means network layer can intercept events

**Not implemented at launch**. The architecture is multiplayer-ready but no networking code should be written until post-launch.
