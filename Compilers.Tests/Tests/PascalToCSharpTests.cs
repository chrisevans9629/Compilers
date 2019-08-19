using System.Globalization;
using System.IO;
using FluentAssertions;
using Minesweeper.Test;
using Minesweeper.Test.Symbols;
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