using UnityEngine;
using UnityEditor;
using TalesOfTao.Core;
using TalesOfTao.Core.EventChannels;
using TalesOfTao.Core.TurnSystem;
using TalesOfTao.UI.HUD;
using TalesOfTao.Sects;

namespace TalesOfTao.Editor.TurnSystem
{
    public static class TurnSystemSceneSetup
    {
        [MenuItem("TalesOfTao/Setup Turn System Test Scene")]
        public static void SetupScene()
        {
            CreateEventChannelIfNeeded<ZodiacBonusesEventChannelSO>("EC_OnZodiacBonuses", "Assets/_Game/Data/EventChannels/EC_OnZodiacBonuses.asset");

            var calGO = new GameObject("ZodiacCalendar");
            var calendar = calGO.AddComponent<ZodiacCalendar>();

            var driverGO = new GameObject("TurnDriver");
            var driver = driverGO.AddComponent<TurnDriver>();
            driver.Initialize(calendar, null, null, null, 0f);

            var hudGO = new GameObject("TurnTestHUD");
            hudGO.AddComponent<TurnTestHUD>();

            var sectGO = new GameObject("SectFoundingTest");
            sectGO.AddComponent<SectFoundingTest>();

            var fixGO = new GameObject("EventSystemFix");
            fixGO.AddComponent<EventSystemFix>();

            Selection.activeGameObject = hudGO;
            Debug.Log("[TurnSystemSetup] Complete. Press Play to test.");
            Debug.Log("Wait for Action phase, then press T on a hex tile to found a sect.");
        }

        private static void CreateEventChannelIfNeeded<T>(string name, string path) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null) return;

            var asset = ScriptableObject.CreateInstance<T>();
            asset.name = name;
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
        }
    }
}
