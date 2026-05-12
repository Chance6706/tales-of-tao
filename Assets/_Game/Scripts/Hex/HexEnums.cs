namespace TalesOfTao.Hex
{
    // Explicit integer values required — reordering without them silently
    // corrupts serialized save data that stores enum values as ints.

    public enum TerrainType
    {
        Plains     = 0,
        Mountain   = 1,
        Forest     = 2,
        River      = 3,
        Lake       = 4,
        Desert     = 5,
        Swamp      = 6,
        SacredPeak = 7,
    // All enumerations that describe the static and dynamic state of a hex tile.
    // Kept in one file so the GDD tables map 1:1 to code with no hunting.

    public enum TerrainType
    {
        Plains,
        Mountain,
        Forest,
        River,
        Lake,
        Desert,
        Swamp,
        SacredPeak,
    }

    public enum ElevationLevel
    {
        Low    = 0,
        Medium = 1,
        High   = 2,
        Summit = 3,
        Low,
        Medium,
        High,
        Summit,
    }

    public enum QiDensityLevel
    {
        None     = 0,
        Sparse   = 1,
        Moderate = 2,
        Dense    = 3,
        LeyLine  = 4,
        None,
        Sparse,
        Moderate,
        Dense,
        LeyLine,
    }

    public enum DepositType
    {
        None           = 0,
        IronOre        = 1,
        Jade           = 2,
        MedicinalHerbs = 3,
        SpiritHerbs    = 4,
        TeaLeaves      = 5,
        Horses         = 6,
        Lumber         = 7,
        RareEarth      = 8,
        None,
        IronOre,
        Jade,
        MedicinalHerbs,
        SpiritHerbs,
        TeaLeaves,
        Horses,
        Lumber,
        RareEarth,
    }

    public enum TileFeature
    {
        None            = 0,
        AncientRuins    = 1,
        HotSpring       = 2,
        SpiritVein      = 3,
        BanditCamp      = 4,
        WanderingMaster = 5,
        None,
        AncientRuins,
        HotSpring,
        SpiritVein,
        BanditCamp,
        WanderingMaster,
    }

    public enum ControlState
    {
        Unowned             = 0,
        SectTerritory       = 1,
        SettlementInfluence = 2,
        Contested           = 3,
        Unowned,
        SectTerritory,
        SettlementInfluence,
        Contested,
    }

    public enum FortificationLevel
    {
        None       = 0,
        Watchtower = 1,
        Garrison   = 2,
        Fortress   = 3,
        None,
        Watchtower,
        Garrison,
        Fortress,
    }
}
