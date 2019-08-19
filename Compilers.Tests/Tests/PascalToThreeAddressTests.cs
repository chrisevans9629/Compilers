using Compilers;
using Compilers.Ast;
using Compilers.Interpreter;
using Compilers.Symbols;
using NUnit.Framework;

namespace Minesweeper.Test.Tests
{
    [TestFixture]
    public class PascalToThreeAddressTests
    {
        private PascalLexer lexer;
        private PascalAst ast;
        private PascalSemanticAnalyzer analyzer;
        private PascalToThreeAddress threeAddress;
        [SetUp]
        public void Setup()
        {
            lexer = new PascalLexer();
            ast = new PascalAst();
            analyzer = new PascalSemanticAnalyzer();
            threeAddress = new PascalToThreeAddress();
        }
        [Test]
        public void MathTest()
        {
            var input = @"
program test;
function factorial(i : integer) : integer;
begin
    if i = 1 then factorial := 1
    else factorial := factorial(i-1) * i;
end;
begin
    writeln(factorial(10));
end.";
            var ouput = @"
push 10
goto factorial
call writeln
factorial:
i = pop
if i = 1 then push 1
";
        }
    }
}