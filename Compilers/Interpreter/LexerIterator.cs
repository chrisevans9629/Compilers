namespace Minesweeper.Test
{
    public class LexerIterator : Iterator<char>
    {
        public int Line { get; set; }
        public int Column { get; set; }

        public override void Advance()
        {
            if (Current == '\n')
            {
                Line++;
                Column = 0;
            }
            Column++;
            base.Advance();
        }

        public LexerIterator(char[] str) : base(str)
        {
            Line = 1;
            Column = 1;
        }
    }
}