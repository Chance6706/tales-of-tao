using System;
using UnityEngine;
using TalesOfTao.Core.EventChannels;

namespace TalesOfTao.Core.TurnSystem
{
    /// <summary>
    /// Tracks the current turn and zodiac year. Broadcasts zodiac bonus changes
    /// via ZodiacBonusesEventChannelSO.
    /// </summary>
    public class ZodiacCalendar : MonoBehaviour
    {
        [SerializeField] private ZodiacBonusesEventChannelSO _zodiacBonusesChannel;

        private int _currentTurn;
        private int _currentYear; // 1-12 zodiac cycle

        public int CurrentTurn => _currentTurn;
        public int CurrentYear => _currentYear;
        public string CurrentAnimal => ZodiacBonuses.ForYear(_currentYear).Animal;
        public ZodiacBonuses CurrentBonuses { get; private set; }

        public event Action<int> OnTurnStarted;
        public event Action<int> OnYearChanged;

        private void Awake()
        {
            _currentTurn = 0;
            _currentYear = 1;
            CurrentBonuses = ZodiacBonuses.Neutral;
        }

        /// <summary>
        /// Advances to the next turn, updating the zodiac year and broadcasting bonuses.
        /// </summary>
        public void AdvanceTurn()
        {
            _currentTurn++;
            int previousYear = _currentYear;
            _currentYear = ((_currentTurn - 1) % 12) + 1;

            CurrentBonuses = ZodiacBonuses.ForYear(_currentYear);

            if (_currentYear != previousYear)
            {
                OnYearChanged?.Invoke(_currentYear);
            }

            _zodiacBonusesChannel?.Raise(CurrentBonuses);
            OnTurnStarted?.Invoke(_currentTurn);

            Debug.Log($"[ZodiacCalendar] Turn {_currentTurn}, Year {_currentYear} ({CurrentAnimal})");
        }

        /// <summary>
        /// Initializes the calendar at turn 1 with proper zodiac bonuses.
        /// </summary>
        public void Initialize()
        {
            _currentTurn = 1;
            _currentYear = 1;
            CurrentBonuses = ZodiacBonuses.ForYear(_currentYear);
            _zodiacBonusesChannel?.Raise(CurrentBonuses);
            OnTurnStarted?.Invoke(_currentTurn);
            Debug.Log($"[ZodiacCalendar] Initialized at Turn 1, Year 1 ({CurrentAnimal})");
        }
    }
}
