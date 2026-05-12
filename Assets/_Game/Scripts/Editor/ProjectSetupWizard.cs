using UnityEngine;
using UnityEditor;
using TalesOfTao.Hex;
using TalesOfTao.Core;
using TalesOfTao.Core.EventChannels;

namespace TalesOfTao.Editor
{
    public static class ProjectSetupWizard
    {
        private const string TerrainDir      = "Assets/_Game/Data/Terrain";
        private const string EventChannelDir = "Assets/_Game/Data/EventChannels";

        [MenuItem("TalesOfTao/1 - Create Data Assets")]
        public static void CreateDataAssets()
        {
            EnsureDirectory(TerrainDir);
            EnsureDirectory(EventChannelDir);
            CreateTerrainAssets();
            CreateEventChannelAssets();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[TalesOfTao] Data assets created. Check Assets/_Game/Data/.");
        }

        private static void CreateTerrainAssets()
        {
            CreateTerrain("TerrainType_Plains",    TerrainType.Plains,    "Plains",      1.0f,  0.00f, 1.0f, new Color(0.55f, 0.76f, 0.29f), false);
            CreateTerrain("TerrainType_Forest",    TerrainType.Forest,    "Forest",      2.0f,  0.25f, 1.1f, new Color(0.18f, 0.42f, 0.18f), false);
            CreateTerrain("TerrainType_Mountain",  TerrainType.Mountain,  "Mountain",    3.0f,  0.50f, 1.5f, new Color(0.55f, 0.55f, 0.58f), false);
            CreateTerrain("TerrainType_SacredPeak",TerrainType.SacredPeak,"Sacred Peak", 4.0f,  0.75f, 2.0f, new Color(0.84f, 0.91f, 0.97f), false);
            CreateTerrain("TerrainType_River",     TerrainType.River,     "River",       1.5f,  0.00f, 1.2f, new Color(0.28f, 0.60f, 0.82f), false);
            CreateTerrain("TerrainType_Lake",      TerrainType.Lake,      "Lake",        0.0f,  0.00f, 1.3f, new Color(0.10f, 0.28f, 0.62f), true);
            CreateTerrain("TerrainType_Desert",    TerrainType.Desert,    "Desert",      2.0f,  0.00f, 0.5f, new Color(0.87f, 0.78f, 0.49f), false);
            CreateTerrain("TerrainType_Swamp",     TerrainType.Swamp,     "Swamp",       3.0f, -0.10f, 0.8f, new Color(0.35f, 0.44f, 0.22f), false);
        }

        private static void CreateTerrain(
            string fileName, TerrainType type, string displayName,
            float moveCost, float defenseBon, float qiMod, Color tint, bool impassable)
        {
            string path = $"{TerrainDir}/{fileName}.asset";
            if (AssetDatabase.LoadAssetAtPath<TerrainTypeSO>(path) != null) return;

            var so  = ScriptableObject.CreateInstance<TerrainTypeSO>();
            AssetDatabase.CreateAsset(so, path);

            var ser = new SerializedObject(so);
            SetProp(ser, "_type",         p => p.enumValueIndex = (int)type,   fileName);
            SetProp(ser, "_displayName",  p => p.stringValue    = displayName, fileName);
            SetProp(ser, "_movementCost", p => p.floatValue     = moveCost,    fileName);
            SetProp(ser, "_defenseBonus", p => p.floatValue     = defenseBon,  fileName);
            SetProp(ser, "_qiModifier",   p => p.floatValue     = qiMod,       fileName);
            SetProp(ser, "_tintColor",    p => p.colorValue     = tint,        fileName);
            SetProp(ser, "_isImpassable", p => p.boolValue      = impassable,  fileName);
            ser.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetProp(SerializedObject ser, string field,
            System.Action<SerializedProperty> assign, string assetName)
        {
            var prop = ser.FindProperty(field);
            if (prop == null)
            {
                Debug.LogError($"[TalesOfTao] SerializedProperty '{field}' not found on {assetName}.");
                return;
            }
            assign(prop);
        }

        private static void CreateEventChannelAssets()
        {
            CreateChannel<GamePhaseEventChannelSO>("EC_OnPhaseChanged");
            CreateChannel<VoidEventChannelSO>     ("EC_OnTurnEnded");
            CreateChannel<StringEventChannelSO>   ("EC_OnResourceChanged");
            CreateChannel<VoidEventChannelSO>     ("EC_OnUnitMoved");
            CreateChannel<VoidEventChannelSO>     ("EC_OnCombatResolved");
        }

        private static void CreateChannel<T>(string fileName) where T : ScriptableObject
        {
            string path = $"{EventChannelDir}/{fileName}.asset";
            if (AssetDatabase.LoadAssetAtPath<T>(path) != null) return;
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<T>(), path);
        }

        private static void EnsureDirectory(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string[] parts  = path.Split('/');
            string   current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
