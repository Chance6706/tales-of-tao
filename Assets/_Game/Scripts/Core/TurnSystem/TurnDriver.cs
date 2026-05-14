using UnityEngine;
using TalesOfTao.Core.EventChannels;

namespace TalesOfTao.Core.TurnSystem
{
    /// <summary>
    /// Simple MonoBehaviour that drives the turn cycle.
    /// </summary>
    public class TurnDriver : MonoBehaviour
    {
        [Header("Event Channels")]
        [SerializeField] private GamePhaseEventChannelSO _phaseChangedChannel;
        [SerializeField] private VoidEventChannelSO _turnEndedChannel;
        [SerializeField] private ZodiacBonusesEventChannelSO _zodiacBonusesChannel;

        [Header("Settings")]
        [SerializeField] private float _autoAdvanceDelay = 0f;
        [SerializeField] private bool _logPhases = true;

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
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
                {
                    EndTurn();
                    return;
                }
            }

            if (_autoAdvanceDelay > 0f)
            {
                _phaseTimer += Time.deltaTime;
                if (_phaseTimer >= _autoAdvanceDelay)
                {
                    _phaseTimer = 0f;
                    AdvancePhase();
                }
            }
        }

        public void StartTurn()
        {
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
            var phase = (GamePhase)_currentPhase;
            if (_logPhases)
                Debug.Log($"[Turn {_turnNumber}] Exiting {phase} phase");
        }

        private void CompleteTurn()
        {
            _active = false;
            _turnEndedChannel?.Raise();
            Debug.Log($"[TurnDriver] Turn {_turnNumber} complete.");
        }
    }
}
