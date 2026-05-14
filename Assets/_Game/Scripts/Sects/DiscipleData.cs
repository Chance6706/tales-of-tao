using System;
using UnityEngine;

namespace TalesOfTao.Sects
{
    /// <summary>
    /// Runtime state for a single disciple. Created when a disciple is recruited or promoted.
    /// Stats are calculated from rank, sect config, and traits.
    /// </summary>
    [Serializable]
    public class DiscipleData
    {
        public string Name;
        public DiscipleRank Rank;
        public int HP;
        public int MaxHP;
        public int Attack;
        public int Defense;
        public int Speed;
        public int QiPower;
        public int RootQuality;       // 0-10 cultivation potential
        public string[] Techniques;   // assigned technique IDs
        public string Trait;          // Lucky, Resilient, Perceptive, Reckless, Fragile, or empty
        public string BondedBeast;    // empty if none
        public bool IsAlive = true;

        /// <summary>
        /// Calculates base stats from rank and sect config.
        /// Called on creation and on promotion.
        /// </summary>
        public void CalculateStats(SectConfigSO config)
        {
            // Base stats by rank
            switch (Rank)
            {
                case DiscipleRank.Peon:
                    HP = 20; Attack = 2; Defense = 1; Speed = 2; QiPower = 0;
                    break;
                case DiscipleRank.OuterDisciple:
                    HP = 40; Attack = 6; Defense = 4; Speed = 3; QiPower = 5;
                    break;
                case DiscipleRank.InnerDisciple:
                    HP = 70; Attack = 12; Defense = 8; Speed = 4; QiPower = 15;
                    break;
                case DiscipleRank.Elder:
                    HP = 120; Attack = 20; Defense = 15; Speed = 5; QiPower = 30;
                    break;
                case DiscipleRank.HighElder:
                    HP = 200; Attack = 35; Defense = 25; Speed = 6; QiPower = 60;
                    break;
            }

            MaxHP = HP;

            // Apply sect trait bonuses
            if (config != null)
            {
                switch (config.Trait)
                {
                    case SectTrait.HpBonus: // Shaolin
                        HP = Mathf.RoundToInt(HP * 1.15f);
                        MaxHP = HP;
                        break;
                    case SectTrait.SwordDamage: // Mount Hua — applied in combat calc, not here
                        break;
                }
            }

            // Apply trait bonuses
            ApplyTraitBonuses();

            // Root quality: random 0-10, higher = better cultivation potential
            RootQuality = UnityEngine.Random.Range(0, 11);
        }

        private void ApplyTraitBonuses()
        {
            if (string.IsNullOrEmpty(Trait)) return;

            switch (Trait)
            {
                case "Lucky":
                    // +10% to random stat
                    Speed += 1;
                    break;
                case "Resilient":
                    HP = Mathf.RoundToInt(HP * 1.08f);
                    MaxHP = HP;
                    break;
                case "Perceptive":
                    Attack += 1;
                    break;
                case "Reckless":
                    Attack += 2;
                    Defense -= 1;
                    break;
                case "Fragile":
                    HP = Mathf.RoundToInt(HP * 0.92f);
                    MaxHP = HP;
                    break;
            }
        }

        /// <summary>
        /// Promotes this disciple to the next rank. Returns false if already HighElder.
        /// </summary>
        public bool TryPromote(SectConfigSO config)
        {
            if (Rank == DiscipleRank.HighElder) return false;
            Rank = (DiscipleRank)((int)Rank + 1);
            CalculateStats(config);
            return true;
        }

        /// <summary>
        /// Generates a procedural Chinese-style name.
        /// </summary>
        public static string GenerateName()
        {
            string[] surnames = { "Li", "Wang", "Zhang", "Liu", "Chen", "Yang", "Zhao", "Huang", "Zhou", "Wu", "Xu", "Sun", "Ma", "Zhu", "Hu" };
            string[] givenNames = { "Wei", "Fang", "Jing", "Ming", "Hao", "Xin", "Yun", "Feng", "Lei", "Tao", "Xuan", "Chen", "Long", "Xing", "Rui", "Kai", "Shan", "Ling", "Hua", "Qiang" };
            string surname = surnames[UnityEngine.Random.Range(0, surnames.Length)];
            string given = givenNames[UnityEngine.Random.Range(0, givenNames.Length)];
            return $"{surname} {given}";
        }
    }
}
