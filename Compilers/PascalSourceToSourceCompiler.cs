using System;
using System.Collections.Generic;
using System.Linq;
using Minesweeper.Test.Symbols;

namespace Minesweeper.Test.Tests
{
    public class PascalSourceToSourceCompiler : IPascalNodeVisitor<string>
    {
        private PascalLexer lexer;
        private PascalAst ast;
        private PascalSemanticAnalyzer table;
        public PascalSourceToSourceCompiler(ILogger logger)
        {
            lexer = new PascalLexer();
            ast = new PascalAst();
            table = new PascalSemanticAnalyzer(logger);
        }

        private ScopedSymbolTable CurrentScope;
        public string Convert(string pascalInput)
        {
            var tokens = lexer.Tokenize(pascalInput);
            var tree = ast.Evaluate(tokens);

            table.CheckSyntax(tree);

            return VisitNode(tree);
        }

        public string VisitNode(Node node)
        {
            if (node is PascalProgramNode program)
            {
                return VisitProgram(program);
            }

            if (node is BlockNode block)
            {
                return VisitBlock(block);
            }

            if (node is VarDeclarationNode declaration)
            {
                return VisitVarDeclaration(declaration);
            }
            if (node is ProcedureDeclarationNode procedureDeclaration)
            {
                return VisitProcedureDec(procedureDeclaration);
            }

            if (node is AssignmentNode assign)
            {
                return $"{AddSpaces()}{VisitVariable(assign.Left)} := {VisitNode(assign.Right)};\r\n";
            }

            if (node is VariableOrFunctionCall varaible)
            {
                return VisitVariable(varaible);
            }
            if (node is BinaryOperator op)
            {
                return $"{VisitNode(op.Left)} {op.TokenItem.Value} {VisitNode(op.Right)}";
            }
            if (node is NoOp no)
            {
                return "";
            }
            //return "";
            throw new NotImplementedException($"no implementation for node {node}");
        }

        private string VisitVariable(VariableOrFunctionCall variable)
        {
            return $"<{variable.VariableName}{CurrentScope.LookupSymbolScope(variable.VariableName)}:{CurrentScope.LookupSymbol(variable.VariableName, true).Type.Name}>";
        }

        private string VisitProcedureDec(ProcedureDeclarationNode procedureDeclaration)
        {
            var prev = CurrentScope;
            var namedec = $"{AddSpaces()}procedure {procedureDeclaration.ProcedureId}{CurrentScope.ScopeLevel}";
            CurrentScope = new ScopedSymbolTable(procedureDeclaration.ProcedureId,  prev);
            var dec =
                $"{namedec}({VisitProcedureDecParams(procedureDeclaration.Parameters)});\r\n" +
                VisitBlock(procedureDeclaration.Block);
            CurrentScope = prev;
            return dec;
        }

        private string VisitProcedureDecParams(IList<ParameterNode> parameters)
        {
            var str = "";
            foreach (var procedureParameter in parameters)
            {
                var symbol = new VariableSymbol(procedureParameter.Declaration.VarNode.VariableName,
                    CurrentScope.LookupSymbol(procedureParameter.Declaration.TypeNode.TypeValue, true));

                CurrentScope.Define(symbol);
                str +=
                    $"{procedureParameter.Declaration.VarNode.VariableName}{CurrentScope.ScopeLevel} : {procedureParameter.Declaration.TypeNode.TypeValue.ToUpper()};";
            }

            if (str.EndsWith(";"))
            {
                str = str.Remove(str.LastIndexOf(";", StringComparison.InvariantCulture), 1);
            }
            return str;
        }

        string AddSpaces(int add = 0)
        {
            var spaces = "";
            for (int i = 0; i < (CurrentScope.ScopeLevel * 3) + add; i++)
            {
                spaces += " ";
            }

            return spaces;
        }
        public string VisitVarDeclaration(VarDeclarationNode declaration)
        {
            var symbol = new VariableSymbol(declaration.VarNode.VariableName,
                CurrentScope.LookupSymbol(declaration.TypeNode.TypeValue, true));

            CurrentScope.Define(symbol);
            return $"{AddSpaces()}var {declaration.VarNode.VariableName}{CurrentScope.ScopeLevel} : {declaration.TypeNode.TypeValue.ToUpper()};\r\n";
        }

        public string VisitNoOp(NoOp noOp)
        {
            throw new NotImplementedException();
        }

        public string VisitBlock(BlockNode block)
        {
            var str = "";
            foreach (var blockDeclaration in block.Declarations)
            {
                str += VisitNode(blockDeclaration);
            }
            str += VisitCompoundStatement(block.CompoundStatement);
            return str;
        }

        public string VisitCompoundStatement(CompoundStatementNode compoundStatement)
        {
            var str = $"{AddSpaces(-3)}begin\r\n";
            foreach (var compoundStatementNode in compoundStatement.Nodes)
            {
                str += VisitNode(compoundStatementNode);
            }

            str += AddSpaces(-3) + "end; {END OF " + CurrentScope.ScopeName + "}\r\n";
            return str;
        }

        public string VisitAssignment(AssignmentNode assignment)
        {
            throw new NotImplementedException();
        }

        public string VisitVariableOrFunctionCall(VariableOrFunctionCall call)
        {
            throw new NotImplementedException();
        }


        public string VisitProgram(PascalProgramNode program)
        {
            var zero = new ScopedSymbolTable(program.ProgramName);
            PascalSemanticAnalyzer.DefineBuiltIns(zero);
            CurrentScope = zero;
            var programStr = VisitProgramBlockNode(program);
            return programStr;
        }

        public string VisitProcedureDeclaration(ProcedureDeclarationNode procedureDeclaration)
        {
            throw new NotImplementedException();
        }

        public string VisitProcedureCall(ProcedureCallNode procedureCall)
        {
            throw new NotImplementedException();
        }

        public string VisitFunctionCall(CallNode functionCall)
        {
            throw new NotImplementedException();
        }

        public string VisitEqualExpression(EqualExpression equalExpression)
        {
            throw new NotImplementedException();
        }

        public string VisitNegationOperator(NegationOperator negationOperator)
        {
            throw new NotImplementedException();
        }

        public string VisitIfStatement(IfStatementNode ifStatement)
        {
            throw new NotImplementedException();
        }

        public string VisitForLoop(ForLoopNode forLoop)
        {
            throw new NotImplementedException();
        }

        public string VisitConstantDeclaration(ConstantDeclarationNode constantDeclaration)
        {
            throw new NotImplementedException();
        }

        public string VisitPointer(PointerNode pointer)
        {
            throw new NotImplementedException();
        }

        public string VisitInOperator(InOperator inOperator)
        {
            throw new NotImplementedException();
        }

        public string VisitCaseStatement(CaseStatementNode caseStatement)
        {
            throw new NotImplementedException();
        }

        public string VisitWhileLoop(WhileLoopNode whileLoop)
        {
            throw new NotImplementedException();
        }

        public string VisitReal(RealNode real)
        {
            throw new NotImplementedException();
        }

        public string VisitInteger(IntegerNode integer)
        {
            throw new NotImplementedException();
        }

        public string VisitBinaryOperator(BinaryOperator binary)
        {
            throw new NotImplementedException();
        }

        public string VisitUnary(UnaryOperator unary)
        {
            throw new NotImplementedException();
        }

        public string Fail(Node node)
        {
            throw new NotImplementedException();
        }

        public string VisitString(StringNode str)
        {
            throw new NotImplementedException();
        }

        public string VisitFunctionDeclaration(FunctionDeclarationNode faDeclarationNode)
        {
            throw new NotImplementedException();
        }

        public string VisitBool(BoolNode boolNode)
        {
            throw new NotImplementedException();
        }

        public string VisitRangeExpression(ListRangeExpressionNode listRange)
        {
            throw new NotImplementedException();
        }

        public string VisitListItemsExpression(ListItemsExpressionNode itemsExpressionNode)
        {
            throw new NotImplementedException();
        }

        private string VisitProgramBlockNode(PascalProgramNode program)
        {
            var programStr = $"program {program.ProgramName}{CurrentScope.ScopeLevel};\r\n";
            var global = new ScopedSymbolTable("Global", CurrentScope);
            CurrentScope = global;

            foreach (var blockDeclaration in program.Block.Declarations)
            {
                programStr += VisitNode(blockDeclaration);
            }
            programStr += $"{AddSpaces(-3)}begin\r\n";
            foreach (var compoundStatementNode in program.Block.CompoundStatement.Nodes)
            {
                programStr += VisitNode(compoundStatementNode);
            }
            CurrentScope = CurrentScope.ParentScope;
            programStr += AddSpaces(-3) + "end. {END OF " + CurrentScope.ScopeName + "}\r\n";
            return programStr;
        }
    }
}