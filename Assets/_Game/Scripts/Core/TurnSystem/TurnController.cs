using UnityEngine;
using TalesOfTao.Core.TurnSystem;

namespace TalesOfTao.Core.TurnSystem
{
    /// <summary>
    /// Top-level controller that initializes and drives the turn system.
    /// Attach to a GameObject in the scene alongside GameManager.
    /// </summary>
    public class TurnController : MonoBehaviour
    {
        [SerializeField] private TurnStateMachine _stateMachine;
        [SerializeField] private ZodiacCalendar _calendar;
        [SerializeField] private PhaseInputController _inputController;

        private bool _turnActive;

        private void Start()
        {
            // Initialize the state machine
            _stateMachine.Initialize(_calendar);
            _inputController.Initialize(_stateMachine);

            // Subscribe to turn completion
            _stateMachine.OnTurnCompleted += OnTurnCompleted;

            // Start the first turn
            _calendar.Initialize();
            _stateMachine.StartTurn();
            _turnActive = true;
        }

        private void OnDestroy()
        {
            if (_stateMachine != null)
            {
                _stateMachine.OnTurnCompleted -= OnTurnCompleted;
            }
        }

        private void OnTurnCompleted()
        {
            _turnActive = false;

            // Advance the calendar and start the next turn
            _calendar.AdvanceTurn();
            _stateMachine.StartTurn();
            _turnActive = true;
        }

        /// <summary>
        /// Public API for the End Turn button.
        /// </summary>
        public void EndTurn()
        {
            if (_turnActive)
            {
                _stateMachine.EndTurn();
            }
        }
    }
}
