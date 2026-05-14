using UnityEngine;
using TalesOfTao.Core;
using TalesOfTao.Core.TurnSystem;

namespace TalesOfTao.UI.HUD
{
    public class TurnTestHUD : MonoBehaviour
    {
        private TurnDriver _turnDriver;
        private ZodiacCalendar _calendar;
        private string _phaseText = "...";
        private string _turnText = "...";
        private string _zodiacText = "...";
        private bool _canEndTurn;
        private Rect _buttonRect;
        private GUIStyle _labelStyle;
        private GUIStyle _buttonStyle;
        private bool _stylesCreated;
        private float _turnEndTimer;
        private bool _waitingForNextTurn;

        public static bool IsMouseOverButton { get; private set; }

        private void Start()
        {
            var calGO = new GameObject("ZodiacCalendar");
            _calendar = calGO.AddComponent<ZodiacCalendar>();

            var driverGO = new GameObject("TurnDriver");
            _turnDriver = driverGO.AddComponent<TurnDriver>();
            _turnDriver.Initialize(_calendar, null, null, null, 0f);

            _turnDriver.OnPhaseChanged += OnPhaseChanged;
            _turnDriver.OnTurnStarted += OnTurnStarted;

            _turnDriver.StartTurn();
        }

        private void Update()
        {
            // Auto-start next turn after a short delay
            if (_waitingForNextTurn)
            {
                _turnEndTimer += Time.deltaTime;
                if (_turnEndTimer >= 1f)
                {
                    _waitingForNextTurn = false;
                    _turnDriver.StartTurn();
                }
            }
        }

        private void OnDestroy()
        {
            if (_turnDriver != null)
            {
                _turnDriver.OnPhaseChanged -= OnPhaseChanged;
                _turnDriver.OnTurnStarted -= OnTurnStarted;
            }
        }

        private void CreateStyles()
        {
            if (_stylesCreated) return;
            _stylesCreated = true;

            _labelStyle = new GUIStyle
            {
                fontSize = 24,
                normal = { textColor = Color.white },
                alignment = TextAnchor.UpperLeft
            };

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 18
            };
        }

        private void OnGUI()
        {
            CreateStyles();

            float x = 20;
            float y = 20;
            float lineHeight = 32;

            GUI.color = new Color(0, 0, 0, 0.7f);
            GUI.DrawTexture(new Rect(10, 10, 420, lineHeight * 3 + 20), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUI.Label(new Rect(x, y, 400, lineHeight), _phaseText, _labelStyle);
            y += lineHeight;
            GUI.Label(new Rect(x, y, 400, lineHeight), _turnText, _labelStyle);
            y += lineHeight;
            GUI.Label(new Rect(x, y, 400, lineHeight), _zodiacText, _labelStyle);

            float btnW = 160;
            float btnH = 50;
            _buttonRect = new Rect(Screen.width - btnW - 20, Screen.height - btnH - 20, btnW, btnH);

            Event e = Event.current;
            IsMouseOverButton = _buttonRect.Contains(e.mousePosition);
            GameState.IsMouseOverUI = IsMouseOverButton;

            if (!_canEndTurn)
                GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

            if (GUI.Button(_buttonRect, "End Turn", _buttonStyle))
            {
                if (_canEndTurn)
                    OnEndTurnClicked();
            }

            GUI.color = Color.white;

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
            }
        }

        private void OnEndTurnClicked()
        {
            _turnDriver?.EndTurn();
            // The driver's CompleteTurn will fire the turn ended channel
            // We wait a moment then start the next turn
            _waitingForNextTurn = true;
            _turnEndTimer = 0f;
        }
    }
}
