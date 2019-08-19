using System;
using System.Collections.Generic;
using System.Linq;
using Compilers.Interpreter;

namespace Compilers.Ast
{
   

    public class PascalAst : AbstractSyntaxTreeBase
    {

        private IList<TokenItem> items;


        public PascalAst(ILogger logger = null) : base(logger)
        {

        }

        CompoundStatementNode CompoundStatement()
        {
            Eat(PascalTerms.Begin);
            var nodes = StatementList();
            Eat(PascalTerms.End);
            var root = new CompoundStatementNode();
            root.Nodes = nodes.ToList();
            return root;
        }

        private BlockNode Block()
        {
            var dec = Declarations();
            var comStat = CompoundStatement();
            var node = new BlockNode(dec, comStat);
            return node;
        }

        private IList<Node> Declarations()
        {
            var dec = new List<Node>();

            while (Name == PascalTerms.Const)
            {
                Eat(PascalTerms.Const);
                while (Name == PascalTerms.Id)
                {
                    var vardec = ConstantDeclarations();
                    foreach (var varDeclarationNode in vardec)
                    {
                        dec.Add(varDeclarationNode);
                    }
                }

            }

            while (Name == PascalTerms.Var)
            {
                Eat(PascalTerms.Var);
                while (Name == PascalTerms.Id)
                {
                    var vardec = VariableDeclaration();
                    foreach (var varDeclaration in vardec)
                    {
                        dec.Add(varDeclaration);
                    }
                    Eat(PascalTerms.Semi);
                }
            }

            while (Name == PascalTerms.Procedure || Name == PascalTerms.Function)
            {
                if (Name == PascalTerms.Procedure)
                {
                    var proc = ProcedureDeclaration();
                    dec.Add(proc);
                }

                if (Name == PascalTerms.Function)
                {
                    var fun = FunctionDeclaration();
                    dec.Add(fun);
                }
            }

            return dec;
        }
        public FunctionDeclarationNode FunctionDeclaration()
        {
            var parameters = new List<ParameterNode>();
            Eat(PascalTerms.Function);
            var token = Current;
            Eat(PascalTerms.Id);
            var procedureId = token.Value;

            if (Name == PascalTerms.LParinth)
            {
                Eat(PascalTerms.LParinth);
                RightParenthesisWithBreak(() =>
                {
                    parameters.AddRange(VariableDeclaration().Select(p => new ParameterNode(p)));
                    if (Name == PascalTerms.Semi)
                    {
                        Eat(PascalTerms.Semi);
                    }
                });

                Eat(PascalTerms.RParinth);
            }
            Eat(PascalTerms.Colon);
            var returnType = TypeSpec();
            Eat(PascalTerms.Semi);
            var block = Block();
            Eat(PascalTerms.Semi);
            var proc = new FunctionDeclarationNode(procedureId, parameters, block, token, returnType);
            return proc;
        }

        private void RightParenthesisWithBreak(Action action)
        {
            var errorCount = this.pascalResult.Errors.Count;
            while (Name != PascalTerms.RParinth)
            {
                action();

                var currentErrors = pascalResult.Errors.Count;
                if (currentErrors > errorCount)
                {
                    Eat(PascalTerms.RParinth);
                    break;
                }
            }
        }

        public ProcedureDeclarationNode ProcedureDeclaration()
        {
            var parameters = new List<ParameterNode>();
            Eat(PascalTerms.Procedure);
            var procedureId = Current.Value;
            Eat(PascalTerms.Id);
            if (Name == PascalTerms.LParinth)
            {
                Eat(PascalTerms.LParinth);
                RightParenthesisWithBreak(() =>
                {
                    parameters.AddRange(VariableDeclaration().Select(p => new ParameterNode(p)));
                    if (Name == PascalTerms.Semi)
                    {
                        Eat(PascalTerms.Semi);
                    }
                });

                Eat(PascalTerms.RParinth);
            }

            Eat(PascalTerms.Semi);
            var block = Block();
            Eat(PascalTerms.Semi);
            var proc = new ProcedureDeclarationNode(procedureId, block, parameters);
            return proc;
        }

        IList<ConstantDeclarationNode> ConstantDeclarations()
        {
            var nodes = new List<ConstantDeclarationNode>() { ConstantDeclaration() };
            while (Name == PascalTerms.Id)
            {
                nodes.Add(ConstantDeclaration());
            }
            return nodes;
        }

        private ConstantDeclarationNode ConstantDeclaration()
        {
            var current = Current;
            Eat(PascalTerms.Id);
            Eat(PascalTerms.Equal);
            var value = Expression();
            Eat(PascalTerms.Semi);
            return new ConstantDeclarationNode(current.Value, value, current);
        }

        private IList<VarDeclarationNode> VariableDeclaration()
        {
            var nodes = new List<VariableOrFunctionCall> { Variable() };
            while (Name == PascalTerms.Comma)
            {
                Eat(PascalTerms.Comma);
                nodes.Add(Variable());
            }
            Eat(PascalTerms.Colon);

            var type = TypeSpec();
            return nodes.Select(p => new VarDeclarationNode(p, type)).ToList();
        }

        private TypeNode TypeSpec()
        {
            if (Current == null)
                Error("Type");
            var result = new TypeNode(Current);

            if (PascalTerms.BuiltInTypes.Contains(result.TypeValue.ToUpper()))
            {
                Eat(Name);
            }

            return result;
        }

        IList<Node> StatementList()
        {
            var node = Statement();

            var results = new List<Node> { node };

            while (Name == PascalTerms.Semi)
            {
                Eat(PascalTerms.Semi);
                results.Add(Statement());
            }

            //if (_tokens.Current?.Token?.Name == Pascal.Id)
            //{
            //    Error();
            // }

            return results;
        }

        protected override ExpressionNode ParaUnaryOperators()
        {
            var current = _tokens.Current;
            var par = Parenthese();
            if (par != null) return par;

            if (current.Token.Name == PascalTerms.Add)
            {
                Eat(PascalTerms.Add);
                return new UnaryOperator(ParaUnaryOperators(), current);
            }
            if (current.Token.Name == PascalTerms.Sub)
            {
                Eat(PascalTerms.Sub);
                return new UnaryOperator(ParaUnaryOperators(), current);
            }

            if (Name == PascalTerms.Id && this._tokens.Peek().Token.Name == PascalTerms.LParinth)
            {
                return FunctionCall();
            }
            if (current.Token.Name == PascalTerms.Id)
            {
                return Variable();
            }
            if (current.Token.Name == PascalTerms.Equal)
            {
                Eat(PascalTerms.Equal);
            }

            if (current.Token.Name == PascalTerms.BoolConst)
            {
                return ParseBool();
            }
            return base.ParaUnaryOperators();
            //else
            //{
            //    var node = Variable();
            //    return node;
            //}
        }
        private ExpressionNode ParseBool()
        {
            var b = new BoolNode(_tokens.Current);
            _tokens.Advance();
            return b;
        }
        private FunctionCallNode FunctionCall()
        {
            var procedureName = Current.Value;
            var token = Current;
            Eat(PascalTerms.Id);
            Eat(PascalTerms.LParinth);
            var parameters = new List<Node>();
            var errorCount = pascalResult.Errors.Count;
            while (Current.Token.Name != PascalTerms.RParinth)
            {
                parameters.Add(Expression());
                if (Current.Token.Name == PascalTerms.Comma)
                {
                    Eat(PascalTerms.Comma);
                }
                var currentErrors = pascalResult.Errors.Count;
                if (currentErrors > errorCount)
                {
                    Eat(PascalTerms.RParinth);
                    break;
                }
            }
            Eat(PascalTerms.RParinth);
            return new FunctionCallNode(procedureName, parameters, token);
        }

        public Node Statement()
        {
            Node node = null;
            if (Name == PascalTerms.For)
            {
                node = ForLoop();
            }
            else if (Name == PascalTerms.Case)
            {
                node = CaseStatement();
            }
            else if (Name == PascalTerms.If)
            {
                node = IfStatement();
            }
            else if (Name == PascalTerms.Begin)
            {
                node = CompoundStatement();
            }
            else if (Name == PascalTerms.Id && _tokens.Peek()?.Token?.Name == PascalTerms.Assign)
            {
                node = AssignmentStatement();
            }
            else if (Name == PascalTerms.Id)
            {
                node = ProcedureCall();
            }
            else if (Name == PascalTerms.While)
            {
                node = WhileLoop();
            }
            else
            {
                node = Empty();
            }
            return node;
        }

        private WhileLoopNode WhileLoop()
        {
            Eat(PascalTerms.While);

            var exp = BoolExpression();
            Eat(PascalTerms.Do);

            var statement = Statement();
            return new WhileLoopNode(exp, statement);
        }

        private CaseStatementNode CaseStatement()
        {
            Eat(PascalTerms.Case);
            var exp = Expression();
            Eat(PascalTerms.Of);
            var items = new List<CaseItemNode>();
            while (Name != PascalTerms.End && Name != PascalTerms.Else)
            {
                items.Add(CaseItem());
            }

            Node elseExpressionNode = null;
            if (Name == PascalTerms.Else)
            {
                Eat(PascalTerms.Else);
                elseExpressionNode = Statement();
                Eat(PascalTerms.Semi);
            }
            Eat(PascalTerms.End);
            return new CaseStatementNode(exp, items, elseExpressionNode);
        }

        private CaseItemNode CaseItem()
        {
            var items = new List<ExpressionNode>(){Expression()};
            while (Name == PascalTerms.Comma)
            {
                Eat(PascalTerms.Comma);
                items.Add(Expression());
            }
            Eat(PascalTerms.Colon);
            var statement = Statement();
            Eat(PascalTerms.Semi);
            return new CaseItemNode(items, statement);

        }

        private Node ForLoop()
        {
            Eat(PascalTerms.For);
            var assign = AssignmentStatement();
            Eat(PascalTerms.To);
            var ex = Expression();
            Eat(PascalTerms.Do);
            var statements = Statement();
            return new ForLoopNode(assign, ex, statements);
        }

        Node BoolExpression()
        {
            if (Name == PascalTerms.Not)
            {
                var current = Current;
                Eat(PascalTerms.Not);
                return new NegationOperator(Expression(),current);
            }
            return Expression();
        }
        private IfStatementNode IfStatement()
        {
            Eat(PascalTerms.If);
            var equal = BoolExpression();
            Eat(PascalTerms.Then);
            
            var tstatement = Statement();
            Node estate = null;
            if (Current.Token.Name == PascalTerms.Else)
            {
                Eat(PascalTerms.Else);
                estate =Statement();
            }
            return new IfStatementNode(equal, tstatement, estate);
        }

        private ProcedureCallNode ProcedureCall()
        {
            var procedureName = Current.Value;
            var token = Current;
            Eat(PascalTerms.Id);
            var parameters = new List<Node>();

            if (Name == PascalTerms.LParinth)
            {
                var errorCount = pascalResult.Errors.Count;
                Eat(PascalTerms.LParinth);
                while (Name != PascalTerms.RParinth)
                {
                    parameters.Add(Expression());
                    if (Name == PascalTerms.Comma)
                    {
                        Eat(PascalTerms.Comma);
                    }
                    else
                    {
                        break;
                    }
                    var currentErrors = pascalResult.Errors.Count;
                    if (currentErrors > errorCount)
                    {
                        Eat(PascalTerms.RParinth);
                        break;
                    }

                }
                Eat(PascalTerms.RParinth);
            }
           
            return new ProcedureCallNode(procedureName, parameters, token);
        }

        AssignmentNode AssignmentStatement()
        {
            var left = Variable();
            var token = _tokens.Current;
            Eat(PascalTerms.Assign);
            var right = Expression();
            var node = new AssignmentNode(left, token, right);
            return node;
        }

        VariableOrFunctionCall Variable()
        {
            var token = _tokens.Current;
            Eat(PascalTerms.Id);

            var node = new VariableOrFunctionCall(token);
            return node;
        }
        Node Parse()
        {
            var node = Program();
            if (this._tokens.Current != null)
            {
                Error("null");
            }
            return node;
        }
        NoOp Empty()
        {
            return new NoOp();
        }
        Node Program()
        {
            Eat(PascalTerms.Program);
            var name = _tokens?.Current;
            Eat(PascalTerms.Id);
            Eat(PascalTerms.Semi);
            var node = Block();
            Eat(PascalTerms.Dot);
            var programNode = new PascalProgramNode(name, node);
            return programNode;
        }
        public override Node Evaluate()
        {
            return Evaluate(items);
        }

        public PascalResult<Node> EvaluateResult(IList<TokenItem> tokens)
        {
            CreateIterator(tokens);
            var node = Parse();
            pascalResult.Result = node;
            return pascalResult;
        }
        public Node Evaluate(IList<TokenItem> tokens)
        {
            var result = EvaluateResult(tokens);
            if (result.Errors.Any())
            {
                throw result.Errors[0];
            }
            return result.Result;
        }

        public void CreateIterator(IList<TokenItem> tokens)
        {
            pascalResult = new PascalResult<Node>();
            _tokens = new Iterator<TokenItem>(tokens.ToArray());
        }

        
    }

    public class ConstantDeclarationNode : Node
    {
        public string ConstantName { get; }
        public Node Value { get; }
        public TokenItem TokenItem { get; }

        public ConstantDeclarationNode(string constantName, Node value, TokenItem tokenItem)
        {
            ConstantName = constantName;
            Value = value;
            TokenItem = tokenItem;
        }

        public override IEnumerable<Node> Children => new[] {Value};

        public override string Display()
        {
            return $"Constant({ConstantName}, {Value})";
        }
    }
}