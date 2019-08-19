using System.Globalization;
using System.IO;
using Compilers.Ast;
using Compilers.Interpreter;
using Compilers.Symbols;
using FluentAssertions;
using Minesweeper.Test;
using NUnit.Framework;

namespace Minesweeper.Test.Tests
{
    [TestFixture]
    public class PascalToCSharpTests
    {
        private PascalLexer lexer;
        private PascalAst ast;
        private PascalSemanticAnalyzer table;
        PascalToCSharp cSharp;
        [SetUp]
        public void Setup()
        {
            lexer = new PascalLexer();
            ast = new PascalAst();
            table = new PascalSemanticAnalyzer();
            cSharp = new PascalToCSharp();
        }

        private string CscPath = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe";

        [Test]
        public void CscPath_ShouldFind()
        {
            File.Exists(CscPath).Should().BeTrue("the path to the csc is broken.  may break other tests");
        }

        [TestCase("program test; var asdf : integer;begin end.",
            @"public static class test
{
    static int asdf;
    public static void Main()
    {
    }
}")]
        [TestCase("program test; var asdf : integer;begin WriteLn('hello world!'); end.",
            @"using System;
public static class test
{
    static int asdf;
    public static void Main()
    {
        Console.WriteLine(" + "\"hello world!\"" + @");
    }
}")]
        [TestCase(@"
program HelloWorld;
procedure test(str :string);
    procedure write(str : string);
    begin
        writeln(str);
    end;
begin
    write(str);
end;
begin
    test('Hello, world!');
end.", @"using System;
public static class HelloWorld
{
    void test(string str)
    {
	    void write(string str1) 
	    {
		    Console.WriteLine(str1);
	    };
	    write(str);
    }
    public static void Main()
    {
	    test(" +"\"Hello, world!\"" + @");
    }
}
")]
        public void PascalInput_Should_CreateOutput(string input, string output)
        {
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            table.CheckSyntax(node);
            cSharp.VisitNode(node).Should().Be(output);

            CompileCSharp.CompileExecutable(output, "test").Should().BeTrue();
        }
    }
}