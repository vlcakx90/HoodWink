using HoodWink.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HoodWink.Languages.Cpp.Compilers
{
    public class Exe : Models.Base.Compiler
    {
        public override string Description => "clang-cl & lld-link";
        public override string CompilerPath => @""; // Not Used

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
            
            // Get Path
            int index = sourcePath.LastIndexOf('\\');            
            string folderPath = sourcePath.Substring(0, index + 1) + "\\";
            string targetSrc = sourcePath;
            string targetObj = sourcePath.Replace(".cpp", ".obj");
            string targetLib = sourcePath.Replace(".cpp", ".lib");
            string targetExe = sourcePath.Replace(".cpp", ".exe");

            // Compile
            if (CompileCode(folderPath, targetSrc, targetObj, targetLib, targetExe))
            {
                compiledPath = targetExe;
            }
            else
            {
                WriteService.Error("Compiler Exited with Errors");
            }

            return compiledPath;
        }

        private static bool CompileCode(string folderPath, string targetSrc, string targetObj, string targetLib, string targetExe)
        {

            //// COMPILE
            // Powershell is used because it wasnt working without it (will fix later)            
            // Example cmd:
            // & 'C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Tools\Llvm\x64\bin\clang-cl.exe' /I C:\Users\mperi\source\repos\HoodWinkSources\Cpp\Cpp\cryptopp-headers\ /c /Z7 /nologo /W3 /WX- /diagnostics:column /O2 /Oi /D NDEBUG /D _CONSOLE /D _UNICODE /D UNICODE /EHsc /MT /GS /Gy /fp:precise /Fo".\" /Gd /TP -m64  CryptoCppTest.cpp
            string filename = "powershell.exe";
            // Compiler
            //string arg0 = @"& 'C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Tools\Llvm\x64\bin\clang-cl.exe'";
            string arg0 = @"& '" + Services.PathService.CLANG_CL_PATH + @"'";
            // Include libs
            string arg1 = @" /I " +  Services.PathService.CRYPTO_HEADERS_PATH;
            // FLags
            string arg2 = @" /c /Z7 /nologo /W3 /WX- /diagnostics:column /O2 /Oi /D NDEBUG /D _CONSOLE /D _UNICODE /D UNICODE /EHsc /MT /GS /Gy /fp:precise /Fo""" + folderPath + @""" /Gd /TP -m64";
            // Target
            string arg3 = " " + targetSrc;
            // Concat
            string arguments = arg0 + arg1 + arg2 + arg3;
            // Run
            if (!RunCommand(filename, arguments))
            {
                return false;
            }


            //// LINK            
            // Example cmd:
            // & 'C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Tools\Llvm\x64\bin\lld-link.exe' /OUT:"C:\Users\mperi\Downloads\TEST\CryptoCppTest.exe" C:\Users\mperi\source\repos\HoodWinkSources\Cpp\cryptopplib\cryptlib.lib /LIBPATH:"C:\Program Files (x86)\Windows Kits\10\Lib\10.0.22621.0\um\x64\" kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /MACHINE:X64 /SUBSYSTEM:CONSOLE /OPT:REF /OPT:ICF /DYNAMICBASE /NXCOMPAT /IMPLIB:"C:\Users\mperi\Downloads\TEST\CryptoCppTest.lib"  C:\Users\mperi\Downloads\TEST\CryptoCppTest.obj
            filename = Services.PathService.LLD_LINK_PATH;            
            // Out
            arg1 = @" /OUT:""" + targetExe + @"""";
            // Libs
            arg2 = " " + Services.PathService.CRYPTOPP_LIB_PATH + @" /LIBPATH:""" + Services.PathService.WINDOWS_LIB_PATH + @""" kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib";
            // Flags
            arg3 = @" /MACHINE:X64 /SUBSYSTEM:CONSOLE /OPT:REF /OPT:ICF /DYNAMICBASE /NXCOMPAT /IMPLIB:""" + targetLib + @"""";
            // Target
            string arg4 = " " + targetObj;
            arguments = arg1 + arg2 + arg3 + arg4;
            // Run
            if (!RunCommand(filename, arguments))
            {
                return false;
            }

            return true;
        }

        private static bool RunCommand(string filename, string args)
        {
            bool status = true;
            string stdout = "";
            string stderr = "";

            try
            {
                Process proc = new Process();
                proc.StartInfo = new ProcessStartInfo();
                proc.StartInfo.FileName = filename;
                proc.StartInfo.Arguments = args;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.Start();

                stdout = proc.StandardOutput.ReadToEnd();
                stderr = proc.StandardError.ReadToEnd();
                proc.WaitForExit();

                if (stdout != "")
                {
                    WriteService.Error("From stdout: " + stdout);
                }
                if (stderr != "")
                {
                    WriteService.Error("From stderr: " + stderr);
                }
                else
                {
                    //WriteService.Success("Command did not return any errors :)");
                }
            }
            catch (Exception ex)
            {
                WriteService.Error(ex.ToString());

                status = false;
            }

            return status;
        }
    }
}