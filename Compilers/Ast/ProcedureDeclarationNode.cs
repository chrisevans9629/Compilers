using System.Collections.Generic;

namespace Compilers.Ast
{
    public class ProcedureDeclarationNode : DeclarationNode
    {
        public string ProcedureId => Name;

        public ProcedureDeclarationNode(string procedureId, BlockNode block, IList<ParameterNode> parameters)
        {
            Name = procedureId;
            Block = block;
            Parameters = parameters;
            MethodType = "Procedure";
        }


    }
}