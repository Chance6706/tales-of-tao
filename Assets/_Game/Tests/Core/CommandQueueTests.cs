using NUnit.Framework;
using TalesOfTao.Core.Commands;

namespace TalesOfTao.Tests.Core
{
    [TestFixture]
    public class CommandQueueTests
    {
        private class TestCommand : Command
        {
            public bool Executed { get; private set; }
            public bool Undone { get; private set; }
            public bool CanExec { get; set; } = true;

            public override bool CanExecute() => CanExec;
            public override void Execute() => Executed = true;
            public override void Undo() => Undone = true;
        }

        [Test]
        public void Execute_PushesCommandOntoUndoStack()
        {
            var queue = new CommandQueue();
            var cmd = new TestCommand();

            bool result = queue.Execute(cmd);

            Assert.IsTrue(result);
            Assert.IsTrue(cmd.Executed);
            Assert.AreEqual(1, queue.UndoCount);
        }

        [Test]
        public void Execute_ReturnsFalse_WhenCanExecuteIsFalse()
        {
            var queue = new CommandQueue();
            var cmd = new TestCommand { CanExec = false };

            bool result = queue.Execute(cmd);

            Assert.IsFalse(result);
            Assert.IsFalse(cmd.Executed);
            Assert.AreEqual(0, queue.UndoCount);
        }

        [Test]
        public void TryUndo_ReturnsFalse_WhenStackEmpty()
        {
            var queue = new CommandQueue();
            Assert.IsFalse(queue.TryUndo());
        }

        [Test]
        public void TryUndo_CallsUndoAndPushesToRedoStack()
        {
            var queue = new CommandQueue();
            var cmd = new TestCommand();
            queue.Execute(cmd);

            bool result = queue.TryUndo();

            Assert.IsTrue(result);
            Assert.IsTrue(cmd.Undone);
            Assert.AreEqual(0, queue.UndoCount);
            Assert.AreEqual(1, queue.RedoCount);
        }

        [Test]
        public void TryRedo_ReturnsFalse_WhenRedoStackEmpty()
        {
            var queue = new CommandQueue();
            Assert.IsFalse(queue.TryRedo());
        }

        [Test]
        public void TryRedo_ReExecutesAndPushesToUndoStack()
        {
            var queue = new CommandQueue();
            var cmd = new TestCommand();
            queue.Execute(cmd);
            queue.TryUndo();

            bool result = queue.TryRedo();

            Assert.IsTrue(result);
            Assert.AreEqual(1, queue.UndoCount);
            Assert.AreEqual(0, queue.RedoCount);
        }

        [Test]
        public void Execute_ClearsRedoStack()
        {
            var queue = new CommandQueue();
            var cmd1 = new TestCommand();
            var cmd2 = new TestCommand();
            queue.Execute(cmd1);
            queue.TryUndo();
            Assert.AreEqual(1, queue.RedoCount);

            queue.Execute(cmd2);

            Assert.AreEqual(0, queue.RedoCount);
            Assert.AreEqual(1, queue.UndoCount);
        }

        [Test]
        public void ClearHistory_ClearsBothStacks()
        {
            var queue = new CommandQueue();
            queue.Execute(new TestCommand());
            queue.Execute(new TestCommand());
            queue.TryUndo();

            queue.ClearHistory();

            Assert.AreEqual(0, queue.UndoCount);
            Assert.AreEqual(0, queue.RedoCount);
        }

        [Test]
        public void Execute_ThrowsOnNullCommand()
        {
            var queue = new CommandQueue();
            Assert.Throws<System.ArgumentNullException>(() => queue.Execute(null));
        }
    }
}
