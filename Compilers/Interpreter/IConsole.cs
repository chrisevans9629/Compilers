namespace Minesweeper.Test
{
    public interface IConsole
    {
        void Write(string str);
        char Read();
        void WriteLine(string str);
        string Output { get; }
    }
}