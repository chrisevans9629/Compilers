using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class StringNode : ExpressionNode
    {
        public string CurrentValue { get; }
        public TokenItem TokenItem { get; }

        public StringNode(TokenItem tokenItem)
        {
            CurrentValue = tokenItem.Value;
            TokenItem = tokenItem;
        }

        public override IEnumerable<Node> Children { get; }

        public override string Display()
        {
            return $"String({CurrentValue})";
        }
    }
}