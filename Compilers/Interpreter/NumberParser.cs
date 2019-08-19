using System;
using System.Collections.Generic;

namespace Minesweeper.Test
{
    //public class NumberParser
    //{
    //    public static (bool, NumberValue) ParseNumber(IMathValue lastNode, string currentValue, List<IMathValue> operators)
    //    {
    //        NumberValue previousValue = null;
    //        //add to the number value
    //        if (lastNode is NumberValue v)
    //        {
    //            currentValue = v.StringValue + currentValue;
    //            if (Double.TryParse(currentValue, out var ts))
    //            {
    //                previousValue = v;
    //                v.Value = ts;
    //                v.StringValue = currentValue;
    //                return (true, previousValue);
    //            }
    //        }
    //        else if (Double.TryParse(currentValue, out var t) || currentValue == ".")
    //        {
    //            var val = new NumberValue() { Value = t, StringValue = currentValue };
    //            previousValue = val;

    //            operators.Add(val);
    //        }

    //        return (false, previousValue);
    //    }
    //}
}