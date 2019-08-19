using System.Collections.Generic;
using Compilers.Interpreter;

namespace Compilers.Ast
{
    
    public interface IAbstractSyntaxTree
    {
        double Evaluate(IList<TokenItem> tokens);
    }
}