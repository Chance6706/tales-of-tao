using UnityEngine;
using UnityEngine.InputSystem;
using TalesOfTao.Core.EventChannels;

namespace TalesOfTao.Core.TurnSystem
{
    /// <summary>
    /// Drives the turn cycle. Auto-advances through non-Action phases,
    /// waits for player input during Action phase.
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

        private ZodiacCalendar _calendar;
        private int _currentPhase;
        private int _turnNumber;
        private float _phaseTimer;
        private bool _active;
        private bool _initialized;

        public int TurnNumber => _turnNumber;
        public GamePhase CurrentPhase => (GamePhase)_currentPhase;
        public string CurrentAnimal => _calendar != null ? _calendar.CurrentAnimal : "None";
        public bool IsActive => _active;

        public event System.Action<GamePhase> OnPhaseChanged;
        public event System.Action<int> OnTurnStarted;

        public void Initialize(ZodiacCalendar calendar, GamePhaseEventChannelSO phaseCh, VoidEventChannelSO turnEndCh, ZodiacBonusesEventChannelSO zodiacCh, float autoDelay)
        {
            _calendar = calendar;
            _phaseChangedChannel = phaseCh;
            _turnEndedChannel = turnEndCh;
            _zodiacBonusesChannel = zodiacCh;
            _autoAdvanceDelay = autoDelay;
            _initialized = true;
        }

        private void Update()
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
            Debug.Log($"[TurnDriver] StartTurn: _initialized={_initialized}, _active={_active}, _turnNumber={_turnNumber}");
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
    }
}
