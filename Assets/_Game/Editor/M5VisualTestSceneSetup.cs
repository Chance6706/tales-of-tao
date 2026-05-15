using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

namespace TalesOfTao.Editor
{
    /// <summary>
    /// Creates and opens the M5 visual test scene.
    /// Menu: Tales of Tao / M5 / Open Visual Test Scene
    /// </summary>
    public class M5VisualTestSceneSetup
    {
        [MenuItem("Tales of Tao/M5/Open Visual Test Scene")]
        static void OpenVisualTestScene()
        {
            string scenePath = "Assets/_Game/Scenes/M5VisualTest.unity";

            // Create the scene if it doesn't exist
            if (!File.Exists(scenePath))
            {
                // Create a new empty scene
                var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

                // Add the visual test component to the scene
                var testGO = new GameObject("M5VisualTest");
                testGO.AddComponent<Tests.M5VisualTest>();

                // Save the scene
                EditorSceneManager.SaveScene(newScene, scenePath);
                Debug.Log($"[M5VisualTest] Created test scene at {scenePath}");
            }
            else
            {
                EditorSceneManager.OpenScene(scenePath);
                Debug.Log($"[M5VisualTest] Opened test scene at {scenePath}");
            }
        }
    }
}
