using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    //public class SecondMultiply : DoubleOperator
    //{
    //    public override double Calculate(double first, double second)
    //    {
    //        return first * second;
    //    }
    //}

    //public class Multiply : NumberOperator
    //{
    //    public override double Calculate(double first, double second)
    //    {
    //        return first * second;
    //    }
    //}

    //public class DoubleOperatorInject : DoubleOperator
    //{
    //    private readonly Operation _operation;

    //    public DoubleOperatorInject(Operation operation)
    //    {
    //        _operation = operation;
    //    }
    //    public override double Calculate(double first, double second)
    //    {
    //        return _operation(first, second);
    //    }
    //}

    //public class FullOperator
    //{
    //    public FullOperator(Operation operation)
    //    {
    //        NumberOperator = new NumberOperatorInject(operation);
    //    }
    //    public NumberOperator NumberOperator { get; set; }
    //}

    //public class MathParser : IMathParser
    //{
    //    MathOperatorCollection collection = new MathOperatorCollection();
    //    public MathParser()
    //    {
    //        collection.AddOperator('*', (first, second) => first * second);
    //        collection.AddOperator('+', (first, second) => first + second);
    //        collection.AddOperator('-', (first, second) => first - second);
    //    }
    //    public double Calculate(string value)
    //    {
            
    //        var operators = new List<IMathValue>();
    //        NumberValue previousValue = null;
    //        foreach (char valueChars in value)
    //        {
    //            var currentValue = valueChars.ToString();
    //            var last = operators.LastOrDefault();
    //            var parseResult = NumberParser.ParseNumber(last, currentValue, operators);
    //            if (parseResult.Item2 != null)
    //            {
    //                previousValue = parseResult.Item2;
    //            }
    //            if (parseResult.Item1)
    //            {
    //                continue;
    //            }
    //            var lastOp = operators.OfType<NumberOperator>().LastOrDefault();

    //            var tresult = collection.ParseChar(valueChars, lastOp, previousValue);
    //            if(tresult != null) operators.Add(tresult);
                

    //            //var secAdd = operators.OfType<DoubleOperator>().LastOrDefault();
    //            var secAdd = operators.OfType<NumberOperator>().LastOrDefault(p => p.First is NumberOperator);
    //            if (secAdd != null && secAdd.Second == null)
    //            {
    //                secAdd.Second = previousValue;
    //                continue;
    //            }
    //            if (lastOp != null && lastOp.Second == null)
    //            {
    //                lastOp.Second = previousValue;
    //                continue;
    //            }
    //        }

    //        var si = operators.OfType<NumberValue>().FirstOrDefault();
    //        if (si != null && operators.Count == 1)
    //        {
    //            return si.Value ?? 0;
    //        }

    //        return operators.OfType<Operator>().Sum(p => p.Value ?? 0);
    //    }
    //}
}