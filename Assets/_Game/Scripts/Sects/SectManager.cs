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
    /// </summary>
    public class SectManager : MonoBehaviour
    {
        [Header("Event Channels")]
        [SerializeField] private GamePhaseEventChannelSO _onPhaseChanged;
        [SerializeField] private VoidEventChannelSO _onTurnEnded;
        [SerializeField] private ZodiacBonusesEventChannelSO _onZodiacBonuses;

        [Header("State")]
        [SerializeField] private SectData _sectData;

        public SectData Data => _sectData;
        public bool HasFoundedSect => _sectData.IsFounded;

        public event Action<SectData> OnSectFounded;
        public event Action<string> OnResourceChanged;

        private void OnEnable()
        {
            if (_onPhaseChanged != null)
                _onPhaseChanged.Subscribe(OnPhaseChanged);
            if (_onTurnEnded != null)
                _onTurnEnded.Subscribe(OnTurnEnded);
            if (_onZodiacBonuses != null)
                _onZodiacBonuses.Subscribe(OnZodiacBonuses);
        }

        private void OnDisable()
        {
            if (_onPhaseChanged != null)
                _onPhaseChanged.Unsubscribe(OnPhaseChanged);
            if (_onTurnEnded != null)
                _onTurnEnded.Unsubscribe(OnTurnEnded);
            if (_onZodiacBonuses != null)
                _onZodiacBonuses.Unsubscribe(OnZodiacBonuses);
        }

        /// <summary>
        /// Called by FoundSectCommand after successful execution.
        /// </summary>
        public void SetSectData(SectData data)
        {
            _sectData = data;
            OnSectFounded?.Invoke(_sectData);
        }

        private void OnPhaseChanged(GamePhase phase)
        {
            if (!_sectData.IsFounded) return;

            switch (phase)
            {
                case GamePhase.Income:
                    ProcessIncome();
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
