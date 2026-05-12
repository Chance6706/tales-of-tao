using System;
using System.Collections.Generic;

namespace TalesOfTao.Core.Commands
{
    public class CommandQueue
    {
        private readonly Stack<Command> _history = new();

        public int HistoryCount => _history.Count;

        public bool Execute(Command command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (!command.CanExecute()) return false;

            command.Execute();
            _history.Push(command);
            return true;
        }

        public bool TryUndo()
        {
            if (_history.Count == 0) return false;
            _history.Pop().Undo();
            return true;
        }

        // Returns any commands that were skipped (CanExecute failed).
        public IReadOnlyList<Command> Replay(IEnumerable<Command> commands)
        {
            var skipped = new List<Command>();
            foreach (var cmd in commands)
                if (!Execute(cmd)) skipped.Add(cmd);
            return skipped;
        }

        public void ClearHistory() => _history.Clear();
    }
}
