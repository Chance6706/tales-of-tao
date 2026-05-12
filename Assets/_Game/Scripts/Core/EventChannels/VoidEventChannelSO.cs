using System;
using System.Collections.Generic;
using UnityEngine;

namespace TalesOfTao.Core.EventChannels
{
    // Void (no-payload) event channel. Use for signals that carry no data,
    // e.g. OnTurnEnded, OnUnitMoved.
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

        private void OnDisable() => _listeners.Clear();
    }
}
