using System;

namespace TalesOfTao.Hex
{
    [Serializable]
    public class HexTileData
    {
        public HexCoords          Coords        = HexCoords.Zero;
        public TerrainTypeSO      Terrain;
        public ElevationLevel     Elevation     = ElevationLevel.Low;
        public QiDensityLevel     QiDensity     = QiDensityLevel.Sparse;
        public int                CaveCount;
        public DepositType[]      Deposits      = Array.Empty<DepositType>();
        public TileFeature        Feature       = TileFeature.None;
        public ControlState       Control       = ControlState.Unowned;
        public FortificationLevel Fortification = FortificationLevel.None;

        public float MovementCost => Terrain?.MovementCost ?? 1f;
        public float DefenseBonus => Terrain?.DefenseBonus ?? 0f;
        public float QiModifier   => Terrain?.QiModifier   ?? 1f;
        public bool  IsImpassable => Terrain?.IsImpassable ?? false;
        public bool  IsLeyLine    => QiDensity == QiDensityLevel.LeyLine;
    }
}
