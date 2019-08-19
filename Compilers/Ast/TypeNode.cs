using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class TypeNode : Node
    {
        public string TypeValue { get; set; }
        public TokenItem TokenItem { get; set; }

        public TypeNode(TokenItem token)
        {
            TokenItem = token;
            TypeValue = token.Value;
        }

        public override IEnumerable<Node> Children { get; }

        public override string Display()
        {
            return $"Type({TypeValue})";
        }
    }
}