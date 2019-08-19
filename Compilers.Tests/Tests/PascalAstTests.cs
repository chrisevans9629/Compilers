using System;
using System.Collections.Generic;
using System.Linq;
using Compilers.Ast;
using Compilers.Interpreter;
using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test.Tests
{
    [TestFixture]
    public class PascalAstTests
    {
        private PascalLexer lexer;
        private PascalAst ast;
        [SetUp]
        public void Setup()
        {
            lexer = new PascalLexer();
            ast = new PascalAst();
        }

        [Test]
        public void PointerI_Should_BeI()
        {
            var input = @"^I";

            var tokens = lexer.Tokenize(input);
            ast.CreateIterator(tokens);
            var node = ast.Expression();

            node.Should().BeOfType<PointerNode>().Which.Value.Should().Be('I');
        }

        [Test]
        public void WhileLoopAndInList_Should_Pass()
        {
            var input = @"
procedure Expression;
begin
   Term;
   while Look in ['+', '-'] do
		begin
		   EmitLn('MOVE D0,D1');
		   case Look of
			'+': Add;
			'-': Subtract;
		   else Expected('Addop');
		   end;
		end;
	end;
   
end;";
            var tokens = lexer.Tokenize(input);
            ast.CreateIterator(tokens);
            var node = ast.ProcedureDeclaration();
        }

        [Test]
        public void ListItems_Should_Have2Items()
        {
            var input = @" ['+', '-']";

            var tokens = lexer.Tokenize(input);
            ast.CreateIterator(tokens);
            var node = ast.ListNode().Should().BeOfType<ListItemsExpressionNode>().Subject;

            node.Items.Should().HaveCount(2);
        }
        [Test]
        public void CaseStatementProcedureWithElse_ShouldPass()
        {
            var input = @"
procedure Expression;
begin
   Term;
   EmitLn('MOVE D0,D1');
   case Look of
    '+': Add;
    '-': Subtract;
   else Expected('Addop');
   end;
end;";
            var tokens = lexer.Tokenize(input);
            ast.CreateIterator(tokens);
            var node = ast.ProcedureDeclaration();
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
            var tokens = lexer.Tokenize(input);

            var node = ast.Evaluate(tokens).Should().BeOfType<PascalProgramNode>().Subject;

            var statements = node.Block.CompoundStatement.Nodes;

            statements[0].Should().BeOfType<AssignmentNode>();
            var t = statements[1].Should().BeOfType<CaseStatementNode>().Subject;
            t.CompareExpression.Should().BeOfType<VariableOrFunctionCall>();
            var items = t.CaseItemNodes;
            items.Should().HaveCount(4);

            items[1].Cases.Should().HaveCount(2);
            items[1].Statement.Should().BeOfType<ProcedureCallNode>();

            statements[2].Should().BeOfType<ProcedureCallNode>();

        }

        [Test]
        public void FunctionWithIfStatementAndReturn_ShouldBeSeperate()
        {
            var input = @"
function GetName: char;
begin
   if not IsAlpha(Look) then Expected('Name');
   GetName := UpCase(Look);
   GetChar;
end;";
            var tokens = lexer.Tokenize(input);
            ast.CreateIterator(tokens);
            var node = ast.FunctionDeclaration();

            Console.WriteLine(node);
            var func = node.Should().BeOfType<FunctionDeclarationNode>().Which;


            var nodes = func.Block.CompoundStatement.Nodes;
            func.Block.CompoundStatement.Nodes.Should().HaveCount(4);


            var ifstat = nodes[0].Should().BeOfType<IfStatementNode>().Which;
            nodes[1].Should().BeOfType<AssignmentNode>();
            nodes[2].Should().BeOfType<ProcedureCallNode>();
            nodes[3].Should().BeOfType<NoOp>();

        }

        [Test]
        public void FullProgramTestTreeOutput()
        {
            var input = "PROGRAM Part10AST;\r\nVAR\r\n   a, b : INTEGER;\r\n   y    : REAL;\r\n\r\nBEGIN {Part10AST}\r\n   a := 2;\r\n   b := 10 * a + 10 * a DIV 4;\r\n   y := 20 / 7 + 3.14;\r\nEND.  {Part10AST}";

            var tokens = lexer.Tokenize(input);

            var node = ast.Evaluate(tokens);


            Console.WriteLine(node.Display());
            //var interpreter = new PascalInterpreter();
            //interpreter.Evaluate(ast.Evaluate());
        }


        [TestCase("10*10=100")]
        [TestCase("10+10=20")]
        [TestCase("10+10.0=20")]
        [TestCase("10/10.0=20")]
        [TestCase("10-10.0=20")]
        [TestCase("10 Div 10.0=20")]
        [TestCase("10*2+10=20")]
        [TestCase("10+2*10=20")]
        public void EqualTest(string input)
        {
            var tokens = lexer.Tokenize(input);
            ast.CreateIterator(tokens);
            var node = ast.Expression().Should().BeOfType<EqualOperator>();
        }

        [TestCase("10+10*10")]
        public void AddMultiExpression_Add_Should_BeTopNode(string input)
        {
            var tokens = lexer.Tokenize(input);
            ast.CreateIterator(tokens);
            var node = ast.Expression().Should().BeOfType<BinaryOperator>().Which.Name.Should().Be(PascalTerms.Add);
        }

        [Test]
        public void RecursiveFunctionTest()
        {
            var input =
                "function Summation (num : integer) : integer;\r\nbegin\r\n  if num = 1 \r\n  then Summation := 1\r\n  else Summation := Summation(num-1) + num\r\nend;";
            var tokens = lexer.Tokenize(input);
            ast.CreateIterator(tokens);
            var node = ast.FunctionDeclaration();

            var fun = node.Should().BeOfType<FunctionDeclarationNode>().Which;
            fun.FunctionName.Should().Be("Summation");
            fun.ReturnType.TypeValue.Should().Be("integer");
            fun.Parameters.Should().HaveCount(1);
            fun.Parameters[0].Declaration.TypeNode.TypeValue.Should().Be("integer");
            fun.Parameters[0].Declaration.VarNode.VariableName.Should().Be("num");

            var block = fun.Block;
            block.Declarations.Should().BeEmpty();
            var ifStatement = block.CompoundStatement.Nodes.Should().HaveCount(1).And.Subject.First().Should().BeOfType<IfStatementNode>().Which;

            ifStatement.IfCheck.Should().BeOfType<EqualOperator>().Which.Left.Should().BeOfType<VariableOrFunctionCall>();
            ifStatement.IfCheck.Should().BeOfType<EqualOperator>().Which.Right.Should().BeOfType<IntegerNode>();
           var trueStatement = ifStatement.IfTrue.Should().BeOfType<AssignmentNode>().Which;
           trueStatement.Left.VariableName.Should().Be("Summation");
           trueStatement.Right.Should().BeOfType<IntegerNode>();
           var falseStatement =  ifStatement.IfFalse.Should().BeOfType<AssignmentNode>().Which;

           falseStatement.Left.VariableName.Should().Be("Summation");
           falseStatement.Right.Should().BeOfType<BinaryOperator>().Which.Left.Should().BeOfType<FunctionCallNode>()
               .Which.Parameters.Should().HaveCount(1).And.Subject.First().Should().BeOfType<BinaryOperator>();
        }

        [Test]
        public void PascalIfTest()
        {
            var input = "if x = 10 then x := 5 else x := 1";

            var tokens = lexer.Tokenize(input);

            ast.CreateIterator(tokens);

           var result = ast.Statement();
           var iff = result.Should().BeOfType<IfStatementNode>().Which;
           iff.IfTrue.Should().BeOfType<AssignmentNode>();
           iff.IfFalse.Should().BeOfType<AssignmentNode>();

        }
        [Test]
        public void BooleanParse()
        {
            var file = PascalInterpreterTests.GetFile("PascalBoolean.txt");

            var tokens = lexer.Tokenize(file);
            var node = ast.Evaluate(tokens);
        }

        [Test]
        public void BooleanSimpleParse()
        {
            var input = "3 = 3";
            var tokens = lexer.Tokenize(input);
            ast.CreateIterator(tokens);
            var node = ast.Expression();
            var equal = node.Should().BeOfType<EqualOperator>().Which;
            equal.Left.Should().BeOfType<IntegerNode>();
            equal.Right.Should().BeOfType<IntegerNode>();
        }

        [TestCase("4 = 5")]
        [TestCase("(1+1) = (2+1)")]
        [TestCase("2*1= (2+1)")]
        [TestCase("(2+1)*2 = (2+1)")]
        public void BoolTheory(string input)
        {
            var tokens = lexer.Tokenize(input);
            ast.CreateIterator(tokens);
            var node = ast.Expression();
            node.Should().BeOfType<EqualOperator>();
        }


        [Test]
        public void ThrowParserException()
        {
            var input = "program;";

            var tokens = lexer.Tokenize(input);
            var result = Assert.Throws<ParserException>(() => ast.Evaluate(tokens));

            result.Token.Line.Should().Be(1);
            result.Message.Should().Be("Expected an 'ID' token but was ';' at line 1 column 8 index 7");
        }

        [Test]
        public void PascalAst_ShouldStartWithProgram()
        {
            var input =
                "Program test; BEGIN\r\n    BEGIN\r\n        number := 2;\r\n        a := number;\r\n        b := 10 * a + 10 * number / 4;\r\n        c := a - - b\r\n    END;\r\n    x := 11;\r\nEND.";
            var tokens = lexer.Tokenize(input);
            var node = ast.Evaluate(tokens);
            var t = node.Display();
            node.Should().BeOfType<PascalProgramNode>();
        }

        [TestCase(PascalTestInputs.PascalProgramWithProcedures)]
        [TestCase(PascalTestInputs.PascalProgramWithProceduresWithParameters)]
        [TestCase(PascalTestInputs.PascalProgramWithProceduresWithMultipleParameters)]
        [TestCase(PascalTestInputs.PascalProgramWithMultipleVarDeclarations)]
        public void PascalProgram_WithProcedures_ShouldPass(string input)
        {
            var tokens = lexer.Tokenize(input);
            var inter = ast.Evaluate(tokens);

        }

    }
    
}