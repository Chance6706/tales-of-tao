using UnityEngine;

namespace TalesOfTao.Core.TurnSystem
{
    /// <summary>
    /// Stub AI controller for multiplayer.
    /// Executes AI decisions during the Resolution phase.
    /// Full AI logic will be implemented in M8.
    /// </summary>
    public class AIController : MonoBehaviour
    {
        private int _playerId;
        private bool _initialized;

        public void Initialize(int playerId)
        {
            _playerId = playerId;
            _initialized = true;
        }

        /// <summary>
        /// Called by TurnCoordinator at the start of each turn.
        /// AI makes all its decisions for this turn here.
        /// </summary>
        public void ExecuteTurn()
        {
            if (!_initialized) return;

            // Stub: AI will make build/research/action decisions here
            // For now, AI just ends its phase immediately
            Debug.Log($"[AIController] Player {_playerId} executing turn.");

            // TODO (M8): Implement utility-based AI decision making
            // - Evaluate strategic goal
            // - Issue build commands
            // - Issue research commands
            // - Issue unit movement commands
        }
    }
}
