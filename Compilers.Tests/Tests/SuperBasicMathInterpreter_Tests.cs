using Compilers.Ast;
using Compilers.Interpreter;
using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test.Tests
{
    [TestFixture]
    public class SuperBasicMathInterpreter_Tests
    {
        [TestCase("1+1", 2)]
        [TestCase("1-1", 0)]
        [TestCase("14 + 2 * 3 - 6 / 2", 17)]
        [TestCase("2 * 2", 4)]
        [TestCase("(1+2)*2", 6)]
        [TestCase("2 * 2 +2", 6)]
        [TestCase("7 + 3 * (10 / (12 / (3 + 1) - 1))", 22)]
        [TestCase("+ - 3", -3)]
        [TestCase("5 - - 2", 7)]
        [TestCase("5 + - 2", 3)]
        [TestCase("--2", 2)]
        public void Evaluate_Test(string input, double output)
        {
            var lex = new RegexLexer();
            SuperBasicMathAst.AddMathTokens(lex);
            var tree = new SuperBasicMathAst(lex.Tokenize(input));
            var t = tree.Evaluate();

            var inter = new PascalInterpreter();

            var r = inter.VisitNode(t);
            r.Should().Be(output);
        }


        [Test]
        public void UnaryOperation()
        {
            var input = "-2";

            var lex = new RegexLexer();
            SuperBasicMathAst.AddMathTokens(lex);
            var tree = new SuperBasicMathAst(lex.Tokenize(input));

            var node = tree.Evaluate();
            node.Should().BeOfType<UnaryOperator>().Which.Name.Should().Be(PascalTerms.Sub);
        }

        [Test]
        public void BasicAst_Should_Add()
        {
            var input = "1+2";
            var lex = new RegexLexer();
            SuperBasicMathAst.AddMathTokens(lex);
            var tree = new SuperBasicMathAst(lex.Tokenize(input));
            var t = tree.Evaluate();

            var node = t.Should().BeOfType<BinaryOperator>().Which;
            node.Name.Should().Be(PascalTerms.Add);
            var left = node.Left.Should().BeOfType<IntegerNode>().Which;
            left.TokenItem.Token.Name.Should().Be(PascalTerms.IntegerConst);
            left.TokenItem.Value.Should().Be("1");
            var right = node.Right.Should().BeOfType<IntegerNode>().Which;

            right.TokenItem.Token.Name.Should().Be(PascalTerms.IntegerConst);
            right.TokenItem.Value.Should().Be("2");
        }

        [Test]
        public void AddAndSubtract_Should_HaveSubAtTopOfTree()
        {
            var input = "1+2-3";

            var lex = new RegexLexer();
            SuperBasicMathAst.AddMathTokens(lex);
            var tree = new SuperBasicMathAst(lex.Tokenize(input));

            var t = tree.Evaluate().Should().BeOfType<BinaryOperator>().Which;
            t.Name.Should().Be(PascalTerms.Sub);
        }

        [Test]
        public void BasicAst_Should_Subtract()
        {
            var input = "1-2";
            var lex = new RegexLexer();
            SuperBasicMathAst.AddMathTokens(lex);
            var tree = new SuperBasicMathAst(lex.Tokenize(input));
            var t = tree.Evaluate().Should().BeOfType<BinaryOperator>().Which;

            t.TokenItem.Token.Name.Should().Be(PascalTerms.Sub);

            var left = t.Left.Should().BeOfType<IntegerNode>().Which;
            var right = t.Right.Should().BeOfType<IntegerNode>().Which;

            left.TokenItem.Token.Name.Should().Be(PascalTerms.IntegerConst);
            left.TokenItem.Value.Should().Be("1");
            right.TokenItem.Token.Name.Should().Be(PascalTerms.IntegerConst);
            right.TokenItem.Value.Should().Be("2");
        }

        [Test]
        public void TestTree()
        {
            var mulToken = new TokenItem { Value = "*", Token = new Token() { Name = PascalTerms.Multi } };
            var addToken = new TokenItem { Value = "+", Token = new Token() { Name = PascalTerms.Add } };
            var mulop = new BinaryOperator(
                new RealNode(new TokenItem { Value = "2", Token = new Token() { Name = PascalTerms.IntegerConst } }),
                new RealNode(new TokenItem { Value = "7", Token = new Token() { Name = PascalTerms.IntegerConst } }),
                mulToken);
            var addop = new BinaryOperator(
                mulop,
                new RealNode(new TokenItem { Value = "3", Token = new Token() { Name = PascalTerms.IntegerConst } }),
                addToken);

            addop.TokenItem.Token.Name.Should().Be(PascalTerms.Add);

        }
    }
}