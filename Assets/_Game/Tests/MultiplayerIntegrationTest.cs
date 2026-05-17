using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TalesOfTao.Core;
using TalesOfTao.Core.TurnSystem;
using TalesOfTao.Core.EventChannels;

namespace TalesOfTao.Tests
{
    /// <summary>
    /// Integration test: simulates a 4-player multiplayer game (3 human + 1 AI)
    /// through 2 full turn cycles. Validates the complete flow.
    /// </summary>
    [TestFixture]
    public class MultiplayerIntegrationTest
    {
        private TurnCoordinator _coordinator;
        private TurnDriver _driver1;
        private TurnDriver _driver2;
        private TurnDriver _driver3;
        private GamePhaseEventChannelSO _phaseChannel;
        private VoidEventChannelSO _turnEndChannel;
        private ZodiacBonusesEventChannelSO _zodiacChannel;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            // Create event channels
            _phaseChannel = ScriptableObject.CreateInstance<GamePhaseEventChannelSO>();
            _turnEndChannel = ScriptableObject.CreateInstance<VoidEventChannelSO>();
            _zodiacChannel = ScriptableObject.CreateInstance<ZodiacBonusesEventChannelSO>();

            // Create coordinator
            var coordGO = new GameObject("TurnCoordinator");
            _coordinator = coordGO.AddComponent<TurnCoordinator>();

            // Set event channels via reflection
            SetField("_phaseChangedChannel", _phaseChannel);
            SetField("_turnEndedChannel", _turnEndChannel);
            SetField("_zodiacBonusesChannel", _zodiacChannel);

            // Set short phase durations for testing
            SetField("_eventPhaseDuration", 0.1f);
            SetField("_resolutionPhaseDuration", 0.1f);
            SetField("_managementPhaseTimeout", 5f);
            SetField("_actionPhaseTimeout", 5f);

            // Create drivers
            var driverGO1 = new GameObject("TurnDriver1");
            _driver1 = driverGO1.AddComponent<TurnDriver>();
            var driverGO2 = new GameObject("TurnDriver2");
            _driver2 = driverGO2.AddComponent<TurnDriver>();
            var driverGO3 = new GameObject("TurnDriver3");
            _driver3 = driverGO3.AddComponent<TurnDriver>();

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (_coordinator != null) Object.Destroy(_coordinator.gameObject);
            if (_driver1 != null) Object.Destroy(_driver1.gameObject);
            if (_driver2 != null) Object.Destroy(_driver2.gameObject);
            if (_driver3 != null) Object.Destroy(_driver3.gameObject);
            if (_phaseChannel != null) Object.Destroy(_phaseChannel);
            if (_turnEndChannel != null) Object.Destroy(_turnEndChannel);
            if (_zodiacChannel != null) Object.Destroy(_zodiacChannel);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_FullGame_4Players_2Turns()
        {
            // === SETUP: Register 4 players ===
            int p1 = _coordinator.RegisterPlayer("Player 1", false, Color.red);
            int p2 = _coordinator.RegisterPlayer("Player 2", false, Color.blue);
            int p3 = _coordinator.RegisterPlayer("Player 3", false, Color.yellow);
            int p4 = _coordinator.RegisterPlayer("AI 1", true, Color.green);

            Assert.AreEqual(4, _coordinator.PlayerCount, "Should have 4 players");

            // Initialize drivers
            _driver1.InitializeMultiplayer(p1);
            _driver2.InitializeMultiplayer(p2);
            _driver3.InitializeMultiplayer(p3);

            // Track phase transitions
            GamePhase lastPhase = GamePhase.Event;
            int turnCount = 0;
            _coordinator.OnPhaseStarted += (phase) => lastPhase = phase;
            _coordinator.OnTurnStarted += (turn) => turnCount = turn;

            // === START GAME ===
            _coordinator.StartGame();
            Assert.IsTrue(_coordinator.IsGameActive, "Game should be active");
            Assert.AreEqual(1, turnCount, "Should be turn 1");

            // === TURN 1 ===
            Debug.Log("[MPIntegration] === TURN 1 ===");

            // EVENT phase (auto-advances)
            yield return new WaitForSeconds(0.3f);
            Assert.AreEqual(GamePhase.Income, lastPhase, "Should auto-advance to Income");

            // INCOME phase (auto-advances)
            yield return new WaitForSeconds(0.3f);
            Assert.AreEqual(GamePhase.Build, lastPhase, "Should auto-advance to Build");

            // BUILD phase (player-driven) — all players signal ready
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(GamePhase.Build, lastPhase, "Should be in Build phase");
            _driver1.SignalReady();
            _driver2.SignalReady();
            _driver3.SignalReady();
            // AI is auto-ready

            // Wait for phase to advance
            yield return new WaitForSeconds(0.3f);
            Assert.AreEqual(GamePhase.Research, lastPhase, "Should advance to Research after all ready");

            // RESEARCH phase (auto-advances)
            yield return new WaitForSeconds(0.3f);
            Assert.AreEqual(GamePhase.Action, lastPhase, "Should auto-advance to Action");

            // ACTION phase (player-driven)
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(GamePhase.Action, lastPhase, "Should be in Action phase");
            _driver1.SignalReady();
            _driver2.SignalReady();
            _driver3.SignalReady();

            // Wait for phase to advance
            yield return new WaitForSeconds(0.3f);
            Assert.AreEqual(GamePhase.Resolution, lastPhase, "Should advance to Resolution");

            // RESOLUTION phase (auto-advances)
            yield return new WaitForSeconds(0.3f);

            // === TURN 2 ===
            Debug.Log("[MPIntegration] === TURN 2 ===");
            Assert.AreEqual(2, turnCount, "Should be turn 2");

            // EVENT phase
            yield return new WaitForSeconds(0.3f);
            Assert.AreEqual(GamePhase.Income, lastPhase, "Turn 2: Should be in Income");

            // INCOME phase
            yield return new WaitForSeconds(0.3f);
            Assert.AreEqual(GamePhase.Build, lastPhase, "Turn 2: Should be in Build");

            // BUILD phase — all ready
            yield return new WaitForSeconds(0.1f);
            _driver1.SignalReady();
            _driver2.SignalReady();
            _driver3.SignalReady();
            yield return new WaitForSeconds(0.3f);
            Assert.AreEqual(GamePhase.Research, lastPhase, "Turn 2: Should advance to Research");

            // RESEARCH
            yield return new WaitForSeconds(0.3f);
            Assert.AreEqual(GamePhase.Action, lastPhase, "Turn 2: Should advance to Action");

            // ACTION — all ready
            yield return new WaitForSeconds(0.1f);
            _driver1.SignalReady();
            _driver2.SignalReady();
            _driver3.SignalReady();
            yield return new WaitForSeconds(0.3f);
            Assert.AreEqual(GamePhase.Resolution, lastPhase, "Turn 2: Should advance to Resolution");

            // RESOLUTION
            yield return new WaitForSeconds(0.3f);

            // === VERIFY ===
            Assert.AreEqual(3, turnCount, "Should be turn 3 now");
            Assert.IsTrue(_coordinator.IsGameActive, "Game should still be active");

            Debug.Log("[MPIntegration] OK Full 4-player, 2-turn simulation passed");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_CommandSubmission_DuringBuildPhase()
        {
            // Setup
            int p1 = _coordinator.RegisterPlayer("Player 1", false, Color.red);
            int p2 = _coordinator.RegisterPlayer("AI 1", true, Color.green);

            _driver1.InitializeMultiplayer(p1);

            _coordinator.StartGame();

            // Wait for Build phase
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(GamePhase.Build, _coordinator.CurrentPhase, "Should be in Build phase");

            // Submit a command during Build phase
            var testCmd = new TestIntegrationCommand();
            _driver1.SubmitCommand(testCmd);

            Assert.IsTrue(testCmd.WasSubmitted, "Command should be submitted to coordinator");

            // Ready up to advance
            _driver1.SignalReady();
            yield return new WaitForSeconds(0.3f);

            // Command should have been executed during phase transition
            Assert.IsTrue(testCmd.WasExecuted, "Command should be executed after phase advance");

            Debug.Log("[MPIntegration] OK Command submission during Build phase");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_AutoReady_AIPlayers()
        {
            // Register only AI players
            int ai1 = _coordinator.RegisterPlayer("AI 1", true, Color.red);
            int ai2 = _coordinator.RegisterPlayer("AI 2", true, Color.blue);

            Assert.AreEqual(2, _coordinator.PlayerCount);

            // Both AI should be ready immediately
            var slot1 = _coordinator.GetPlayer(ai1);
            var slot2 = _coordinator.GetPlayer(ai2);
            Assert.IsTrue(slot1.IsReady, "AI 1 should be auto-ready");
            Assert.IsTrue(slot2.IsReady, "AI 2 should be auto-ready");

            // Ready system should report all ready
            Assert.IsTrue(_coordinator.ReadySystem.AreAllPlayersReady, "All AI players should be ready");

            // Start game — should auto-advance through all phases since AI is always ready
            _coordinator.StartGame();
            yield return new WaitForSeconds(0.5f);

            // Should have advanced past Event and Income
            Assert.Greater((int)_coordinator.CurrentPhase, (int)GamePhase.Event,
                "Should auto-advance past Event phase with all-AI players");

            Debug.Log("[MPIntegration] OK AI auto-ready behavior");
            yield return null;
        }

        private void SetField(string fieldName, object value)
        {
            var field = typeof(TurnCoordinator).GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(_coordinator, value);
        }

        private class TestIntegrationCommand : Command
        {
            public bool WasSubmitted;
            public bool WasExecuted;

            public override bool CanExecute() => true;

            public override void Execute()
            {
                WasExecuted = true;
                Debug.Log("[MPIntegration] Test command executed");
            }

            public override void Undo() { }
        }
    }
}
