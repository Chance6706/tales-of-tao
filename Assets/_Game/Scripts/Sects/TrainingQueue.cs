using System;
using System.Collections.Generic;
using UnityEngine;
using TalesOfTao.Core;
using TalesOfTao.Core.EventChannels;
using TalesOfTao.Core.TurnSystem;

namespace TalesOfTao.Sects
{
    /// <summary>
    /// Manages the disciple training/promotion queue. Processes during Build phase.
    /// Capacity determined by Training Grounds tier.
    /// </summary>
    public class TrainingQueue : MonoBehaviour
    {
        [Serializable]
        public struct TrainingEntry
        {
            public string DiscipleName;
            public DiscipleRank FromRank;
            public DiscipleRank ToRank;
            public int TurnsRemaining;
            public int TotalTurns;
            public bool IsComplete;
            public bool IsCancelled;
        }

        [Header("Events")]
        [SerializeField] private StringEventChannelSO _onDiscipleTrained;

        [Header("State")]
        [SerializeField] private List<TrainingEntry> _queue = new();

        public event Action<string, DiscipleRank> OnTrainingCompleted;
        public int QueueLength => _queue != null ? _queue.Count : 0;
        public int MaxConcurrent { get; set; } = 5; // Set by Training Grounds tier

        /// <summary>
        /// Checks if a disciple can be queued for training.
        /// </summary>
        public bool CanQueue(DiscipleRank from, DiscipleRank to)
        {
            if (from >= to) return false;
            if (to > DiscipleRank.HighElder) return false;

            return GetActiveCount() < MaxConcurrent;
        }

        /// <summary>
        /// Adds a disciple to the training queue.
        /// </summary>
        public void Enqueue(string discipleName, DiscipleRank from, DiscipleRank to, int turns)
        {
            if (!CanQueue(from, to))
            {
                Debug.LogWarning($"[TrainingQueue] Cannot queue {discipleName} ({from} -> {to}).");
                return;
            }

            _queue.Add(new TrainingEntry
            {
                DiscipleName = discipleName,
                FromRank = from,
                ToRank = to,
                TurnsRemaining = turns,
                TotalTurns = turns,
                IsComplete = false,
                IsCancelled = false
            });

            Debug.Log($"[TrainingQueue] Queued {discipleName}: {from} -> {to} ({turns} turns). Active: {GetActiveCount()}/{MaxConcurrent}");
        }

        /// <summary>
        /// Processes one turn of training. Called during Build phase.
        /// </summary>
        public void ProcessBuildPhase()
        {
            if (_queue == null || _queue.Count == 0) return;

            bool changed = false;
            for (int i = _queue.Count - 1; i >= 0; i--)
            {
                var entry = _queue[i];
                if (entry.IsComplete || entry.IsCancelled) continue;

                entry.TurnsRemaining--;
                changed = true;

                if (entry.TurnsRemaining <= 0)
                {
                    entry.IsComplete = true;
                    _queue[i] = entry;

                    Debug.Log($"[TrainingQueue] Training complete: {entry.DiscipleName} is now {entry.ToRank}");
                    OnTrainingCompleted?.Invoke(entry.DiscipleName, entry.ToRank);
                    _onDiscipleTrained?.Raise(entry.DiscipleName);
                }
                else
                {
                    _queue[i] = entry;
                }
            }

            if (changed)
            {
                CleanupCompleted();
            }
        }

        /// <summary>
        /// Cancels a training entry by index.
        /// </summary>
        public void Cancel(int index)
        {
            if (_queue == null || index < 0 || index >= _queue.Count) return;

            var entry = _queue[index];
            if (entry.IsComplete) return;

            entry.IsCancelled = true;
            _queue[index] = entry;

            Debug.Log($"[TrainingQueue] Cancelled: {entry.DiscipleName}");
        }

        /// <summary>
        /// Gets the current queue state (for UI display).
        /// </summary>
        public List<TrainingEntry> GetQueue()
        {
            return _queue ?? new List<TrainingEntry>();
        }

        private int GetActiveCount()
        {
            if (_queue == null) return 0;
            int count = 0;
            foreach (var entry in _queue)
            {
                if (!entry.IsComplete && !entry.IsCancelled) count++;
            }
            return count;
        }

        private void CleanupCompleted()
        {
            if (_queue == null) return;
            _queue.RemoveAll(e => e.IsComplete || e.IsCancelled);
        }
    }
}
