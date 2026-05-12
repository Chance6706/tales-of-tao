namespace TalesOfTao.Hex
{
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
        Low,
        Medium,
        High,
        Summit,
    }

    public enum QiDensityLevel
    {
        None,
        Sparse,
        Moderate,
        Dense,
        LeyLine,
    }

    public enum DepositType
    {
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
        None,
        AncientRuins,
        HotSpring,
        SpiritVein,
        BanditCamp,
        WanderingMaster,
    }

    public enum ControlState
    {
        Unowned,
        SectTerritory,
        SettlementInfluence,
        Contested,
    }

    public enum FortificationLevel
    {
        None,
        Watchtower,
        Garrison,
        Fortress,
    }
}
