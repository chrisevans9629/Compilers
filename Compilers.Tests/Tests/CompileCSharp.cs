using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;

namespace Minesweeper.Test.Tests
{
    public class CompilerSettings : ICompilerSettings
    {
        public string CompilerFullPath =>
            @"c:\Program Files (x86)\Microsoft Visual Studio\2019\professional\MSBuild\Current\Bin\Roslyn";
        public int CompilerServerTimeToLive { get; }
    }

    public class CompileCSharp
    {
        public static bool CompileExecutable2(string sourceCode, string appName)
        {
            String exeName = $@"{System.Environment.CurrentDirectory}\{appName}.exe";
            var tempFile = Path.GetTempFileName();
            var tempSource = Path.ChangeExtension(tempFile, ".cs");

            File.WriteAllText(tempSource, sourceCode);
            var settings = new CompilerSettings();

            var csc = Path.Combine(settings.CompilerFullPath, "csc.exe");

            var process = new Process( );
            process.StartInfo.Arguments = $"/target:exe /out:{exeName} {tempSource}";
            process.StartInfo.FileName = csc;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            process.WaitForExit();

            var output = process.StandardError.ReadToEnd();
            Console.Write(output);
            if (process.ExitCode != 0)
            {
                return false;
            }

            return true;
        }

        public static bool CompileExecutable(string sourceName, string appName)
        {
            String exeName = $@"{System.Environment.CurrentDirectory}\{appName}.exe";

            CodeDomProvider provider = null;
            bool compileOk = false;

            provider = new CSharpCodeProvider(new CompilerSettings());


            if (provider != null)
            {

                CompilerParameters cp = new CompilerParameters();
                cp.GenerateExecutable = true;
                cp.OutputAssembly = exeName;
                cp.GenerateInMemory = false;
                cp.TreatWarningsAsErrors = false;
                CompilerResults cr = provider.CompileAssemblyFromSource(cp,
                    sourceName);

                if (cr.Errors.Count > 0)
                {
                    Console.WriteLine("Errors building {0} into {1}",
                        sourceName, cr.PathToAssembly);
                    foreach (CompilerError ce in cr.Errors)
                    {
                        Console.WriteLine("  {0}", ce.ToString());
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("Source {0} built into {1} successfully.",
                        sourceName, cr.PathToAssembly);
                }

                if (cr.Errors.Count <= 0)
                {
                    compileOk = true;
                }
            }
            return compileOk;
        }
    }
}