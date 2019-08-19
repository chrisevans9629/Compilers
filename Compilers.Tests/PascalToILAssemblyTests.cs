using System;
using System.Reflection;
using System.Reflection.Emit;
using FluentAssertions;
using Minesweeper.Test;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Compilers.Tests
{
    [TestFixture]
    public class PascalToILAssemblyTests
    {
//        [Test]
//        public void IlTest()
//        {
//            var input = @"
//program test; 
//var val : integer; 
//begin
//    val := 10;
//end.";
//            var lexer = new PascalLexer();
//            var tokens = lexer.Tokenize(input);
//            var ast = new PascalAst();
//            var node = ast.Evaluate(tokens);

//            var pascal = new PascalToIl();

//            var type = pascal.VisitNode(node).Should().BeAssignableTo<Type>().Subject;

//            type.GetFields(BindingFlags.Public | BindingFlags.Static).Should().HaveCount(1);

//            type.GetField("val", BindingFlags.Public | BindingFlags.Static).GetValue(null).Should().Be(10);
//            // the method that will hold our expression code
//            //MethodBuilder meth = tpb.DefineMethod(
//            //    "WriteValue", MethodAttributes.Public | MethodAttributes.Static);


//            //ILGenerator il = meth.GetILGenerator();


//            //var t = asm.GetType("ExpressionExecutor").GetField("val").GetValue(null);
//        }

        [Test]
        public void CreateField()
        {
            AppDomain domain = AppDomain.CurrentDomain;
            AssemblyName name = new AssemblyName();
            name.Name = "CalculatorExpression";
            // make a run-only assembly (can't save as .exe)
            var Assembly = domain.DefineDynamicAssembly(
                 name, AssemblyBuilderAccess.Run);
            ModuleBuilder module = Assembly.DefineDynamicModule(
                "CalculatorExpressionModule");
            var type = module.DefineType(
                 "ExpressionExecutor", TypeAttributes.Public);
            var field = type.DefineField("test", typeof(bool), FieldAttributes.Public | FieldAttributes.Static);
            //field.SetValue(null, true);
            var t = type.CreateType();

            t.GetField("test", BindingFlags.Public | BindingFlags.Static ).Should().NotBeNull();

            //t.GetField("test")
        }
    }
}