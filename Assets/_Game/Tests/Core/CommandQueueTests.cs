using NUnit.Framework;
using TalesOfTao.Core.Commands;

namespace TalesOfTao.Tests.Core
{
    public class CommandQueueTests
    {
        // ── Stub command ──────────────────────────────────────────────────────

        private class CounterCommand : Command
        {
            private readonly int _delta;
            private int[] _counter; // ref-via-array so lambda can capture

            public CounterCommand(int[] counter, int delta = 1)
            {
                _counter = counter;
                _delta = delta;
            }

            public override void Execute() => _counter[0] += _delta;
            public override void Undo()    => _counter[0] -= _delta;
        }

        private class BlockedCommand : Command
        {
            public override bool CanExecute() => false;
            public override void Execute() { }
            public override void Undo()    { }
        }

        // ── Execute ───────────────────────────────────────────────────────────

        [Test]
        public void Execute_ValidCommand_RunsAndPushesHistory()
        {
            var queue = new CommandQueue();
            int[] counter = { 0 };

            queue.Execute(new CounterCommand(counter));

            Assert.AreEqual(1, counter[0]);
            Assert.AreEqual(1, queue.HistoryCount);
        }

        [Test]
        public void Execute_BlockedCommand_ReturnsFalseAndSkips()
        {
            var queue = new CommandQueue();

            bool result = queue.Execute(new BlockedCommand());

            Assert.IsFalse(result);
            Assert.AreEqual(0, queue.HistoryCount);
        }

        // ── Undo ──────────────────────────────────────────────────────────────

        [Test]
        public void TryUndo_AfterExecute_ReversesEffect()
        {
            var queue = new CommandQueue();
            int[] counter = { 0 };
            queue.Execute(new CounterCommand(counter, delta: 5));

            queue.TryUndo();

            Assert.AreEqual(0, counter[0]);
            Assert.AreEqual(0, queue.HistoryCount);
        }

        [Test]
        public void TryUndo_EmptyHistory_ReturnsFalse()
        {
            var queue = new CommandQueue();
            Assert.IsFalse(queue.TryUndo());
        }

        [Test]
        public void TryUndo_UndoesInReverseOrder()
        {
            var queue = new CommandQueue();
            int[] counter = { 0 };
            queue.Execute(new CounterCommand(counter, delta: 3));
            queue.Execute(new CounterCommand(counter, delta: 7));
            // counter = 10

            queue.TryUndo(); // undo +7 → counter = 3
            Assert.AreEqual(3, counter[0]);

            queue.TryUndo(); // undo +3 → counter = 0
            Assert.AreEqual(0, counter[0]);
        }

        // ── Clear ─────────────────────────────────────────────────────────────

        [Test]
        public void ClearHistory_EmptiesStack()
        {
            var queue = new CommandQueue();
            int[] counter = { 0 };
            queue.Execute(new CounterCommand(counter));
            queue.Execute(new CounterCommand(counter));

            queue.ClearHistory();

            Assert.AreEqual(0, queue.HistoryCount);
        }
    }
}
