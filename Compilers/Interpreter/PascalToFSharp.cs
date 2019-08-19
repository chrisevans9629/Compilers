using System;
using Compilers.Ast;

namespace Compilers.Interpreter
{
    public class PascalToFSharp : IPascalNodeVisitor<string>
    {
        public string VisitNode(Node node)
        {
            return this.VisitNodeModel(node);
        }

        public string VisitCompoundStatement(CompoundStatementNode statement)
        {
            throw new NotImplementedException();
        }

        public string VisitAssignment(AssignmentNode assignment)
        {
            throw new NotImplementedException();
        }

        public string VisitVariableOrFunctionCall(VariableOrFunctionCall call)
        {
            throw new NotImplementedException();
        }

        public string VisitNoOp(NoOp noOp)
        {
            throw new NotImplementedException();
        }

        public string VisitBlock(BlockNode block)
        {
            throw new NotImplementedException();
        }

        public string VisitVarDeclaration(VarDeclarationNode varDeclaration)
        {
            throw new NotImplementedException();
        }

        public string VisitProgram(PascalProgramNode program)
        {
            throw new NotImplementedException();
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
            return this.FailModel(node);
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
    }
}