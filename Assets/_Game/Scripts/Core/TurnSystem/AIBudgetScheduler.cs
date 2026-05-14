using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TalesOfTao.Core.TurnSystem
{
    /// <summary>
    /// Executes AI sect turns during the Resolution phase.
    /// Time-budgeted to prevent long stalls on large maps.
    /// </summary>
    public class AIBudgetScheduler
    {
        private readonly Stopwatch _stopwatch = new();
        private readonly long _budgetTicks;

        /// <summary>
        /// Creates a new AI budget scheduler.
        /// </summary>
        /// <param name="budgetMs">Maximum milliseconds for all AI turns combined.</param>
        public AIBudgetScheduler(float budgetMs = 500f)
        {
            _budgetTicks = (long)(budgetMs * TimeSpan.TicksPerMillisecond / 10000.0);
        }

        /// <summary>
        /// Executes AI turns for all sects, respecting the time budget.
        /// Returns the number of sects that completed their turn.
        /// </summary>
        public int ExecuteTurns(IEnumerable<AISectTurn> sects)
        {
            _stopwatch.Restart();
            int completed = 0;

            foreach (var sect in sects)
            {
                if (_stopwatch.ElapsedTicks >= _budgetTicks)
                {
                    Debug.LogWarning($"[AIBudget] Budget exhausted after {completed} sects. Remaining sects deferred.");
                    break;
                }

                try
                {
                    sect.ExecuteTurn();
                    completed++;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[AIBudget] AI turn failed for sect: {ex.Message}");
                }
            }

            _stopwatch.Stop();
            Debug.Log($"[AIBudget] {completed} AI turns completed in {_stopwatch.ElapsedMilliseconds}ms.");
            return completed;
        }

        /// <summary>
        /// Remaining budget in milliseconds.
        /// </summary>
        public float RemainingMs =>
            Mathf.Max(0f, (_budgetTicks - _stopwatch.ElapsedTicks) * 10000.0f / TimeSpan.TicksPerMillisecond);
    }

    /// <summary>
    /// Interface for an AI sect's turn execution.
    /// Implemented by the AI system (M12).
    /// </summary>
    public interface AISectTurn
    {
        string SectName { get; }
        void ExecuteTurn();
    }

    /// <summary>
    /// Stub implementation of AISectTurn for testing.
    /// Replace with full AI controller in M12.
    /// </summary>
    public class AISectTurnStub : AISectTurn
    {
        public string SectName { get; }

        public AISectTurnStub(string name)
        {
            SectName = name;
        }

        public void ExecuteTurn()
        {
            // Stub: just log
            Debug.Log($"[AI] {SectName} turn executed (stub).");
        }
    }
}
