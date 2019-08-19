using Compilers.MathParser;

namespace Compilers.Interpreter
{
    public abstract class NumberOperator : Operator
    {
        private bool calculated = false;

        public IMathNode First { get; set; }
        public IMathNode Second { get; set; }
        public override double Calculate()
        {
            if (!calculated)
            {
                calculated = true;
                return Calculate(First.Value ?? 0, Second.Value ?? 0);
            }
            return 0;
        }

    }
}