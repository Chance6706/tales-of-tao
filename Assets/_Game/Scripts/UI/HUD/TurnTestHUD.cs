using UnityEngine;
using TalesOfTao.Core;
using TalesOfTao.Core.TurnSystem;

namespace TalesOfTao.UI.HUD
{
    public class TurnTestHUD : MonoBehaviour
    {
        [SerializeField] private TurnDriver _turnDriver;

        private string _phaseText = "...";
        private string _turnText = "...";
        private string _zodiacText = "...";
        private bool _canEndTurn;
        private bool _started;
        private Rect _buttonRect;

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

            // Subscribe to events BEFORE starting the turn
            _turnDriver.OnPhaseChanged += OnPhaseChanged;
            _turnDriver.OnTurnStarted += OnTurnStarted;
            _started = true;

            // Now start the turn (events will fire and be caught)
            _turnDriver.StartTurn();
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

            // Check if mouse is over the button — if so, consume the event so TileSelector doesn't fire
            Event e = Event.current;
            bool mouseOverButton = _buttonRect.Contains(e.mousePosition);

            if (mouseOverButton && e.type == EventType.MouseDown)
            {
                e.Use(); // Consume the event so it doesn't reach TileSelector
            }

            if (GUI.Button(_buttonRect, "End Turn", buttonStyle))
            {
                OnEndTurnClicked();
            }

            // Keyboard shortcut
            if (e.type == EventType.KeyDown && (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.Space))
            {
                if (_canEndTurn)
                {
                    OnEndTurnClicked();
                    e.Use();
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
