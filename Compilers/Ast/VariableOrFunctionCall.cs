﻿using System.Collections.Generic;
using Compilers.Interpreter;

namespace Compilers.Ast
{
    public class VariableOrFunctionCall : ExpressionNode
    {
        public string VariableName { get; set; }
        public TokenItem TokenItem { get; set; }
        public VariableOrFunctionCall(TokenItem token)
        {
            TokenItem = token;
            VariableName = token.Value;
        }

        public override IEnumerable<Node> Children { get; }

        public override string Display()
        {
            return $"Variable({VariableName})";
        }
    }
}