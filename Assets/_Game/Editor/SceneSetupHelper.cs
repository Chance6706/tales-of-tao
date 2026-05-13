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

        // Layer indices matching TagManager.asset configuration
        private const int HexTileLayer = 8;

        [MenuItem("TalesOfTao/2 - Setup Main Scene")]
        public static void SetupScene()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            SetupGameManager();
            SetupMainCamera();
            SetupGridManager();
            SetupGridRenderer();
            SetupHexTile();
            SetupTileSelector();
            SetupTileInfoPanel();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[TalesOfTao] Main scene setup complete. Press Play to verify the hex tile.");
        }

        [MenuItem("TalesOfTao/2b - Import TMP Essential Resources")]
        public static void ImportTMPResources()
        {
            // Opens the TMP Essential Resources import window
            EditorApplication.ExecuteMenuItem("Window/TextMeshPro/Import TMP Essential Resources");
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

        private static void SetupMainCamera()
        {
            if (Camera.main != null) return;

            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.15f, 0.15f, 0.2f, 1f);
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.nearClipPlane = 0.3f;
            cam.farClipPlane = 1000f;
            camGo.transform.position = new Vector3(0f, 10f, -10f);
            camGo.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
            camGo.AddComponent<AudioListener>();
            Debug.Log("[TalesOfTao] Created Main Camera (orthographic, isometric view).");
        }

        private static void SetupGridManager()
        {
            if (Object.FindAnyObjectByType<HexGridManager>() != null) return;

            var go = new GameObject("HexGridManager");
            go.AddComponent<HexGridManager>();
            Debug.Log("[TalesOfTao] Created HexGridManager. Use '3 - Generate Map' to generate the grid.");
        }

        private static void SetupGridRenderer()
        {
            if (Object.FindAnyObjectByType<HexGridRenderer>() != null) return;

            var go = new GameObject("HexGridRenderer");
            go.transform.SetParent(null);
            var renderer = go.AddComponent<HexGridRenderer>();

            // Auto-wire to grid manager
            var gridManager = Object.FindAnyObjectByType<HexGridManager>();
            if (gridManager != null)
            {
                renderer.SetGridManager(gridManager);
            }

            Debug.Log("[TalesOfTao] Created HexGridRenderer.");
        }

        private static void SetupHexTile()
        {
            if (Object.FindAnyObjectByType<HexTile>() != null) return;

            int layerIndex = LayerMask.NameToLayer("HexTile");
            if (layerIndex < 0)
            {
                Debug.LogWarning("[TalesOfTao] Layer 'HexTile' not found in TagManager. Using default layer. " +
                                 "Add 'HexTile' layer in Project Settings > Tags and Layers, then re-run setup.");
                layerIndex = 0; // Default layer
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
            if (layerIndex < 0) layerIndex = 0;

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
