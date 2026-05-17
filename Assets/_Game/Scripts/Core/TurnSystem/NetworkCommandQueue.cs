using System;
using System.Collections.Generic;
using TalesOfTao.Core.Commands;

namespace TalesOfTao.Core.TurnSystem
{
    /// <summary>
    /// Collects commands from all players during a phase,
    /// then executes them deterministically at phase end.
    /// </summary>
    public class NetworkCommandQueue
    {
        private readonly List<QueuedCommand> _commands = new();

        public int Count => _commands.Count;

        public void Enqueue(int playerId, Command command)
        {
            _commands.Add(new QueuedCommand
            {
                PlayerId = playerId,
                Command = command,
                Timestamp = Time.realtimeSinceStartup
            });
        }

        public void ExecuteAll()
        {
            // Sort by player ID first, then by submission order (timestamp)
            _commands.Sort((a, b) =>
            {
                int cmp = a.PlayerId.CompareTo(b.PlayerId);
                if (cmp != 0) return cmp;
                return a.Timestamp.CompareTo(b.Timestamp);
            });

            foreach (var qc in _commands)
            {
                if (qc.Command.CanExecute())
                {
                    qc.Command.Execute();
                }
            }
        }

        public void Clear()
        {
            _commands.Clear();
        }

        private struct QueuedCommand
        {
            public int PlayerId;
            public Command Command;
            public float Timestamp;
        }
    }
}
