using UnityEngine;
using TalesOfTao.Hex;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// Stores preset map data for non-procedural maps.
    /// Each preset defines terrain types, elevation, and special features for every tile.
    /// </summary>
    [CreateAssetMenu(menuName = "TalesOfTao/Maps/Preset Map Data", fileName = "PresetMap_New")]
    public class PresetMapData : ScriptableObject
    {
        [Header("Map Identity")]
        public string MapName = "New Preset Map";
        public string Description = "";
        public MapSize MapSize = MapSize.Medium;

        [Header("Terrain Data")]
        [Tooltip("Flat array of terrain types, row-major order. Length must equal Width*Height.")]
        public TerrainType[] TerrainTypes;

        [Tooltip("Flat array of elevation levels, row-major order.")]
        public ElevationLevel[] Elevations;

        [Tooltip("Flat array of Qi density levels, row-major order.")]
        public QiDensityLevel[] QiDensities;

        [Header("Special Features")]
        public SacredPeakLocation[] SacredPeaks;
        public SettlementLocation[] Settlements;
        public StartingLocation[] StartingLocations;

        [Header("Resources")]
        public ResourceDeposit[] ResourceDeposits;

        /// <summary>
        /// Validates that all arrays match the expected map dimensions.
        /// </summary>
        public bool Validate()
        {
            int expectedTiles = GetWidth(MapSize) * GetHeight(MapSize);

            if (TerrainTypes != null && TerrainTypes.Length != expectedTiles)
            {
                Debug.LogError($"[PresetMap] {MapName}: TerrainTypes length ({TerrainTypes.Length}) != expected ({expectedTiles})");
                return false;
            }
            if (Elevations != null && Elevations.Length != expectedTiles)
            {
                Debug.LogError($"[PresetMap] {MapName}: Elevations length ({Elevations.Length}) != expected ({expectedTiles})");
                return false;
            }
            if (QiDensities != null && QiDensities.Length != expectedTiles)
            {
                Debug.LogError($"[PresetMap] {MapName}: QiDensities length ({QiDensities.Length}) != expected ({expectedTiles})");
                return false;
            }

            return true;
        }

        public static int GetWidth(MapSize size) => size switch
        {
            MapSize.Small => 60,
            MapSize.Medium => 80,
            MapSize.Large => 120,
            MapSize.Epic => 160,
            _ => 80
        };

        public static int GetHeight(MapSize size) => size switch
        {
            MapSize.Small => 40,
            MapSize.Medium => 60,
            MapSize.Large => 80,
            MapSize.Epic => 100,
            _ => 60
        };
    }

    [System.Serializable]
    public struct SacredPeakLocation
    {
        public int Q;
        public int R;
        public string Name; // e.g., "Mount Hua", "Wudang Shan"
    }

    [System.Serializable]
    public struct SettlementLocation
    {
        public int Q;
        public int R;
        public string Name;
        public int InitialTrust; // 0-100
    }

    [System.Serializable]
    public struct StartingLocation
    {
        public int Q;
        public int R;
        public int PlayerIndex; // 0 = player, 1+ = AI
    }

    [System.Serializable]
    public struct ResourceDeposit
    {
        public int Q;
        public int R;
        public DepositType Type;
        public int Abundance; // 1-3
    }
}
