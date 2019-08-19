using System;
using System.Collections.Generic;
using System.Linq;
using Minesweeper.Test.Symbols;

namespace Minesweeper.Test
{
    public class PascalToCSharp : PascalNodeVisitor<string>
    {
        private ScopedSymbolTable CurrentScope;
        IList<string> _assembliesCalled = new List<string>();
        public override string VisitNode(Node node)
        {
            return this.VisitNodeModel(node);
        }

        public override string VisitProcedureDeclaration(ProcedureDeclarationNode procedureDeclaration)
        {
            var str = $"{AddSpaces()}public void {procedureDeclaration.ProcedureId}\r\n";
           // str += AddSpaces() + "{\r\n";

            CurrentScope = new ScopedSymbolTable(procedureDeclaration.ProcedureId, CurrentScope);

            str += VisitNode(procedureDeclaration.Block);

            CurrentScope = CurrentScope.ParentScope;

            //str += AddSpaces() + "}\r\n";
            return str;
        }

        public override string VisitProcedureCall(ProcedureCallNode procedureCall)
        {
            if (procedureCall.ProcedureName.ToUpper() == "WRITELN")
            {
                var assembly = "using System;";
                if (_assembliesCalled.Contains(assembly) != true)
                {
                    _assembliesCalled.Add(assembly);
                }

                var param = "";
                if (procedureCall.Parameters.Any())
                {
                    foreach (var procedureCallParameter in procedureCall.Parameters)
                    {
                        param += VisitNode(procedureCallParameter);
                        param += ",";
                    }

                    param = param.Remove(param.Length - 1);
                }
               
                
                return $"{AddSpaces(4)}Console.WriteLine({param});\r\n";
            }

            return "";
        }
        

        public override string VisitBinaryOperator(BinaryOperator binary)
        {
            return $"{VisitNode(binary.Left)} {binary.Name} {VisitNode(binary.Right)}";
        }
        

        public override string VisitString(StringNode str)
        {
            return "\"" + str.CurrentValue + "\"";
        }
        

        public override string VisitBool(BoolNode boolNode)
        {
            return boolNode.Value.ToString();
        }
        
        public override string VisitVarDeclaration(VarDeclarationNode varDeclaration)
        {
            var typeValue = varDeclaration.TypeNode.TypeValue.ToUpper();
            if (PascalTerms.Int == typeValue)
            {
                typeValue = "int";
            }

            if (PascalTerms.Real == typeValue)
            {
                typeValue = "double";
            }

            if (PascalTerms.Boolean == typeValue)
            {
                typeValue = "bool";
            }
            return $"{AddSpaces()}static {typeValue} {varDeclaration.VarNode.VariableName};\r\n";
        }

        public override string VisitProgram(PascalProgramNode program)
        {
            var zero = new ScopedSymbolTable(program.ProgramName);
            PascalSemanticAnalyzer.DefineBuiltIns(zero);
            CurrentScope = zero;
            name = "Main";
            type = "void";
            var block = VisitBlock(program.Block);
            var assems = "";

            foreach (var s in _assembliesCalled)
            {
                assems += s + "\r\n";
            }

            var str = $"{assems}public static class {program.ProgramName}\r\n{block}\r\n";
            return str.Trim();
        }
        string AddSpaces(int add = 0)
        {
            var spaces = "";
            for (int i = 0; i < (CurrentScope.ScopeLevel * 4) + add; i++)
            {
                spaces += " ";
            }

            return spaces;
        }

        public override string VisitNoOp(NoOp noOp)
        {
            return "";
        }

        public override string VisitBlock(BlockNode block)
        {
            var str = AddSpaces() + "{\r\n";
            CurrentScope = new ScopedSymbolTable(name,  CurrentScope);
            str += $"{VisitNodes(block.Declarations)}{VisitCompoundStatement(block.CompoundStatement)}";
            CurrentScope = CurrentScope.ParentScope;
            str += AddSpaces() + "}\r\n";
            return str;
        }

        private string type;
        private string name;
        public override string VisitCompoundStatement(CompoundStatementNode compoundStatement)
        {
            return $"{AddSpaces()}public static {type} {name}()\r\n" + AddSpaces() + "{\r\n" + VisitNodes(compoundStatement.Nodes) + AddSpaces() + "}\r\n";
        }

        public override string VisitAssignment(AssignmentNode assignment)
        {
            return $"{AddSpaces()}{VisitNode(assignment.Left)} = {VisitNode(assignment.Right)};\r\n";
        }

        public override string VisitVariableOrFunctionCall(VariableOrFunctionCall call)
        {
            return call.VariableName;
        }

        private string VisitNodes(IList<Node> blockDeclarations)
        {
            var str = "";
            foreach (var blockDeclaration in blockDeclarations)
            {
                str += VisitNode(blockDeclaration);
            }
            return str;
        }
    }
}