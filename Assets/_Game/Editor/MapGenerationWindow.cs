using UnityEditor;
using UnityEngine;

namespace TalesOfTao.Editor
{
    public class MapGenerationWindow : EditorWindow
    {
        private int _seed;
        private bool _randomizeSeed = true;
        private HexGridManager _gridManager;

        [MenuItem("TalesOfTao/3 - Generate Map")]
        public static void ShowWindow()
        {
            GetWindow<MapGenerationWindow>("Map Generation");
        }

        private void OnGUI()
        {
            GUILayout.Label("Map Generation", EditorStyles.boldLabel);

            _gridManager = (HexGridManager)EditorGUILayout.ObjectField(
                "Grid Manager", _gridManager, typeof(HexGridManager), true);

            _randomizeSeed = EditorGUILayout.Toggle("Random Seed", _randomizeSeed);
            if (!_randomizeSeed)
            {
                _seed = EditorGUILayout.IntField("Seed", _seed);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate Map"))
            {
                if (_gridManager == null)
                {
                    Debug.LogError("[TalesOfTao] Assign a HexGridManager first.");
                    return;
                }

                _gridManager.GenerateMap(_randomizeSeed ? null : _seed);
                Debug.Log("[TalesOfTao] Map generated successfully.");
            }

            if (GUILayout.Button("Generate New Random Map"))
            {
                if (_gridManager == null)
                {
                    Debug.LogError("[TalesOfTao] Assign a HexGridManager first.");
                    return;
                }

                _gridManager.GenerateMap();
                Debug.Log("[TalesOfTao] Random map generated.");
            }
        }
    }
}
