using System;
using System.Collections.Generic;

namespace Compilers.Interpreter
{
    public class Grammer
    {
        public string[] Expression { get; set; }

        public Func<IList<TokenItem>, double, double> Evaluate { get; set; }
    }
}