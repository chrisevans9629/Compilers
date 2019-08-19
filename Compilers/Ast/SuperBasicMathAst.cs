using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
    public class SuperBasicMathAst : AbstractSyntaxTreeBase
    {
        public SuperBasicMathAst(IList<TokenItem> data, ILogger logger = null) : base(logger)
        {
           
            _tokens = new Iterator<TokenItem>(data.ToArray());
            //_tokens = GetTokens();
        }

        public static void AddMathTokens(RegexLexer lex)
        {
            lex.Ignore(" ");
            lex.Add(PascalTerms.LParinth, @"\(");
            lex.Add(PascalTerms.RParinth, @"\)");
            lex.Add(PascalTerms.IntegerConst, @"\d+");
            lex.Add(PascalTerms.Add, @"\+");
            lex.Add(PascalTerms.Sub, @"-");
            lex.Add(PascalTerms.Multi, @"\*");
            lex.Add(PascalTerms.FloatDiv, @"/");
        }

        

        public override Node Evaluate()
        {
            var result = Expression();
            return result;
        }

       


    }
}