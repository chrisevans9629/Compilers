using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Minesweeper.Test.Tests
{
    [TestFixture]
    public class MathInterpreterTests
    {
        [Test]
        public void RpnTest()
        {
            var input = "(5 + 3) * 12 / 3";
            var lex = new RegexLexer();
            SuperBasicMathAst.AddMathTokens(lex);
            var tree = new SuperBasicMathAst(lex.Tokenize(input));

            var t = tree.Evaluate();

            var inter = new SuperBasicRpnMathInterpreter();

            var result = inter.Evaluate(t);

            result.Should().Be("5 3 + 12 * 3 /");
        }


        

        [TestCase("2 + 3", "(+ 2 3)")]
        [TestCase("(2 + 3 * 5)", "(+ 2 (* 3 5))")]
        public void LispTest(string input, string output)
        {
            var lex = new RegexLexer();
            SuperBasicMathAst.AddMathTokens(lex);
            var tree = new SuperBasicMathAst(lex.Tokenize(input));

            var t = tree.Evaluate();

            var inter = new SuperBasicMathLispInterpreter();

            var result = inter.Evaluate(t);

            result.Should().Be(output);
        }
    }
}