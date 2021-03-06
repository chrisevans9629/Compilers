﻿using System.Collections.Generic;
using Compilers.Interpreter;

namespace Compilers.Ast
{
    public class BoolNode : ExpressionNode
    {
        public TokenItem TokensCurrent { get; }
        public bool Value { get; set; }
        public TokenItem TokenItem { get; set; }
        public BoolNode(TokenItem tokensCurrent)
        {
            TokensCurrent = tokensCurrent;
            Value = bool.Parse(tokensCurrent.Value.ToLower());
        }


        public override IEnumerable<Node> Children { get; }

        public override string Display()
        {
            return Value.ToString();
        }
    }
}