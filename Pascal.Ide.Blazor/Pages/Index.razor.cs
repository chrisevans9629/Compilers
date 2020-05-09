using Compilers.Ast;
using Compilers.Interpreter;
using Compilers.Symbols;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pascal.Ide.Blazor.Pages
{

    public class IndexBase : ComponentBase, IDisposable, ILogger
    {
        PascalLexer lexer;
        PascalAst ast;
        PascalSemanticAnalyzer analyzer;
        PascalToCSharp csharp;
        PascalInterpreter interpreter;
        ConsoleModel con;
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        public Node Tree { get; set; }

        //private string _pascalCode;

        //public string PascalCode
        //{
        //    get => _pascalCode;
        //    set
        //    {
        //        _pascalCode = value;
        //        Compile();
        //    }
        //}

        public Dictionary<string, string> Examples { get; set; } = new Dictionary<string, string>();

        public string CSharpCode { get; set; }
        public List<string> ErrorMessage { get; set; } = new List<string>();
        public string Output { get; set; }
        public string Build { get; set; }
        public void Compile(string PascalCode)
        {
            Build = "";
            con.Clear();
            ErrorMessage.Clear();

            var tokens = lexer.TokenizeResult(PascalCode);

            ErrorMessage.AddRange(tokens.Errors.Select(p => p.Message));

            var tree = ast.EvaluateResult(tokens.Result);
            Tree = tree.Result;
            ErrorMessage.AddRange(tree.Errors.Select(p => p.Message));
            var symbols = analyzer.CheckSyntaxResult(tree.Result);

            ErrorMessage.AddRange(symbols.Errors.Select(p => p.Message));

            if (ErrorMessage.Any())
            {
                StateHasChanged();
                return;
            }
            CSharpCode = csharp.VisitNode(tree.Result);
            interpreter.Interpret(tree.Result);

            Output = con.Output;

            this.StateHasChanged();
        }

        [JSInvokable]
        public async Task EditorChanged(string code)
        {
            //PascalCode = code;
            Compile(code);
        }
        DotNetObjectReference<IndexBase> Reference;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                Reference = DotNetObjectReference.Create(this);
                await JsRuntime.InvokeVoidAsync("SetupEditor", Reference);
                await SelectedExampleChanged(new ChangeEventArgs() { Value = Examples.First().Key });
            }
        }

        public void Log(object obj)
        {
            Build += obj.ToString() + Environment.NewLine;
        }
        public async Task SelectedExampleChanged(ChangeEventArgs args)
        {
            var value = Examples[args.Value.ToString()];

            await JsRuntime.InvokeVoidAsync("setEditorValue", value);
            Compile(value);
        }
        protected override async Task OnInitializedAsync()
        {
            lexer = new PascalLexer(this);
            ast = new PascalAst(this);
            analyzer = new PascalSemanticAnalyzer(this);
            csharp = new PascalToCSharp();
            con = new ConsoleModel();
            interpreter = new PascalInterpreter(console: con);
            Examples.Add("Hello World", @"program hello;
begin
    writeln('hello world!');
end.");

            Examples.Add("Simple Program", @"program test;
begin
end.");
            Examples.Add("Math", @"program math;
var a,b,c : integer;
begin
    a := 10;
    b := 20;
    c := 30;
    writeln(a + c - b);
end.");
            //Compile();
            base.OnInitialized();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Reference?.Dispose();
        }
    }
}
