using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Compilers.Interpreter;
using FastColoredTextBoxNS;
using Pascal.Ide.Wpf.Views;
using Color = System.Drawing.Color;
using FontFamily = System.Drawing.FontFamily;
using FontStyle = System.Drawing.FontStyle;
using Style = FastColoredTextBoxNS.Style;

namespace Pascal.Ide.Wpf.Models
{
    public class HighlightStyles
    {
        public HighlightParameters HighlightParameters { get; set; }
        public TextStyle TextStyle { get; set; }
    }
    public class FastCodeDocumentService : IDocumentService
    {
        private FastColoredTextBox fastColoredTextBox;
        public string Code
        {
            get => fastColoredTextBox?.Text;
            set => CodeHasChanged(value);
        }

        private void CodeHasChanged(string value)
        {
            if (fastColoredTextBox != null)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    fastColoredTextBox.Text = value;
                });

            }
        }

        public event EventHandler CodeChanged;
        public void Initialize(object rtb)
        {
            fastColoredTextBox = rtb as FastColoredTextBox;

            fastColoredTextBox.AutoIndent = true;
            fastColoredTextBox.AutoIndentChars = true;
            fastColoredTextBox.AutoIndentCharsPatterns = "Begin\r\nEnd";
            fastColoredTextBox.BackColor = Color.FromArgb(61, 61, 61);
            fastColoredTextBox.ForeColor = Color.White;

            fastColoredTextBox.Font = new Font("Times New Roman", 18);
            fastColoredTextBox.TextChangedDelayed += FastColoredTextBoxOnTextChangedDelayed;
        }

        private void FastColoredTextBoxOnTextChangedDelayed(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            OnCodeChanged();
        }



        IList<HighlightStyles> HighlightStyleList { get; set; }
        public void HighlightSyntax(IList<HighlightParameters> parameters, IList<TokenItem> tokens, IList<PascalException> errors)
        {
            fastColoredTextBox.VisibleRange.ClearStyle(errorStyle);


            if (HighlightStyleList == null)
            {
                HighlightStyleList = parameters.Select(p => new HighlightStyles()
                {
                    HighlightParameters = p,
                    TextStyle = new TextStyle(new SolidBrush(p.Color), new SolidBrush(Color.Transparent), FontStyle.Regular)
                }).ToList();
            }
            fastColoredTextBox.VisibleRange.ClearStyle(HighlightStyleList.Select(p => p.TextStyle as Style).ToArray());
            foreach (var tokenItem in tokens)
            {
                
                foreach (var highlightParameterse in HighlightStyleList)
                {
                    if (highlightParameterse.HighlightParameters.Filter(tokenItem))
                    {
                        var range = fastColoredTextBox.GetRange(tokenItem.Index, tokenItem.Index + tokenItem.Value.Length);
                        range.SetStyle(highlightParameterse.TextStyle);
                        
                    }
                }
            }

            foreach (var e in errors)
            {
                if (e.Token != null)
                {
                    var range = fastColoredTextBox.GetRange(e.Token.Index, e.Token.Index + e.Token.Value.Length);
                    range.SetStyle(errorStyle);
                }
            }
        }

        TextStyle errorStyle = new TextStyle(new SolidBrush(Color.Red), new SolidBrush(Color.Transparent), FontStyle.Regular);

        protected virtual void OnCodeChanged()
        {
            CodeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}