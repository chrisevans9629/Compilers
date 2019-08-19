using System;

namespace Minesweeper.Test
{
    public abstract class PascalNodeVisitor<T> : IPascalNodeVisitor<T>
    {
        public virtual T VisitNode(Node node)
        {
            return this.VisitNodeModel(node);
        }

        public virtual T VisitCompoundStatement(CompoundStatementNode statement)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitAssignment(AssignmentNode assignment)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitVariableOrFunctionCall(VariableOrFunctionCall call)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitNoOp(NoOp noOp)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitBlock(BlockNode block)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitVarDeclaration(VarDeclarationNode varDeclaration)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitProgram(PascalProgramNode program)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitProcedureDeclaration(ProcedureDeclarationNode procedureDeclaration)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitProcedureCall(ProcedureCallNode procedureCall)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitFunctionCall(CallNode functionCall)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitEqualExpression(EqualExpression equalExpression)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitNegationOperator(NegationOperator negationOperator)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitIfStatement(IfStatementNode ifStatement)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitForLoop(ForLoopNode forLoop)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitConstantDeclaration(ConstantDeclarationNode constantDeclaration)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitPointer(PointerNode pointer)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitInOperator(InOperator inOperator)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitCaseStatement(CaseStatementNode caseStatement)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitWhileLoop(WhileLoopNode whileLoop)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitReal(RealNode real)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitInteger(IntegerNode integer)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitBinaryOperator(BinaryOperator binary)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitUnary(UnaryOperator unary)
        {
            throw new NotImplementedException();
        }

        public virtual T Fail(Node node)
        {
            return this.FailModel(node);
        }

        public virtual T VisitString(StringNode str)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitFunctionDeclaration(FunctionDeclarationNode faDeclarationNode)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitBool(BoolNode boolNode)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitRangeExpression(ListRangeExpressionNode listRange)
        {
            throw new NotImplementedException();
        }

        public virtual T VisitListItemsExpression(ListItemsExpressionNode itemsExpressionNode)
        {
            throw new NotImplementedException();
        }
    }
}