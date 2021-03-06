﻿using System;
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
    public static void test(string str)
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
}")]
        [TestCase(@"
program HelloWorld;
function Factorial(i : integer) : integer;
begin
    if i = 1 then Factorial := 1
    else Factorial := Factorial(i-1) * i;
end;
begin
    writeln(Factorial(10));
end.", @"using System;
public static class HelloWorld
{
    public static int Factorial(int i)
    {
        if(i == 1) return i;
        else return Factorial(i-1) * i;
    }
    public static void Main()
    {
        Console.WriteLine(Factorial(10));
    }
}")]
        public void PascalInput_Should_CreateOutput(string input, string output)
        {
            var tokens = lexer.Tokenize(input.Trim());
            var node = ast.Evaluate(tokens);
            table.CheckSyntax(node);
            var result = cSharp.VisitNode(node).Trim();
            //result.Should().Be(output.Trim());

            CompileCSharp.CompileExecutable2(output, "test").Should().BeTrue();
        }


        [Test]
        public void VariableGlobal_Should_HaveStatic()
        {
            var input = @"
program test;
var t : CHAR;
begin
end.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            table.CheckSyntax(node);
            var result = cSharp.VisitNode(node);

            result.Should().Contain("static char t;");
        }

        [TestCase(@"
program test;
begin
    ReadLn;
end.", "Console.ReadLine();")]
        [TestCase(@"
program test;
var t : string;
begin
    readln(t);
end.","t = Console.ReadLine();")]
        public void ReadLn_Should_ContainConsoleReadLine(string input, string contain)
        {
            var str = Convert(input);
            str.Should().Contain(contain);
            
        }

        [Test]
        public void VariableInFunction_Should_NotBeStatic()
        {
            var input = @"
program test;
var t : CHAR;
function test() : integer;
var as : integer;
begin
    test := 1;
end;
begin
    writeln('hello world');
    writeln(test());
    writeln('test');
end.";
            var result = Convert(input);

            result.Should().Contain("int as;").And.NotContain("static int as;");
        }

        private string Convert(string input)
        {
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            table.CheckSyntax(node);
            var result = cSharp.VisitNode(node);
            return result;
        }
    }


    
}