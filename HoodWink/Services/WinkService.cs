using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HoodWink.Services
{
    public static class WinkService
    {
        public static void Build(string file, string lang, string format, string extra, string protection, string technique)
        {
            WriteService.Progress($"file: {file} lang: {lang} form: {format} extr: {extra} prot: {protection} tech: {technique}");

            // Types
            List<Type> langTypes;
            Type formatType;
            Type techniqueType;
            Type protectionType;
            Type extraTypes;        // Will be List later
            Type generatorType;
            Type compilerType;
            List<string> dependencies;

            // Instances
            Models.Base.FormatExe formatExeInstance;
            Models.Base.Technique techniqueInstance;
            Models.Base.Protections protectionInstance;
            Models.Base.Extras extraInstance;
            Models.Base.Generator generatorInstance;
            Models.Base.Compiler compilerInstance;

            // Verify file exists
            if (!File.Exists(file))
            {
                WriteService.ErrorExit($"File: {file} NOT found!");
            }

            // Load all Types for lang
            LoadLanguage(lang, out langTypes);

            // Load specified Types
            LoadTypes(ref langTypes, ref format, out formatType, ref technique, out techniqueType, ref protection, out protectionType, ref extra, out extraTypes, out generatorType, out compilerType);

            // Create extraTypes
            CreateInctances(ref formatType, out formatExeInstance, ref techniqueType, out techniqueInstance, ref protectionType, out protectionInstance, ref extraTypes, out extraInstance, ref generatorType, out generatorInstance, ref compilerType, out compilerInstance);

            // Load Dependencies
            LoadDependencies(out dependencies, ref formatExeInstance, ref techniqueInstance, ref protectionInstance, ref extraInstance);

            // Make Payload Dir
            string curDir = Directory.GetCurrentDirectory();
            string payloadDir = curDir + @"\Payloads\";
            if (!Directory.Exists(payloadDir))
            {
                Directory.CreateDirectory(payloadDir);
            }

            // Make target             
            string guid = Guid.NewGuid().ToString();
            string filename = lang + "_" + extra + "_" + protection + "_" + technique + "-" + guid.Substring(0, 8);
            if (lang == Utils.Enums.LANGUAGES.Csharp.ToString())
            {
                filename += ".cs";
            }
            else if (lang == Utils.Enums.LANGUAGES.Cpp.ToString())
            {
                filename += ".cpp";
            }
            string targetSourcePath = payloadDir + filename;

            // Generate
            string generatedFilePath = generatorInstance.Gen(ref targetSourcePath, ref file, ref formatExeInstance, ref techniqueInstance, ref protection, ref protectionInstance, ref extraInstance);
            if (generatedFilePath is null)
            {
                WriteService.ErrorExit("Generator exited with errors");
            }
            else
            {
                WriteService.Success($"Generated: {generatedFilePath}");
            }

            // Compile
            string compiledPath = compilerInstance.Compile(generatedFilePath, dependencies);
            if (compiledPath is null)
            {
                WriteService.ErrorExit("Compiler exited with errors");
            }
            else
            {
                WriteService.Success($"Compiled: {compiledPath}");
            }
        }

        public static void LoadLanguage(string lang, out List<Type> langTypes)
        {
            langTypes = new List<Type>();

            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type a in assembly.GetTypes())
            {
                int indexLang = a.FullName.IndexOf("Languages");
                if (indexLang != -1)
                {
                    int indexDot = a.FullName.IndexOf(".", indexLang) + 1;
                    int indexDot2 = a.FullName.IndexOf('.', indexDot);
                    string langName = a.FullName.Substring(indexDot, indexDot2 - indexDot);

                    if (langName == lang)
                    {
                        langTypes.Add(a);
                    }
                }
            }

            // Check if succesful
            if (langTypes.Count == 0)
            {
                WriteService.ErrorExit($"LoadLanguage did not find any Types for language: {lang}");
            }
            else // DEBUG
            {
                //WriteService.Success("LoadLanguage");
            }
        }

        private static void LoadTypes(ref List<Type> langTypes, ref string format, out Type formatType, ref string technique, out Type techniqueType, ref string protection, out Type protectionType, ref string extras, out Type extrasType, out Type generatorType, out Type compilerType)
        {
            formatType = null;
            techniqueType = null;
            protectionType = null;
            extrasType = null;
            generatorType = null;
            compilerType = null;

            // Load Types
            foreach (Type a in langTypes)
            {
                int index = a.FullName.LastIndexOf(".") + 1;
                string name = a.FullName.Substring(index, a.FullName.Length - index);

                if (a.FullName.Contains(Utils.Enums.MODULES.Formats.ToString()) && name == format)
                {
                    formatType = a;
                }
                else if (a.FullName.Contains(Utils.Enums.MODULES.Techniques.ToString()) && name == technique)
                {
                    techniqueType = a;
                }
                else if (a.FullName.Contains(Utils.Enums.MODULES.Protections.ToString()) && name == protection)
                {
                    protectionType = a;
                }
                else if (a.FullName.Contains(Utils.Enums.MODULES.Extras.ToString()) && name == extras)
                {
                    extrasType = a;
                }
                else if (a.FullName.Contains(Utils.Enums.MODULES.Generators.ToString()) && name == format) // name to match format
                {
                    generatorType = a;
                }
                else if (a.FullName.Contains(Utils.Enums.MODULES.Compilers.ToString()) && name == format) // name to match format
                {
                    compilerType = a;
                }
            }

            // Check if succesful
            int errors = 0;
            if (formatType is null)
            {
                errors++;
                WriteService.Error($"Failed to Load Type: {format}");
            }
            if (techniqueType is null)
            {
                errors++;
                WriteService.Error($"Failed to Load Type: {technique}");
            }
            if (protectionType is null)
            {
                errors++;
                WriteService.Error($"Failed to Load Type: {protection}");
            }
            if (extrasType is null)
            {
                errors++;
                WriteService.Error($"Failed to Load Type: {extras}");
            }
            if (generatorType is null)
            {
                errors++;
                WriteService.Error($"Failed to Load Generator for Format: {format}");
            }
            if (compilerType is null)
            {
                errors++;
                WriteService.Error($"Failed to Load Compiler for Format: {format}");
            }

            // Exit on errors
            if (errors > 0)
            {
                WriteService.ErrorExit($"LoadTypes encountered ({errors}) errors");
            }
            else // DEBUG
            {
                //WriteService.Success("LoadTypes");
            }
        }

        private static void CreateInctances(ref Type formatType, out Models.Base.FormatExe formatExeInstance, ref Type techniqueType, out Models.Base.Technique techniqueInstance, ref Type protectionType, out Models.Base.Protections protectionInstance, ref Type extraType, out Models.Base.Extras extraInstance, ref Type generatorType, out Models.Base.Generator generatorInstance, ref Type compilerType, out Models.Base.Compiler compilerInstance) // Will change base format
        {
            formatExeInstance = null;
            techniqueInstance = null;
            protectionInstance = null;
            extraInstance = null;
            generatorInstance = null;
            compilerInstance = null;

            // Format
            try
            {
                formatExeInstance = (Models.Base.FormatExe)Activator.CreateInstance(formatType);
            }
            catch (Exception ex)
            {
                WriteService.ErrorExit($"Failed to create instance for: {formatType.FullName}");
            }
            // Technique
            try
            {
                techniqueInstance = (Models.Base.Technique)Activator.CreateInstance(techniqueType);
            }
            catch (Exception ex)
            {
                WriteService.ErrorExit($"Failed to create instance for: {techniqueType.FullName}");
            }
            // Protection
            try
            {
                protectionInstance = (Models.Base.Protections)Activator.CreateInstance(protectionType);
            }
            catch (Exception ex)
            {
                WriteService.ErrorExit($"Failed to create instance for: {protectionType.FullName}");
            }
            // Extra
            try
            {
                extraInstance = (Models.Base.Extras)Activator.CreateInstance(extraType);
            }
            catch (Exception ex)
            {
                WriteService.ErrorExit($"Failed to create instance for: {extraType.FullName}");
            }
            // Generator
            try
            {
                generatorInstance = (Models.Base.Generator)Activator.CreateInstance(generatorType);
            }
            catch (Exception ex)
            {
                WriteService.ErrorExit($"Failed to create instance for: {generatorType.FullName}");
            }
            // Compiler
            try
            {
                compilerInstance = (Models.Base.Compiler)Activator.CreateInstance(compilerType);
            }
            catch (Exception ex)
            {
                WriteService.ErrorExit($"Failed to create instance for: {compilerType.FullName}");
            }

            // Check if successful (Should not be necessary but just in case)
            if (formatExeInstance is null || techniqueInstance is null || protectionInstance is null || extraInstance is null || generatorInstance is null || compilerInstance is null)
            {
                WriteService.ErrorExit("CreateInstance for generator and compiler encountered errors");
            }
            else // DEBUG
            {
                //WriteService.Success("CreateInstances");
            }
        }

        private static void LoadDependencies(out List<string> dependencies, ref Models.Base.FormatExe formatInstance, ref Models.Base.Technique techniqueInstance, ref Models.Base.Protections protectionInstance, ref Models.Base.Extras extraInstance)
        {
            dependencies = new List<string>();

            try
            {

                // Format
                foreach (var file in formatInstance.FileDependencies)
                {
                    if (file != "")
                    {
                        dependencies.Add(PathService.PROJECT_PATH + file);
                    }
                }
                // Technique
                foreach (var file in techniqueInstance.FileDependencies)
                {
                    if (file != "")
                    {
                        dependencies.Add(PathService.PROJECT_PATH + file);
                    }
                }
                // Protection
                foreach (var file in protectionInstance.FileDependencies)
                {
                    if (file != "")
                    {
                        dependencies.Add(PathService.PROJECT_PATH + file);
                    }
                }
                // Extra
                foreach (var file in extraInstance.FileDependencies)
                {
                    if (file != "")
                    {
                        dependencies.Add(PathService.PROJECT_PATH + file);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteService.ErrorExit("LoadDependencies failed, modules not implemented?");
            }

            // Skipping check if dependencies.Count == 0 since it might not have any

            // Verify Files
            int errors = 0;
            foreach (var path in dependencies)
            {
                if (!File.Exists(path))
                {
                    errors++;
                    WriteService.Error($"File Dependency NOT found: {path}");
                }
            }

            // Exit on errors
            if (errors > 0)
            {
                WriteService.ErrorExit($"LoadDependencies encountered {errors}");
            }
            else // DEBUG
            {
                //WriteService.Success("LoadDependencies");
            }
        }
    }
}