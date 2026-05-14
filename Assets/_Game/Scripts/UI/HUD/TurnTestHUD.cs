using UnityEngine;
using TalesOfTao.Core;
using TalesOfTao.Core.TurnSystem;

namespace TalesOfTao.UI.HUD
{
    public class TurnTestHUD : MonoBehaviour
    {
        private TurnDriver _turnDriver;
        private string _phaseText = "...";
        private string _turnText = "...";
        private string _zodiacText = "...";
        private bool _canEndTurn;
        private Rect _buttonRect;

        private void Start()
        {
            Debug.Log("[TurnTestHUD] Start");

            // Always create our own turn system to avoid conflicts
            var calGO = new GameObject("ZodiacCalendar");
            var calendar = calGO.AddComponent<ZodiacCalendar>();

            var driverGO = new GameObject("TurnDriver");
            _turnDriver = driverGO.AddComponent<TurnDriver>();
            _turnDriver.Initialize(calendar, null, null, null, 0f);

            // Subscribe BEFORE starting
            _turnDriver.OnPhaseChanged += OnPhaseChanged;
            _turnDriver.OnTurnStarted += OnTurnStarted;

            Debug.Log("[TurnTestHUD] Subscribed, starting turn...");
            _turnDriver.StartTurn();
            Debug.Log("[TurnTestHUD] StartTurn returned");
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
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                normal = { textColor = Color.white }
            };

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 18
            };

            float x = 20;
            float y = 20;
            float lineHeight = 28;

            GUI.Label(new Rect(x, y, 400, lineHeight), _phaseText, labelStyle);
            y += lineHeight;
            GUI.Label(new Rect(x, y, 400, lineHeight), _turnText, labelStyle);
            y += lineHeight;
            GUI.Label(new Rect(x, y, 400, lineHeight), _zodiacText, labelStyle);

            float btnW = 160;
            float btnH = 50;
            _buttonRect = new Rect(Screen.width - btnW - 20, Screen.height - btnH - 20, btnW, btnH);

            // Consume mouse events over the button to prevent TileSelector raycast
            Event e = Event.current;
            if (e.isMouse && _buttonRect.Contains(e.mousePosition))
            {
                if (e.type == EventType.MouseDown || e.type == EventType.MouseUp)
                {
                    e.Use();
                }
            }

            if (GUI.Button(_buttonRect, "End Turn", buttonStyle))
            {
                Debug.Log("[TurnTestHUD] Button clicked!");
                OnEndTurnClicked();
            }

            // Keyboard shortcut
            if (e.type == EventType.KeyDown && (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.Space))
            {
                if (_canEndTurn)
                {
                    Debug.Log("[TurnTestHUD] Key pressed!");
                    OnEndTurnClicked();
                    e.Use();
                }
            }
        }

        private void OnPhaseChanged(GamePhase phase)
        {
            Debug.Log($"[TurnTestHUD] OnPhaseChanged: {phase}");
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
            Debug.Log($"[TurnTestHUD] OnTurnStarted: {turn}");
            _turnText = $"Turn {turn}";
            if (_turnDriver != null)
            {
                string animal = _turnDriver.CurrentAnimal;
                _zodiacText = $"Year of the {animal}";
                Debug.Log($"[TurnTestHUD] Zodiac: {animal}");
            }
        }

        private void OnEndTurnClicked()
        {
            Debug.Log("[TurnTestHUD] End Turn!");
            _turnDriver?.EndTurn();
        }
    }
}
