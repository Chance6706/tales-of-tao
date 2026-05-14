using UnityEngine;
using UnityEngine.EventSystems;

namespace TalesOfTao.Core
{
    /// <summary>
    /// Ensures the EventSystem uses InputSystemUIInputModule instead of StandaloneInputModule.
    /// The old StandaloneInputModule conflicts with the New Input System.
    /// Attach to any GameObject in the scene, or let it auto-create.
    /// </summary>
    public class EventSystemFix : MonoBehaviour
    {
        private void Awake()
        {
            var es = FindObjectOfType<EventSystem>();
            if (es == null)
            {
                var go = new GameObject("EventSystem");
                es = go.AddComponent<EventSystem>();
            }

            // Remove old StandaloneInputModule if present
            var oldModule = es.GetComponent<StandaloneInputModule>();
            if (oldModule != null)
                Destroy(oldModule);

            // Add Input System UI module if not present
            if (es.GetComponent<InputSystemUIInputModule>() == null)
                es.gameObject.AddComponent<InputSystemUIInputModule>();
        }
    }
}
