using System;
using System.Collections.Generic;
using UnityEngine;

namespace TalesOfTao.Sects
{
    /// <summary>
    /// Permanent bonuses baked into a sect at founding time.
    /// Never recalculated — represents lasting advantage of founding location.
    /// </summary>
    [Serializable]
    public struct FoundingTileStats
    {
        public float BaseQiIncome;
        public float CaveBonus;
        public float TerrainDefenseBonus;
    }

    /// <summary>
    /// Resource stockpile for a sect.
    /// </summary>
    [Serializable]
    public struct ResourceStockpile
    {
        public int Tael;
        public int Qi;
        public int Lumber;
        public int IronOre;
        public int Jade;
        public int MedicinalHerbs;
        public int SpiritHerbs;
        public int TeaLeaves;

        public bool CanAfford(ResourceCost cost)
        {
            return Tael >= cost.Tael
                && Qi >= cost.Qi
                && Lumber >= cost.Lumber
                && IronOre >= cost.IronOre
                && Jade >= cost.Jade
                && MedicinalHerbs >= cost.MedicinalHerbs
                && SpiritHerbs >= cost.SpiritHerbs;
        }

        public static ResourceStockpile operator -(ResourceStockpile a, ResourceCost b)
        {
            return new ResourceStockpile
            {
                Tael = a.Tael - b.Tael,
                Qi = a.Qi - b.Qi,
                Lumber = a.Lumber - b.Lumber,
                IronOre = a.IronOre - b.IronOre,
                Jade = a.Jade - b.Jade,
                MedicinalHerbs = a.MedicinalHerbs - b.MedicinalHerbs,
                SpiritHerbs = a.SpiritHerbs - b.SpiritHerbs,
                TeaLeaves = a.TeaLeaves
            };
        }
    }

    /// <summary>
    /// Tracks a constructed building instance.
    /// </summary>
    [Serializable]
    public struct BuildingRecord
    {
        public string BuildingTypeId;
        public int Tier;
        public Vector3 Position;
    }

    /// <summary>
    /// Runtime state for a player's sect. Created by FoundSectCommand.
    /// </summary>
    [Serializable]
    public class SectData
    {
        public string SectName;
        public SectConfigSO Config;
        public FoundingTileStats FoundingStats;
        public ResourceStockpile Stockpile;
        public int DissentLevel;
        public bool IsFounded;

        // Disciple tracking
        [SerializeField] private List<DiscipleData> _disciples = new();

        // Building tracking
        [SerializeField] private List<BuildingRecord> _buildings = new();

        // Founding tile coords
        public int FoundingTileQ;
        public int FoundingTileR;

        public int TotalDisciples => _disciples != null ? _disciples.Count : 0;

        /// <summary>
        /// Gets the count of disciples at a specific rank.
        /// </summary>
        public int GetDiscipleCount(DiscipleRank rank)
        {
            if (_disciples == null) return 0;
            int count = 0;
            foreach (var d in _disciples)
            {
                if (d.Rank == rank && d.IsAlive) count++;
            }
            return count;
        }

        /// <summary>
        /// Adds a disciple to the sect.
        /// </summary>
        public void AddDisciple(DiscipleData disciple)
        {
            _disciples ??= new List<DiscipleData>();
            _disciples.Add(disciple);
        }

        /// <summary>
        /// Gets all disciples (for UI display).
        /// </summary>
        public List<DiscipleData> GetDisciples()
        {
            return _disciples ?? new List<DiscipleData>();
        }

        /// <summary>
        /// Finds a disciple by name.
        /// </summary>
        public DiscipleData FindDisciple(string name)
        {
            if (_disciples == null) return null;
            foreach (var d in _disciples)
            {
                if (d.Name == name && d.IsAlive) return d;
            }
            return null;
        }

        /// <summary>
        /// Promotes a disciple to the next rank.
        /// </summary>
        public bool PromoteDisciple(string name)
        {
            if (_disciples == null) return false;
            for (int i = 0; i < _disciples.Count; i++)
            {
                if (_disciples[i].Name == name && _disciples[i].IsAlive)
                {
                    var disciple = _disciples[i];
                    if (disciple.TryPromote(Config))
                    {
                        _disciples[i] = disciple;
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes a disciple by name.
        /// </summary>
        public bool RemoveDisciple(string name)
        {
            if (_disciples == null) return false;
            for (int i = 0; i < _disciples.Count; i++)
            {
                if (_disciples[i].Name == name)
                {
                    _disciples.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds a building record.
        /// </summary>
        public void AddBuilding(string buildingTypeId, int tier, Vector3 position)
        {
            _buildings ??= new List<BuildingRecord>();
            _buildings.Add(new BuildingRecord
            {
                BuildingTypeId = buildingTypeId,
                Tier = tier,
                Position = position
            });
        }

        /// <summary>
        /// Checks if a building of the given type/tier exists.
        /// </summary>
        public bool HasBuilding(string buildingTypeId, int tier)
        {
            if (_buildings == null) return false;
            foreach (var b in _buildings)
            {
                if (b.BuildingTypeId == buildingTypeId && b.Tier >= tier) return true;
            }
            return false;
        }

        /// <summary>
        /// Gets all building records.
        /// </summary>
        public List<BuildingRecord> GetBuildings()
        {
            return _buildings ?? new List<BuildingRecord>();
        }

        /// <summary>
        /// Calculates total upkeep per turn based on disciple counts.
        /// </summary>
        public int CalculateUpkeep()
        {
            return GetDiscipleCount(DiscipleRank.Peon) * 1
                 + GetDiscipleCount(DiscipleRank.OuterDisciple) * 3
                 + GetDiscipleCount(DiscipleRank.InnerDisciple) * 8
                 + GetDiscipleCount(DiscipleRank.Elder) * 20
                 + GetDiscipleCount(DiscipleRank.HighElder) * 50;
        }

        /// <summary>
        /// Checks if recruiting a disciple of the given rank would exceed the 1:10 management ratio.
        /// </summary>
        public bool WouldExceedRatio(int rank)
        {
            return rank switch
            {
                0 => (GetDiscipleCount(DiscipleRank.Peon) + 1) > GetDiscipleCount(DiscipleRank.OuterDisciple) * 10,
                1 => (GetDiscipleCount(DiscipleRank.OuterDisciple) + 1) > GetDiscipleCount(DiscipleRank.InnerDisciple) * 10,
                2 => (GetDiscipleCount(DiscipleRank.InnerDisciple) + 1) > GetDiscipleCount(DiscipleRank.Elder) * 10,
                3 => (GetDiscipleCount(DiscipleRank.Elder) + 1) > GetDiscipleCount(DiscipleRank.HighElder) * 10,
                _ => false
            };
        }

        /// <summary>
        /// Gets the Dissent accumulation rate based on current ratio violations.
        /// </summary>
        public int CalculateDissentRate()
        {
            int rate = 0;
            int peonCount = GetDiscipleCount(DiscipleRank.Peon);
            int outerCount = GetDiscipleCount(DiscipleRank.OuterDisciple);
            int innerCount = GetDiscipleCount(DiscipleRank.InnerDisciple);
            int elderCount = GetDiscipleCount(DiscipleRank.Elder);
            int highElderCount = GetDiscipleCount(DiscipleRank.HighElder);

            if (outerCount > 0 && peonCount > outerCount * 10)
                rate += Mathf.Max(0, (peonCount - outerCount * 10) / Mathf.Max(1, outerCount) * 2);
            if (innerCount > 0 && outerCount > innerCount * 10)
                rate += Mathf.Max(0, (outerCount - innerCount * 10) / Mathf.Max(1, innerCount) * 2);
            if (elderCount > 0 && innerCount > elderCount * 10)
                rate += Mathf.Max(0, (innerCount - elderCount * 10) / Mathf.Max(1, elderCount) * 2);
            if (highElderCount > 0 && elderCount > highElderCount * 10)
                rate += Mathf.Max(0, (elderCount - highElderCount * 10) / Mathf.Max(1, highElderCount) * 2);
            return rate;
        }
    }
}
