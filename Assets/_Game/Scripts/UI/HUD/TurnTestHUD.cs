using UnityEngine;
using TalesOfTao.Core;
using TalesOfTao.Core.TurnSystem;
using TalesOfTao.Sects;

namespace TalesOfTao.UI.HUD
{
    /// <summary>
    /// Minimal test HUD for the turn system. Uses IMGUI (OnGUI).
    /// Shows phase, turn number, zodiac year, End Turn button, and build/training queue.
    /// </summary>
    public class TurnTestHUD : MonoBehaviour
    {
        private TurnDriver _turnDriver;
        private SectManager _sectManager;
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
            else if (!_turnDriver.IsActive)
            {
                var calGO = new GameObject("ZodiacCalendar_Auto");
                var calendar = calGO.AddComponent<ZodiacCalendar>();
                _turnDriver.Initialize(calendar, null, null, null, 0f);
            }

            _turnDriver.OnPhaseChanged += OnPhaseChanged;
            _turnDriver.OnTurnStarted += OnTurnStarted;

            // Find SectManager for build queue display
            _sectManager = FindAnyObjectByType<SectManager>();

            if (!_turnDriver.IsActive)
                _turnDriver.StartTurn();

            OnPhaseChanged(_turnDriver.CurrentPhase);
            OnTurnStarted(_turnDriver.TurnNumber);
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
                fontSize = 14,
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
            float lineHeight = 22;

            // --- Top-left: Turn info ---
            float topPanelH = lineHeight * 3 + 20;
            GUI.color = new Color(0, 0, 0, 0.7f);
            GUI.DrawTexture(new Rect(10, 10, 420, topPanelH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUI.Label(new Rect(x, y, 400, lineHeight), _phaseText, _labelStyle);
            y += lineHeight;
            GUI.Label(new Rect(x, y, 400, lineHeight), _turnText, _labelStyle);
            y += lineHeight;
            GUI.Label(new Rect(x, y, 400, lineHeight), _zodiacText, _labelStyle);

            // --- End Turn button ---
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

            // --- Build Queue display (below turn info) ---
            if (_sectManager != null && _sectManager.HasFoundedSect)
            {
                float queueY = topPanelH + 20;
                float queuePanelH = lineHeight * 6 + 20;

                GUI.color = new Color(0, 0, 0, 0.7f);
                GUI.DrawTexture(new Rect(10, queueY, 420, queuePanelH), Texture2D.whiteTexture);
                GUI.color = Color.white;

                float qx = 20;
                float qy = queueY + 5;

                GUI.Label(new Rect(qx, qy, 400, lineHeight), "Build Queue:", _labelStyle);
                qy += lineHeight;

                var buildQueue = _sectManager.BuildQueue;
                if (buildQueue != null && buildQueue.QueueLength > 0)
                {
                    var entries = buildQueue.GetQueue();
                    for (int i = 0; i < entries.Length && i < 3; i++)
                    {
                        var entry = entries[i];
                        if (!entry.IsComplete && !entry.IsCancelled)
                        {
                            GUI.Label(new Rect(qx, qy, 400, lineHeight),
                                "  " + entry.BuildingTypeId + " T" + entry.Tier + " (" + entry.TurnsRemaining + " turns)", _labelStyle);
                            qy += lineHeight;
                        }
                    }
                }
                else
                {
                    GUI.Label(new Rect(qx, qy, 400, lineHeight), "  (empty)", _labelStyle);
                    qy += lineHeight;
                }

                GUI.Label(new Rect(qx, qy, 400, lineHeight), "Training Queue:", _labelStyle);
                qy += lineHeight;

                var trainingQueue = _sectManager.TrainingQueue;
                if (trainingQueue != null && trainingQueue.QueueLength > 0)
                {
                    var entries = trainingQueue.GetQueue();
                    for (int i = 0; i < entries.Length && i < 3; i++)
                    {
                        var entry = entries[i];
                        if (!entry.IsComplete && !entry.IsCancelled)
                        {
                            GUI.Label(new Rect(qx, qy, 400, lineHeight),
                                "  " + entry.DiscipleName + " (" + entry.TurnsRemaining + " turns)", _labelStyle);
                            qy += lineHeight;
                        }
                    }
                }
                else
                {
                    GUI.Label(new Rect(qx, qy, 400, lineHeight), "  (empty)", _labelStyle);
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
            _turnText = "Turn " + turn;
            if (_turnDriver != null)
                _zodiacText = "Year of the " + _turnDriver.CurrentAnimal;
        }

        private void OnEndTurnClicked()
        {
            _turnDriver?.EndTurn();
            _waitingForNextTurn = true;
            _turnEndTimer = 0f;
        }
    }
}