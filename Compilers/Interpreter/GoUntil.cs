namespace Compilers.Interpreter
{
    public class GoUntil : Component
    {
        private readonly Component _from;
        private readonly Component _to;

        public GoUntil(Component from, Component to)
        {
            _from = @from;
            _to = to;
        }
        public override bool AddValueIfValid(char nextValue)
        {
            return _from.AddValueIfValid(nextValue) || _to.AddValueIfValid(nextValue);
        }
    }
}