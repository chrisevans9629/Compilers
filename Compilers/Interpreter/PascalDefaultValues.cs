using System.Collections.Generic;

namespace Minesweeper.Test
{
    public class PascalDefaultValues
    {
        public PascalDefaultValues()
        {
            DefaultValues.Add(PascalTerms.Int, 0);
            DefaultValues.Add(PascalTerms.Real, 0.0);
        }
        public Dictionary<string, object> DefaultValues { get; set; } = new Dictionary<string, object>();

    }
}