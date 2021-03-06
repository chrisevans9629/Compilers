﻿using System.Collections.Generic;

namespace Compilers.Ast
{
    public class ParameterNode : Node
    {
        public VarDeclarationNode Declaration { get; }

        public ParameterNode(VarDeclarationNode declaration)
        {
            Declaration = declaration;
        }

        public override IEnumerable<Node> Children => new[] {Declaration};

        public override string Display()
        {
            return $"Param({Declaration.Display()})";
        }
    }
}