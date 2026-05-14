using UnityEngine;
using UnityEditor;
using TalesOfTao.Sects;

namespace TalesOfTao.Editor.Sects
{
    public static class SectConfigCreator
    {
        [MenuItem("TalesOfTao/Create Sect Config Assets")]
        public static void CreateSectConfigs()
        {
            string path = "Assets/_Game/Data/Sects";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets/_Game/Data", "Sects");
            }

            CreateConfig(path, "SectConfig_WuDang", "Wu Dang", SectAffinity.InternalQi, SectTrait.QiBonus,
                new Color(0.2f, 0.4f, 0.8f), new Color(0.9f, 0.9f, 1f), "TrainingGrounds", "Disciples train 25% faster");

            CreateConfig(path, "SectConfig_Shaolin", "Shaolin", SectAffinity.BodyCultivation, SectTrait.HpBonus,
                new Color(0.8f, 0.6f, 0.1f), new Color(1f, 0.9f, 0.5f), "TrainingGrounds", "+5% HP per cave trial");

            CreateConfig(path, "SectConfig_TangClan", "Tang Clan", SectAffinity.Poison, SectTrait.PoisonImmunity,
                new Color(0.6f, 0.1f, 0.6f), new Color(0.9f, 0.5f, 0.9f), "MedicineHall", "Produces poisons AND medicine");

            CreateConfig(path, "SectConfig_MountHua", "Mount Hua", SectAffinity.SwordArts, SectTrait.SwordDamage,
                new Color(0.8f, 0.1f, 0.1f), new Color(1f, 0.8f, 0.8f), "Library", "Martial research -30% Qi cost");

            CreateConfig(path, "SectConfig_Emei", "Emei", SectAffinity.Balanced, SectTrait.RenownDiplomacy,
                new Color(0.9f, 0.5f, 0.7f), new Color(1f, 0.8f, 0.9f), "ExternalAffairsHall", "Settlement trust gain x1.5");

            CreateConfig(path, "SectConfig_Kunlun", "Kunlun", SectAffinity.IceElemental, SectTrait.MountainDefense,
                new Color(0.5f, 0.7f, 0.9f), new Color(0.8f, 0.9f, 1f), "CaveSystem", "All caves become Qi Refinement type");

            AssetDatabase.SaveAssets();
            Debug.Log("[SectConfigCreator] Created 6 sect configs in " + path);
        }

        private static void CreateConfig(string path, string name, string displayName, SectAffinity affinity,
            SectTrait trait, Color primary, Color secondary, string replacesBuilding, string uniqueHallBonus)
        {
            var existing = AssetDatabase.LoadAssetAtPath<SectConfigSO>($"{path}/{name}.asset");
            if (existing != null)
            {
                Debug.Log($"[SectConfigCreator] Already exists: {name}");
                return;
            }

            var config = ScriptableObject.CreateInstance<SectConfigSO>();
            config.name = name;

            // Use reflection-free approach: set via serialized object
            var so = new SerializedObject(config);
            so.FindProperty("_displayName").stringValue = displayName;
            so.FindProperty("_affinity").enumValueIndex = (int)affinity;
            so.FindProperty("_trait").enumValueIndex = (int)trait;
            so.FindProperty("_primaryColor").colorValue = primary;
            so.FindProperty("_secondaryColor").colorValue = secondary;
            so.FindProperty("_replacesBuilding").stringValue = replacesBuilding;
            so.FindProperty("_uniqueHallBonus").stringValue = uniqueHallBonus;
            so.ApplyModifiedProperties();

            AssetDatabase.CreateAsset(config, $"{path}/{name}.asset");
            Debug.Log($"[SectConfigCreator] Created: {name}");
        }
    }
}
