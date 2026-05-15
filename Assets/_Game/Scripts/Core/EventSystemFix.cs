using UnityEngine;
using UnityEngine.EventSystems;

namespace TalesOfTao.Core
{
    /// <summary>
    /// Prevents StandaloneInputModule from conflicting with the New Input System.
    /// Destroys any auto-created StandaloneInputModule on EventSystem.
    /// IMGUI-based UI doesn't need an EventSystem at all.
    /// </summary>
    [DefaultExecutionOrder(-200)]
    public class EventSystemFix : MonoBehaviour
    {
        private void Awake()
        {
            var es = FindObjectOfType<EventSystem>();
            if (es != null)
            {
                var oldModule = es.GetComponent<StandaloneInputModule>();
                if (oldModule != null)
                {
                    Destroy(oldModule);
                    Debug.Log("[EventSystemFix] Removed StandaloneInputModule from EventSystem (incompatible with New Input System).");
                }
            }
        }
    }
}
