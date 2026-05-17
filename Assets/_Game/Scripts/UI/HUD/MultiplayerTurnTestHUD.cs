using UnityEngine;
using TalesOfTao.Core.TurnSystem;
using TalesOfTao.Core.EventChannels;

namespace TalesOfTao.UI.HUD
{
    /// <summary>
    /// Multiplayer turn HUD. Shows player list, ready status, phase info, and ready button.
    /// </summary>
    public class MultiplayerTurnTestHUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TurnCoordinator _coordinator;
        [SerializeField] private TurnDriver _localDriver;

        [Header("UI Settings")]
        [SerializeField] private float _panelWidth = 300f;
        [SerializeField] private float _lineHeight = 22f;

        private GUIStyle _labelStyle;
        private GUIStyle _titleStyle;
        private GUIStyle _readyStyle;
        private GUIStyle _notReadyStyle;
        private bool _stylesCreated;

        private void Start()
        {
            if (_coordinator == null)
                _coordinator = FindAnyObjectByType<TurnCoordinator>();
            if (_localDriver == null)
                _localDriver = FindAnyObjectByType<TurnDriver>();

            if (_coordinator != null)
            {
                _coordinator.OnPlayerReadyChanged += OnPlayerReadyChanged;
            }
        }

        private void OnDestroy()
        {
            if (_coordinator != null)
            {
                _coordinator.OnPlayerReadyChanged -= OnPlayerReadyChanged;
            }
        }

        private void OnPlayerReadyChanged(PlayerSlot slot)
        {
            // Could trigger UI refresh; for now OnGUI polls
        }

        private void CreateStyles()
        {
            if (_stylesCreated) return;
            _stylesCreated = true;

            _titleStyle = new GUIStyle
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
                alignment = TextAnchor.UpperLeft
            };

            _labelStyle = new GUIStyle
            {
                fontSize = 14,
                normal = { textColor = Color.white },
                alignment = TextAnchor.UpperLeft
            };

            _readyStyle = new GUIStyle
            {
                fontSize = 14,
                normal = { textColor = Color.green },
                alignment = TextAnchor.UpperLeft
            };

            _notReadyStyle = new GUIStyle
            {
                fontSize = 14,
                normal = { textColor = Color.yellow },
                alignment = TextAnchor.UpperLeft
            };
        }

        private void OnGUI()
        {
            CreateStyles();

            if (_coordinator == null || !_coordinator.IsGameActive)
            {
                DrawNotActive();
                return;
            }

            DrawMultiplayerHUD();
        }

        private void DrawNotActive()
        {
            float x = 20;
            float y = 20;
            float w = _panelWidth;

            GUI.color = new Color(0, 0, 0, 0.75f);
            GUI.DrawTexture(new Rect(x - 10, y - 10, w, 60), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUI.Label(new Rect(x, y, w - 20, _lineHeight + 4), "Multiplayer Turn System", _titleStyle);
            y += _lineHeight + 8;
            GUI.Label(new Rect(x, y, w - 20, _lineHeight), "Game not active. Waiting...", _labelStyle);
        }

        private void DrawMultiplayerHUD()
        {
            float x = 20;
            float y = 20;
            float w = _panelWidth;

            // Calculate panel height based on player count
            int playerCount = _coordinator.PlayerCount;
            float panelH = _lineHeight * (playerCount + 6) + 30;

            // Background
            GUI.color = new Color(0, 0, 0, 0.75f);
            GUI.DrawTexture(new Rect(x - 10, y - 10, w, panelH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // Title
            GUI.Label(new Rect(x, y, w - 20, _lineHeight + 4), "Multiplayer Turn", _titleStyle);
            y += _lineHeight + 8;

            // Turn and phase info
            GUI.Label(new Rect(x, y, w - 20, _lineHeight),
                $"Turn {_coordinator.TurnNumber} | Phase: {_coordinator.CurrentPhase}", _labelStyle);
            y += _lineHeight;

            // Ready timer (if running)
            if (_coordinator.ReadySystem.IsTimerRunning)
            {
                float remaining = _coordinator.ReadySystem.RemainingTime;
                GUI.Label(new Rect(x, y, w - 20, _lineHeight),
                    $"Timer: {remaining:F0}s", _labelStyle);
                y += _lineHeight;
            }

            y += 5;
            GUI.Label(new Rect(x, y, w - 20, _lineHeight), "Players:", _titleStyle);
            y += _lineHeight + 4;

            // Player list
            int readyCount = 0;
            foreach (var player in _coordinator.GetAllPlayers())
            {
                if (player.IsReady) readyCount++;

                var style = player.IsReady ? _readyStyle : _notReadyStyle;
                string status = player.IsReady ? "✓" : "○";
                string type = player.IsAI ? "[AI]" : "[Human]";
                GUI.Label(new Rect(x, y, w - 20, _lineHeight),
                    $"{status} {type} {player.PlayerName}", style);
                y += _lineHeight;
            }

            y += 5;
            GUI.Label(new Rect(x, y, w - 20, _lineHeight),
                $"Ready: {readyCount}/{playerCount}", _labelStyle);
            y += _lineHeight + 5;

            // Ready button (only for local human player)
            if (_localDriver != null && _localDriver.IsMultiplayer)
            {
                var localPlayer = _coordinator.GetPlayer(_localDriver.LocalPlayerId);
                if (localPlayer != null && !localPlayer.IsAI && !localPlayer.IsReady)
                {
                    if (GUI.Button(new Rect(x, y, w - 20, _lineHeight + 4), "Ready (Enter)"))
                    {
                        _localDriver.SignalReady();
                    }
                }
            }
        }
    }
}
