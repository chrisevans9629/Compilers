using System.Collections.Generic;

namespace Compilers.Ast
{
    public class VarDeclarationNode : Node
    {
        public VariableOrFunctionCall VarNode { get; set; }
        public TypeNode TypeNode { get; set; }

        public VarDeclarationNode(VariableOrFunctionCall varNode, TypeNode typeNode)
        {
            VarNode = varNode;
            TypeNode = typeNode;
        }

        public override IEnumerable<Node> Children => new Node[] {VarNode, TypeNode};

        public override string Display()
        {
            return $"VarDec({VarNode.Display()}, {TypeNode.Display()})";
        }
    }
}