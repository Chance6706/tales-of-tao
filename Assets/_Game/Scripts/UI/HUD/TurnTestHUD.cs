using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TalesOfTao.Core;
using TalesOfTao.Core.TurnSystem;

namespace TalesOfTao.UI.HUD
{
    public class TurnTestHUD : MonoBehaviour
    {
        [SerializeField] private TurnDriver _turnDriver;

        private TextMeshProUGUI _phaseText;
        private TextMeshProUGUI _turnText;
        private TextMeshProUGUI _zodiacText;
        private Button _endTurnButton;

        private void Start()
        {
            // Find or create turn driver
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

            CreateUI();

            _turnDriver.OnPhaseChanged += OnPhaseChanged;
            _turnDriver.OnTurnStarted += OnTurnStarted;

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
            var canvasGO = new GameObject("TurnHUD_Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            // IMPORTANT: Add an EventSystem if one doesn't exist
            if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var esGO = new GameObject("EventSystem");
                esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            canvasGO.AddComponent<GraphicRaycaster>();

            _phaseText = CreateLabel(canvasGO, "PhaseText", new Vector2(20, -20), new Vector2(400, 40), 24, Color.white);
            _turnText = CreateLabel(canvasGO, "TurnText", new Vector2(20, -60), new Vector2(400, 30), 18, Color.yellow);
            _zodiacText = CreateLabel(canvasGO, "ZodiacText", new Vector2(20, -90), new Vector2(400, 30), 18, new Color(0.8f, 0.6f, 1f));

            CreateButton(canvasGO);

            // Initial state
            _phaseText.text = "...";
            _turnText.text = "...";
            _zodiacText.text = "...";
        }

        private TextMeshProUGUI CreateLabel(GameObject parent, string name, Vector2 pos, Vector2 size, int fontSize, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            var text = go.AddComponent<TextMeshProUGUI>();
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = TMPro.TextAlignmentOptions.TopLeft;
            text.raycastTarget = false;

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;

            return text;
        }

        private void CreateButton(GameObject parent)
        {
            var btnGO = new GameObject("EndTurnButton");
            btnGO.transform.SetParent(parent.transform, false);

            var image = btnGO.AddComponent<Image>();
            image.color = new Vector4(0.2f, 0.5f, 0.8f, 0.9f);

            _endTurnButton = btnGO.AddComponent<Button>();
            _endTurnButton.onClick.AddListener(OnEndTurnClicked);
            _endTurnButton.interactable = false;

            var rect = btnGO.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(1, 0);
            rect.anchoredPosition = new Vector2(-20, 20);
            rect.sizeDelta = new Vector2(160, 50);

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
                _endTurnButton.interactable = (phase == GamePhase.Action);
        }

        private void OnTurnStarted(int turn)
        {
            if (_turnText != null)
                _turnText.text = $"Turn {turn}";

            if (_zodiacText != null && _turnDriver != null)
            {
                string animal = _turnDriver.CurrentAnimal;
                _zodiacText.text = $"Year of the {animal}";
                Debug.Log($"[TurnTestHUD] Turn {turn}, Zodiac: {animal}");
            }
        }

        private void OnEndTurnClicked()
        {
            Debug.Log("[TurnTestHUD] End Turn clicked!");
            _turnDriver?.EndTurn();
        }
    }
}
