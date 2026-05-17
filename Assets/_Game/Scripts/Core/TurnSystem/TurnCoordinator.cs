using System;
using System.Collections.Generic;
using UnityEngine;
using TalesOfTao.Core.Commands;
using TalesOfTao.Core.EventChannels;

namespace TalesOfTao.Core.TurnSystem
{
    /// <summary>
    /// Server/host authority for multiplayer turn management.
    /// Owns the canonical turn state and coordinates all players.
    /// Replaces TurnDriver's local state ownership in multiplayer games.
    /// </summary>
    public class TurnCoordinator : MonoBehaviour
    {
        [Header("Event Channels")]
        [SerializeField] private GamePhaseEventChannelSO _phaseChangedChannel;
        [SerializeField] private VoidEventChannelSO _turnEndedChannel;
        [SerializeField] private ZodiacBonusesEventChannelSO _zodiacBonusesChannel;

        [Header("Settings")]
        [SerializeField] private float _managementPhaseTimeout = 120f;
        [SerializeField] private float _actionPhaseTimeout = 180f;
        [SerializeField] private float _eventPhaseDuration = 5f;
        [SerializeField] private float _resolutionPhaseDuration = 3f;
        [SerializeField] private bool _logPhases = true;

        // Player management
        private readonly Dictionary<int, PlayerSlot> _players = new();
        private int _nextPlayerId = 1;

        // Turn state
        private int _currentPhase;
        private int _turnNumber;
        private bool _gameActive;
        private float _phaseTimer;

        // Subsystems
        private readonly ReadySystem _readySystem = new();
        private readonly NetworkCommandQueue _commandQueue = new();

        // AI controllers (one per AI player)
        private readonly Dictionary<int, AIController> _aiControllers = new();

        // Public access
        public int TurnNumber => _turnNumber;
        public GamePhase CurrentPhase => (GamePhase)_currentPhase;
        public bool IsGameActive => _gameActive;
        public int PlayerCount => _players.Count;
        public float PhaseTimer => _phaseTimer;
        public ReadySystem ReadySystem => _readySystem;
        public NetworkCommandQueue CommandQueue => _commandQueue;

        // Events
        public event Action<GamePhase> OnPhaseStarted;
        public event Action<GamePhase> OnPhaseEnded;
        public event Action<int> OnTurnStarted;
        public event Action<PlayerSlot> OnPlayerReadyChanged;
        public event Action<int> OnPlayerRegistered;
        public event Action<int> OnPlayerUnregistered;

        private void Awake()
        {
            _readySystem.OnAllPlayersReady += HandleAllPlayersReady;
        }

        private void OnDestroy()
        {
            _readySystem.OnAllPlayersReady -= HandleAllPlayersReady;
        }

        private void Update()
        {
            if (!_gameActive) return;

            // Tick the ready system timer
            _readySystem.Tick(Time.deltaTime);

            // Auto-advance non-player phases
            if (IsAutoPhase((GamePhase)_currentPhase))
            {
                _phaseTimer += Time.deltaTime;
                float duration = GetPhaseDuration((GamePhase)_currentPhase);
                if (_phaseTimer >= duration)
                {
                    _phaseTimer = 0f;
                    AdvancePhase();
                }
            }
        }

        #region Player Management

        /// <summary>
        /// Registers a new player slot. Returns the assigned player ID.
        /// </summary>
        public int RegisterPlayer(string playerName, bool isAI, Color color)
        {
            int id = _nextPlayerId++;
            var slot = new PlayerSlot(id, playerName, isAI, color);
            _players[id] = slot;
            _readySystem.AddPlayer(id, isAI);

            if (isAI)
            {
                // Create AI controller for this player
                var ai = gameObject.AddComponent<AIController>();
                ai.Initialize(id);
                _aiControllers[id] = ai;
            }

            OnPlayerRegistered?.Invoke(id);

            if (_logPhases)
                Debug.Log($"[TurnCoordinator] Player registered: {slot}");

            return id;
        }

        /// <summary>
        /// Unregisters a player (e.g., disconnected).
        /// </summary>
        public void UnregisterPlayer(int playerId)
        {
            if (!_players.ContainsKey(playerId)) return;

            _players.Remove(playerId);
            _readySystem.RemovePlayer(playerId);

            if (_aiControllers.TryGetValue(playerId, out var ai))
            {
                _aiControllers.Remove(playerId);
                if (ai != null) Destroy(ai);
            }

            OnPlayerUnregistered?.Invoke(playerId);

            if (_logPhases)
                Debug.Log($"[TurnCoordinator] Player unregistered: {playerId}");
        }

        /// <summary>
        /// Gets a player slot by ID.
        /// </summary>
        public PlayerSlot GetPlayer(int playerId)
        {
            return _players.TryGetValue(playerId, out var slot) ? slot : null;
        }

        /// <summary>
        /// Gets all registered players.
        /// </summary>
        public IEnumerable<PlayerSlot> GetAllPlayers()
        {
            return _players.Values;
        }

        #endregion

        #region Game Flow

        /// <summary>
        /// Starts the game. Registers all players first, then call this.
        /// </summary>
        public void StartGame()
        {
            if (_players.Count == 0)
            {
                Debug.LogError("[TurnCoordinator] Cannot start game with 0 players.");
                return;
            }

            _gameActive = true;
            _turnNumber = 0;
            _currentPhase = -1;

            if (_logPhases)
                Debug.Log($"[TurnCoordinator] Game started with {_players.Count} players.");

            StartTurn();
        }

        /// <summary>
        /// Signals that a player is ready for the next phase.
        /// </summary>
        public void OnPlayerReady(int playerId)
        {
            if (!_players.ContainsKey(playerId)) return;

            _readySystem.SetPlayerReady(playerId);
            _players[playerId].IsReady = true;
            OnPlayerReadyChanged?.Invoke(_players[playerId]);

            if (_logPhases)
                Debug.Log($"[TurnCoordinator] Player {playerId} ready ({_readySystem.ReadyPlayers}/{_readySystem.TotalPlayers})");
        }

        /// <summary>
        /// Submits a command from a player during the current phase.
        /// </summary>
        public void SubmitCommand(int playerId, Command command)
        {
            if (!_gameActive) return;
            if (!_players.ContainsKey(playerId)) return;

            _commandQueue.Enqueue(playerId, command);

            if (_logPhases)
                Debug.Log($"[TurnCoordinator] Command from player {playerId}: {command.GetType().Name}");
        }

        #endregion

        #region Phase Management

        private void StartTurn()
        {
            _turnNumber++;
            _currentPhase = (int)GamePhase.Event;
            _phaseTimer = 0f;

            // Reset ready state for new turn
            _readySystem.ResetAll();

            // Execute AI decisions for this turn
            ExecuteAITurn();

            // Enter Event phase
            EnterPhase();
            OnTurnStarted?.Invoke(_turnNumber);
        }

        private void EnterPhase()
        {
            var phase = (GamePhase)_currentPhase;

            if (_logPhases)
                Debug.Log($"[Turn {_turnNumber}] Entering {phase} phase");

            // Broadcast phase change
            _phaseChangedChannel?.Raise(phase);
            OnPhaseStarted?.Invoke(phase);

            // For player-driven phases, start the ready timer
            if (IsPlayerDrivenPhase(phase))
            {
                float timeout = GetPhaseTimeout(phase);
                _readySystem.StartTimer(timeout);
            }
        }

        private void ExitPhase()
        {
            var phase = (GamePhase)_currentPhase;

            if (_logPhases)
                Debug.Log($"[Turn {_turnNumber}] Exiting {phase} phase");

            // Execute all queued commands
            if (_commandQueue.Count > 0)
            {
                if (_logPhases)
                    Debug.Log($"[TurnCoordinator] Executing {_commandQueue.Count} commands");
                _commandQueue.ExecuteAll();
                _commandQueue.Clear();
            }

            _readySystem.StopTimer();
            OnPhaseEnded?.Invoke(phase);
        }

        public void AdvancePhase()
        {
            if (!_gameActive) return;

            ExitPhase();
            _currentPhase++;

            if (_currentPhase > (int)GamePhase.Resolution)
            {
                CompleteTurn();
                return;
            }

            EnterPhase();
        }

        private void CompleteTurn()
        {
            _turnEndedChannel?.Raise();

            if (_logPhases)
                Debug.Log($"[TurnCoordinator] Turn {_turnNumber} complete.");

            // Start next turn
            StartTurn();
        }

        private void HandleAllPlayersReady()
        {
            if (!_gameActive) return;

            if (_logPhases)
                Debug.Log($"[TurnCoordinator] All players ready, advancing from {(GamePhase)_currentPhase}");

            AdvancePhase();
        }

        #endregion

        #region AI

        private void ExecuteAITurn()
        {
            foreach (var kvp in _aiControllers)
            {
                kvp.Value.ExecuteTurn();
            }
        }

        #endregion

        #region Helpers

        private bool IsPlayerDrivenPhase(GamePhase phase)
        {
            return phase == GamePhase.Build || phase == GamePhase.Action;
        }

        private bool IsAutoPhase(GamePhase phase)
        {
            return phase == GamePhase.Event || phase == GamePhase.Resolution;
        }

        private float GetPhaseDuration(GamePhase phase)
        {
            return phase switch
            {
                GamePhase.Event => _eventPhaseDuration,
                GamePhase.Resolution => _resolutionPhaseDuration,
                _ => 0f
            };
        }

        private float GetPhaseTimeout(GamePhase phase)
        {
            return phase switch
            {
                GamePhase.Build => _managementPhaseTimeout,
                GamePhase.Action => _actionPhaseTimeout,
                _ => 60f
            };
        }

        #endregion
    }
}
