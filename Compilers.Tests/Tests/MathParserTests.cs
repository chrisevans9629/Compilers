using FluentAssertions;
using NUnit.Framework;

namespace Minesweeper.Test.Tests
{
    //public abstract class DoubleOperator : Operator
    //{
    //    private bool _calculated = false;

    //    public override double Calculate()
    //    {
    //        if (!_calculated)
    //        {
    //            _calculated = true;
    //            return Calculate((First.Value ?? 0), (Second.Value ?? 0));
    //        }
    //        return 0;
    //    }

    //    public IMathNode First { get; set; }
    //    public IMathNode Second { get; set; }
    //}


    //[TestFixture]
    //public class MathParserTests
    //{
    //    private MathParser parser;

    //    [SetUp]
    //    public void Setup()
    //    {
    //        parser = new MathParser();
    //    }

    //    [Test]
    //    public void Two()
    //    {
    //        parser.Calculate("2").Should().Be(2);
    //    }

    //    [Test]
    //    public void TwoAddTwo()
    //    {
    //        parser.Calculate("2+2").Should().Be(4);
    //    }

    //    [TestCase("2+2+2", 6)]
    //    [TestCase("2+2+2+2", 8)]
    //    [TestCase("2+4", 6)]
    //    [TestCase("20+1", 21)]
    //    [TestCase("20+15", 35)]
    //    [TestCase("20", 20)]
    //    [TestCase("200+10", 210)]
    //    [TestCase("200-10", 190)]
    //    [TestCase("200*10", 2000)]
    //    [TestCase("1.5+2.5", 4)]
    //    [TestCase(".5+2.5", 3)]
    //    [TestCase("10+10-10", 10)]
    //    // [TestCase("10-10*10", 90)]
    //    //[TestCase("10-10-10*10", 80)]

    //    public void Tests(string math, double result)
    //    {
    //        parser.Calculate(math).Should().Be(result);
    //    }
    //}
}