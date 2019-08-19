using System;
using System.IO;
using AutoFixture;
using FluentAssertions;
using Minesweeper.Test.Symbols;
using Moq;
using NUnit.Framework;

namespace Minesweeper.Test.Tests
{

    public class PascalInterpreterTests
    {
        private PascalInterpreter interpreter;
        private PascalLexer lexer;
        private PascalAst ast;
        private PascalSemanticAnalyzer analyzer;
        private ConsoleModel console;
        [SetUp]
        public void Setup()
        {
            console = new ConsoleModel();
            interpreter = new PascalInterpreter(null, console);
            lexer = new PascalLexer();
            ast = new PascalAst();
            analyzer = new PascalSemanticAnalyzer();
        }

        [Test]
        public void CaseStatementWithoutElse()
        {
            var input = @"
program checkCase;
var
   grade: char;
begin
   grade := 'A';

   case (grade) of
      'A' : writeln('Excellent!' );
      'B', 'C': writeln('Well done' );
      'D' : writeln('You passed' );
      'F' : writeln('Better try again' );
   end;     
   
   writeln('Your grade is  ', grade );
end.
";
            Evaluate(input);
            console.Output.Should().Be("Excellent!\r\nYour grade is  A\r\n");
        }


       

        [TestCase("1", "\tMOVE #1,D0\r\n")]
        [TestCase("1+2", "\tMOVE #1,D0\r\n\tMOVE D0,D1\r\n\tMOVE #2,D0\r\n\tADD D1,D0\r\n")]
        [TestCase("", "\r\n.Error: Integer Expected.\r\n")]
        public void PascalCompiler_Should_Pass(string test, string output)
        {

            console.Input = new Iterator<char>(test.ToCharArray());
            var input = GetFile("PascalCompiler.txt");
            Evaluate(input);
            console.Output.Should().Be(output);
        }

        [Test]
        public void WriteExpectedHault_ShouldWrite()
        {
            var input = @"
program test;
procedure Error(s: string);
begin
   WriteLn;
   WriteLn(^G, 'Error: ', s, '.');
end;

procedure Abort(s: string);
begin
   Error(s);
   Halt;
end;

procedure Expected(s: string);
begin
   Abort(s + ' Expected');
end;
begin
    Expected('test');
end.";
            Evaluate(input);
            console.Output.Should().Be($"{Environment.NewLine}.Error: test Expected.{Environment.NewLine}");

        }


        [TestCase("t","t","test")]
        [TestCase("t","s", "test2")]
        public void CharIfStatement(string inputConsole, string value, string output)
        {
            console.Input = new Iterator<char>(inputConsole.ToCharArray());
            var input = $@"
program test;
var tval,inp : char;
begin
    tval := '{value}';
    Read(inp);
    if tval = inp then 
        Write('test')
    else Write('test2');
end.";
            var mem = Evaluate(input).Should().BeOfType<Memory>().Subject;

            mem.GetValue("tval").Should().Be(value);
            mem.GetValue("inp").Should().Be(inputConsole);

            console.Output.Should().Be(output);
        }


        [TestCase("12","2", "\r\n.Error: 2 Expected.\r\n")]
        [TestCase("12","1", null)]
        public void Match_ShouldReturnResult(string consoleInput, string match, string result)
        {

            console.Input = new Iterator<char>(consoleInput.ToCharArray());
            var input = $@"
program test;
var Look: char;              
                              
procedure GetChar;
begin
   Read(Look);
end;

procedure Error(s: string);
begin
   WriteLn;
   WriteLn(^G, 'Error: ', s, '.');
end;

procedure Abort(s: string);
begin
   Error(s);
   Halt;
end;

procedure Expected(s: string);
begin
   Abort(s + ' Expected');
end;

procedure Match(x: char);
begin
   if Look = x then GetChar
   else Expected(x);
end;

begin
    GetChar;
    Match('{match}');
end.";
            Evaluate(input);

            console.Output.Should().Be(result);

        }


        [Test]
        public void WriteErrorHault_ShouldHault()
        {
            var input = @"
program test;
procedure Error(s: string);
begin
   WriteLn;
   WriteLn(^G, 'Error: ', s, '.');
end;


{--------------------------------------------------------------}
{ Report Error and Halt }

procedure Abort(s: string);
begin
   Error(s);
   Halt;
end;
begin
    Abort('test');
end.";
            Evaluate(input);
            console.Output.Should().Be($"{Environment.NewLine}.Error: test.{Environment.NewLine}");
        }


        [Test]
        public void WriteError_Should_WriteToConsole()
        {
            var input = @"
program test;
procedure Error(s: string);
begin
   WriteLn;
   WriteLn(^G, 'Error: ', s, '.');
end;
begin
    Error('test');
end.";
            Evaluate(input);
            console.Output.Should().Be($"{Environment.NewLine}.Error: test.{Environment.NewLine}");
        }


        [Test]
        public void ToStringOnString_Should_ReturnString()
        {
            var test = ".";

            test.ToString().Should().Be(".");
        }

        [Test]
        public void PointerG_Should_ReturnDot()
        {
            var input = @"^G";

            var tokens = lexer.Tokenize(input);
            ast.CreateIterator(tokens);

            var node = ast.Expression();

            interpreter.CreateGlobalMemory();
            var s = interpreter.VisitNode(node);

            s.ToString().Should().Be(".");
        }





        public object test()
        {
            return ".";
        }

        [Test]
        public void TestString_Should_ReturnDot()
        {
            //Passes!
            Assert.AreEqual(".", test());
        }


        private static object VisitPointer3()
        {
            return ".";
        }


        [Test]
        public void AddIntegerToItself_Should_Pass()
        {
            var input = @"
program test;
var i : integer;
begin
    i := i + 1;
    writeln(i);
end.";
            this.Evaluate(input);
            Assert.Pass();
        }

        [Test]
        public void EvaluatePointerG_Should_ReturnDot3()
        {
            var s = VisitPointer3();
            //Fails!
            Assert.AreEqual(".", s);
        }

        [Test]
        public void EvaluatePointerG_Should_ReturnDot()
        {
            var s = interpreter.VisitPointer(new PointerNode(new TokenItem() { Value = "G" }));

            s.Should().BeOfType<string>().Subject.Should().Be(".");
        }



        [Test]
        public void EvaluatePointerG_Should_ReturnDot2()
        {
            var s = interpreter.VisitPointer2("t");

            s.ToString().Should().Be(".");
        }

     

      

        [Test]
        public void AddStrings_Should_CombineStrings()
        {
            var input = @"
program test;
var item : string;
begin
    item := 'test' + 'test2';
    Write(item);
end.";
            Evaluate(input);

            console.Output.Should().Be("testtest2");
        }


        [Test]
        public void PascalReadValue_Should_SetValue()
        {
            var fixture = new Fixture();
            var num = fixture.Create<string>();
            var input = @"
program HelloWorld;
var item : char;
begin
    Read(item);
    Write(item);
end.";
            console.Input = new Iterator<char>(num.ToCharArray());
            Evaluate(input);
            console.Output[0].Should().Be(num[0]);
        }

        private object Evaluate(string input)
        {
            
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            analyzer.CheckSyntax(node);

            var n = interpreter.Interpret(node);
            return n;
        }


        [Test]
        public void PascalProcedureCall()
        {
            var input = PascalTestInputs.ProcedureCallXEquals10;

            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);

            var interpret = interpreter.Interpret(node);

            var scope = interpret.Should().BeOfType<Memory>().Which;

            scope.GetValue("x", true).Should().Be(10);
        }

        [Test]
        public void PascalForLoop()
        {
            var file = GetFile("PascalForLoop.txt");

            var tokens = lexer.Tokenize(file);
            var node = ast.Evaluate(tokens);

            var interpret = interpreter.Interpret(node);

            var scope = interpret.Should().BeOfType<Memory>().Which;

            scope.GetValue("x", true).Should().Be(10);
        }

        [Test]
        public void PascalFunctionCall()
        {
            var file = GetFile("PascalFunction.txt");

            var tokens = lexer.Tokenize(file);
            var node = ast.Evaluate(tokens);

            var interpret = interpreter.Interpret(node);

            var scope = interpret.Should().BeOfType<Memory>().Which;

            scope.GetValue("x", true).Should().Be(20);
            scope.GetValue("Add").Should().BeOfType<FunctionDeclarationNode>();
        }

        [Test]
        public void PascalFunctionSelfCall()
        {
            var file = GetFile("PascalFunctionSelfCall.txt");

            var tokens = lexer.Tokenize(file);
            var node = ast.Evaluate(tokens);

            var interpret = interpreter.Interpret(node);

            var scope = interpret.Should().BeOfType<Memory>().Which;

            scope.GetValue("x", true).Should().Be(20);
            scope.GetValue("Add").Should().BeOfType<FunctionDeclarationNode>();
        }



        [Test]
        public void PascalIf()
        {
            var file = GetFile("PascalIf.txt");

            var tokens = lexer.Tokenize(file);
            var node = ast.Evaluate(tokens);

            var interpret = interpreter.Interpret(node);

            var scope = interpret.Should().BeOfType<Memory>().Which;

            scope.GetValue("x", true).Should().Be(5);
        }

        [Test]
        public void PascalBoolean()
        {
            var file = GetFile("PascalBoolean.txt");

            var tokens = lexer.Tokenize(file);
            var node = ast.Evaluate(tokens);

            var interpret = interpreter.Interpret(node);

            var scope = interpret.Should().BeOfType<Memory>().Which;

            scope.GetValue("x", true).Should().Be(10);
            scope.GetValue("constant", true).Should().Be(true);
            scope.GetValue("determined", true).Should().Be(true);
        }


        [TestCase("1 = 1", true)]
        [TestCase("1 + 1 = 2", true)]
        [TestCase("1 = 2", false)]
        [TestCase("1.0 = 1", false)]
        public void EqualOperatorTests(string input, bool result)
        {
            var tokens = lexer.Tokenize(input);
            ast.CreateIterator(tokens);
            var node = ast.Expression().Should().BeOfType<EqualOperator>().Which;

            interpreter.CreateGlobalMemory();
            var interpret = interpreter.VisitNode(node);
            interpret.Should().BeOfType<bool>().Which.Should().Be(result);
        }

        [Test]
        public void AddIntegers_Should_BeIntegers()
        {
            var input = "1+1";
            var tokens = lexer.Tokenize(input);
            ast.CreateIterator(tokens);
            var node = ast.Expression().Should().BeOfType<BinaryOperator>().Which;
            interpreter.VisitBinaryOperator(node).Should().BeOfType<int>();
        }


        [Test]
        public void AddDouble_Should_BeDouble()
        {
            var input = "1+1.0";

            var tokens = lexer.Tokenize(input);
            ast.CreateIterator(tokens);
            var node = ast.Expression().Should().BeOfType<BinaryOperator>().Which;
            interpreter.VisitBinaryOperator(node).Should().BeOfType<double>();
        }

        [Test]
        public void PascalRecursiveFunctionSelfCall()
        {
            var file = GetFile("PascalRecursiveFunction.txt");

            var tokens = lexer.Tokenize(file);
            var node = ast.Evaluate(tokens);
            analyzer.CheckSyntax(node);
            var interpret = interpreter.Interpret(node);
            

            var scope = interpret.Should().BeOfType<Memory>().Which;

            scope.GetValue("x", true).Should().Be(6);
            scope.GetValue("Add").Should().BeOfType<FunctionDeclarationNode>();
        }

        [Test]
        public void PascalRecursiveProcedureSelfCall()
        {
            var file = GetFile("PascalRecursiveProcedure.txt");

            var tokens = lexer.Tokenize(file);
            var node = ast.Evaluate(tokens);

            var interpret = interpreter.Interpret(node);

            var scope = interpret.Should().BeOfType<Memory>().Which;

            scope.GetValue("x", true).Should().Be(6);
        }

        public static string GetFile(string fileName)
        {
            var file = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, $"TestData", fileName));
            return file;
        }

        [Test]
        public void PascalUnderScore_ShouldBeRecognized()
        {
            var input = "program test; Begin begin _num := 2 div 2; end; End.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            interpreter.Interpret(node);

            interpreter.GetVar("_num").Should().Be(1);
        }

        [TestCase("20 / 7.0", 2.95)]
        [TestCase("3.5 + 1", 4.5)]
        public void PascalMath_Should_BeDoubleAndReturnResult(string math, double result)
        {

            var input = $"program test; Begin begin num := {math}; end; End.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            interpreter.Interpret(node);

            interpreter.GetVar("num").Should().BeOfType<double>().Which.Should().BeInRange(result - 0.5, result);
        }


        [TestCase("20 div 7", 2)]
        public void PascalMath_Should_BeIntAndReturnResult(string math, int result)
        {

            var input = $"program test; Begin begin num := {math}; end; End.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            interpreter.Interpret(node);

            interpreter.GetVar("num").Should().BeOfType<int>().Which.Should().Be(result);
        }

        [TestCase("PRogram test; BEGIN\r\n\r\n    BEGIN\r\n        number := 2;\r\n        a := NumBer;\r\n        B := 10 * a + 10 * NUMBER / 4;\r\n        c := a - - b\r\n    end;\r\n\r\n    x := 11;\r\nEND.")]
        [TestCase("Program test; BEGIN\r\n    BEGIN\r\n        number := 2;\r\n        a := number;\r\n        b := 10 * a + 10 * number / 4;\r\n        c := a - - b\r\n    END;\r\n    x := 11;\r\nEND.")]
        [TestCase("Program test; BEGIN\r\n    BEGIN\r\n    {THIS IS A COMMENT}number := 2;\r\n        a := number;\r\n        b := 10 * a + 10 * number / 4;\r\n        c := a - - b\r\n    END;\r\n    x := 11;\r\nEND.")]
        public void PascalInterpreter_ShouldExcludeCommentsAndCasing(string input)
        {
            var tokens = lexer.Tokenize(input);


            var node = ast.Evaluate(tokens);

            interpreter.Interpret(node);

            interpreter.GetVar("a").Should().Be(2);
            interpreter.GetVar("x").Should().Be(11);
            interpreter.GetVar("c").Should().Be(27);
            interpreter.GetVar("b").Should().Be(25);
            interpreter.GetVar("nuMber").Should().Be(2);
        }

        [Test]
        public void FullProgram_ShouldWork()
        {
            var input =
                "PROGRAM Part10;\r\nVAR\r\n   number     : INTEGER;\r\n   a, b, c, x : INTEGER;\r\n   y          : REAL;\r\n\r\nBEGIN {Part10}\r\n   BEGIN\r\n      number := 2;\r\n      a := number;\r\n      b := 10 * a + 10 * number DIV 4;\r\n      c := a - - b\r\n   END;\r\n   x := 11;\r\n   y := 20 / 7 + 3.14;\r\n   { writeln('a = ', a); }\r\n   { writeln('b = ', b); }\r\n   { writeln('c = ', c); }\r\n   { writeln('number = ', number); }\r\n   { writeln('x = ', x); }\r\n   { writeln('y = ', y); }\r\nEND.  {Part10}";
            var node = ast.Evaluate(lexer.Tokenize(input));
            interpreter.Interpret(node);
        }

        [Test]
        public void BasicProgram_ShouldPass()
        {
            var input = "PROGRAM Part10;\r\nBEGIN\r\nEND.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            interpreter.Interpret(node);
        }

        [Test]
        public void LimitedScopes_GlobalMemory_ShouldNotContainVariablesInProcedure()
        {
            var input =
                "program test; var x : integer; procedure test(a : integer;); var b : integer; begin end; begin end.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var memory = interpreter.Interpret(node);

            var global = memory.Should().BeOfType<Memory>().Which;

            global.ContainsKey("x").Should().BeTrue();
            global.ContainsKey("a").Should().BeFalse();
            global.ContainsKey("b").Should().BeFalse();
        }

        [TestCase(PascalTestInputs.PascalProgramWithProcedures)]
        [TestCase(PascalTestInputs.PascalProgramWithProceduresWithParameters)]
        [TestCase(PascalTestInputs.PascalProgramWithProceduresWithMultipleParameters)]
        public void PascalProgram_WithProcedures_ShouldPass(string input)
        {
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var memory = interpreter.Interpret(node);
        }
    }
}