using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compilers.Ast;
using Compilers.Interpreter;
using Compilers.Symbols;

namespace Compilers.Build
{
    using Microsoft.Build.Utilities;

    public class BuildPascal : AppDomainIsolatedTask
    {
        public string FileName { get; set; }
        public override bool Execute()
        {
            try
            {
                var text = File.ReadAllText(FileName);

                var lexer = new PascalLexer();
                var tokens = lexer.Tokenize(text);

                var ast = new PascalAst();

                var node = ast.Evaluate(tokens);

                var syntaxChecker = new PascalSemanticAnalyzer();
                syntaxChecker.CheckSyntax(node);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            //System.Diagnostics.Debugger.Launch();


        }


    }

}
