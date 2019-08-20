using System.Collections.Generic;
using Compilers.Ast;

namespace Minesweeper.Test.Tests
{
    public class LoggerMock : Logger
    {
        public List<string> Calls { get; set; } = new List<string>();
        public override void Log(object obj)
        {
            Calls.Add(obj?.ToString());
            base.Log(obj);
        }
    }
}