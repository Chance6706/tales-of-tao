using UnityEngine;

namespace TalesOfTao.Sects
{
    /// <summary>
    /// Combat/magic affinity for a sect. Determines starting technique pool.
    /// </summary>
    public enum SectAffinity
    {
        InternalQi = 0,    // Taiji (Wu Dang)
        BodyCultivation = 1, // Body (Shaolin)
        Poison = 2,         // Hidden Weapons (Tang Clan)
        SwordArts = 3,      // Sword (Mount Hua)
        Balanced = 4,       // Sword + Qi (Emei)
        IceElemental = 5,   // Ice/Qi (Kunlun)
        Movement = 6,       // Lightness (Peng Clan)
        NobleArts = 7,      // Sword/Noble (Namgung)
        Forbidden = 8,      // Forbidden Arts (Demonic Cult)
        Imperial = 9,       // Imperial Authority (Imperial Palace)
    }

    /// <summary>
    /// Passive trait that defines a sect's strategic identity.
    /// </summary>
    public enum SectTrait
    {
        None = 0,
        QiBonus = 1,           // +20% Qi income (Wu Dang)
        HpBonus = 2,           // +15% disciple HP (Shaolin)
        PoisonImmunity = 3,    // +20% assassination, immune to poison (Tang Clan)
        SwordDamage = 4,       // +10% sword technique damage (Mount Hua)
        RenownDiplomacy = 5,   // +15% Renown from diplomacy (Emei)
        MountainDefense = 6,   // +20% defense on Mountain/Sacred Peak (Kunlun)
        MovementBonus = 7,     // +2 base movement (Peng Clan)
        TaelBonus = 8,         // +10% Tael income (Namgung)
        Sacrifice = 9,         // Can sacrifice disciples for Qi (Demonic Cult)
        ImperialTax = 10,      // Can levy settlement tax (Imperial Palace)
    }

    /// <summary>
    /// Resource/commodity types for sect stockpiles.
    /// </summary>
    public enum ResourceType
    {
        None = 0,
        Tael = 1,          // Currency
        Qi = 2,            // Cultivation energy
        Lumber = 3,
        IronOre = 4,
        Jade = 5,
        MedicinalHerbs = 6,
        SpiritHerbs = 7,
        TeaLeaves = 8,
    }
}
