namespace Minesweeper.Test
{
    public class Iterator<T>
    {
        private T[] _str;
        public int index;

        public Iterator(T[] str)
        {
            index = 0;
            _str = str;
        }
        public T Peek()
        {
            if (index < _str.Length-1)
                return _str[index + 1];
            return default(T);
        }
        public T Current => (index <= _str.Length - 1) ? _str[index] : default(T);
        public virtual void Advance()
        {
            index++;
        }

        public T CurrentOrPrevious()
        {
            if (Current != null)
            {
                return Current;
            }
            else if(index > 0)
            {
                return _str[index - 1];
            }

            return default(T);
        }
    }
}