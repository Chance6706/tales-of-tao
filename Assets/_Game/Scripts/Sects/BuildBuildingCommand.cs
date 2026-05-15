using UnityEngine;
using TalesOfTao.Core;
using TalesOfTao.Core.Commands;

namespace TalesOfTao.Sects
{
    /// <summary>
    /// Command to start construction of a building. Deducts resources and adds to BuildQueue.
    /// </summary>
    public class BuildBuildingCommand : Command
    {
        private readonly SectData _sect;
        private readonly BuildQueue _buildQueue;
        private readonly BuildingConfigSO _config;
        private readonly int _tier;

        public BuildBuildingCommand(SectData sect, BuildQueue buildQueue, BuildingConfigSO config, int tier)
        {
            _sect = sect;
            _buildQueue = buildQueue;
            _config = config;
            _tier = tier;
        }

        public override bool CanExecute()
        {
            if (_sect == null || _buildQueue == null || _config == null) return false;
            if (_tier < 1 || _tier > 3) return false;

            // Check if already built or under construction
            if (_buildQueue.IsComplete(_config.BuildingTypeId, _tier))
            {
                Debug.LogWarning($"[BuildBuildingCommand] {_config.BuildingTypeId} T{_tier} already built.");
                return false;
            }

            if (_buildQueue.IsUnderConstruction(_config.BuildingTypeId, _tier))
            {
                Debug.LogWarning($"[BuildBuildingCommand] {_config.BuildingTypeId} T{_tier} already under construction.");
                return false;
            }

            // Check prerequisites
            string prereq = _config.PrerequisiteBuilding;
            if (!string.IsNullOrEmpty(prereq))
            {
                int prereqTier = _config.PrerequisiteTier;
                if (!_sect.HasBuilding(prereq, prereqTier))
                {
                    Debug.LogWarning($"[BuildBuildingCommand] Prerequisite not met: {prereq} T{prereqTier}.");
                    return false;
                }
            }

            // Check resources
            ResourceCost cost = _config.GetTierCost(_tier);
            if (!_sect.Stockpile.CanAfford(cost))
            {
                Debug.LogWarning($"[BuildBuildingCommand] Cannot afford {_config.BuildingTypeId} T{_tier}.");
                return false;
            }

            // Check build queue capacity
            if (!_buildQueue.CanQueue(_config.BuildingTypeId, _tier))
            {
                Debug.LogWarning($"[BuildBuildingCommand] Build queue full.");
                return false;
            }

            return true;
        }

        public override void Execute()
        {
            if (!CanExecute()) return;

            // Deduct resources
            ResourceCost cost = _config.GetTierCost(_tier);
            _sect.Stockpile = _sect.Stockpile - cost;

            // Add to build queue
            int turns = _config.GetBuildTurns(_tier);
            _buildQueue.Enqueue(_config.BuildingTypeId, _tier, turns);

            Debug.Log($"[BuildBuildingCommand] Started construction: {_config.BuildingTypeId} T{_tier} ({turns} turns).");
        }

        public override void Undo()
        {
            // Cancel the build if still in queue
            if (_buildQueue.IsUnderConstruction(_config.BuildingTypeId, _tier))
            {
                var queue = _buildQueue.GetQueue();
                for (int i = 0; i < queue.Length; i++)
                {
                    if (queue[i].BuildingTypeId == _config.BuildingTypeId && queue[i].Tier == _tier)
                    {
                        _buildQueue.Cancel(i);
                        break;
                    }
                }
            }

            // Refund resources
            ResourceCost cost = _config.GetTierCost(_tier);
            _sect.Stockpile.Tael += cost.Tael;
            _sect.Stockpile.Qi += cost.Qi;
            _sect.Stockpile.Lumber += cost.Lumber;
            _sect.Stockpile.IronOre += cost.IronOre;
            _sect.Stockpile.Jade += cost.Jade;
            _sect.Stockpile.MedicinalHerbs += cost.MedicinalHerbs;
            _sect.Stockpile.SpiritHerbs += cost.SpiritHerbs;

            Debug.Log($"[BuildBuildingCommand] Undone: cancelled {_config.BuildingTypeId} T{_tier}, refunded resources");
        }
    }
}
