using UnityEngine;
using TalesOfTao.Core;
using TalesOfTao.Core.Commands;
using TalesOfTao.Hex;

namespace TalesOfTao.Sects
{
    public class RecruitPeonCommand : Command
    {
        private readonly SectData _sect;
        private readonly SectConfigSO _sectConfig;
        private readonly HexGridManager _grid;
        private readonly GameObject _peonPrefab;
        private GameObject _spawnedPeon;
        private const int RECRUIT_COST = 10;

        public RecruitPeonCommand(SectData sect, SectConfigSO sectConfig, HexGridManager grid, GameObject peonPrefab)
        {
            _sect = sect;
            _sectConfig = sectConfig;
            _grid = grid;
            _peonPrefab = peonPrefab;
        }

        public override bool CanExecute()
        {
            if (_sect == null || _sectConfig == null) return false;
            if (_sect.Stockpile.Tael < RECRUIT_COST) return false;
            int outerCount = _sect.GetDiscipleCount(DiscipleRank.OuterDisciple);
            int peonCount = _sect.GetDiscipleCount(DiscipleRank.Peon);
            if (outerCount > 0 && peonCount + 1 > outerCount * 5) return false;
            return true;
        }

        public override void Execute()
        {
            if (!CanExecute()) return;

            _sect.Stockpile.Tael -= RECRUIT_COST;

            var disciple = new DiscipleData
            {
                Name = DiscipleData.GenerateName(),
                Rank = DiscipleRank.Peon,
                IsAlive = true,
                Techniques = new string[0],
                Trait = RollTrait(),
                BondedBeast = ""
            };
            disciple.CalculateStats(_sectConfig);
            _sect.AddDisciple(disciple);

            if (_grid != null && _peonPrefab != null)
            {
                var tile = _grid.GetTile(_sect.FoundingTileQ, _sect.FoundingTileR);
                if (tile != null)
                {
                    var worldPos = tile.Coords.ToWorldPosition(1.0f);
                    float offsetX = _sect.GetDiscipleCount(DiscipleRank.Peon) * 0.8f;
                    worldPos += new Vector3(offsetX, 0, 0);
                    _spawnedPeon = UnityEngine.Object.Instantiate(_peonPrefab, worldPos, Quaternion.identity);
                    _spawnedPeon.name = disciple.Name;
                }
            }

            Debug.Log("[RecruitPeonCommand] Recruited peon: " + disciple.Name + " (Trait: " + disciple.Trait + ")");
        }

        public override void Undo()
        {
            if (_spawnedPeon != null) UnityEngine.Object.Destroy(_spawnedPeon);
            var disciples = _sect.GetDisciples();
            if (disciples.Count > 0)
            {
                var last = disciples[disciples.Count - 1];
                if (last.Rank == DiscipleRank.Peon)
                {
                    _sect.RemoveDisciple(last.Name);
                    _sect.Stockpile.Tael += RECRUIT_COST;
                }
            }
        }

        private static string RollTrait()
        {
            string[] traits = { "Lucky", "Resilient", "Perceptive", "Reckless", "Fragile", "" };
            if (Random.value < 0.3f) return traits[Random.Range(0, traits.Length - 1)];
            return "";
        }
    }
}