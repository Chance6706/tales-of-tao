using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;
using TalesOfTao.Core;
using TalesOfTao.Core.EventChannels;
using TalesOfTao.Hex;
using TalesOfTao.UI;

namespace TalesOfTao.Editor
{
    public static class SceneSetupHelper
    {
        private const string ScenePath       = "Assets/_Game/Scenes/Main.unity";
        private const string TerrainDir      = "Assets/_Game/Data/Terrain";
        private const string EventChannelDir = "Assets/_Game/Data/EventChannels";

        [MenuItem("TalesOfTao/2 - Setup Main Scene")]
        public static void SetupScene()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            SetupGameManager();
            SetupHexTile();
            SetupTileSelector();
            SetupTileInfoPanel();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[TalesOfTao] Main scene setup complete. Press Play to verify the hex tile.");
        }

        private static void SetupGameManager()
        {
            if (Object.FindAnyObjectByType<GameManager>() != null) return;

            var go = new GameObject("GameManager");
            var gm = go.AddComponent<GameManager>();
            var ser = new SerializedObject(gm);
            AssignChannel(ser, "_onPhaseChanged",    "EC_OnPhaseChanged.asset");
            AssignChannel(ser, "_onTurnEnded",       "EC_OnTurnEnded.asset");
            AssignChannel(ser, "_onResourceChanged", "EC_OnResourceChanged.asset");
            AssignChannel(ser, "_onUnitMoved",       "EC_OnUnitMoved.asset");
            AssignChannel(ser, "_onCombatResolved",  "EC_OnCombatResolved.asset");
            ser.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void AssignChannel(SerializedObject ser, string propName, string assetFile)
        {
            var prop = ser.FindProperty(propName);
            if (prop == null) { Debug.LogError($"[TalesOfTao] Property '{propName}' not found on GameManager."); return; }
            var asset = AssetDatabase.LoadMainAssetAtPath($"{EventChannelDir}/{assetFile}");
            if (asset != null) prop.objectReferenceValue = asset;
            else Debug.LogWarning($"[TalesOfTao] Channel asset not found: {assetFile}. Run '1 - Create Data Assets' first.");
        }

        private static void SetupHexTile()
        {
            if (Object.FindAnyObjectByType<HexTile>() != null) return;

            int layerIndex = LayerMask.NameToLayer("HexTile");
            if (layerIndex < 0)
            {
                Debug.LogError("[TalesOfTao] Layer 'HexTile' not found. Add it in Project Settings > Tags and Layers.");
                return;
            }

            var go   = new GameObject("HexTile_Plains");
            go.layer = layerIndex;
            var tile = go.AddComponent<HexTile>();

            var plains = AssetDatabase.LoadAssetAtPath<TerrainTypeSO>($"{TerrainDir}/TerrainType_Plains.asset");
            if (plains != null)
            {
                var ser = new SerializedObject(tile);
                ser.FindProperty("_terrainOverride").objectReferenceValue = plains;
                ser.ApplyModifiedPropertiesWithoutUndo();
            }
            else Debug.LogWarning("[TalesOfTao] TerrainType_Plains.asset not found. Run '1 - Create Data Assets' first.");
        }

        private static void SetupTileSelector()
        {
            var cam = Camera.main;
            if (cam == null) { Debug.LogWarning("[TalesOfTao] No Main Camera found. TileSelector not added."); return; }
            if (cam.GetComponent<TileSelector>() != null) return;

            var selector   = cam.gameObject.AddComponent<TileSelector>();
            int layerIndex = LayerMask.NameToLayer("HexTile");
            if (layerIndex < 0) { Debug.LogError("[TalesOfTao] Layer 'HexTile' not found — TileSelector mask not set."); return; }

            var ser = new SerializedObject(selector);
            ser.FindProperty("_hexLayer").intValue = 1 << layerIndex;
            ser.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetupTileInfoPanel()
        {
            if (Object.FindAnyObjectByType<TileInfoPanel>() != null) return;

            var canvasGo = new GameObject("HUD_Canvas");
            var canvas   = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();

            var panelGo  = new GameObject("TileInfoPanel");
            panelGo.transform.SetParent(canvasGo.transform, false);
            panelGo.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);

            var panelRect      = panelGo.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0f, 0f);
            panelRect.anchorMax = new Vector2(0.28f, 0.45f);
            panelRect.offsetMin = new Vector2(10f, 10f);
            panelRect.offsetMax = new Vector2(-10f, -10f);

            var infoPanel = panelGo.AddComponent<TileInfoPanel>();

            var textGo = new GameObject("ContentText");
            textGo.transform.SetParent(panelGo.transform, false);
            var tmp    = textGo.AddComponent<TextMeshProUGUI>();
            tmp.fontSize = 14;
            tmp.color    = Color.white;
            tmp.margin   = new Vector4(8f, 8f, 8f, 8f);

            var textRect      = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var ser = new SerializedObject(infoPanel);
            ser.FindProperty("_contentText").objectReferenceValue = tmp;
            ser.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
