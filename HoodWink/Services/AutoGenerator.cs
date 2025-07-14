using HoodWink.Models.Base;
using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace HoodWink.Services
{
    public static class AutoGenerator
    {
        // Generate payload for each technique for given file, lang, prot
        public static void AllLanguageTechniques(string file, string lang, string form, string extr, string prot) 
        {
            WriteService.Header($"All Techniques for file: {file} lang: {lang} form: {form} extr: {extr} prot: {prot}");
            // Timer
            Stopwatch watch = Stopwatch.StartNew();

            // Get types
            List<Type> langTypes;
            WinkService.LoadLanguage(lang, out langTypes);

            // for each technique
            List<string> techniqueTypes = new List<string>();
            foreach (var type in langTypes)
            {
                int index = type.FullName.LastIndexOf(".") + 1;
                string name = type.FullName.Substring(index, type.FullName.Length - index);

                if (type.FullName.Contains(Utils.Enums.MODULES.Techniques.ToString()))
                {
                    techniqueTypes.Add(name);
                }
            }

            foreach (var tech in techniqueTypes)
            {                
                WinkService.Build(file, lang, form, extr, prot, tech);
            }

            // Elapsed Time
            watch.Stop();
            WriteService.Info($"Time Elapsed: {watch.ElapsedMilliseconds}ms");
        }

        // // Generate payload for each technique for given file,lang, prot for every language
        public static void EveryLanuagesTechniques(string file)
        {
            WriteService.Header($"Everything for file: {file}");
            Stopwatch watch = Stopwatch.StartNew();

            // Get Lanuguages
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
                WriteService.Info($"Starting: {language}");

                // Get types
                List<Type> langTypes = new List<Type>();
                WinkService.LoadLanguage(language, out langTypes);

                List<string> formatTypes = new List<string>();
                List<string> techniqueTypes = new List<string>();
                List<string> protectionTypes = new List<string>();
                List<string> extraTypes = new List<string>();
                
                // Get all modules
                WinkService.LoadLanguage(language, out langTypes);
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
                }

                // Permutation (all combinations)
                foreach (string format in formatTypes) 
                {
                    foreach (string extra in extraTypes)
                    {
                        foreach (string protect  in protectionTypes)
                        {
                            foreach (string tech in techniqueTypes)
                            {
                                //WriteService.Progress($"file: {file} lang: {language} form: {format} extr: {extra} prot: {protect} tech: {tech}");
                                WinkService.Build(file, language, format, extra, protect, tech);
                            }
                        }
                    }
                }
            }

            // Elapsed Time
            watch.Stop();
            WriteService.Info($"Time Elapsed: {watch.ElapsedMilliseconds}ms");
        }
    }
}
