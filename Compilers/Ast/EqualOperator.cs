using System.Collections.Generic;

namespace Minesweeper.Test
{
    public abstract class EqualExpression : ExpressionNode
    {
        public override IEnumerable<Node> Children => new[] {Left, Right};  
        public Node Left { get; set; }
        public Node Right { get; set; }
        public TokenItem TokenItem { get; set; }
        public string Name { get; }

        public EqualExpression(Node left, Node right, TokenItem tokenItem, string name)
        {
            Left = left;
            Right = right;
            TokenItem = tokenItem;
            Name = name;
        }
        public override string Display()
        {
            return $"{Name}({Left?.Display()}, {Right?.Display()})";
        }
    }

    public class NegationOperator : Node
    {
        public Node Right { get; }
        public TokenItem TokenItem { get; }

        public NegationOperator( Node right, TokenItem tokenItem)
        {
            Right = right;
            TokenItem = tokenItem;
        }

        public override IEnumerable<Node> Children => new[] {Right};

        public override string Display()
        {
            return $"Negate({Right})";
        }
    }

    public class NotEqualOperator : EqualExpression
    {

        public NotEqualOperator(Node left, Node right, TokenItem tokenItem) : base(left, right, tokenItem, "NotEqual")
        {
        }
       
    }
    public class EqualOperator : EqualExpression
    {
       

        public EqualOperator(Node left, Node right, TokenItem tokenItem) :base(left, right, tokenItem, "Equal")
        {
            
        }

      
    }
}