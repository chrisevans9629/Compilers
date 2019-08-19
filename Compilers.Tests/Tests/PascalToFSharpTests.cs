using System;
using System.IO;
using System.Threading;
using Compilers.Ast;
using Compilers.Interpreter;
using Compilers.Symbols;
using FluentAssertions;
using FSharp.Compiler;
using FSharp.Compiler.SourceCodeServices;
using Microsoft.FSharp.Core;
using NUnit.Framework;

namespace Minesweeper.Test.Tests
{
    [TestFixture]
    public class PascalToFSharpTests
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
        [TestCase("let x = 10", true)]
        [TestCase("let x = 10 + y", false)]
        public void Test(string input, bool isValid)
        {
            var temp1 = Path.GetTempFileName();

            var fn1 = Path.ChangeExtension(temp1, ".fsx");


            File.WriteAllText(fn1, input);
            var fn2 = Path.ChangeExtension(temp1, ".dll");
            FSharpChecker checker = FSharpChecker.Create(FSharpOption<int>.None,
                FSharpOption<bool>.None,
                FSharpOption<bool>.None,
                FSharpOption<ReferenceResolver.Resolver>.None,
                FSharpOption<FSharpFunc<Tuple<string, DateTime>,
                    FSharpOption<Tuple<object, IntPtr, int>>>>.None,
                FSharpOption<bool>.None);
            var result = checker.CompileToDynamicAssembly(new[] { "-o", fn2, "-a", fn1 },
                FSharpOption<Tuple<TextWriter, TextWriter>>.None, FSharpOption<string>.None);
            var t = Microsoft.FSharp.Control.FSharpAsync.RunSynchronously(result, FSharpOption<int>.None,
                FSharpOption<CancellationToken>.None);
            if (isValid)
            {
                t.Item1.Should().BeNullOrEmpty();
            }
            else
            {
                t.Item1.Should().NotBeNullOrEmpty();

            }
        }
    }
}