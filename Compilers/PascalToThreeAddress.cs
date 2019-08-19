using System.Collections.Generic;
using Compilers.Ast;
using Compilers.Interpreter;

namespace Compilers
{
    public class ThreeAddressCollection
    {
        List<AddressNode> address = new List<AddressNode>();
        public void Add(AddressNode addressNode)
        {
            address.Add(addressNode);
        }
    }
    public class AddressNode
    {

    }
    public class PascalToThreeAddress : IPascalNodeVisitor<AddressNode>
    {
        public AddressNode VisitNode(Node node)
        {
            return this.VisitNodeModel(node);
        }

        public AddressNode VisitCompoundStatement(CompoundStatementNode statement)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitAssignment(AssignmentNode assignment)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitVariableOrFunctionCall(VariableOrFunctionCall call)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitNoOp(NoOp noOp)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitBlock(BlockNode block)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitVarDeclaration(VarDeclarationNode varDeclaration)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitProgram(PascalProgramNode program)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitProcedureDeclaration(ProcedureDeclarationNode procedureDeclaration)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitProcedureCall(ProcedureCallNode procedureCall)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitFunctionCall(CallNode functionCall)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitEqualExpression(EqualExpression equalExpression)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitNegationOperator(NegationOperator negationOperator)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitIfStatement(IfStatementNode ifStatement)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitForLoop(ForLoopNode forLoop)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitConstantDeclaration(ConstantDeclarationNode constantDeclaration)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitPointer(PointerNode pointer)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitInOperator(InOperator inOperator)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitCaseStatement(CaseStatementNode caseStatement)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitWhileLoop(WhileLoopNode whileLoop)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitReal(RealNode real)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitInteger(IntegerNode integer)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitBinaryOperator(BinaryOperator binary)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitUnary(UnaryOperator unary)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode Fail(Node node)
        {
            return this.FailModel(node);
        }

        public AddressNode VisitString(StringNode str)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitFunctionDeclaration(FunctionDeclarationNode faDeclarationNode)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitBool(BoolNode boolNode)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitRangeExpression(ListRangeExpressionNode listRange)
        {
            throw new System.NotImplementedException();
        }

        public AddressNode VisitListItemsExpression(ListItemsExpressionNode itemsExpressionNode)
        {
            throw new System.NotImplementedException();
        }
    }
}