using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TalesOfTao.Core;
using TalesOfTao.Core.TurnSystem;

namespace TalesOfTao.UI.HUD
{
    /// <summary>
    /// Minimal test HUD for the turn system. Creates its own Canvas.
    /// Shows current phase, turn number, zodiac year, and End Turn button.
    /// </summary>
    public class TurnTestHUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TurnDriver _turnDriver;

        [Header("UI Elements (auto-created)")]
        [SerializeField] private TextMeshProUGUI _phaseText;
        [SerializeField] private TextMeshProUGUI _turnText;
        [SerializeField] private TextMeshProUGUI _zodiacText;
        [SerializeField] private Button _endTurnButton;

        private Canvas _canvas;

        public void Initialize(TurnDriver driver)
        {
            _turnDriver = driver;
            CreateUI();

            if (_turnDriver != null)
            {
                _turnDriver.OnPhaseChanged += OnPhaseChanged;
                _turnDriver.OnTurnStarted += OnTurnStarted;
            }
        }

        private void Start()
        {
            // Auto-create turn system if not assigned
            if (_turnDriver == null)
            {
                var existing = FindObjectOfType<TurnDriver>();
                if (existing != null)
                {
                    Initialize(existing);
                }
                else
                {
                    // Create a minimal self-contained turn system
                    var driverGO = new GameObject("TurnDriver_Auto");
                    _turnDriver = driverGO.AddComponent<TurnDriver>();

                    var calGO = new GameObject("ZodiacCalendar_Auto");
                    var calendar = calGO.AddComponent<ZodiacCalendar>();
                    _turnDriver.Initialize(calendar, null, null, null, 0f);

                    CreateUI();
                    _turnDriver.OnPhaseChanged += OnPhaseChanged;
                    _turnDriver.OnTurnStarted += OnTurnStarted;
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

        private void CreateUI()
        {
            // Create Canvas
            var canvasGO = new GameObject("TurnHUD_Canvas");
            _canvas = canvasGO.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 100;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            // Create phase text
            var phaseGO = new GameObject("PhaseText");
            phaseGO.transform.SetParent(canvasGO.transform, false);
            _phaseText = phaseGO.AddComponent<TextMeshProUGUI>();
            _phaseText.fontSize = 24;
            _phaseText.color = Color.white;
            _phaseText.alignment = TextAlignmentOptions.TopLeft;
            var phaseRect = phaseGO.GetComponent<RectTransform>();
            phaseRect.anchorMin = new Vector2(0, 1);
            phaseRect.anchorMax = new Vector2(0, 1);
            phaseRect.pivot = new Vector2(0, 1);
            phaseRect.anchoredPosition = new Vector2(20, -20);
            phaseRect.sizeDelta = new Vector2(400, 40);

            // Create turn text
            var turnGO = new GameObject("TurnText");
            turnGO.transform.SetParent(canvasGO.transform, false);
            _turnText = turnGO.AddComponent<TextMeshProUGUI>();
            _turnText.fontSize = 18;
            _turnText.color = Color.yellow;
            _turnText.alignment = TextAlignmentOptions.TopLeft;
            var turnRect = turnGO.GetComponent<RectTransform>();
            turnRect.anchorMin = new Vector2(0, 1);
            turnRect.anchorMax = new Vector2(0, 1);
            turnRect.pivot = new Vector2(0, 1);
            turnRect.anchoredPosition = new Vector2(20, -60);
            turnRect.sizeDelta = new Vector2(400, 30);

            // Create zodiac text
            var zodiacGO = new GameObject("ZodiacText");
            zodiacGO.transform.SetParent(canvasGO.transform, false);
            _zodiacText = zodiacGO.AddComponent<TextMeshProUGUI>();
            _zodiacText.fontSize = 18;
            _zodiacText.color = new Color(0.8f, 0.6f, 1f);
            _zodiacText.alignment = TextAlignmentOptions.TopLeft;
            var zodiacRect = zodiacGO.GetComponent<RectTransform>();
            zodiacRect.anchorMin = new Vector2(0, 1);
            zodiacRect.anchorMax = new Vector2(0, 1);
            zodiacRect.pivot = new Vector2(0, 1);
            zodiacRect.anchoredPosition = new Vector2(20, -90);
            zodiacRect.sizeDelta = new Vector2(400, 30);

            // Create End Turn button
            var btnGO = new GameObject("EndTurnButton");
            btnGO.transform.SetParent(canvasGO.transform, false);
            _endTurnButton = btnGO.AddComponent<Button>();
            var btnImage = btnGO.AddComponent<Image>();
            btnImage.color = new Color(0.2f, 0.5f, 0.8f, 0.9f);
            var btnRect = btnGO.GetComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(1, 0);
            btnRect.anchorMax = new Vector2(1, 0);
            btnRect.pivot = new Vector2(1, 0);
            btnRect.anchoredPosition = new Vector2(-20, 20);
            btnRect.sizeDelta = new Vector2(160, 50);

            // Button text
            var btnTextGO = new GameObject("Text");
            btnTextGO.transform.SetParent(btnGO.transform, false);
            var btnText = btnTextGO.AddComponent<TextMeshProUGUI>();
            btnText.text = "End Turn";
            btnText.fontSize = 20;
            btnText.color = Color.white;
            btnText.alignment = TextAlignmentOptions.Center;
            var btnTextRect = btnTextGO.GetComponent<RectTransform>();
            btnTextRect.anchorMin = Vector2.zero;
            btnTextRect.anchorMax = Vector2.one;
            btnTextRect.sizeDelta = Vector2.zero;

            _endTurnButton.onClick.AddListener(OnEndTurnClicked);

            UpdatePhaseDisplay(GamePhase.Event);
            UpdateTurnDisplay(0);
            UpdateZodiacDisplay("None");
        }

        private void OnPhaseChanged(GamePhase phase)
        {
            UpdatePhaseDisplay(phase);
            if (_endTurnButton != null)
            {
                _endTurnButton.interactable = (phase == GamePhase.Action);
            }
        }

        private void OnTurnStarted(int turn)
        {
            UpdateTurnDisplay(turn);
            if (_turnDriver != null)
            {
                UpdateZodiacDisplay(_turnDriver.CurrentAnimal);
            }
        }

        private void OnEndTurnClicked()
        {
            _turnDriver?.EndTurn();
        }

        private void UpdatePhaseDisplay(GamePhase phase)
        {
            if (_phaseText != null)
            {
                _phaseText.text = phase switch
                {
                    GamePhase.Event      => "Event Phase",
                    GamePhase.Income     => "Income Phase",
                    GamePhase.Build      => "Build Phase",
                    GamePhase.Research   => "Research Phase",
                    GamePhase.Action     => "Action Phase",
                    GamePhase.Resolution => "Resolution Phase",
                    _                    => "Unknown"
                };
            }
        }

        private void UpdateTurnDisplay(int turn)
        {
            if (_turnText != null)
            {
                _turnText.text = $"Turn {turn}";
            }
        }

        private void UpdateZodiacDisplay(string animal)
        {
            if (_zodiacText != null)
            {
                _zodiacText.text = $"Year of the {animal}";
            }
        }
    }
}
