using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Linq;

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

            if (!File.Exists(scenePath))
            {
                var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

                var testGO = new GameObject("M5VisualTest");
                var testType = System.AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => { try { return a.GetTypes(); } catch { return Enumerable.Empty<System.Type>(); } })
                    .FirstOrDefault(t => t.Name == "M5VisualTest" && t.IsSubclassOf(typeof(MonoBehaviour)));

                if (testType != null)
                    testGO.AddComponent(testType);
                else
                    Debug.LogWarning("[M5VisualTest] M5VisualTest type not found. Add component manually via Add Component.");

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
