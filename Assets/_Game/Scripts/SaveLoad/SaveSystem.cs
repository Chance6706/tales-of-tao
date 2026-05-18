using System;
using System.Collections.Generic;
using UnityEngine;
using TalesOfTao.Core;
using TalesOfTao.Core.TurnSystem;
using TalesOfTao.Sects;
using TalesOfTao.Hex;
using TalesOfTao.Units;

namespace TalesOfTao.SaveLoad
{
    /// <summary>
    /// MonoBehaviour that manages save/load operations and autosave.
    /// Attach to a persistent GameObject in the scene.
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool _autosaveEnabled = true;
        [SerializeField] private int _autosaveInterval = SaveConstants.AutosaveIntervalTurns;

        private TurnCoordinator _turnCoordinator;
        private HexGridManager _gridManager;
        private int _lastAutosaveTurn;

        public bool AutosaveEnabled
        {
            get => _autosaveEnabled;
            set => _autosaveEnabled = value;
        }

        public event Action<bool> OnSaveCompleted;
        public event Action<SaveData> OnLoadCompleted;

        private void Awake()
        {
            _lastAutosaveTurn = 0;
        }

        private void OnEnable()
        {
            if (_turnCoordinator != null)
                _turnCoordinator.OnTurnStarted += HandleTurnStarted;
        }

        private void OnDisable()
        {
            if (_turnCoordinator != null)
                _turnCoordinator.OnTurnStarted -= HandleTurnStarted;
        }

        /// <summary>
        /// Initialize references to required systems. Call after all systems are created.
        /// </summary>
        public void Initialize(TurnCoordinator turnCoordinator, HexGridManager gridManager)
        {
            _turnCoordinator = turnCoordinator;
            _gridManager = gridManager;

            if (_turnCoordinator != null)
                _turnCoordinator.OnTurnStarted += HandleTurnStarted;
        }

        #region Public API

        /// <summary>
        /// Saves the current game state to the specified slot.
        /// </summary>
        public bool SaveGame(int slot)
        {
            SaveData data = CaptureGameState();
            bool success = SaveManager.Save(slot, data);
            OnSaveCompleted?.Invoke(success);
            return success;
        }

        /// <summary>
        /// Loads game state from the specified slot.
        /// </summary>
        public bool LoadGame(int slot)
        {
            SaveData data = SaveManager.Load(slot);
            if (data == null) return false;

            ApplyGameState(data);
            OnLoadCompleted?.Invoke(data);
            return true;
        }

        /// <summary>
        /// Checks if a save exists for the given slot.
        /// </summary>
        public bool HasSave(int slot) => SaveManager.SaveExists(slot);

        /// <summary>
        /// Deletes a save slot.
        /// </summary>
        public bool DeleteSave(int slot) => SaveManager.DeleteSave(slot);

        /// <summary>
        /// Gets metadata for all existing save slots.
        /// </summary>
        public List<SaveSlotInfo> GetSaveInfos() => SaveManager.GetSaveSlotInfos();

        #endregion

        #region Game State Capture

        /// <summary>
        /// Captures the complete current game state into a SaveData object.
        /// This is the serialization boundary — all runtime state must be converted to serializable DTOs here.
        /// </summary>
        public SaveData CaptureGameState()
        {
            var data = new SaveData();

            // Turn state
            if (_turnCoordinator != null)
            {
                data.turnNumber = _turnCoordinator.TurnNumber;
                data.currentPhase = (int)_turnCoordinator.CurrentPhase;
            }

            // Map state
            if (_gridManager != null && _gridManager.IsGenerated)
            {
                data.mapWidth = _gridManager.Width;
                data.mapHeight = _gridManager.Height;

                var allTiles = _gridManager.GetAllTiles();
                for (int i = 0; i < allTiles.Count; i++)
                {
                    var tile = allTiles[i];
                    data.tiles.Add(new TileSaveData
                    {
                        x = tile.Coords.X,
                        z = tile.Coords.Z,
                        terrainTypeIndex = (int)(tile.Terrain?.Type ?? 0),
                        elevation = (int)tile.Elevation,
                        qiDensity = (int)tile.QiDensity,
                        caveType = tile.CaveCount,
                        feature = (int)tile.Feature,
                        controlState = (int)tile.Control,
                        fortificationLevel = (int)tile.Fortification,
                        ownerSectId = tile.Control == ControlState.SectTerritory ? 0 : -1,
                        hasRoad = false // TODO: road system in Phase 2
                    });
                }
            }

            // Game state defaults
            data.gameState.zodiacYear = 1; // TODO: wire ZodiacCalendar
            data.gameState.globalEventSeed = 0; // TODO: wire EventSystem

            return data;
        }

        #endregion

        #region Game State Apply

        /// <summary>
        /// Applies loaded SaveData to the current game state.
        /// This is the deserialization boundary — all DTOs must be converted back to runtime state here.
        /// Note: Full implementation requires regenerating the map from saved tile data and
        /// restoring all runtime systems. For now, this provides the framework.
        /// </summary>
        public void ApplyGameState(SaveData data)
        {
            if (data == null) return;

            Debug.Log($"[SaveSystem] Applying save data (turn {data.turnNumber}, {data.tiles.Count} tiles, {data.sects.Count} sects)");

            // Map state — regenerate from saved dimensions and tile data
            if (_gridManager != null && data.tiles.Count > 0)
            {
                ApplyTileData(data);
            }

            // TODO: Restore turn state (requires TurnCoordinator setters)
            // TODO: Restore sect state (requires SectManager loader)
            // TODO: Restore unit state
            // TODO: Restore formation state
        }

        private void ApplyTileData(SaveData data)
        {
            // Regenerate the map with the saved dimensions
            // HexGridManager.GenerateMap uses _mapSize enum, not raw dimensions
            // For now, log what would need to happen
            Debug.Log($"[SaveSystem] Would restore {data.mapWidth}x{data.mapHeight} map with {data.tiles.Count} tiles");

            // Full implementation: iterate saved tiles and apply terrain/elevation/qi data
            // This requires either:
            //   a) A HexGridManager.LoadFromSave(TileSaveData[]) method, or
            //   b) Regenerating with seed + applying overrides
        }

        #endregion

        #region Autosave

        private void HandleTurnStarted(int turnNumber)
        {
            if (!_autosaveEnabled) return;
            if (turnNumber - _lastAutosaveTurn < _autosaveInterval) return;

            _lastAutosaveTurn = turnNumber;
            bool success = SaveGame(SaveConstants.AutosaveSlot);

            if (success)
                Debug.Log($"[SaveSystem] Autosave complete (turn {turnNumber})");
            else
                Debug.LogWarning($"[SaveSystem] Autosave failed (turn {turnNumber})");
        }

        #endregion
    }
}
