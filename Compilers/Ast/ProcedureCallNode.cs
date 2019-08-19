using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class ProcedureCallNode : CallNode, IStatementNode
    {
        public string ProcedureName => Name;

        public ProcedureCallNode(string name, IList<Node> parameters, TokenItem token) : base(name, parameters, token, "Procedure")
        {
        }
    }
}