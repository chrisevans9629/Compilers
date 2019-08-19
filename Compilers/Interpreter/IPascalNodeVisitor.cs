using System;

namespace Minesweeper.Test
{
    public interface IPascalNodeVisitor<T>
    {
        T VisitNode(Node node);
        T VisitCompoundStatement(CompoundStatementNode statement);
        T VisitAssignment(AssignmentNode assignment);
        T VisitVariableOrFunctionCall(VariableOrFunctionCall call);
        T VisitNoOp(NoOp noOp);
        T VisitBlock(BlockNode block);
        T VisitVarDeclaration(VarDeclarationNode varDeclaration);
        T VisitProgram(PascalProgramNode program);
        T VisitProcedureDeclaration(ProcedureDeclarationNode procedureDeclaration);
        T VisitProcedureCall(ProcedureCallNode procedureCall);
        T VisitFunctionCall(CallNode functionCall);
        T VisitEqualExpression(EqualExpression equalExpression);
        T VisitNegationOperator(NegationOperator negationOperator);
        T VisitIfStatement(IfStatementNode ifStatement);
        T VisitForLoop(ForLoopNode forLoop);
        T VisitConstantDeclaration(ConstantDeclarationNode constantDeclaration);
        T VisitPointer(PointerNode pointer);
        T VisitInOperator(InOperator inOperator);
        T VisitCaseStatement(CaseStatementNode caseStatement);
        T VisitWhileLoop(WhileLoopNode whileLoop);
        T VisitReal(RealNode real);
        T VisitInteger(IntegerNode integer);
        T VisitBinaryOperator(BinaryOperator binary);
        T VisitUnary(UnaryOperator unary);
        T Fail(Node node);
        T VisitString(StringNode str);
        T VisitFunctionDeclaration(FunctionDeclarationNode faDeclarationNode);
        T VisitBool(BoolNode boolNode);
        T VisitRangeExpression(ListRangeExpressionNode listRange);
        T VisitListItemsExpression(ListItemsExpressionNode itemsExpressionNode);
    }
}
