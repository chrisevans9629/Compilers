using System;
using System.CodeDom.Compiler;

namespace Minesweeper.Test.Tests
{
    public class CompileCSharp
    {
        public static bool CompileExecutable(string sourceName, string appName)
        {
            //FileInfo sourceFile = new FileInfo(sourceName);
            CodeDomProvider provider = null;
            bool compileOk = false;

            // Select the code provider based on the input file extension.
            //if (sourceFile.Extension.ToUpper(CultureInfo.InvariantCulture) == ".CS")
            //{
            provider = CodeDomProvider.CreateProvider("CSharp");
            //}
            //else if (sourceFile.Extension.ToUpper(CultureInfo.InvariantCulture) == ".VB")
            //{
            //    provider = CodeDomProvider.CreateProvider("VisualBasic");
            //}
            //else
            //{
            //    Console.WriteLine("Source file must have a .cs or .vb extension");
            //}

            if (provider != null)
            {

                // Format the executable file name.
                // Build the output assembly path using the current directory
                // and <source>_cs.exe or <source>_vb.exe.

                String exeName = $@"{System.Environment.CurrentDirectory}\{appName}.exe";

                CompilerParameters cp = new CompilerParameters();

                // Generate an executable instead of 
                // a class library.
                cp.GenerateExecutable = true;

                // Specify the assembly file name to generate.
                cp.OutputAssembly = exeName;

                // Save the assembly as a physical file.
                cp.GenerateInMemory = false;

                // Set whether to treat all warnings as errors.
                cp.TreatWarningsAsErrors = false;

                // Invoke compilation of the source file.

                CompilerResults cr = provider.CompileAssemblyFromSource(cp,
                    sourceName);

                if (cr.Errors.Count > 0)
                {
                    // Display compilation errors.
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
                    // Display a successful compilation message.
                    Console.WriteLine("Source {0} built into {1} successfully.",
                        sourceName, cr.PathToAssembly);
                }

                // Return the results of the compilation.
                if (cr.Errors.Count > 0)
                {
                    compileOk = false;
                }
                else
                {
                    compileOk = true;
                }
            }
            return compileOk;
        }
    }
}