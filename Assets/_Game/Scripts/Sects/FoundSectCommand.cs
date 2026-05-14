using UnityEngine;
using TalesOfTao.Core.Commands;
using TalesOfTao.Hex;

namespace TalesOfTao.Sects
{
    /// <summary>
    /// Undoable command that founds a sect on the given tile.
    /// Creates SectData, places Temple, removes Founder from roster.
    /// </summary>
    public class FoundSectCommand : Command
    {
        private readonly SectConfigSO _config;
        private readonly int _tileQ;
        private readonly int _tileR;
        private readonly HexGridManager _gridManager;
        private SectData _createdData;
        private bool _executed;

        public SectData CreatedData => _createdData;

        public FoundSectCommand(SectConfigSO config, int tileQ, int tileR, HexGridManager gridManager)
        {
            _config = config;
            _tileQ = tileQ;
            _tileR = tileR;
            _gridManager = gridManager;
        }

        public override bool CanExecute()
        {
            if (_config == null || _gridManager == null) return false;

            var tile = _gridManager.GetTile(_tileQ, _tileR);
            if (tile == null) return false;
            if (tile.IsImpassable) return false;
            if (tile.Terrain?.Type == TerrainType.Lake) return false;

            return true;
        }

        public override void Execute()
        {
            if (!CanExecute())
            {
                Debug.LogError("[FoundSectCommand] Cannot execute: invalid tile or missing data.");
                return;
            }

            var tile = _gridManager.GetTile(_tileQ, _tileR);

            // Create SectData with founding bonuses baked in
            _createdData = new SectData
            {
                SectName = _config.DisplayName,
                Config = _config,
                IsFounded = true,
                FoundingTileQ = _tileQ,
                FoundingTileR = _tileR,
                FoundingStats = new FoundingTileStats
                {
                    BaseQiIncome = GetQiDensityValue(tile.QiDensity) * 10f,
                    CaveBonus = tile.CaveCount * 2f,
                    TerrainDefenseBonus = tile.Terrain?.DefenseBonus ?? 0f,
                },
                Stockpile = new ResourceStockpile
                {
                    Tael = _config.StartingTael,
                    Qi = _config.StartingQi,
                },
                PeonCount = _config.StartingPeons,
                DissentLevel = 0,
            };

            // Mark tile as sect territory
            tile.Control = ControlState.SectTerritory;

            // TODO: Place Temple building on tile (uses BuildingController)
            // TODO: Remove Founder unit from roster

            _executed = true;
            Debug.Log($"[FoundSectCommand] Sect '{_config.DisplayName}' founded at ({_tileQ},{_tileR}). " +
                      $"BaseQi={_createdData.FoundingStats.BaseQiIncome}, CaveBonus={_createdData.FoundingStats.CaveBonus}");
        }

        public override void Undo()
        {
            if (!_executed || _createdData == null) return;

            var tile = _gridManager.GetTile(_tileQ, _tileR);
            if (tile != null)
            {
                tile.Control = ControlState.Unowned;
            }

            _createdData.IsFounded = false;
            _executed = false;
            Debug.Log($"[FoundSectCommand] Undone: Sect '{_config.DisplayName}' removed from ({_tileQ},{_tileR})");
        }

        private static float GetQiDensityValue(QiDensityLevel density)
        {
            return density switch
            {
                QiDensityLevel.None => 0f,
                QiDensityLevel.Sparse => 5f,
                QiDensityLevel.Moderate => 10f,
                QiDensityLevel.Dense => 15f,
                QiDensityLevel.LeyLine => 25f,
                _ => 0f,
            };
        }
    }
}
