using System.Collections.Generic;

namespace Compilers.Ast
{
    public class NoOp : Node
    {
        public override IEnumerable<Node> Children { get; }

        public override string Display()
        {
            return "NoOp";
        }
    }
}