using HoodWink.Services;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;


namespace HoodWink.Languages.Csharp.Compilers
{
    public class Exe : Models.Base.Compiler
    {
        public override string Description => "Nuget Package > Microsoft.CodeDom.Providers.DotNetCompilerPlatform";
        public override string CompilerPath => throw new NotImplementedException(); // Using Microsoft.CodeDom.Providers.DotNetCompilerPlatform instead

        public override string Compile(string sourcePath, List<string> dependencies)
        {
            string compiledPath = null;
            string[] sourceFiles = new string[dependencies.Count + 1];
            sourceFiles[0] = sourcePath;

            // Set source and dependencies
            for (int i = 1; i < sourceFiles.Length; i++)
            {
                sourceFiles[i] = dependencies[i - 1];
            }

            string targetPath = sourcePath.Replace(".cs", ".exe");

            // Compiler
            CSharpCodeProvider provider = new CSharpCodeProvider();

            // Compile
            if (CompileCode(provider, sourceFiles, targetPath))
            {
                compiledPath = targetPath;
            }
            else
            {
                WriteService.Error("Compiler Exited with Errors");
            }

            return compiledPath;
        }

        // Based off of: https://learn.microsoft.com/en-us/dotnet/api/system.codedom.compiler.compilerparameters.generateinmemory?view=dotnet-plat-ext-7.0&redirectedfrom=MSDN#System_CodeDom_Compiler_CompilerParameters_GenerateInMemory
        private static bool CompileCode(CodeDomProvider provider, String[] sourceFile, String exeFile)
        {

            CompilerParameters cp = new CompilerParameters();

            // Generate an executable instead of
            // a class library.
            cp.GenerateExecutable = true;

            // Set the assembly file name to generate.
            cp.OutputAssembly = exeFile;

            // Generate debug information.
            cp.IncludeDebugInformation = true;

            // Add an assembly reference.
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("mscorlib.dll"); // added

            // Save the assembly as a physical file.
            cp.GenerateInMemory = false;

            // Set the level at which the compiler
            // should start displaying warnings.
            cp.WarningLevel = 3;

            // Set whether to treat all warnings as errors.
            cp.TreatWarningsAsErrors = false;

            // Set compiler argument to optimize output.
            cp.CompilerOptions = "/optimize";

            // Set a temporary files collection.
            // The TempFileCollection stores the temporary files
            // generated during a build in the current directory,
            // and does not delete them after compilation.
            cp.TempFiles = new TempFileCollection(".", false);  // false so temp files are deleted

            if (provider.Supports(GeneratorSupport.EntryPointMethod))
            {
                // Specify the class that contains
                // the main method of the executable.
                cp.MainClass = "Wink.Program";                    // All source files need to match this
            }

            //if (Directory.Exists("Resources"))
            //{
            //    if (provider.Supports(GeneratorSupport.Resources))
            //    {
            //        // Set the embedded resource file of the assembly.
            //        // This is useful for culture-neutral resources,
            //        // or default (fallback) resources.
            //        cp.EmbeddedResources.Add("Resources\\Default.resources");

            //        // Set the linked resource reference files of the assembly.
            //        // These resources are included in separate assembly files,
            //        // typically localized for a specific language and culture.
            //        cp.LinkedResources.Add("Resources\\nb-no.resources");
            //    }
            //}

            // Invoke compilation.
            CompilerResults cr = provider.CompileAssemblyFromFile(cp, sourceFile);

            if (cr.Errors.Count > 0)
            {
                // Display compilation errors.
                Console.WriteLine("Errors building {0} into {1}", sourceFile, cr.PathToAssembly);
                foreach (CompilerError ce in cr.Errors)
                {
                    Console.WriteLine("  {0}", ce.ToString());
                    Console.WriteLine();
                }

                return false;
            }
            else
            {
                // No Errors :)
                //Console.WriteLine("Source {0} built into {1} successfully.", sourceFile, cr.PathToAssembly);
                //Console.WriteLine("{0} temporary files created during the compilation.", cp.TempFiles.Count.ToString());
                return true;
            }

            // Return the results of compilation.
            //if (cr.Errors.Count > 0)
            //{
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}
        }
    }
}