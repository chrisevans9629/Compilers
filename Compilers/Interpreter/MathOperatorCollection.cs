using System;
using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class MathOperatorCollection
    {
      

        public IMathValue ParseChar(char value, NumberOperator lastOp, NumberValue previousValue)
        {
            if (operators.ContainsKey(value))
            {
                var opFunc = operators[value];
                if (lastOp != null)
                {
                    var op = opFunc();
                    op.First = lastOp;
                    return op;
                }
                else
                {
                    var op = opFunc();
                    op.First = previousValue;
                    return op;
                }
            }

            return null;
        }
        public void AddOperator(char value, Operation op)
        {
            operators.Add(value, () => new NumberOperatorInject(op));
        }



        private Dictionary<char, Func<NumberOperator>> operators = new Dictionary<char, Func<NumberOperator>>();
    }
}