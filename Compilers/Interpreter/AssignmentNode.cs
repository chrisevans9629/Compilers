using System.Collections.Generic;
using Compilers.Ast;

namespace Compilers.Interpreter
{
    public class AssignmentNode : Node, IStatementNode
    {
        public VariableOrFunctionCall Left { get; set; }
        public Node Right { get; set; }
        public TokenItem TokenItem { get; set; }
        public AssignmentNode(VariableOrFunctionCall left, TokenItem item, Node right)
        {
            Left = left;
            Right = right;
            TokenItem = item;
        }

        public override IEnumerable<Node> Children => new[] {Left, Right};

        public override string Display()
        {
            return $"Assign({Left?.Display()}, {Right?.Display()})";
        }
    }
}