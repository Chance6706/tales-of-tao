using System;
using System.Collections.Generic;
using UnityEngine;

namespace TalesOfTao.Core.EventChannels
{
    /// <summary>
    /// Event channel for parameterless (void) events.
    /// </summary>
    /// <remarks>
    /// <b>Thread safety:</b> not thread-safe. All subscribe/raise calls must come from
    /// the main thread. If you add async save/load systems, use a lock or main-thread
    /// dispatcher for event access.
    /// </remarks>
    [CreateAssetMenu(menuName = "TalesOfTao/Events/Void Event Channel", fileName = "VoidEventChannel")]
    public class VoidEventChannelSO : ScriptableObject
    {
        private readonly List<Action> _listeners = new();

        public void Subscribe(Action listener)
        {
            if (!_listeners.Contains(listener))
                _listeners.Add(listener);
        }

        public void Unsubscribe(Action listener) => _listeners.Remove(listener);

        public void Raise()
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
                _listeners[i]?.Invoke();
        }

        protected virtual void OnDisable() => _listeners.Clear();
    }
}
