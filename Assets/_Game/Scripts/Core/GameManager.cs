using TalesOfTao.Core.Commands;
using TalesOfTao.Core.EventChannels;
using TalesOfTao.Core.TurnSystem;
using UnityEngine;

namespace TalesOfTao.Core
{
    [AddComponentMenu("")]
    [DefaultExecutionOrder(-100)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Event Channels")]
        [SerializeField] private GamePhaseEventChannelSO _onPhaseChanged;
        [SerializeField] private VoidEventChannelSO _onTurnEnded;
        [SerializeField] private StringEventChannelSO _onResourceChanged;
        [SerializeField] private VoidEventChannelSO _onUnitMoved;
        [SerializeField] private VoidEventChannelSO _onCombatResolved;
        [SerializeField] private ZodiacBonusesEventChannelSO _onZodiacBonuses;

        [Header("Turn System")]
        [SerializeField] private TurnStateMachine _turnStateMachine;
        [SerializeField] private ZodiacCalendar _zodiacCalendar;
        [SerializeField] private PhaseInputController _phaseInputController;

        public GamePhaseEventChannelSO OnPhaseChanged    => _onPhaseChanged;
        public VoidEventChannelSO      OnTurnEnded       => _onTurnEnded;
        public StringEventChannelSO    OnResourceChanged => _onResourceChanged;
        public VoidEventChannelSO      OnUnitMoved       => _onUnitMoved;
        public VoidEventChannelSO      OnCombatResolved  => _onCombatResolved;
        public ZodiacBonusesEventChannelSO OnZodiacBonuses => _onZodiacBonuses;

        public TurnStateMachine TurnStateMachine => _turnStateMachine;
        public ZodiacCalendar ZodiacCalendar => _zodiacCalendar;
        public PhaseInputController PhaseInputController => _phaseInputController;

        public CommandQueue PlayerCommandQueue { get; } = new();

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"[GameManager] Duplicate instance on '{gameObject.name}' — destroying it. " +
                               $"Existing instance is on '{Instance.gameObject.name}'.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            ValidateChannels();
            ValidateTurnSystem();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void ValidateChannels()
        {
            if (_onPhaseChanged    == null) Debug.LogWarning("[GameManager] OnPhaseChanged channel not assigned.");
            if (_onTurnEnded       == null) Debug.LogWarning("[GameManager] OnTurnEnded channel not assigned.");
            if (_onResourceChanged == null) Debug.LogWarning("[GameManager] OnResourceChanged channel not assigned.");
            if (_onUnitMoved       == null) Debug.LogWarning("[GameManager] OnUnitMoved channel not assigned.");
            if (_onCombatResolved  == null) Debug.LogWarning("[GameManager] OnCombatResolved channel not assigned.");
            if (_onZodiacBonuses   == null) Debug.LogWarning("[GameManager] OnZodiacBonuses channel not assigned.");
        }

        private void ValidateTurnSystem()
        {
            if (_turnStateMachine == null) Debug.LogWarning("[GameManager] TurnStateMachine not assigned.");
            if (_zodiacCalendar == null) Debug.LogWarning("[GameManager] ZodiacCalendar not assigned.");
            if (_phaseInputController == null) Debug.LogWarning("[GameManager] PhaseInputController not assigned.");
        }
    }
}
