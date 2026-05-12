using System;
using System.Collections.Generic;
using UnityEngine;

namespace TalesOfTao.Core.EventChannels
{
    /// <summary>
    /// Base class for all typed event channels.
    /// </summary>
    /// <remarks>
    /// <b>Thread safety:</b> not thread-safe. All subscribe/raise calls must come from
    /// the main thread. If you add async save/load systems, use a lock or main-thread
    /// dispatcher for event access.
    /// </remarks>
    public abstract class EventChannelSO<T> : ScriptableObject
    {
        private readonly List<Action<T>> _listeners = new();

        public void Subscribe(Action<T> listener)
        {
            if (!_listeners.Contains(listener))
                _listeners.Add(listener);
        }

        public void Unsubscribe(Action<T> listener) => _listeners.Remove(listener);

        public void Raise(T value)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
                _listeners[i]?.Invoke(value);
        }

        protected virtual void OnDisable() => _listeners.Clear();
    }
}
