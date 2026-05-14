using UnityEngine;
using TalesOfTao.Core;
using TalesOfTao.Core.Commands;

namespace TalesOfTao.Sects
{
    /// <summary>
    /// Command to recruit a new peon disciple. Costs 10 Tael.
    /// Peon appears after 1 turn delay (processed during Build phase).
    /// </summary>
    public class RecruitPeonCommand : Command
    {
        private readonly SectData _sect;
        private readonly SectConfigSO _sectConfig;
        private const int RECRUIT_COST = 10;

        public RecruitPeonCommand(SectData sect, SectConfigSO sectConfig)
        {
            _sect = sect;
            _sectConfig = sectConfig;
        }

        public override bool CanExecute()
        {
            if (_sect == null || _sectConfig == null) return false;

            // Check Tael cost
            if (_sect.Stockpile.Tael < RECRUIT_COST)
            {
                Debug.LogWarning("[RecruitPeonCommand] Not enough Tael.");
                return false;
            }

            // Check management ratio: peons must be <= 5x Outer Disciples
            int outerCount = _sect.GetDiscipleCount(DiscipleRank.OuterDisciple);
            int peonCount = _sect.GetDiscipleCount(DiscipleRank.Peon);
            if (peonCount + 1 > outerCount * 5)
            {
                Debug.LogWarning("[RecruitPeonCommand] Management ratio exceeded. Need more Outer Disciples.");
                return false;
            }

            return true;
        }

        public override void Execute()
        {
            if (!CanExecute()) return;

            // Deduct cost
            _sect.Stockpile.Tael -= RECRUIT_COST;

            // Create peon disciple
            var disciple = new DiscipleData
            {
                Name = DiscipleData.GenerateName(),
                Rank = DiscipleRank.Peon,
                IsAlive = true,
                Techniques = System.Array.Empty<string>(),
                Trait = RollTrait(),
                BondedBeast = ""
            };
            disciple.CalculateStats(_sectConfig);

            // Add to sect
            _sect.AddDisciple(disciple);

            Debug.Log($"[RecruitPeonCommand] Recruited peon: {disciple.Name} (Trait: {disciple.Trait})");
        }

        public override void Undo()
        {
            // Remove the last added peon
            var disciples = _sect.GetDisciples();
            if (disciples.Count > 0)
            {
                var lastDisciple = disciples[disciples.Count - 1];
                if (lastDisciple.Rank == DiscipleRank.Peon)
                {
                    _sect.RemoveDisciple(lastDisciple.Name);
                    _sect.Stockpile.Tael += RECRUIT_COST;
                    Debug.Log($"[RecruitPeonCommand] Undone: removed {lastDisciple.Name}, refunded {RECRUIT_COST} Tael");
                }
            }
        }

        private static string RollTrait()
        {
            string[] traits = { "Lucky", "Resilient", "Perceptive", "Reckless", "Fragile", "" };
            float roll = Random.value;
            if (roll < 0.3f) return traits[Random.Range(0, traits.Length - 1)]; // 30% chance of trait
            return ""; // 70% no trait
        }
    }
}
