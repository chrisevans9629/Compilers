using System;
using System.Collections.Generic;
using System.Linq;
using Compilers.Ast;
using Compilers.Symbols;

namespace Compilers.Interpreter
{
    public class PascalToCSharp : PascalNodeVisitor<string>
    {
        //public ScopedSymbolTable CurrentScope { get; private set; }

        IList<string> _assembliesCalled = new List<string>();
        public override string VisitNode(Node node)
        {
            return this.VisitNodeModel(node);
        }

        private const string Void = "void";
        private ScopedSymbolTable current;
        public override string VisitProcedureDeclaration(ProcedureDeclarationNode procedureDeclaration)
        {
            var str = "";
            //var str = $"{AddSpaces()}public void {procedureDeclaration.ProcedureId}\r\n";
            // str += AddSpaces() + "{\r\n";
            var symbols = procedureDeclaration.Annotations["SymbolTable"] as ScopedSymbolTable;
            current = symbols;
            var param = "";
            for (var index = 0; index < procedureDeclaration.Parameters.Count; index++)
            {
                var p = procedureDeclaration.Parameters[index];
                var name = p.Declaration.VarNode.VariableName;
                var matches = symbols.LookupSymbols<Symbol>(name, true);

                if (matches.Count > 1)
                {
                    name = current.AddAlias(name);
                }

                param +=
                    $"{p.Declaration.TypeNode.TypeValue} {name}";
                if (index != procedureDeclaration.Parameters.Count - 1)
                {
                    param += ",";
                }
            }

            if (procedureDeclaration.Annotations.ContainsKey("Nested"))
            {
                str += $"{AddSpaces()}void {procedureDeclaration.ProcedureId}({param})\r\n";
            }
            else
            {
                str += $"{AddSpaces()}public static void {procedureDeclaration.ProcedureId}({param})\r\n";
            }
            //CurrentScope = new ScopedSymbolTable(procedureDeclaration.ProcedureId, CurrentScope);

            str += VisitBlock(procedureDeclaration.Block);

            if (procedureDeclaration.Annotations.ContainsKey("Nested"))
            {
                str = str.Remove(str.Length - 2);
                str += ";\r\n";
            }
            //CurrentScope = CurrentScope.ParentScope;
            current = symbols.ParentScope;
            //str += AddSpaces() + "}\r\n";
            return str;
        }

        public override string VisitProcedureCall(ProcedureCallNode procedureCall)
        {
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
            if (procedureCall.ProcedureName.ToUpper() == "WRITELN")
            {
                var assembly = "using System;";
                if (_assembliesCalled.Contains(assembly) != true)
                {
                    _assembliesCalled.Add(assembly);
                }

                return $"{AddSpaces()}Console.WriteLine({param});\r\n";
            }

            return $"{AddSpaces()}{procedureCall.ProcedureName}({param});\r\n";
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
            // var zero = new ScopedSymbolTable(program.ProgramName);
            // PascalSemanticAnalyzer.DefineBuiltIns(zero);
            //CurrentScope = zero;
            var block = "{\r\n";
            indentLevel++;
            block += VisitBlock(program.Block);
            indentLevel--;
            block += "}\r\n";
            var assems = "";

            foreach (var s in _assembliesCalled)
            {
                assems += s + "\r\n";
            }

            var str = $"{assems}public static class {program.ProgramName}\r\n{block}\r\n";
            return str.Trim();
        }

        private int indentLevel;
        string AddSpaces(int add = 0)
        {
            var spaces = "";
            for (int i = 0; i < (indentLevel * 4) + add; i++)
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
            var str = "";

            if (block.Annotations.ContainsKey("Main"))
            {
                str += VisitNodes(block.Declarations);
                str += $"{AddSpaces()}public static void Main()\r\n" + AddSpaces() + "{\r\n";
                indentLevel++;
                //CurrentScope = new ScopedSymbolTable(name, CurrentScope);

                str += VisitCompoundStatement(block.CompoundStatement);
                //CurrentScope = CurrentScope.ParentScope;
                indentLevel--;
                str += AddSpaces() + "}\r\n";
            }
            else
            {
                str += AddSpaces() + "{\r\n";
                // CurrentScope = new ScopedSymbolTable(name, CurrentScope);
                indentLevel++;
                str += VisitNodes(block.Declarations);
                str += VisitCompoundStatement(block.CompoundStatement);
                //CurrentScope = CurrentScope.ParentScope;
                indentLevel--;
                str += AddSpaces() + "}\r\n";
            }
            // var str = AddSpaces() + "{\r\n";
            // str += $"{VisitNodes(block.Declarations)}{VisitCompoundStatement(block.CompoundStatement)}";
            //str += AddSpaces() + "}\r\n";
            return str;
        }

        private string type;
        private string name;

        public override string VisitCompoundStatement(CompoundStatementNode compoundStatement)
        {
            return VisitNodes(compoundStatement.Nodes);
            // return $"{AddSpaces()}public static {type} {name}()\r\n" + AddSpaces() + "{\r\n" + VisitNodes(compoundStatement.Nodes) + AddSpaces() + "}\r\n";
        }

        public override string VisitAssignment(AssignmentNode assignment)
        {
            return $"{AddSpaces()}{VisitNode(assignment.Left)} = {VisitNode(assignment.Right)};\r\n";
        }

        public override string VisitVariableOrFunctionCall(VariableOrFunctionCall call)
        {
            return current.GetName(call.VariableName);
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