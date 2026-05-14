using System;

namespace TalesOfTao.Core.TurnSystem
{
    /// <summary>
    /// Data struct broadcast when zodiac bonuses change.
    /// All subscribing systems read from this for the duration of the turn.
    /// </summary>
    public struct ZodiacBonuses
    {
        public int Year;
        public string Animal;
        public float TaelIncomeMultiplier;
        public float QiIncomeMultiplier;
        public float CombatStatMultiplier;
        public float ResearchSpeedMultiplier;
        public float RenownMultiplier;
        public float BuildingSpeedMultiplier;
        public float TradeRouteMultiplier;
        public float HerbYieldMultiplier;
        public float DefenseMultiplier;
        public float TrustGainMultiplier;
        public float DissentRecoveryMultiplier;
        public float MovementBonus;
        public float SpyDetectionModifier;
        public float TribulationFailureModifier;

        public static ZodiacBonuses ForYear(int year)
        {
            int idx = ((year - 1) % 12 + 12) % 12;
            return idx switch
            {
                0  => new ZodiacBonuses { Year = year, Animal = "Rat",     TaelIncomeMultiplier = 1.15f, SpyDetectionModifier = -0.10f },
                1  => new ZodiacBonuses { Year = year, Animal = "Ox",      BuildingSpeedMultiplier = 1.15f },
                2  => new ZodiacBonuses { Year = year, Animal = "Tiger",   CombatStatMultiplier = 1.15f, TribulationFailureModifier = -0.10f },
                3  => new ZodiacBonuses { Year = year, Animal = "Rabbit",  RenownMultiplier = 1.15f },
                4  => new ZodiacBonuses { Year = year, Animal = "Dragon",  QiIncomeMultiplier = 1.20f },
                5  => new ZodiacBonuses { Year = year, Animal = "Snake",   ResearchSpeedMultiplier = 1.10f },
                6  => new ZodiacBonuses { Year = year, Animal = "Horse",   MovementBonus = 1f },
                7  => new ZodiacBonuses { Year = year, Animal = "Goat",    HerbYieldMultiplier = 1.15f },
                8  => new ZodiacBonuses { Year = year, Animal = "Monkey",  ResearchSpeedMultiplier = 1.15f },
                9  => new ZodiacBonuses { Year = year, Animal = "Rooster", DefenseMultiplier = 1.10f, BuildingSpeedMultiplier = 1.15f },
                10 => new ZodiacBonuses { Year = year, Animal = "Dog",     TrustGainMultiplier = 1.15f, DissentRecoveryMultiplier = 2.00f },
                11 => new ZodiacBonuses { Year = year, Animal = "Pig",     TradeRouteMultiplier = 1.20f },
                _  => new ZodiacBonuses { Year = year, Animal = "Unknown" }
            };
        }

        /// <summary>Neutral bonuses (no active zodiac year).</summary>
        public static ZodiacBonuses Neutral => new() { Year = 0, Animal = "None" };
    }
}
