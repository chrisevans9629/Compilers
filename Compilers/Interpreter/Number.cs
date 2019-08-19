namespace Minesweeper.Test
{
    public class Number : Component
    {
        public override bool AddValueIfValid(char nextValue)
        {
            if (double.TryParse(MatchingCharacters + nextValue, out var t))
            {
                Value = t;
                MatchingCharacters += nextValue;
                return true;
            }

            return false;
        }
    }
}