using System;
using System.Collections.Generic;
using UnityEngine;

namespace TalesOfTao.Core.TurnSystem
{
    /// <summary>
    /// Tracks player ready state and manages the ready timer.
    /// Fires event when all players are ready.
    /// </summary>
    public class ReadySystem
    {
        private readonly Dictionary<int, bool> _playerReady = new();
        private readonly HashSet<int> _aiPlayers = new();
        private float _timerDuration;
        private float _timerRemaining;
        private bool _timerRunning;

        public event Action OnAllPlayersReady;
        public event Action<float> OnTimerUpdated;

        public bool IsTimerRunning => _timerRunning;
        public float RemainingTime => _timerRemaining;
        public int TotalPlayers => _playerReady.Count;
        public int ReadyPlayers => GetReadyCount();

        public void AddPlayer(int playerId, bool isAI)
        {
            _playerReady[playerId] = false;
            if (isAI)
            {
                _aiPlayers.Add(playerId);
                _playerReady[playerId] = true; // AI is always ready
            }
        }

        public void RemovePlayer(int playerId)
        {
            _playerReady.Remove(playerId);
            _aiPlayers.Remove(playerId);
        }

        public void SetPlayerReady(int playerId)
        {
            if (!_playerReady.ContainsKey(playerId)) return;
            _playerReady[playerId] = true;
            
            if (AreAllPlayersReady())
            {
                StopTimer();
                OnAllPlayersReady?.Invoke();
            }
        }

        public void SetPlayerUnready(int playerId)
        {
            if (!_playerReady.ContainsKey(playerId)) return;
            if (_aiPlayers.Contains(playerId)) return; // AI can't be unready
            _playerReady[playerId] = false;
        }

        public bool IsPlayerReady(int playerId)
        {
            return _playerReady.TryGetValue(playerId, out bool ready) && ready;
        }

        public bool AreAllPlayersReady()
        {
            foreach (var kvp in _playerReady)
            {
                if (!kvp.Value) return false;
            }
            return _playerReady.Count > 0;
        }

        public void StartTimer(float duration)
        {
            _timerDuration = duration;
            _timerRemaining = duration;
            _timerRunning = true;
        }

        public void StopTimer()
        {
            _timerRunning = false;
            _timerRemaining = 0f;
        }

        public void ResetAll()
        {
            foreach (var key in new List<int>(_playerReady.Keys))
            {
                _playerReady[key] = _aiPlayers.Contains(key);
            }
            StopTimer();
        }

        public void Tick(float deltaTime)
        {
            if (!_timerRunning) return;
            
            _timerRemaining -= deltaTime;
            OnTimerUpdated?.Invoke(_timerRemaining);
            
            if (_timerRemaining <= 0f)
            {
                StopTimer();
                // Force all unready players to ready
                foreach (var key in new List<int>(_playerReady.Keys))
                {
                    if (!_aiPlayers.Contains(key))
                        _playerReady[key] = true;
                }
                OnAllPlayersReady?.Invoke();
            }
        }

        private int GetReadyCount()
        {
            int count = 0;
            foreach (var kvp in _playerReady)
            {
                if (kvp.Value) count++;
            }
            return count;
        }
    }
}
