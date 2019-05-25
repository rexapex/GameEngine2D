using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace CSGEngineDLLCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());

            // Find all C# script files in the Scripts directory
            string[] files = Directory.GetFiles("Scripts", "*", SearchOption.AllDirectories);
            
            // Initialize C# compiler and parameters
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters options = new CompilerParameters();
            options.GenerateExecutable = false;
            options.GenerateInMemory = false;
            options.CompilerOptions = "/optimize";
            options.OutputAssembly = "Scripts.dll";

            // Add references required by the scripts
            options.ReferencedAssemblies.Add("System.dll");
            options.ReferencedAssemblies.Add("System.Core.dll");
            options.ReferencedAssemblies.Add("C:\\Users\\James\\source\\repos\\GameEngine2D\\GameEngine2D\\bin\\Debug\\GameEngine2D.exe");

            // Compile all the files into a dll
            CompilerResults results = codeProvider.CompileAssemblyFromFile(options, files);

            // Output errors
            for (int i = 0; i < results.Output.Count; i++)
                Console.WriteLine(results.Output[i]);
            for (int i = 0; i < results.Errors.Count; i++)
                Console.WriteLine(i.ToString() + ": " + results.Errors[i].ToString());

            System.Console.ReadKey();
        }
    }
}
