using System;
using System.Collections.Generic;
using UnityEngine;

namespace TalesOfTao.Core.EventChannels
{
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
