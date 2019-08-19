namespace Compilers.MathParser
{
    public class MathTree
    {
        public MathTree()
        {

        }
        public IMathNode ParentNode { get; set; }

        public double? Value => ParentNode.Value;
    }
}