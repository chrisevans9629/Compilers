using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Minesweeper.Test
{
    public abstract class ExpressionNode : Node
    {

    }


    public class CaseItemNode : Node
    {
        public CaseItemNode(IList<ExpressionNode> cases, Node statement)
        {

            Cases = cases;
            Statement = statement ?? throw new ArgumentNullException(nameof(statement));
        }

        public IList<ExpressionNode> Cases { get; set; }
        public Node Statement { get; set; }
        public override IEnumerable<Node> Children => new Node[] { Statement }.Union(Cases);

        public override string Display()
        {
            return $"{Aggregate(Cases)} : {Statement}";
        }
    }
    public class CaseStatementNode : Node, IStatementNode
    {
        public CaseStatementNode(ExpressionNode compareExpression, IList<CaseItemNode> caseItemNodes, Node elseStatement)
        {
            CompareExpression = compareExpression;
            CaseItemNodes = caseItemNodes;
            ElseStatement = elseStatement;
        }

        public ExpressionNode CompareExpression { get; set; }
        public IList<CaseItemNode> CaseItemNodes { get; set; }

        public Node ElseStatement { get; set; }
        public override IEnumerable<Node> Children => new Node[] { CompareExpression }.Union(CaseItemNodes).Append(ElseStatement);

        public override string Display()
        {
            return $"Case({CompareExpression} of {Aggregate(CaseItemNodes)} else {ElseStatement}";
        }
    }
    public class Logger : ILogger
    {
        public virtual void Log(object obj)
        {
            Console.WriteLine(obj);
        }
    }

    public interface ILogger
    {
        void Log(object obj);
    }
    public abstract class AbstractSyntaxTreeBase
    {
        protected PascalResult<Node> pascalResult;

        protected readonly ILogger Logger;

        public AbstractSyntaxTreeBase(ILogger logger)
        {
            Logger = logger ?? new Logger();
        }

        protected string Name => Current?.Token?.Name;
        protected TokenItem Current => _tokens.Current;

        public ExpressionNode Expression()
        {

            var result = AddSub();
            if (Name == PascalTerms.In)
            {
                var current = Current;
                Eat(PascalTerms.In);
                result = new InOperator(result, ListNode(), current);
            }
            if (_tokens.Current != null && Name == PascalTerms.Equal)
            {
                var current = Current;
                Eat(PascalTerms.Equal);
                result = new EqualOperator(result, AddSub(), current);
            }
            if (_tokens.Current != null && Name == PascalTerms.NotEqual)
            {
                var current = Current;
                Eat(PascalTerms.NotEqual);
                result = new NotEqualOperator(result, AddSub(), current);
            }
            return result;
        }

        public Node ListNode()
        {
            var list = Current;
            Eat(PascalTerms.LeftBracket);
            var from = new StringNode(Current);
            Eat(PascalTerms.StringConst);
            if (Name == PascalTerms.Dot)
            {
                Eat(PascalTerms.Dot);
                Eat(PascalTerms.Dot);
                var to = new StringNode(Current);
                Eat(PascalTerms.StringConst);
                Eat(PascalTerms.RightBracket);
                return new ListRangeExpressionNode(from, to, list);
            }
            else
            {
                var items = new List<StringNode>() { from };
                while (Name == PascalTerms.Comma)
                {
                    Eat(PascalTerms.Comma);
                    items.Add(new StringNode(Current));
                    Eat(PascalTerms.StringConst);
                }
                Eat(PascalTerms.RightBracket);
                return new ListItemsExpressionNode(items, list);
            }

        }

        private ExpressionNode AddSub()
        {
            ExpressionNode Action()
            {
                return MultiDiv();
            }

            var result = Action();
            while (_tokens.Current != null && _tokens.Current.Token.Name != PascalTerms.IntegerConst)
            {
                if (_tokens.Current.Token.Name == PascalTerms.Add)
                {
                    var token = _tokens.Current;
                    Eat(PascalTerms.Add);
                    result = new BinaryOperator(result, Action(), token);
                }

                else if (_tokens.Current.Token.Name == PascalTerms.Sub)
                {
                    var token = _tokens.Current;
                    Eat(PascalTerms.Sub);
                    result = new BinaryOperator(result, Action(), token);
                }
                else
                {
                    break;
                }
            }

            return result;
        }



        protected virtual ExpressionNode ParseNumber()
        {
            var current = _tokens.Current;

            if (Name == PascalTerms.Pointer)
            {
                Eat(PascalTerms.Pointer);
                return new PointerNode(current);
            }

            if (Name == PascalTerms.StringConst)
            {
                var s = new StringNode(Current);
                Eat(PascalTerms.StringConst);
                return s;
            }
            if (current.Token.Name == PascalTerms.RealConst)
            {
                Eat(PascalTerms.RealConst);
                var value = new RealNode(current);
                return value;
            }

            if (current.Token.Name == PascalTerms.IntegerConst)
            {
                Eat(PascalTerms.IntegerConst);
                var value = new IntegerNode(current);
                return value;
            }

            Error("Number");
            return null;
        }

        protected virtual ExpressionNode ParaUnaryOperators()
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
            return ParseNumber();
        }



        protected virtual ExpressionNode Parenthese()
        {
            if (_tokens.Current.Token.Name == PascalTerms.LParinth)
            {
                Eat(PascalTerms.LParinth);
                var result = Expression();
                Eat(PascalTerms.RParinth);
                var para = result;
                return para;
            }
            return null;
        }




        ExpressionNode MultiDiv()
        {
            ExpressionNode Action()
            {
                return ParaUnaryOperators();
            }

            var result = Action();

            while (_tokens.Current != null && _tokens.Current.Token.Name != PascalTerms.IntegerConst)
            {
                var token = _tokens.Current;

                if (_tokens.Current.Token.Name == PascalTerms.Multi)
                {
                    Eat(PascalTerms.Multi);
                    result = new BinaryOperator(result, Action(), token);
                }

                else if (_tokens.Current.Token.Name == PascalTerms.FloatDiv)
                {
                    Eat(PascalTerms.FloatDiv);
                    result = new BinaryOperator(result, Action(), token);
                }
                else if (_tokens.Current.Token.Name == PascalTerms.IntDiv)
                {
                    Eat(PascalTerms.IntDiv);
                    result = new BinaryOperator(result, Action(), token);
                }
                else
                {
                    break;
                }
            }


            return result;
        }
        protected Iterator<TokenItem> _tokens;

        public void Eat(string name)
        {
            if (_tokens.Current?.Token.Name == name)
            {
                Logger.Log($"Ate: {name}");
                _tokens.Advance();
            }
            else
            {
                Error(name);
            }
        }

        protected void Error(string expectedName)
        {
            var current = _tokens.CurrentOrPrevious();

            pascalResult.Errors.Add(new ParserException(ErrorCode.UnexpectedToken,
                current,
                $"Expected an '{expectedName}' token but was '{current?.Token.Name}'"));
        }

        public abstract Node Evaluate();
    }
}