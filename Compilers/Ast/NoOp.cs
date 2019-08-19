using System.Collections.Generic;

namespace Minesweeper.Test
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