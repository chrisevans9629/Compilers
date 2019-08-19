using System;
using System.Collections.Generic;

namespace Minesweeper.Test
{
    
    public interface IAbstractSyntaxTree
    {
        double Evaluate(IList<TokenItem> tokens);
    }
}