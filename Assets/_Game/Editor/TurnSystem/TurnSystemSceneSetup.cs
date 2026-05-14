using UnityEngine;
using UnityEditor;
using TalesOfTao.Core.EventChannels;
using TalesOfTao.Core.TurnSystem;

namespace TalesOfTao.Editor.TurnSystem
{
    /// <summary>
    /// Editor menu to set up the turn system test scene.
    /// </summary>
    public static class TurnSystemSceneSetup
    {
        [MenuItem("TalesOfTao/Setup Turn System Test Scene")]
        public static void SetupScene()
        {
            // Create event channel assets if they don't exist
            CreateEventChannelIfNeeded<ZodiacBonusesEventChannelSO>("EC_OnZodiacBonuses", "Assets/_Game/Data/EventChannels/EC_OnZodiacBonuses.asset");

            // Create ZodiacCalendar
            var calGO = new GameObject("ZodiacCalendar");
            var calendar = calGO.AddComponent<ZodiacCalendar>();

            // Create TurnDriver
            var driverGO = new GameObject("TurnDriver");
            var driver = driverGO.AddComponent<TurnDriver>();

            // Create TurnTestHUD
            var hudGO = new GameObject("TurnTestHUD");
            var hud = hudGO.AddComponent<TurnTestHUD>();

            // Wire up HUD to driver
            hud.Initialize(driver);

            // Set up GameManager references
            var gm = GameManager.Instance;
            if (gm != null)
            {
                // Use reflection-free approach: the components find their own references
                Debug.Log("[TurnSystemSetup] GameManager found. Turn system components created.");
            }
            else
            {
                Debug.LogWarning("[TurnSystemSetup] GameManager not found in scene. Add a GameManager first.");
            }

            Debug.Log("[TurnSystemSetup] Turn system test scene setup complete!");
            Debug.Log("Press Play to test. The turn cycle will auto-advance through phases.");
            Debug.Log("During Action phase, click 'End Turn' button or press Enter to advance.");
        }

        private static void CreateEventChannelIfNeeded<T>(string name, string path) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null)
            {
                Debug.Log($"[TurnSystemSetup] Event channel already exists: {path}");
                return;
            }

            var asset = ScriptableObject.CreateInstance<T>();
            asset.name = name;
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            Debug.Log($"[TurnSystemSetup] Created event channel: {path}");
        }
    }
}
