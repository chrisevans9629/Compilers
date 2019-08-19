using System.Collections.Generic;
using Compilers.Interpreter;

namespace Compilers.Ast
{

    public class IntegerNode : ExpressionNode
    {
        public TokenItem TokenItem { get; }
        public int Value { get; set; }
        public IntegerNode(TokenItem tokenItem)
        {
            TokenItem = tokenItem;
            Value = int.Parse(tokenItem.Value);
        }

        public override IEnumerable<Node> Children { get; }

        public override string Display()
        {
            return $"Integer({Value})";
        }
    }

    public class RealNode : ExpressionNode
    {
        public RealNode(TokenItem token)
        {
            Value = double.Parse(token.Value);
            TokenItem = token;
        }

        public TokenItem TokenItem { get; set; }
        public double Value { get; set; }
        public override IEnumerable<Node> Children { get; }

        public override string Display()
        {
            return $"Real({Value})";
        }
    }
}