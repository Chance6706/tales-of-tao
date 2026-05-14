using System;

namespace TalesOfTao.Core.TurnSystem
{
    /// <summary>
    /// Data struct broadcast when zodiac bonuses change.
    /// All subscribing systems read from this for the duration of the turn.
    /// </summary>
    public readonly struct ZodiacBonuses
    {
        public readonly int Year;           // 1-12
        public readonly string Animal;      // e.g. "Dragon"
        public readonly float TaelIncomeMultiplier;
        public readonly float QiIncomeMultiplier;
        public readonly float CombatStatMultiplier;
        public readonly float ResearchSpeedMultiplier;
        public readonly float RenownMultiplier;
        public readonly float BuildingSpeedMultiplier;
        public readonly float TradeRouteMultiplier;
        public readonly float HerbYieldMultiplier;
        public readonly float DefenseMultiplier;
        public readonly float TrustGainMultiplier;
        public readonly float DissentRecoveryMultiplier;
        public readonly float MovementBonus;
        public readonly float SpyDetectionModifier;
        public readonly float TribulationFailureModifier;

        public static ZodiacBonuses ForYear(int year)
        {
            int idx = ((year - 1) % 12 + 12) % 12; // Normalize to 0-11
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
