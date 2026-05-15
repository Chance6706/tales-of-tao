using System;
using UnityEngine;
using TalesOfTao.Core;
using TalesOfTao.Core.EventChannels;
using TalesOfTao.Core.TurnSystem;

namespace TalesOfTao.Sects
{
    /// <summary>
    /// Manages sect state: income processing, compound management, upkeep.
    /// Subscribes to turn events. Processes during Income phase.
    /// Handles Build phase (construction + training) and building completion.
    /// </summary>
    public class SectManager : MonoBehaviour
    {
        [Header("Event Channels")]
        [SerializeField] private GamePhaseEventChannelSO _onPhaseChanged;
        [SerializeField] private VoidEventChannelSO _onTurnEnded;
        [SerializeField] private ZodiacBonusesEventChannelSO _onZodiacBonuses;
        [SerializeField] private StringEventChannelSO _onBuildingCompleted;
        [SerializeField] private StringEventChannelSO _onDiscipleTrained;
        [SerializeField] private VoidEventChannelSO _onPeonRecruited;

        [Header("State")]
        [SerializeField] private SectData _sectData;

        [Header("Systems")]
        [SerializeField] private BuildQueue _buildQueue;
        [SerializeField] private TrainingQueue _trainingQueue;
        [SerializeField] private BuildingFactory _buildingFactory;

        public SectData Data => _sectData;
        public bool HasFoundedSect => _sectData.IsFounded;
        public BuildQueue BuildQueue => _buildQueue;
        public TrainingQueue TrainingQueue => _trainingQueue;
        public BuildingFactory BuildingFactory => _buildingFactory;

        public event Action<SectData> OnSectFounded;
        public event Action<string> OnResourceChanged;
        public event Action<string> OnBuildingCompleted;
        public event Action<string, DiscipleRank> OnDisciplePromoted;

        private void OnEnable()
        {
            if (_onPhaseChanged != null)
                _onPhaseChanged.Subscribe(OnPhaseChanged);
            if (_onTurnEnded != null)
                _onTurnEnded.Subscribe(OnTurnEnded);
            if (_onZodiacBonuses != null)
                _onZodiacBonuses.Subscribe(OnZodiacBonuses);
            if (_onBuildingCompleted != null)
                _onBuildingCompleted.Subscribe(OnBuildingCompletedEvent);
            if (_onDiscipleTrained != null)
                _onDiscipleTrained.Subscribe(OnDiscipleTrainedEvent);
        }

        private void OnDisable()
        {
            if (_onPhaseChanged != null)
                _onPhaseChanged.Unsubscribe(OnPhaseChanged);
            if (_onTurnEnded != null)
                _onTurnEnded.Unsubscribe(OnTurnEnded);
            if (_onZodiacBonuses != null)
                _onZodiacBonuses.Unsubscribe(OnZodiacBonuses);
            if (_onBuildingCompleted != null)
                _onBuildingCompleted.Unsubscribe(OnBuildingCompletedEvent);
            if (_onDiscipleTrained != null)
                _onDiscipleTrained.Unsubscribe(OnDiscipleTrainedEvent);
        }

        /// <summary>
        /// Called by FoundSectCommand after successful execution.
        /// </summary>
        public void SetSectData(SectData data)
        {
            _sectData = data;
            OnSectFounded?.Invoke(_sectData);
        }

        /// <summary>
        /// Sets the build queue reference. Called during initialization.
        /// </summary>
        public void SetBuildQueue(BuildQueue queue)
        {
            _buildQueue = queue;
        }

        /// <summary>
        /// Sets the training queue reference. Called during initialization.
        /// </summary>
        public void SetTrainingQueue(TrainingQueue queue)
        {
            _trainingQueue = queue;
        }

        /// <summary>
        /// Sets the building factory reference. Called during initialization.
        /// </summary>
        public void SetBuildingFactory(BuildingFactory factory)
        {
            _buildingFactory = factory;
        }

        private void OnPhaseChanged(GamePhase phase)
        {
            if (!_sectData.IsFounded) return;

            switch (phase)
            {
                case GamePhase.Income:
                    ProcessIncome();
                    break;
                case GamePhase.Build:
                    ProcessBuildPhase();
                    break;
                case GamePhase.Resolution:
                    ProcessDissent();
                    break;
            }
        }

        private void OnTurnEnded()
        {
            // End-of-turn cleanup if needed
        }

        private void OnZodiacBonuses(ZodiacBonuses bonuses)
        {
            // Store current bonuses for income calculations
            // Applied during Income phase
        }

        /// <summary>
        /// Handles building construction and disciple training during Build phase.
        /// </summary>
        private void ProcessBuildPhase()
        {
            if (_buildQueue != null)
            {
                _buildQueue.ProcessBuildPhase();
            }

            if (_trainingQueue != null)
            {
                _trainingQueue.ProcessBuildPhase();
            }
        }

        /// <summary>
        /// Called when a building finishes construction (via BuildQueue event).
        /// Creates the building GameObject and adds it to SectData.
        /// </summary>
        private void OnBuildingCompletedEvent(string buildingTypeId)
        {
            if (_buildingFactory == null || _sectData == null) return;

            // Note: The event only passes the buildingTypeId. In a full implementation,
            // we'd include the tier as well. For now, default to tier 1.
            _sectData.AddBuilding(buildingTypeId, 1, Vector3.zero);
            OnBuildingCompleted?.Invoke(buildingTypeId);

            Debug.Log($"[SectManager] Building completed: {buildingTypeId}");
        }

        /// <summary>
        /// Called when a disciple finishes training (via TrainingQueue event).
        /// Promotes the disciple in SectData.
        /// </summary>
        private void OnDiscipleTrainedEvent(string discipleName)
        {
            if (_sectData == null) return;

            var disciple = _sectData.FindDisciple(discipleName);
            if (disciple == null)
            {
                Debug.LogWarning($"[SectManager] Trained disciple not found: {discipleName}");
                return;
            }

            var oldRank = disciple.Rank;
            if (_sectData.PromoteDisciple(discipleName))
            {
                var newDisciple = _sectData.FindDisciple(discipleName);
                OnDisciplePromoted?.Invoke(discipleName, newDisciple.Rank);
                Debug.Log($"[SectManager] Disciple promoted: {discipleName} {oldRank} -> {newDisciple.Rank}");
            }
        }

        /// <summary>
        /// Processes Tael and Qi income for the turn.
        /// Called during Income phase.
        /// </summary>
        private void ProcessIncome()
        {
            if (!_sectData.IsFounded) return;

            // Base Qi income from founding tile
            float qiIncome = _sectData.FoundingStats.BaseQiIncome + _sectData.FoundingStats.CaveBonus;

            // Apply zodiac bonus if applicable
            // TODO: Apply zodiac multiplier

            // Apply sect trait bonus
            if (_sectData.Config != null)
            {
                switch (_sectData.Config.Trait)
                {
                    case SectTrait.QiBonus:
                        qiIncome *= 1.20f;
                        break;
                    case SectTrait.TaelBonus:
                        _sectData.Stockpile.Tael += Mathf.RoundToInt(5); // +10% of base 50
                        break;
                }
            }

            _sectData.Stockpile.Qi += Mathf.RoundToInt(qiIncome);

            // Deduct upkeep
            int upkeep = _sectData.CalculateUpkeep();
            _sectData.Stockpile.Tael -= upkeep;

            OnResourceChanged?.Invoke("Tael");
            OnResourceChanged?.Invoke("Qi");
        }

        /// <summary>
        /// Processes Dissent accumulation/recovery.
        /// Called during Resolution phase.
        /// </summary>
        private void ProcessDissent()
        {
            if (!_sectData.IsFounded) return;

            int dissentRate = _sectData.CalculateDissentRate();
            if (dissentRate > 0)
            {
                _sectData.DissentLevel += dissentRate;
            }
            else
            {
                // Recover 5 per turn when ratios are in bounds
                _sectData.DissentLevel = Mathf.Max(0, _sectData.DissentLevel - 5);
            }
        }

        /// <summary>
        /// Adds Tael to the stockpile.
        /// </summary>
        public void AddTael(int amount)
        {
            _sectData.Stockpile.Tael += amount;
            OnResourceChanged?.Invoke("Tael");
        }

        /// <summary>
        /// Adds Qi to the stockpile.
        /// </summary>
        public void AddQi(int amount)
        {
            _sectData.Stockpile.Qi += amount;
            OnResourceChanged?.Invoke("Qi");
        }

        /// <summary>
        /// Attempts to spend Tael. Returns true if successful.
        /// </summary>
        public bool TrySpendTael(int amount)
        {
            if (_sectData.Stockpile.Tael < amount) return false;
            _sectData.Stockpile.Tael -= amount;
            OnResourceChanged?.Invoke("Tael");
            return true;
        }
    }
}
