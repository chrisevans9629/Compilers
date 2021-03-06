﻿using System;
using System.Collections.Generic;
using System.Linq;
using Compilers.Ast;
using Compilers.Symbols;

namespace Compilers.Interpreter
{
    public class PascalToCSharp : PascalNodeVisitor<string>
    {
        //public ScopedSymbolTable CurrentScope { get; private set; }
        CSharpTypesConverter typesConverter = new CSharpTypesConverter();
        IList<string> _assembliesCalled = new List<string>();
        public override string VisitNode(Node node)
        {
            return this.VisitNodeModel(node);
        }

        private const string Void = "void";
        private ScopedSymbolTable current;
        public override string VisitInteger(IntegerNode integer)
        {
            return integer.Value.ToString();
        }



        public override string VisitEqualExpression(EqualExpression equalExpression)
        {
            return $"{VisitNode(equalExpression.Left)} == {VisitNode(equalExpression.Right)}";
        }

        public override string VisitIfStatement(IfStatementNode ifStatement)
        {
            var str = "";
            str += $"{AddSpaces()}if({VisitNode(ifStatement.IfCheck)})\r\n";
            str += $"{VisitNode(ifStatement.IfTrue)};\r\n";
            if (ifStatement.IfFalse != null)
            {
                str += $"{AddSpaces()}else\r\n";
                str += $"{VisitNode(ifStatement.IfFalse)};\r\n";
            }

            return str;
        }

        public override string VisitFunctionDeclaration(FunctionDeclarationNode faDeclarationNode)
        {
            var str = "";
            var procedureDeclaration = faDeclarationNode;
            //var str = $"{AddSpaces()}public void {procedureDeclaration.ProcedureId}\r\n";
            // str += AddSpaces() + "{\r\n";
            var symbols = procedureDeclaration.Annotations["SymbolTable"] as ScopedSymbolTable;
            current = symbols;
            var param = CreateParameters(procedureDeclaration, symbols);

            var type = typesConverter.GetTypeName(procedureDeclaration.ReturnType.TypeValue);
            if (procedureDeclaration.Annotations.ContainsKey("Nested"))
            {
                str += $"{AddSpaces()}{type} {procedureDeclaration.FunctionName}({param})\r\n";
            }
            else
            {
                str += $"{AddSpaces()}public static {type} {procedureDeclaration.FunctionName}({param})\r\n";
            }
            //CurrentScope = new ScopedSymbolTable(procedureDeclaration.ProcedureId, CurrentScope);

            str += VisitBlock(procedureDeclaration.Block);

            if (procedureDeclaration.Annotations.ContainsKey("Nested"))
            {
                //str = str.Remove(str.Length - 2);
                //str += ";\r\n";
            }
            //CurrentScope = CurrentScope.ParentScope;
            current = symbols.ParentScope;
            //str += AddSpaces() + "}\r\n";
            return str;
        }

        private string CreateParameters(DeclarationNode procedureDeclaration, ScopedSymbolTable symbols)
        {
            string param = "";
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
                    $"{typesConverter.GetTypeName(p.Declaration.TypeNode.TypeValue)} {name}";
                if (index != procedureDeclaration.Parameters.Count - 1)
                {
                    param += ",";
                }
            }

            return param;
        }

        public override string VisitProcedureDeclaration(ProcedureDeclarationNode procedureDeclaration)
        {
            var str = "";
            //var str = $"{AddSpaces()}public void {procedureDeclaration.ProcedureId}\r\n";
            // str += AddSpaces() + "{\r\n";
            var symbols = procedureDeclaration.Annotations["SymbolTable"] as ScopedSymbolTable;
            current = symbols;
            var param = CreateParameters(procedureDeclaration, symbols);

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
               // str = str.Remove(str.Length - 2);
               // str += ";\r\n";
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

                return $"{AddSpaces()}Console.WriteLine({param})";
            }

            if (procedureCall.ProcedureName.ToUpper() == "READLN")
            {
                var assembly = "using System;";
                if (_assembliesCalled.Contains(assembly) != true)
                {
                    _assembliesCalled.Add(assembly);
                }

                if (procedureCall.Parameters.Count > 1)
                {
                    
                    throw new SemanticException(ErrorCode.Runtime,procedureCall.Token, "cannot have more than one parameter");
                }
                if (procedureCall.Parameters.Any())
                {
                    return $"{AddSpaces()}{param} = Console.ReadLine()";
                }
                return $"{AddSpaces()}Console.ReadLine()";
            }

            return $"{AddSpaces()}{procedureCall.ProcedureName}({param})";
        }

        public override string VisitFunctionCall(CallNode functionCall)
        {
            var procedureCall = functionCall;
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
            //if (procedureCall.Name.ToUpper() == "WRITELN")
            //{
            //    var assembly = "using System;";
            //    if (_assembliesCalled.Contains(assembly) != true)
            //    {
            //        _assembliesCalled.Add(assembly);
            //    }

            //    return $"{AddSpaces()}Console.WriteLine({param});\r\n";
            //}

            return $"{AddSpaces()}{procedureCall.Name}({param})";
        }
        public override string VisitBinaryOperator(BinaryOperator binary)
        {
            return $"{VisitNode(binary.Left)} {binary.Name} {VisitNode(binary.Right)}";
        }

        public override string VisitUnary(UnaryOperator unary)
        {
            return unary.Name + VisitNode(unary.Value);
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


            typeValue = this.typesConverter.GetTypeName(typeValue);
            if (varDeclaration.Annotations.ContainsKey("Global"))
            {
                return $"{AddSpaces()}static {typeValue} {varDeclaration.VarNode.VariableName};";
            }
            return $"{AddSpaces()}{typeValue} {varDeclaration.VarNode.VariableName}";

        }

        public override string VisitProgram(PascalProgramNode program)
        {
            // var zero = new ScopedSymbolTable(program.ProgramName);
            // PascalSemanticAnalyzer.DefineBuiltIns(zero);
            //CurrentScope = zero;
            var block = "{\r\n";
            indentLevel++;
            current = program.Annotations["SymbolTable"] as ScopedSymbolTable;
            block += VisitBlock(program.Block);
            current = current.ParentScope;
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
                str += VisitNodes(block.Declarations, false);
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
                str += VisitNodes(block.Declarations, true);
                str += VisitCompoundStatement(block.CompoundStatement);
                //CurrentScope = CurrentScope.ParentScope;
                indentLevel--;
                str += AddSpaces() + "}";
            }
            // var str = AddSpaces() + "{\r\n";
            // str += $"{VisitNodes(block.Declarations)}{VisitCompoundStatement(block.CompoundStatement)}";
            //str += AddSpaces() + "}\r\n";
            return str;
        }


        public override string VisitCompoundStatement(CompoundStatementNode compoundStatement)
        {
            return VisitNodes(compoundStatement.Nodes, true);
            // return $"{AddSpaces()}public static {type} {name}()\r\n" + AddSpaces() + "{\r\n" + VisitNodes(compoundStatement.Nodes) + AddSpaces() + "}\r\n";
        }

        public override string VisitAssignment(AssignmentNode assignment)
        {
            if (assignment.Annotations.ContainsKey("Return"))
            {
                return $"{AddSpaces()}return {VisitNode(assignment.Right)}";
            }
            return $"{AddSpaces()}{VisitVariableOrFunctionCall(assignment.Left)} = {VisitNode(assignment.Right)}";
        }

        public override string VisitVariableOrFunctionCall(VariableOrFunctionCall call)
        {
            //if (call.VariableName.ToUpper() == "WRITELN")
            //{
            //    return "Console.WriteLine()";
            //}

            //if (call.VariableName.ToUpper() == "READLN")
            //{
            //    return "Console.ReadLine()";
            //}
            return current.GetName(call.VariableName);
        }

        private string VisitNodes(IList<Node> blockDeclarations, bool addSemiColon)
        {
            var str = "";
            var semi = addSemiColon ? ";" : "";
            foreach (var blockDeclaration in blockDeclarations)
            {
                if (blockDeclaration is NoOp != true)
                    str += VisitNode(blockDeclaration) + $"{semi}\r\n";
            }
            return str;
        }
    }
}