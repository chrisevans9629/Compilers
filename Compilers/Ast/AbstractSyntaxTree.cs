using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.Test
{
        //public class AbstractSyntaxTree : IAbstractSyntaxTree
        //{
        //    private readonly IList<Grammer> _expressions = new List<Grammer>();

        //    public void AddExpress(string expression, Func<IList<TokenItem>, double, double> eval)
        //    {
        //        _expressions.Add(new Grammer() { Expression = expression.Split(' '), Evaluate = eval });
        //    }

        //    public double Evaluate(IList<TokenItem> tokens)
        //    {
        //        var index = 0;
        //        var length = 1;
        //        var str = 0.0;
        //        do
        //        {

        //            foreach (var expression in _expressions)
        //            {
        //                var tokenSample = tokens.Skip(index).Take(length).ToList();
        //                if (expression.Expression.Length == tokenSample.Count)
        //                {
        //                    var valid = true;
        //                    for (var i = 0; i < expression.Expression.Length; i++)
        //                    {
        //                        var s = expression.Expression[i];
        //                        if (s != tokenSample[i].Token.Name)
        //                        {
        //                            valid = false;
        //                            break;
        //                        }
        //                    }

        //                    if (valid)
        //                    {
        //                        str = expression.Evaluate(tokenSample, str);
        //                        index += tokenSample.Count;
        //                        length = 1;
        //                    }

        //                }

        //                continue;
        //            }

        //            length++;
        //        } while (tokens.Count > index && tokens.Count >= length);

        //        return str;
        //    }
        //}
}