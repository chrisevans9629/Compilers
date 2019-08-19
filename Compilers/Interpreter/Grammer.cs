using System;
using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class Grammer
    {
        public string[] Expression { get; set; }

        public Func<IList<TokenItem>, double, double> Evaluate { get; set; }
    }
}