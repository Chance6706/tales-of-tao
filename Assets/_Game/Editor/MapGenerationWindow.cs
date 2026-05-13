using UnityEditor;
using UnityEngine;
using TalesOfTao.Hex;

namespace TalesOfTao.Editor
{
    public class MapGenerationWindow : EditorWindow
    {
        private int _seed;
        private bool _randomizeSeed = true;
        private HexGridManager _gridManager;
        private HexGridRenderer _gridRenderer;

        [MenuItem("TalesOfTao/3 - Generate Map")]
        public static void ShowWindow()
        {
            GetWindow<MapGenerationWindow>("Map Generation");
        }

        private void OnEnable()
        {
            // Auto-find references in the scene
            if (_gridManager == null)
                _gridManager = Object.FindAnyObjectByType<HexGridManager>();
            if (_gridRenderer == null)
                _gridRenderer = Object.FindAnyObjectByType<HexGridRenderer>();
        }

        private void OnGUI()
        {
            GUILayout.Label("Map Generation", EditorStyles.boldLabel);

            _gridManager = (HexGridManager)EditorGUILayout.ObjectField(
                "Grid Manager", _gridManager, typeof(HexGridManager), true);

            _gridRenderer = (HexGridRenderer)EditorGUILayout.ObjectField(
                "Grid Renderer", _gridRenderer, typeof(HexGridRenderer), true);

            _randomizeSeed = EditorGUILayout.Toggle("Random Seed", _randomizeSeed);
            if (!_randomizeSeed)
            {
                _seed = EditorGUILayout.IntField("Seed", _seed);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate Map"))
            {
                Generate(_randomizeSeed ? null : _seed);
            }

            if (GUILayout.Button("Generate New Random Map"))
            {
                Generate();
            }
        }

        private void Generate(int? forcedSeed = null)
        {
            if (_gridManager == null)
            {
                Debug.LogError("[TalesOfTao] Assign a HexGridManager first.");
                return;
            }

            _gridManager.GenerateMap(forcedSeed);

            if (_gridRenderer != null)
            {
                _gridRenderer.Initialize();
                Debug.Log("[TalesOfTao] Map generated and chunks built.");
            }
            else
            {
                Debug.LogWarning("[TalesOfTao] Map generated but no HexGridRenderer found. Chunks not built.");
            }
        }
    }
}
