using System.Collections.Generic;
using Compilers.Interpreter;

namespace Compilers.Ast
{

    public interface IToken
    {
        TokenItem TokenItem { get; set; }
    }
    public class ListItemsExpressionNode : Node, IToken
    {
        public TokenItem TokenItem { get; set; }

        public ListItemsExpressionNode(IList<StringNode> items, TokenItem tokenItem)
        {
            Items = items;
            TokenItem = tokenItem;
        }

        public IList<StringNode> Items { get; set; }
        public override IEnumerable<Node> Children => Items;

        public override string Display()
        {
            return $"List({Aggregate(Items)})";
        }
    }
    public class ListRangeExpressionNode : Node, IToken
    {
        public StringNode FromNode { get; }
        public StringNode ToNode { get; }

        public ListRangeExpressionNode(StringNode fromNode, StringNode toNode, TokenItem tokenItem)
        {
            FromNode = fromNode;
            ToNode = toNode;
            TokenItem = tokenItem;
        }

        public override IEnumerable<Node> Children => new[] {FromNode, ToNode};

        public override string Display()
        {
            return $"List({FromNode}..{ToNode})";
        }

        public TokenItem TokenItem { get; set; }
    }
}