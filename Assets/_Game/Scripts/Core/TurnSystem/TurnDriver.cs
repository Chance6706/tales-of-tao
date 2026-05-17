using UnityEngine;
using UnityEngine.InputSystem;
using TalesOfTao.Core.Commands;
using TalesOfTao.Core.EventChannels;

namespace TalesOfTao.Core.TurnSystem
{
    /// <summary>
    /// Drives the turn cycle for a local player.
    /// In multiplayer, receives phase events from TurnCoordinator.
    /// In single-player (no coordinator), falls back to local state management.
    /// </summary>
    public class TurnDriver : MonoBehaviour
    {
        [Header("Event Channels")]
        [SerializeField] private GamePhaseEventChannelSO _phaseChangedChannel;
        [SerializeField] private VoidEventChannelSO _turnEndedChannel;
        [SerializeField] private ZodiacBonusesEventChannelSO _zodiacBonusesChannel;

        [Header("Settings")]
        [SerializeField] private float _autoAdvanceDelay = 0f;
        [SerializeField] private float _nonActionPhaseDelay = 0.5f;
        [SerializeField] private bool _logPhases = false;

        [Header("Multiplayer")]
        [SerializeField] private int _localPlayerId = 1;

        // Single-player state (used when no coordinator)
        private ZodiacCalendar _calendar;
        private int _currentPhase;
        private int _turnNumber;
        private float _phaseTimer;
        private bool _active;
        private bool _initialized;

        // Multiplayer reference
        private TurnCoordinator _coordinator;

        // Public access
        public int TurnNumber => _coordinator != null ? _coordinator.TurnNumber : _turnNumber;
        public GamePhase CurrentPhase => _coordinator != null ? _coordinator.CurrentPhase : (GamePhase)_currentPhase;
        public string CurrentAnimal => _calendar != null ? _calendar.CurrentAnimal : "None";
        public bool IsActive => _coordinator != null ? _coordinator.IsGameActive : _active;
        public bool IsMultiplayer => _coordinator != null;
        public int LocalPlayerId => _localPlayerId;

        public event System.Action<GamePhase> OnPhaseChanged;
        public event System.Action<int> OnTurnStarted;

        /// <summary>
        /// Initialize for single-player mode.
        /// </summary>
        public void Initialize(ZodiacCalendar calendar, GamePhaseEventChannelSO phaseCh, VoidEventChannelSO turnEndCh, ZodiacBonusesEventChannelSO zodiacCh, float autoDelay)
        {
            _calendar = calendar;
            _phaseChangedChannel = phaseCh;
            _turnEndedChannel = turnEndCh;
            _zodiacBonusesChannel = zodiacCh;
            _autoAdvanceDelay = autoDelay;
            _initialized = true;
        }

        /// <summary>
        /// Initialize for multiplayer mode. Finds and binds to the TurnCoordinator.
        /// </summary>
        public void InitializeMultiplayer(int playerId)
        {
            _localPlayerId = playerId;
            _coordinator = FindAnyObjectByType<TurnCoordinator>();

            if (_coordinator == null)
            {
                Debug.LogWarning("[TurnDriver] No TurnCoordinator found. Falling back to single-player mode.");
                return;
            }

            // Subscribe to coordinator events
            _coordinator.OnPhaseStarted += HandlePhaseStarted;
            _coordinator.OnPhaseEnded += HandlePhaseEnded;
            _coordinator.OnTurnStarted += HandleTurnStarted;

            _initialized = true;

            if (_logPhases)
                Debug.Log($"[TurnDriver] Initialized for multiplayer as player {playerId}");
        }

        private void OnDestroy()
        {
            if (_coordinator != null)
            {
                _coordinator.OnPhaseStarted -= HandlePhaseStarted;
                _coordinator.OnPhaseEnded -= HandlePhaseEnded;
                _coordinator.OnTurnStarted -= HandleTurnStarted;
            }
        }

        private void Update()
        {
            if (!_initialized) return;

            if (IsMultiplayer)
            {
                UpdateMultiplayer();
            }
            else
            {
                UpdateSinglePlayer();
            }
        }

        #region Single-Player (original behavior)

        private void UpdateSinglePlayer()
        {
            if (!_active) return;

            // Keyboard shortcut: Enter or Space to end turn during Action phase
            if ((GamePhase)_currentPhase == GamePhase.Action)
            {
                bool endTurnPressed = false;
                var keyboard = Keyboard.current;
                if (keyboard != null)
                {
                    if (keyboard.enterKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame)
                        endTurnPressed = true;
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
                        endTurnPressed = true;
                }

                if (endTurnPressed)
                {
                    EndTurn();
                    return;
                }
            }

            // Auto-advance non-Action phases
            if ((GamePhase)_currentPhase != GamePhase.Action)
            {
                _phaseTimer += Time.deltaTime;
                float delay = _autoAdvanceDelay > 0f ? _autoAdvanceDelay : _nonActionPhaseDelay;
                if (_phaseTimer >= delay)
                {
                    _phaseTimer = 0f;
                    AdvancePhase();
                }
            }
        }

        public void StartTurn()
        {
            if (IsMultiplayer)
            {
                Debug.LogWarning("[TurnDriver] StartTurn() should be called on TurnCoordinator in multiplayer mode.");
                return;
            }

            if (!_initialized) return;

            _turnNumber++;
            _currentPhase = 0;
            _phaseTimer = 0f;
            _active = true;

            if (_calendar != null)
            {
                if (_turnNumber == 1)
                    _calendar.Initialize();
                else
                    _calendar.AdvanceTurn();
            }

            EnterPhase();
            OnTurnStarted?.Invoke(_turnNumber);
        }

        public void AdvancePhase()
        {
            if (IsMultiplayer)
            {
                Debug.LogWarning("[TurnDriver] AdvancePhase() should be called on TurnCoordinator in multiplayer mode.");
                return;
            }

            if (!_active) return;

            ExitPhase();
            _currentPhase++;

            if (_currentPhase >= 6)
            {
                CompleteTurn();
                return;
            }

            EnterPhase();
        }

        public void EndTurn()
        {
            if (IsMultiplayer)
            {
                // In multiplayer, signal ready to coordinator
                _coordinator?.OnPlayerReady(_localPlayerId);
                return;
            }

            if (!_active) return;
            if ((GamePhase)_currentPhase != GamePhase.Action)
            {
                Debug.LogWarning($"[TurnDriver] EndTurn called during {(GamePhase)_currentPhase}, ignoring.");
                return;
            }

            ExitPhase();
            _currentPhase = 5; // Resolution
            EnterPhase();
            ExitPhase();
            CompleteTurn();
        }

        /// <summary>
        /// Submits a command. In single-player, executes immediately.
        /// In multiplayer, sends to coordinator.
        /// </summary>
        public void SubmitCommand(Command command)
        {
            if (IsMultiplayer)
            {
                _coordinator?.SubmitCommand(_localPlayerId, command);
            }
            else
            {
                if (command.CanExecute())
                    command.Execute();
            }
        }

        private void EnterPhase()
        {
            var phase = (GamePhase)_currentPhase;
            _phaseChangedChannel?.Raise(phase);
            OnPhaseChanged?.Invoke(phase);

            if (_logPhases)
                Debug.Log($"[Turn {_turnNumber}] Entering {phase} phase");
        }

        private void ExitPhase()
        {
            if (_logPhases)
            {
                var phase = (GamePhase)_currentPhase;
                Debug.Log($"[Turn {_turnNumber}] Exiting {phase} phase");
            }
        }

        private void CompleteTurn()
        {
            _active = false;
            _turnEndedChannel?.Raise();
        }

        #endregion

        #region Multiplayer

        private void UpdateMultiplayer()
        {
            // In multiplayer, the coordinator drives phase transitions.
            // TurnDriver only handles local input (e.g., pressing Ready).
            
            if (!IsActive) return;

            var phase = CurrentPhase;

            // During player-driven phases, check for ready input
            if (phase == GamePhase.Build || phase == GamePhase.Action)
            {
                var keyboard = Keyboard.current;
                if (keyboard != null)
                {
                    if (keyboard.enterKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame)
                    {
                        SignalReady();
                    }
                }
            }
        }

        /// <summary>
        /// Signals to the coordinator that the local player is ready to advance.
        /// </summary>
        public void SignalReady()
        {
            if (!IsMultiplayer) return;
            _coordinator?.OnPlayerReady(_localPlayerId);
        }

        private void HandlePhaseStarted(GamePhase phase)
        {
            OnPhaseChanged?.Invoke(phase);

            if (_logPhases)
                Debug.Log($"[TurnDriver] Phase started: {phase}");
        }

        private void HandlePhaseEnded(GamePhase phase)
        {
            if (_logPhases)
                Debug.Log($"[TurnDriver] Phase ended: {phase}");
        }

        private void HandleTurnStarted(int turn)
        {
            OnTurnStarted?.Invoke(turn);

            if (_logPhases)
                Debug.Log($"[TurnDriver] Turn {turn} started");
        }

        #endregion
    }
}
