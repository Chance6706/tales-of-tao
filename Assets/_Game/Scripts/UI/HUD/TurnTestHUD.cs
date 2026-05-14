using UnityEngine;
using TalesOfTao.Core;
using TalesOfTao.Core.TurnSystem;

namespace TalesOfTao.UI.HUD
{
    /// <summary>
    /// Minimal test HUD using IMGUI (OnGUI). No Canvas/EventSystem needed.
    /// Shows phase, turn, zodiac, and End Turn button.
    /// </summary>
    public class TurnTestHUD : MonoBehaviour
    {
        [SerializeField] private TurnDriver _turnDriver;

        private string _phaseText = "...";
        private string _turnText = "...";
        private string _zodiacText = "...";
        private bool _canEndTurn;
        private bool _started;

        private void Start()
        {
            if (_turnDriver == null)
                _turnDriver = FindAnyObjectByType<TurnDriver>();

            if (_turnDriver == null)
            {
                var calGO = new GameObject("ZodiacCalendar");
                var calendar = calGO.AddComponent<ZodiacCalendar>();

                var driverGO = new GameObject("TurnDriver");
                _turnDriver = driverGO.AddComponent<TurnDriver>();
                _turnDriver.Initialize(calendar, null, null, null, 0f);
            }

            _turnDriver.OnPhaseChanged += OnPhaseChanged;
            _turnDriver.OnTurnStarted += OnTurnStarted;
            _turnDriver.StartTurn();
            _started = true;
        }

        private void OnDestroy()
        {
            if (_turnDriver != null)
            {
                _turnDriver.OnPhaseChanged -= OnPhaseChanged;
                _turnDriver.OnTurnStarted -= OnTurnStarted;
            }
        }

        private void OnGUI()
        {
            if (!_started) return;

            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                normal = { textColor = Color.white }
            };

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 18
            };

            // Top-left: phase info
            float x = 20;
            float y = 20;
            float lineHeight = 28;

            GUI.Label(new Rect(x, y, 400, lineHeight), _phaseText, style);
            y += lineHeight;
            GUI.Label(new Rect(x, y, 400, lineHeight), _turnText, style);
            y += lineHeight;
            GUI.Label(new Rect(x, y, 400, lineHeight), _zodiacText, style);

            // Bottom-right: End Turn button
            float btnW = 160;
            float btnH = 50;
            float btnX = Screen.width - btnW - 20;
            float btnY = Screen.height - btnH - 20;

            if (GUI.Button(new Rect(btnX, btnY, btnW, btnH), "End Turn", buttonStyle))
            {
                OnEndTurnClicked();
            }

            // Also handle keyboard
            if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.Space))
            {
                if (_canEndTurn)
                {
                    OnEndTurnClicked();
                    Event.current.Use();
                }
            }
        }

        private void OnPhaseChanged(GamePhase phase)
        {
            _phaseText = phase switch
            {
                GamePhase.Event      => "Event Phase",
                GamePhase.Income     => "Income Phase",
                GamePhase.Build      => "Build Phase",
                GamePhase.Research   => "Research Phase",
                GamePhase.Action     => "Action Phase",
                GamePhase.Resolution => "Resolution Phase",
                _                    => "Unknown"
            };
            _canEndTurn = (phase == GamePhase.Action);
        }

        private void OnTurnStarted(int turn)
        {
            _turnText = $"Turn {turn}";
            if (_turnDriver != null)
            {
                _zodiacText = $"Year of the {_turnDriver.CurrentAnimal}";
                Debug.Log($"[TurnTestHUD] Turn {turn}, Zodiac: {_turnDriver.CurrentAnimal}");
            }
        }

        private void OnEndTurnClicked()
        {
            Debug.Log("[TurnTestHUD] End Turn!");
            _turnDriver?.EndTurn();
        }
    }
}
