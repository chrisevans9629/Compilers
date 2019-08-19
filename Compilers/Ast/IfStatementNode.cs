using System.Collections.Generic;

namespace Minesweeper.Test
{
    public interface INode
    {
         string Display();

    }
    public interface IStatementNode : INode
    {

    }
    public class IfStatementNode : Node, IStatementNode
    {
        public Node IfCheck { get; }
        public Node IfTrue { get; }
        public Node IfFalse { get; }

        public IfStatementNode(Node ifCheck, Node ifTrue, Node ifFalse)
        {
            IfCheck = ifCheck;
            this.IfTrue = ifTrue;
            this.IfFalse = ifFalse;
        }

        public override IEnumerable<Node> Children => new[] {IfCheck, IfTrue, IfFalse};

        public override string Display()
        {
            return $"If ({IfCheck}) then {IfTrue} else {IfFalse}";
        }
    }
}