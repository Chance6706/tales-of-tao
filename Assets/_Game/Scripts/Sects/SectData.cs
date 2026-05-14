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

        // Disciple counts by rank
        public int PeonCount;
        public int OuterDiscipleCount;
        public int InnerDiscipleCount;
        public int ElderCount;
        public int HighElderCount;

        // Founding tile coords
        public int FoundingTileQ;
        public int FoundingTileR;

        public int TotalDisciples => PeonCount + OuterDiscipleCount + InnerDiscipleCount + ElderCount + HighElderCount;

        /// <summary>
        /// Calculates total upkeep per turn based on disciple counts.
        /// </summary>
        public int CalculateUpkeep()
        {
            return PeonCount * 1
                 + OuterDiscipleCount * 3
                 + InnerDiscipleCount * 8
                 + ElderCount * 20
                 + HighElderCount * 50;
        }

        /// <summary>
        /// Checks if recruiting a disciple of the given rank would exceed the 1:10 management ratio.
        /// </summary>
        public bool WouldExceedRatio(int rank)
        {
            return rank switch
            {
                0 => (PeonCount + 1) > OuterDiscipleCount * 10,    // Peons per Outer Disciple
                1 => (OuterDiscipleCount + 1) > InnerDiscipleCount * 10,
                2 => (InnerDiscipleCount + 1) > ElderCount * 10,
                3 => (ElderCount + 1) > HighElderCount * 10,
                _ => false
            };
        }

        /// <summary>
        /// Gets the Dissent accumulation rate based on current ratio violations.
        /// </summary>
        public int CalculateDissentRate()
        {
            int rate = 0;
            if (PeonCount > OuterDiscipleCount * 10)
                rate += Mathf.Max(0, (PeonCount - OuterDiscipleCount * 10) / (OuterDiscipleCount * 10 / 10) * 2);
            if (OuterDiscipleCount > InnerDiscipleCount * 10)
                rate += Mathf.Max(0, (OuterDiscipleCount - InnerDiscipleCount * 10) / (InnerDiscipleCount * 10 / 10) * 2);
            if (InnerDiscipleCount > ElderCount * 10)
                rate += Mathf.Max(0, (InnerDiscipleCount - ElderCount * 10) / (ElderCount * 10 / 10) * 2);
            if (ElderCount > HighElderCount * 10)
                rate += Mathf.Max(0, (ElderCount - HighElderCount * 10) / (HighElderCount * 10 / 10) * 2);
            return rate;
        }
    }
}
