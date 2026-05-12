using System;
using System.Collections.Generic;

namespace TalesOfTao.Core.Commands
{
    // Command pattern with undo/redo support.
    //
    // Usage:
    //   var queue = new CommandQueue();
    //   queue.Execute(new MoveCommand(...));
    //   queue.TryUndo();  // undoes MoveCommand
    //   queue.TryRedo();  // re-executes MoveCommand
    //   queue.Execute(new AttackCommand(...));  // pushes redo stack
    public class CommandQueue
    {
        private readonly Stack<Command> _undoStack = new();
        private readonly Stack<Command> _redoStack = new();

        public int UndoCount => _undoStack.Count;
        public int RedoCount => _redoStack.Count;

        /// <summary>Executes the command and pushes it onto the undo stack. Clears the redo stack.</summary>
        public bool Execute(Command command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (!command.CanExecute()) return false;

            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear();  // new command invalidates redo history
            return true;
        }

        /// <summary>Undoes the most recent command and pushes it onto the redo stack.</summary>
        public bool TryUndo()
        {
            if (_undoStack.Count == 0) return false;
            var cmd = _undoStack.Pop();
            cmd.Undo();
            _redoStack.Push(cmd);
            return true;
        }

        /// <summary>Re-executes the most recently undone command and pushes it back onto the undo stack.</summary>
        public bool TryRedo()
        {
            if (_redoStack.Count == 0) return false;
            var cmd = _redoStack.Pop();
            cmd.Execute();
            _undoStack.Push(cmd);
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

        public void ClearHistory()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}
