using System;
using Compilers.MathParser;

namespace Compilers.Interpreter
{
    public abstract class Operator : IMathValue
    {
        public abstract double Calculate(double first, double second);
        public abstract double Calculate();
        public double? Value { get => Calculate(); set => new NotImplementedException(); }

    }
}