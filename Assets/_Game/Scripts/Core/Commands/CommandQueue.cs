using System;
using System.Collections.Generic;

namespace TalesOfTao.Core.Commands
{
using System.Collections.Generic;
using UnityEngine;

namespace TalesOfTao.Core.Commands
{
    // Executes commands and maintains a history stack for undo.
    // One CommandQueue per player turn; AI uses its own instance.
    public class CommandQueue
    {
        private readonly Stack<Command> _history = new();

        public int HistoryCount => _history.Count;

        public bool Execute(Command command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (!command.CanExecute()) return false;
        // Validates, executes, and records the command.
        // Returns false and skips execution if CanExecute() fails.
        public bool Execute(Command command)
        {
            if (!command.CanExecute())
            {
                Debug.LogWarning($"[CommandQueue] {command.GetType().Name} cannot execute — preconditions not met.");
                return false;
            }

            command.Execute();
            _history.Push(command);
            return true;
        }

        // Reverses the most recent command. Returns false if history is empty.
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
        // Replays all commands in chronological order (oldest first).
        // Used for deterministic save-game restore from a command log.
        public void Replay(IEnumerable<Command> commands)
        {
            foreach (var cmd in commands)
                Execute(cmd);
        }

        public void ClearHistory() => _history.Clear();
    }
}
