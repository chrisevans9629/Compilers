using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public class BlockNode : Node
    {
        public IList<Node> Declarations { get; }
        public CompoundStatementNode CompoundStatement { get; }

        public BlockNode(IList<Node> declarations, CompoundStatementNode compoundStatement)
        {
            Declarations = declarations;
            CompoundStatement = compoundStatement;
        }

        public override IEnumerable<Node> Children => Declarations.Append(CompoundStatement);

        public override string Display()
        {
            return $"Block({Aggregate(Declarations)}, {CompoundStatement.Display()})";
        }
    }
}