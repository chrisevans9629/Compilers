namespace Compilers.Interpreter
{
    public class NumberOperatorInject : NumberOperator
    {
        private readonly Operation _operation;

        public NumberOperatorInject(Operation operation)
        {
            _operation = operation;
        }
        public override double Calculate(double first, double second)
        {
            return _operation(first, second);
        }
    }
}