using System;
using Compilers.Ast;

namespace Compilers.Interpreter
{
    public class SuperBasicMathLispInterpreter
    {
        string VisitNum(RealNode num)
        {
            return num.Value.ToString();
        }

        string VisitNode(Node node)
        {
            if (node is RealNode leaf)
            {
                return VisitNum(leaf);
            }

            if (node is IntegerNode integer)
            {
                return integer.Value.ToString();
            }
            if (node is BinaryOperator op) return VisitBin(op);

            throw new Exception($"did not recognize node {node}");
        }
        string VisitBin(BinaryOperator op)
        {
            if (op.Name == PascalTerms.Add) return "(+ " + (VisitNode(op.Left) + " " + VisitNode(op.Right) + ")");
            if (op.Name == PascalTerms.Sub) return "(- " + VisitNode(op.Left) + " " + VisitNode(op.Right) + ")";
            if (op.Name == PascalTerms.Multi) return "(* "+ VisitNode(op.Left) + " " + VisitNode(op.Right) + ")";
            if (op.Name == PascalTerms.FloatDiv) return "(/ " + VisitNode(op.Left) + " " + VisitNode(op.Right) + ")";

            throw new Exception($"did not recognize operation {op.Name}");

        }

        public string Evaluate(Node node)
        {
            return VisitNode(node).Trim();
        }
    }
}