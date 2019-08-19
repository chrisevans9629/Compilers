namespace Minesweeper.Test
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