using System;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// Runtime data for a single hex tile. This is a mutable data class — not a MonoBehaviour.
    /// Properties with private setters can only be modified by HexGridManager during generation.
    /// Gameplay systems should only mutate state through validated pathways.
    /// </summary>
    [Serializable]
    public class HexTileData
    {
        public HexCoords Coords { get; internal set; } = HexCoords.Zero;
        public TerrainTypeSO Terrain { get; internal set; }
        public ElevationLevel Elevation { get; internal set; } = ElevationLevel.Low;
        public QiDensityLevel QiDensity { get; internal set; } = QiDensityLevel.Sparse;
        public int CaveCount { get; internal set; }
        public DepositType[] Deposits { get; internal set; } = Array.Empty<DepositType>();
        public TileFeature Feature { get; internal set; } = TileFeature.None;
        public ControlState Control { get; set; } = ControlState.Unowned;
        public FortificationLevel Fortification { get; internal set; } = FortificationLevel.None;

        /// <summary>True if this tile lies within 1 hex of a generated Ley Line path.</summary>
        public bool IsLeyLine => QiDensity == QiDensityLevel.LeyLine;

        public float MovementCost => Terrain?.MovementCost ?? 1f;
        public float DefenseBonus => Terrain?.DefenseBonus ?? 0f;
        public float QiModifier   => Terrain?.QiModifier   ?? 1f;
        public bool  IsImpassable  => Terrain?.IsImpassable ?? false;
    }
}
