using System;
using UnityEngine;

namespace TalesOfTao.Core.TurnSystem
{
    /// <summary>
    /// Interface for each turn phase state.
    /// Each phase has Enter, Tick (for async operations), and Exit.
    /// </summary>
    public interface ITurnState
    {
        GamePhase Phase { get; }
        void Enter();
        void Tick();
        void Exit();
    }

    /// <summary>
    /// State machine driving the 6 turn phases.
    /// Transitions are event-driven: each phase signals completion,
    /// and the machine advances to the next.
    /// </summary>
    public class TurnStateMachine : MonoBehaviour
    {
        [SerializeField] private GamePhaseEventChannelSO _onPhaseChanged;
        [SerializeField] private VoidEventChannelSO _onTurnEnded;
        [SerializeField] private ZodiacBonusesEventChannelSO _zodiacBonusesChannel;

        private ITurnState[] _states;
        private int _currentStateIndex;
        private bool _isRunning;
        private ZodiacCalendar _calendar;

        public GamePhase CurrentPhase => (GamePhase)_currentStateIndex;
        public ITurnState CurrentState => _states[_currentStateIndex];
        public bool IsRunning => _isRunning;
        public ZodiacCalendar Calendar => _calendar;

        public event Action<GamePhase> OnPhaseEntered;
        public event Action OnTurnCompleted;

        public void Initialize(ZodiacCalendar calendar)
        {
            _calendar = calendar;

            // Create the 6 phase states
            _states = new ITurnState[]
            {
                new EventState(_zodiacBonusesChannel),
                new IncomeState(),
                new BuildState(),
                new ResearchState(),
                new ActionState(),
                new ResolutionState(),
            };

            _currentStateIndex = 0;
            _isRunning = false;
        }

        /// <summary>
        /// Starts the turn cycle from the Event phase.
        /// </summary>
        public void StartTurn()
        {
            if (_states == null)
            {
                Debug.LogError("[TurnStateMachine] Not initialized. Call Initialize() first.");
                return;
            }

            _isRunning = true;
            _currentStateIndex = 0;
            EnterCurrentPhase();
        }

        /// <summary>
        /// Advances to the next phase. Called when the current phase completes.
        /// </summary>
        public void AdvancePhase()
        {
            if (!_isRunning) return;

            CurrentState.Exit();
            _currentStateIndex++;

            if (_currentStateIndex >= _states.Length)
            {
                // Turn complete
                CompleteTurn();
            }
            else
            {
                EnterCurrentPhase();
            }
        }

        /// <summary>
        /// Ends the player's Action phase and triggers the rest of the turn
        /// (Resolution phase + next turn's Event/Income/Build/Research).
        /// </summary>
        public void EndTurn()
        {
            if (!_isRunning) return;
            if (CurrentPhase != GamePhase.Action)
            {
                Debug.LogWarning($"[TurnStateMachine] EndTurn called during {CurrentPhase}, ignoring.");
                return;
            }

            // Exit Action phase
            CurrentState.Exit();
            _currentStateIndex = (int)GamePhase.Resolution;
            EnterCurrentPhase();

            // Auto-advance through Resolution
            CurrentState.Exit();
            CompleteTurn();
        }

        private void EnterCurrentPhase()
        {
            var phase = CurrentPhase;
            Debug.Log($"[TurnStateMachine] Entering phase: {phase}");
            _onPhaseChanged?.Raise(phase);
            CurrentState.Enter();
            OnPhaseEntered?.Invoke(phase);
        }

        private void CompleteTurn()
        {
            _isRunning = false;
            _onTurnEnded?.Raise();
            OnTurnCompleted?.Invoke();
            Debug.Log("[TurnStateMachine] Turn complete.");
        }
    }
}
