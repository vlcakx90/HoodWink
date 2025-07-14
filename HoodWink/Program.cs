using HoodWink.Services;
using System;

namespace HoodWink
{
    internal class Program
    {
        static void Main(string[] args) // ./program.exe  -lang l -file f
        {
            if (args.Length == 0)
            {
                Usage();
                System.Environment.Exit(1);
            }

            string file = null;
            string lang = null;
            string form = null;
            string tech = null;
            string prot = null;
            string extr = null;  // Will be List later
            bool genEvery = false;

            // Parse Args
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-file" && args.Length >= i + 2)
                {
                    file = args[i + 1];
                }
                else if (args[i] == "-lang" && args.Length >= i + 2)
                {
                    lang = args[i + 1];
                }
                else if (args[i] == "-form" && args.Length >= i + 2)
                {
                    form = args[i + 1];
                }
                else if (args[i] == "-tech" && args.Length >= i + 2)
                {
                    tech = args[i + 1];
                }
                else if (args[i] == "-prot" && args.Length >= i + 2)
                {
                    prot = args[i + 1];
                }
                else if (args[i] == "-extr" && args.Length >= i + 2)
                {
                    extr = args[i + 1];
                }
                else if (args[i] == "-langs")
                {
                    PrintService.PrintLanguages();
                    System.Environment.Exit(0);
                }
                else if (args[i] == "-help")
                {
                    HelpFlags();
                    System.Environment.Exit(0);
                }
                else if (args[i] == "-showall")
                {
                    PrintService.PrintAllModules();
                    System.Environment.Exit(0);
                }
                else if (args[i] == "-show" && args.Length >= i + 2)
                {
                    Console.WriteLine($"args.length : {args.Length}");
                    PrintService.PrintLanguageModules(args[i + 1]);
                    System.Environment.Exit(0);
                }
                else if (args[i] == "-descall")
                {
                    PrintService.PrintAllModulesDescriptions();
                    System.Environment.Exit(0);
                }
                else if (args[i] == "-desc" && args.Length >= i + 2)
                {
                    PrintService.PrintLanguageModulesDescriptions(args[i + 1]);
                    System.Environment.Exit(0);
                }                
                else if (args[i] == "-genEvery")
                {
                    genEvery = true;
                }
            }


            if (tech == "All") // Generate All Techniques
            {
                if (file != null && lang != null && form != null && extr != null)
                {
                    AutoGenerator.AllLanguageTechniques(file, lang, form, extr, prot);
                }
                else
                {
                    WriteService.ErrorExit("-genAll requires: file, lang, form, extr, prot");
                }
            }
            else if (genEvery) // Generate Every
            {
                if (file != null)
                {
                    AutoGenerator.EveryLanuagesTechniques(file);
                }
                else
                {
                    WriteService.ErrorExit("-genEvery requires: file, lang, form, extr, prot");
                }
            }
            else if (file != null && lang != null && form != null && tech != null && prot != null && extr != null) // Build Single
            {
                WinkService.Build(file, lang, form, extr, prot, tech);
            }
            else
            {
                WriteService.Error("Args Error");
                Usage();
                System.Environment.Exit(0);
            }
        }

        // Usage
        private static void Usage()
        {
            WriteService.Header("Helper Flags: ");
            WriteService.Info(@"    .\HoodWink.exe  -help         :  Show help menu for Flags");
            WriteService.Info(@"    .\HoodWink.exe  -langs        :  Show all Languages");
            WriteService.Info(@"    .\HoodWink.exe  -showall      :  Show all Modules");
            WriteService.Info(@"    .\HoodWink.exe  -show <lang>  :  Show Modules for lang");
            WriteService.Info(@"    .\HoodWink.exe  -descall      :  Show all Modules + Descriptions");
            WriteService.Info(@"    .\HoodWink.exe  -desc <lang>  :  Show Modules + Descriptions for lang");
            Console.WriteLine();
            WriteService.Header("Syntax: Build Single");
            WriteService.Info(@"    .\HoodWink.exe -file <name> -lang <name> -form <name> -extr <name> -prot <name> -tech <name> ");
            Console.WriteLine();
            WriteService.Header("Syntax: Build all Techniques");
            WriteService.Info(@"    .\HoodWink.exe -file <name> -lang <name> -form <name> -extr <name> -prot <name> -tech All ");
            Console.WriteLine();
            WriteService.Header("Syntax: Build Everything");
            WriteService.Info(@"    .\HoodWink.exe -file <name> -genEvery");
            Console.WriteLine();
            WriteService.Header("Example: ");
            WriteService.Info(@"    .\HoodWink.exe -file C:\Payloads\msf.bin -lang Csharp -form Exe -extr AmsiBypass -prot Aes256 -tech Spawn_QueueApc");
        }

        // Help
        private static void HelpFlags()
        {
            WriteService.Header("Help: ", "flags");
            WriteService.Info(@"    -file   :  file containing payload (shellcode)");
            WriteService.Info(@"    -lang   :  wich languages to use");
            WriteService.Info(@"    -form   :  which file format to use");
            WriteService.Info(@"    -extr   :  extra technique to add");
            WriteService.Info(@"    -prot   :  protection type (ex. encryption)");
            WriteService.Info(@"    -tech   :  technique to use (ex. injection)");
        }
    }
}