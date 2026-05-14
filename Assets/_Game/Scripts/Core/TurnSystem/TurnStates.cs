using System;
using UnityEngine;

namespace TalesOfTao.Core.TurnSystem
{
    /// <summary>
    /// Phase 1: Event — Zodiac bonuses applied, random events evaluated,
    /// diplomatic notifications delivered.
    /// </summary>
    public class EventState : ITurnState
    {
        public GamePhase Phase => GamePhase.Event;

        private readonly ZodiacBonusesEventChannelSO _zodiacChannel;
        private ZodiacBonuses _activeBonuses;

        public EventState(ZodiacBonusesEventChannelSO zodiacChannel)
        {
            _zodiacChannel = zodiacChannel;
        }

        public void Enter()
        {
            // Subscribe to zodiac bonuses for this turn
            if (_zodiacChannel != null)
            {
                // The calendar already raised the event; we just note it
                Debug.Log("[TurnEvent] Event phase entered. Zodiac bonuses active.");
            }

            // TODO (M12): Evaluate random events
            // TODO (M10): Deliver diplomatic notifications

            // Auto-complete: Event phase has no blocking operations
            // The TurnStateMachine will advance when EndPhase is called
        }

        public void Tick()
        {
            // Event modals would be processed here in the full implementation
        }

        public void Exit()
        {
            Debug.Log("[TurnEvent] Event phase complete.");
        }
    }

    /// <summary>
    /// Phase 2: Income — Tael collected, Qi collected, commodity yields added,
    /// upkeep deducted. Fully automatic, no player agency.
    /// </summary>
    public class IncomeState : ITurnState
    {
        public GamePhase Phase => GamePhase.Income;

        public void Enter()
        {
            Debug.Log("[TurnIncome] Income phase entered.");

            // TODO (M8): EconomyManager processes income
            // - Tael from trade routes + taxation
            // - Qi from Temple + caves + Ley Lines
            // - Commodity yields added to stockpile
            // - Upkeep deducted
        }

        public void Tick()
        {
            // Automatic processing
        }

        public void Exit()
        {
            Debug.Log("[TurnIncome] Income phase complete.");
        }
    }

    /// <summary>
    /// Phase 3: Build — Construction timers tick, completed buildings finalize,
    /// training queue ticks, promotions complete. Fully automatic.
    /// </summary>
    public class BuildState : ITurnState
    {
        public GamePhase Phase => GamePhase.Build;

        public void Enter()
        {
            Debug.Log("[TurnBuild] Build phase entered.");

            // TODO (M5): Process build queues
            // - Construction timers decrement
            // - Completed buildings finalize
            // - Training queue ticks
            // - Promotions complete
        }

        public void Tick()
        {
            // Automatic processing
        }

        public void Exit()
        {
            Debug.Log("[TurnBuild] Build phase complete.");
        }
    }

    /// <summary>
    /// Phase 4: Research — Research progress advances on active nodes
    /// (up to 3, one per branch). Completions apply unlock effects. Fully automatic.
    /// </summary>
    public class ResearchState : ITurnState
    {
        public GamePhase Phase => GamePhase.Research;

        public void Enter()
        {
            Debug.Log("[TurnResearch] Research phase entered.");

            // TODO (M9): ResearchManager advances progress
            // - Up to 3 active nodes (one per branch)
            // - Completions apply unlock effects
        }

        public void Tick()
        {
            // Automatic processing
        }

        public void Exit()
        {
            Debug.Log("[TurnResearch] Research phase complete.");
        }
    }

    /// <summary>
    /// Phase 5: Action — Full player control. Move units, initiate combat,
    /// issue diplomatic actions, manage queues, use espionage.
    /// This phase blocks until the player presses End Turn.
    /// </summary>
    public class ActionState : ITurnState
    {
        public GamePhase Phase => GamePhase.Action;

        public void Enter()
        {
            Debug.Log("[TurnAction] Action phase entered. Waiting for player input.");

            // Player has full control here
            // The TurnStateMachine.EndTurn() is called when the player presses End Turn
        }

        public void Tick()
        {
            // Player-driven actions happen here
        }

        public void Exit()
        {
            Debug.Log("[TurnAction] Action phase complete.");
        }
    }

    /// <summary>
    /// Phase 6: Resolution — Combat resolves, AI sects execute turns,
    /// trade routes rebalance, market prices drift, victory check runs.
    /// Fully automatic.
    /// </summary>
    public class ResolutionState : ITurnState
    {
        public GamePhase Phase => GamePhase.Resolution;

        public void Enter()
        {
            Debug.Log("[TurnResolution] Resolution phase entered.");

            // TODO (M7): Resolve pending combat
            // TODO (M12): Execute AI turns (time-budgeted)
            // TODO (M8): Rebalance trade routes, drift market prices
            // TODO (M13): Run victory check
        }

        public void Tick()
        {
            // Automatic processing
        }

        public void Exit()
        {
            Debug.Log("[TurnResolution] Resolution phase complete.");
        }
    }
}
