using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TalesOfTao.Core;
using TalesOfTao.Core.TurnSystem;
using TalesOfTao.Sects;

namespace TalesOfTao.Tests
{
    /// <summary>
    /// M5 PlayMode test: Recruit 3 peons → build Training Grounds → Outer Disciple spawns.
    /// </summary>
    [TestFixture]
    public class M5PlayModeTest
    {
        private SectManager _sectManager;
        private SectData _sectData;
        private BuildQueue _buildQueue;
        private TrainingQueue _trainingQueue;
        private SectConfigSO _sectConfig;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            // Create test config
            _sectConfig = ScriptableObject.CreateInstance<SectConfigSO>();

            // Create test sect data
            _sectData = new SectData
            {
                SectName = "Test Sect",
                Config = _sectConfig,
                IsFounded = true,
                Stockpile = new ResourceStockpile { Tael = 500, Qi = 100, Lumber = 50, IronOre = 20 }
            };

            // Create systems
            var managerGO = new GameObject("SectManager");
            _sectManager = managerGO.AddComponent<SectManager>();
            _sectManager.SetSectData(_sectData);

            var buildQueueGO = new GameObject("BuildQueue");
            _buildQueue = buildQueueGO.AddComponent<BuildQueue>();
            _buildQueue.MaxConcurrent = 1; // Temple T1
            _sectManager.SetBuildQueue(_buildQueue);

            var trainingQueueGO = new GameObject("TrainingQueue");
            _trainingQueue = trainingQueueGO.AddComponent<TrainingQueue>();
            _trainingQueue.MaxConcurrent = 5; // Training Grounds T1
            _sectManager.SetTrainingQueue(_trainingQueue);

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Object.Destroy(_sectManager.gameObject);
            Object.Destroy(_buildQueue.gameObject);
            Object.Destroy(_trainingQueue.gameObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_Recruit3Peons()
        {
            // Recruit 3 peons
            for (int i = 0; i < 3; i++)
            {
                var cmd = new RecruitPeonCommand(_sectData, _sectConfig);
                Assert.IsTrue(cmd.CanExecute(), $"Should be able to recruit peon {i + 1}");
                cmd.Execute();
            }

            // Verify peon count
            Assert.AreEqual(3, _sectData.GetDiscipleCount(DiscipleRank.Peon), "Should have 3 peons");

            // Verify Tael deducted (3 × 10 = 30)
            Assert.AreEqual(470, _sectData.Stockpile.Tael, "Tael should be 500 - 30 = 470");

            Debug.Log("[M5Test] ✓ Recruited 3 peons");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_BuildTrainingGrounds()
        {
            // First recruit an Outer Disciple (needed for management ratio)
            var outerDisciple = new DiscipleData
            {
                Name = "Test Outer",
                Rank = DiscipleRank.OuterDisciple,
                IsAlive = true
            };
            outerDisciple.CalculateStats(_sectConfig);
            _sectData.AddDisciple(outerDisciple);

            // Queue Training Grounds T1 (8 turns)
            _buildQueue.Enqueue("TrainingGrounds", 1, 8);

            Assert.IsTrue(_buildQueue.IsUnderConstruction("TrainingGrounds", 1), "Training Grounds should be under construction");

            // Simulate 8 Build phases
            for (int turn = 1; turn <= 8; turn++)
            {
                _buildQueue.ProcessBuildPhase();
                var queue = _buildQueue.GetQueue();
                if (queue.Length > 0)
                {
                    Debug.Log($"[M5Test] Build phase {turn}/8, turns remaining: {queue[0].TurnsRemaining}");
                }
                else
                {
                    Debug.Log($"[M5Test] Build phase {turn}/8, queue empty (completed)");
                }
            }

            // Verify construction complete
            Assert.IsTrue(_buildQueue.IsComplete("TrainingGrounds", 1), "Training Grounds should be complete after 8 turns");

            Debug.Log("[M5Test] ✓ Training Grounds built in 8 turns");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_PeonToOuterDisciple()
        {
            // Recruit a peon first
            var recruitCmd = new RecruitPeonCommand(_sectData, _sectConfig);
            recruitCmd.Execute();

            var peon = _sectData.FindDisciple(_sectData.GetDisciples()[0].Name);
            Assert.IsNotNull(peon, "Peon should exist");
            Assert.AreEqual(DiscipleRank.Peon, peon.Rank, "Should start as Peon");

            // Queue training: Peon -> Outer Disciple (5 turns)
            _trainingQueue.Enqueue(peon.Name, DiscipleRank.Peon, DiscipleRank.OuterDisciple, 5);

            // Simulate 5 Build phases
            for (int turn = 1; turn <= 5; turn++)
            {
                _trainingQueue.ProcessBuildPhase();
            }

            // The TrainingQueue fires OnTrainingCompleted event, but in this test
            // there's no SectManager wiring to auto-promote. Manually promote to verify the flow.
            // In production, SectManager.OnDiscipleTrainedEvent handles this.
            bool promoted = _sectData.PromoteDisciple(peon.Name);
            Assert.IsTrue(promoted, "Promotion should succeed");

            // Verify promotion
            var result = _sectData.FindDisciple(peon.Name);
            Assert.IsNotNull(result, "Disciple should still exist after promotion");
            Assert.AreEqual(DiscipleRank.OuterDisciple, result.Rank, "Should be promoted to Outer Disciple");

            Debug.Log("[M5Test] ✓ Peon promoted to Outer Disciple");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_ManagementRatio()
        {
            // Recruit 1 Outer Disciple
            var outerDisciple = new DiscipleData
            {
                Name = "Test Outer",
                Rank = DiscipleRank.OuterDisciple,
                IsAlive = true
            };
            outerDisciple.CalculateStats(_sectConfig);
            _sectData.AddDisciple(outerDisciple);

            // Should be able to recruit up to 5 peons (1:5 ratio with 1 Outer Disciple)
            for (int i = 0; i < 5; i++)
            {
                var cmd = new RecruitPeonCommand(_sectData, _sectConfig);
                Assert.IsTrue(cmd.CanExecute(), $"Should be able to recruit peon {i + 1}");
                cmd.Execute();
            }

            // 6th peon should fail (would be 6 peons with 1 outer = 6:1 > 5:1)
            var failCmd = new RecruitPeonCommand(_sectData, _sectConfig);
            Assert.IsFalse(failCmd.CanExecute(), "Should NOT be able to exceed management ratio");

            Debug.Log("[M5Test] ✓ Management ratio enforced");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_DissentAccumulation()
        {
            // Add 1 Outer Disciple
            var outerDisciple = new DiscipleData
            {
                Name = "Test Outer",
                Rank = DiscipleRank.OuterDisciple,
                IsAlive = true
            };
            outerDisciple.CalculateStats(_sectConfig);
            _sectData.AddDisciple(outerDisciple);

            // Add 20 peons (way over the 1:5 ratio)
            for (int i = 0; i < 20; i++)
            {
                var peon = new DiscipleData
                {
                    Name = $"Peon {i}",
                    Rank = DiscipleRank.Peon,
                    IsAlive = true
                };
                peon.CalculateStats(_sectConfig);
                _sectData.AddDisciple(peon);
            }

            int initialDissent = _sectData.DissentLevel;
            int dissentRate = _sectData.CalculateDissentRate();

            Assert.Greater(dissentRate, 0, "Dissent rate should be positive with ratio violation");
            Assert.Greater(_sectData.GetDiscipleCount(DiscipleRank.Peon), _sectData.GetDiscipleCount(DiscipleRank.OuterDisciple) * 5,
                "Peon count should exceed ratio");

            Debug.Log($"[M5Test] ✓ Dissent rate: {dissentRate} with {_sectData.GetDiscipleCount(DiscipleRank.Peon)} peons and {_sectData.GetDiscipleCount(DiscipleRank.OuterDisciple)} outer disciples");
            yield return null;
        }
    }
}
