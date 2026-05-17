# Multiplayer Turn System Design

## Goals
- Support 4-8 players (human + AI) in a single game
- Simultaneous execution during player-driven phases
- No sequential lockstep — players don't wait for each other mid-phase
- AI players execute instantly at phase start
- Foundation for network transport (not implementing netcode yet)

## Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                    TurnCoordinator                       │
│  (server/host authority, one per game)                  │
│                                                         │
│  - Tracks all players (human + AI)                      │
│  - Manages phase lifecycle                              │
│  - Collects ready signals                               │
│  - Broadcasts phase events to all TurnDrivers           │
│  - Executes AI decisions                                │
│  - Manages command queue                                │
└──────────────┬──────────────────────────┬───────────────┘
               │ phase events             │ phase events
       ┌───────▼───────┐          ┌───────▼───────┐
       │  TurnDriver   │          │  TurnDriver   │
       │  (Player 1)   │          │  Player 2)   │
       │               │          │               │
       │  - Local input │          │  - Local input │
       │  - UI updates  │          │  - UI updates  │
       │  - Sends ready │          │  - Sends ready │
       └───────────────┘          └───────────────┘
```

## New Phase Structure (4 phases, down from 6)

| Phase | Name | Duration | Player-Driven? | What Happens |
|-------|------|----------|----------------|--------------|
| 1 | EVENT | ~5s auto | No | Zodiac bonuses, random events fire. Auto-advances after all players acknowledge. |
| 2 | MANAGEMENT | Untimed | Yes | All players simultaneously: queue builds, assign research, recruit peons, manage disciples. Income auto-calculated. |
| 3 | ACTION | Untimed | Yes | All players simultaneously: move units, initiate combat, perform sect actions. |
| 4 | RESOLUTION | ~10s auto | No | Combat resolves, training completes, buildings finish, events fire. |

### Phase Flow (all players simultaneous)

```
EVENT ──► MANAGEMENT ──► ACTION ──► RESOLUTION ──► (next turn)
           ▲ all ready      ▲ all ready     ▲ auto
           │                │               │
           └── wait for ────┘               │
                all players                 │
                                            └── auto-advance
```

## Key Components

### 1. TurnCoordinator (new)
**Location:** `Assets/_Game/Scripts/Core/TurnSystem/TurnCoordinator.cs`
**Type:** MonoBehaviour (singleton)

**Responsibilities:**
- Owns the authoritative turn state (phase, turn number)
- Tracks all registered players (human + AI)
- Manages ready state per phase
- Broadcasts phase start/end events
- Executes AI player decisions at phase start
- Manages the ready timer (configurable timeout)
- Collects and executes commands from all players

**Public API:**
```csharp
void RegisterPlayer(PlayerSlot player);
void UnregisterPlayer(int playerId);
void OnPlayerReady(int playerId);
void SubmitCommand(int playerId, Command command);
void StartGame();
```

**Events:**
```csharp
event Action<GamePhase> OnPhaseStarted;
event Action<GamePhase> OnPhaseEnded;
event Action<int> OnTurnStarted;  // turn number
event Action<PlayerSlot> OnPlayerReadyChanged;
```

### 2. PlayerSlot (new)
**Location:** `Assets/_Game/Scripts/Core/TurnSystem/PlayerSlot.cs`
**Type:** Plain C# class

```csharp
public class PlayerSlot
{
    public int PlayerId;
    public string PlayerName;
    public bool IsAI;
    public bool IsReady;
    public bool IsConnected;
    public int SectConfigIndex;  // which sect they're playing
    public Color PlayerColor;
}
```

### 3. ReadySystem (new)
**Location:** `Assets/_Game/Scripts/Core/TurnSystem/ReadySystem.cs`
**Type:** Plain C# class

**Responsibilities:**
- Tracks which players are ready for current phase
- Fires event when all players are ready
- Manages optional ready timer
- Handles AI players (auto-ready immediately)

```csharp
public class ReadySystem
{
    public event Action OnAllPlayersReady;
    public event Action<float> OnTimerUpdated;  // remaining seconds
    
    void SetPlayerReady(int playerId);
    void SetPlayerUnready(int playerId);
    void AddPlayer(int playerId, bool isAI);
    void RemovePlayer(int playerId);
    void StartTimer(float duration);
    void StopTimer();
    bool AreAllPlayersReady { get; }
    float RemainingTime { get; }
}
```

### 4. TurnDriver (refactored)
**Location:** `Assets/_Game/Scripts/Core/TurnSystem/TurnDriver.cs`
**Changes:**
- No longer owns turn state — receives phase events from TurnCoordinator
- No longer auto-advances phases — waits for coordinator signals
- Sends ready signal to coordinator when player finishes
- Sends commands to coordinator instead of executing locally
- Still handles local input and UI updates

**Removed:**
- `_currentPhase`, `_turnNumber`, `_active` (owned by coordinator)
- `StartTurn()`, `AdvancePhase()`, `EndTurn()` (coordinator-driven)
- Auto-advance logic in `Update()`

**Added:**
```csharp
void OnPhaseStarted(GamePhase phase);
void OnPhaseEnded(GamePhase phase);
void SignalReady();
void SubmitCommand(Command command);
```

### 5. NetworkCommandQueue (new)
**Location:** `Assets/_Game/Scripts/Core/TurnSystem/NetworkCommandQueue.cs`
**Type:** Plain C# class

**Responsibilities:**
- Collects commands from all players during a phase
- Orders commands deterministically (by player ID, then submission order)
- Executes all commands at phase end
- Validates commands before execution

```csharp
public class NetworkCommandQueue
{
    public void Enqueue(int playerId, Command command);
    public void ExecuteAll();
    public void Clear();
    public int Count { get; }
}
```

### 6. MultiplayerTurnTestHUD (new)
**Location:** `Assets/_Game/Scripts/UI/HUD/MultiplayerTurnTestHUD.cs`
**Type:** MonoBehaviour

**Responsibilities:**
- Shows all players and their ready status
- Shows current phase and turn number
- Shows ready timer countdown
- "Ready" button for local player
- Player list with color coding (ready/not ready/AI)

## Assembly Structure

New assembly: `TalesOfTao.Multiplayer`
- References: `TalesOfTao.Core`, `TalesOfTao.Sects`, `TalesOfTao.Hex`, `TalesOfTao.UI`
- Contains: `TurnCoordinator`, `ReadySystem`, `NetworkCommandQueue`, `PlayerSlot`, `MultiplayerTurnTestHUD`

Existing `TalesOfTao.Core` changes:
- `TurnDriver` refactored (minimal changes, backward compatible)
- `GamePhase` enum unchanged
- `Command` base class unchanged

## Implementation Order

1. `PlayerSlot` — simple data class
2. `ReadySystem` — ready tracking + timer
3. `NetworkCommandQueue` — command collection + execution
4. `TurnCoordinator` — orchestrates everything
5. `TurnDriver` refactor — remove local state, add coordinator hooks
6. `MultiplayerTurnTestHUD` — UI for testing
7. `TalesOfTao.Multiplayer.asmdef` — assembly definition
8. EditMode tests
9. Integration test: 4-player local simulation

## Backward Compatibility

The existing single-player flow still works:
- `TurnDriver` without a `TurnCoordinator` falls back to current behavior
- `M5VisualTest` continues to work as-is
- All existing commands work unchanged

## Network Transport (future, not this phase)

The design is network-ready:
- `TurnCoordinator` is the server authority
- `TurnDriver` on each client only sends commands and receives events
- `NetworkCommandQueue` serializes commands (just need to add serialization later)
- Player slots track connection state

When we add networking later:
- Host runs `TurnCoordinator` as server
- Clients run `TurnDriver` + send commands to host
- Host broadcasts phase events to all clients
- AI players run on the host
