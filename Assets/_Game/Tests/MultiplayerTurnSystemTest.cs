using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TalesOfTao.Core;
using TalesOfTao.Core.TurnSystem;

namespace TalesOfTao.Tests
{
    [TestFixture]
    public class MultiplayerTurnSystemTest
    {
        private TurnCoordinator _coordinator;
        private TurnDriver _driver1;
        private TurnDriver _driver2;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            // Create coordinator
            var coordGO = new GameObject("TurnCoordinator");
            _coordinator = coordGO.AddComponent<TurnCoordinator>();

            // Create event channels (required by coordinator)
            var phaseChannel = ScriptableObject.CreateInstance<GamePhaseEventChannelSO>();
            var turnEndChannel = ScriptableObject.CreateInstance<VoidEventChannelSO>();
            var zodiacChannel = ScriptableObject.CreateInstance<ZodiacBonusesEventChannelSO>();

            // Use reflection to set serialized fields
            var phaseField = typeof(TurnCoordinator).GetField("_phaseChangedChannel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            phaseField?.SetValue(_coordinator, phaseChannel);
            var turnEndField = typeof(TurnCoordinator).GetField("_turnEndedChannel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            turnEndField?.SetValue(_coordinator, turnEndChannel);
            var zodiacField = typeof(TurnCoordinator).GetField("_zodiacBonusesChannel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            zodiacField?.SetValue(_coordinator, zodiacChannel);

            // Create local driver
            var driverGO = new GameObject("TurnDriver");
            _driver1 = driverGO.AddComponent<TurnDriver>();

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (_coordinator != null) Object.Destroy(_coordinator.gameObject);
            if (_driver1 != null) Object.Destroy(_driver1.gameObject);
            if (_driver2 != null) Object.Destroy(_driver2.gameObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_RegisterPlayers()
        {
            int p1 = _coordinator.RegisterPlayer("Player 1", false, Color.red);
            int p2 = _coordinator.RegisterPlayer("Player 2", false, Color.blue);
            int p3 = _coordinator.RegisterPlayer("AI 1", true, Color.green);

            Assert.AreEqual(3, _coordinator.PlayerCount, "Should have 3 players");

            var slot1 = _coordinator.GetPlayer(p1);
            Assert.IsNotNull(slot1, "Player 1 should exist");
            Assert.AreEqual("Player 1", slot1.PlayerName);
            Assert.IsFalse(slot1.IsAI, "Player 1 should be human");

            var slot3 = _coordinator.GetPlayer(p3);
            Assert.IsNotNull(slot3, "AI 1 should exist");
            Assert.IsTrue(slot3.IsAI, "AI 1 should be AI");
            Assert.IsTrue(slot3.IsReady, "AI should be auto-ready");

            Debug.Log("[MPTest] OK Registered 3 players (2 human, 1 AI)");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_ReadySystem_AllPlayersReady()
        {
            int p1 = _coordinator.RegisterPlayer("Player 1", false, Color.red);
            int p2 = _coordinator.RegisterPlayer("Player 2", false, Color.blue);
            int p3 = _coordinator.RegisterPlayer("AI 1", true, Color.green);

            bool allReadyFired = false;
            _coordinator.ReadySystem.OnAllPlayersReady += () => allReadyFired = true;

            // AI is already ready, need to ready the 2 humans
            _coordinator.OnPlayerReady(p1);
            Assert.IsFalse(allReadyFired, "Should not fire with 1 human ready");

            _coordinator.OnPlayerReady(p2);
            Assert.IsTrue(allReadyFired, "Should fire when all players ready");

            Debug.Log("[MPTest] OK All players ready fires correctly");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_ReadySystem_TimerForcesReady()
        {
            int p1 = _coordinator.RegisterPlayer("Player 1", false, Color.red);
            int p2 = _coordinator.RegisterPlayer("Player 2", false, Color.blue);

            bool allReadyFired = false;
            _coordinator.ReadySystem.OnAllPlayersReady += () => allReadyFired = true;

            // Start a short timer
            _coordinator.ReadySystem.StartTimer(0.5f);

            // Wait for timer to expire
            yield return new WaitForSeconds(1f);

            Assert.IsTrue(allReadyFired, "Timer should force all players ready");

            Debug.Log("[MPTest] OK Timer forces ready");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_TurnDriver_MultiplayerMode()
        {
            int p1 = _coordinator.RegisterPlayer("Player 1", false, Color.red);

            // Initialize driver in multiplayer mode
            _driver1.InitializeMultiplayer(p1);

            Assert.IsTrue(_driver1.IsMultiplayer, "Driver should be in multiplayer mode");
            Assert.AreEqual(p1, _driver1.LocalPlayerId, "Driver should have correct player ID");

            Debug.Log("[MPTest] OK TurnDriver multiplayer mode");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_TurnDriver_SignalReady()
        {
            int p1 = _coordinator.RegisterPlayer("Player 1", false, Color.red);
            int p2 = _coordinator.RegisterPlayer("AI 1", true, Color.green);

            _driver1.InitializeMultiplayer(p1);

            bool allReadyFired = false;
            _coordinator.ReadySystem.OnAllPlayersReady += () => allReadyFired = true;

            // Signal ready via driver
            _driver1.SignalReady();

            Assert.IsTrue(allReadyFired, "Driver SignalReady should trigger all ready (AI already ready)");

            Debug.Log("[MPTest] OK TurnDriver SignalReady");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_NetworkCommandQueue_Ordering()
        {
            var queue = new NetworkCommandQueue();

            // Create test commands
            var cmd1 = new TestCommand("P1Cmd1");
            var cmd2 = new TestCommand("P2Cmd1");
            var cmd3 = new TestCommand("P1Cmd2");

            // Enqueue out of order
            queue.Enqueue(2, cmd2);
            queue.Enqueue(1, cmd1);
            queue.Enqueue(1, cmd3);

            Assert.AreEqual(3, queue.Count, "Should have 3 commands");

            // Execute — should be ordered by player ID, then submission order
            queue.ExecuteAll();

            Assert.AreEqual("P1Cmd1", TestCommand.ExecutionLog[0], "First should be P1Cmd1");
            Assert.AreEqual("P1Cmd2", TestCommand.ExecutionLog[1], "Second should be P1Cmd2");
            Assert.AreEqual("P2Cmd1", TestCommand.ExecutionLog[2], "Third should be P2Cmd1");

            TestCommand.ExecutionLog.Clear();

            Debug.Log("[MPTest] OK NetworkCommandQueue ordering");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_GameStart_4Players()
        {
            // Register 4 players (3 human + 1 AI)
            int p1 = _coordinator.RegisterPlayer("Player 1", false, Color.red);
            int p2 = _coordinator.RegisterPlayer("Player 2", false, Color.blue);
            int p3 = _coordinator.RegisterPlayer("Player 3", false, Color.yellow);
            int p4 = _coordinator.RegisterPlayer("AI 1", true, Color.green);

            Assert.AreEqual(4, _coordinator.PlayerCount, "Should have 4 players");

            // Start the game
            _coordinator.StartGame();

            Assert.IsTrue(_coordinator.IsGameActive, "Game should be active");
            Assert.AreEqual(1, _coordinator.TurnNumber, "Should be turn 1");

            Debug.Log("[MPTest] OK Game started with 4 players");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_PlayerSlot_Data()
        {
            var slot = new PlayerSlot(1, "Test Player", false, Color.red);

            Assert.AreEqual(1, slot.PlayerId);
            Assert.AreEqual("Test Player", slot.PlayerName);
            Assert.IsFalse(slot.IsAI);
            Assert.IsFalse(slot.IsReady);
            Assert.IsTrue(slot.IsConnected);
            Assert.AreEqual(Color.red, slot.PlayerColor);

            string str = slot.ToString();
            Assert.IsTrue(str.Contains("Test Player"), "ToString should contain player name");
            Assert.IsTrue(str.Contains("Human"), "ToString should show Human");

            Debug.Log("[MPTest] OK PlayerSlot data");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Verify_UnregisterPlayer()
        {
            int p1 = _coordinator.RegisterPlayer("Player 1", false, Color.red);
            int p2 = _coordinator.RegisterPlayer("Player 2", false, Color.blue);

            Assert.AreEqual(2, _coordinator.PlayerCount);

            _coordinator.UnregisterPlayer(p1);

            Assert.AreEqual(1, _coordinator.PlayerCount, "Should have 1 player after unregister");
            Assert.IsNull(_coordinator.GetPlayer(p1), "Unregistered player should be null");
            Assert.IsNotNull(_coordinator.GetPlayer(p2), "Remaining player should exist");

            Debug.Log("[MPTest] OK Unregister player");
            yield return null;
        }

        // Helper test command
        private class TestCommand : Command
        {
            public static readonly System.Collections.Generic.List<string> ExecutionLog = new();
            private readonly string _name;

            public TestCommand(string name) { _name = name; }

            public override void Execute()
            {
                ExecutionLog.Add(_name);
            }

            public override void Undo()
            {
                ExecutionLog.Remove(_name);
            }
        }
    }
}
