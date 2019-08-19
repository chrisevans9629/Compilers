namespace Minesweeper.Test
{
    public static class PascalNodeVisitorExtension
    {
        public static T FailModel<T>(this IPascalNodeVisitor<T> visit, Node node)
        {
            throw new ParserException(ErrorCode.UnexpectedToken, null, $"did not recognize node '{node}'");
        }
        public static T VisitNodeModel<T>(this IPascalNodeVisitor<T> visitor, Node node)
        {
            if (node is CompoundStatementNode compound)
            {
                return visitor.VisitCompoundStatement(compound);
            }

            if (node is BlockNode block)
            {
                return visitor.VisitBlock(block);
            }
            if (node is EqualExpression exp)
            {
                return visitor.VisitEqualExpression(exp);
            }

            if (node is NegationOperator negation)
            {
                return visitor.VisitNegationOperator(negation);
            }
            if (node is AssignmentNode assign)
            {
                return visitor.VisitAssignment(assign);
            }

            if (node is VariableOrFunctionCall var)
            {
                return visitor.VisitVariableOrFunctionCall(var);
            }

            if (node is NoOp no)
            {
                return visitor.VisitNoOp(no);
            }

            if (node is PascalProgramNode program)
            {
                return visitor.VisitProgram(program);
            }

            if (node is ProcedureCallNode call)
            {
                return visitor.VisitProcedureCall(call);
            }

            if (node is ProcedureDeclarationNode procedure)
            {
                return visitor.VisitProcedureDeclaration(procedure);
            }

            if (node is BoolNode b)
            {
                return visitor.VisitBool(b);
            }

            if (node is IfStatementNode ifNode)
            {
                return visitor.VisitIfStatement(ifNode);
            }

            if (node is WhileLoopNode whileLoop)
            {
                return visitor.VisitWhileLoop(whileLoop);
            }

            if (node is ForLoopNode forLoop)
            {
                return visitor.VisitForLoop(forLoop);
            }

            if (node is FunctionCallNode funcCall)
            {
                return visitor.VisitFunctionCall(funcCall);
            }

            if (node is FunctionDeclarationNode funcdec)
            {
                return visitor.VisitFunctionDeclaration(funcdec);
            }

            if (node is VarDeclarationNode declaration)
            {
                return visitor.VisitVarDeclaration(declaration);
            }

            if (node is ConstantDeclarationNode constantDeclarationNode)
            {
                return visitor.VisitConstantDeclaration(constantDeclarationNode);
            }

            if (node is PointerNode pointer)
            {
                return visitor.VisitPointer(pointer);
            }

            if (node is StringNode str)
            {
                return visitor.VisitString(str);
            }

            if (node is InOperator inOperator)
            {
                return visitor.VisitInOperator(inOperator);
            }

            if (node is CaseStatementNode caseStatement)
            {
                return visitor.VisitCaseStatement(caseStatement);
            }

            if (node is RealNode leaf)
            {
                return visitor.VisitReal(leaf);
            }

            if (node is IntegerNode integer)
            {
                return visitor.VisitInteger(integer);
            }

            if (node is BinaryOperator op)
            {
                return visitor.VisitBinaryOperator(op);
            }

            if (node is UnaryOperator un)
            {
                return visitor.VisitUnary(un);
            }

            if (node is ListItemsExpressionNode itemsExpressionNode)
            {
                return visitor.VisitListItemsExpression(itemsExpressionNode);
            }

            if (node is ListRangeExpressionNode listRange)
            {
                return visitor.VisitRangeExpression(listRange);
            }
            return visitor.Fail(node);
        }
    }
}