using System;
using Compilers.MathParser;

namespace Compilers.Interpreter
{
    public class OperatorValue : IMathValue
    {
        private readonly Operation _op;

        public OperatorValue(Operation op)
        {
            _op = op;
        }

        public double? Value
        {
            get => _op(First.Value ?? 0, Second.Value ?? 0);
            set => throw new NotImplementedException();
        }

        public IMathNode First { get; set; }
        public IMathNode Second { get; set; }

    }
}