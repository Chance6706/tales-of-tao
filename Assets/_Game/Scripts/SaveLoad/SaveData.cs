using System;
using System.Collections.Generic;
using UnityEngine;

namespace TalesOfTao.SaveLoad
{
    /// <summary>
    /// Current save format version. Increment when adding new fields that require migration.
    /// </summary>
    public static class SaveConstants
    {
        public const int CurrentVersion = 1;
        public const int AutosaveSlot = 0;
        public const int MinManualSlot = 1;
        public const int MaxManualSlot = 3;
        public const int AutosaveIntervalTurns = 5;
    }

    /// <summary>
    /// Top-level save file data. Contains all game state for a complete snapshot.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public int version = SaveConstants.CurrentVersion;
        public string timestamp;
        public int turnNumber;
        public int activePlayerIndex;
        public int currentPhase;
        public int mapSeed;
        public int mapWidth;
        public int mapHeight;
        public List<SectSaveData> sects;
        public List<TileSaveData> tiles;
        public List<UnitSaveData> units;
        public List<FormationSaveData> formations;
        public GameStateSaveData gameState;

        public SaveData()
        {
            sects = new List<SectSaveData>();
            tiles = new List<TileSaveData>();
            units = new List<UnitSaveData>();
            formations = new List<FormationSaveData>();
            gameState = new GameStateSaveData();
        }
    }

    /// <summary>
    /// Serializable sect data.
    /// </summary>
    [Serializable]
    public class SectSaveData
    {
        public string sectId;
        public string sectName;
        public int factionId;
        public float tael;
        public float qi;
        public float renown;
        public float face;
        public int dissentLevel;
        public int templeTier;
        public int hallTier;
        public int alchemyHallTier;
        public int armoryTier;
        public int libraryTier;
        public List<string> completedResearch;
        public List<ActiveResearchSaveData> activeResearch;
        public int foundingTileX;
        public int foundingTileZ;
        public Dictionary<string, int> resources;
        public List<BuildingSaveData> buildings;
        public List<DiscipleSaveData> disciples;
        public List<DiplomacySaveData> relations;
        public int sectTrait;
        public int sectAffinity;
        public bool isFounded;
        public int foundingTileQ;
        public int foundingTileR;

        public SectSaveData()
        {
            completedResearch = new List<string>();
            activeResearch = new List<ActiveResearchSaveData>();
            resources = new Dictionary<string, int>();
            buildings = new List<BuildingSaveData>();
            disciples = new List<DiscipleSaveData>();
            relations = new List<DiplomacySaveData>();
        }
    }

    /// <summary>
    /// Active research progress for a single tech node.
    /// </summary>
    [Serializable]
    public class ActiveResearchSaveData
    {
        public string techId;
        public float progress01; // 0.0 to 1.0
        public int branch;       // 0=Alchemy, 1=Forge, 2=Martial
    }

    /// <summary>
    /// Serializable building data.
    /// </summary>
    [Serializable]
    public class BuildingSaveData
    {
        public string buildingTypeId;
        public int tier;
        public float posX;
        public float posY;
        public float posZ;
    }

    /// <summary>
    /// Serializable disciple data.
    /// </summary>
    [Serializable]
    public class DiscipleSaveData
    {
        public string name;
        public int rank;
        public int hp;
        public int maxHP;
        public int attack;
        public int defense;
        public int speed;
        public int qiPower;
        public int rootQuality;
        public string trait;
        public string bondedBeast;
        public bool isAlive;
        public string[] techniques;
    }

    /// <summary>
    /// Serializable diplomacy data for a bilateral relation.
    /// </summary>
    [Serializable]
    public class DiplomacySaveData
    {
        public int otherSectId;
        public float trust;
        public bool hasTradeAgreement;
        public bool isAllied;
        public bool isAtWar;
    }

    /// <summary>
    /// Serializable tile data.
    /// </summary>
    [Serializable]
    public class TileSaveData
    {
        public int x;
        public int z;
        public int terrainTypeIndex;
        public int elevation;
        public int qiDensity;
        public int caveType;
        public int feature;
        public int controlState;
        public int fortificationLevel;
        public int ownerSectId;
        public bool hasRoad;
        public bool[] visibilityPerSect;

        public TileSaveData()
        {
            ownerSectId = -1;
        }
    }

    /// <summary>
    /// Serializable unit data.
    /// </summary>
    [Serializable]
    public class UnitSaveData
    {
        public string unitId;
        public string unitType;
        public int ownerSectId;
        public int tileX;
        public int tileZ;
        public float currentHP;
        public float maxHP;
        public int movementPointsRemaining;
        public int formationType; // -1 = no formation
        public List<string> assignedDisciples;
        public string activeAction;

        public UnitSaveData()
        {
            formationType = -1;
            assignedDisciples = new List<string>();
            activeAction = "None";
        }
    }

    /// <summary>
    /// Serializable formation data.
    /// </summary>
    [Serializable]
    public class FormationSaveData
    {
        public string formationId;
        public int formationType; // 0=Sword, 1=Commander, 2=Scout
        public int tileX;
        public int tileZ;
        public List<string> unitIds;

        public FormationSaveData()
        {
            unitIds = new List<string>();
        }
    }

    /// <summary>
    /// Serializable global game state.
    /// </summary>
    [Serializable]
    public class GameStateSaveData
    {
        public int zodiacYear;
        public int globalEventSeed;
        public List<string> triggeredEvents;
        public Dictionary<string, float> marketPrices;
        public int nextUnitId;
        public int nextFormationId;

        public GameStateSaveData()
        {
            triggeredEvents = new List<string>();
            marketPrices = new Dictionary<string, float>();
            zodiacYear = 1;
            globalEventSeed = 0;
            nextUnitId = 1;
            nextFormationId = 1;
        }
    }
}
