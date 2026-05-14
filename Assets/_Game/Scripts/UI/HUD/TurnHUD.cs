using UnityEngine;
using TalesOfTao.Core;
using TalesOfTao.Core.TurnSystem;

namespace TalesOfTao.UI.HUD
{
    /// <summary>
    /// HUD strip showing current phase, zodiac year, End Turn button, and resource counters.
    /// Uses Unity UI (Canvas + TextMeshPro) for the initial implementation.
    /// TODO: Migrate to UI Toolkit (UXML/USS) for wuxia-themed styling.
    /// </summary>
    public class TurnHUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TurnStateMachine _turnStateMachine;
        [SerializeField] private ZodiacCalendar _zodiacCalendar;

        [Header("Phase Display")]
        [SerializeField] private TMPro.TextMeshProUGUI _phaseText;
        [SerializeField] private TMPro.TextMeshProUGUI _turnText;
        [SerializeField] private TMPro.TextMeshProUGUI _zodiacText;

        [Header("Resource Display")]
        [SerializeField] private TMPro.TextMeshProUGUI _taelText;
        [SerializeField] private TMPro.TextMeshProUGUI _qiText;

        [Header("End Turn")]
        [SerializeField] private UnityEngine.UI.Button _endTurnButton;

        private void Start()
        {
            if (_endTurnButton != null)
            {
                _endTurnButton.onClick.AddListener(OnEndTurnClicked);
            }

            UpdatePhaseDisplay(GamePhase.Event);
            UpdateTurnDisplay(0);
            UpdateZodiacDisplay("None");
            UpdateResourceDisplay(0, 0);
        }

        private void OnEnable()
        {
            if (_turnStateMachine != null)
            {
                _turnStateMachine.OnPhaseEntered += OnPhaseEntered;
            }
        }

        private void OnDisable()
        {
            if (_turnStateMachine != null)
            {
                _turnStateMachine.OnPhaseEntered -= OnPhaseEntered;
            }

            if (_endTurnButton != null)
            {
                _endTurnButton.onClick.RemoveListener(OnEndTurnClicked);
            }
        }

        private void OnPhaseEntered(GamePhase phase)
        {
            UpdatePhaseDisplay(phase);
            UpdateEndTurnButton(phase == GamePhase.Action);
        }

        private void OnEndTurnClicked()
        {
            _turnStateMachine?.EndTurn();
        }

        private void Update()
        {
            // Update turn/zodiac display each frame in case calendar changes
            if (_zodiacCalendar != null)
            {
                UpdateTurnDisplay(_zodiacCalendar.CurrentTurn);
                UpdateZodiacDisplay(_zodiacCalendar.CurrentAnimal);
            }
        }

        public void UpdatePhaseDisplay(GamePhase phase)
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

        public void UpdateTurnDisplay(int turn)
        {
            if (_turnText != null)
            {
                _turnText.text = $"Turn {turn}";
            }
        }

        public void UpdateZodiacDisplay(string animal)
        {
            if (_zodiacText != null)
            {
                _zodiacText.text = $"Year of the {animal}";
            }
        }

        public void UpdateResourceDisplay(int tael, int qi)
        {
            if (_taelText != null)
            {
                _taelText.text = $"Tael: {tael}";
            }
            if (_qiText != null)
            {
                _qiText.text = $"Qi: {qi}";
            }
        }

        private void UpdateEndTurnButton(bool enabled)
        {
            if (_endTurnButton != null)
            {
                _endTurnButton.interactable = enabled;
            }
        }
    }
}
