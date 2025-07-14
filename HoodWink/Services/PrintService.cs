using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Reflection;


namespace HoodWink.Services
{
    public static class PrintService
    {        
        // Print Langs
        public static void PrintLanguages()
        {
            List<string> languages = new List<string>();

            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type a in assembly.GetTypes())
            {
                int indexLang = a.FullName.IndexOf("Languages");
                if (indexLang != -1)
                {
                    int indexDot = a.FullName.IndexOf(".", indexLang) + 1;
                    int indexDot2 = a.FullName.IndexOf('.', indexDot);
                    string langName = a.FullName.Substring(indexDot, indexDot2 - indexDot);

                    if (!languages.Contains(langName))
                    {
                        languages.Add(langName);
                    }
                }
            }

            WriteService.Header("Languages");
            foreach (string language in languages)
            {
                WriteService.Info("          -" + language);
            }
        }

        // Print Lang Modules
        public static void PrintLanguageModules(string lang)
        {
            // Lists for each Module Type
            List<Type> langTypes = new List<Type>();
            List<string> formatTypes = new List<string>();
            List<string> techniqueTypes = new List<string>();
            List<string> protectionTypes = new List<string>();
            List<string> extraTypes = new List<string>();
            List<string> generatorTypes = new List<string>();
            List<string> compilerTypes = new List<string>();

            // Load
            WinkService.LoadLanguage(lang, out langTypes);
            foreach (var type in langTypes)
            {
                int index = type.FullName.LastIndexOf(".") + 1;
                string name = type.FullName.Substring(index, type.FullName.Length - index);

                //Console.WriteLine(type.FullName);
                if (type.FullName.Contains(Utils.Enums.MODULES.Formats.ToString()))
                {
                    formatTypes.Add(name);
                }
                else if (type.FullName.Contains(Utils.Enums.MODULES.Techniques.ToString()))
                {
                    techniqueTypes.Add(name);
                }
                else if (type.FullName.Contains(Utils.Enums.MODULES.Protections.ToString()))
                {
                    protectionTypes.Add(name);
                }
                else if (type.FullName.Contains(Utils.Enums.MODULES.Extras.ToString()))
                {
                    extraTypes.Add(name);
                }
                else if (type.FullName.Contains(Utils.Enums.MODULES.Generators.ToString()))
                {
                    generatorTypes.Add(name);
                }
                else if (type.FullName.Contains(Utils.Enums.MODULES.Compilers.ToString()))
                {
                    compilerTypes.Add(name);
                }
            }

            // Display
            WriteService.Header("[+] Modules for: ", lang);
            WriteService.Header("Formats:");
            foreach (var mod in formatTypes) { PrintModule(mod); }
            WriteService.Header("Extras:");
            foreach (var mod in extraTypes) { PrintModule(mod); }
            WriteService.Header("Protections:");
            foreach (var mod in protectionTypes) { PrintModule(mod); }
            WriteService.Header("Techniques:");
            foreach (var mod in techniqueTypes) { PrintModule(mod); }
            WriteService.Header("Generators:");
            foreach (var mod in generatorTypes) { PrintModule(mod); }
            WriteService.Header("Compilers:");
            foreach (var mod in compilerTypes) { PrintModule(mod); }
        }

        // Print All Modules   
        public static void PrintAllModules()
        {
            List<string> languages = new List<string>();

            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type a in assembly.GetTypes())
            {
                int indexLang = a.FullName.IndexOf("Languages");
                if (indexLang != -1)
                {
                    int indexDot = a.FullName.IndexOf(".", indexLang) + 1;
                    int indexDot2 = a.FullName.IndexOf('.', indexDot);
                    string langName = a.FullName.Substring(indexDot, indexDot2 - indexDot);

                    if (!languages.Contains(langName))
                    {
                        languages.Add(langName);
                    }
                }
            }

            foreach (string language in languages)
            {
                PrintLanguageModules(language);
            }
        }

        // Print Lang Modules with Descriptions
        public static void PrintLanguageModulesDescriptions(string lang)
        {
            // Lists for each Module Type
            List<Type> langTypes = new List<Type>();
            //List<string> formatTypes = new List<string>();
            //List<string> techniqueTypes = new List<string>();
            //List<string> protectionTypes = new List<string>();
            //List<string> extraTypes = new List<string>();
            //List<string> generatorTypes = new List<string>();
            //List<string> compilerTypes = new List<string>();

            // NEW
            List<(string, string)> formatTypes = new List<(string, string)>();
            List<(string, string)> techniqueTypes = new List<(string, string)>();
            List<(string, string)> protectionTypes = new List<(string, string)>();
            List<(string, string)> extraTypes = new List<(string, string)>();
            List<(string, string)> generatorTypes = new List<(string, string)>();
            List<(string, string)> compilerTypes = new List<(string, string)>();

            string spacer = " : ";

            // Load
            WinkService.LoadLanguage(lang, out langTypes);
            foreach (var type in langTypes)
            {
                int index = type.FullName.LastIndexOf(".") + 1;
                string name = type.FullName.Substring(index, type.FullName.Length - index);

                //Console.WriteLine(type.FullName);
                if (type.FullName.Contains(Utils.Enums.MODULES.Formats.ToString()))
                {
                    Models.Base.FormatExe inst = null;
                    try
                    {
                        inst = (Models.Base.FormatExe)Activator.CreateInstance(type);
                    }
                    catch (Exception ex)
                    {
                        WriteService.ErrorExit($"Failed to create instance for: {type.FullName}");
                    }

                    formatTypes.Add( (name, inst.Description) );
                }
                else if (type.FullName.Contains(Utils.Enums.MODULES.Techniques.ToString()))
                {
                    Models.Base.Technique inst = null;
                    try
                    {
                        inst = (Models.Base.Technique)Activator.CreateInstance(type);
                    }
                    catch (Exception ex)
                    {
                        WriteService.ErrorExit($"Failed to create instance for: {type.FullName}");
                    }

                    techniqueTypes.Add( (name, inst.Description) );
                }
                else if (type.FullName.Contains(Utils.Enums.MODULES.Protections.ToString()))
                {
                    Models.Base.Protections inst = null;
                    try
                    {
                        inst = (Models.Base.Protections)Activator.CreateInstance(type);
                    }
                    catch (Exception ex)
                    {
                        WriteService.ErrorExit($"Failed to create instance for: {type.FullName}");
                    }

                    protectionTypes.Add( (name, inst.Description) );
                }
                else if (type.FullName.Contains(Utils.Enums.MODULES.Extras.ToString()))
                {
                    Models.Base.Extras inst = null;
                    try
                    {
                        inst = (Models.Base.Extras)Activator.CreateInstance(type);
                    }
                    catch (Exception ex)
                    {
                        WriteService.ErrorExit($"Failed to create instance for: {type.FullName}");
                    }

                    extraTypes.Add( (name, inst.Description) );
                }
                else if (type.FullName.Contains(Utils.Enums.MODULES.Generators.ToString()))
                {
                    Models.Base.Generator inst = null;
                    try
                    {
                        inst = (Models.Base.Generator)Activator.CreateInstance(type);
                    }
                    catch (Exception ex)
                    {
                        WriteService.ErrorExit($"Failed to create instance for: {type.FullName}");
                    }

                    generatorTypes.Add( (name, inst.Description) );
                }
                else if (type.FullName.Contains(Utils.Enums.MODULES.Compilers.ToString()))
                {
                    Models.Base.Compiler inst = null;
                    try
                    {
                        inst = (Models.Base.Compiler)Activator.CreateInstance(type);
                    }
                    catch (Exception ex)
                    {
                        WriteService.ErrorExit($"Failed to create instance for: {type.FullName}");
                    }

                    compilerTypes.Add( (name, inst.Description) );
                }
            }

            // Display
            WriteService.Header("[+] Modules for: ", lang);
            WriteService.Header("Formats:");
            foreach (var mod in formatTypes) { PrintModuleDescription(mod.Item1, mod.Item2); }
            WriteService.Header("Extras:");
            foreach (var mod in extraTypes) { PrintModuleDescription(mod.Item1, mod.Item2); }
            WriteService.Header("Protections:");
            foreach (var mod in protectionTypes) { PrintModuleDescription(mod.Item1, mod.Item2); }
            WriteService.Header("Techniques:");
            foreach (var mod in techniqueTypes) { PrintModuleDescription(mod.Item1, mod.Item2); }
            WriteService.Header("Generators:");
            foreach (var mod in generatorTypes) { PrintModuleDescription(mod.Item1, mod.Item2); }
            WriteService.Header("Compilers:");
            foreach (var mod in compilerTypes) { PrintModuleDescription(mod.Item1, mod.Item2); }
        }

        // Print All Moduls with Descriptions
        public static void PrintAllModulesDescriptions()
        {
            List<string> languages = new List<string>();

            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type a in assembly.GetTypes())
            {
                int indexLang = a.FullName.IndexOf("Languages");
                if (indexLang != -1)
                {
                    int indexDot = a.FullName.IndexOf(".", indexLang) + 1;
                    int indexDot2 = a.FullName.IndexOf('.', indexDot);
                    string langName = a.FullName.Substring(indexDot, indexDot2 - indexDot);

                    if (!languages.Contains(langName))
                    {
                        languages.Add(langName);
                    }
                }
            }

            foreach (string language in languages)
            {
                PrintLanguageModulesDescriptions(language);
            }
        }

        // Print Module
        private static void PrintModule(string module)
        {
            WriteService.Description(module);
        }

        // Print Module Description
        private static void PrintModuleDescription(string name, string desc)
        {
            WriteService.Description(name, desc);
        }

    }
}
