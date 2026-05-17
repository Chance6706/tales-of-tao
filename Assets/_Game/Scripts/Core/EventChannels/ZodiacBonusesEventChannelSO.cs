using UnityEngine;

namespace TalesOfTao.Core.EventChannels
{
    /// <summary>
    /// Event channel for broadcasting zodiac bonus changes.
    /// </summary>
    [CreateAssetMenu(menuName = "TalesOfTao/Events/Zodiac Bonuses Event Channel", fileName = "ZodiacBonusesEventChannel")]
    public class ZodiacBonusesEventChannelSO : EventChannelSO<TurnSystem.ZodiacBonuses> { }
}
