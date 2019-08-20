using System;
using System.Collections.Generic;
using System.Linq;
using Compilers.Ast;
using Compilers.Interpreter;
using Compilers.Symbols;
using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test.Tests
{
    [TestFixture]
    public class PascalSemanticAnalyzerTests
    {
        private PascalInterpreter interpreter;
        private PascalLexer lexer;
        private PascalAst ast;
        private PascalSemanticAnalyzer analyzer;
        private LoggerMock logger;
        [SetUp]
        public void Setup()
        {
            logger = new LoggerMock();
            interpreter = new PascalInterpreter(logger);
            lexer = new PascalLexer();
            ast = new PascalAst();
            analyzer = new PascalSemanticAnalyzer(logger);
        }

        [TestCase(@"
program test;
function factorial(
begin
    writeln(10);
end.")]
        [TestCase(@"
program test;
procedure factorial(
begin
    writeln(10);
end.")]
        [TestCase(@"
program test;
begin
    t := 10 + ( 1
    writeln(10);
end.")]
        [TestCase(@"
program test;
begin
    writeln(10;
end.")]
        [TestCase(@"
program test;
begin
    t := writeln(10;
end.")]
        public void AnalyzeWithMultipleErrors_Should_NotStackoverflow(string input)
        {
            Assert.Throws<ParserException>(() => CheckSyntax(input));
        }


        [Test]
        public void NestedFunction_Should_HaveNestedAnnotation()
        {
            var input = @"
program test; 
function write() : string;
    function write2() : string;
    begin
        write2 := 'asdf';
    end;
begin
    write := 'test';
end;
begin 
end.";
            var node = CheckSyntaxAndReturnNode(input);
            node.Should().BeOfType<PascalProgramNode>()
                .Which.Block
                .Declarations.First()
                .Should()
                .BeOfType<FunctionDeclarationNode>()
                .Which
                .Block.Declarations
                .First().Should().BeOfType<FunctionDeclarationNode>().Which.Annotations.Should().ContainKey("Nested");
        }

        [Test]
        public void NestedProcedure_Should_HaveNestedAnnotation()
        {
            var input = @"
program test; 
function write() : string;
    procedure write2();
    begin
    end;
begin
    write := 'test';
end;
begin 
end.";
            var node = CheckSyntaxAndReturnNode(input);
            node.Should().BeOfType<PascalProgramNode>()
                .Which.Block
                .Declarations.First()
                .Should()
                .BeOfType<FunctionDeclarationNode>()
                .Which
                .Block.Declarations
                .First().Should().BeOfType<ProcedureDeclarationNode>().Which.Annotations.Should().ContainKey("Nested");
        }

        [Test]
        public void Procedure_Should_NotHaveNestedAnnotation()
        {
            var input = @"
program test; 
procedure write();
    procedure write2();
    begin
    end;
begin
end;
begin 
end.";
            var node = CheckSyntaxAndReturnNode(input);
            node.Should().BeOfType<PascalProgramNode>()
                .Which.Block
                .Declarations.First()
                .Should()
                .BeOfType<ProcedureDeclarationNode>()
                .Which.Annotations.Should().NotContainKey("Nested");
        }
        [Test]
        public void Procedure_Should_HaveSymbolTable()
        {
            var input = @"
program test; 
procedure write();
    procedure write2();
    begin
    end;
begin
end;
begin 
end.";
            var node = CheckSyntaxAndReturnNode(input);
            node.Should().BeOfType<PascalProgramNode>()
                .Which.Block
                .Declarations.First()
                .Should()
                .BeOfType<ProcedureDeclarationNode>()
                .Which.Annotations.Should().ContainKey("SymbolTable").WhichValue.Should().BeOfType<ScopedSymbolTable>();
        }

        [Test]
        public void Function_Should_NotHaveNestedAnnotation()
        {
            var input = @"
program test; 
function write() : string;
    procedure write2();
    begin
    end;
begin
    write := 'test';
end;
begin 
end.";
            var node = CheckSyntaxAndReturnNode(input);
            node.Should().BeOfType<PascalProgramNode>()
                .Which.Block
                .Declarations.First()
                .Should()
                .BeOfType<FunctionDeclarationNode>()
                .Which.Annotations.Should().NotContainKey("Nested");
        }
        [Test]
        public void Function_Should_HaveSymbolTable()
        {
            var input = @"
program test; 
function write() : string;
    procedure write2();
    begin
    end;
begin
    write := 'test';
end;
begin 
end.";
            var node = CheckSyntaxAndReturnNode(input);
            node.Should().BeOfType<PascalProgramNode>()
                .Which.Block
                .Declarations.First()
                .Should()
                .BeOfType<FunctionDeclarationNode>()
                .Which.Annotations.Should().ContainKey("SymbolTable").WhichValue.Should().BeOfType<ScopedSymbolTable>();
        }

        [Test]
        public void Program_Should_HaveSymbolTable()
        {
            var input = @"program test;begin end.";
            var node = CheckSyntaxAndReturnNode(input);
            node.Should().BeOfType<PascalProgramNode>().Which.Block.Annotations.Should().ContainKey("SymbolTable").WhichValue.Should().BeOfType<ScopedSymbolTable>();
        }

        [Test]
        public void MainProgram_Should_HaveAnnotation()
        {
            var input = @"program test;begin end.";
            var node = CheckSyntaxAndReturnNode(input);
            node.Should().BeOfType<PascalProgramNode>().Which.Block.Annotations.Should().ContainKey("Main");
        }

        private Node CheckSyntaxAndReturnNode(string input)
        {
            var tokens = this.lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            analyzer.CheckSyntax(node);
            return node;
        }

        [Test]
        public void Procedure_Should_NotHaveMainAnnotation()
        {
            var input = @"program test;procedure test(); begin end; begin end.";
            var node = CheckSyntaxAndReturnNode(input);

            node.Should().BeOfType<PascalProgramNode>().Which.Block.Declarations.First().Should()
                .BeOfType<ProcedureDeclarationNode>().Which.Block.Annotations.Should().NotContainKey("Main");
        }

        [Test]
        public void FunctionAssignment_Should_Pass()
        {
            var input = @"
function GetNum: char;
var look : char;
begin
   if look = 't' then Writeln('Integer');
   GetNum := Look;
end;";
            var tokens = lexer.Tokenize(input);
            ast.CreateIterator(tokens);
            var node = ast.FunctionDeclaration();
            analyzer.CreateCurrentScope("test");
            this.analyzer.VisitFunctionDeclaration(node);
        }

        [Test]
        public void StringWithChar_Should_Pass()
        {
            var input = @"
Program HelloWorld;
var t : string;
begin
  t := 't';
  t := 'tt';
  writeln('Hello, world!');
end.
";
            CheckSyntax(input);
        }

        [Test]
        public void CharToString_Should_Fail()
        {
            var input = @"
Program HelloWorld;
var t : char;
begin
  t := 't';
  t := 'tt';
  writeln('Hello, world!');
end.";
            Assert.Throws<SemanticException>(() => CheckSyntax(input));
        }

        [Test]
        public void FunctionDeclaration_Should_CreateFunctionSymbol()
        {
            var input = PascalInterpreterTests.GetFile("PascalFunction.txt");

            var table = CheckSyntax(input);

            table.LookupSymbol<FunctionDeclarationSymbol>("Add", true).Should().NotBeNull();
        }
        [TestCase(PascalTestInputs.Invalid.FunctionDoesNotHaveReturn)]
        [TestCase(PascalTestInputs.Invalid.UndefinedProcedureAdd)]
        [TestCase(PascalTestInputs.Invalid.TooManyParametersProcedureAdd)]
        [TestCase(PascalTestInputs.Invalid.UndefinedVariableCallInProcedureAdd)]
        [TestCase(PascalTestInputs.Invalid.MismatchingType)]
        public void UndefinedProcedure_Should_ThrowSemanticException(string input)
        {
            Assert.Throws<SemanticException>(() => CheckSyntax(input));
        }


        [Test]
        public void PascalProgram_ShouldGetSymbols()
        {
            var input =
                @"PROGRAM Part10;
VAR
   number     : INTEGER;
   a, b, c, x : INTEGER;
   y          : REAL;

BEGIN {Part10}
   BEGIN
      number := 2;
      a := number;
      b := 10 * a + 10 * number DIV 4;
      c := a - - b
   END;
   x := 11;
   y := 20 / 7 + 3.14;
   { writeln('a = ', a); }
   { writeln('b = ', b); }
   { writeln('c = ', c); }
   { writeln('number = ', number); }
   { writeln('x = ', x); }
   { writeln('y = ', y); }
END.  {Part10}";
            var result = analyzer.CheckSyntax(ast.Evaluate(lexer.Tokenize(input)));
            result.Should().NotBeNull();

            var symbol = result.LookupSymbol("number", true);

            symbol.Type.Name.Should().Be(PascalTerms.Int);
        }


        [Test]
        public void PascalAssignIntegerToReal_Should_Pass()
        {
            var input = @"
program HelloWorld;
var t : real;
begin
    t := 10 * 10.0;
    writeln('Hello, world!');
end.";
            CheckSyntax(input);
        }


        [TestCase(@"
program HelloWorld;
var t : integer;
begin
    t := 10 * 10.0;
    writeln('Hello, world!');
end.")]
        [TestCase(@"
program HelloWorld;
var t : integer;
begin
    t := 10.0 * 10;
    writeln('Hello, world!');
end.")]
        public void PascalAssignRealToInteger_Should_Fail(string input)
        {
            Assert.Throws<SemanticException>(() => CheckSyntax(input));
        }



        [TestCase("program test; begin a := 2; end.")]
        [TestCase("program test; var a: integer; begin a := b; end.")]
        [TestCase("program SymTab5;\r\n    var x : integer;\r\n\r\nbegin\r\n    x := y;\r\nend.")]
        public void PascalVariableNotDelcared_Should_ThrowException(string input)
        {

            var node = ast.Evaluate(lexer.Tokenize(input));

            var tableBuilder = this.analyzer;

            var t = Assert.Throws<SemanticException>(() => tableBuilder.CheckSyntax(node));
            t.Error.Should().Be(ErrorCode.IdNotFound);
        }

        [TestCase("program SymTab6;\r\n   var x, y : integer;\r\n  y : real;\r\nbegin\r\n   x := x + y;\r\nend.")]
        public void PascalVariableDuplicated_Should_ThrowException(string input)
        {
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var result = Assert.Throws<SemanticException>(() => analyzer.CheckSyntax(node));
            result.Error.Should().Be(ErrorCode.DuplicateId);
        }

        [Test]
        public void LevelZeroScope_Should_Return2Symbols()
        {
            var input = "program SymTab3; begin end.";
            var result = CheckSyntax(input);

            result.Count.Should().BeGreaterThan(2);
        }

        private ScopedSymbolTable CheckSyntax(string input)
        {
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var result = analyzer.CheckSyntax(node);
            return result;
        }


        [Test]
        public void VariableSymbols_Should_Return4Symbols()
        {
            var input = "program SymTab3;\r\n   var x, y : integer;\r\n\r\nbegin\r\n\r\nend.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var result = analyzer.CheckSyntax(node);

            result.Count.Should().BeGreaterOrEqualTo(4);
        }

        [TestCase("PROGRAM Part11;\r\nVAR\r\n   number : INTEGER;\r\n   a, b   : INTEGER;\r\n   y      : REAL;\r\n\r\nBEGIN {Part11}\r\n   number := 2;\r\n   a := number ;\r\n   b := 10 * a + 10 * number DIV 4;\r\n   y := 20 / 7 + 3.14\r\nEND.  {Part11}")]
        public void PascalProgram_ShouldInterpretAndCreateSymbols(string input)
        {
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var result = analyzer.CheckSyntax(node);
            var memory = interpreter.Interpret(node).Should().BeOfType<Memory>().Subject;
            memory.ContainsKey("a").Should().BeTrue();
        }


        [TestCase(PascalTestInputs.PascalProgramWithProceduresWithMultipleParameters)]
        [TestCase(PascalTestInputs.PascalProgramWithMultipleVarDeclarations)]
        [TestCase(PascalTestInputs.PascalProgramWithProcedures)]
        [TestCase(PascalTestInputs.PascalProgramWithProceduresWithParameters)]
        [TestCase(PascalTestInputs.PascalSourceToSource)]
        [TestCase(PascalTestInputs.ProcedureCallXEquals10)]
        public void Pascal_OnlyPascalExceptions(string input)
        {
            DoesNotFail(input);
        }

        [Test]
        public void PascalNoNonPascalExceptionsWhenAddingTokens()
        {
            var file = PascalInterpreterTests.GetFile("PascalFunctionSelfCall.txt");
            DoesNotFail(file);
        }

        private void DoesNotFail(string file)
        {
            var list = new List<char>();
            int i = 0;
            while (list.Count < file.Length)
            {
                try
                {
                    list.Add(file[i]);
                    i++;
                    var tokens = lexer.Tokenize(file);

                    var node = ast.Evaluate(tokens);
                    var result = analyzer.CheckSyntax(node);
                }
                catch (PascalException e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        [Test]
        public void LimitedScopes_GlobalMemory_ShouldNotContainVariablesInProcedure()
        {
            var input =
                "program test; var x : integer; procedure test(a : integer;); var b : integer; begin end; begin end.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var memory = analyzer.CheckSyntax(node);

            memory.LookupSymbol("test", true).Should().NotBeNull();
            memory.LookupSymbol("x", true).Should().NotBeNull();
            memory.LookupSymbol("a", true).Should().BeNull();
            memory.LookupSymbol("b", true).Should().BeNull();
        }

        [Test]
        public void LimitedScopes_ProcedureScope_Should_CallGlobalVariable()
        {
            var input = "program globalTest; var x : integer; procedure test(a : integer;); var b : integer; begin x := 2; end; begin end.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var memory = analyzer.CheckSyntax(node);
        }

        [Test]
        public void LimitedScopes_Should_BeAbleToOverrideGlobalValue()
        {
            var input = "program globalTest; var x : integer; procedure test(x : integer;); var b : integer; begin x := 2; end; begin end.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var memory = analyzer.CheckSyntax(node);
        }

        [Test]
        public void LimitedScopes_OpensAndClosesScope()
        {
            var input = "program globalTest; var x : integer; procedure test(a : integer;); var b : integer; begin a := 2; end; begin end.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var memory = analyzer.CheckSyntax(node);


            logger.Calls.Should().Contain("Opened Scope globalTest");
            logger.Calls.Should().Contain("Opened Scope test");
            logger.Calls.Should().Contain("Closed Scope test");
            logger.Calls.Should().Contain("Closed Scope globalTest");
        }
        [TestCase(PascalTestInputs.PascalProgramWithProcedures)]
        [TestCase(PascalTestInputs.PascalProgramWithProceduresWithMultipleParameters)]
        [TestCase(PascalTestInputs.PascalProgramWithProceduresWithParameters)]
        public void PascalProgram_WithProcedures_ShouldPass(string input)
        {
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var result = analyzer.CheckSyntax(node);
        }



    }
}