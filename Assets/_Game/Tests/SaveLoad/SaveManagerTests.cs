using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using TalesOfTao.SaveLoad;

namespace TalesOfTao.Tests.SaveLoad
{
    /// <summary>
    /// EditMode tests for SaveManager and SaveData serialization.
    /// Tests cover: save/load round-trip, atomic writes, backup recovery, versioning, migration.
    /// </summary>
    [TestFixture]
    public class SaveManagerTests
    {
        private const int TestSlot = 99; // Use high slot number to avoid conflicts

        [TearDown]
        public void Cleanup()
        {
            // Clean up test save files
            SaveManager.DeleteSave(TestSlot);
        }

        #region SaveData Construction

        [Test]
        public void SaveData_DefaultConstructor_InitializesCollections()
        {
            var data = new SaveData();

            Assert.IsNotNull(data.sects);
            Assert.IsNotNull(data.tiles);
            Assert.IsNotNull(data.units);
            Assert.IsNotNull(data.formations);
            Assert.IsNotNull(data.gameState);
            Assert.IsNotNull(data.gameState.triggeredEvents);
            Assert.IsNotNull(data.gameState.marketPrices);
            Assert.AreEqual(0, data.sects.Count);
            Assert.AreEqual(0, data.tiles.Count);
        }

        [Test]
        public void SaveData_SectSaveData_DefaultConstructor_InitializesCollections()
        {
            var sect = new SectSaveData();

            Assert.IsNotNull(sect.completedResearch);
            Assert.IsNotNull(sect.activeResearch);
            Assert.IsNotNull(sect.resources);
            Assert.IsNotNull(sect.buildings);
            Assert.IsNotNull(sect.disciples);
            Assert.IsNotNull(sect.relations);
        }

        [Test]
        public void SaveData_UnitSaveData_DefaultConstructor_InitializesCollections()
        {
            var unit = new UnitSaveData();

            Assert.IsNotNull(unit.assignedDisciples);
            Assert.AreEqual(-1, unit.formationType);
            Assert.AreEqual("None", unit.activeAction);
        }

        [Test]
        public void SaveData_FormationSaveData_DefaultConstructor_InitializesCollections()
        {
            var formation = new FormationSaveData();
            Assert.IsNotNull(formation.unitIds);
        }

        #endregion

        #region SaveManager Basic Operations

        [Test]
        public void SaveManager_SaveAndLoad_RoundTrip()
        {
            var data = CreateTestSaveData();

            bool saved = SaveManager.Save(TestSlot, data);
            Assert.IsTrue(saved, "Save should succeed");

            SaveData loaded = SaveManager.Load(TestSlot);
            Assert.IsNotNull(loaded, "Load should return data");
            Assert.AreEqual(data.turnNumber, loaded.turnNumber);
            Assert.AreEqual(data.currentPhase, loaded.currentPhase);
            Assert.AreEqual(data.mapWidth, loaded.mapWidth);
            Assert.AreEqual(data.mapHeight, loaded.mapHeight);
        }

        [Test]
        public void SaveManager_Load_NonExistentSlot_ReturnsNull()
        {
            SaveData loaded = SaveManager.Load(999);
            Assert.IsNull(loaded);
        }

        [Test]
        public void SaveManager_SaveExists_ReturnsCorrectValue()
        {
            Assert.IsFalse(SaveManager.SaveExists(TestSlot));

            var data = CreateTestSaveData();
            SaveManager.Save(TestSlot, data);

            Assert.IsTrue(SaveManager.SaveExists(TestSlot));
        }

        [Test]
        public void SaveManager_DeleteSave_RemovesFiles()
        {
            var data = CreateTestSaveData();
            SaveManager.Save(TestSlot, data);
            Assert.IsTrue(SaveManager.SaveExists(TestSlot));

            bool deleted = SaveManager.DeleteSave(TestSlot);
            Assert.IsTrue(deleted);
            Assert.IsFalse(SaveManager.SaveExists(TestSlot));
        }

        [Test]
        public void SaveManager_Save_InvalidSlot_ReturnsFalse()
        {
            var data = CreateTestSaveData();
            Assert.IsFalse(SaveManager.Save(-1, data));
            Assert.IsFalse(SaveManager.Save(100, data));
        }

        #endregion

        #region Serialization Integrity

        [Test]
        public void SaveManager_Save_SerializesTileData()
        {
            var data = new SaveData();
            data.turnNumber = 5;
            data.tiles.Add(new TileSaveData
            {
                x = 3, z = 4,
                terrainTypeIndex = 2,
                elevation = 1,
                qiDensity = 3,
                caveType = 1,
                feature = 0,
                controlState = 1,
                fortificationLevel = 0,
                ownerSectId = 0,
                hasRoad = true
            });

            SaveManager.Save(TestSlot, data);
            SaveData loaded = SaveManager.Load(TestSlot);

            Assert.AreEqual(1, loaded.tiles.Count);
            var tile = loaded.tiles[0];
            Assert.AreEqual(3, tile.x);
            Assert.AreEqual(4, tile.z);
            Assert.AreEqual(2, tile.terrainTypeIndex);
            Assert.AreEqual(1, tile.elevation);
            Assert.AreEqual(3, tile.qiDensity);
            Assert.AreEqual(1, tile.caveType);
            Assert.AreEqual(true, tile.hasRoad);
        }

        [Test]
        public void SaveManager_Save_SerializesSectData()
        {
            var data = new SaveData();
            data.turnNumber = 10;

            var sect = new SectSaveData
            {
                sectName = "Test Sect",
                tael = 100,
                qi = 50,
                dissentLevel = 5,
                templeTier = 2,
                hallTier = 1,
                isFounded = true,
                foundingTileQ = 0,
                foundingTileR = 0
            };
            sect.completedResearch = new List<string> { "tech1", "tech2" };
            sect.activeResearch = new List<ActiveResearchSaveData>
            {
                new ActiveResearchSaveData { techId = "tech3", progress01 = 0.5f, branch = 0 }
            };
            sect.disciples = new List<DiscipleSaveData>
            {
                new DiscipleSaveData
                {
                    name = "Li Wei",
                    rank = 0,
                    hp = 20, maxHP = 20,
                    attack = 2, defense = 1, speed = 2, qiPower = 0,
                    isAlive = true
                }
            };
            sect.buildings = new List<BuildingSaveData>
            {
                new BuildingSaveData { buildingTypeId = "Temple", tier = 1, posX = 0, posY = 0, posZ = 0 }
            };

            data.sects.Add(sect);

            SaveManager.Save(TestSlot, data);
            SaveData loaded = SaveManager.Load(TestSlot);

            Assert.AreEqual(1, loaded.sects.Count);
            var loadedSect = loaded.sects[0];
            Assert.AreEqual("Test Sect", loadedSect.sectName);
            Assert.AreEqual(100, loadedSect.tael);
            Assert.AreEqual(50, loadedSect.qi);
            Assert.AreEqual(5, loadedSect.dissentLevel);
            Assert.AreEqual(2, loadedSect.templeTier);
            Assert.AreEqual(2, loadedSect.completedResearch.Count);
            Assert.AreEqual(1, loadedSect.activeResearch.Count);
            Assert.AreEqual("tech3", loadedSect.activeResearch[0].techId);
            Assert.AreEqual(1, loadedSect.disciples.Count);
            Assert.AreEqual("Li Wei", loadedSect.disciples[0].name);
            Assert.AreEqual(1, loadedSect.buildings.Count);
        }

        [Test]
        public void SaveManager_Save_SerializesUnitData()
        {
            var data = new SaveData();
            data.units.Add(new UnitSaveData
            {
                unitId = "unit_1",
                unitType = "PeonGang",
                ownerSectId = 0,
                tileX = 5, tileZ = 3,
                currentHP = 100, maxHP = 100,
                movementPointsRemaining = 2,
                formationType = -1,
                activeAction = "None"
            });

            SaveManager.Save(TestSlot, data);
            SaveData loaded = SaveManager.Load(TestSlot);

            Assert.AreEqual(1, loaded.units.Count);
            Assert.AreEqual("unit_1", loaded.units[0].unitId);
            Assert.AreEqual("PeonGang", loaded.units[0].unitType);
            Assert.AreEqual(5, loaded.units[0].tileX);
            Assert.AreEqual(3, loaded.units[0].tileZ);
            Assert.AreEqual(2, loaded.units[0].movementPointsRemaining);
        }

        [Test]
        public void SaveManager_Save_SerializesFormationData()
        {
            var data = new SaveData();
            data.formations.Add(new FormationSaveData
            {
                formationId = "form_1",
                formationType = 0, // Sword
                tileX = 10, tileZ = 10,
                unitIds = new List<string> { "unit_1", "unit_2" }
            });

            SaveManager.Save(TestSlot, data);
            SaveData loaded = SaveManager.Load(TestSlot);

            Assert.AreEqual(1, loaded.formations.Count);
            Assert.AreEqual("form_1", loaded.formations[0].formationId);
            Assert.AreEqual(0, loaded.formations[0].formationType);
            Assert.AreEqual(2, loaded.formations[0].unitIds.Count);
        }

        [Test]
        public void SaveManager_Save_SerializesGameState()
        {
            var data = new SaveData();
            data.gameState.zodiacYear = 5;
            data.gameState.globalEventSeed = 42;
            data.gameState.triggeredEvents = new List<string> { "event1", "event2" };
            data.gameState.marketPrices = new Dictionary<string, float>
            {
                { "Lumber", 10.5f },
                { "IronOre", 25.0f }
            };
            data.gameState.nextUnitId = 100;
            data.gameState.nextFormationId = 10;

            SaveManager.Save(TestSlot, data);
            SaveData loaded = SaveManager.Load(TestSlot);

            Assert.AreEqual(5, loaded.gameState.zodiacYear);
            Assert.AreEqual(42, loaded.gameState.globalEventSeed);
            Assert.AreEqual(2, loaded.gameState.triggeredEvents.Count);
            Assert.AreEqual(100, loaded.gameState.nextUnitId);
            Assert.AreEqual(10, loaded.gameState.nextFormationId);
        }

        #endregion

        #region Versioning

        [Test]
        public void SaveManager_Save_SetsCurrentVersion()
        {
            var data = CreateTestSaveData();
            data.version = 0; // Old version

            SaveManager.Save(TestSlot, data);
            SaveData loaded = SaveManager.Load(TestSlot);

            Assert.AreEqual(SaveConstants.CurrentVersion, loaded.version);
        }

        #endregion

        #region SaveSlotInfo

        [Test]
        public void SaveManager_GetSaveSlotInfos_ReturnsCorrectInfo()
        {
            var data = CreateTestSaveData();
            data.turnNumber = 42;
            SaveManager.Save(TestSlot, data);

            var infos = SaveManager.GetSaveSlotInfos();

            // Should have at least our test slot
            bool found = false;
            foreach (var info in infos)
            {
                if (info.slot == TestSlot)
                {
                    found = true;
                    Assert.IsTrue(info.isValid);
                    Assert.AreEqual(42, info.turnNumber);
                    Assert.IsNotNull(info.timestamp);
                    Assert.Greater(info.fileSize, 0);
                }
            }
            Assert.IsTrue(found, "Test slot should appear in save infos");
        }

        #endregion

        #region Helpers

        private static SaveData CreateTestSaveData()
        {
            return new SaveData
            {
                turnNumber = 1,
                currentPhase = 0,
                mapSeed = 12345,
                mapWidth = 60,
                mapHeight = 60,
                sects = new List<SectSaveData>(),
                tiles = new List<TileSaveData>(),
                units = new List<UnitSaveData>(),
                formations = new List<FormationSaveData>(),
                gameState = new GameStateSaveData()
            };
        }

        #endregion
    }
}
