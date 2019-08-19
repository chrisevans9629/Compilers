using System.Collections.Generic;
using System.Linq;
using Compilers.MathParser;

namespace Compilers.Interpreter
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