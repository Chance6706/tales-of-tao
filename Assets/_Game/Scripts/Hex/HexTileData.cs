using System;

namespace TalesOfTao.Hex
{
    // Runtime state for one hex tile. Plain C# class — no MonoBehaviour overhead.
    // Serialized into GameStateDTO for save/load (Phase 18).
    [Serializable]
    public class HexTileData
    {
        public HexCoords       Coords        = HexCoords.Zero;
        public TerrainTypeSO   Terrain;                              // null until terrain is assigned
        public ElevationLevel  Elevation     = ElevationLevel.Low;
        public QiDensityLevel  QiDensity     = QiDensityLevel.Sparse;
        public int             CaveCount;
        public DepositType[]   Deposits      = Array.Empty<DepositType>();
        public TileFeature     Feature       = TileFeature.None;
        public ControlState    Control       = ControlState.Unowned;
        public FortificationLevel Fortification = FortificationLevel.None;

        // ── Derived helpers ───────────────────────────────────────────────────

        public float MovementCost  => Terrain?.MovementCost  ?? 1f;
        public float DefenseBonus  => Terrain?.DefenseBonus  ?? 0f;
        public float QiModifier    => Terrain?.QiModifier    ?? 1f;
        public bool  IsImpassable  => Terrain?.IsImpassable  ?? false;
        public bool  IsLeyLine     => QiDensity == QiDensityLevel.LeyLine;
    }
}
