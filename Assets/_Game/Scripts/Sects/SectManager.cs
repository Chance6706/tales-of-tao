using System;
using UnityEngine;
using TalesOfTao.Core;
using TalesOfTao.Core.EventChannels;
using TalesOfTao.Core.TurnSystem;

namespace TalesOfTao.Sects
{
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
        public bool HasFoundedSect => _sectData != null && _sectData.IsFounded;
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

        public void SetSectData(SectData data)
        {
            _sectData = data;
            OnSectFounded?.Invoke(_sectData);
        }

        public void SetBuildQueue(BuildQueue queue)
        {
            _buildQueue = queue;
        }

        public void SetTrainingQueue(TrainingQueue queue)
        {
            _trainingQueue = queue;
        }

        public void SetBuildingFactory(BuildingFactory factory)
        {
            _buildingFactory = factory;
        }

        private void OnPhaseChanged(GamePhase phase)
        {
            if (_sectData == null || !_sectData.IsFounded) return;

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

        private void OnTurnEnded() { }

        private void OnZodiacBonuses(ZodiacBonuses bonuses) { }

        private void ProcessBuildPhase()
        {
            if (_buildQueue != null)
                _buildQueue.ProcessBuildPhase();
            if (_trainingQueue != null)
                _trainingQueue.ProcessBuildPhase();
        }

        private void OnBuildingCompletedEvent(string buildingTypeId)
        {
            if (_sectData == null) return;

            _sectData.AddBuilding(buildingTypeId, 1, Vector3.zero);
            OnBuildingCompleted?.Invoke(buildingTypeId);

            // When Training Grounds completes, auto-start training a peon if available
            if (buildingTypeId == "TrainingGrounds" && _trainingQueue != null)
            {
                AutoEnqueueTraining();
            }

            Debug.Log("[SectManager] Building completed: " + buildingTypeId);
        }

        /// <summary>
        /// Automatically finds an untrained peon and starts training them to Outer Disciple.
        /// Training Grounds T1: 8 turns, cap 5/batch.
        /// </summary>
        private void AutoEnqueueTraining()
        {
            if (_sectData == null || _trainingQueue == null) return;

            // Find a peon that's not already being trained
            var disciples = _sectData.GetDisciples();
            foreach (var d in disciples)
            {
                if (d.Rank == DiscipleRank.Peon && d.IsAlive)
                {
                    // Check if this peon is already in the training queue
                    bool alreadyTraining = false;
                    var queue = _trainingQueue.GetQueue();
                    foreach (var entry in queue)
                    {
                        if (entry.DiscipleName == d.Name && !entry.IsComplete && !entry.IsCancelled)
                        {
                            alreadyTraining = true;
                            break;
                        }
                    }

                    if (!alreadyTraining && _trainingQueue.CanQueue(DiscipleRank.Peon, DiscipleRank.OuterDisciple))
                    {
                        // Training Grounds T1: 8 turns per GDD
                        _trainingQueue.Enqueue(d.Name, DiscipleRank.Peon, DiscipleRank.OuterDisciple, 8);
                        Debug.Log("[SectManager] Auto-enqueued training for " + d.Name + " (Peon -> Outer Disciple, 8 turns)");
                        return; // Only enqueue one per building completion
                    }
                }
            }

            Debug.Log("[SectManager] Training Grounds complete but no available peons to train");
        }

        private void OnDiscipleTrainedEvent(string discipleName)
        {
            if (_sectData == null) return;

            var disciple = _sectData.FindDisciple(discipleName);
            if (disciple == null)
            {
                Debug.LogWarning("[SectManager] Trained disciple not found: " + discipleName);
                return;
            }

            var oldRank = disciple.Rank;
            if (_sectData.PromoteDisciple(discipleName))
            {
                var newDisciple = _sectData.FindDisciple(discipleName);
                OnDisciplePromoted?.Invoke(discipleName, newDisciple.Rank);
                Debug.Log("[SectManager] Disciple promoted: " + discipleName + " " + oldRank + " -> " + newDisciple.Rank);
            }
        }

        private void ProcessIncome()
        {
            if (_sectData == null || !_sectData.IsFounded) return;

            float qiIncome = _sectData.FoundingStats.BaseQiIncome + _sectData.FoundingStats.CaveBonus;

            if (_sectData.Config != null)
            {
                switch (_sectData.Config.Trait)
                {
                    case SectTrait.QiBonus:
                        qiIncome *= 1.20f;
                        break;
                    case SectTrait.TaelBonus:
                        _sectData.Stockpile.Tael += Mathf.RoundToInt(5);
                        break;
                }
            }

            _sectData.Stockpile.Qi += Mathf.RoundToInt(qiIncome);

            int upkeep = _sectData.CalculateUpkeep();
            _sectData.Stockpile.Tael -= upkeep;

            OnResourceChanged?.Invoke("Tael");
            OnResourceChanged?.Invoke("Qi");
        }

        private void ProcessDissent()
        {
            if (_sectData == null || !_sectData.IsFounded) return;

            int dissentRate = _sectData.CalculateDissentRate();
            if (dissentRate > 0)
                _sectData.DissentLevel += dissentRate;
            else
                _sectData.DissentLevel = Mathf.Max(0, _sectData.DissentLevel - 5);
        }

        public void AddTael(int amount)
        {
            _sectData.Stockpile.Tael += amount;
            OnResourceChanged?.Invoke("Tael");
        }

        public void AddQi(int amount)
        {
            _sectData.Stockpile.Qi += amount;
            OnResourceChanged?.Invoke("Qi");
        }

        public bool TrySpendTael(int amount)
        {
            if (_sectData.Stockpile.Tael < amount) return false;
            _sectData.Stockpile.Tael -= amount;
            OnResourceChanged?.Invoke("Tael");
            return true;
        }
    }
}
