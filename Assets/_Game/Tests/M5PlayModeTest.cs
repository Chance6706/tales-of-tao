using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TalesOfTao.Core;
using TalesOfTao.Core.TurnSystem;
using TalesOfTao.Sects;
using TalesOfTao.Hex;

namespace TalesOfTao.Tests
{
    [TestFixture]
    public class M5PlayModeTest
    {
        private SectManager _sectManager;
        private SectData _sectData;
        private BuildQueue _buildQueue;
        private TrainingQueue _trainingQueue;
        private SectConfigSO _sectConfig;
        private HexGridManager _grid;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            _sectConfig = ScriptableObject.CreateInstance<SectConfigSO>();

            _sectData = new SectData
            {
                SectName = "Test Sect",
                Config = _sectConfig,
                IsFounded = true,
                Stockpile = new ResourceStockpile { Tael = 500, Qi = 100, Lumber = 50, IronOre = 20 },
                FoundingTileQ = 0,
                FoundingTileR = 0
            };

            var managerGO = new GameObject("SectManager");
            _sectManager = managerGO.AddComponent<SectManager>();
            _sectManager.SetSectData(_sectData);

            var buildQueueGO = new GameObject("BuildQueue");
            _buildQueue = buildQueueGO.AddComponent<BuildQueue>();
            _buildQueue.MaxConcurrent = 1;
            _sectManager.SetBuildQueue(_buildQueue);

            var trainingQueueGO = new GameObject("TrainingQueue");
            _trainingQueue = trainingQueueGO.AddComponent<TrainingQueue>();
            _trainingQueue.MaxConcurrent = 5;
            _sectManager.SetTrainingQueue(_trainingQueue);

            // Create a minimal grid for peon spawning
            var gridGO = new GameObject("HexGridManager");
            _grid = gridGO.AddComponent<HexGridManager>();

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Object.Destroy(_sectManager.gameObject);
            Object.Destroy(_buildQueue.gameObject);
            Object.Destroy(_trainingQueue.gameObject);
            if (_grid != null) Object.Destroy(_grid.gameObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_Recruit3Peons()
        {
            for (int i = 0; i < 3; i++)
            {
                var cmd = new RecruitPeonCommand(_sectData, _sectConfig, _grid, null);
                Assert.IsTrue(cmd.CanExecute(), "Should be able to recruit peon " + (i + 1));
                cmd.Execute();
            }

            Assert.AreEqual(3, _sectData.GetDiscipleCount(DiscipleRank.Peon), "Should have 3 peons");
            Assert.AreEqual(470, _sectData.Stockpile.Tael, "Tael should be 500 - 30 = 470");

            Debug.Log("[M5Test] OK Recruited 3 peons");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_BuildTrainingGrounds()
        {
            var outerDisciple = new DiscipleData
            {
                Name = "Test Outer",
                Rank = DiscipleRank.OuterDisciple,
                IsAlive = true
            };
            outerDisciple.CalculateStats(_sectConfig);
            _sectData.AddDisciple(outerDisciple);

            _buildQueue.Enqueue("TrainingGrounds", 1, 8);

            Assert.IsTrue(_buildQueue.IsUnderConstruction("TrainingGrounds", 1), "Training Grounds should be under construction");

            for (int turn = 1; turn <= 8; turn++)
            {
                _buildQueue.ProcessBuildPhase();
                var queue = _buildQueue.GetQueue();
                if (queue.Length > 0)
                {
                    Debug.Log("[M5Test] Build phase " + turn + "/8, turns remaining: " + queue[0].TurnsRemaining);
                }
                else
                {
                    Debug.Log("[M5Test] Build phase " + turn + "/8, queue empty (completed)");
                }
            }

            Assert.IsTrue(_buildQueue.IsComplete("TrainingGrounds", 1), "Training Grounds should be complete after 8 turns");

            Debug.Log("[M5Test] OK Training Grounds built in 8 turns");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_PeonToOuterDisciple()
        {
            var recruitCmd = new RecruitPeonCommand(_sectData, _sectConfig, _grid, null);
            recruitCmd.Execute();

            var peon = _sectData.FindDisciple(_sectData.GetDisciples()[0].Name);
            Assert.IsNotNull(peon, "Peon should exist");
            Assert.AreEqual(DiscipleRank.Peon, peon.Rank, "Should start as Peon");

            _trainingQueue.Enqueue(peon.Name, DiscipleRank.Peon, DiscipleRank.OuterDisciple, 5);

            for (int turn = 1; turn <= 5; turn++)
            {
                _trainingQueue.ProcessBuildPhase();
            }

            bool promoted = _sectData.PromoteDisciple(peon.Name);
            Assert.IsTrue(promoted, "Promotion should succeed");

            var result = _sectData.FindDisciple(peon.Name);
            Assert.IsNotNull(result, "Disciple should still exist after promotion");
            Assert.AreEqual(DiscipleRank.OuterDisciple, result.Rank, "Should be promoted to Outer Disciple");

            Debug.Log("[M5Test] OK Peon promoted to Outer Disciple");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_ManagementRatio()
        {
            var outerDisciple = new DiscipleData
            {
                Name = "Test Outer",
                Rank = DiscipleRank.OuterDisciple,
                IsAlive = true
            };
            outerDisciple.CalculateStats(_sectConfig);
            _sectData.AddDisciple(outerDisciple);

            for (int i = 0; i < 5; i++)
            {
                var cmd = new RecruitPeonCommand(_sectData, _sectConfig, _grid, null);
                Assert.IsTrue(cmd.CanExecute(), "Should be able to recruit peon " + (i + 1));
                cmd.Execute();
            }

            var failCmd = new RecruitPeonCommand(_sectData, _sectConfig, _grid, null);
            Assert.IsFalse(failCmd.CanExecute(), "Should NOT be able to exceed management ratio");

            Debug.Log("[M5Test] OK Management ratio enforced");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_DissentAccumulation()
        {
            var outerDisciple = new DiscipleData
            {
                Name = "Test Outer",
                Rank = DiscipleRank.OuterDisciple,
                IsAlive = true
            };
            outerDisciple.CalculateStats(_sectConfig);
            _sectData.AddDisciple(outerDisciple);

            for (int i = 0; i < 20; i++)
            {
                var peon = new DiscipleData
                {
                    Name = "Peon " + i,
                    Rank = DiscipleRank.Peon,
                    IsAlive = true
                };
                peon.CalculateStats(_sectConfig);
                _sectData.AddDisciple(peon);
            }

            int dissentRate = _sectData.CalculateDissentRate();

            Assert.Greater(dissentRate, 0, "Dissent rate should be positive with ratio violation");
            Assert.Greater(_sectData.GetDiscipleCount(DiscipleRank.Peon), _sectData.GetDiscipleCount(DiscipleRank.OuterDisciple) * 5,
                "Peon count should exceed ratio");

            Debug.Log("[M5Test] OK Dissent rate: " + dissentRate + " with " + _sectData.GetDiscipleCount(DiscipleRank.Peon) + " peons and " + _sectData.GetDiscipleCount(DiscipleRank.OuterDisciple) + " outer disciples");
            yield return null;
        }
    }
}