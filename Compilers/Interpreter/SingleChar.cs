using System.Linq;

namespace Minesweeper.Test
{

    public class SingleChar : Component
    {
        private readonly char _matchValue;

        public SingleChar(char matchValue)
        {
            _matchValue = matchValue;
        }
        public override bool AddValueIfValid(char nextValue)
        {
            if (_matchValue == nextValue)
            {
                this.MatchingCharacters = nextValue.ToString();
                Value = nextValue;
                return true;
            }

            return false;
        }
    }
}