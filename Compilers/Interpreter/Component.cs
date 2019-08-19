namespace Compilers.Interpreter
{
    public abstract class Component
    {
        public object Value { get; set; }
        public string MatchingCharacters { get; set; }
        public string PreviousCharacters { get; set; }
        public abstract bool AddValueIfValid(char nextValue);
    }
}