namespace TalesOfTao.Core.Commands
{
    public abstract class Command
    {
        public virtual bool CanExecute() => true;
        public abstract void Execute();
        public abstract void Undo();
    }
}
