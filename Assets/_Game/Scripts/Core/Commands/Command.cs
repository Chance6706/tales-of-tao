namespace TalesOfTao.Core.Commands
{
    // Base class for every player and AI action in the game.
    // All mutations of game state must go through a Command so that
    // undo, replay, and save serialization share a single path.
    public abstract class Command
    {
        // Returns false if preconditions are not met; callers should check
        // before enqueuing so invalid commands never enter the history stack.
        public virtual bool CanExecute() => true;

        public abstract void Execute();

        public abstract void Undo();
    }
}
