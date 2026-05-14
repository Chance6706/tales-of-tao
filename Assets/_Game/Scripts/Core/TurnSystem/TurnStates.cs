using System;
using UnityEngine;
using TalesOfTao.Core.EventChannels;

namespace TalesOfTao.Core.TurnSystem
{
    /// <summary>
    /// Interface for each turn phase state.
    /// </summary>
    public interface ITurnState
    {
        GamePhase Phase { get; }
        void Enter();
        void Tick();
        void Exit();
    }

    /// <summary>
    /// Phase 1: Event — Zodiac bonuses applied, random events evaluated,
    /// diplomatic notifications delivered.
    /// </summary>
    public class EventState : ITurnState
    {
        public GamePhase Phase => GamePhase.Event;

        private readonly ZodiacBonusesEventChannelSO _zodiacChannel;

        public EventState(ZodiacBonusesEventChannelSO zodiacChannel)
        {
            _zodiacChannel = zodiacChannel;
        }

        public void Enter()
        {
            Debug.Log("[TurnEvent] Event phase entered. Zodiac bonuses active.");
        }

        public void Tick() { }
        public void Exit()
        {
            Debug.Log("[TurnEvent] Event phase complete.");
        }
    }

    /// <summary>
    /// Phase 2: Income — Tael collected, Qi collected, commodity yields added,
    /// upkeep deducted. Fully automatic.
    /// </summary>
    public class IncomeState : ITurnState
    {
        public GamePhase Phase => GamePhase.Income;

        public void Enter()
        {
            Debug.Log("[TurnIncome] Income phase entered.");
            // TODO (M8): EconomyManager processes income
        }

        public void Tick() { }
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
        }

        public void Tick() { }
        public void Exit()
        {
            Debug.Log("[TurnBuild] Build phase complete.");
        }
    }

    /// <summary>
    /// Phase 4: Research — Research progress advances on active nodes.
    /// Fully automatic.
    /// </summary>
    public class ResearchState : ITurnState
    {
        public GamePhase Phase => GamePhase.Research;

        public void Enter()
        {
            Debug.Log("[TurnResearch] Research phase entered.");
            // TODO (M9): ResearchManager advances progress
        }

        public void Tick() { }
        public void Exit()
        {
            Debug.Log("[TurnResearch] Research phase complete.");
        }
    }

    /// <summary>
    /// Phase 5: Action — Full player control. Blocks until End Turn.
    /// </summary>
    public class ActionState : ITurnState
    {
        public GamePhase Phase => GamePhase.Action;

        public void Enter()
        {
            Debug.Log("[TurnAction] Action phase entered. Waiting for player input.");
        }

        public void Tick() { }
        public void Exit()
        {
            Debug.Log("[TurnAction] Action phase complete.");
        }
    }

    /// <summary>
    /// Phase 6: Resolution — Combat resolves, AI sects execute turns,
    /// trade routes rebalance, market prices drift, victory check runs.
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

        public void Tick() { }
        public void Exit()
        {
            Debug.Log("[TurnResolution] Resolution phase complete.");
        }
    }
}
