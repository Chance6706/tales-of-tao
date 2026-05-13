using System;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// Runtime data for a single hex tile. This is a mutable data class — not a MonoBehaviour.
    /// All properties have private setters to enforce that only HexGridManager modifies tile data
    /// during generation, and only game systems (not arbitrary code) modify it during gameplay.
    /// </summary>
    [Serializable]
    public class HexTileData
    {
        public HexCoords Coords { get; set; } = HexCoords.Zero;
        public TerrainTypeSO Terrain { get; set; }
        public ElevationLevel Elevation { get; set; } = ElevationLevel.Low;
        public QiDensityLevel QiDensity { get; set; } = QiDensityLevel.Sparse;
        public int CaveCount { get; set; }
        public DepositType[] Deposits { get; set; } = Array.Empty<DepositType>();
        public TileFeature Feature { get; set; } = TileFeature.None;
        public ControlState Control { get; set; } = ControlState.Unowned;
        public FortificationLevel Fortification { get; set; } = FortificationLevel.None;

        /// <summary>True if this tile lies within 1 hex of a generated Ley Line path.</summary>
        public bool IsLeyLine => QiDensity == QiDensityLevel.LeyLine;

        public float MovementCost => Terrain?.MovementCost ?? 1f;
        public float DefenseBonus => Terrain?.DefenseBonus ?? 0f;
        public float QiModifier   => Terrain?.QiModifier   ?? 1f;
        public bool  IsImpassable  => Terrain?.IsImpassable ?? false;
    }
}
