using System.Collections.Generic;
using Compilers.Ast;

namespace Compilers.Interpreter
{
    public class BinaryOperator : ExpressionNode
    {
        public BinaryOperator(Node left, Node right, TokenItem @operator)
        {
            Left = left;
            Right = right;
            TokenItem = @operator;
        }

        public string Name => TokenItem.Token.Name;
        public TokenItem TokenItem { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
        public override IEnumerable<Node> Children => new[] {Left, Right};

        public override string Display()
        {
            return $"{Left?.Display()} {TokenItem?.Value} {Right?.Display()}";
        }
    }
}