using System.Collections.Generic;
using Compilers.Interpreter;

namespace Compilers.Ast
{
    public class ProcedureCallNode : CallNode, IStatementNode
    {
        public string ProcedureName => Name;

        public ProcedureCallNode(string name, IList<Node> parameters, TokenItem token) : base(name, parameters, token, "Procedure")
        {
        }
    }
}