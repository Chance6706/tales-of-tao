using TalesOfTao.Core.Commands;
using TalesOfTao.Core.EventChannels;
using UnityEngine;

namespace TalesOfTao.Core
{
    // Scene entry point and global service locator for event channels.
    // Add to a GameObject in Main.unity and wire channel assets in the Inspector.
    //
    // Channel assets are created via:
    //   Assets > Create > TalesOfTao > Events > <Type> Event Channel
    // then dragged into the corresponding field here.
    [DefaultExecutionOrder(-100)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Event Channels")]
        [SerializeField] private GamePhaseEventChannelSO _onPhaseChanged;
        [SerializeField] private VoidEventChannelSO      _onTurnEnded;
        [SerializeField] private StringEventChannelSO    _onResourceChanged;
        [SerializeField] private VoidEventChannelSO      _onUnitMoved;
        [SerializeField] private VoidEventChannelSO      _onCombatResolved;

        public GamePhaseEventChannelSO OnPhaseChanged    => _onPhaseChanged;
        public VoidEventChannelSO      OnTurnEnded       => _onTurnEnded;
        public StringEventChannelSO    OnResourceChanged => _onResourceChanged;
        public VoidEventChannelSO      OnUnitMoved       => _onUnitMoved;
        public VoidEventChannelSO      OnCombatResolved  => _onCombatResolved;

        [SerializeField] private VoidEventChannelSO _onTurnEnded;
        [SerializeField] private StringEventChannelSO _onResourceChanged;
        [SerializeField] private VoidEventChannelSO _onUnitMoved;
        [SerializeField] private VoidEventChannelSO _onCombatResolved;

        // Public accessors — other systems read these references rather than
        // holding direct dependencies on GameManager.
        public GamePhaseEventChannelSO OnPhaseChanged => _onPhaseChanged;
        public VoidEventChannelSO OnTurnEnded => _onTurnEnded;
        public StringEventChannelSO OnResourceChanged => _onResourceChanged;
        public VoidEventChannelSO OnUnitMoved => _onUnitMoved;
        public VoidEventChannelSO OnCombatResolved => _onCombatResolved;

        // Shared command queue for the current player's Action Phase.
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
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (Instance == this)
                Instance = null;
        }

        private void ValidateChannels()
        {
            if (_onPhaseChanged    == null) Debug.LogWarning("[GameManager] OnPhaseChanged channel not assigned.");
            if (_onTurnEnded       == null) Debug.LogWarning("[GameManager] OnTurnEnded channel not assigned.");
            if (_onResourceChanged == null) Debug.LogWarning("[GameManager] OnResourceChanged channel not assigned.");
            if (_onUnitMoved       == null) Debug.LogWarning("[GameManager] OnUnitMoved channel not assigned.");
            if (_onCombatResolved  == null) Debug.LogWarning("[GameManager] OnCombatResolved channel not assigned.");
            if (_onPhaseChanged == null)
                Debug.LogWarning("[GameManager] OnPhaseChanged channel not assigned.");
            if (_onTurnEnded == null)
                Debug.LogWarning("[GameManager] OnTurnEnded channel not assigned.");
            if (_onResourceChanged == null)
                Debug.LogWarning("[GameManager] OnResourceChanged channel not assigned.");
            if (_onUnitMoved == null)
                Debug.LogWarning("[GameManager] OnUnitMoved channel not assigned.");
            if (_onCombatResolved == null)
                Debug.LogWarning("[GameManager] OnCombatResolved channel not assigned.");
        }
    }
}
