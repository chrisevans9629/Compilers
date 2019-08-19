using System;
using System.Collections.Generic;
using System.Linq;
using Compilers.Ast;

namespace Compilers.Interpreter
{
    public class PascalInterpreter : PascalNodeVisitor<object>
    {
        private readonly IConsole _console;
        private ILogger _logger;
        public PascalInterpreter(ILogger logger = null, IConsole console = null)
        {
            _console = console ?? new ConsoleModel();
            this._logger = logger ?? new Logger();
        }


        public override object VisitReal(RealNode num)
        {
            return num.Value;
        }

        public override object VisitUnary(UnaryOperator op)
        {
            if (op.Name == PascalTerms.Add)
            {
                return VisitNode(op.Value);
            }

            if (op.Name == PascalTerms.Sub)
            {
                var value = VisitNode(op.Value);
                if (value is double d)
                {
                    return -d;
                }
                if (value is int i)
                {
                    return -i;
                }
            }
            return Fail(op);
        }

        public override object Fail(Node node)
        {
            return this.FailModel(node);
        }



        public override object VisitInteger(IntegerNode integer)
        {
            return integer.Value;
        }

        public override object VisitBinaryOperator(BinaryOperator op)
        {
            var doubleActions = new Dictionary<string, Func<double, double, double>>();
            doubleActions.Add(PascalTerms.Add, (d, d1) => d + d1);
            doubleActions.Add(PascalTerms.Sub, (d, d1) => d - d1);
            doubleActions.Add(PascalTerms.Multi, (d, d1) => d * d1);
            doubleActions.Add(PascalTerms.FloatDiv, (d, d1) => d / d1);
            doubleActions.Add(PascalTerms.IntDiv, (d, d1) => (int)d / (int)d1);

            var intActions = new Dictionary<string, Func<int, int, int>>();
            intActions.Add(PascalTerms.Add, (d, d1) => d + d1);
            intActions.Add(PascalTerms.Sub, (d, d1) => d - d1);
            intActions.Add(PascalTerms.Multi, (d, d1) => d * d1);
            intActions.Add(PascalTerms.FloatDiv, (d, d1) => d / d1);
            intActions.Add(PascalTerms.IntDiv, (d, d1) => d / d1);


            var strActions = new Dictionary<string, Func<string, string, string>>();
            strActions.Add(PascalTerms.Add, (d, d1) => d + d1);



            var left = VisitNode(op.Left);
            var right = VisitNode(op.Right);
            if (left is int l && right is int r)
            {
                if (intActions.ContainsKey(op.Name) != true)
                {
                    return Fail(op);
                }
                return intActions[op.Name](l, r);
            }
            else if ((left is int || left is double) && (right is int || right is double))
            {
                if (doubleActions.ContainsKey(op.Name) != true)
                {
                    return Fail(op);
                }
                return doubleActions[op.Name](Convert.ToDouble(left), Convert.ToDouble(right));
            }
            else if (left is string || right is string)
            {
                if (strActions.ContainsKey(op.Name) != true)
                {
                    return Fail(op);
                }

                return strActions[op.Name](left?.ToString(), right?.ToString());
            }
            else
            {
                return Fail(op);
            }

        }



        public object Interpret(Node node)
        {
            CreateGlobalMemory();

            try
            {
                var result = VisitNode(node);
            }
            catch (HaltException e)
            {
                Console.WriteLine(e);
            }
            return CurrentScope;


        }

        public void CreateGlobalMemory()
        {
            CurrentScope = new Memory("GLOBAL");
            // CurrentScope.Add("Read",null);
        }


        public object GetVar(string key)
        {
            return CurrentScope.GetValue(key, true);
        }
        private Memory CurrentScope;
        public override object VisitCompoundStatement(CompoundStatementNode compound)
        {
            foreach (var compoundNode in compound.Nodes)
            {
                VisitNode(compoundNode);
            }

            return compound;
        }

        public override object VisitNode(Node node)
        {
            return this.VisitNodeModel(node);
        }


        public override object VisitBool(BoolNode boolNode)
        {
            return boolNode.Value;
        }

        public override object VisitRangeExpression(ListRangeExpressionNode listRange)
        {
            var from = listRange.FromNode.CurrentValue[0];

            var to = listRange.ToNode.CurrentValue[0];
            var list = Enumerable.Range(from, to).Select(p => (char)p);

            return list.Cast<object>();
        }

        public override object VisitListItemsExpression(ListItemsExpressionNode itemsExpressionNode)
        {
            var items = itemsExpressionNode.Items.Select(p => p.CurrentValue[0]);

            return items.Cast<object>();
        }

        public override object VisitFunctionDeclaration(FunctionDeclarationNode funcdec)
        {
            CurrentScope.Add(funcdec.FunctionName, funcdec);
            return null;
        }

        public override object VisitString(StringNode str)
        {
            return str.CurrentValue;
        }

        public override object VisitWhileLoop(WhileLoopNode whileLoop)
        {
            while ((bool)VisitNode(whileLoop.BoolExpression))
            {
                VisitNode(whileLoop.DoStatement);
            }
            return null;
        }

        public override object VisitCaseStatement(CaseStatementNode caseStatement)
        {
            var comparer = VisitNode(caseStatement.CompareExpression);

            foreach (var caseItem in caseStatement.CaseItemNodes)
            {
                foreach (var caseItemCase in caseItem.Cases)
                {
                    var value = VisitNode(caseItemCase);
                    if (value.Equals(comparer))
                    {
                        return VisitNode(caseItem.Statement);
                    }
                }
            }
            if (caseStatement.ElseStatement != null)
                return VisitNode(caseStatement.ElseStatement);
            return null;
        }

        public override object VisitInOperator(InOperator inOperator)
        {
            var valueToContain = VisitNode(inOperator.CompareNode).ToString()[0];

            var list = VisitNode(inOperator.ListExpression);
            if (list is IEnumerable<object> enumerable)
            {
                return enumerable.Contains(valueToContain);
            }

            throw new NotImplementedException("must be a list");

        }

        public object VisitPointer2(object pointer)
        {
            if (pointer != null)
            {
                return ".";
            }
            return null;
        }
        public override object VisitPointer(PointerNode pointer)
        {
            if (pointer.Value == 'I')
            {
                return "\t";
            }

            if (pointer.Value == 'G')
            {
                return ".";
            }
            throw new NotImplementedException($"{pointer}");
        }

        public override object VisitConstantDeclaration(ConstantDeclarationNode constantDeclarationNode)
        {
            CurrentScope.Add(constantDeclarationNode.ConstantName, VisitNode(constantDeclarationNode.Value));
            return null;
        }

        public override object VisitForLoop(ForLoopNode forLoop)
        {
            var fromName = forLoop.AssignFromNode.Left.VariableName;
            VisitAssignment(forLoop.AssignFromNode);
            var toInt = VisitNode(forLoop.ToNode);
            var fromValue = CurrentScope.GetValue(fromName, true);
            CurrentScope = new Memory("_ForLoop_", CurrentScope);
            for (var i = (int)fromValue; i <= (int)toInt; i++)
            {
                VisitNode(forLoop.DoStatements);
            }
            CurrentScope = CurrentScope.Parent;
            return null;
        }

        public override object VisitIfStatement(IfStatementNode ifNode)
        {
            if ((bool)VisitNode(ifNode.IfCheck))
            {
                var node = ifNode.IfTrue;
                VisitNode(node);
            }
            else if (ifNode.IfFalse != null)
            {
                var node = ifNode.IfFalse;
                VisitNode(node);
            }

            return null;
        }



        public override object VisitNegationOperator(NegationOperator negation)
        {
            var right = VisitNode(negation.Right);
            if (right is bool b)
            {
                return !b;
            }

            throw new RuntimeException(ErrorCode.UnexpectedToken, negation.TokenItem,
                $"Did not find an equality operator called {negation.TokenItem.Token.Name}");
        }

        public override object VisitEqualExpression(EqualExpression exp)
        {
            var left = VisitNode(exp.Left);
            var right = VisitNode(exp.Right);
            if (exp is EqualOperator)
            {
                return left.Equals(right);
            }
            else if (exp is NotEqualOperator)
            {
                return !left.Equals(right);
            }

            throw new RuntimeException(ErrorCode.UnexpectedToken, exp.TokenItem,
                $"Did not find an equality operator called {exp.Name}");
        }

        private void VisitCall<T>(CallNode call) where T : DeclarationNode
        {
            try
            {
                var declaration = CurrentScope.GetValue<T>(call.Name, true);
                var values = new List<object>();
                for (var i = 0; i < declaration.Parameters.Count; i++)
                {
                    var value = VisitNode(call.Parameters[i]);
                    values.Add(value);
                }

                var previous = CurrentScope;
                CurrentScope = new Memory(call.Name, previous);
                CurrentScope.Add(call.Name, null);
                for (var i = 0; i < declaration.Parameters.Count; i++)
                {
                    var parameter = declaration.Parameters[i];
                    VisitVarDeclaration(parameter.Declaration);
                    CurrentScope.SetValue(parameter.Declaration.VarNode.VariableName, values[i]);
                }

                VisitBlock(declaration.Block);
            }
            catch (HaltException)
            {
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new RuntimeException(ErrorCode.Runtime, call.Token, "Unexpected Error", e);
            }

        }

        public override object VisitFunctionCall(CallNode call)
        {
            try
            {
                VisitCall<FunctionDeclarationNode>(call);

                var funResult = CurrentScope.GetValue(call.Name);
                CurrentScope = CurrentScope.Parent;
                return funResult;
            }
            catch (HaltException)
            {
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new RuntimeException(ErrorCode.Runtime, call.Token, "Unexpected error", e);
            }

        }

        public override object VisitProcedureCall(ProcedureCallNode call)
        {
            if (call.ProcedureName.ToUpper() == "READ")
            {
                var result = _console.Read();
                if (call.Parameters[0] is VariableOrFunctionCall v)
                {
                    CurrentScope.SetValue(v.VariableName, result.ToString());
                }

                return null;
            }

            if (call.ProcedureName.ToUpper() == "WRITE")
            {
                var str = "";
                foreach (var callParameter in call.Parameters)
                {
                    str += VisitNode(callParameter).ToString();
                }
                _console.Write(str);
                return null;
            }
            if (call.ProcedureName.ToUpper() == "WRITELN")
            {
                var str = "";
                foreach (var callParameter in call.Parameters)
                {
                    str += VisitNode(callParameter).ToString();
                }
                _console.WriteLine(str);
                return null;
            }

            if (call.ProcedureName.ToUpper() == "HALT")
            {
                throw new HaltException(ErrorCode.Runtime, call.Token, "Halted");
            }
            VisitCall<ProcedureDeclarationNode>(call);

            CurrentScope = CurrentScope.Parent;

            return null;
        }

        public override object VisitProcedureDeclaration(ProcedureDeclarationNode procedure)
        {
            this.CurrentScope.Add(procedure.ProcedureId, procedure);
            return null;
        }

        public override object VisitProgram(PascalProgramNode programNode)
        {
            return VisitBlock(programNode.Block);
        }
        PascalDefaultValues defaultValues = new PascalDefaultValues();
        public override object VisitVarDeclaration(VarDeclarationNode varDeclaration)
        {
            var typeName = varDeclaration.TypeNode.TypeValue.ToUpper();
            if (defaultValues.DefaultValues.ContainsKey(typeName))
            {
                this.CurrentScope.Add(varDeclaration.VarNode.VariableName, defaultValues.DefaultValues[typeName]);
            }
            else
            {
                this.CurrentScope.Add(varDeclaration.VarNode.VariableName, null);
            }
            return null;
        }
        public override object VisitBlock(BlockNode block)
        {
            foreach (var blockDeclaration in block.Declarations)
            {
                VisitNode(blockDeclaration);
            }

            return VisitCompoundStatement(block.CompoundStatement);
        }

        public override object VisitAssignment(AssignmentNode node)
        {
            var name = node.Left.VariableName.ToUpper();
            var value = VisitNode(node.Right);
            if (CurrentScope.ContainsKey(name))
            {
                CurrentScope.SetValue(name, value);
            }
            else
            {
                CurrentScope.Add(name, value);
            }

            return value;
        }

        public override object VisitVariableOrFunctionCall(VariableOrFunctionCall var)
        {
            var name = var.VariableName.ToUpper();
            var value = CurrentScope.GetValue(name, true);
            if (value is FunctionDeclarationNode node)
            {
                var call = new FunctionCallNode(node.FunctionName, new List<Node>(), node.Token);
                return VisitNode(call);
            }
            return value;
        }

        public override object VisitNoOp(NoOp noop)
        {
            return noop;
        }
    }
}