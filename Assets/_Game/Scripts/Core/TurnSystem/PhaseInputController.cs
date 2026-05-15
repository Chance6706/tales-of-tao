using UnityEngine;

namespace TalesOfTao.Core.TurnSystem
{
    /// <summary>
    /// Controls which input actions are valid during each turn phase.
    /// Attach to the player input manager to gate actions by phase.
    /// </summary>
    public class PhaseInputController : MonoBehaviour
    {
        private TurnStateMachine _stateMachine;

        /// <summary>
        /// Whether the player can perform unit actions (move, attack, etc.).
        /// Only allowed during the Action phase.
        /// </summary>
        public bool CanPerformUnitActions =>
            _stateMachine != null &&
            _stateMachine.IsRunning &&
            _stateMachine.CurrentPhase == GamePhase.Action;

        /// <summary>
        /// Whether the player can manage queues (build, research, training).
        /// Allowed during Action phase.
        /// </summary>
        public bool CanManageQueues => CanPerformUnitActions;

        /// <summary>
        /// Whether the player can perform diplomatic actions.
        /// Allowed during Action phase.
        /// </summary>
        public bool CanPerformDiplomacy => CanPerformUnitActions;

        /// <summary>
        /// Whether the player can use espionage actions.
        /// Allowed during Action phase.
        /// </summary>
        public bool CanUseEspionage => CanPerformUnitActions;

        /// <summary>
        /// Whether the End Turn button should be available.
        /// Only during Action phase.
        /// </summary>
        public bool CanEndTurn => CanPerformUnitActions;

        public void Initialize(TurnStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        /// <summary>
        /// Validates and performs a unit action if allowed.
        /// Returns true if the action was performed.
        /// </summary>
        public bool TryPerformUnitAction(System.Action action)
        {
            if (!CanPerformUnitActions)
            {
                Debug.Log("[PhaseInput] Unit action blocked: not in Action phase.");
                return false;
            }
            action();
            return true;
        }

        /// <summary>
        /// Validates and performs a queue management action if allowed.
        /// </summary>
        public bool TryManageQueue(System.Action action)
        {
            if (!CanManageQueues)
            {
                Debug.Log("[PhaseInput] Queue management blocked: not in Action phase.");
                return false;
            }
            action();
            return true;
        }

        /// <summary>
        /// Validates and performs a diplomatic action if allowed.
        /// </summary>
        public bool TryPerformDiplomacy(System.Action action)
        {
            if (!CanPerformDiplomacy)
            {
                Debug.Log("[PhaseInput] Diplomacy blocked: not in Action phase.");
                return false;
            }
            action();
            return true;
        }
    }
}
