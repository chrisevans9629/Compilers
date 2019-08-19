using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public class NumberValue : IMathValue
    {
        public string StringValue { get; set; }
        public double? Value { get; set; }
        public IEnumerable<IMathNode> GetMathNodes()
        {
            return Enumerable.Empty<IMathNode>();
        }
    }
}