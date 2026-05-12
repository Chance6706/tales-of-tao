using System;
using System.Collections.Generic;
using UnityEngine;

namespace TalesOfTao.Core.EventChannels
{
    // Generic ScriptableObject event channel.
    // Create a concrete subclass with [CreateAssetMenu] for each event type.
    // Systems subscribe/unsubscribe at OnEnable/OnDisable; raise with Raise(value).
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
            // Iterate backwards so listeners can safely unsubscribe during callback.
            for (int i = _listeners.Count - 1; i >= 0; i--)
                _listeners[i]?.Invoke(value);
        }

        protected virtual void OnDisable() => _listeners.Clear();
        // Clears all listeners — useful between Play sessions in the Editor.
        private void OnDisable() => _listeners.Clear();
    }
}
