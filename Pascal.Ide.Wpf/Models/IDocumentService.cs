using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Compilers.Interpreter;
using Pascal.Ide.Wpf.Models;
using Color = System.Drawing.Color;

namespace Pascal.Ide.Wpf.Views
{
    
    public interface IDocumentService
    {
        string Code { get; set; }
        event EventHandler CodeChanged;
        void Initialize(object rtb);

        void HighlightSyntax(IList<HighlightParameters> parameters, IList<TokenItem> tokens, IList<PascalException> errors);
    }

    public class HighlightParameters
    {
        public Func<TokenItem, bool> Filter { get; set; }

        public Color Color { get; set; }
    }
}