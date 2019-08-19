using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class PascalProgramNode : Node
    {
        public string ProgramName { get; set; }
        public BlockNode Block { get; set; }
        public TokenItem TokenItem { get; set; }
        public PascalProgramNode(TokenItem name, BlockNode block)
        {
            ProgramName = name?.Value;
            Block = block;
            TokenItem = name;
        }

        public override IEnumerable<Node> Children => new[] {Block};

        public override string Display()
        {
            return $"Program({ProgramName}, {Block.Display()})";
        }
    }
}