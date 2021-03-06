﻿using System.Collections.Generic;
using Compilers.Interpreter;

namespace Compilers.Ast
{
    public class PointerNode : ExpressionNode
    {
        public char Value { get; }
        public TokenItem TokenItem { get; }

        public PointerNode(TokenItem tokenItem)
        {
            Value = tokenItem.Value[0];
            TokenItem = tokenItem;
        }

        public override IEnumerable<Node> Children { get; }

        public override string Display()
        {
            return $"Pointer({Value})";
        }
    }
}