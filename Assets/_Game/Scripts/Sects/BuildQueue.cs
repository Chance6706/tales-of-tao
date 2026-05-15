using System;
using System.Collections.Generic;
using UnityEngine;
using TalesOfTao.Core;
using TalesOfTao.Core.EventChannels;
using TalesOfTao.Core.TurnSystem;

namespace TalesOfTao.Sects
{
    /// <summary>
    /// Manages the construction queue for a sect. Processes during Build phase.
    /// Simultaneous build limit = Temple tier (1-3).
    /// </summary>
    public class BuildQueue : MonoBehaviour
    {
        [Serializable]
        public struct BuildEntry
        {
            public string BuildingTypeId;
            public int Tier;
            public int TurnsRemaining;
            public int TotalTurns;
            public bool IsComplete;
            public bool IsCancelled;
        }

        [Header("Events")]
        [SerializeField] private StringEventChannelSO _onBuildingCompleted;

        [Header("State")]
        [SerializeField] private List<BuildEntry> _queue = new();

        public event Action<string, int> OnBuildingCompleted;
        public int QueueLength => _queue != null ? _queue.Count : 0;
        public int MaxConcurrent { get; set; } = 1; // Set by Temple tier

        /// <summary>
        /// Checks if a building of the given type/tier can be queued.
        /// </summary>
        public bool CanQueue(string buildingTypeId, int tier)
        {
            if (string.IsNullOrEmpty(buildingTypeId)) return false;
            if (tier < 1 || tier > 3) return false;

            if (_queue != null)
            {
                foreach (var entry in _queue)
                {
                    if (!entry.IsComplete && !entry.IsCancelled
                        && entry.BuildingTypeId == buildingTypeId
                        && entry.Tier == tier)
                    {
                        return false;
                    }
                }
            }

            return GetActiveCount() < MaxConcurrent;
        }

        /// <summary>
        /// Adds a building to the construction queue.
        /// </summary>
        public void Enqueue(string buildingTypeId, int tier, int turns)
        {
            if (!CanQueue(buildingTypeId, tier))
            {
                Debug.LogWarning($"[BuildQueue] Cannot queue {buildingTypeId} T{tier}.");
                return;
            }

            _queue.Add(new BuildEntry
            {
                BuildingTypeId = buildingTypeId,
                Tier = tier,
                TurnsRemaining = turns,
                TotalTurns = turns,
                IsComplete = false,
                IsCancelled = false
            });

            Debug.Log($"[BuildQueue] Queued {buildingTypeId} T{tier} ({turns} turns). Active: {GetActiveCount()}/{MaxConcurrent}");
        }

        /// <summary>
        /// Processes one turn of construction. Called during Build phase.
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

                    Debug.Log($"[BuildQueue] Construction complete: {entry.BuildingTypeId} T{entry.Tier}");
                    OnBuildingCompleted?.Invoke(entry.BuildingTypeId, entry.Tier);
                    _onBuildingCompleted?.Raise(entry.BuildingTypeId);
                }
                else
                {
                    _queue[i] = entry;
                }
            }

            if (changed)
            {
                CleanupCancelled();
            }
        }

        /// <summary>
        /// Cancels a queued building by index.
        /// </summary>
        public void Cancel(int index)
        {
            if (_queue == null || index < 0 || index >= _queue.Count) return;

            var entry = _queue[index];
            if (entry.IsComplete) return;

            entry.IsCancelled = true;
            _queue[index] = entry;

            Debug.Log($"[BuildQueue] Cancelled: {entry.BuildingTypeId} T{entry.Tier}");
        }

        /// <summary>
        /// Gets the current queue state (for UI display).
        /// </summary>
        public List<BuildEntry> GetQueue()
        {
            return _queue ?? new List<BuildEntry>();
        }

        /// <summary>
        /// Checks if a building of the given type/tier is already complete.
        /// </summary>
        public bool IsComplete(string buildingTypeId, int tier)
        {
            if (_queue == null) return false;
            foreach (var entry in _queue)
            {
                if (entry.IsComplete && entry.BuildingTypeId == buildingTypeId && entry.Tier == tier)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if a building of the given type/tier is currently under construction.
        /// </summary>
        public bool IsUnderConstruction(string buildingTypeId, int tier)
        {
            if (_queue == null) return false;
            foreach (var entry in _queue)
            {
                if (!entry.IsComplete && !entry.IsCancelled
                    && entry.BuildingTypeId == buildingTypeId && entry.Tier == tier)
                    return true;
            }
            return false;
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

        private void CleanupCancelled()
        {
            if (_queue == null) return;
            _queue.RemoveAll(e => e.IsCancelled);
        }
    }
}
