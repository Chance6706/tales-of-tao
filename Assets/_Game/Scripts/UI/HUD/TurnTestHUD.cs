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

        private Canvas _canvas;
        private TextMeshProUGUI _phaseText;
        private TextMeshProUGUI _turnText;
        private TextMeshProUGUI _zodiacText;
        private Button _endTurnButton;

        private void Start()
        {
            // Find or create turn driver
            if (_turnDriver == null)
                _turnDriver = FindFirstObjectByType<TurnDriver>();

            if (_turnDriver == null)
            {
                // Create a minimal self-contained turn system
                var calGO = new GameObject("ZodiacCalendar");
                var calendar = calGO.AddComponent<ZodiacCalendar>();

                var driverGO = new GameObject("TurnDriver");
                _turnDriver = driverGO.AddComponent<TurnDriver>();
                _turnDriver.Initialize(calendar, null, null, null, 0f);
            }

            CreateUI();

            _turnDriver.OnPhaseChanged += OnPhaseChanged;
            _turnDriver.OnTurnStarted += OnTurnStarted;

            // Start the first turn
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

        private void CreateUI()
        {
            // Create Canvas
            var canvasGO = new GameObject("TurnHUD_Canvas");
            _canvas = canvasGO.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 100;

            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            // GraphicRaycaster is needed for button clicks but blocks 3D raycasts.
            // We make the canvas NOT block raycasts on 3D objects by using a separate
            // camera for the UI or by disabling raycast target on background elements.
            var raycaster = canvasGO.AddComponent<GraphicRaycaster>();

            // Phase text (top-left)
            _phaseText = CreateText(canvasGO, "PhaseText", new Vector2(20, -20), new Vector2(400, 40), 24, Color.white, TextAnchor.UpperLeft);
            _phaseText.text = "Event Phase";

            // Turn text
            _turnText = CreateText(canvasGO, "TurnText", new Vector2(20, -60), new Vector2(400, 30), 18, Color.yellow, TextAnchor.UpperLeft);
            _turnText.text = "Turn 0";

            // Zodiac text
            _zodiacText = CreateText(canvasGO, "ZodiacText", new Vector2(20, -90), new Vector2(400, 30), 18, new Color(0.8f, 0.6f, 1f), TextAnchor.UpperLeft);
            _zodiacText.text = "Year of the None";

            // End Turn button (bottom-right)
            CreateEndTurnButton(canvasGO);
        }

        private TextMeshProUGUI CreateText(Canvas canvas, string name, Vector2 pos, Vector2 size, int fontSize, Color color, TextAnchor alignment)
        {
            var go = new GameObject(name);
            go.transform.SetParent(canvas.transform, false);
            var text = go.AddComponent<TextMeshProUGUI>();
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment switch
            {
                TextAnchor.UpperLeft => TMPro.TextAlignmentOptions.TopLeft,
                TextAnchor.UpperRight => TMPro.TextAlignmentOptions.TopRight,
                TextAnchor.LowerLeft => TMPro.TextAlignmentOptions.BottomLeft,
                TextAnchor.LowerRight => TMPro.TextAlignmentOptions.BottomRight,
                _ => TMPro.TextAlignmentOptions.Center
            };

            // Disable raycast target so it doesn't block 3D clicks
            text.raycastTarget = false;

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;

            return text;
        }

        private void CreateEndTurnButton(Canvas canvas)
        {
            var btnGO = new GameObject("EndTurnButton");
            btnGO.transform.SetParent(canvas.transform, false);

            var image = btnGO.AddComponent<Image>();
            image.color = new Vector4(0.2f, 0.5f, 0.8f, 0.9f);

            _endTurnButton = btnGO.AddComponent<Button>();
            _endTurnButton.onClick.AddListener(OnEndTurnClicked);

            var rect = btnGO.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(1, 0);
            rect.anchoredPosition = new Vector2(-20, 20);
            rect.sizeDelta = new Vector2(160, 50);

            // Button text
            var textGO = new GameObject("Text");
            textGO.transform.SetParent(btnGO.transform, false);
            var text = textGO.AddComponent<TextMeshProUGUI>();
            text.text = "End Turn";
            text.fontSize = 20;
            text.color = Color.white;
            text.alignment = TMPro.TextAlignmentOptions.Center;
            text.raycastTarget = false;

            var textRect = textGO.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;

            _endTurnButton.interactable = false;
        }

        private void OnPhaseChanged(GamePhase phase)
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

            if (_endTurnButton != null)
            {
                _endTurnButton.interactable = (phase == GamePhase.Action);
            }
        }

        private void OnTurnStarted(int turn)
        {
            if (_turnText != null)
                _turnText.text = $"Turn {turn}";

            if (_zodiacText != null && _turnDriver != null)
                _zodiacText.text = $"Year of the {_turnDriver.CurrentAnimal}";
        }

        private void OnEndTurnClicked()
        {
            _turnDriver?.EndTurn();
        }
    }
}
