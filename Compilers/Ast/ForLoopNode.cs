using System.Collections.Generic;

namespace Minesweeper.Test
{

    public class WhileLoopNode : Node
    {
        public WhileLoopNode(Node boolExpression, Node doStatement)
        {
            BoolExpression = boolExpression;
            DoStatement = doStatement;
        }

        public Node BoolExpression { get; set; }
        public Node DoStatement { get; set; }

        public override IEnumerable<Node> Children => new[] {BoolExpression, DoStatement};

        public override string Display()
        {
            return $"While({BoolExpression}, do {DoStatement})";
        }
    }

    public class ForLoopNode : Node, IStatementNode
    {
        public ForLoopNode(AssignmentNode assignFromNode, Node toNode, Node doStatements)
        {
            AssignFromNode = assignFromNode;
            ToNode = toNode;
            DoStatements = doStatements;
        }
        //for < variable-name > := < initial_value > to [down to] < final_value > do S;

        public AssignmentNode AssignFromNode { get;  }
        public Node ToNode { get;  }

        public Node DoStatements { get;  }

        public override IEnumerable<Node> Children => new[] {AssignFromNode, ToNode, DoStatements};

        public override string Display()
        {
            return $"For({AssignFromNode} To {ToNode} Do({DoStatements}))";
        }
    }
}