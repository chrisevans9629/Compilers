using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Compilers.Interpreter
{
    public class RegexLexer
    {
        readonly IList<Token> tokens = new List<Token>();
        public void Add(string name, string expression)
        {
            tokens.Add(new Token() { Expression = expression, Name = name });
        }
        readonly IList<string> ignoreables = new List<string>();
        public void Ignore(string expression)
        {
            ignoreables.Add(expression);
        }



        public IList<TokenItem> Tokenize(string str)
        {
            var items = new List<TokenItem>();

            var matches = tokens.Select(p => new { match = Regex.Matches(str, (string)p.Expression), p });
            foreach (var match in matches)
            {
                foreach (Match o in match.match)
                {
                    if (o.Success)
                    {
                        items.Add(new TokenItem() { Token = match.p, Index = o.Index, Value = o.Value });
                    }
                }
            }

            //Validation
            {
                var rStr = str;
                foreach (var token in tokens)
                {
                    rStr = Regex.Replace(rStr, token.Expression, "");
                }

                foreach (var ignoreable in ignoreables)
                {
                    rStr = Regex.Replace(rStr, ignoreable, "");
                }

                if (string.IsNullOrEmpty(rStr) != true)
                {
                    var index = str.IndexOf(rStr, StringComparison.InvariantCulture);
                    throw new Exception($"Token '{rStr}' unrecognized at position {index} line 0");
                }
            }

            var resultTokens = items.OrderBy(p => p.Index).ToList();

            return resultTokens;
        }
    }
}