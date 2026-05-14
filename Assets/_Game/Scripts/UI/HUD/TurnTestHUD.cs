using UnityEngine;
using TalesOfTao.Core;
using TalesOfTao.Core.TurnSystem;

namespace TalesOfTao.UI.HUD
{
    /// <summary>
    /// Minimal test HUD for the turn system. Uses IMGUI (OnGUI).
    /// Shows phase, turn number, zodiac year, and End Turn button.
    /// Auto-creates turn system if not present in scene.
    /// </summary>
    public class TurnTestHUD : MonoBehaviour
    {
        private TurnDriver _turnDriver;
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

        /// <summary>
        /// True while mouse is over the End Turn button. TileSelector checks this.
        /// </summary>
        public static bool IsMouseOverButton { get; private set; }

        private void Start()
        {
            Debug.Log("[TurnTestHUD] Start called");
            if (_turnDriver == null)
                _turnDriver = FindAnyObjectByType<TurnDriver>();

            if (_turnDriver == null)
            {
                Debug.Log("[TurnTestHUD] Creating new turn system");
                var calGO = new GameObject("ZodiacCalendar");
                var calendar = calGO.AddComponent<ZodiacCalendar>();
                var driverGO = new GameObject("TurnDriver");
                _turnDriver = driverGO.AddComponent<TurnDriver>();
                _turnDriver.Initialize(calendar, null, null, null, 0f);
            }
            else
            {
                Debug.Log($"[TurnTestHUD] Found existing driver, IsActive={_turnDriver.IsActive}");
            }

            _turnDriver.OnPhaseChanged += OnPhaseChanged;
            _turnDriver.OnTurnStarted += OnTurnStarted;

            if (!_turnDriver.IsActive)
            {
                Debug.Log("[TurnTestHUD] Calling StartTurn");
                _turnDriver.StartTurn();
                Debug.Log($"[TurnTestHUD] After StartTurn, IsActive={_turnDriver.IsActive}, Phase={_turnDriver.CurrentPhase}");
            }

            // Force initial UI update with current state
            Debug.Log($"[TurnTestHUD] Forcing update: Phase={_turnDriver.CurrentPhase}, Turn={_turnDriver.TurnNumber}, Animal={_turnDriver.CurrentAnimal}");
            OnPhaseChanged(_turnDriver.CurrentPhase);
            OnTurnStarted(_turnDriver.TurnNumber);
            Debug.Log($"[TurnTestHUD] After update: phaseText={_phaseText}, turnText={_turnText}, zodiacText={_zodiacText}");
        }

        private void Update()
        {
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

            // Dark background for readability
            GUI.color = new Color(0, 0, 0, 0.7f);
            GUI.DrawTexture(new Rect(10, 10, 420, lineHeight * 3 + 20), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUI.Label(new Rect(x, y, 400, lineHeight), _phaseText, _labelStyle);
            y += lineHeight;
            GUI.Label(new Rect(x, y, 400, lineHeight), _turnText, _labelStyle);
            y += lineHeight;
            GUI.Label(new Rect(x, y, 400, lineHeight), _zodiacText, _labelStyle);

            // End Turn button
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
            }
        }

        private void OnEndTurnClicked()
        {
            _turnDriver?.EndTurn();
            _waitingForNextTurn = true;
            _turnEndTimer = 0f;
        }
    }
}
